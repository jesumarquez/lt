using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using System.Text;

namespace Logictracker.Reportes.Estadistica
{
    public partial class MobilePoisHistoric : SecuredGridReportPage<MobilePoiHistoricVo>
    {
        protected override string VariableName { get { return "DOP_CERCANIA_POI"; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlDistrito.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
        }

        #region Protected Methods

        protected override void AddToolBarIcons() { ToolBar.AddCsvToolbarButton(); }

        /// <summary>
        /// Gets report results.
        /// </summary>
        /// <returns></returns>
        protected override List<MobilePoiHistoricVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.Value);
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.Value);

            var coches = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues, ddlBase.SelectedValues)
                .Where(c => c.Estado < Coche.Estados.Inactivo || c.DtCambioEstado > desde)
                .ToList();

            return ReportFactory.MobilePoiHistoricDAO.GetMobileNearPois(coches, ddlGeoRef.Selected, npDistancia.Number, desde, hasta)
                .Select(r => new MobilePoiHistoricVo(r))
                .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobilePoiHistoricVo dataItem)
        {
             e.Row.BackColor = dataItem.Velocidad > 0 ? Color.LightGreen : Color.LightGray;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById( ddlDistrito.Selected).RazonSocial : null },
                           {CultureManager.GetEntity("PARENTI02"),ddlBase.Selected > 0? DAOFactory.LineaDAO.FindById(ddlBase.Selected).DescripcionCorta: null},
                           {CultureManager.GetMenu("PAR_POI"), ddlGeoRef.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(ddlGeoRef.Selected).Descripcion : null},
                           {CultureManager.GetLabel("DISTANCIA"), npDistancia.Number.ToString("#0")},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.Value.ToShortDateString() + " " + dpDesde.SelectedDate.Value.ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.Value.ToShortDateString() + " " + dpHasta.SelectedDate.Value.ToShortTimeString()}
                       };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            var dic = new Dictionary<string, string>();
            var sDistritos = new StringBuilder();
            var sBases = new StringBuilder();

            foreach (var distrito in ddlDistrito.SelectedValues)
            {
                if (!sDistritos.ToString().Equals(""))
                    sDistritos.Append(",");

                sDistritos.Append((string) distrito.ToString("#0"));
            }
            foreach (var bas in ddlBase.SelectedValues)
            {
                if (!sBases.ToString().Equals(""))
                    sBases.Append(",");

                sBases.Append((string) bas.ToString("#0"));
            }

            dic.Add("USER", Usuario.Id.ToString("#0"));
            dic.Add("DISTRITOS", sDistritos.ToString());
            dic.Add("BASES", sBases.ToString());
            dic.Add("GEOREF", ddlGeoRef.Selected.ToString("#0"));
            dic.Add("DISTANCIA", npDistancia.Number.ToString("#0"));
            return dic;
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "MOBILE_POIS_HISTORIC"; }

        #endregion
    }
}
