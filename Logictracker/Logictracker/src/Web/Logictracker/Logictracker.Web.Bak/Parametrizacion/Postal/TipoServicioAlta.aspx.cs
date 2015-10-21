#region Usings

using System;
using Logictracker.Types.BusinessObjects.Postal;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion.Postal
{
    public partial class TipoServicioAlta : SecuredAbmPage<TipoServicio>
    {
        #region Protected Properties

        /// <summary>
        /// Report title.
        /// </summary>
        protected override string VariableName { get { return "PAR_TIPO_SERVICIO"; } }

        /// <summary>
        /// Associated list page url.
        /// </summary>
        protected override string RedirectUrl { get { return "TipoServicioLista.aspx"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "TIPOSERVICIO"; }

        /// <summary>
        /// Binds the data associated to the service type currently beeing edited.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtDescri.Text = EditObject.Descripcion;
            txtDescriCorta.Text = EditObject.DescripcionCorta;
            ddlReferencia.SelectedIndex = EditObject.ConReferencia != null ? EditObject.ConReferencia.Value : 0;
            ddlLaterales.SelectedIndex = EditObject.ConLaterales != null ? EditObject.ConLaterales.Value : 0;
            ddlGPS.SelectedIndex = EditObject.ConGps != null ? EditObject.ConGps.Value : 0;
            ddlFoto.SelectedIndex = EditObject.ConFoto != null ? EditObject.ConFoto.Value : 0;
            ddlValidacion.SelectedIndex = EditObject.ConAcuse != null ? EditObject.ConAcuse.Value : 0;
        }

        /// <summary>
        /// Deletes the current service type.
        /// </summary>
        protected override void OnDelete() { DAOFactory.TipoServicioDAO.Delete(EditObject); }

        /// <summary>
        /// Saves all changes made to the current object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescri.Text;
            EditObject.DescripcionCorta = txtDescriCorta.Text;
            EditObject.ConReferencia = Convert.ToInt16(ddlReferencia.SelectedValue);
            EditObject.ConLaterales = Convert.ToInt16(ddlLaterales.SelectedValue);
            EditObject.ConGps = Convert.ToInt16(ddlGPS.SelectedValue);
            EditObject.ConFoto = Convert.ToInt16(ddlFoto.SelectedValue);
            EditObject.ConAcuse = Convert.ToInt16(ddlValidacion.SelectedValue);
            EditObject.FechaModificacion = DateTime.UtcNow;

            DAOFactory.TipoServicioDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Perfoms validations prior to saving the edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtDescri.Text, "DESCRIPCION");
            ValidateEmpty(txtDescriCorta.Text, "DESCRIPCION_CORTA");

            var byCodigo = DAOFactory.TipoServicioDAO.FindByCode(codigo);
            ValidateDuplicated(byCodigo, "CODE");
        }

        #endregion
    }
}
