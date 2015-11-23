using C1.Web.UI.Controls.C1GridView;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class VerificadorEmpleados : SecuredGridReportPage<VerificadorEmpleadoVo>
    {
        protected override string VariableName { get { return "VERIF_EMPLEADOS"; } }
        protected override string GetRefference() { return "VERIF_EMPLEADOS"; }
        protected override bool ExcelButton { get { return true; } }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }

        public override C1GridView Grid { get { return grid; } }
        protected override InfoLabel NotFound { get { return infoLabel1; } }
        protected override ToolBar ToolBar { get { return ToolBar1; } }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }
        protected override ResourceButton BtnSearch { get { return btnActualizar; } }
        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }
        protected override C1GridView GridPrint { get {return gridPrint;}}
        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }
        protected override Repeater PrintFilters { get { return FiltrosPrint; } }
        public override int PageSize { get { return 10000; } }
        public override string SearchString { get { return txtBuscar.Text; } }

        protected override List<VerificadorEmpleadoVo> GetResults()
        {
            return ReportFactory.VerificadorEmpleadoDAO.GetVerificadorEmpleados(cbEmpresa.SelectedValues,
                                                                                cbLinea.SelectedValues,
                                                                                lstCentroCosto.SelectedValues,
                                                                                lstDepartamento.SelectedValues,
                                                                                lstCategoriaAcceso.SelectedValues,
                                                                                txtLegajo.Text.Trim(),
                                                                                lstTipoZonaAcceso.SelectedValues,
                                                                                lstZonaAcceso.SelectedValues,
                                                                                lstPuerta.SelectedValues)
                                                       .Select(ver => new VerificadorEmpleadoVo(ver, (int)npHorasFichada.Value)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grilla, C1GridViewRowEventArgs e, VerificadorEmpleadoVo dataItem)
        {
            base.OnRowDataBound(grid, e, dataItem);

            e.Row.Cells[VerificadorEmpleadoVo.IndexEnPeriodo].Text = dataItem.EnPeriodo ? CultureManager.GetLabel("SI") : CultureManager.GetLabel("NO");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {  
                           {CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text }, 
                           {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text },
                           {CultureManager.GetLabel("HORAS_FICHADA"), npHorasFichada.Value.ToString("#0")}
                       };
        }
    }
}
