using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BaseControls;

namespace Logictracker.App_Controls
{
    public partial class AppControlsAltaEntidad : BaseUserControl
    {
        #region Protected Properties

        /// <summary>
        /// The entity been edited.
        /// </summary>
        private Entidad Entidad
        {
            get { return ViewState["Entidad"] == null ? new Entidad() : (Entidad) ViewState["Entidad"]; }
            set { ViewState["Entidad"] = value; }
        }

        public bool ShowDir
        {
            get { return (bool)(ViewState["ShowDir"] ?? true); }
            set { ViewState["ShowDir"] = value; }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEntidad();
                DireccionSearch1.Visible = ShowDir;
            }
        }

        #region Public Methods

        /// <summary>
        /// Sets the initial entity.
        /// </summary>
        /// <param name="entidad">The entity to be modified.</param>
        public void SetEntidad(Entidad entidad) { Entidad = entidad; }

        /// <summary>
        /// Gets the actual entity.
        /// </summary>
        /// <param name="isDuplicate">Determines if the entity is a clone of an existing one.</param>
        /// <returns></returns>
        public Entidad GetEntidad(bool isDuplicate)
        {
            var entidad = isDuplicate ? new Entidad() : Entidad;

            ValidateEntity();

            entidad.Apellido = txtApellido.Text;
            entidad.Nombre = txtNombre.Text;
            entidad.NroDocumento = txtDocumento.Text;
            entidad.TipoDocumento = txtTipo.Text;
            entidad.Cuil = txtCuil.Text;

            if (DireccionSearch1.Selected != null)
            {
                if (entidad.Direccion == null)
                    entidad.Direccion = new Direccion { Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                entidad.Direccion.CloneData(DireccionSearch1.Selected);
            }
            else entidad.Direccion = null;

            if (isDuplicate)
            {
                entidad.Id = 0;

                if (entidad.Direccion != null)
                {
                    //entidad.Direccion.Id = 0;
                    var dir = new Direccion { Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    dir.CloneData(entidad.Direccion);
                    entidad.Direccion = dir;
                }
            }

            return entidad;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the entity data.
        /// </summary>
        private void ValidateEntity()
        {
            if (txtNombre.Text == string.Empty) throw new Exception(string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetLabel("FULLNAME")));
        }

        /// <summary>
        /// Binds a entity.
        /// </summary>
        private void BindEntidad()
        {
            if (Entidad.Id <= 0) return;

            txtTipo.Text = Entidad.TipoDocumento;
            txtApellido.Text = Entidad.Apellido;
            txtNombre.Text = Entidad.Nombre;
            txtDocumento.Text = Entidad.NroDocumento;
            txtCuil.Text = Entidad.Cuil;
            DireccionSearch1.SetDireccion(Entidad.Direccion);
        }

        #endregion
    }
}
