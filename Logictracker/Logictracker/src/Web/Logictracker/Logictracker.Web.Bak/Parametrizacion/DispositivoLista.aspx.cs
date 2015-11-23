using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class DispositivoLista : SecuredListPage<DispositivoVo>
    {
        protected override string VariableName { get { return "PAR_DISPOSITIVOS"; } }
        protected override string RedirectUrl { get { return "DispositivoAlta.aspx"; } }
        protected override string GetRefference() { return "DISPOSITIVO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<DispositivoVo> GetListData()
        {
            return DAOFactory.DispositivoDAO.GetList(ddlLocacion.SelectedValues, ddlPlanta.SelectedValues,
                                                  ddlTipoDispositivo.SelectedValues, chkSoloSinAsignar.Checked)
                    .Select(d => new DispositivoVo(d, DAOFactory.CocheDAO.FindMobileByDevice(d.Id)))
                    .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, DispositivoVo dataItem)
        {
            switch (dataItem.Estado)
            {
                case Dispositivo.Estados.Activo: e.Row.BackColor = Color.FromArgb(152, 251, 152); break;
                case Dispositivo.Estados.EnMantenimiento: e.Row.BackColor = Color.FromArgb(238, 221, 130); break;
                case Dispositivo.Estados.Inactivo: e.Row.BackColor = Color.FromArgb(205, 92, 92); break;
            }
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, ddlLocacion);
            data.LoadStaticFilter(FilterData.StaticBase, ddlPlanta);
            data.LoadStaticFilter(FilterData.StaticTipoDispositivo, ddlTipoDispositivo);
            data.LoadLocalFilter((string) "SIN_ASIGNAR", (CheckBox) chkSoloSinAsignar);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlLocacion.Selected);
            data.AddStatic(FilterData.StaticBase, ddlPlanta.Selected);
            data.AddStatic(FilterData.StaticTipoDispositivo, ddlTipoDispositivo.Selected);
            data.Add("SIN_ASIGNAR", chkSoloSinAsignar.Checked);
            return data;
        }
    }
}