#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_FirmwareAlta : SecuredAbmPage<Firmware>
    {
        #region Private Const Properties

        /// <summary>
        /// Binary variable name.
        /// </summary>
        private const string BINARIO = "BINARIO";

        /// <summary>
        /// Description variable name.
        /// </summary>
        private const string DESCRIPCION = "DESCRIPCION";

        /// <summary>
        /// Signature variable name.
        /// </summary>
        private const string FIRMA = "FIRMA";

        /// <summary>
        /// Name variable name.
        /// </summary>
        private const string NAME = "Name";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "PAR_FIRMWARE"; } }
        protected override string RedirectUrl { get { return "FirmwareLista.aspx"; } }
        protected override string GetRefference() { return "FIRMWARE"; }

        #endregion

        #region Protected Method

        /// <summary>
        /// Binds the current edited object.
        /// </summary>
        protected override void Bind()
        {
            tbNombre.Text = EditObject.Nombre;
            tbDescripcion.Text = EditObject.Descripcion;
            tbFirma.Text = EditObject.Firma;
        }

        /// <summary>
        /// Saves the current edited object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Nombre = tbNombre.Text;
            EditObject.Descripcion = tbDescripcion.Text;
            EditObject.Firma = tbFirma.Text;
            if (EditObject.Id == 0) EditObject.Fecha = DateTime.Now;
            if(fuBinario.HasFile) EditObject.Binario = fuBinario.FileBytes;

            DAOFactory.FirmwareDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnInit(EventArgs e)
        {
            ToolBar.AsyncPostBacks = false;

            base.OnInit(e);
        }

        /// <summary>
        /// Deletes the current edited object.
        /// </summary>
        protected override void OnDelete() { DAOFactory.FirmwareDAO.Delete(EditObject); }

        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(tbNombre.Text)) ThrowMustEnter(NAME);

            if (string.IsNullOrEmpty(tbDescripcion.Text)) ThrowMustEnter(DESCRIPCION);

            if (string.IsNullOrEmpty(tbFirma.Text)) ThrowMustEnter(FIRMA);

            if (EditObject.Binario == null && !fuBinario.HasFile) ThrowMustEnter(BINARIO);
        }

        #endregion
    }
}
