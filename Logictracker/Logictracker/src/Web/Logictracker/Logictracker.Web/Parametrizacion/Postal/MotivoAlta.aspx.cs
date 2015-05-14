#region Usings

using System;
using Logictracker.Types.BusinessObjects.Postal;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion.Postal
{
    public partial class MotivoAlta : SecuredAbmPage<Motivo>
    {
        #region Protected Properties

        /// <summary>
        /// Report title.
        /// </summary>
        protected override string VariableName { get { return "PAR_MOTIVOS"; } }

        /// <summary>
        /// Associated list page url.
        /// </summary>
        protected override string RedirectUrl { get { return "MotivoLista.aspx"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "MOTIVO"; }

        /// <summary>
        /// Binds the data associated to the reason beeing edited.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            npOrden.Value = EditObject.Orden;
            cbGestion.Checked = EditObject.EsGestion != null ? EditObject.EsGestion.Value : false;
            cbEntrega.Checked = EditObject.EsEntrega != null ? EditObject.EsEntrega.Value : false;
            cbDevolucion.Checked = EditObject.EsDevolucion != null ? EditObject.EsDevolucion.Value : false;
        }

        /// <summary>
        /// Deletes the current object.
        /// </summary>
        protected override void OnDelete() { DAOFactory.MotivoDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Orden = Convert.ToInt32(npOrden.Value);
            EditObject.EsGestion = cbGestion.Checked;
            EditObject.EsEntrega = cbEntrega.Checked;
            EditObject.EsDevolucion = cbDevolucion.Checked;
            EditObject.FechaModificacion = DateTime.UtcNow;
            EditObject.Codigo = txtCodigo.Text;

            DAOFactory.MotivoDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Performs validation prior to saving the changes made to the current object.
        /// </summary>
        protected override void ValidateSave()
        {
            ValidateEmpty("txtDescripcion.Text", "DESCRIPCION");
            var codigo = ValidateEmpty("txtCodigo.Text", "CODE");
            if (!cbDevolucion.Checked && !cbEntrega.Checked && !cbGestion.Checked) 
                ThrowMustEnter("SERVICE_TYPE_ACTION");

            var byCodigo = DAOFactory.MotivoDAO.FindByCodigo(codigo);
            ValidateDuplicated(byCodigo, "CODE");
        }

        #endregion
    }
}
