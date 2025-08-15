using System.Reflection.PortableExecutable;

namespace NetzplanBot
{
    public class NetzplanUngueltigException : Exception
    {
        public NetzplanUngueltigException(string beschreibung, int zeile) 
            : base($"Fehler in der Eingabe, Zeile {zeile}: " + beschreibung) { }

        public NetzplanUngueltigException(string beschreibung)
            : base($"Fehler in der Eingabe: " + beschreibung) { }
    }

    public class Netzplan
    {

        internal Projektabschnitt _start;

        // nicht nötig, da Knoten untereinander durch Verweise verknüpft sind
        // nützlich für zB Testzugriff
        internal Dictionary<string, Projektabschnitt> Knoten;
        
        public Netzplan(string eingabeStr)
        {
            string[] zeilen = eingabeStr.Split('\n');

            foreach (string z in zeilen)
                z.TrimEnd('\r');

            Init(zeilen);
        }

        public Netzplan(StreamReader reader)
        {
            // skip the first line
            reader.ReadLine();

            List<string> zeilen = new();

            while (!reader.EndOfStream)
                zeilen.Add(reader.ReadLine());

            Init(zeilen.ToArray());

        }


        #region Initialisierungslogik
        
        private void Init(string[] datenZeilen)
        {
            int linecounter = 1;

            Knoten = [];

            foreach(string zeile in datenZeilen)
            {
                string[] Data = zeile.Trim().Split(';');

                if (Data.Length != 4)
                    throw new NetzplanUngueltigException("Ungültiges Format", linecounter);

                Projektabschnitt neuerAbschnitt = new();

                neuerAbschnitt.Name = Data[0];
                neuerAbschnitt.Beschreibung = Data[1];
                neuerAbschnitt.Dauer = int.Parse(Data[2]);

                // Vorgänger zufügen:
                // Sonderfall Startknoten
                // Ansonsten: Vorgänger und Nachkommen registrieren

                if (Data[3] == "-" || Data[3] == "")
                {
                    if (this._start != null)
                        throw new NetzplanUngueltigException("Nur ein Startknoten zulässig", linecounter);
                    else
                        this._start = neuerAbschnitt;
                }
                else
                    foreach (string vorgaengarID in Data[3].Split(','))
                    {
                        if (!Knoten.ContainsKey(vorgaengarID))
                            throw new NetzplanUngueltigException("" +
                                "Ungültiger Vorgänger", linecounter);

                        Projektabschnitt vorgaenger = Knoten[vorgaengarID];

                        vorgaenger.Nachfolger.Add(neuerAbschnitt);
                        neuerAbschnitt.Vorgaenger.Add(vorgaenger);
                    }

                Knoten[neuerAbschnitt.Name] = neuerAbschnitt;

                linecounter++;

            }

            // Final Check: Valide Graphstruktur:
            // 1. Eindeutiger Endknoten
            // 2. keine Zyklen

            Projektabschnitt endknoten = null;

            foreach (var (key, knoten) in Knoten)
                if (knoten.IstEndknoten())
                {
                    if (endknoten == null)
                        endknoten = knoten;
                    else
                        throw new NetzplanUngueltigException("Endknoten muss eindeutig sein");
                }

            if (endknoten == null)
                throw new NetzplanUngueltigException("Kein Endknoten gefunden");

            // TODO: Zykelfreiheit prüfen
            // jeden möglichen pfad vom start- zum endknoten

            // Vorwärtsrechnung
            // Bestimmt FAZ + FEZ
            Vorwaertsrechnung(_start);

            //Bestimmt SAZ + SEZ
            Rueckwaertsrechnung(endknoten);

            // Bestimmt Gesamtpuffer und freien Puffer
            PufferBerechnung(_start);
        }

        /*
         * Ein Netzplan ist ein gerichteter Graph, bei dem die einzelnen
         * Projekt_knoten als Knoten und die Abhängigkeiten zwischen 
         * zwei _knotenn als Kanten dargestellt werden.
         */

        // Rekursive Funktion (Knoten -> alle Nachfolger) zur Berechnung von
        // FAZ (Frühester Anfangszeitpunkt) und
        // FEZ (Frühester Endzeitpunkt)
        private void Vorwaertsrechnung(Projektabschnitt knoten)
        {
            if(knoten.IstStartknoten())
            {
                knoten.FAZ = 0;
                knoten.FEZ = knoten.Dauer;
            }
            else
            {
                // FAZ: späteste FEZ aller Vorgänger
                int maxfez = 0;
                foreach (var vorg in knoten.Vorgaenger)
                    maxfez = Math.Max(maxfez, vorg.FEZ);

                knoten.FAZ = maxfez;

                knoten.FEZ = knoten.FAZ + knoten.Dauer;
            }

            foreach (var nachf in knoten.Nachfolger)
                Vorwaertsrechnung(nachf);
        }

        // Rekursive Funktion (Knoten -> alle Vorgänger) zur Berechnung von
        // SAZ (Spätester Anfangszeitpunkt) und
        // SEZ (Spätester Endzeitpunkt)
        private void Rueckwaertsrechnung(Projektabschnitt knoten)
        {
            if (knoten.IstEndknoten())
            {
                knoten.SAZ = knoten.FAZ;
                knoten.SEZ = knoten.FEZ;
            }
            else
            {
                // SEZ ist frühester SAZ der Vorgänger
                int minstart = int.MaxValue;
                foreach (var nachfolger in knoten.Nachfolger)
                    minstart = Math.Min(minstart, nachfolger.SAZ);

                knoten.SEZ = minstart;
                knoten.SAZ = knoten.SEZ - knoten.Dauer;
            }

            foreach (var vorg in knoten.Vorgaenger)
                Rueckwaertsrechnung(vorg);
        }

        // Rekursive Funktion (Knoten -> alle Nachfolger) zur Berechnung von
        // GP (Gesamtpuffer)
        // FP (Freier Puffer)
        private void PufferBerechnung(Projektabschnitt knoten)
        {
            // Gesamtpuffer: Unterschied in frühestmöglicher
            // und spätestmöglicher Ausführung
            knoten.GesamtPuffer = knoten.SAZ - knoten.FAZ;

            // Freier Puffer ist Puffer der in Anspruch genommen
            // werden kann ohne den Anfang eines Nachfolgers 
            // zu verzögern
            int kleinsteDifferenz = knoten.GesamtPuffer;
            foreach (var nachf in knoten.Nachfolger)
                kleinsteDifferenz = Math.Min(kleinsteDifferenz, nachf.FAZ - knoten.FEZ);
            
            knoten.FreierPuffer = kleinsteDifferenz;

            foreach (var nachf in knoten.Nachfolger)
                PufferBerechnung(nachf);

        }

        /*
        bool ZykelSuchen(Projektabschnitt startKnoten, List<Projektabschnitt> besuchteKnoten)
        {
            if (startKnoten.IstEndknoten())
                return false;

            foreach(Projektabschnitt nachfolger in startKnoten.Nachfolger)
            {
                if (besuchteKnoten.Contains(nachfolger))
                    return true;
                
            }
        }
        */
        #endregion

    }
}
