#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Urbetrack.Common.Utils;
using Urbetrack.Common.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Urbetrack.Common.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;
using Urbetrack.Common.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects.Tickets;
using Urbetrack.Common.Web.CustomWebControls.DropDownLists;
using Urbetrack.Common.Web.CustomWebControls.DropDownLists.ControlDeCombustible;
using Urbetrack.Common.Web.CustomWebControls.ListBoxs;
using Urbetrack.Common.Web.Helpers.CultureHelpers;
using Urbetrack.DAL.Factories;
using Urbetrack.DatabaseTracer.Enums;
using Urbetrack.Types.BusinessObjects;
using Urbetrack.Types.BusinessObjects.ControlDeCombustible;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Messages;
using Urbetrack.Types.BusinessObjects.ReferenciasGeograficas;
using Urbetrack.Types.BusinessObjects.Tickets;
using Urbetrack.Types.BusinessObjects.Vehiculos;

#endregion

namespace Urbetrack.Common.Web.CustomWebControls.Helpers
{
    /// <summary>
    /// Custom controls bindings helper.
    /// </summary>
    public class BindingManager
    {
        #region Private Properties

        private DAOFactory _daof;

        /// <summary>
        /// DAOFactory.
        /// </summary>
        private DAOFactory DaoFactory { get { return _daof ?? (_daof = new DAOFactory()); } }

        /// <summary>
        /// A list of Usuario tipes.
        /// </summary>
        private static IEnumerable<Pair> TiposUsuario
        {
            get
            {
                return new List<Pair>
                                        {
                                            new Pair(NivelAccesoUsuario.NoAccess, CultureManager.GetUser("USERTYPE_NOACCESS")),
                                            new Pair(NivelAccesoUsuario.Public, CultureManager.GetUser("USERTYPE_PUBLIC")),
                                            new Pair(NivelAccesoUsuario.User , CultureManager.GetUser("USERTYPE_USER")),
                                            new Pair(NivelAccesoUsuario.Installer, CultureManager.GetUser("USERTYPE_INSTALLER")),
                                            new Pair(NivelAccesoUsuario.AdminUser,CultureManager.GetUser("USERTYPE_ADMIN_USER")),
                                            new Pair(NivelAccesoUsuario.SysAdmin, CultureManager.GetUser("USERTYPE_SYS_ADMIN")),
                                            new Pair(NivelAccesoUsuario.Developer, CultureManager.GetUser("USERTYPE_DEVELOPER")),
                                            new Pair(NivelAccesoUsuario.SuperAdmin, CultureManager.GetUser("USERTYPE_SUPER_ADMIN"))
                                        };
            }
        }

        /// <summary>
        /// A list of Validation types.
        /// </summary>
        private static IEnumerable<Pair> TiposValidacion
        {
            get
            {
                return new List<Pair>
                                        {
                                            new Pair(0, CultureManager.GetLabel("NUNCA")),
                                            new Pair(1, CultureManager.GetLabel("ENTREGADO")),
                                            new Pair(2, CultureManager.GetLabel("NO_ENTREGADO")),
                                            new Pair(3, CultureManager.GetLabel("SIEMPRE"))
                                        };
            }
        }

        /// <summary>
        /// Function types list.
        /// </summary>
        private static IEnumerable<Pair> TiposFuncion
        {
            get
            {
                return new List<Pair>
                                        {
                                            new Pair(1, CultureManager.GetLabel("FUNCTIONTYPE_ASPX")),
                                            new Pair(2, CultureManager.GetLabel("FUNCTIONTYPE_HTML")),
                                            new Pair(3, CultureManager.GetLabel("FUNCTIONTYPE_JS")),
                                            new Pair(5, CultureManager.GetLabel("FUNCTIONTYPE_OTHER"))
                                        };
            }
        }

        /// <summary>
        /// Vehicle states.
        /// </summary>
        private static IEnumerable<Pair> EstadoCoches
        {
            get
            {
                return new List<Pair>
                           {
                               new Pair(0, new OptionGroupConfiguration
                                       {
                                           OptionGroupDescription = CultureManager.GetLabel("VEHICLESTATE_ACTIVE"),
                                           ImageUrl = "~/images/vehicle_active.png"}),
                                new Pair(1,new OptionGroupConfiguration
                                        {
                                           OptionGroupDescription = CultureManager.GetLabel("VEHICLESTATE_TALLER"),
                                           ImageUrl = "~/images/vehicle_in_manteinance.png"}),
                                new Pair(2, new OptionGroupConfiguration
                                    {
                                        OptionGroupDescription = CultureManager.GetLabel("VEHICLESTATE_INACTIVE"),
                                        ImageUrl = "~/images/vehicle_inactive.png"})
                           };
            }
        }

        /// <summary>
        /// Vehicle states.
        /// </summary>
        private static IEnumerable<Pair> EstadoDispositivos
        {
            get
            {
                return new List<Pair>
                                              {
                                                  new Pair(0, CultureManager.GetLabel("DEVICESTATE_ACTIVE")),
                                                  new Pair(1, CultureManager.GetLabel("DEVICESTATE_REPAIR")),
                                                  new Pair(2, CultureManager.GetLabel("DEVICESTATE_INACTIVE"))
                                              };
            }
        }

        /// <summary>
        /// Logic cycle types.
        /// </summary>
        private static IEnumerable<Pair> TiposLogisticos
        {
            get
            {
                return new List<Pair>
                                           {
                                               new Pair((int)ETAPAS.Normal, CultureManager.GetLabel("EST_LOG_NORMAL")),
                                               new Pair((int)ETAPAS.Iniciado, CultureManager.GetLabel("EST_LOG_INICIADO")),
                                               new Pair((int)ETAPAS.GiroTrompoDerecha, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_DERECHA")),
                                               new Pair((int)ETAPAS.GiroTrompoIzquierda, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_IZQUIERDA")),
                                               new Pair((int)ETAPAS.TolvaActiva, CultureManager.GetLabel("EST_LOG_TOLVA_ACTIVA")),
                                               new Pair((int)ETAPAS.TolvaInactiva, CultureManager.GetLabel("EST_LOG_TOLVA_INACTIVA")),
                                               new Pair((int)ETAPAS.SaleDePlanta, CultureManager.GetLabel("EST_LOG_SALE_PLANTA")),
                                               new Pair((int)ETAPAS.LlegaAObra, CultureManager.GetLabel("EST_LO_LLEGA_OBRA")),
                                               new Pair((int)ETAPAS.SaleDeObra, CultureManager.GetLabel("EST_LOG_SALE_OBRA")),
                                               new Pair((int)ETAPAS.LlegaAPlanta, CultureManager.GetLabel("EST_LO_LLEGA_PLANTA")),
                                               new Pair((int)ETAPAS.GiroTrompoDetenido, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_DETENIDO")),
                                               new Pair((int)ETAPAS.GiroTrompoHorarioLento, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_HOR_LENTO")),
                                               new Pair((int)ETAPAS.GiroTrompoHorarioRapido, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_HOR_RAPIDO")),
                                               new Pair((int)ETAPAS.GiroTrompoAntihorarioLento, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_AH_LENTO")),
                                               new Pair((int)ETAPAS.GiroTrompoAntihorarioRapido, CultureManager.GetLabel("EST_LOG_GIRO_TROMPO_AH_RAPIDO"))
                                           };
            }
        }

        /// <summary>
        /// Message sources.
        /// </summary>
        private static IEnumerable<Pair> OrigenesMensaje
        {
            get
            {
                return new List<Pair>
                                           {
                                               new Pair(0, CultureManager.GetLabel("TARGETTYPE_SYSTEM")),
                                               new Pair(1, CultureManager.GetLabel("TARGETTYPE_MOBILE")),
                                               new Pair(2, CultureManager.GetLabel("TARGETTYPE_ANY"))
                                           };
            }
        }

        /// <summary>
        /// Gets all ticket states
        /// </summary>
        private static Dictionary<short, string> EstadosTicket
        {
            get
            {
                return new Dictionary<short, string>
                                         {
                                             {0, CultureManager.GetLabel("TICKETSTATE_PROGRAMMED")},
                                             {1, CultureManager.GetLabel("TICKETSTATE_CURRENT")},
                                             {9, CultureManager.GetLabel("TICKETSTATE_CLOSED")}
                                         };
            }
        }

        /// <summary>
        /// A list of DetalleCiclo types.
        /// </summary>
        private static Dictionary<short, string> TiposDetalleCiclo
        {
            get
            {
                return new Dictionary<short, string>
                                        {
                                            {DetalleCiclo.TipoTiempo, "Tiempo"},
                                            {DetalleCiclo.TipoEvento, "Evento"},
                                            {DetalleCiclo.TipoEntradaPoi, "Entrada a Referencia Geografica"},
                                            {DetalleCiclo.TipoSalidaPoi, "Salida de Referencia Geografica"},
                                            {DetalleCiclo.TipoCicloLogistico, "Ciclo Logistico"}
                                        };
            }
        }

        #endregion

        #region Public Properties

        public static string AllItemsName { get { return CultureManager.GetControl("DDL_ALL_ITEMS"); } }
        public static string AllItemsValue { get { return "-1"; } }

        #endregion

        #region Public Method

        /// <summary>
        /// Location data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        /// <param name="filter"></param>
        public void BindLocacion(IAutoBindeable autoBindeable, params int[] filter)
        {
            autoBindeable.ClearItems();

            if (autoBindeable.Usuario == null) return;

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var empresas = DaoFactory.EmpresaDAO.FindByUsuario(user);
            if (filter != null && filter.Length > 0)
                empresas = empresas.Where(e => filter.Contains(e.Id)).ToList();

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (var empresa in empresas) autoBindeable.AddItem(empresa.RazonSocial, empresa.Id);
        }

        /// <summary>
        /// Planta Binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        /// <param name="filter"></param>
        public void BindPlanta(IAutoBindeable autoBindeable, params int[] filter)
        {
            autoBindeable.ClearItems();

            var ddlb = autoBindeable.GetParentByType(typeof(Empresa));

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var lineas = (ddlb != null ? DaoFactory.LineaDAO.FindByEmpresasyUsuario(ddlb.SelectedValues, user) : DaoFactory.LineaDAO.FindLineasByUsuario(user)).OfType<Linea>();
            if (filter != null && filter.Length > 0)
                lineas = lineas.Where(l => filter.Contains(l.Id));

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (var linea in lineas) autoBindeable.AddItem(linea.Descripcion, linea.Id);
        }

        /// <summary>
        /// Binds users by the logged user.
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="usuario"></param>
        public void BindUsuarios(UsuarioDropDownList ddl, Usuario usuario)
        {
            foreach (var user in DaoFactory.UsuarioDAO.FindByUsuario(usuario)) ddl.Items.Add(new ListItem(user.NombreUsuario, user.Id.ToString())); 
        }

        /// <summary>
        /// Movil Binding.
        /// </summary>
        public void BindOdometro(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddle = autoBindeable.GetParentByType(typeof(Empresa));
            var ddlb = autoBindeable.GetParentByType(typeof(Linea));
            var ddltv = autoBindeable.GetParentByType(typeof(TipoCoche));

            var linea = ddlb != null && ddlb.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddlb.Selected) : null;
            var empresa = linea != null ? linea.Empresa : ddle != null && ddle.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddle.Selected) : null;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var tipos = ddltv != null ? ddltv.SelectedValues : new List<int> {-1};

            foreach (Odometro odometro in DaoFactory.OdometroDAO.FindByEmpresaYLineaYTipo(empresa, linea, tipos, user)) autoBindeable.AddItem(odometro.Descripcion, odometro.Id);
        }
        
        public void BindLogTypes(LogTypeDropDownList logTypesDropDownList)
        {
            logTypesDropDownList.ClearItems();

            var i = 0;

            if (logTypesDropDownList.AddAllItem) logTypesDropDownList.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (var tipo in Enum.GetNames(typeof (LogTypes)))
            {
                logTypesDropDownList.AddItem(tipo, i);
                i++;
            }
        }

        public void BindLogModules(LogModuleDropDownList logModuleDropDownList)
        {
            logModuleDropDownList.ClearItems();

            var i = 0;

            if (logModuleDropDownList.AddAllItem) logModuleDropDownList.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (var tipo in Enum.GetNames(typeof(LogModules)))
            {
                logModuleDropDownList.AddItem(tipo, i);
                i++;
            }
        }

        /// <summary>
        /// Movil Binding.
        /// </summary>
        public void BindMovil(IMovilAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddle = autoBindeable.GetParentByType(typeof (Empresa));
            var ddlb = autoBindeable.GetParentByType(typeof (Linea));
            var ddltv = autoBindeable.GetParentByType(typeof (TipoCoche));
            var ddlet = autoBindeable.GetParentByType(typeof (Transportista));
            var ddlCli = autoBindeable.GetParentByType(typeof (Cliente));
            var ddlcc = autoBindeable.GetParentByType(typeof (CentroDeCostos));

            if (ddlb == null) return;

            var user = DaoFactory.UsuarioDAO.FindById(ddlb.Usuario.Id);

            var empresas = ddle != null ? ddle.SelectedValues : new List<int> { -1 };
            var lineas = ddlb.SelectedValues;
            var tipos = ddltv != null ? ddltv.SelectedValues : new List<int> { -1 };
            var transportistas = ddlet != null ? ddlet.SelectedValues : new List<int> { -1 };
            var centros = ddlcc != null ? ddlcc.SelectedValues : new List<int> { -1 };
            var clientes = ddlCli != null ? ddlCli.SelectedValues : new List<int> { -1 };

            var coches = DaoFactory.CocheDAO.GetCoches(user, empresas, lineas, tipos, transportistas, clientes, centros);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            var showChofer = autoBindeable.ShowDriverName;

            if (autoBindeable.UseOptionGroup) coches = OrderMobilesByOptionGroupProperty(autoBindeable.OptionGroupProperty, coches);
            
            foreach (Coche movil in coches)
            {
                var desc = showChofer ? (movil.Chofer != null ? movil.Chofer.Entidad.Descripcion : CultureManager.GetControl("DDL_NO_EMPLOYEE")) : movil.Interno;

                var groupDescription = new OptionGroupConfiguration();

                if (autoBindeable.UseOptionGroup) groupDescription = GetMobileGroupDescription(autoBindeable.OptionGroupProperty, movil);

                autoBindeable.AddItem(desc, movil.Id, groupDescription);
            }
        }

        /// <summary>
        /// Messages types binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTipoMensaje(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddld = autoBindeable.GetParentByType(typeof (Empresa));
            var ddlb = autoBindeable.GetParentByType(typeof (Linea));

            var linea = ddlb != null && ddlb.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddlb.Selected) : null;
            var empresa = ddld != null && ddld.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddld.Selected) : linea != null ? linea.Empresa : null;

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            foreach (TipoMensaje tipomensaje in DaoFactory.TipoMensajeDAO.FindByEmpresaLineaYUsuario(empresa, linea,user)) autoBindeable.AddItem(tipomensaje.Descripcion, tipomensaje.Id);
        }

        /// <summary>
        /// Messages binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindMensajes(IMensajeAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddlbtm = autoBindeable.GetParentByType(typeof(TipoMensaje));
            var ddlbl = autoBindeable.GetParentByType(typeof(Linea));
            var ddle = autoBindeable.GetParentByType(typeof(Empresa));

            var tipoUsuario = DaoFactory.UsuarioDAO.GetByNombreUsuario(autoBindeable.Usuario.Name).Tipo;
            var tipoMensaje = ddlbtm != null && ddlbtm.Selected > 0 ? DaoFactory.TipoMensajeDAO.FindById(ddlbtm.Selected) : null;
            var empresa = ddle != null && ddle.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddle.Selected) : null;
            var linea = ddlbl != null && ddlbl.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddlbl.Selected) : null;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var mensajes = DaoFactory.MensajeDAO.FindByTipo(tipoMensaje, empresa, linea,user);

            if (autoBindeable.AddSinMensaje) autoBindeable.AddItem(CultureManager.GetControl("DDL_NO_MESSAGE"), -2);

            var messages = mensajes
                .Where(mensaje => mensaje.Acceso <= tipoUsuario && ((!autoBindeable.SoloMantenimiento || (autoBindeable.SoloMantenimiento && mensaje.TipoMensaje.DeMantenimiento))
                     && (!autoBindeable.SoloCombustible || (autoBindeable.SoloCombustible && mensaje.TipoMensaje.DeCombustible)))).OrderBy(m => m.Descripcion);

            foreach (var mensaje in messages) autoBindeable.AddItem(mensaje.Descripcion, mensaje.Codigo);
        }

        /// <summary>
        /// Messages binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindMensajesEstadosLogisticos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddlp = autoBindeable.GetParentByType(typeof (Linea));
            var ddle = autoBindeable.GetParentByType(typeof (Empresa));

            var emprsa = ddle != null && ddle.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddle.Selected) : null;
            var linea = ddlp != null && ddlp.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddlp.Selected) : null;

            foreach (var mensaje in DaoFactory.MensajeDAO.FindCicloLogistico(emprsa, linea)) autoBindeable.AddItem(mensaje.Descripcion, mensaje.Id);
        }

        /// <summary>
        /// Systems binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindSubSistema(SubSistemaDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (Sistema subSistema in DaoFactory.SistemaDAO.FindAll())
                autoBindeable.Items.Add(new ListItem(CultureManager.GetString("Menu", subSistema.Descripcion), subSistema.Id.ToString()));
        }

        /// <summary>
        /// Sounds binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindSonidos(SonidoDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (Sonido sonido in DaoFactory.SonidoDAO.FindAll()) autoBindeable.Items.Add(new ListItem(sonido.Descripcion, sonido.Id.ToString()));

            autoBindeable.Items.Insert(0, new ListItem(CultureManager.GetControl("DDL_NONE"), ""));
        }

        /// <summary>
        /// Usuario types binging.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindUserTypes(TipoUsuarioDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();
            

            foreach (var pair in TiposUsuario) 
                if(Convert.ToInt16(pair.First) <= autoBindeable.Usuario.AccessLevel)
                    autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), ((short)pair.First).ToString()));
        }

        /// <summary>
        /// Usuario types binging.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTipoValidacion(TipoValidacionDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in TiposValidacion) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }

        /// <summary>
        /// Profiles binding.
        /// </summary>
        /// <param name="lb"></param>
        public void BindProfiles(IAutoBindeable lb)
        {
            lb.ClearItems();

            var user = DaoFactory.UsuarioDAO.FindById(lb.Usuario.Id);

            var perfs = user.Perfiles.IsEmpty ? DaoFactory.PerfilDAO.FindAll() : (from Perfil p in user.Perfiles orderby p.Descripcion select p).ToList();

            foreach (var perfil in perfs) lb.AddItem(perfil.Descripcion, perfil.Id);
        }

        /// <summary>
        /// Funtion types binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindFunctionTypes(TiposFuncionDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in TiposFuncion) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }
      
        /// <summary>
        /// Vehicle types binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindVehicleTypes(IAutoBindeable autoBindeable)
        {
            var ddlEmpresa = autoBindeable.GetParentByType(typeof (Empresa));
            var ddlLinea = autoBindeable.GetParentByType(typeof (Linea));

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            autoBindeable.ClearItems();

            var lineas = ddlLinea != null ? ddlLinea.SelectedValues : new List<int> {-1};
            var empresas = ddlEmpresa != null ? ddlEmpresa.SelectedValues : !lineas.Contains(-1) ? (from int l in lineas select DaoFactory.LineaDAO.FindById(l).Empresa.Id).ToList() : new List<int> { -1 };

            var listaTipos = DaoFactory.TipoCocheDAO.FindByEmpresasAndLineas(empresas, lineas, user);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (TipoCoche tipoCoche in listaTipos) autoBindeable.AddItem(tipoCoche.Descripcion, tipoCoche.Id);
        }
        
        /// <summary>
        /// Address types binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTipoReferenciaGeografica(IAutoBindeable autoBindeable)
        {
            var ddlEmpresa = autoBindeable.GetParentByType(typeof(Empresa));
            var ddlLinea = autoBindeable.GetParentByType(typeof(Linea));

            autoBindeable.ClearItems();

            var linea = ddlLinea != null ? ddlLinea.Selected : -1;
            var empresa = ddlEmpresa != null ? ddlEmpresa.Selected : linea > 0 ? DaoFactory.LineaDAO.FindById(linea).Empresa.Id : -1;

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var listaTipos = DaoFactory.TipoReferenciaGeograficaDAO.FindByEmpresaLineaYUsuario(empresa, linea,user);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            var tipoDomicilios = from tipoDomicilio in listaTipos
                                 let ownlinea = tipoDomicilio.Owner != null && tipoDomicilio.Owner.Linea != null ? tipoDomicilio.Owner.Linea.Id : -1
                                 let ownempresa = tipoDomicilio.Owner != null && tipoDomicilio.Owner.Empresa != null ? tipoDomicilio.Owner.Empresa.Id : -1
                                    where autoBindeable.FilterMode || empresa != -1 || linea != -1 || (ownempresa == -1 && ownlinea == -1)
                                 where autoBindeable.FilterMode || linea != -1 || ownlinea == -1
                                 select tipoDomicilio;

            foreach (var tipoDomicilio in tipoDomicilios) autoBindeable.AddItem(tipoDomicilio.Descripcion, tipoDomicilio.Id);
        }

        /// <summary>
        /// Address types binding.
        /// </summary>
        /// <param name="lb"></param>
        public void BindTipoDomicilio(TipoReferenciaGeograficaListBox lb)
        {
            var ddlEmpresa = lb.GetParentByType(typeof(Empresa));
            var ddlLinea = lb.GetParentByType(typeof(Linea));

            lb.Items.Clear();

            var empresa = ddlEmpresa != null ? ddlEmpresa.Selected : -1;
            var linea = ddlLinea != null ? ddlLinea.Selected : -1;
            var user = DaoFactory.UsuarioDAO.FindById(lb.Usuario.Id);
            var tipos = DaoFactory.TipoReferenciaGeograficaDAO.FindByEmpresaLineaYUsuario(empresa, linea,user);

            foreach (var tipoDomicilio in tipos) lb.Items.Add(new ListItem(tipoDomicilio.Descripcion, tipoDomicilio.Id.ToString()));
        }

        /// <summary>
        /// Brands bindings.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindMarcas(MarcaDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (Marca marca in DaoFactory.MarcaDAO.FindAll()) autoBindeable.Items.Add(new ListItem(marca.Descripcion, marca.Id.ToString()));
        }

        /// <summary>
        /// Transportistas binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTransportista(ITransportistaAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddld = autoBindeable.GetParentByType(typeof (Empresa));
            var ddll = autoBindeable.GetParentByType(typeof (Linea));

            var idBase = ddll != null ? ddll.Selected : -1;
            var idDistrito = ddld != null ? ddld.Selected : idBase > 0 ? DaoFactory.LineaDAO.FindById(idBase).Empresa.Id : -1;

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var transportistas = DaoFactory.TransportistaDAO.FindByEmpresaLineaYUsuario(new List<int> { idDistrito }, new List<int> { idBase }, user);

            if (autoBindeable.AddAllItem)
                            autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            if (autoBindeable.AddSinTransportista && user.MostrarSinTransportista) autoBindeable.AddItem(CultureManager.GetControl("DDL_NO_TRANSPORTISTA"), -2);


            foreach (var transportista in transportistas) autoBindeable.AddItem(transportista.Descripcion, transportista.Id);
        }

        /// <summary>
        /// Employees data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindEmpleados(IEmpleadoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddld = autoBindeable.GetParentByType(typeof (Empresa));
            var ddll = autoBindeable.GetParentByType(typeof (Linea));
            var ddlt = autoBindeable.GetParentByType(typeof (Transportista));
            var ddlti = autoBindeable.GetParentByType(typeof (TipoEmpleado));

            if (autoBindeable.AddSinEmpleado) autoBindeable.AddItem(CultureManager.GetControl("DDL_NO_EMPLOYEE"), -2);

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var tipo = ddlti != null ? ddlti.Selected : -1;
            var emp = ddld != null ? ddld.SelectedValues : new List<int> { -1 };
            var lin = ddll != null ? ddll.SelectedValues : new List<int> { -1 };
            var transportista = ddlt != null ? ddlt.SelectedValues : new List<int> { -1 };

            var choferes = DaoFactory.EmpleadoDAO.GetByEmpresaAndLineaAndTipoAndTransportista(emp, lin, new List<int> { tipo }, transportista, user, autoBindeable.AllowOnlyDistrictBinding).Cast<Empleado>()
                .Where(chofer => (!autoBindeable.SoloResponsables || (autoBindeable.SoloResponsables && chofer.EsResponsable)));

            foreach (var chofer in choferes) autoBindeable.AddItem(chofer.Entidad.Descripcion, chofer.Id);
        }

        /// <summary>
        /// Employees data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTiposEmpleado(ITipoEmpleadoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddld = autoBindeable.GetParentByType(typeof(Empresa));
            var ddll = autoBindeable.GetParentByType(typeof(Linea));
           
            var linea = (ddll != null && ddll.Selected > 0) ? DaoFactory.LineaDAO.FindById(ddll.Selected) : null;
            var empresa = (ddld != null && ddld.Selected > 0) ? DaoFactory.EmpresaDAO.FindById(ddld.Selected) : linea != null ? linea.Empresa : null;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);
            if (autoBindeable.AddSinTipoEmpleado) autoBindeable.AddItem(CultureManager.GetControl("DDL_NO_TIPOEMPLEADO"), -2);

            foreach (TipoEmpleado tipo in DaoFactory.TipoEmpleadoDAO.FindByEmpresaAndLinea(empresa, linea, user)) autoBindeable.AddItem(tipo.Descripcion, tipo.Id);
        }

        /// <summary>
        /// Devices binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindDispositivos(IDispositivoAutoBindeable autoBindeable)
        {
            var ddld = autoBindeable.GetParentByType(typeof (Empresa));
            var ddll = autoBindeable.GetParentByType(typeof (Linea));

            autoBindeable.ClearItems();

            autoBindeable.AddItem(CultureManager.GetControl("DDL_NONE"), -1);

            var linea = ddll != null && ddll.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddll.Selected) : null;
            var empresa = ddld != null && ddld.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddld.Selected) : linea != null ? linea.Empresa : null;

            foreach (Dispositivo dispositivo in DaoFactory.DispositivoDAO.GetDispositivosLibresByEmpresaAndLinea(empresa, linea)) autoBindeable.AddItem(dispositivo.Codigo, dispositivo.Id);

            if (autoBindeable.Coche <= 0) return;

            var coche = DaoFactory.CocheDAO.FindById(autoBindeable.Coche);

            if (coche == null || coche.Dispositivo == null) return;

            autoBindeable.AddItem(coche.Dispositivo.Codigo, coche.Dispositivo.Id);
        }

        /// <summary>
        /// Firmwares binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindFirmwares(FirmwareDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var firmware in DaoFactory.FirmwareDAO.FindAll()) autoBindeable.Items.Add( new ListItem(firmware.Nombre, firmware.Id.ToString()));

            autoBindeable.Items.Insert(0, new ListItem(CultureManager.GetControl("DDL_UNASSIGNED"), "-1"));
        }

        /// <summary>
        /// Devices Configs binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindConfigurations(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            foreach (var config in DaoFactory.ConfiguracionDispositivoDAO.FindAll().OrderBy(c => c.Orden).ThenBy(c=> c.Nombre)) autoBindeable.AddItem(config.Nombre, config.Id);
        }

        /// <summary>
        /// Cards binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTarjetas(TarjetaDropDownList autoBindeable)
        {
            var ddlEmpresa = autoBindeable.GetParentByType(typeof (Empresa));
            var ddlLinea = autoBindeable.GetParentByType(typeof (Linea));

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var linea = ddlLinea != null ? DaoFactory.LineaDAO.FindById(ddlLinea.Selected) : null;
            var empresa = ddlEmpresa != null ? DaoFactory.EmpresaDAO.FindById(ddlEmpresa.Selected) : linea != null ? linea.Empresa : null;

            autoBindeable.Items.Clear();
            
            foreach (var tarjeta in DaoFactory.TarjetaDAO.GetTarjetasLibres(user, empresa, linea)) autoBindeable.Items.Add(new ListItem(tarjeta.Numero, tarjeta.Id.ToString()));

            autoBindeable.Items.Insert(0, new ListItem(CultureManager.GetControl("DDL_NONE"), "-1"));

            if (autoBindeable.Chofer <= 0) return;

            var empleado = DaoFactory.EmpleadoDAO.FindById(autoBindeable.Chofer);

            if (empleado == null || empleado.Tarjeta == null) return;

            autoBindeable.Items.Add(new ListItem(empleado.Tarjeta.Numero, empleado.Tarjeta.Id.ToString()));
            autoBindeable.Items[autoBindeable.Items.Count - 1].Selected = true;
        }
        
        /// <summary>
        /// Vehicle states binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindEstadoCoches(EstadoChocesDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in EstadoCoches) autoBindeable.Items.Add(new ListItem( ((OptionGroupConfiguration)pair.Second).OptionGroupDescription, pair.First.ToString()));
        }
        
        /// <summary>
        /// Device states binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindEstadoDispositivos(EstadoDispositivosDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in EstadoDispositivos) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }

        /// <summary>
        /// Logic types binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTiposLogisticos(TipoLogisticoDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in TiposLogisticos) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }

        /// <summary>
        /// Message source binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindOrigenesMensajes(MensajeOrigenDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in OrigenesMensaje) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }


        /// <summary>
        /// Point of interest data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindReferenciaGeografica(IAutoBindeable autoBindeable)
        {
            var ddle = autoBindeable.GetParentByType(typeof(Empresa));
            var ddll = autoBindeable.GetParentByType(typeof(Linea));
            var ddltd = autoBindeable.GetParentByType(typeof(TipoReferenciaGeografica));
            
            autoBindeable.ClearItems();

            var linea = ddll != null ? ddll.Selected : -1;
            var empresa = ddle != null ? ddle.Selected : linea > 0 ? DaoFactory.LineaDAO.FindById(linea).Empresa.Id : -1;
            
            var tipos = ddltd != null && ddltd.SelectedValues.Count > 0 ? ddltd.SelectedValues : new List<int>{-1};

            var lin = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
            var emp = empresa > 0 ? DaoFactory.EmpresaDAO.FindById(empresa) : lin != null ? lin.Empresa : null;

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var domicilios = DaoFactory.ReferenciaGeograficaDAO.FindByEmpresaLineaTipoUsuario(emp, lin, tipos,user);

            foreach (var domicilio in domicilios) autoBindeable.AddItem(domicilio.Descripcion, domicilio.Id);
        }

        /// <summary>
        /// Tipo de dispositivos data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTipoDispositivo(ITipoDispositivoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            if (autoBindeable.AddSinAsignar) autoBindeable.AddItem(CultureManager.GetControl("DDL_UNASSIGNED"), -1);

            foreach (var tipoDispositivo in DaoFactory.TipoDispositivoDAO.FindAll()) autoBindeable.AddItem(tipoDispositivo.Modelo, tipoDispositivo.Id);
        }

        /// <summary>
        /// Tipo parametro de dispositivos binding
        /// </summary>
        /// <param name="lb"></param>
        public void BindTipoParametroDispositivo(TipoParametroDispositivoListBox lb)
        {
            var ddl = lb.GetParentByType(typeof(TipoDispositivo));
            
            lb.Items.Clear();

            var parameters = DaoFactory.TipoParametroDispositivoDAO.FindByTipoDispositivo(ddl.Selected);

            foreach (var param in parameters) lb.Items.Add(new ListItem(param.Nombre, param.Id.ToString()));
        }

        /// <summary>
        /// Dispositivo binding by tipoDispositivo
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindDispositivoByTipo(IAutoBindeable autoBindeable)
        {
            var ddld = autoBindeable.GetParentByType(typeof (Empresa));
            var ddll = autoBindeable.GetParentByType(typeof (Linea));
            var ddl = autoBindeable.GetParentByType(typeof (TipoDispositivo));

            autoBindeable.ClearItems();

            var linea = ddll != null && ddll.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddll.Selected) : null;
            var empresa = ddld != null && ddld.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddld.Selected) : linea != null ? linea.Empresa : null;
            var tipo = ddl != null && ddl.Selected > 0 ? DaoFactory.TipoDispositivoDAO.FindById(ddl.Selected) : null;

            foreach (var device in DaoFactory.DispositivoDAO.GetDispositivosByTipoAndEmpresaAndLinea(empresa, linea, tipo)) autoBindeable.AddItem(device.Codigo, device.Id);
        }

        /// <summary>
        /// Cliente Binding.
        /// </summary>
        public void BindCliente(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddld = autoBindeable.GetParentByType(typeof (Empresa));
            var ddlb = autoBindeable.GetParentByType(typeof (Linea));

            var linea = ddlb != null ? ddlb.Selected : -1;
            var distrito = ddld != null ? ddld.Selected : linea > 0 ? DaoFactory.LineaDAO.FindById(linea).Empresa.Id : -1;
            
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var clientes = DaoFactory.ClienteDAO.FindByEmpresaYLinea(distrito, linea, user);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            foreach (Cliente c in clientes) autoBindeable.AddItem(c.Descripcion, c.Id);
        }

        /// <summary>
        /// Equipo data binding.
        /// </summary>
        public void BindEquipo(IEquipoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddle = autoBindeable.GetParentByType(typeof (Empresa));
            var ddlb = autoBindeable.GetParentByType(typeof (Linea));
            var ddlc = autoBindeable.GetParentByType(typeof (Cliente));

            var empresa = ddle == null ? -1 : ddle.Selected;
            var linea = ddlb == null ? -1 : ddlb.Selected;
            var cliente = ddlc == null ? -1 : ddlc.Selected;

            var lin = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
            var emp = empresa > 0 ? DaoFactory.EmpresaDAO.FindById(empresa) : lin != null ? lin.Empresa : null;
            var cli = cliente > 0 ? DaoFactory.ClienteDAO.FindById(cliente) : null;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var equipos = DaoFactory.EquipoDAO.FindByEmpresaLineaYCliente(emp, lin, cli, user);

            if(autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            if (autoBindeable.AddNoEquipment) autoBindeable.AddItem(CultureManager.GetControl("DDL_NO_EQUIPMENT"), -2);

            foreach(Equipo equipo in equipos) autoBindeable.AddItem(equipo.Descripcion, equipo.Id);
        }

        ///// <summary>
        ///// CentroDeCarga Binding
        ///// </summary>
        ///// <param name="ddl"></param>
        public void BindCentroDeCostos(ICentroCostosAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddlEmpresa = autoBindeable.GetParentByType(typeof(Empresa));
            var ddlBase = autoBindeable.GetParentByType(typeof (Linea));

            if (autoBindeable.Usuario == null) return;

            var baseIndex = (ddlBase == null) ? new List<int>{-1} : ddlBase.SelectedValues;
            var empresaIndex = (ddlEmpresa == null) ? new List<int> { -1 } : ddlEmpresa.SelectedValues;

            var empresas = !empresaIndex.Contains(-1) && !empresaIndex.Contains(0) ? (from int e in empresaIndex select DaoFactory.EmpresaDAO.FindById(e)).ToList() : null;
            var lineas = !baseIndex.Contains(-1) && !baseIndex.Contains(0) ? (from b in baseIndex select DaoFactory.LineaDAO.FindById(b)).ToList() : null;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var emp = empresas != null ? (from Empresa e in empresas where e != null select e.Id).ToList() 
                                        : lineas != null ? (from Linea l in lineas where l != null && l.Empresa != null select l.Empresa.Id).ToList()
                                            : new List<int> { -1 };
            var lin = lineas != null ? (from Linea l in lineas where l != null select l.Id).ToList() : new List<int> { -1 };

            var centers = DaoFactory.CentroDeCostosDAO.FindByEmpresasYLineasAndUser(emp, lin, user);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            if (autoBindeable.AddSinCentroDeCostos) autoBindeable.AddItem(CultureManager.GetControl("DDL_SIN_CENTRO"), -2); 
            
            foreach (CentroDeCostos center in centers) autoBindeable.AddItem(center.Descripcion, center.Id);
        }

        /// <summary>
        /// TimeZone data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTimeZones(TimeZoneDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            foreach(var zone in timeZones) autoBindeable.Items.Add(new ListItem(zone.DisplayName, zone.Id));
        }

        /// <summary>
        /// Menu resources data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindMenuResources(MenuResourcesDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if(autoBindeable.AddEmptyItem) autoBindeable.Items.Add(string.Empty);

            var ord = from r in CultureManager.GetAllStrings("Menu") 
                      where r.Key.StartsWith("SYS_") != autoBindeable.FunctionMode
                      orderby r.Value select r;
            
            foreach (var pair in ord) autoBindeable.Items.Add(new ListItem(pair.Value, pair.Key));
        }

        /// <summary>
        /// Resource groups data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindResourceGroup(ResourceGroupDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            var resourceGroups = ConfigurationHelper.UrbetrackResourcesGroups;

            if (string.IsNullOrEmpty(resourceGroups)) return;

            var groups = resourceGroups.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var group in groups) autoBindeable.Items.Add(new ListItem(group, group));
        }

        /// <summary>
        /// Ticket status data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindEstadoTicket(EstadoTicketDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if (autoBindeable.AddAllItem) autoBindeable.Items.Add(new ListItem(CultureManager.GetControl("DDL_ALL_ITEMS"), "-1"));

            foreach (var pair in EstadosTicket) autoBindeable.Items.Add(new ListItem(pair.Value, pair.Key.ToString()));
        }

        /// <summary>
        /// Tanks data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTanque(ITanqueAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var tanques = new List<Tanque>();
            
            var ddlDistrito = autoBindeable.GetParentByType(typeof(Empresa));
            var ddlLinea = autoBindeable.GetParentByType(typeof(Linea));
            var ddlEquipo = autoBindeable.GetParentByType(typeof(Equipo));

            var linea = ddlLinea != null && ddlLinea.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddlLinea.Selected) : null;
            var distrito = ddlDistrito != null && ddlDistrito.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : linea != null ? linea.Empresa : null;
            var equipo = ddlEquipo != null && ddlEquipo.Selected > 0 ? DaoFactory.EquipoDAO.FindById(ddlEquipo.Selected).Id : ddlEquipo != null ? ddlEquipo.Selected : -1;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            if (autoBindeable.AllowEquipmentBinding)
                tanques.AddRange(
                    (from Tanque t in DaoFactory.TanqueDAO.FindByEquipo(user, distrito, linea, equipo) select t).ToList());
            if (autoBindeable.AllowBaseBinding)
                tanques.AddRange(
                    (from Tanque t in DaoFactory.TanqueDAO.FindByEmpresaAndLinea(distrito, linea, user) select t).ToList());

            foreach (var tank in tanques) autoBindeable.AddItem(tank.Descripcion, tank.Id);
        }

        /// <summary>
        /// TipoMovimiento data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindTipoMovimiento(TipoMovimientoDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if (autoBindeable.Usuario == null) return;

            var list = DaoFactory.TipoMovimientoDAO.FindAllUserAvaiable();

            foreach (TipoMovimiento t in list) autoBindeable.Items.Add(new ListItem(t.Descripcion, t.Id.ToString()));
        }

        /// <summary>
        /// MotivoConciliacion data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindMotivoConciliacion(MotivoConciliacionDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if (autoBindeable.Usuario == null) return;

            var list = DaoFactory.MotivoConciliacionDAO.FindAll();

            foreach (var m in list) autoBindeable.Items.Add(new ListItem(m.Descripcion, m.Id.ToString()));
        }

        /// <summary>
        /// Engines Data Binding.
        /// </summary>
        /// <param name="lb"></param>
        public void BindCaudalimetro(ICaudalimetroAutoBindeable lb)
        {
            lb.ClearItems();

            var ddle = lb.GetParentByType(typeof (Equipo));
            var ddlb = lb.GetParentByType(typeof (Linea));
            var ddlem = lb.GetParentByType(typeof (Empresa));

            if (ddlem == null || ddlb == null ||ddlem.Selected == 0 || ddlb.Selected == 0) return;
            
            var equipo = ddle.Selected > 0 ? DaoFactory.EquipoDAO.FindById(ddle.Selected) : null;
            var empresa = ddlem.Selected > 0 ? DaoFactory.EmpresaDAO.FindById(ddlem.Selected) : null;
            var linea = ddlb.Selected > 0 ? DaoFactory.LineaDAO.FindById(ddlb.Selected) : null;
            var user = DaoFactory.UsuarioDAO.FindById(lb.Usuario.Id);

            var motores = DaoFactory.CaudalimetroDAO.FindByEquipoEmpresaAndLinea(equipo, empresa, linea, !lb.ShowDeIngreso,user);

            foreach (Caudalimetro m in motores) lb.AddItem(m.Descripcion, m.Id);
        }

        /// <summary>
        /// Address types binding.
        /// </summary>
		/// <param name="autoBindeable"></param>
        public void BindTipoDocumento(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var cbEmpresa = autoBindeable.GetParentByType(typeof(Empresa));
            var cbLinea = autoBindeable.GetParentByType(typeof(Linea));
            var empresa = cbEmpresa != null ? cbEmpresa.Selected : -1;
            var linea = cbLinea != null ? cbLinea.Selected : -1;

            var tipos = DaoFactory.TipoDocumentoDAO.FindByEmpresaLinea(empresa, linea);

            foreach (var tipoDocumento in tipos) autoBindeable.AddItem(tipoDocumento.Descripcion, tipoDocumento.Id);
        }

        /// <summary>
        /// Delivery points data binding.
        /// </summary>
        /// <param name="autoBindeable"></param>
        public void BindPuntoDeEntrega(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddlc = autoBindeable.GetParentByType(typeof(Cliente));

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(CultureManager.GetControl("DDL_ALL_ITEMS"), -1);

            if (ddlc == null || ddlc.Selected <= 0) return;

            foreach (PuntoEntrega punto in DaoFactory.PuntoEntregaDAO.FindByCliente(ddlc.Selected)) autoBindeable.AddItem(punto.Descripcion, punto.Id);            
        }

        public void BindTipoDetalleCiclo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            foreach (var tipo in TiposDetalleCiclo) autoBindeable.AddItem(tipo.Value, tipo.Key);
        }

        public void BindCiclosLogisticos(ICicloLogisticoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var ddlem = autoBindeable.GetParentByType(typeof(Empresa));
            var ddlb = autoBindeable.GetParentByType(typeof(Linea));

            IList<CicloLogistico> ciclos = DaoFactory.CicloLogisticoDAO.GetByType(
                ddlem != null ? ddlem.Selected : -1,
                ddlb != null ? ddlb.Selected : -1,
                autoBindeable.AddCiclos,
                autoBindeable.AddEstados);

            foreach (var ciclo in ciclos) autoBindeable.AddItem(ciclo.Descripcion, ciclo.Id);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the string value for the OptionGroupProperty of the selected mobile.
        /// </summary>
        /// <param name="optionGroupProperty"></param>
        /// <param name="movil"></param>
        /// <returns></returns>
        private static OptionGroupConfiguration GetMobileGroupDescription(MovilOptionGroupValue optionGroupProperty, Coche movil)
        {
            var groupDesc = new OptionGroupConfiguration();

            switch (optionGroupProperty)
            {
                case MovilOptionGroupValue.TipoVehiculo: groupDesc.OptionGroupDescription = movil.TipoCoche != null ? movil.TipoCoche.Descripcion : String.Empty; break;
                case MovilOptionGroupValue.Estado: groupDesc = (OptionGroupConfiguration)EstadoCoches.ElementAt(movil.Estado).Second; break;
                default: break;
            }

            return groupDesc;
        }

        /// <summary>
        /// Orders the mobiles list according to the selectes OptionGroupProperty.
        /// </summary>
        /// <param name="optionGroupProperty"></param>
        /// <param name="coches"></param>
        /// <returns></returns>
        private static List<Coche> OrderMobilesByOptionGroupProperty(MovilOptionGroupValue optionGroupProperty, List<Coche> coches)
        {
            switch (optionGroupProperty)
            {
                case MovilOptionGroupValue.TipoVehiculo: coches = (from Coche coche in coches orderby coche.TipoCoche.Descripcion select coche).ToList(); break;
                case MovilOptionGroupValue.Estado: coches = (from Coche coche in coches orderby coche.Estado select coche).ToList(); break;
                default: break;
            }

            return coches;
        }

        #endregion
    }
}
