using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos.Controls
{
    public partial class DatosParte : BaseUserControl
    {
        private DAOFactory __daoFactory;
        private int kmReal;
        private int kmRep;
        private double minReal;
        private double minRep;

        protected int cocheID
        {
            get { return (int)(ViewState["cocheID"] ?? 0); }
            set { ViewState["cocheID"] = value; }
        }
        protected int parteID
        {
            get { return (int)(ViewState["parteID"] ?? 0); }
            set { ViewState["parteID"] = value; }
        }

        private DAOFactory daoFactory
        {
            get
            {
                if (__daoFactory == null)
                    __daoFactory = (Page as ApplicationSecuredPage).DAOFactory;
                return __daoFactory;
            }
        }

        public List<TurnoPartePersonal> Turnos
        {
            get
            {
                //daoFactory = new DAOFactory();
                var turnos = new List<TurnoPartePersonal>();
                var date = Convert.ToDateTime(lblFecha.Text, new CultureInfo("es-AR"));
                var lastDate = date;
                foreach (RepeaterItem item in Repeater1.Items)
                {
                    var lblSalidaAlPozo = item.FindControl("lblSalidaAlPozo") as Label;
                    var lblLlegadaAlPozo = item.FindControl("lblLlegadaAlPozo") as Label;
                    var lblSalidaDelPozo = item.FindControl("lblSalidaDelPozo") as Label;
                    var lblLlegadaDelPozo = item.FindControl("lblLlegadaDelPozo") as Label;
                    var lblKilometraje = item.FindControl("lblKilometraje") as Label;
                    var lblResponsable = item.FindControl("lblResponsable") as Label;
                    var txtSalidaAlPozo = item.FindControl("txtSalidaAlPozo") as TextBox;
                    var txtLlegadaAlPozo = item.FindControl("txtLlegadaAlPozo") as TextBox;
                    var txtSalidaDelPozo = item.FindControl("txtSalidaDelPozo") as TextBox;
                    var txtLlegadaDelPozo = item.FindControl("txtLlegadaDelPozo") as TextBox;
                    var txtKilometraje = item.FindControl("txtKilometraje") as TextBox;

                    var turno = new TurnoPartePersonal();

                    date = lastDate.Date.Add(GetHours(txtSalidaAlPozo.Text));
                    while (date < lastDate) date = date.AddDays(1);
                    turno.AlPozoSalida = date.ToDataBaseDateTime();
                    lastDate = date;

                    date = lastDate.Date.Add(GetHours(txtLlegadaAlPozo.Text));
                    while (date < lastDate) date = date.AddDays(1);
                    turno.AlPozoLlegada = date.ToDataBaseDateTime();
                    lastDate = date;

                    date = lastDate.Date.Add(GetHours(txtSalidaDelPozo.Text));
                    while (date < lastDate) date = date.AddDays(1);
                    turno.DelPozoSalida = date.ToDataBaseDateTime();
                    lastDate = date;

                    date = lastDate.Date.Add(GetHours(txtLlegadaDelPozo.Text));
                    while (date < lastDate) date = date.AddDays(1);
                    turno.DelPozoLlegada = date.ToDataBaseDateTime();
                    lastDate = date;

                    int km;
                    if (!int.TryParse(txtKilometraje.Text, out km))
                        throw new ApplicationException("El kilometraje debe ser un valor numerico");

                    turno.Km = km;

                    turno.KmGps = CalcularKm(turno, daoFactory);

                    turnos.Add(turno);
                }

                return turnos;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SetData(Documento parte, DAOFactory daof)
        {
            __daoFactory = daof;
            //Coche coche = daof.CocheDAO.FindById(Convert.ToInt32(parte.Valores["Vehiculo"]));


            var pp = new PartePersonal(parte, DAOFactory);
            cocheID = pp.Vehiculo;//coche.Id;
            parteID = pp.Id;

            var dt = pp.Turnos[0].AlPozoSalida;
            dt = parte.Fecha.Date.Add(dt.Subtract(dt.Date));

            lblFecha.Text = dt.ToDisplayDateTime().ToString("dd/MM/yyyy");
            lblCodigo.Text = parte.Codigo;
            lblEmpresa.Text = pp.Empresa;//daof.TransportistaDAO.FindById(Convert.ToInt32(parte.Valores["Empresa"])).Descripcion;
            lblInterno.Text = pp.Interno;//coche.Interno;
            lblEquipo.Text = pp.Equipo;//parte.Valores["Equipo"].ToString();
            lblSalida.Text = parte.Valores["Lugar Partida"].ToString();
            lblLlegada.Text = parte.Valores["Lugar Llegada"].ToString();
            lblTipoServicio.Text = pp.TipoServicio;


            Repeater1.DataSource = pp.Turnos;
            Repeater1.DataBind();
        }
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer) SetTotals(e.Item);
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var lblSalidaAlPozo = e.Item.FindControl("lblSalidaAlPozo") as Label;
            var lblLlegadaAlPozo = e.Item.FindControl("lblLlegadaAlPozo") as Label;
            var lblSalidaDelPozo = e.Item.FindControl("lblSalidaDelPozo") as Label;
            var lblLlegadaDelPozo = e.Item.FindControl("lblLlegadaDelPozo") as Label;
            var lblKilometraje = e.Item.FindControl("lblKilometraje") as Label;
            var lblResponsable = e.Item.FindControl("lblResponsable") as Label;
            var txtSalidaAlPozo = e.Item.FindControl("txtSalidaAlPozo") as TextBox;
            var txtLlegadaAlPozo = e.Item.FindControl("txtLlegadaAlPozo") as TextBox;
            var txtSalidaDelPozo = e.Item.FindControl("txtSalidaDelPozo") as TextBox;
            var txtLlegadaDelPozo = e.Item.FindControl("txtLlegadaDelPozo") as TextBox;
            var txtKilometraje = e.Item.FindControl("txtKilometraje") as TextBox;
            var lblOdometroInicio = e.Item.FindControl("lblOdometroInicio") as Label;
            var lblOdometroFin = e.Item.FindControl("lblOdometroFin") as Label;

            const string dateFormat = "HH:mm";

            var turno = e.Item.DataItem as TurnoPartePersonal;

            lblSalidaAlPozo.Text = turno.AlPozoSalida.ToDisplayDateTime().ToString(dateFormat);
            lblLlegadaAlPozo.Text = turno.AlPozoLlegada.ToDisplayDateTime().ToString(dateFormat);
            lblSalidaDelPozo.Text = turno.DelPozoSalida.ToDisplayDateTime().ToString(dateFormat);
            lblLlegadaDelPozo.Text = turno.DelPozoLlegada.ToDisplayDateTime().ToString(dateFormat);
            lblKilometraje.Text = turno.Km.ToString();
            lblResponsable.Text = turno.Responsable;

            var alPozoSale = turno.AlPozoSalidaControl != DateTime.MinValue
                ? turno.AlPozoSalidaControl
                : turno.AlPozoSalida;

            var alPozoLlega = turno.AlPozoLlegadaControl != DateTime.MinValue
                ? turno.AlPozoLlegadaControl
                : turno.AlPozoLlegada;

            var delPozoSale = turno.DelPozoSalidaControl != DateTime.MinValue
                ? turno.DelPozoSalidaControl
                : turno.DelPozoSalida;

            var delPozoLlega = turno.DelPozoLlegadaControl != DateTime.MinValue
                ? turno.DelPozoLlegadaControl
                : turno.DelPozoLlegada;

            var kmControl = turno.KmControl > 0
                ? turno.KmControl
                : CalcularKm(turno, daoFactory);

            lblOdometroInicio.Text = turno.OdometroInicial.ToString();
            lblOdometroFin.Text = turno.OdometroFinal.ToString();
            txtKilometraje.Text = kmControl.ToString();
            txtSalidaAlPozo.Text = alPozoSale.ToDisplayDateTime().ToString(dateFormat);
            txtLlegadaAlPozo.Text = alPozoLlega.ToDisplayDateTime().ToString(dateFormat);
            txtSalidaDelPozo.Text = delPozoSale.ToDisplayDateTime().ToString(dateFormat);
            txtLlegadaDelPozo.Text = delPozoLlega.ToDisplayDateTime().ToString(dateFormat);

            txtSalidaAlPozo.Attributes.Add("onchange", "setModified(this, true);");
            txtLlegadaAlPozo.Attributes.Add("onchange", "setModified(this, true);");
            txtSalidaDelPozo.Attributes.Add("onchange", "setModified(this, true);");
            txtLlegadaDelPozo.Attributes.Add("onchange", "setModified(this, true);");
            txtKilometraje.Attributes.Add("onchange", "setModified(this, false);");


            kmRep += turno.Km;
            kmReal += kmControl;
            minRep += turno.AlPozoLlegada.Subtract(turno.AlPozoSalida).TotalMinutes;
            minRep += turno.DelPozoLlegada.Subtract(turno.DelPozoSalida).TotalMinutes;
            minReal += alPozoLlega.Subtract(alPozoSale).TotalMinutes;
            minReal += delPozoLlega.Subtract(delPozoSale).TotalMinutes;
        }

        private void SetTotals(RepeaterItem footer)
        {
            var lblHorasRep = footer.FindControl("lblHorasRep") as Label;
            var lblHorasReal = footer.FindControl("lblHorasReal") as Label;
            var lblHorasDif = footer.FindControl("lblHorasDif") as Label;
            var lblKmRep = footer.FindControl("lblKmRep") as Label;
            var lblKmReal = footer.FindControl("lblKmReal") as Label;
            var lblKmDif = footer.FindControl("lblKmDif") as Label;
            var lblHsKm = footer.FindControl("lblHsKm") as Label;

            var difHs = minRep - minReal;

            lblHorasRep.Text = Math.Floor(minRep / 60.0).ToString("00") + ":" + (minRep % 60).ToString("00");
            lblHorasReal.Text = Math.Floor(minReal / 60.0).ToString("00") + ":" + (minReal % 60).ToString("00");
            lblHorasDif.Text = Math.Floor(difHs / 60.0).ToString("00") + ":" + (difHs % 60).ToString("00");

            lblKmRep.Text = kmRep.ToString();
            lblKmReal.Text = kmReal.ToString();
            lblKmDif.Text = (kmRep - kmReal).ToString();

            lblHsKm.Text = (kmReal / (minReal / 60)).ToString("0.00");

            Page.ClientScript.RegisterStartupScript(typeof(string), "totales",
                string.Format(@" var lblHorasReal = '{0}';
                var lblHorasDif = '{1}';
                var lblKmReal = '{2}';
                var lblKmDif = '{3}';
                var lblHsKm = '{4}';",
                    lblHorasReal.ClientID,
                    lblHorasDif.ClientID,
                    lblKmReal.ClientID,
                    lblKmDif.ClientID,
                    lblHsKm.ClientID
                 )
                , true);

        }
        public void RecalcularKm(DAOFactory daof)
        {
            var turnos = Turnos;
            for (var i = 0; i < turnos.Count; i++)
            {
                var item = Repeater1.Items[i];
                var txtKilometraje = item.FindControl("txtKilometraje") as TextBox;

                txtKilometraje.Text = CalcularKm(turnos[i], daof).ToString();
            }
        }

        public int CalcularKm(TurnoPartePersonal turno, DAOFactory daof)
        {
            double total = 0;

            total = daof.CocheDAO.GetDistance(cocheID, turno.AlPozoSalida, turno.AlPozoLlegada) + daof.CocheDAO.GetDistance(cocheID, turno.DelPozoSalida, turno.DelPozoLlegada);

            return (int)Math.Round(total);
        }

        private TimeSpan GetHours(string text)
        {
            var parts = text.Split(':');
            if (parts.Length != 2)
                throw new ApplicationException("La hora no tiene el formato correcto");

            int hh, mm;
            if (!int.TryParse(parts[0], out hh) || !int.TryParse(parts[1], out mm))
                throw new ApplicationException("La hora no tiene el formato correcto");

            if (hh < 0 || hh > 23 || mm < 0 || mm > 59)
                throw new ApplicationException("La hora no tiene el formato correcto");

            var ts = new TimeSpan(0, hh, mm, 0);

            return ts;
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var page = (Page as BasePage);

            var coche = daoFactory.CocheDAO.FindById(cocheID);

            var turno = Turnos[e.Item.ItemIndex];

            Session["Distrito"] = coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1;
            Session["Location"] = coche.Linea != null ? coche.Linea.Id : -1;
            Session["TypeMobile"] = coche.TipoCoche.Id;
            Session["Mobile"] = cocheID;
            Session["InitialDate"] = turno.AlPozoSalida;
            Session["FinalDate"] = turno.DelPozoLlegada;

            page.OpenWin("../Monitor/MonitorDeCalidad/monitorDeCalidad.aspx", "Monitor De Calidad");
        }
    }
}