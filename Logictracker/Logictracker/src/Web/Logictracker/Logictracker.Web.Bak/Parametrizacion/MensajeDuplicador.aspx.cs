#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionMensajeDuplicador : ApplicationSecuredPage
    {
        #region Private Constants

        private const string MensajesDuplicados = "MENSAJES_DUPLICADOS";

        #endregion

        #region Protected Properties

        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "MENSAJE"; }

        /// <summary>
        /// Saves the duplicated messenges.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            if (lbEmpresa.SelectedValues.Contains(0) && lbLinea.SelectedValues.Contains(0)) return;

            var messenges = (from mens in Mensajes select DAOFactory.MensajeDAO.FindById(mens)).ToList();

            foreach (var mensaje in messenges)
            {
                if (!lbEmpresa.SelectedValues.Contains(0)) DuplicarMensajePorEmpresa(mensaje);

                if (lbLinea.SelectedValues.Contains(0)) continue;

                DuplicarMensajePorLinea(mensaje);
            }

            infoLabel1.Text = CultureManager.GetError(MensajesDuplicados);
        }

        #endregion

        #region Private Methods

        private IEnumerable<int> Mensajes
        {
            get
            {
                if (Session["Mensajes"] != null)
                {
                    ViewState["Mensajes"] = Session["Mensajes"];

                    Session["Mensajes"] = null;
                }
                return (IEnumerable<int>)ViewState["Mensajes"];
            }
        }

        private void DuplicarMensajePorLinea(Mensaje mensaje)
        {
            foreach (var duplicado in from Linea linea in SelectedLines()
                                      where DAOFactory.MensajeDAO.GetByCodigo(mensaje.Codigo, linea.Empresa, linea) == null
                                      select new Mensaje
                                                 {
                                                     Acceso = mensaje.Acceso,
                                                     Origen = mensaje.Origen,
                                                     TipoMensaje = mensaje.TipoMensaje,
                                                     Revision = mensaje.Revision,
                                                     Codigo = mensaje.Codigo,
                                                     Ttl = mensaje.Ttl,
                                                     Descripcion = mensaje.Descripcion,
                                                     Destino = mensaje.Destino,
                                                     Texto = mensaje.Texto,
                                                     EsAlarma = mensaje.EsAlarma,
                                                     Icono = mensaje.Icono,
                                                     EsBaja = mensaje.EsBaja,
                                                     Empresa = linea.Empresa,
                                                     Linea = linea
                                                 })
            {
                AsignarTipoMensaje(mensaje, duplicado);
                DAOFactory.MensajeDAO.SaveOrUpdate(duplicado);
            }
        }

        private void DuplicarMensajePorEmpresa(Mensaje mensaje)
        {
            foreach (var duplicado in from Empresa empresa in SelectedLocations()
                                      where DAOFactory.MensajeDAO.GetByCodigo(mensaje.Codigo, empresa, null) == null
                                      select new Mensaje
                                                 {
                                                     Acceso = mensaje.Acceso,
                                                     Origen = mensaje.Origen,
                                                     Revision = mensaje.Revision,
                                                     Codigo = mensaje.Codigo,
                                                     Ttl = mensaje.Ttl,
                                                     Descripcion = mensaje.Descripcion,
                                                     Destino = mensaje.Destino,
                                                     Texto = mensaje.Texto,
                                                     EsAlarma = mensaje.EsAlarma,
                                                     Icono = mensaje.Icono,
                                                     EsBaja = mensaje.EsBaja,
                                                     Empresa = empresa,
                                                     Linea = null
                                                 })
            {
                AsignarTipoMensaje(mensaje, duplicado);
                DAOFactory.MensajeDAO.SaveOrUpdate(duplicado);
            }
        }

        private void AsignarTipoMensaje(Mensaje mensaje, Mensaje duplicado)
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var tipos = DAOFactory.TipoMensajeDAO.FindByEmpresaLineaYUsuario(duplicado.Empresa, duplicado.Linea, user);

            var type = (from TipoMensaje tipo in tipos where tipo.Descripcion.Equals(mensaje.TipoMensaje.Descripcion) select tipo).
                FirstOrDefault();

            if (type == null)
            {
                type = new TipoMensaje
                           {
                               Baja = false,
                               Codigo = mensaje.TipoMensaje.Codigo,
                               DeCombustible = mensaje.TipoMensaje.DeCombustible,
                               DeEstadoLogistico = mensaje.TipoMensaje.DeEstadoLogistico,
                               DeMantenimiento = mensaje.TipoMensaje.DeMantenimiento,
                               DeUsuario = mensaje.TipoMensaje.DeUsuario,
                               Descripcion = mensaje.TipoMensaje.Descripcion,
                               Empresa = duplicado.Empresa,
                               EsGenerico = mensaje.TipoMensaje.EsGenerico,
                               Icono = mensaje.TipoMensaje.Icono,
                               Linea = null
                           };
                DAOFactory.TipoMensajeDAO.SaveOrUpdate(type);
            }
            duplicado.TipoMensaje = type;
        }

        private IEnumerable<Empresa> SelectedLocations()
        {
            var empresas = (from empresa in lbEmpresa.SelectedValues select DAOFactory.EmpresaDAO.FindById(empresa)).ToList();

            var lineas = SelectedLines();

            if (lineas == null) return empresas;

            return (from e in empresas where !(from Linea linea in lineas select linea.Empresa.Id).Contains(e.Id) select e)
                .ToList();
        }

        private IEnumerable<Linea> SelectedLines()
        {
            return lbLinea.SelectedValues.Contains(0) ? null : (from linea in lbLinea.SelectedValues select DAOFactory.LineaDAO.FindById(linea)).ToList();
        }

        #endregion
    }
}
