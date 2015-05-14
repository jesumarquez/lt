using System.Drawing;
using System.Windows.Forms;

namespace Logictracker.Geocoder
{
    public partial class RichConsole : UserControl
    {
        public event LinkClickedEventHandler LinkClicked;

        public RichConsole()
        {
            InitializeComponent();
            
        }
        public void BeginIndent() { txtResult.SelectionIndent += 20; }
        public void EndIndent() { txtResult.SelectionIndent -= 20; }
        public void BeginBold() { txtResult.SelectionFont = new Font(txtResult.SelectionFont, FontStyle.Bold); }
        public void EndBold() { txtResult.SelectionFont = new Font(txtResult.SelectionFont, FontStyle.Regular); }
        public void Write(string text, Color color)
        {
            var oldColor = txtResult.SelectionColor;
            txtResult.SelectionColor = color;
            txtResult.AppendText(text);
            txtResult.SelectionColor = oldColor;
            txtResult.ScrollToCaret();
            txtResult.Refresh();
        }
        public void Write(string text) { Write(text, Color.Black); }
        public void WriteLine(string text, Color color) { Write(text + "\n", color); }
        public void WriteLine(string text) { WriteLine(text, Color.Black); }
        public void WriteLine() { WriteLine(string.Empty); }

        public void Clear() { txtResult.Clear(); }

        private void TxtResultLinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (LinkClicked != null) LinkClicked(this, e);
        }
    }
}
