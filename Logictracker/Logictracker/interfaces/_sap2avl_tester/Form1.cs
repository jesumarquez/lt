using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using etao.sap2avl;

namespace _sap2avl_tester
{
    public partial class Form1 : Form
    {
        etao.sap2avl.Watcher w = new Watcher(null);
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = ConfigurationManager.AppSettings["watch_path"];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //w.Create();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            etao.sap2avl.ProjImport a = new ProjImport(null);
            string _format = "10,custcode;40,name;8,shortname;40,calle;40,altura;40,ciudad;10,state;10,cp;3,pais;40,ignore;14,telefono;203,ignore;2,valid"; // ;40;14;14;14;14;3;12;8;8;40;30;40;20";
           // a.ImportFixed("D:\\watch\\cust.imp", _format, "D:\\watch\\urbetrack_cust.log", "sp_urb_lomax_insert_cust");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            etao.sap2avl.ProjImport a = new ProjImport(null);
            string _format = "10,custcode;12,contract;40,name;12,qty;40,obs;14,telefono;30,ignore;8,setupdate;23,ignore;40,calle;30,altura;8,ignore;16,cp;12,ignore;5,uom;14,state;14,ciudad;14,localidad;1,porf;1,valid;44,ignore;8,expiredate;40,instructions;40,contactname";
          //  a.ImportFixed("D:\\watch\\proj.imp", _format, "D:\\watch\\urbetrack_proj.log", "sp_urb_lomax_insert_proj");
            w.reload_dir(null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}