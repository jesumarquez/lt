using System;
using System.Drawing;
using System.Windows.Forms;

namespace Map
{
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        private Color toolsColor;
        public Color ToolsColor
        {
            get { return toolsColor; }
            set
            {
                toolsColor = value;
                Invalidate();
            }
        }

        private void MapView_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            Brush b = new SolidBrush(ToolsColor);

            // calculate width of progress bar filler in pixels
            var bar_width = (((43 / 100) * Width));

            // draw the progress bar
            e.Graphics.FillRectangle(b, 0, 0, bar_width, Height);

            b.Dispose();
        }
    }
}
