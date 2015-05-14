namespace Logictracker.Qtree.AutoGen
{
    public class NivelAutoGen
    {
        public int NivelPoligonal { get; set; }
        public int NivelQtree { get; set; }
        public int BrushSize { get; set; }
        public int Prioridad { get; set; }
        public NivelAutoGen(int prioridad, int nivelPoligonal, int nivelQtree, int brushSize)
        {
            Prioridad = prioridad;
            NivelPoligonal = nivelPoligonal;
            NivelQtree = nivelQtree;
            BrushSize = brushSize;
        }
    }
}
