using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Urbetrack.Postal.DataSources;
using Urbetrack.Postal.Enums;

namespace Urbetrack.Postal.Forms
{
    public partial class ListServicios : Form
    {
        private bool Disposing = false;

        public ListServicios()
        {
            InitializeComponent();
        }

        private void BindGrid()
        {
            var list = (from o in Rutas.GetRutas() select new ServicioView(o)).ToList();
            var src = new BindingSource(list,"");

            gridServicios.DataSource = src;

            gridServicios.Cols[2].Caption = String.Empty;
            gridServicios.Cols[2].Width = 20;

            gridServicios.Cols[3].Caption = "Falta";
            gridServicios.Cols[3].Width = 35;

            gridServicios.Cols[4].Caption = "Tipo";
            gridServicios.Cols[4].Width = 30;

            gridServicios.Cols[0].Visible = false;
            gridServicios.Cols[1].Visible = false;
            gridServicios.Cols[6].Visible = false;

            var i = 1;

            foreach (var l in list)
            {
                switch (l.Estado)
                {
                    case (EstadoServicio.EnCurso): gridServicios.Rows[i].StyleNew.BackColor = Color.Gold; break;
                    case (EstadoServicio.Pendiente): gridServicios.Rows[i].StyleNew.BackColor = Color.Pink; break;
                    case (EstadoServicio.Terminado): gridServicios.Rows[i].StyleNew.BackColor = Color.LightGreen; break;

                    default: break;
                }

                i++;
            }

            gridServicios.Refresh();
            gridServicios.Focus();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            ShowSelectedServiceDetail(false);
        }

        private void ShowSelectedServiceDetail(bool edicion)
        {
            var rowIndex = gridServicios.RowSel-1;
            
            if (rowIndex < 0) return;

            var servicio = new BindingSource(gridServicios.DataSource,"");
            var obj = servicio[rowIndex] as ServicioView;

            if (obj == null) return;

            var serv = (from o in Rutas.GetRutas() where o.Id == obj.Id select o).FirstOrDefault();

            var form = new Detalle(serv, edicion);
            form.BringToFront();
            form.Show();
        }

        private void TomarLaterales()
        {
            var list = (from r in Rutas.GetRutas()
                        where r.Selected 
                        select r).ToList();

            if (list.Count() > 0)
            {

                var det = new Detalle(list);
                det.TomarLateralesReferencia(false, true, false);

            }

/*                
                var rowIndex = gridServicios.RowSel - 1;

                if (rowIndex < 0) return;

                var servicio = new BindingSource(gridServicios.DataSource, "");
                var obj = servicio[rowIndex] as ServicioView;

                if (obj == null) return;

                var serv = (from o in Rutas.GetRutas() where o.Id == obj.Id select o).FirstOrDefault();

                if (serv.Motivo != null)
                {                
                    var form = new Detalle(serv, true);
                    form.TomarLateralesReferencia(false,true,false);
                    serv.CalcularEstados();
                }
            */
 
        }

        private void TomarReferencia()
        {
            var list = (from r in Rutas.GetRutas()
                        where r.Selected 
                        select r).ToList();

            if (list.Count() > 0)
            {

                var det = new Detalle(list);
                det.TomarLateralesReferencia(false, false, true);

            }



            /*            var rowIndex = gridServicios.RowSel - 1;

            if (rowIndex < 0) return;

            var servicio = new BindingSource(gridServicios.DataSource, "");
            var obj = servicio[rowIndex] as ServicioView;

            if (obj == null) return;

            var serv = (from o in Rutas.GetRutas() where o.Id == obj.Id select o).FirstOrDefault();

            if (serv.Motivo != null)
            {
                var form = new Detalle(serv, true);
                form.TomarLateralesReferencia(false, false, true);
                serv.CalcularEstados();
            }*/
        }

        private void LoadGPS()
        {
            var gps = new GPS();
            gps.Show();         
        }

        private void TomarGPS()
        {
            var list = (from r in Rutas.GetRutas()
                        where r.Selected 
                        select r).ToList();

            if (list.Count() > 0)
            {

                var det = new Detalle(list);
                det.TomarGps(false);

            }



/*            var rowIndex = gridServicios.RowSel - 1;

            if (rowIndex < 0) return;

            var servicio = new BindingSource(gridServicios.DataSource, "");
            var obj = servicio[rowIndex] as ServicioView;

            if (obj == null) return;

            var serv = (from o in Rutas.GetRutas() where o.Id == obj.Id select o).FirstOrDefault();

            if (serv.Motivo != null)
            {
                var form = new Detalle(serv, true);
                form.TomarGPS(false);
                serv.CalcularEstados();
            }*/
        }

        private void TomarFoto()
        {

            var list = (from r in Rutas.GetRutas()
                        where r.Selected
                        select r).ToList();

            if (list.Count() > 0)
            {

                var det = new Detalle(list);
                det.TomarFoto(false);

            }


/*            var rowIndex = gridServicios.RowSel - 1;

            if (rowIndex < 0) return;

            var servicio = new BindingSource(gridServicios.DataSource, "");
            var obj = servicio[rowIndex] as ServicioView;

            if (obj == null) return;

            var serv = (from o in Rutas.GetRutas() where o.Id == obj.Id select o).FirstOrDefault();

            if (serv.Motivo != null)
            {
                var form = new Detalle(serv, true);
                form.TomarFoto(false);
                serv.CalcularEstados();
            }*/
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {

            bool hizo = false;

/*                MessageBox.Show(@"Debe seleccionar servicios pendientes",
                string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);*/


                var list = (from r in Rutas.GetRutas()
                            where r.Selected && (r.LlevaLaterales || r.LlevaReferencia)
                            select r).ToList();

                if (list.Count() > 0)
                {

                    var det = new Detalle(list);
                    det.TomarLateralesReferencia(false,true,true);
                    hizo = true;
                }


                list = (from r in Rutas.GetRutas()
                            where r.Selected && (r.LlevaGPS)
                            select r).ToList();

                if (list.Count() > 0)
                {

                    var det = new Detalle(list);
                    det.TomarGps(false);
                    hizo = true;
                }

                list = (from r in Rutas.GetRutas()
                            where r.Selected && (r.LlevaFoto)
                            select r).ToList();

                if (list.Count() > 0)
                {

                    var det = new Detalle(list);
                    det.TomarFoto(false);
                    hizo = true;
                }


                if (!hizo) MessageBox.Show(@"Debe seleccionar servicios pendientes",
              string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
              MessageBoxDefaultButton.Button1);

                Focus();
                return;
            
            //hacer
        }


        /*
        private void GuardarEstados()
        {
            for (var j = 1; j < gridServicios.Rows.Count; j++)
            {
                if ((bool)gridServicios[j,2])
                {
                    var id = (int) gridServicios[j, 1];
                    var servicio = Rutas.GetRutas().Where(s => s.Id == id).FirstOrDefault();
                    //var servicio = finbyid;
                    servicio.Estado = EstadoServicio.Terminado;
                    Rutas.AddRuta(servicio);
                }
            }
        }*/

        private void ListServicios_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter))
            {
                gridServicios[gridServicios.RowSel, 2] = (bool)gridServicios[gridServicios.RowSel, 2] ? false: true;

                var rowIndex = gridServicios.RowSel - 1;

                if (rowIndex < 0) return;
                Rutas.GetRutas()[rowIndex].Selected = (bool) gridServicios[gridServicios.RowSel, 2]; 
            }       

            if ((e.KeyValue == 205)) //e
            {
                ShowSelectedServiceDetail(true);
            }

            if ((e.KeyValue == 222)) //l
            {
                TomarLaterales();
            }

            if ((e.KeyValue == 49)) //r
            {
                TomarReferencia();
            }

            if ((e.KeyValue == 53)) //g
            {
                TomarGPS();
            }

            if ((e.KeyValue == 52)) //f
            {
                TomarFoto();
            }
            if(e.KeyCode == Keys.T)
            {
                LoadGPS();
            }
            if (e.KeyCode == Keys.S)
            {
                var result = MessageBox.Show("¿Desea salir de la aplicación?", "Urbetrack Postal",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                             MessageBoxDefaultButton.Button2);
                if(result == DialogResult.Yes)
                {
                    Disposing = true;
                    Application.Exit();
                }
            }
        }


        private void OnKeyPress(object sender, KeyPressEventArgs ke)
        {
            // Determine if ESC key value is pressed. ESC = Back Button
            if (ke.KeyChar == (Char)Keys.Escape)
            {
                // Handle the event to provide functionality.
                ke.Handled = true;

/*                var form = new Menu();
                form.BringToFront();
                form.Show();*/
                Disposing = true;
                Close();
            }

        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!Disposing) BindGrid();
        }
    }
}