using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.Estadistica
{
    public partial class EstadisticaMobileMaintenance : SecuredGridReportPage<MobileManteinanceVo>
    {
        protected override string GetRefference() { return "MOBILE_MAINTENANCE"; }
        protected override string VariableName { get { return "STAT_REP_MANTENIMIENTO"; } }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) 
                
            dpDia.SetDate();
        }

        protected override List<MobileManteinanceVo> GetResults()
        {
            Watch.Start();
            if (!dpDia.SelectedDate.HasValue) throw new ApplicationException("Ingrese Fecha");
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDia.SelectedDate.Value.Date);
            var hasta = desde.Add(new TimeSpan(23, 59, 59)).ToDataBaseDateTime();

            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            var vehiculos = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues, ddlBase.SelectedValues, ddlTipoVehiculo.SelectedValues);

            var list = ReportFactory.MobileMaintenanceDAO.GetMobilesMaintenanceData(user, vehiculos, desde, hasta)
                .Select(o => new MobileManteinanceVo(o))
                .ToList();

            var time = Watch.Stop();
            return list;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string> { 
                                                    {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                                    {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                                                    {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                                                    {CultureManager.GetLabel("FECHA"), dpDia.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDia.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                                                    };
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileManteinanceVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileManteinanceVo.IndexKilometros).Text = String.Format("{0:0.00} km", dataItem.Kilometros);
        }
    }
}