using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using Logictracker.Web.CustomWebControls.BaseControls;
using System.Web.UI.WebControls;
using Logictracker.Web.CustomWebControls.Input;

namespace Logictracker.Web.BaseClasses.BasePages
{
    public class FilterData
    {
        public const string StaticDistrito = "PARENTI01";
        public const string StaticBase = "PARENTI02";
        public const string StaticVehiculo = "PARENTI03";
        public const string StaticDepartamento = "PARENTI04";
        public const string StaticTransportista = "PARENTI07";
        public const string StaticCategoriaAcceso = "PARENTI15";
        public const string StaticEquipo = "PARENTI19";
        public const string StaticTipoDocumento = "PARENTI25";
        public const string StaticSistema = "SYSFUNC02";
        public const string StaticTipoDispositivo = "PARENTI32";
        public const string StaticTipoVehiculo = "PARENTI17";
        public const string StaticCentroCostos = "PARENTI37";
        public const string StaticTipoEmpleado = "PARENTI43";
        public const string StaticCliente = "PARENTI18";
        public const string StaticTipoReferenciaGeografica = "PARENTI10";
        public const string StaticTipoMensaje = "PARENTI16";
        public const string StaticPuntoEntrega = "PARENTI44";
        public const string StaticInsumo = "PARENTI58";
        public const string StaticProveedor = "PARENTI59";
        public const string StaticTipoInsumo = "PARENTI60";
        public const string StaticProducto = "PARENTI63";
        public const string StaticTipoEntidad = "PARENTI76";
        public const string StaticTipoMedicion = "PARENTI77";
        public const string StaticEntidad = "PARENTI79";
        public const string StaticDetalle = "PARENTI82";
        public const string StaticTipoProveedor = "PARENTI86";
        public const string StaticDeposito = "PARENTI87";
        public const string StaticZona = "PARENTI89";
        public const string StaticTipoZonaAcceso = "PARENTI91";
        public const string StaticTaller = "PARENTI35";

        public const string StaticBocaDeCarga = "PARTICK04";
        public const string StaticEmpresaTelefonica = "EMPRESA";
        public const string StaticTipoLineaTelefonica = "TIPO_LINEA";

        protected const string StaticPrefix = "#STATIC#";
        protected Dictionary<string, object> Data = new Dictionary<string, object>();

        public void Add(string name, object value)
        {
            if (Data.ContainsKey(name)) Data[name] = value;
            else Data.Add(name, value);
        }
        public void AddStatic(string name, object value)
        {
            var session = HttpContext.Current.Session;
            session[StaticPrefix + name] = value;
        }

        public object this[string name]
        {
            get
            {
                if (Data.ContainsKey(name)) return Data[name];
                var session = HttpContext.Current.Session;
                return session[StaticPrefix + name];
            }
        }

        public void Save(Page page)
        {
            page.Session[page.GetType().FullName] = Data;
        }

        public static FilterData Load(Page page)
        {
            var filterData = new FilterData();
            var data = page.Session[page.GetType().FullName] as Dictionary<string, object>;
            if(data != null) filterData.Data = data;
            return filterData;
        }

        public void LoadStaticFilter(string staticName, DropDownListBase combo)
        {
            var filter = this[staticName];
            if (filter != null) combo.SetSelectedValue((int)filter);
        }

        public void LoadLocalFilter(string localName, DropDownListBase combo)
        {
            var filter = this[localName];
            if (filter != null) combo.SetSelectedValue((int)filter);
        }

        public void LoadLocalFilter(string localName, ListBoxBase listBox)
        {
            var filter = this[localName];
            if (filter != null) listBox.SetSelectedIndexes((List<int>)filter);
        }

        public void LoadLocalFilter(string localName, CheckBox checkbox)
        {
            var filter = this[localName];
            if (filter != null) checkbox.Checked = (bool)filter;
        }

        public void LoadLocalFilter(string localName, DateTimePicker dateTimePicker)
        {
            var filter = this[localName];
            if (filter != null) dateTimePicker.SelectedDate = (DateTime?)filter;
        }
    }
}
