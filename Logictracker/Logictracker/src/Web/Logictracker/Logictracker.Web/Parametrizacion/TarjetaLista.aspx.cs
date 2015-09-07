using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Empleados;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.Parametrizacion
{
    public partial class TarjetaLista : SecuredListPage<TarjetaVo>
    {
        protected override string RedirectUrl { get { return "TarjetaAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TARJETAS"; } }
        protected override string GetRefference() { return "TARJETA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TarjetaVo> GetListData()
        {
            return (from Tarjeta card
                    in DAOFactory.TarjetaDAO.GetList(ddlLocacion.SelectedValues, ddlPlanta.SelectedValues)
                    let empleado = DAOFactory.EmpleadoDAO.FindByTarjeta(card.Id)
                    select new TarjetaVo(card, empleado)).ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) ddlLocacion.SetSelectedValue((int)empresa);
            if (linea != null) ddlPlanta.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlLocacion.Selected);
            data.AddStatic(FilterData.StaticBase, ddlPlanta.Selected);
            return data;
        }
    }
}
