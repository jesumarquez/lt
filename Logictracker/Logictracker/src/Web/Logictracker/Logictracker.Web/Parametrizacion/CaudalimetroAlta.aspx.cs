#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionCaudalimetroAlta : SecuredAbmPage<Caudalimetro>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_CAUDALIMETRO"; } }
        protected override string RedirectUrl { get { return "CaudalimetroLista.aspx"; } }
        protected override string GetRefference() { return "CAUDALIMETRO"; }
        protected override bool AddButton { get { return false; } }
        protected override bool DuplicateButton { get { return false; } }
        protected override bool DeleteButton { get { return false; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Validates current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (npCaudalMaximo.Value < 0 || npSinReportar.Value < 0) ThrowError("MUST_ENTER_NUMBER_VALUE");
        }

        /// <summary>
        /// Saves current object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.CaudalMaximo = npCaudalMaximo.Value > 0 ? Convert.ToDouble(npCaudalMaximo.Value) : 0;
            EditObject.TiempoSinReportar = Convert.ToInt32(npSinReportar.Value > 0 ? npSinReportar.Value : 0);
            EditObject.Descripcion = txtDescripcion.Text;

            DAOFactory.CaudalimetroDAO.SaveOrUpdate(EditObject);
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
            txtDescripcion.Text = EditObject.Descripcion;
            npCaudalMaximo.Value = (int)EditObject.CaudalMaximo;
            npSinReportar.Value = EditObject.TiempoSinReportar;
        }
        #endregion
    }
}
