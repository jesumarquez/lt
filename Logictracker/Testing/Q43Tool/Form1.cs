using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Urbetrack.QuadTree;

namespace Q43Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            QuadTree.i().Open(@"C:\Documents and Settings\evecchio\Escritorio\GR2");
            QuadTree.i().GetPositionZone(-12, -13);

        }
    }
}
