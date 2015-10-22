#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTanqueAlta : SecuredAbmPage<Tanque>
    {
        #region Protected Properties

        /// <summary>
        /// Associated list page.
        /// </summary>
        protected override string RedirectUrl { get { return "TanqueLista.aspx"; } }

        /// <summary>
        /// Report Name.
        /// </summary>
        protected override string VariableName { get { return "PAR_TANQUE"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Stablished text changed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            npPorcent.ValueChanged += refresh_Difference;
        }

        /// <summary>
        /// Validates current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (String.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            //try
            //{
            //    Convert.ToDouble(txtCapacidad.Text);

            //    if (txtStockRepo.Text != String.Empty ) Convert.ToDouble(txtStockRepo.Text);
            //    if (txtStockCritico.Text != String.Empty) Convert.ToDouble(txtStockCritico.Text);
            //    if (txtAguaMin.Text != String.Empty) Convert.ToDouble(txtAguaMin.Text);
            //    if (txtAguaMax.Text != String.Empty) Convert.ToDouble(txtAguaMax.Text);
            //    if (txtAguaMax.Text != String.Empty) Convert.ToDouble(txtAguaMax.Text);
            //    if (txtSinReportar.Text != string.Empty) Convert.ToInt32(txtSinReportar.Text);
            //}
            //catch (Exception) { throw new Exception(CultureManager.GetError("MUST_ENTER_NUMBER_VALUE")); }
        }

        /// <summary>
        /// Saves current object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Capacidad = npCapacidad.Value;
            EditObject.Descripcion = txtDescripcion.Text;
            if(EditObject.Codigo == null) EditObject.Codigo = String.Empty;
            EditObject.StockReposicion = !npStockRepo.Value.Equals(0) ? (Double?) npStockRepo.Value : null;
            EditObject.StockCritico = !npStockCritico.Value.Equals(0) ? (Double?) npStockCritico.Value : null;
            EditObject.StockReposicion = !npStockRepo.Value.Equals(0) ? (Double?) npStockRepo.Value: null;
            EditObject.AguaMin = !npAguaMin.Value.Equals(0) ? (Double?) npAguaMin.Value : null;
            EditObject.AguaMax = !npAguaMax.Value.Equals(0) ? (Double?) npAguaMax.Value: null;
            EditObject.TiempoSinReportar = Convert.ToInt32(npSinReportar.Value);
            EditObject.PorcentajeRealVsTeorico = Convert.ToInt32(npPorcent.Value);

            DAOFactory.TanqueDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Deletes current edited object.
        /// </summary>
        protected override void OnDelete() { }

        /// <summary>
        /// Binds current edited object.
        /// </summary>
        protected override void Bind()
        {
            if (EditObject.Capacidad > 0 ) npCapacidad.Value = EditObject.Capacidad;

            txtDescripcion.Text = EditObject.Descripcion;
            npStockCritico.Value = EditObject.StockCritico.HasValue ? Convert.ToDouble(EditObject.StockCritico) : 0;
            npStockRepo.Value = EditObject.StockReposicion.HasValue ?  Convert.ToDouble(EditObject.StockReposicion) : 0;
            npAguaMin.Value = EditObject.AguaMin.HasValue ?  Convert.ToDouble(EditObject.AguaMin ): 0;
            npAguaMax.Value = EditObject.AguaMax.HasValue ? Convert.ToDouble(EditObject.AguaMax ): 0;
            npSinReportar.Value = EditObject.TiempoSinReportar.HasValue ? Convert.ToDouble(EditObject.TiempoSinReportar) : 0;

            npPorcent.Value = EditObject.PorcentajeRealVsTeorico.HasValue ? (int) EditObject.PorcentajeRealVsTeorico : 0;

            txtMaxVolRealvsTeorico.Text = EditObject.PorcentajeRealVsTeorico.HasValue ? ((int) EditObject.PorcentajeRealVsTeorico*EditObject.Capacidad/100).ToString() : string.Empty;
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "TANQUE"; }

        /// <summary>
        /// Adds actions buttons.
        /// </summary>
        protected override void AddToolBarIcons()
        {
            var module = Module;

            if (module == null) return;

            if (module.Edit) ToolBar.AddSaveToolbarButton();

            ToolBar.AddListToolbarButton();
        }
    
        /// <summary>
        /// Refreshes the Volume Difference TextBox between Real And Teoric Volume.
        /// Based in npPorcent and txtCapacidad values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void refresh_Difference(object sender, EventArgs e)
        {
            if (!IsPostBack) return;

            try
            {
                EditObject.Capacidad = npCapacidad.Value;
                EditObject.PorcentajeRealVsTeorico = Convert.ToInt32(npPorcent.Value);
            }
            catch (Exception) { txtMaxVolRealvsTeorico.Text = @"Error"; }

            txtMaxVolRealvsTeorico.Text = EditObject.PorcentajeRealVsTeorico.HasValue ? ((int)EditObject.PorcentajeRealVsTeorico * EditObject.Capacidad / 100).ToString() : string.Empty;
        }

        #endregion
    }
}