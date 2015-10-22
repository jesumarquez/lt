using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;
using System;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionDetalleDispositivosLista : SecuredListPage<DetalleDispositivosVo>
    {
        protected override string VariableName { get { return "PAR_LIST_DET_DISPOSITIVO"; } }
        protected override string RedirectUrl { get { return "DispositivoAlta.aspx?t=1"; } }
        protected override string GetRefference() { return "PAR_LIST_DET_DISPOSITIVO"; }

        protected override bool AddButton { get { return false; } }
        protected override bool OpenInNewWindow { get { return true; } }

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Deshabilito el update paner de la masterpage para que no me refresque los filtros salvo que yo lo pida
            // Esto es para que los listbox de dispositivos y parametros no se refresquen y pierdan la posición del
            // scroll cuando se selecciona un valor.
            MasterPage.UpdatePanelFilters.ChildrenAsTriggers = false;
            MasterPage.UpdatePanelFilters.UpdateMode = UpdatePanelUpdateMode.Conditional;
        }
        protected override void FilterChangedHandler(object sender, EventArgs e)
        {
            base.FilterChangedHandler(sender, e);
            MasterPage.UpdatePanelFilters.Update();
        }
        protected void lbDispositivos_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bind();
        }

        protected override List<DetalleDispositivosVo> GetListData()
        {
            var detalles = DAOFactory.DetalleDispositivoDAO.GetDevicesDetail(lbDispositivos.SelectedValues, lbParametros.SelectedValues);

            return (from det in detalles
                    let coche = DAOFactory.CocheDAO.FindMobileByDevice(det.Dispositivo.Id)
                    select new DetalleDispositivosVo(det, coche != null ? coche.Interno : null))
                .ToList();
        }

        
        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadLocalFilter("TIPO", ddlTipo);
            data.LoadLocalFilter("DISPOSITIVOS", lbDispositivos);
            data.LoadLocalFilter("PARAMETROS", lbParametros);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.Add("TIPO", ddlTipo.Selected);
            data.Add("DISPOSITIVOS", lbDispositivos.SelectedValues);
            data.Add("PARAMETROS", lbParametros.SelectedValues);
            return data;
        }
    }
}
