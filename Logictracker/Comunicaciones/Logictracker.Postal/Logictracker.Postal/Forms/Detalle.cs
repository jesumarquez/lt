using System;
using System.Linq;
using System.Windows.Forms;
using Urbetrack.Postal.DataSources;
using Urbetrack.Postal.Enums;
using System.Collections.Generic;


namespace Urbetrack.Postal.Forms
{
    public partial class Detalle : Form
    {
        private readonly Ruta _service;
        private readonly List<Ruta> _services;
        private readonly bool _edicion;
        private readonly bool _motivoEnabled;
        private Motivo _motivoSeleccionado;
        private readonly bool _multiple;


        public Detalle(Ruta servicio) : this(servicio, false) { }

        public Detalle(List<Ruta> servicios)
        {
            InitializeComponent();

            _multiple = true;

            _service = servicios[0];
            _services = servicios;

            tbDireccion.Text = _service.Direccion;
            tbPieza.Text = _service.Pieza;
            tbDestinatario.Text = _service.Destinatario;
            tbTipoServicio.Text = _service.TipoServicioDesc;
            tbLateral1.Text = _service.Lateral1;
            tbLateral2.Text = _service.Lateral2;
            tbReferencia.Text = _service.Referencia;

            BindingManager.BindMotivos(cbMotivo, _service.Cliente);

            _motivoEnabled = true;

            if (_service.Motivo != null)
            {
                cbMotivo.SelectedValue = _service.Motivo.Value;
                cbMotivo.Enabled = false;
                _motivoEnabled = false;
            }
        }

        public Detalle(Ruta servicio, bool edicion)
        {
            InitializeComponent();

            _multiple = false;
            _edicion = edicion;
            _service = servicio;
            _services = new List<Ruta> { servicio };

            tbDireccion.Text = servicio.Direccion;
            tbPieza.Text = servicio.Pieza;
            tbDestinatario.Text = servicio.Destinatario;
            tbTipoServicio.Text = servicio.TipoServicioDesc;
            tbLateral1.Text = servicio.Lateral1;
            tbLateral2.Text = servicio.Lateral2;
            tbReferencia.Text = servicio.Referencia;

            BindingManager.BindMotivos(cbMotivo, servicio.Cliente);

            _motivoEnabled = true;

            if (servicio.Motivo != null)
            {
                cbMotivo.SelectedValue = servicio.Motivo.Value;
                if (!edicion)
                {
                    cbMotivo.Enabled = false;
                    _motivoEnabled = false;
                }

            }
            else
            {
                cbMotivo.SelectedItem = null;
            }
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            var list = new ListServicios();
            list.Show();
            Close();
        }

        public void TomarLateralesReferencia(bool preguntar, bool forzarLaterales, bool forzarReferencia)
        {
            var sq = new SQLiteDataSet();
            bool hacer = false;

            if (preguntar)
            {
                var mbLatRef = MessageBox.Show(@"¿Desea cargar la información adicional ahora?",
                                               string.Empty,
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question,
                                               MessageBoxDefaultButton.Button1);

                if (mbLatRef == DialogResult.Yes) hacer = true;
            }
            else
            {
                hacer = true;
            }

            if (hacer)
            {
                try
                {

                    using (var lateralesReferencia = new LateralesReferencia())
                    {
                        lateralesReferencia.Referencia = _service.Referencia;
                        lateralesReferencia.Lateral1 = _service.Lateral1;
                        lateralesReferencia.Lateral2 = _service.Lateral2;


                        if (forzarReferencia || forzarLaterales)
                        {
                            lateralesReferencia.UsaReferencia = forzarReferencia;
                            lateralesReferencia.UsaLaterales = forzarLaterales;
                        }
                        else
                        {
                            lateralesReferencia.UsaReferencia = _service.ConReferencia.Validate(_service.Motivo.EsEntrega);// || forzarReferencia;
                            lateralesReferencia.UsaLaterales = _service.ConLaterales.Validate(_service.Motivo.EsEntrega);// || forzarLaterales;
                        }


                        lateralesReferencia.Opcionales = (forzarReferencia && forzarLaterales);
                        lateralesReferencia.ShowDialog();


                        if (lateralesReferencia.acepto)
                        {
                            if (lateralesReferencia.UsaReferencia) _service.Referencia = lateralesReferencia.Referencia;
                            if (lateralesReferencia.UsaLaterales)
                            {
                                _service.Lateral1 = lateralesReferencia.Lateral1;
                                _service.Lateral2 = lateralesReferencia.Lateral2;
                            }

                            _service.CalcularEstados();

                            if (_multiple)
                            {

                                foreach (Ruta r in _services)
                                {
                                    if (lateralesReferencia.UsaReferencia) r.Referencia = _service.Referencia;
                                    if (lateralesReferencia.UsaLaterales)
                                    {
                                        r.Lateral1 = _service.Lateral1;
                                        r.Lateral2 = _service.Lateral2;
                                    }

                                    r.CalcularEstados();
                                }
                            }
                            sq.UpdateLateralesReferencia(_services);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        public void TomarGps(bool preguntar)
        {
            var sq = new SQLiteDataSet();
            bool hacer = false;

            if (preguntar)
            {
                var mbFoto = MessageBox.Show(@"¿Desea tomar la posición GPS ahora?", string.Empty,
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                             MessageBoxDefaultButton.Button1);


                if (mbFoto == DialogResult.Yes) hacer = true;
            }
            else
            {
                hacer = true;
            }

            if (hacer)
            {

                try
                {
                    using (var gps = new GPS())
                    {
                        gps.ShowDialog();
                        if (gps.gotFix)
                        {
                            _service.TieneGPS = true;
                            _service.Latitud = gps.lastPosition.dblLatitude;
                            _service.Longitud = gps.lastPosition.dblLongitude;

                            sq.UpdateGPS(_services);

                            if (_multiple)
                            {
                                foreach (Ruta r in _services)
                                {
                                    r.Latitud = _service.Latitud;
                                    r.Longitud = _service.Longitud;
                                    r.CalcularEstados();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        public void TomarFoto(bool preguntar)
        {
            var sq = new SQLiteDataSet();
            bool hacer = false;

            if (preguntar)
            {
                var mbFoto = MessageBox.Show(@"¿Desea sacar la foto ahora?",
                                             string.Empty,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question,
                                             MessageBoxDefaultButton.Button1);


                if (mbFoto == DialogResult.Yes) hacer = true;
            }
            else
            {
                hacer = true;
            }

            if (hacer && Configuration.PhotoNeedsGps)
            {
                if (_services.Any(s => s.LlevaGPS && !s.TieneGPS))
                {
                    TomarGps(false);

                    if (_services.Any(s => s.LlevaGPS && !s.TieneGPS))
                    {
                        MessageBox.Show("No puede tomar la foto porque no tiene señal de GPS");
                        hacer = false;
                    }
                }
            }

            if (hacer)
            {

                try
                {
                    using (var camara = new Camara())
                    {
                        camara.ShowDialog();

                        if (camara.Acepto)
                        {
                            _service.TieneFoto = true;
                            _service.Foto = camara.Image;

                            sq.UpdateFoto(_services);

                            if (_multiple)
                            {
                                foreach (Ruta r in _services)
                                {
                                    r.Foto = _service.Foto;
                                    r.CalcularEstados();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        public void TomarDatos(bool preguntar)
        {
            //var sq = new SQLiteDataSet();

            _service.CalcularEstados();

            if ((!_service.TieneLaterales || _edicion) || (!_service.TieneReferencia || _edicion))
            {
                if (_service.LlevaReferencia || _service.LlevaLaterales)
                {
                    TomarLateralesReferencia(preguntar, false, false);
                }
            }


            if (!_service.TieneGPS || _edicion)
            {
                if (_service.LlevaGPS)
                {
                    TomarGps(preguntar);
                }
            }



            if (!_service.TieneFoto || _edicion)
            {

                if (_service.LlevaFoto)
                {
                    TomarFoto(preguntar);
                }
            }


            _service.CalcularEstados();

            /*
            if ((_service.LlevaLaterales && _service.TieneLaterales) &&
                (_service.LlevaReferencia && _service.TieneReferencia) && (_service.LlevaGPS && _service.TieneGPS) &&
                (_service.LlevaFoto && _service.TieneFoto))
                _service.Estado = EstadoServicio.Terminado;
             */
        }


        private void btAceptar_Click(object sender, EventArgs e)
        {

            if (_motivoSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un motivo");
                return;
            }

            _service.Motivo = _motivoSeleccionado;

            if (_service.Estado == EstadoServicio.Terminado) IrALista();

            if (_motivoEnabled)
            {
                if (_service.Confirma.Validate(_service.Motivo.EsEntrega))
                {
                    var mbConfirmar = MessageBox.Show(@"¿Esta usted seguro que desea ejecutar la acción " + _motivoSeleccionado.Text + @"?", string.Empty, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    if (mbConfirmar == DialogResult.No)
                    {
                        Focus();

                        return;
                    }
                }

                //Rutas.AddRuta(_service);
                var sq = new SQLiteDataSet();
                sq.executeSQL(String.Concat("UPDATE rutas set estado = 2, fecha_motivo = current_timestamp, motivo = ", _motivoSeleccionado.Value, " where id = ", _service.Id));
            }

            TomarDatos(true);

            IrALista();
        }

        private void GuardarEstado()
        {
            _service.Estado = EstadoServicio.Terminado;
            Rutas.AddRuta(_service);
        }

        private void IrALista()
        {
            Close();
        }

        private void Detalle_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F1))
            {
                IrALista();
            }
        }

        private void ValidateLaterales(object sender, EventArgs e)
        {
            var obj = sender as TextBox;

            if (obj == null) return;
            try
            {
                Convert.ToInt32(obj.Text);
            }
            catch (Exception) { MessageBox.Show(@"Los laterales deben ser numéricos"); }

        }

        private void cbMotivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMotivo.SelectedItem == null)
                _motivoSeleccionado = null;
            else
                _motivoSeleccionado = (Motivo)cbMotivo.SelectedItem;

        }

    }
}