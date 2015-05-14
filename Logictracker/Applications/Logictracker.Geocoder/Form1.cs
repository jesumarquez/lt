using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Logictracker.Geocoder.GeocoderService;

namespace Logictracker.Geocoder
{
    public partial class Form1 : Form
    {
        private string _sessionId = string.Empty;
        private string[] _columnFormat;
        public Form1()
        {
            InitializeComponent();
            splitContainer1.Panel1.BackColor = Color.FromArgb(250, 184, 2);
            panelLine.BackColor = Color.FromArgb(174, 128, 0);
        }

        private void Form1Load(object sender, EventArgs e)
        {
            Activado = false;
            System.Console.WriteLine("Iniciando...");
            timer1.Start();
        }

        private void Initialize()
        {
            try
            {
                InitWs();
                ConfigureColumns();
                LoadProvincias();
                Activado = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR", Color.Red);
                Console.BeginIndent();
                Console.WriteLine(ex.ToString(), Color.Red);
                Console.EndIndent();
            }
        }
        private void ConfigureColumns()
        {
            gridDirecciones.Columns.Clear();
            var colFormat = new List<string>();
            for(var i = 1; ConfigurationManager.AppSettings["column." + i + ".value"] != null; i++)
            {
                var title = ConfigurationManager.AppSettings["column." + i + ".title"] ?? "Columna " + i;
                var format = ConfigurationManager.AppSettings["column." + i + ".value"];
                gridDirecciones.Columns.Add(title, title);
                colFormat.Add(format);
            }
            _columnFormat = colFormat.ToArray();
        }
        private void InitWs()
        {
            Console.Write("Iniciando Web Service... ");
            Console.WriteLine("OK", Color.Green);
        }
        private bool IsActive()
        {
            Console.Write("Verificando estado... ");

            var result = Client.IsActive(_sessionId);
            if (result.Resultado)
            {
                Console.WriteLine("Conectado", Color.Green);
            }
            else
            {
                Console.WriteLine("Desconectado", Color.Red);
            }
            return result.Resultado;
        }

        private void Login()
        {
            if (IsActive()) return;

            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];

            Console.Write("Iniciando sesion... ");
            
            var result = Client.Login(username, password);
            _sessionId = result.Resultado;
            if (result.RespuestaOk)
            {
                Console.WriteLine("OK", Color.Green);
            }
            else
            {
                Console.WriteLine("ERROR", Color.Red);
                Console.BeginIndent();
                Console.WriteLine(result.Mensaje, Color.Red);
                Console.EndIndent();
            }
        }
        private void LoadProvincias()
        {
            Login();
            Console.Write("Buscando Provincias... ");
            var result = Client.GetProvincias(_sessionId);
            if (result.RespuestaOk)
            {
                Console.WriteLine("OK", Color.Green);
                cbProvincia.DisplayMember = "Nombre";
                cbProvincia.DataSource = result.Resultado;
            }
            else
            {
                Console.WriteLine("ERROR", Color.Red);
                Console.BeginIndent();
                Console.WriteLine(result.Mensaje, Color.Red);
                Console.EndIndent();
            }
        }

        private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) AcceptButton = btBuscarSmart;
            else if (tabControl1.SelectedIndex == 1) AcceptButton = btBuscarNormal;
        }

        private void BtBuscarSmartClick(object sender, EventArgs e)
        {
            Activado = false;
            try
            {
                Login();

                Console.Write("Buscando ");
                Console.BeginBold();
                Console.Write(txtDireccion.Text.Trim() + " ");
                Console.EndBold();

                var result = Client.GetSmartSearch(_sessionId, txtDireccion.Text.Trim());

                txtDireccion.Clear();

                if (result.RespuestaOk)
                {
                    Console.WriteLine("OK", Color.Green);
                    SetResultados(result.Resultado);
                }
                else
                {
                    Console.WriteLine("ERROR", Color.Red);
                    Console.BeginIndent();
                    Console.WriteLine(result.Mensaje, Color.Red);
                    Console.EndIndent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR", Color.Red);
                Console.BeginIndent();
                Console.WriteLine(ex.ToString(), Color.Red);
                Console.EndIndent();
            }
            Activado = true;
        }

        private void BtBuscarNormalClick(object sender, EventArgs e)
        {
            Activado = false;
            try
            {
                Login();
                var calle = txtCalle.Text.Trim();
                var altura = (int)txtAltura.Value;
                var esquina = txtEsquina.Text.Trim();
                var partido = txtPartido.Text.Trim();
                var provincia = (cbProvincia.SelectedItem as ProvinciaVO).Id;
                var provdesc = (cbProvincia.SelectedItem as ProvinciaVO).Nombre;

                Console.Write("Buscando ");
                Console.BeginBold();
                Console.Write(calle+ ", " + altura+ ", " + esquina+ ", " +  partido+ ", " + provdesc);
                Console.EndBold();

                var result = Client.GetDireccion(_sessionId, calle, altura, esquina, partido, provincia);

                txtCalle.Clear();
                txtAltura.Value = 0;
                txtEsquina.Clear();
                txtPartido.Clear();


                if (result.RespuestaOk)
                {
                    Console.WriteLine("OK", Color.Green);
                    SetResultados(result.Resultado);
                }
                else
                {
                    Console.WriteLine("ERROR", Color.Red);
                    Console.BeginIndent();
                    Console.WriteLine(result.Mensaje, Color.Red);
                    Console.EndIndent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR", Color.Red);
                Console.BeginIndent();
                Console.WriteLine(ex.ToString(), Color.Red);
                Console.EndIndent();
            }
            Activado = true;
        }

        private void SetResultados(IEnumerable<DireccionVO> resultados)
        {
            gridDirecciones.Rows.Clear();
            foreach (var vo in resultados)
            {
                System.Console.WriteLine(vo.Direccion, Color.Blue);
                var values = new object[] { vo.Direccion, vo.Calle, vo.Altura, vo.Partido, vo.Provincia, vo.Latitud.ToString(CultureInfo.InvariantCulture), vo.Longitud.ToString(CultureInfo.InvariantCulture) };
                var row = new object[_columnFormat.Length];
                for (int i = 0; i < _columnFormat.Length; i++)
                {
                    var s = _columnFormat[i];
                    row[i] = string.Format(s, values);
                }
                gridDirecciones.Rows.Add(row);
            }
        }

        public bool Activado
        {
            get
            {
                return splitContainer1.Enabled;
            }
            set
            {
                splitContainer1.Enabled = value;
            }
        }

        private static GeocoderWsSoapClient _client;
        public static GeocoderWsSoapClient Client
        {
            get
            {
                return _client ?? (_client = new GeocoderWsSoapClient(GetHttpBinding(), new EndpointAddress(ConfigurationManager.AppSettings["serverurl"])));
            }
        }

        private static BasicHttpBinding GetHttpBinding()
        {
            var binding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(1),
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                AllowCookies = false,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true
            };
            // I think most (or all) of these are defaults--I just copied them from app.config:
            binding.TransferMode = TransferMode.Buffered;
            binding.MaxBufferPoolSize = 524288;
            binding.MaxBufferSize = 65536;
            binding.MaxReceivedMessageSize = 65536;
            return binding;
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            Initialize();
        }
    }
}
