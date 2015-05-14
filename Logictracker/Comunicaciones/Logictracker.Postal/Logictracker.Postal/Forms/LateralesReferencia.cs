using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Urbetrack.Postal.Forms
{
    public partial class LateralesReferencia : Form
    {
        private bool _usaLaterales = false;
        private bool _usaReferencia = false;
        private bool _acepto = false;
        public bool Opcionales { get; set; }

        public bool acepto
        {
            get
            {
                return _acepto;
            }
        }

        public string Lateral1
        {
            get
            {
                return tbLateral1.Text;
            }
            set
            {
                tbLateral1.Text = value;
                tbLateral1.SelectAll();
            }
        }

        public string Lateral2
        {
            get
            {
                return tbLateral2.Text;
            }
            set
            {
                tbLateral2.Text = value;
                tbLateral2.SelectAll();
            }
        }

        public string Referencia
        {
            get
            {
                return tbReferencia.Text;
            }
            set
            {
                tbReferencia.Text = value;
                tbReferencia.SelectAll();
            }
        }

        public bool UsaLaterales
        {
            get
            {
                return _usaLaterales;
            }
            set
            {
                _usaLaterales = value;
                tbLateral1.Enabled = _usaLaterales;
                tbLateral2.Enabled = _usaLaterales;
            }

        }

        public bool UsaReferencia
        {
            get
            {
                return _usaReferencia;
            }
            set
            {
                _usaReferencia = value;
                tbReferencia.Enabled = _usaReferencia;
            }

        }

        public LateralesReferencia()
        {
            InitializeComponent();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Lateral1 = "";
            Lateral2 = "";
            Referencia = "";
            _acepto = false;
            Close();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (!Opcionales)
            {
                if (UsaLaterales && (Lateral1 == ""))
                    MessageBox.Show("Debe ingresar Lateral 1");
                else if (UsaLaterales && (Lateral2 == ""))
                    MessageBox.Show("Debe ingresar Lateral 2");
                else if (UsaReferencia && (Referencia == ""))
                    MessageBox.Show("Debe ingresar Referencia");
                else
                {
                    _acepto = true;
                    Close();
                }
            } else
            {
                _acepto = true;
                Close();                
            }
        }
    }
}