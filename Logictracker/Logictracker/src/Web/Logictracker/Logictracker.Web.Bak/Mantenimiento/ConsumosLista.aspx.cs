using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Input;

namespace Logictracker.Mantenimiento
{
    public partial class ConsumosLista : SecuredListPage<ConsumoVo>
    {
        protected override string RedirectUrl { get { return "ConsumoAlta.aspx"; } }
        protected override string ImportUrl { get { return "ConsumoImport.aspx"; } }
        protected override string VariableName { get { return "MAN_CONSUMOS"; } }
        protected override string GetRefference() { return "MAN_CONSUMOS"; }
        protected override bool ImportButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        
        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
            }
            base.OnLoad(e);
        }

        protected override List<ConsumoVo> GetListData()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dtpDesde.SelectedDate.Value);
            var hasta = SecurityExtensions.ToDataBaseDateTime(dtpHasta.SelectedDate.Value);

            var list = DAOFactory.ConsumoDetalleDAO.GetList(cbEmpresa.SelectedValues,
                                                             cbLinea.SelectedValues,
                                                             cbTransportista.SelectedValues,
                                                             new[] {-1},
                                                             new[] {-1},
                                                             cbTipoVehiculo.SelectedValues,
                                                             cbMovil.SelectedValues,
                                                             new[] {-1},
                                                             new[] {-1},
                                                             cbTipoProveedor.SelectedValues,
                                                             cbProveedor.SelectedValues,
                                                             cbDepositoOrigen.SelectedValues,
                                                             cbDepositoDestino.SelectedValues,
                                                             desde,
                                                             hasta,
                                                             new[] {-1})
                                                    .Where(c => c.ConsumoCabecera.Estado != ConsumoCabecera.Estados.Eliminado);
            

            return list.Select(v => new ConsumoVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTransportista, cbTransportista);
            data.LoadStaticFilter(FilterData.StaticVehiculo, cbMovil);
            data.LoadStaticFilter(FilterData.StaticTipoProveedor, cbTipoProveedor);
            data.LoadStaticFilter(FilterData.StaticProveedor, cbProveedor);
            data.LoadStaticFilter(FilterData.StaticDeposito, cbDepositoOrigen);
            data.LoadLocalFilter((string) "DESDE", (DateTimePicker) dtpDesde);
            data.LoadLocalFilter((string) "HASTA", (DateTimePicker) dtpHasta);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTransportista, cbTransportista.Selected);
            data.AddStatic(FilterData.StaticVehiculo, cbMovil.Selected);
            data.AddStatic(FilterData.StaticTipoProveedor, cbTipoProveedor.Selected);
            data.AddStatic(FilterData.StaticProveedor, cbProveedor.Selected);
            data.AddStatic(FilterData.StaticDeposito, cbDepositoOrigen.Selected);
            data.Add("DESDE", dtpDesde.SelectedDate);
            data.Add("HASTA", dtpHasta.SelectedDate);
            return data;
        }

        #endregion
    }
}
