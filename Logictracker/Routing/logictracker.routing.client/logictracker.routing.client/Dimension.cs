namespace Logictracker.Routing.Client
{
    public class Dimension
    {
        private int Index { get; set; }
        private string Value { get; set; }

        public Dimension(int index, string value)
        {
            Index = index;
            Value = value;
        }
    }
}