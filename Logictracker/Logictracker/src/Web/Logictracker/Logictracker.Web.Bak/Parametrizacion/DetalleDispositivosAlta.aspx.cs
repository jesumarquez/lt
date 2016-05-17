#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.ValueObject.Dispositivos;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionDetalleDispositivosAlta : SecuredAbmPage<Dispositivo>
    {
        #region Private Const Properties

        /// <summary>
        /// Columns constant values.
        /// </summary>
        private const int Template = 4; //the index of template column
        private const int IdDetail = 5; //the index of the DetailID column

        #endregion

        #region Protected Properties

        /// <summary>
        /// Report Name.
        /// </summary>
        protected override string VariableName { get { return "PAR_DET_DISPOSITIVOS"; } }

        /// <summary>
        /// Sets the url for redirection
        /// </summary>
        protected override string RedirectUrl { get { return "DetalleDispositivosLista.aspx"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds the Data from the ViewState
        /// </summary>
        protected override void Bind()
        {
            lblTipo.Text = EditObject.TipoDispositivo != null ? EditObject.TipoDispositivo.Modelo : string.Empty;
            lblIMEI.Text = EditObject.Imei;
            lblCodigo.Text = EditObject.Codigo;
            lblFirm.Text = EditObject.Firmware != null ? EditObject.Firmware.Nombre : String.Empty;

            var detalles = from DetalleDispositivo detalle in EditObject.DetallesDispositivo select new DetalleDispositivosVO(detalle);

            ListObjects = detalles.ToList();

            if (ListObjects.Count > 0)
            {
                Sort();

                gridParametros.PageIndex = 0;
                gridParametros.DataSource = ListObjects;
                gridParametros.DataBind();
            }

            gridParametros.Visible = ListObjects.Count > 0;
        }

        /// <summary>
        /// Adds tooblbar icons according to user privileges.
        /// </summary>
        protected override void AddToolBarIcons()
        {
            ToolBar.AddSaveToolbarButton();

            ToolBar.AddListToolbarButton();
        }

        /// <summary>
        /// Deletes the current device detail.
        /// </summary>
        protected override void OnDelete() { }

        /// <summary>
        /// updates the values of the parameters and increments revision number
        /// </summary>
        protected override void OnSave()
        {
            var dispositivo = EditObject;

            SaveDeviceConfiguration(dispositivo);
        }
    
        /// <summary>
        /// formats the grid to enable value-edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grid_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var detalle = e.Row.DataItem as DetalleDispositivosVO;

            var textbox = e.Row.Cells[Template].FindControl("txtbValor") as TextBox;
            var label = e.Row.Cells[Template].FindControl("lblValor") as Label;

            if (textbox != null)
            {
                textbox.Text = detalle.Valor;
                textbox.Visible = detalle.ParamEditable;
            }

            if (label == null) return;

            label.Text = detalle.Valor;
            label.Visible = !detalle.ParamEditable;
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "DET_DISPO"; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the maximum revision value in a DetalleDispositivos list.
        /// </summary>
        /// <param name="detalles"></param>
        /// <returns></returns>
        private static int MaxRevisionValue(IEnumerable detalles)
        {
            var maxRevision = 0;

            foreach (var detalleDispositivo in detalles.Cast<DetalleDispositivo>().Where(detalleDispositivo => detalleDispositivo.Revision > maxRevision)) 
                maxRevision = detalleDispositivo.Revision;

            return maxRevision;
        }

        /// <summary>
        /// Saves changes to the device configuration
        /// </summary>
        /// <param name="dispositivo"></param>
        private void SaveDeviceConfiguration(Dispositivo dispositivo)
        {
            var maxRevision = MaxRevisionValue(dispositivo.DetallesDispositivo) + 1;

            foreach (C1GridViewRow newParameter in gridParametros.Rows)
            {
                var idDetail = Convert.ToInt32(newParameter.Cells[IdDetail].Text);

                var textbox = newParameter.Cells[Template].FindControl("txtbValor") as TextBox;

                var oldParameter = dispositivo.FindDetailById(idDetail);

                if (textbox == null || oldParameter.Valor == textbox.Text) continue;

                oldParameter.Valor = textbox.Text;
                oldParameter.Revision = maxRevision++;
            }

            DAOFactory.DispositivoDAO.SaveOrUpdate(dispositivo);
        }

        #endregion

        #region Methods and Properties for Sorting

        /// <summary>
        /// Dictionary for column specific IComparer sorting.
        /// </summary>
        private readonly IDictionary<string, ICustomComparer<DetalleDispositivosVO>> _sortDictionary = new Dictionary<string, ICustomComparer<DetalleDispositivosVO>>();

        /// <summary>
        /// Initial sort expression.
        /// </summary>
        private static string InitialSortExpression { get { return "Param"; } }

        /// <summary>
        /// Current sort expression.
        /// </summary>
        private string SortExpression
        {
            get { return ViewState["SortExpression"] != null ? ViewState["SortExpression"].ToString() : InitialSortExpression; }
            set { ViewState["SortExpression"] = value; }
        }

        /// <summary>
        /// Current sort direction.
        /// </summary>
        private C1SortDirection SortDirection
        {
            get { return ViewState["SortDirection"] != null ? (C1SortDirection)ViewState["SortDirection"] : C1SortDirection.Ascending; }
            set { ViewState["SortDirection"] = value; }
        }

        /// <summary>
        ///  Objects list.
        /// </summary>
        private List<DetalleDispositivosVO> ListObjects
        {
            get { return ViewState["ListObject"] != null ? (List<DetalleDispositivosVO>)ViewState["ListObject"] : new List<DetalleDispositivosVO>(); }
            set { ViewState["ListObject"] = value; }
        }

        /// <summary>
        /// Re-sorts the data displayed at the grid using the new sort criteria.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grid_SortingCommand(object sender, C1GridViewSortEventArgs e)
        {
            SortExpression = e.SortExpression;
            SortDirection = e.SortDirection;

            Sort();
        
            gridParametros.DataSource = ListObjects;
            gridParametros.DataBind();
        }

        /// <summary>
        /// Sorts the ListObjects list
        /// </summary>
        private void Sort() { if (!string.IsNullOrEmpty(SortExpression)) ListObjects.Sort(GetSortComparer()); }

        /// <summary>
        /// Gets the sort comparer to use for the current sort expression.
        /// </summary>
        /// <returns></returns>
        private IComparer<DetalleDispositivosVO> GetSortComparer()
        {
            if (_sortDictionary.ContainsKey(SortExpression))
            {
                var comp = _sortDictionary[SortExpression];

                comp.Descending = SortDirection == C1SortDirection.Descending;

                return comp;
            }

            return new ObjectComparer<DetalleDispositivosVO>(SortExpression, SortDirection == C1SortDirection.Descending);
        }

        #endregion
    }
}
