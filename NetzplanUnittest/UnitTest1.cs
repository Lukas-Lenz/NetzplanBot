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
            
            Assert.Equal(0, np.GetFAZ("A"));
            Assert.Equal(3, np.GetFEZ("A"));

            Assert.Equal(3, np.GetFAZ("B"));
            Assert.Equal(10, np.GetFEZ("B"));

            Assert.Equal(3, np.GetFAZ("C"));
            Assert.Equal(7, np.GetFEZ("C"));

            Assert.Equal(10, np.GetFAZ("D"));
            Assert.Equal(11, np.GetFEZ("D"));

            Assert.Equal(11, np.GetFAZ("E"));
            Assert.Equal(13, np.GetFEZ("E"));

            Assert.Equal(13, np.GetFAZ("F"));
            Assert.Equal(14, np.GetFEZ("F"));
        }

        [Fact]
        public void BSP1_Rueckwaertsrechnung()
        {
            Assert.Equal(13, np.GetSAZ("F"));
            Assert.Equal(14, np.GetSEZ("F"));

            Assert.Equal(11, np.GetSAZ("E"));
            Assert.Equal(13, np.GetSEZ("E"));

            Assert.Equal(10, np.GetSAZ("D"));
            Assert.Equal(11, np.GetSEZ("D"));

            Assert.Equal(6, np.GetSAZ("C"));
            Assert.Equal(10, np.GetSEZ("C"));

            Assert.Equal(3, np.GetSAZ("B"));
            Assert.Equal(10, np.GetSEZ("B"));

            Assert.Equal(0, np.GetSAZ("A"));
            Assert.Equal(3, np.GetSEZ("A"));

        }

        [Fact]
        public void BSP1_Pufferberechnung()
        {
            Assert.Equal(0, np.GetGesamtPuffer("A"));
            Assert.Equal(0, np.GetFreierPuffer("A"));
        }
    }
}
