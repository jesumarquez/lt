using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using System.Text;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class ReporteInOut : SecuredGridReportPage<ReporteInOutVo>
    {
        protected override string VariableName { get { return "REP_IN_OUT"; } }
        protected override string GetRefference() { return "REP_IN_OUT"; }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) dpFecha.SetDate();
        }
        
        protected override List<ReporteInOutVo> GetResults()
        {
            if (QueryExtensions.IncludesAll(ddlTipoGeocerca.SelectedValues))
                ddlTipoGeocerca.ToogleItems();

            var empresa = ddlLocacion.Selected;
            var linea = ddlPlanta.Selected;
            var tipos = ddlTipoGeocerca.SelectedValues;
                                                                                       
            var fecha = dpFecha.SelectedDate.Value.ToDataBaseDateTime();
            
            var results = new List<ReporteInOutVo>();

            var vehicles = DAOFactory.CocheDAO.GetList(new[] { empresa }, new[] { linea });
            var referenciasGeograficas = DAOFactory.ReferenciaGeograficaDAO.GetListVigentes(empresa, linea, tipos, fecha);

            foreach (var tipo in tipos)
            {
                var vehiclesIn = 0;
                var vehiclesOut = vehicles.Count();
                var referencias = referenciasGeograficas.Where(r => r.TipoReferenciaGeografica.Id == tipo);

                foreach (var referencia in referencias)
                {
                    var geocerca = DAOFactory.ReferenciaGeograficaDAO.FindGeocerca(referencia.Id);
                    var cant = DAOFactory.LogPosicionDAO.GetVehiclesInside(vehicles.Select(v => v.Id), geocerca, fecha);
                    vehiclesIn += cant;
                    vehiclesOut -= cant;
                }

                var tipoGeorefencia = DAOFactory.TipoReferenciaGeograficaDAO.FindById(tipo);

                var rep = new ReporteInOutVo(tipoGeorefencia.Descripcion, vehiclesIn, vehiclesOut);
                results.Add(rep);
            }

            return results;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteInOutVo dataItem)
        {
            base.OnRowDataBound(grid, e, dataItem);            
            
            var clickScript = ClientScript.GetPostBackEventReference(Grid, string.Format("Select:{0}", e.Row.RowIndex));
            e.Row.Attributes.Remove("onclick");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("FECHA"), dpFecha.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }
                       };
        }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }
    }
}