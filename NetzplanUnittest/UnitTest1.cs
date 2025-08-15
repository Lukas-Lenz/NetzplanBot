using Xunit;
using NetzplanBot;

namespace NetzplanUnittest
{
    public class BSP1
    {

        private Netzplan np;

        public BSP1()
        {
            string beisp = "A;Planung;3;-" + Environment.NewLine +
            "B;Softwareentwicklung;7;A" + Environment.NewLine +
            "C;Datenbankentwicklung;4;A" + Environment.NewLine +
            "D;Testphase;1;B,C" + Environment.NewLine +
            "E;Installation, Integration;2;D" + Environment.NewLine +
            "F;Abnahme;1;E";

            np = new(beisp);
        }

        [Fact]
        public void BSP1_Vorwaertsrechnung()
        {
            
            Assert.Equal(0, np.Knoten["A"].FAZ);
            Assert.Equal(3, np.Knoten["A"].FEZ);

            Assert.Equal(3, np.Knoten["B"].FAZ);
            Assert.Equal(10, np.Knoten["B"].FEZ);

            Assert.Equal(3, np.Knoten["C"].FAZ);
            Assert.Equal(7, np.Knoten["C"].FEZ);

            Assert.Equal(10, np.Knoten["D"].FAZ);
            Assert.Equal(11, np.Knoten["D"].FEZ);

            Assert.Equal(11, np.Knoten["E"].FAZ);
            Assert.Equal(13, np.Knoten["E"].FEZ);

            Assert.Equal(13, np.Knoten["F"].FAZ);
            Assert.Equal(14, np.Knoten["F"].FEZ);
        }

        [Fact]
        public void BSP1_Dauer()
        {
            Assert.Equal(3, np.Knoten["A"].Dauer);
            Assert.Equal(7, np.Knoten["B"].Dauer);
            Assert.Equal(4, np.Knoten["C"].Dauer);
            Assert.Equal(1, np.Knoten["D"].Dauer);
            Assert.Equal(2, np.Knoten["E"].Dauer);
            Assert.Equal(1, np.Knoten["F"].Dauer);
        }

        [Fact]
        public void BSP1_Rueckwaertsrechnung()
        {
           
            Assert.Equal(13, np.Knoten["F"].SAZ);
            Assert.Equal(14, np.Knoten["F"].SEZ);

            Assert.Equal(11, np.Knoten["E"].SAZ);
            Assert.Equal(13, np.Knoten["E"].SEZ);

            Assert.Equal(10, np.Knoten["D"].SAZ);
            Assert.Equal(11, np.Knoten["D"].SEZ);

            Assert.Equal(6, np.Knoten["C"].SAZ);
            Assert.Equal(10, np.Knoten["C"].SEZ);

            Assert.Equal(3, np.Knoten["B"].SAZ);
            Assert.Equal(10, np.Knoten["B"].SEZ);

            Assert.Equal(0, np.Knoten["A"].SAZ);
            Assert.Equal(3, np.Knoten["A"].SEZ);

        }

        [Fact]
        public void BSP1_Pufferberechnung()
        {
            Assert.Equal(0, np.Knoten["A"].GesamtPuffer);
            Assert.Equal(0, np.Knoten["A"].FreierPuffer);

            Assert.Equal(0, np.Knoten["B"].GesamtPuffer);
            Assert.Equal(0, np.Knoten["B"].FreierPuffer);

            Assert.Equal(3, np.Knoten["C"].GesamtPuffer);
            Assert.Equal(3, np.Knoten["C"].FreierPuffer);

            Assert.Equal(0, np.Knoten["D"].GesamtPuffer);
            Assert.Equal(0, np.Knoten["D"].FreierPuffer);

            Assert.Equal(0, np.Knoten["E"].GesamtPuffer);
            Assert.Equal(0, np.Knoten["E"].FreierPuffer);

            Assert.Equal(0, np.Knoten["F"].GesamtPuffer);
            Assert.Equal(0, np.Knoten["F"].FreierPuffer);

        }
    }
}
