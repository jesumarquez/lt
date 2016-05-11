using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Input;

namespace Logictracker.Mantenimiento
{
    public partial class TicketMantenimientoLista : SecuredListPage<TicketMantenimientoVo>
    {
        protected override string VariableName { get { return "MANT_TICKET"; } }
        protected override string GetRefference() { return "MANT_TICKET"; }
        protected override string RedirectUrl { get { return "TicketMantenimientoAlta.aspx"; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ImportButton { get { return true; } }
        protected override string ImportUrl { get { return "TicketMantenimientoImport.aspx"; } }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtDesde.SetDate();
                dtHasta.SetDate();
            }
            base.OnLoad(e);
        }

        protected override List<TicketMantenimientoVo> GetListData()
        {
            return DAOFactory.TicketMantenimientoDAO.GetList(cbTaller.SelectedValues,
                                                             cbEmpresa.SelectedValues,
                                                             cbLinea.SelectedValues,
                                                             cbVehiculo.SelectedValues,
                                                             SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value), 
                                                             SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value))                                                    
                                                    .Select(t => new TicketMantenimientoVo(t))
                                                    .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticVehiculo, cbVehiculo);
            data.LoadStaticFilter(FilterData.StaticTaller, cbTaller);
            data.LoadLocalFilter((string) "DESDE", (DateTimePicker) dtDesde);
            data.LoadLocalFilter((string) "HASTA", (DateTimePicker) dtHasta);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticVehiculo, cbVehiculo.Selected);
            data.AddStatic(FilterData.StaticTaller, cbTaller.Selected);
            data.Add("DESDE", dtDesde.SelectedDate);
            data.Add("HASTA", dtHasta.SelectedDate);
            return data;
        }
    }
}