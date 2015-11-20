using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.CicloLogistico
{
    public partial class AgendaLista : SecuredListPage<AgendaVehicularVo>
    {
        protected override string RedirectUrl { get { return "AgendaAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_AGENDA"; } }
        protected override string GetRefference() { return "PAR_AGENDA"; }
        
        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
            }
            base.OnLoad(e);
        }

        protected void FilterChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }

        protected override List<AgendaVehicularVo> GetListData()
        {
            var list = DAOFactory.AgendaVehicularDAO.GetList(cbEmpresa.SelectedValues,
                                                             cbLinea.SelectedValues,
                                                             cbDepartamento.SelectedValues,
                                                             cbMovil.SelectedValues,
                                                             dtpDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                             dtpHasta.SelectedDate.Value.ToDataBaseDateTime());

            return list.Select(a => new AgendaVehicularVo(a)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticDepartamento, cbDepartamento);
            data.LoadLocalFilter("DESDE", dtpDesde);
            data.LoadLocalFilter("HASTA", dtpHasta);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticDepartamento, cbDepartamento.Selected);
            data.Add("DESDE", dtpDesde.SelectedDate);
            data.Add("HASTA", dtpHasta.SelectedDate);
            return data;
        }
    }
}
