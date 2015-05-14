#region Usings

using System;
using System.Windows.Forms;
using Urbetrack.Comm.Core.Qtree;
using Urbetrack.Toolkit;

#endregion

namespace QtreeTester
{
    public partial class MainForm : Form
    {
        readonly Qtree tree = new Qtree();
        public MainForm()
        {
            T.InitAppDomain();
            InitializeComponent();
            /*
            var bs = new BitStream(8192);

            bs.Push(true);
            bs.Push(false);
            bs.Push(true);
            bs.Push(false);
            bs.Push(true);
            bs.Push(true);
            bs.Push(false);
            Marshall.User("---");
            bs.Push(0xAA);
            Marshall.User("---");
            bs.Push(0xF0);
            Marshall.User("---");
            bs.Push(0x0F); 
            Marshall.User("---");
            bs.Push(5,13); // 7 + 13 = 20 + 24 = 44.

            bs.Pad();

            Marshall.User("Total Bits: {0}", bs.total_bits);
            Marshall.User("");
            
            bs.Rewind();

            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            Marshall.User("> " + (bs.Pop() ? "true" : "false"));
            var aa = new byte();
            bs.Pop(ref aa);
            Marshall.User(">{0:X}", aa);
            aa = 0;
            bs.Pop(ref aa);
            Marshall.User(">{0:X}", aa);
            aa = 0;
            bs.Pop(ref aa);
            Marshall.User(">{0:X}", aa);
            var next = bs.Pop(13);
            Marshall.User(">{0}", next);
            */
            var i = 0;
            var total = 0;
            //Qtree.Upgrade("C:\\Documents and Settings\\administrador.MICROSTAR\\Escritorio\\EMA");
            //tree.Initialize("C:\\Documents and Settings\\administrador.MICROSTAR\\Escritorio\\EMA");
            T.TRACE(">>>> Initialize: Hora Inicial: {0}", DateTime.Now);
            tree.Initialize("D:\\GR2_20090911");
            T.TRACE("<<<< Initialize: Hora Final: {0}", DateTime.Now);

            T.TRACE(">>>> Query: Hora Inicial: {0}", DateTime.Now);
            foreach (var block in tree.GetQTreeCompressed(512,0))
            {
                i++;
                total += block.GetLength(0);
            }
            T.TRACE("<<<< Query: Hora Final: {0}", DateTime.Now);
            T.TRACE("---- TOTAL: {0} paginas, {1} bytes.", i, total);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
