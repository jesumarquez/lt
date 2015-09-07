#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionConfiguracionDispositivoAlta : SecuredAbmPage<ConfiguracionDispositivo>
    {
        protected override string VariableName { get { return "PAR_CONFIG_DISPOSITIVO"; } }
        protected override string RedirectUrl { get { return "ConfiguracionDispositivoLista.aspx"; } }
        protected override string GetRefference() { return "CONFIGURACION"; }

        protected override void Bind()
        {
            tbNombre.Text = EditObject.Nombre;
            tbDescripcion.Text = EditObject.Descripcion;
            tbConfiguracion.Text = EditObject.Configuracion;
            npOrden.Value = EditObject.Orden;
        }

        protected override void OnSave()
        {
            EditObject.Nombre = tbNombre.Text;
            EditObject.Descripcion = tbDescripcion.Text;
            EditObject.Configuracion = tbConfiguracion.Text;
            EditObject.Orden = (Int32)npOrden.Value;

            DAOFactory.ConfiguracionDispositivoDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnDelete()
        {
            try
            {
                //TODO: Eliminar el try..catch y remplazarlo por una validación como la gente. El error puede mostrar los codigos de los elementos que tienen asignada esta configuración
                DAOFactory.ConfiguracionDispositivoDAO.Delete(EditObject);
            }
            catch(Exception ex)
            {
                if(ex.InnerException == null) throw;
                if (ex.InnerException.Message.Contains("dbo.par.par_enti_51_configuraciones_tipo_dispositivo"))
                    ThrowCantDelete("Entities", "PARENTI32");
                else if (ex.InnerException.Message.Contains("par.par_enti_50_asignacion_configuraciones"))
                    ThrowCantDelete("Entities", "PARENTI08");
                else throw;
            }
        }

        protected override void ValidateSave()
        {
            ValidateEmpty(tbNombre.Text, "NAME");
            ValidateEmpty(tbConfiguracion.Text, "CONFIGURACION");
        }
    }
}

