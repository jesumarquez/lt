using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

namespace Logictracker.Reportes.M2M
{
    public partial class AnalizadorMediciones : SecuredGridReportPage<MedicionTourVo>
    {
        protected override string VariableName { get { return "ANALIZADOR_MEDICIONES"; } }
        protected override string GetRefference() { return "ANALIZADOR_MEDICIONES"; }
        protected override bool ExcelButton { get { return true; } }
        
        protected override List<MedicionTourVo> GetResults()
        {
            var desde = dpInitDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpEndDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var codigoInicio = ddlMensajeOrigen.SelectedValue;
            var codigoFin = ddlMenasjeFin.SelectedValue;
            var medidor = new List<int> {ddlSubentidad.Selected};
            var medidores = lbSubentidad.SelectedValues;
            if (medidores.Contains(0))
            {
                medidores = DAOFactory.SubEntidadDAO.GetList(new[] {ddlLocacion.Selected},
                                                             new[] {ddlPlanta.Selected},
                                                             new[] {ddlTipoEntidad.Selected},
                                                             new[] {ddlEntidad.Selected},
                                                             new[] {-1},
                                                             new[] {-1})
                                                    .Where(s => lbSubentidad.TipoMedicion.Split(',').Contains(s.Sensor.TipoMedicion.Codigo))
                                                    .Select(s => s.Id).ToList();
            }

            var inicios = DAOFactory.LogEventoDAO.GetBySubEntitiesAndCodes(medidor,
                                                                           new List<string> {codigoInicio},
                                                                           desde,
                                                                           hasta);
            var finales = DAOFactory.LogEventoDAO.GetBySubEntitiesAndCodes(medidor,
                                                                           new List<string> { codigoFin },
                                                                           desde,
                                                                           hasta);
            var results = new List<MedicionTourVo>();
            foreach (var evento in inicios)
            {
                var cierres = finales.Where(f => f.Fecha > evento.Fecha).OrderBy(f => f.Fecha);
                if (cierres.Any())
                {
                    var reg = DAOFactory.LogEventoDAO.GetMedicionTour(medidores, evento.Fecha, cierres.First().Fecha)
                                                     .Select(m => new MedicionTourVo(m));
                    results.AddRange(reg);
                }
            }
            return results;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack) return;
            dpInitDate.SetDate();
            dpEndDate.SetDate();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI79"), ddlEntidad.SelectedItem.Text},
                           {CultureManager.GetLabel("EVENTO_INICIO"), ddlMensajeOrigen.SelectedItem.Text},
                           {CultureManager.GetLabel("EVENTO_FIN"), ddlMenasjeFin.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpInitDate.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss")},
                           {CultureManager.GetLabel("HASTA"), dpEndDate.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss")}
                       };
        }
    }
}
