namespace NetzplanBot
{
    internal class Projektabschnitt
    {
        internal List<Projektabschnitt> Vorgaenger { get; set; } = [];
        internal List<Projektabschnitt> Nachfolger { get; set; } = [];
        
        internal string Name { get; set; }
        internal string Beschreibung { get; set; }
        internal int Dauer { get; set; }

        internal int FAZ { get; set; }
        internal int FEZ { get; set; }
        internal int SAZ { get; set; }
        internal int SEZ { get; set; }

        internal int GesamtPuffer { get; set; }
        internal int FreierPuffer { get; set; }

        internal bool IstEndknoten() { return Nachfolger.Count == 0; }
        internal bool IstStartknoten() { return Vorgaenger.Count == 0; }

    }
}
