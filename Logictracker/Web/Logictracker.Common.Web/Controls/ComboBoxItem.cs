namespace Logictracker.Web.Controls
{
    public class ComboBoxItem
    {
        public int Value { get; set; }
        public string Texts { get; set; }

        public ComboBoxItem(int value, string text)
        {
            Texts = text;
            Value = value;
        }
    }
}
