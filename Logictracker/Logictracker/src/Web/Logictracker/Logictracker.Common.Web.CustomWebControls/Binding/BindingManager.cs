using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.ListBoxs;
using Logictracker.Messaging;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Web.CustomWebControls.Binding
{
    public partial class BindingManager
    {
        #region Private Properties

        private DAOFactory _daof;
        private DAOFactory DaoFactory { get { return _daof ?? (_daof = new DAOFactory()); } }

        private static IEnumerable<Pair> TiposUsuario
        {
            get
            {
                return new List<Pair>
                                        {
                                            new Pair(Usuario.NivelAcceso.NoAccess, CultureManager.GetUser("USERTYPE_NOACCESS")),
                                            new Pair(Usuario.NivelAcceso.Public, CultureManager.GetUser("USERTYPE_PUBLIC")),
                                            new Pair(Usuario.NivelAcceso.User , CultureManager.GetUser("USERTYPE_USER")),
                                            new Pair(Usuario.NivelAcceso.Installer, CultureManager.GetUser("USERTYPE_INSTALLER")),
                                            new Pair(Usuario.NivelAcceso.AdminUser,CultureManager.GetUser("USERTYPE_ADMIN_USER")),
                                            new Pair(Usuario.NivelAcceso.SysAdmin, CultureManager.GetUser("USERTYPE_SYS_ADMIN")),
                                            new Pair(Usuario.NivelAcceso.Developer, CultureManager.GetUser("USERTYPE_DEVELOPER")),
                                            new Pair(Usuario.NivelAcceso.SuperAdmin, CultureManager.GetUser("USERTYPE_SUPER_ADMIN"))
                                        };
            }
        }
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
                                        ImageUrl = "~/images/vehicle_inactive.png"}),
                                new Pair(3, new OptionGroupConfiguration
                                    {
                                        OptionGroupDescription = CultureManager.GetLabel("VEHICLESTATE_REVISAR"),
                                        ImageUrl = "~/images/vehicle_inactive.png"})
                           };
            }
        }
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

        #endregion

        #region Public Method

        public void BindLocacion(IAutoBindeable autoBindeable, params int[] filter)
        {
            autoBindeable.ClearItems();

            if (autoBindeable.Usuario == null) return;

            var empresas = DaoFactory.EmpresaDAO.GetEmpresasPermitidas();
            if (filter != null && filter.Length > 0) empresas = empresas.Where(e => filter.Contains(e.Id)).ToList();

            AddDefaultItems(autoBindeable);

            if (autoBindeable.Usuario.Fantasia)
            {
                empresas = empresas.OrderBy(e => e.Fantasia).ToList();
                foreach (var empresa in empresas) autoBindeable.AddItem(empresa.Fantasia, empresa.Id);
            }
            else
            {
                empresas = empresas.OrderBy(e => e.RazonSocial).ToList();
                foreach (var empresa in empresas) autoBindeable.AddItem(empresa.RazonSocial, empresa.Id);
            }
        }

        public void BindPlanta(IAutoBindeable autoBindeable, params int[] filter)
        {
            autoBindeable.ClearItems();

            var ddlb = autoBindeable.GetParent<Empresa>();

            var lineas = DaoFactory.LineaDAO.GetLineasPermitidasPorUsuario(ddlb != null ? ddlb.SelectedValues : new List<int> { });
            if (filter != null && filter.Length > 0)lineas = lineas.Where(l => filter.Contains(l.Id)).ToList();

            AddDefaultItems(autoBindeable);

            foreach (var linea in lineas) autoBindeable.AddItem(linea.Descripcion, linea.Id);
        }

        public void BindUsuarios(UsuarioDropDownList ddl, Usuario usuario)
        {
            foreach (var user in DaoFactory.UsuarioDAO.FindByUsuario(usuario)) ddl.Items.Add(new ListItem(user.NombreUsuario, user.Id.ToString("#0"))); 
        }

        public void BindOdometro(OdometroListBox lstBox)
        {
            lstBox.ClearItems();

            var idEmpresa = lstBox.ParentSelected<Empresa>();
            var idLinea = lstBox.ParentSelected<Linea>();
            var idsTipoCoche = lstBox.ParentSelectedValues<TipoCoche>();

            if (lstBox.AddGeneral)
                lstBox.AddItem(CultureManager.GetLabel("KM_TOTALES"), -10);

            var user = DaoFactory.UsuarioDAO.FindById(lstBox.Usuario.Id);

            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;

            var odometros = DaoFactory.OdometroDAO.FindByEmpresaYLineaYTipo(empresa, linea, idsTipoCoche, user);

            foreach (Odometro odometro in odometros) lstBox.AddItem(odometro.Descripcion, odometro.Id);
        }

        public void BindLogTypes(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);
            
            var i = 0;

            foreach (var tipo in Enum.GetNames(typeof(LogTypes)))
            {
                autoBindeable.AddItem(tipo, i);
                i++;
            }
        }

        public void BindLogModules(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var i = 0;

            foreach (var tipo in Enum.GetNames(typeof(LogModules)))
            {
                autoBindeable.AddItem(tipo, i);
                i++;
            }
        }

        public void BindLogComponents(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var i = 0;

            foreach (var tipo in Enum.GetNames(typeof(LogComponents)))
            {
                autoBindeable.AddItem(tipo, i);
                i++;
            }
        }

        public void BindMovil(IMovilAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            if (autoBindeable.GetParent<Linea>() == null && autoBindeable.GetParent<Empresa>() == null) return;

            var idsEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idsLinea = autoBindeable.ParentSelectedValues<Linea>();
            var idsTipoCoche = autoBindeable.ParentSelectedValues<TipoCoche>();
            var idsTransportista = autoBindeable.ParentSelectedValues<Transportista>();
            var idsDepartamentos = autoBindeable.ParentSelectedValues<Departamento>();
            var idsCentroCostos = autoBindeable.ParentSelectedValues<CentroDeCostos>();
            var idsSubCentroCostos = autoBindeable.ParentSelectedValues<SubCentroDeCostos>();
            //var idsCliente = autoBindeable.ParentSelectedValues<Cliente>();
            var idsMarcas = autoBindeable.ParentSelectedValues<Marca>();
            var idsModelos = autoBindeable.ParentSelectedValues<Modelo>();
            var idsEmpleados = autoBindeable.ParentSelectedValues<Empleado>();
            var idsTipoEmpleado = autoBindeable.ParentSelectedValues<TipoEmpleado>();

            var showChofer = autoBindeable.ShowDriverName;

            //var coches = DaoFactory.CocheDAO.GetList(idsEmpresa,
            //                                         idsLinea,
            //                                         idsTipoCoche,
            //                                         idsTransportista,
            //                                         idsDepartamentos,
            //                                         idsCentroCostos,
            //                                         idsSubCentroCostos,
            //                                         idsCliente,
            //                                         idsMarcas,
            //                                         idsModelos,
            //                                         idsTipoEmpleado,
            //                                         idsEmpleados)
            //                                .OrderBy(c => showChofer
            //                                                ? (c.Chofer != null
            //                                                        ? c.Chofer.Entidad.Descripcion
            //                                                        : CultureManager.GetControl("DDL_NO_EMPLOYEE"))
            //                                                : c.Interno)
            //                                .ToList();

            var coches = DaoFactory.CocheDAO.GetCochesPermitidosPorUsuario(idsEmpresa,
                                                                           idsLinea,
                                                                           idsTipoCoche,
                                                                           idsTransportista,
                                                                           idsDepartamentos,
                                                                           idsCentroCostos,
                                                                           idsSubCentroCostos,
                                                                           idsMarcas,
                                                                           idsModelos,
                                                                           idsTipoEmpleado,
                                                                           idsEmpleados)
                                            .OrderBy(c => showChofer
                                                            ? (c.Chofer != null
                                                                    ? c.Chofer.Entidad.Descripcion
                                                                    : CultureManager.GetControl("DDL_NO_EMPLOYEE"))
                                                            : c.Interno)
                                            .ToList();

            AddDefaultItems(autoBindeable);

            if (autoBindeable.UseOptionGroup) coches = OrderMobilesByOptionGroupProperty(autoBindeable.OptionGroupProperty, coches);
            var puertas = autoBindeable.ShowOnlyAccessControl ? DaoFactory.PuertaAccesoDAO.GetList(idsEmpresa, idsLinea).Where(p => p.Vehiculo != null).Select(p => p.Vehiculo.Id).ToList() : new List<int>(0);

            foreach (var movil in coches)
            {
                if (autoBindeable.ShowOnlyAccessControl && !movil.EsPuerta && !movil.TipoCoche.EsControlAcceso) continue;
                if (autoBindeable.HideWithNoDevice && movil.Dispositivo == null) continue;
                if (autoBindeable.HideWithNoDevice && movil.Dispositivo.Codigo.Contains("No borrar.")) continue;
                if (autoBindeable.HideInactive && movil.Estado == Coche.Estados.Inactivo) continue;
                if (movil.Id != autoBindeable.Coche && autoBindeable.ShowOnlyAccessControl && puertas.Contains(movil.Id)) continue;

                var desc = showChofer 
                                ? (movil.Chofer != null 
                                    ? movil.Chofer.Entidad.Descripcion 
                                    : CultureManager.GetControl("DDL_NO_EMPLOYEE")) 
                                : movil.Interno != movil.Patente
                                    ? movil.Interno + " - (" + movil.Patente + ")"
                                    : movil.Interno;
                
                if (movil.TipoCoche.SeguimientoPersona && movil.Chofer != null)
                    desc = movil.Chofer.Entidad.Descripcion;

                var groupDescription = new OptionGroupConfiguration();

                if (autoBindeable.UseOptionGroup) groupDescription = GetMobileGroupDescription(autoBindeable.OptionGroupProperty, movil);

                autoBindeable.AddItem(desc, movil.Id, groupDescription);
            }
        }

        public void BindTipoMensaje(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            foreach (TipoMensaje tipomensaje in DaoFactory.TipoMensajeDAO.FindByEmpresaLineaYUsuario(empresa, linea,user)) autoBindeable.AddItem(tipomensaje.Descripcion, tipomensaje.Id);
        }

        public void BindMensajes(IMensajeAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTipoMensaje = autoBindeable.ParentSelected<TipoMensaje>();

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var tipoUsuario = user.Tipo;

            var tipoMensaje = idTipoMensaje > 0 ? DaoFactory.TipoMensajeDAO.FindById(idTipoMensaje) : null;
            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;
            
            var mensajes = DaoFactory.MensajeDAO.FindByTipo(tipoMensaje, empresa, linea, user);
            
            if (autoBindeable.AddSinMensaje) autoBindeable.AddItem(CultureManager.GetControl("DDL_NO_MESSAGE"), autoBindeable.NoneValue);

            var messages = mensajes
                .Where(mensaje => mensaje.Acceso <= tipoUsuario 
                              && ((!autoBindeable.SoloMantenimiento || (autoBindeable.SoloMantenimiento && mensaje.TipoMensaje.DeMantenimiento))
                              && (!autoBindeable.SoloCombustible || (autoBindeable.SoloCombustible && mensaje.TipoMensaje.DeCombustible))))
                .OrderBy(m => m.Descripcion);

            if (autoBindeable.BindIds)
                foreach (var mensaje in messages) autoBindeable.AddItem(mensaje.Descripcion, mensaje.Id);
            else
                foreach (var mensaje in messages) autoBindeable.AddItem(mensaje.Descripcion, mensaje.Codigo);
        }

        public void BindSubSistema(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var sistemas = DaoFactory.SistemaDAO.FindAll().ToList().OrderBy(t => CultureManager.GetString("Menu", t.Descripcion));

            foreach (var subSistema in sistemas) autoBindeable.AddItem(CultureManager.GetString("Menu", subSistema.Descripcion), subSistema.Id.ToString("#0"));
        }

        public void BindSonidos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            foreach (Sonido sonido in DaoFactory.SonidoDAO.FindAll()) autoBindeable.AddItem(sonido.Descripcion, sonido.Id.ToString("#0"));
        }

        public void BindUserTypes(IAutoBindeable autoBindeable)
        {
        	autoBindeable.ClearItems();

        	foreach (var pair in TiposUsuario.Where(pair => Convert.ToInt16(pair.First) <= autoBindeable.Usuario.AccessLevel))
        	{
        		autoBindeable.AddItem(pair.Second.ToString(), ((short)pair.First).ToString("#0"));
        	}
        }

        public void BindTipoValidacion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            foreach (var pair in TiposValidacion) autoBindeable.AddItem(pair.Second.ToString(), pair.First.ToString());
        }

        public void BindProfiles(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var perfs = user.Perfiles.IsEmpty() ? DaoFactory.PerfilDAO.FindAll().OrderBy(p => p.Descripcion).ToList() : user.Perfiles.OrderBy(p => p.Descripcion).ToList();

            foreach (var perfil in perfs) autoBindeable.AddItem(perfil.Descripcion, perfil.Id);
        }

        public void BindFunctionTypes(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            foreach (var pair in TiposFuncion) autoBindeable.AddItem(pair.Second.ToString(), pair.First.ToString());
        }

        public void BindVehicleTypes(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idsEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idsLinea = autoBindeable.ParentSelectedValues<Linea>();

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);
            var listaTipos = DaoFactory.TipoCocheDAO.FindByEmpresasAndLineas(idsEmpresa, idsLinea, user);
            
            foreach (TipoCoche tipoCoche in listaTipos) autoBindeable.AddItem(tipoCoche.Descripcion, tipoCoche.Id);
        }
        
        public void BindTipoReferenciaGeografica(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            if (idEmpresa <= 0 && idLinea > 0) idEmpresa = DaoFactory.LineaDAO.FindById(idLinea).Empresa.Id;

            var listaTipos = DaoFactory.TipoReferenciaGeograficaDAO.GetList(new[]{idEmpresa}, new[]{idLinea})
                                                                   .OrderBy(d=>d.Descripcion);
            
            var tipoDomicilios = from tipoDomicilio in listaTipos
                                 let ownlinea = tipoDomicilio.Linea != null ? tipoDomicilio.Linea.Id : -1
                                 let ownempresa = tipoDomicilio.Empresa != null ? tipoDomicilio.Empresa.Id : -1
                                 where autoBindeable.FilterMode || idEmpresa != -1 || idLinea != -1 || (ownempresa == -1 && ownlinea == -1)
                                 where autoBindeable.FilterMode || idLinea != -1 || ownlinea == -1
                                 select tipoDomicilio;

            foreach (var tipoDomicilio in tipoDomicilios) autoBindeable.AddItem(tipoDomicilio.Descripcion, tipoDomicilio.Id);
        }

        public void BindTipoDomicilio(TipoReferenciaGeograficaListBox autoBindeable)
        {
            autoBindeable.Items.Clear();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var monitor = autoBindeable.Monitor;

            var tipos = DaoFactory.TipoReferenciaGeograficaDAO.GetList(new[]{idEmpresa}, new[]{idLinea})
                                                              .Where(t => !monitor || !t.ExcluirMonitor)
                                                              .OrderBy(t => t.Descripcion);

            foreach (var tipoDomicilio in tipos) 
                autoBindeable.Items.Add(new ListItem(tipoDomicilio.Descripcion, tipoDomicilio.Id.ToString("#0")));
        }

        public void BindTickets(TicketListBox autoBindeable)
        {
            autoBindeable.Items.Clear();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTransportista = autoBindeable.ParentSelected<Transportista>();
            var idCoche = autoBindeable.ParentSelected<Coche>();

            var tickets = DaoFactory.TicketDAO.GetList(new[] { idEmpresa },
                                                       new[] { idLinea },
                                                       new[] { idTransportista },
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       new[] { idCoche },
                                                       new int[] { Ticket.Estados.EnCurso },
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       DateTime.Today.ToDataBaseDateTime().AddMonths(-3),
                                                       DateTime.Now.ToDataBaseDateTime())
                                              .OrderBy(t => GetDescripcionTicket(t))
                                              .ToList();

            var viajes = DaoFactory.ViajeDistribucionDAO.GetList(new[] { idEmpresa },
                                                                 new[] { idLinea },
                                                                 new[] { idTransportista },
                                                                 new[] { -1 }, // DEPTOS
                                                                 new[] { -1 }, // CENTROS DE COSTO
                                                                 new[] { -1 }, // SUB CENTROS DE COSTO
                                                                 new[] {idCoche},
                                                                 new int[] { ViajeDistribucion.Estados.EnCurso },
                                                                 DateTime.Today.ToDataBaseDateTime().AddMonths(-3),
                                                                 DateTime.Now.ToDataBaseDateTime())
                                                        .OrderBy(t => GetDescripcionViaje(t))
                                                        .ToList();
            
            var items = tickets.Select(item => new ListItem(GetDescripcionTicket(item), "T-" + item.Id)).ToList();
            items.AddRange(viajes.Select(viaje => new ListItem(GetDescripcionViaje(viaje), "V-" + viaje.Id)));

            if (!string.IsNullOrEmpty(autoBindeable.FilterText))
                items = items.Where(i => i.Text.ToLower().Contains(autoBindeable.FilterText.ToLower())).ToList();
            
            autoBindeable.Items.AddRange(items.ToArray());
        }

        private static string GetDescripcionTicket(Ticket ticket)
        {
            var descripcion = string.Empty;
            if (ticket.Cliente != null)
                descripcion = descripcion + ticket.Cliente.Descripcion.Trim();
            else
                if (ticket.PuntoEntrega != null)
                    descripcion = descripcion + ticket.PuntoEntrega.Descripcion.Trim();

            descripcion = descripcion + " - " + ticket.Codigo.Trim();

            if (ticket.PuntoEntrega != null && !string.IsNullOrEmpty(ticket.PuntoEntrega.Telefono))
                descripcion = descripcion + " (" + ticket.PuntoEntrega.Telefono.Trim() + ")";
            return descripcion;
        }

        private static string GetDescripcionViaje(ViajeDistribucion viaje)
        {
            var str = viaje.Codigo + " - " + viaje.NumeroViaje;
            
            if (viaje.Vehiculo != null) str = str  + " (" + viaje.Vehiculo.Interno + ")";

            return str;
        }

        public void BindMarcas(MarcaDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var marcas = DaoFactory.MarcaDAO.GetList(new[]{idEmpresa}, new[]{idLinea}).OrderBy(t => t.Descripcion);

            foreach (var marca in marcas) autoBindeable.AddItem(marca.Descripcion, marca.Id);
        }

        public void BindModelos(ModeloDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idLinea = autoBindeable.ParentSelectedValues<Linea>();
            var idMarca = autoBindeable.ParentSelectedValues<Marca>();
            var modelos = DaoFactory.ModeloDAO.GetList(idEmpresa, idLinea, idMarca).OrderBy(t => t.Descripcion);

            foreach (var modelo in modelos) autoBindeable.AddItem(modelo.Descripcion, modelo.Id);
        }

        public void BindProductos(ProductoDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idBocaDeCarga = autoBindeable.ParentSelected<BocaDeCarga>();

            var productos = DaoFactory.ProductoDAO.GetList(new[] { idEmpresa }, new[] { idLinea }, new[] { idBocaDeCarga })
                                                  .OrderBy(t => t.Descripcion);

            foreach (var producto in productos) autoBindeable.AddItem(producto.Descripcion, producto.Id);
        }

        public void BindTransportista(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            
            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(autoBindeable.AllItemsName, autoBindeable.AllValue.ToString("#0"));
            if (autoBindeable.AddNoneItem && (!user.PorTransportista || user.MostrarSinTransportista)) autoBindeable.AddItem(autoBindeable.NoneItemsName, autoBindeable.NoneValue.ToString("#0"));
            
            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();

            var transportistas = DaoFactory.TransportistaDAO.GetTransportistasPermitidosPorUsuario(empresas, lineas);
            
            foreach (var transportista in transportistas) autoBindeable.AddItem(transportista.Descripcion, transportista.Id);
        }

        public void BindProveedor(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);
            
            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTipoProveedor = autoBindeable.ParentSelected<TipoProveedor>();
            var proveedores = DaoFactory.ProveedorDAO.GetList(new[] {idEmpresa}, 
                                                              new[] {idLinea}, 
                                                              new[] {idTipoProveedor})
                                                     .OrderBy(t => t.Descripcion);
            
            foreach (var proveedor in proveedores) autoBindeable.AddItem(proveedor.Codigo + " - " + proveedor.Descripcion, proveedor.Id);
        }

        public void BindInsumo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTipoInsumo = autoBindeable.ParentSelected<TipoInsumo>();
            var insumos = DaoFactory.InsumoDAO.GetList(new[] { idEmpresa }, new[] { idLinea }, new[]{idTipoInsumo}).OrderBy(t => t.Descripcion);

            try
            {
                var ddl = autoBindeable as InsumoDropDownList;
                if (ddl != null && ddl.DeCombustible)
                    insumos = insumos.Where(i => i.TipoInsumo.DeCombustible).ToList().OrderBy(i => i.Descripcion);
            }
            catch {}

            foreach (var insumo in insumos) autoBindeable.AddItem(insumo.Codigo + " - " + insumo.Descripcion, insumo.Id);
        }

        public void BindTipoInsumo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var tiposInsumo = DaoFactory.TipoInsumoDAO.GetList(new[] { idEmpresa }, new[] { idLinea }).OrderBy(t => t.Descripcion);

            foreach (var tipoInsumo in tiposInsumo) autoBindeable.AddItem(tipoInsumo.Descripcion, tipoInsumo.Id);
        }

        public void BindTipoProveedor(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var tiposProveedor = DaoFactory.TipoProveedorDAO.GetList(new[] { idEmpresa }, new[] { idLinea }).OrderBy(t => t.Descripcion);

            foreach (var tipoProveedor in tiposProveedor) autoBindeable.AddItem(tipoProveedor.Descripcion, tipoProveedor.Id);
        }

        public void BindEmpleados(IEmpleadoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idsEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idsLinea = autoBindeable.ParentSelectedValues<Linea>();
            var idsTransportista = autoBindeable.ParentSelectedValues<Transportista>();
            var idTipoEmpleado = autoBindeable.ParentSelectedValues<TipoEmpleado>();
            var idDepartamentos = autoBindeable.ParentSelectedValues<Departamento>();
            var idCentrosDeCosto = autoBindeable.ParentSelectedValues<CentroDeCostos>();

            var choferes = DaoFactory.EmpleadoDAO.GetList(idsEmpresa, idsLinea, idTipoEmpleado, idsTransportista, idCentrosDeCosto, idDepartamentos)
                .Where(chofer => (!autoBindeable.SoloResponsables || (autoBindeable.SoloResponsables && chofer.EsResponsable)))
                .OrderBy(chofer => chofer.Entidad.Descripcion);

            foreach (var chofer in choferes) autoBindeable.AddItem(chofer.Entidad.Descripcion, chofer.Id);
        }

        public void BindTiposEmpleado(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var tipos = DaoFactory.TipoEmpleadoDAO.GetList(empresas, lineas).OrderBy(t => t.Descripcion);

            foreach (var tipo in tipos) autoBindeable.AddItem(tipo.Descripcion, tipo.Id);
        }

        public void BindDispositivos(IDispositivoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            
            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var tiposDispositivo = autoBindeable.ParentSelectedValues<TipoDispositivo>();

            IEnumerable<Dispositivo> dispositivos = autoBindeable.HideAssigned
                                                        ? DaoFactory.DispositivoDAO.GetUnassigned(empresas, lineas, tiposDispositivo, autoBindeable.Padre)
                                                        : DaoFactory.DispositivoDAO.GetList(empresas, lineas, tiposDispositivo);

            if (autoBindeable.AddNoneItem) autoBindeable.AddItem(autoBindeable.NoneItemsName, autoBindeable.AllValue);

            if (autoBindeable.HideAssigned && autoBindeable.Coche > 0)
            {
                var coche = DaoFactory.CocheDAO.FindById(autoBindeable.Coche);
                if (coche == null || coche.Dispositivo == null) return;
                autoBindeable.AddItem(coche.Dispositivo.Codigo, coche.Dispositivo.Id);
            }

            if (autoBindeable.HideAssigned && autoBindeable.Empleado > 0)
            {
                var empleado = DaoFactory.EmpleadoDAO.FindById(autoBindeable.Empleado);
                if (empleado == null || empleado.Dispositivo == null) return;
                autoBindeable.AddItem(empleado.Dispositivo.Codigo, empleado.Dispositivo.Id);
            }

            foreach (var dispositivo in dispositivos.OrderBy(d => d.Codigo)) autoBindeable.AddItem(dispositivo.Codigo, dispositivo.Id);
        }

        public void BindFirmwares(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            autoBindeable.AddItem(CultureManager.GetControl("DDL_UNASSIGNED"), autoBindeable.AllValue);

            foreach (var firmware in DaoFactory.FirmwareDAO.FindAll()) autoBindeable.AddItem(firmware.Nombre, firmware.Id.ToString("#0"));
        }

        public void BindConfigurations(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            foreach (var config in DaoFactory.ConfiguracionDispositivoDAO.FindAll().OrderBy(c => c.Orden).ThenBy(c=> c.Nombre)) autoBindeable.AddItem(config.Nombre, config.Id);
        }

        public void BindTarjetas(TarjetaDropDownList autoBindeable)
        {
            var idEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idLinea = autoBindeable.ParentSelectedValues<Linea>();

            autoBindeable.Items.Clear();
            autoBindeable.AddItem(autoBindeable.NoneItemsName, autoBindeable.AllValue);

            var tarjetas = DaoFactory.TarjetaDAO.GetTarjetasLibres(idEmpresa, idLinea);
            foreach (var tarjeta in tarjetas) autoBindeable.Items.Add(new ListItem(tarjeta.Numero, tarjeta.Id.ToString("#0")));

            if (autoBindeable.Chofer <= 0) return;

            var empleado = DaoFactory.EmpleadoDAO.FindById(autoBindeable.Chofer);
            if (empleado == null || empleado.Tarjeta == null) return;

            autoBindeable.Items.Add(new ListItem(empleado.Tarjeta.Numero, empleado.Tarjeta.Id.ToString("#0")));
            autoBindeable.Items[autoBindeable.Items.Count - 1].Selected = true;
        }

        public void BindCategoriaAcceso(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var categorias = DaoFactory.CategoriaAccesoDAO.GetList(empresas, lineas).OrderBy(x => x.Nombre);
            
            foreach (var cat in categorias) autoBindeable.AddItem(cat.Nombre, cat.Id.ToString(CultureInfo.InvariantCulture));
        }
        
        public void BindEstadoCoches(EstadoChocesDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in EstadoCoches) autoBindeable.Items.Add(new ListItem(((OptionGroupConfiguration)pair.Second).OptionGroupDescription, pair.First.ToString()));
        }
        
        public void BindEstadoDispositivos(EstadoDispositivosDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in EstadoDispositivos) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }

        public void BindOrigenesMensajes(MensajeOrigenDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            foreach (var pair in OrigenesMensaje) autoBindeable.Items.Add(new ListItem(pair.Second.ToString(), pair.First.ToString()));
        }

        public void BindReferenciaGeografica(IAutoBindeable autoBindeable)
        {    
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idsTiposGeoRef = autoBindeable.ParentSelectedValues<TipoReferenciaGeografica>();

            var domicilios = DaoFactory.ReferenciaGeograficaDAO.GetList(new[] { idEmpresa }, new[] { idLinea }, idsTiposGeoRef)
                .Where(d => d.Vigencia == null || d.Vigencia.Vigente(DateTime.UtcNow))
                .OrderBy(d => d.Descripcion);
            
            foreach (var domicilio in domicilios) autoBindeable.AddItem(domicilio.Descripcion, domicilio.Id);            
        }

        public void BindReferenciaGeografica(ReferenciaGeograficaDropDownList autoBindeable)
        {
            autoBindeable.ClearItems();
            if (autoBindeable.AddNoneItem) autoBindeable.AddItem(autoBindeable.NoneItemsName, autoBindeable.NoneValue.ToString("#0"));

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idsTiposGeoRef = autoBindeable.ParentSelectedValues<TipoReferenciaGeografica>();

            if (autoBindeable.ShowOnlyAccessControl)
            {
                idsTiposGeoRef = DaoFactory.TipoReferenciaGeograficaDAO.GetList(new[] { idEmpresa }, new[] { idLinea })
                                                                       .Where(t => t.EsControlAcceso)
                                                                       .Select(t => t.Id)
                                                                       .ToList();
            }
            
            var domicilios = DaoFactory.ReferenciaGeograficaDAO.GetList(new[] { idEmpresa }, new[] { idLinea }, idsTiposGeoRef)
                                                               .Where(d => d.Vigencia == null || d.Vigencia.Vigente(DateTime.UtcNow))
                                                               .OrderBy(d => d.Descripcion);

            foreach (var domicilio in domicilios) autoBindeable.AddItem(domicilio.Descripcion, domicilio.Id);
        }

        public void BindTipoDispositivo(ITipoDispositivoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            if (autoBindeable.AddAllItem) autoBindeable.AddItem(autoBindeable.AllItemsName, autoBindeable.AllValue);
            if (autoBindeable.AddSinAsignar) autoBindeable.AddItem(CultureManager.GetControl("DDL_UNASSIGNED"), autoBindeable.NoneValue);

            var tipos = DaoFactory.TipoDispositivoDAO.FindAll().OrderBy(t => t.Modelo);
            foreach (var tipoDispositivo in tipos) autoBindeable.AddItem(tipoDispositivo.Modelo, tipoDispositivo.Id);
        }

        public void BindTipoParametroDispositivo(TipoParametroDispositivoListBox autoBindeable)
        {           
            autoBindeable.Items.Clear();

            var idTipoDispositivo = autoBindeable.ParentSelected<TipoDispositivo>();
            var parameters = DaoFactory.TipoParametroDispositivoDAO.FindByTipoDispositivo(idTipoDispositivo);

            foreach (var param in parameters) autoBindeable.Items.Add(new ListItem(param.Nombre, param.Id.ToString("#0")));
        }

        public void BindCliente(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            
            if (idEmpresa <= 0 && idLinea > 0) 
                idEmpresa = DaoFactory.LineaDAO.FindById(idLinea).Empresa.Id;

            var clientes = DaoFactory.ClienteDAO.GetList(new[] {idEmpresa}, new[] {idLinea}).OrderBy(c => c.Descripcion).ToList();
            if (!string.IsNullOrEmpty(autoBindeable.FilterText))
                clientes = clientes.Where(c => c.Descripcion.ToLower().Contains(autoBindeable.FilterText.ToLower())).ToList();
            
            foreach (var c in clientes) autoBindeable.AddItem(c.Descripcion, c.Id);
        }

        public void BindEquipo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idCliente = autoBindeable.ParentSelected<Cliente>();

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;
            var cliente = idCliente > 0 ? DaoFactory.ClienteDAO.FindById(idCliente) : null;

            var equipos = DaoFactory.EquipoDAO.FindByEmpresaLineaYCliente(empresa, linea, cliente, user);

            foreach(Equipo equipo in equipos) autoBindeable.AddItem(equipo.Descripcion, equipo.Id);
        }

        public void BindCentroDeCostos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            if (autoBindeable.Usuario == null) return;

            var idsEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idsLinea = autoBindeable.ParentSelectedValues<Linea>();
            var idsDepartamento = autoBindeable.ParentSelectedValues<Departamento>();
            
            var centers = DaoFactory.CentroDeCostosDAO.GetCentrosDeCostosPermitidosPorUsuario(idsEmpresa, idsLinea, idsDepartamento).OrderBy(cc => cc.Descripcion);
                       
            foreach (var center in centers) autoBindeable.AddItem(center.Descripcion, center.Id);
        }

        public void BindSubCentroDeCostos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            if (autoBindeable.Usuario == null) return;

            var idsEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idsLinea = autoBindeable.ParentSelectedValues<Linea>();
            var idsDepartamento = autoBindeable.ParentSelectedValues<Departamento>();
            var idsCentroDeCosto = autoBindeable.ParentSelectedValues<CentroDeCostos>();

            if (!Logictracker.DAL.DAO.BaseClasses.QueryExtensions.IncludesAll(idsCentroDeCosto)) idsCentroDeCosto.Add(-1);

            var subCenters = DaoFactory.SubCentroDeCostosDAO.GetList(idsEmpresa, idsLinea, idsDepartamento, idsCentroDeCosto);

            foreach (var subCenter in subCenters) autoBindeable.AddItem(subCenter.Descripcion, subCenter.Id);
        }

        public void BindTimeZones(TimeZoneDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            foreach(var zone in timeZones) autoBindeable.Items.Add(new ListItem(zone.DisplayName, zone.Id));
        }

        public void BindMenuResources(MenuResourcesDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if(autoBindeable.AddEmptyItem) autoBindeable.Items.Add(string.Empty);

            var ord = from r in CultureManager.GetAllStrings("Menu") 
                      where r.Key.StartsWith("SYS_") != autoBindeable.FunctionMode
                      orderby r.Value select r;
            
            foreach (var pair in ord) autoBindeable.Items.Add(new ListItem(pair.Value, pair.Key));
        }

        public void BindResourceGroup(ResourceGroupDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            var resourceGroups = Config.LogictrackerResourcesGroups;

            if (string.IsNullOrEmpty(resourceGroups)) return;

            var groups = resourceGroups.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var group in groups) autoBindeable.Items.Add(new ListItem(group, group));
        }

        public void BindTanque(ITanqueAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idEquipo = autoBindeable.ParentSelected<Equipo>();

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;

            var tanques = new List<Tanque>();

            if (autoBindeable.AllowEquipmentBinding)
                tanques.AddRange(DaoFactory.TanqueDAO.FindByEquipo(user, empresa, linea, idEquipo).OfType<Tanque>().ToList());
            if (autoBindeable.AllowBaseBinding)
                tanques.AddRange(DaoFactory.TanqueDAO.FindByEmpresaAndLinea(empresa, linea, user).OfType<Tanque>().ToList());

            foreach (var tank in tanques) autoBindeable.AddItem(tank.Descripcion, tank.Id);
        }

        public void BindTipoMovimiento(TipoMovimientoDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if (autoBindeable.Usuario == null) return;

            var list = DaoFactory.TipoMovimientoDAO.FindAllUserAvaiable();

            foreach (TipoMovimiento t in list) autoBindeable.Items.Add(new ListItem(t.Descripcion, t.Id.ToString("#0")));
        }

        public void BindMotivoConciliacion(MotivoConciliacionDropDownList autoBindeable)
        {
            autoBindeable.Items.Clear();

            if (autoBindeable.Usuario == null) return;

            var list = DaoFactory.MotivoConciliacionDAO.FindAll();

            foreach (var m in list) autoBindeable.Items.Add(new ListItem(m.Descripcion, m.Id.ToString("#0")));
        }

        public void BindCaudalimetro(ICaudalimetroAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            if (autoBindeable.GetParent<Empresa>() == null || autoBindeable.GetParent<Linea>() == null) return;

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idEquipo = autoBindeable.ParentSelected<Equipo>();

            if(idEmpresa == 0 || idLinea == 0) return;

            var user = DaoFactory.UsuarioDAO.FindById(autoBindeable.Usuario.Id);

            var linea = idLinea > 0 ? DaoFactory.LineaDAO.FindById(idLinea) : null;
            var empresa = linea != null ? linea.Empresa : idEmpresa > 0 ? DaoFactory.EmpresaDAO.FindById(idEmpresa) : null;
            var equipo = idEquipo > 0 ? DaoFactory.EquipoDAO.FindById(idEquipo) : null;

            var motores = DaoFactory.CaudalimetroDAO.FindByEquipoEmpresaAndLinea(equipo, empresa, linea, !autoBindeable.ShowDeIngreso, user);

            foreach (Caudalimetro m in motores) autoBindeable.AddItem(m.Descripcion, m.Id);
        }

        public void BindTipoDocumento(ITipoDocumentoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();

            var tipos = DaoFactory.TipoDocumentoDAO.GetList(new[]{idEmpresa}, new[]{idLinea});
            if (autoBindeable.OnlyForVehicles) tipos = tipos.Where(t => t.AplicarAVehiculo).ToList();
            if (autoBindeable.OnlyForEmployees) tipos = tipos.Where(t => t.AplicarAEmpleado).ToList();
            if (autoBindeable.OnlyForEquipment) tipos = tipos.Where(t => t.AplicarAEquipo).ToList();
            if (autoBindeable.OnlyForTransporter) tipos = tipos.Where(t => t.AplicarATransportista).ToList();

            foreach (var tipoDocumento in tipos) autoBindeable.AddItem(tipoDocumento.Descripcion, tipoDocumento.Id);
        }

        public void BindPuntoDeEntrega(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            if (autoBindeable.GetParent<Cliente>() == null) return;

            var idEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idLinea = autoBindeable.ParentSelectedValues<Linea>();
            var idCliente = autoBindeable.ParentSelectedValues<Cliente>();
            
            foreach (var punto in DaoFactory.PuntoEntregaDAO.GetList(idEmpresa, idLinea, idCliente)) autoBindeable.AddItem(punto.Descripcion, punto.Id);
        }

        public void BindDepartamento(DepartamentoDropDownList autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idLinea = autoBindeable.ParentSelectedValues<Linea>();
            var departamentos = DaoFactory.DepartamentoDAO.GetList(idEmpresa, idLinea);
            if (autoBindeable.FiltraPorUsuario)
            {
                var idUsuario = WebSecurity.AuthenticatedUser.EmpleadoId;
                if (idUsuario > 0)
                {
                    var usuario = DaoFactory.EmpleadoDAO.FindById(idUsuario);
                    if (usuario.Departamento != null)
                        departamentos = departamentos.Where(d => d == usuario.Departamento);
                }
            }

            foreach (var item in departamentos) autoBindeable.AddItem(item.Descripcion, item.Id);
        }

        public void BindDepartamento(DepartamentoListBox autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelectedValues<Empresa>();
            var idLinea = autoBindeable.ParentSelectedValues<Linea>();
            var departamentos = DaoFactory.DepartamentoDAO.GetList(idEmpresa, idLinea);            

            foreach (var item in departamentos) autoBindeable.AddItem(item.Descripcion, item.Id);
        }

        public void BindPlan(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();

            var planes = DaoFactory.PlanDAO.GetList(new[] { idEmpresa } , new[] { -1 } );

            foreach (var plan in planes) autoBindeable.AddItem(plan.CodigoAbono, plan.Id);
        }

        public void BindLineaTelefonica(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<int>();
            var lineas = DaoFactory.LineaTelefonicaDAO.GetLibresByEmpresa(new[] { idEmpresa })
                                                      .OrderBy(l => l.NumeroLinea);

            foreach (var linea in lineas) autoBindeable.AddItem(linea.NumeroLinea, linea.Id);
        }

        public void BindEmpresaTelefonica(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem("Claro", 1);
            autoBindeable.AddItem("Movistar", 2);
            autoBindeable.AddItem("Personal", 3);
            autoBindeable.AddItem("Orbcom", 4);
        }

        public void BindEstrategia(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();            
            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var empresa = DaoFactory.EmpresaDAO.FindById(idEmpresa);

            var items = empresa != null 
                            ? LogicLinkFile.Estrategias.GetList().Where(e => e.ToUpperInvariant().Contains(empresa.RazonSocial.ToUpperInvariant())).ToList()
                            : new List<string>();
            
            for (int i = 0; i < items.Count; i++)
            {
                autoBindeable.AddItem(items[i], i);
            }
        }

        public void BindTipoLineaTelefonica(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("CELULAR"), 1);
            autoBindeable.AddItem(CultureManager.GetLabel("SATELITAL"), 2);
        }

        public void BindUnidadMedidaDatos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem("KB", 1);
            autoBindeable.AddItem("MB", 2);
        }

        public void BindReporte(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idMensaje = autoBindeable.ParentSelected<Mensaje>().ToString();

            if (idMensaje == MessageCode.CicloLogisticoCerrado.GetMessageCode())
            {
                autoBindeable.AddItem(CultureManager.GetMenu("REP_DISTRIBUCION"), ProgramacionReporte.Reportes.GetDropDownListIndex(ProgramacionReporte.Reportes.EstadoEntregas));
            }
        }

        public void BindTiposEntidad(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var tipos = DaoFactory.TipoEntidadDAO.GetList(new[] { idEmpresa }, new[] { idLinea })
                                                 .OrderBy(t => t.Descripcion);

            foreach (var tipo in tipos) autoBindeable.AddItem(tipo.Descripcion, tipo.Id);
        }

        public void BindTiposMedicion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var tipos = DaoFactory.TipoMedicionDAO.GetList().OrderBy(t => t.Descripcion);

            foreach (var tipo in tipos) autoBindeable.AddItem(tipo.Descripcion, tipo.Id);
        }

        public void BindEntidades(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idDispositivo = autoBindeable.ParentSelected<Dispositivo>();
            var idTipoEntidad = autoBindeable.ParentSelected<TipoEntidad>();

            var entidades = DaoFactory.EntidadDAO.GetList(new[] { idEmpresa }, 
                                                          new[] { idLinea },
                                                          new[] { idDispositivo},
                                                          new[] { idTipoEntidad })
                                                 .OrderBy(t => t.Descripcion);

            foreach (var entidad in entidades) autoBindeable.AddItem(entidad.Descripcion, entidad.Id);
        }


        public void BindSubEntidades(SubEntidadDropDownList autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTipoEntidad = autoBindeable.ParentSelected<TipoEntidad>();
            var idEntidad = autoBindeable.ParentSelected<EntidadPadre>();
            var idDispositivo = autoBindeable.ParentSelected<Dispositivo>();
            var idSensor = autoBindeable.ParentSelected<Sensor>();
            var tiposMedicion = new string[] {};
            if (autoBindeable.TipoMedicion != null)
            {
                tiposMedicion = autoBindeable.TipoMedicion.Contains(',') 
                                ? autoBindeable.TipoMedicion.Split(',') 
                                : new[] { autoBindeable.TipoMedicion };
            }
            
            var subentidades = DaoFactory.SubEntidadDAO.GetList(new[] { idEmpresa },
                                                                new[] { idLinea },
                                                                new[] { idTipoEntidad },
                                                                new[] { idEntidad }, 
                                                                new[] { idDispositivo },
                                                                new[] { idSensor})
                                                       .Where(s => (s.Sensor != null 
                                                                   && (tiposMedicion.Contains(s.Sensor.TipoMedicion.Codigo)) 
                                                                        || !tiposMedicion.Any()))
                                                       .OrderBy(t => t.Descripcion);

            foreach (var subentidad in subentidades) autoBindeable.AddItem(subentidad.Descripcion, subentidad.Id);
        }

        public void BindSubEntidadesList(SubEntidadListBox autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTipoEntidad = autoBindeable.ParentSelected<TipoEntidad>();
            var idEntidades = autoBindeable.ParentSelectedValues<EntidadPadre>();
            var idDispositivo = autoBindeable.ParentSelected<Dispositivo>();
            var tipoMedicion = DaoFactory.TipoMedicionDAO.FindByCode(autoBindeable.TipoMedicion);
            
            var sensores = new List<int>{-1};
            if (tipoMedicion != null)
            {    
                var s = DaoFactory.SensorDAO.GetList(new[] {idEmpresa},
                                                     new[] {idLinea},
                                                     new[] {idDispositivo},
                                                     new[] {tipoMedicion.Id});

                if (s.Count == 0)
                    return;
                
                sensores = s.Select(sens => sens.Id).ToList();
            }

            IEnumerable<SubEntidad> subentidades = DaoFactory.SubEntidadDAO.GetList(new[] { idEmpresa },
                                                                                    new[] { idLinea },
                                                                                    new[] { idTipoEntidad },
                                                                                    idEntidades,
                                                                                    new[] { idDispositivo },
                                                                                    sensores)
                                                                           .OrderBy(t => t.Descripcion);
            
            if (autoBindeable.UseOptionGroup) subentidades = OrderSubEntidadByOptionGroupProperty(autoBindeable.OptionGroupProperty, subentidades);

            foreach (var subentidad in subentidades)
            {
                var groupDescription = new OptionGroupConfiguration();

                if (autoBindeable.UseOptionGroup) groupDescription = GetSubEntidadGroupDescription(autoBindeable.OptionGroupProperty, subentidad);
                
                autoBindeable.AddItem(subentidad.Descripcion, subentidad.Id, groupDescription);
            }
        }

        public void BindSensores(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idDispositivo = autoBindeable.ParentSelected<Dispositivo>();
            var idTipoMedicion = autoBindeable.ParentSelected<TipoMedicion>();
            var sensores = DaoFactory.SensorDAO.GetSensoresLibres(new[] {idEmpresa},
                                                                  new[] {idLinea},
                                                                  new[] {idDispositivo},
                                                                  new[] {idTipoMedicion})
                                               .OrderBy(s => s.Descripcion);
            
            foreach (var sensor in sensores) autoBindeable.AddItem(sensor.Descripcion, sensor.Id);
        }

        public void BindPrecinto(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var precintos = DaoFactory.PrecintoDAO.GetPrecintosLibres().OrderBy(t => t.Codigo);

            foreach (var precinto in precintos) autoBindeable.AddItem(precinto.Codigo, precinto.Id);
        }

        public void BindDetalle(DetalleDropDownList autoBindeable)
        {
            IEnumerable<Detalle> detalles;

            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var idLinea = autoBindeable.ParentSelected<Linea>();
            var idTipoEntidad = autoBindeable.ParentSelected<TipoEntidad>();

            if (autoBindeable.BindPadres)
            {
                AddDefaultItems(autoBindeable);

                detalles = DaoFactory.DetalleDAO.GetList(new[] {idEmpresa},
                                                         new[] {idLinea},
                                                         new[] {idTipoEntidad},
                                                         new[] {-1});
            }
            else
                detalles = (IEnumerable<Detalle>) (autoBindeable.DataSource ?? new List<Detalle>());

            detalles = detalles.OrderBy(d => d.Nombre);

            foreach (var detalle in detalles)
            {
                var nombre = idTipoEntidad > 0
                                 ? detalle.Nombre
                                 : detalle.Nombre + " (" + detalle.TipoEntidad.Descripcion + ")";

                autoBindeable.AddItem(nombre, detalle.Id);
            }
        }

        public void BindTipoDetalle(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("TEXTO"), 1);
            autoBindeable.AddItem(CultureManager.GetLabel("NUMERICO"), 2);
            autoBindeable.AddItem(CultureManager.GetLabel("FECHA"), 3);
        }

        public void BindRepresentacion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("TEXTBOX"), 1);
            autoBindeable.AddItem(CultureManager.GetLabel("LISTA"), 2);
            autoBindeable.AddItem(CultureManager.GetLabel("SELECCION_MULTIPLE"), 3);
        }

        public void BindUnion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("UNION_Y"), 1);
            autoBindeable.AddItem(CultureManager.GetLabel("UNION_O"), 2);
        }

        public void BindOperador(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("IGUAL_A"), 1);
            autoBindeable.AddItem(CultureManager.GetLabel("MAYOR_A"), 2);
            autoBindeable.AddItem(CultureManager.GetLabel("MENOR_A"), 3);
            autoBindeable.AddItem(CultureManager.GetLabel("CONTIENE_A"), 4);
        }

        public void BindUnidadMedida(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var unidades = DaoFactory.UnidadMedidaDAO.GetList();

            foreach (var unidadMedida in unidades) autoBindeable.AddItem(unidadMedida.Descripcion, unidadMedida.Id);
        }

        public void BindPeriodo(IPeriodoAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            var idEmpresa = autoBindeable.ParentSelected<Empresa>();
            var periodos = DaoFactory.PeriodoDAO.GetByYear(idEmpresa, autoBindeable.Year);

            foreach (Periodo periodo in periodos) autoBindeable.AddItem(periodo.Descripcion, periodo.Id);
        }
        
        public void BindDepartamentos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var departamentos = DaoFactory.DepartamentoDAO.GetList(empresas, lineas);

            foreach(var departamento in departamentos) autoBindeable.AddItem(departamento.Descripcion, departamento.Id);
        }
        
        public void BindPuertas(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var puertas = DaoFactory.PuertaAccesoDAO.GetList(empresas, lineas).OrderBy(p => p.Descripcion);
            
            foreach (var puerta in puertas) autoBindeable.AddItem(puerta.Descripcion, puerta.Id);
        }
        
        public void BindRecorrido(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var recorridos = DaoFactory.RecorridoDAO.GetList(empresas, lineas).OrderBy(p => p.Nombre);

            foreach (var r in recorridos) autoBindeable.AddItem(r.Nombre, r.Id);
        }

        public void BindDeposito(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var depositos = DaoFactory.DepositoDAO.GetList(empresas, lineas).OrderBy(d => d.Descripcion);

            foreach (var d in depositos) autoBindeable.AddItem(d.Descripcion, d.Id);
        }

        public void BindTiposMovimiento(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("DEPOSITO_A_DEPOSITO"), ConsumoCabecera.TiposMovimiento.DepositoADeposito);
            autoBindeable.AddItem(CultureManager.GetLabel("DEPOSITO_A_VEHICULO"), ConsumoCabecera.TiposMovimiento.DepositoAVehiculo);
            autoBindeable.AddItem(CultureManager.GetLabel("PROVEEDOR_A_DEPOSITO"), ConsumoCabecera.TiposMovimiento.ProveedorADeposito);
            autoBindeable.AddItem(CultureManager.GetLabel("PROVEEDOR_A_VEHICULO"), ConsumoCabecera.TiposMovimiento.ProveedorAVehiculo);
        }
        
        public void BindTipoServicioCiclo(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var items = DaoFactory.TipoServicioCicloDAO.GetList(empresas, lineas).OrderBy(d => d.Descripcion);

            foreach (var d in items) autoBindeable.AddItem(d.Descripcion, d.Id);
        }

        public void BindCategoria(CategoriaDropDownList categoriaDropDownList)
        {
            categoriaDropDownList.ClearItems();
            AddDefaultItems(categoriaDropDownList);

            var empresas = categoriaDropDownList.ParentSelectedValues<Empresa>();
            var categorias = DaoFactory.CategoriaDAO.GetList(empresas)
                                                    .Where(c => c.TipoProblema == categoriaDropDownList.TipoProblema)
                                                    .OrderBy(c => c.Descripcion);
            
            foreach (var categoria in categorias) categoriaDropDownList.AddItem(categoria.Descripcion, categoria.Id);
        }

        public void BindCategoria(CategoriaListBox categoriaListBox)
        {
            categoriaListBox.ClearItems();
            AddDefaultItems(categoriaListBox);

            var empresas = categoriaListBox.ParentSelectedValues<Empresa>();
            var categorias = DaoFactory.CategoriaDAO.GetList(empresas)
                                                    .Where(c => c.TipoProblema == categoriaListBox.TipoProblema)
                                                    .OrderBy(c => c.Descripcion);

            foreach (var categoria in categorias) categoriaListBox.AddItem(categoria.Descripcion, categoria.Id);
        }

        public void BindSubCategoria(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var categorias = autoBindeable.ParentSelectedValues<Categoria>();
            var subcategorias = DaoFactory.SubCategoriaDAO.GetList(empresas, categorias).OrderBy(c => c.Descripcion);

            foreach (var subcategoria in subcategorias) autoBindeable.AddItem(subcategoria.Descripcion, subcategoria.Id);
        }

        public void BindNivel(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var niveles = DaoFactory.NivelDAO.GetList(empresas).OrderBy(c => c.Descripcion);

            foreach (var nivel in niveles) autoBindeable.AddItem(nivel.Descripcion, nivel.Id);
        }

        public void BindTipoZona(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();
            var items = DaoFactory.TipoZonaDAO.GetList(empresas, lineas).OrderBy(c => c.Descripcion);

            foreach (var item in items) autoBindeable.AddItem(item.Descripcion, item.Id);
        }

        public void BindTipoDistribucion(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            autoBindeable.AddItem(CultureManager.GetLabel("TIPODISTRI_NORMAL"), ViajeDistribucion.Tipos.Ordenado);
            autoBindeable.AddItem(CultureManager.GetLabel("TIPODISTRI_DESORDENADA"), ViajeDistribucion.Tipos.Desordenado);
            autoBindeable.AddItem(CultureManager.GetLabel("TIPODISTRI_RECORRIDO_FIJO"), ViajeDistribucion.Tipos.RecorridoFijo);
        }

        public void BindNivelComplejidad(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.NivelesComplejidad.GetLabelVariableName(TicketMantenimiento.NivelesComplejidad.Baja)), TicketMantenimiento.NivelesComplejidad.Baja);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.NivelesComplejidad.GetLabelVariableName(TicketMantenimiento.NivelesComplejidad.Media)), TicketMantenimiento.NivelesComplejidad.Media);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.NivelesComplejidad.GetLabelVariableName(TicketMantenimiento.NivelesComplejidad.Alta)), TicketMantenimiento.NivelesComplejidad.Alta);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.NivelesComplejidad.GetLabelVariableName(TicketMantenimiento.NivelesComplejidad.MuyAlta)), TicketMantenimiento.NivelesComplejidad.MuyAlta);
        }

        public void BindEstadosTicketMantenimiento(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(TicketMantenimiento.EstadosTicket.Ingresado)), TicketMantenimiento.EstadosTicket.Ingresado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(TicketMantenimiento.EstadosTicket.Aprobado)), TicketMantenimiento.EstadosTicket.Aprobado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(TicketMantenimiento.EstadosTicket.Terminado)), TicketMantenimiento.EstadosTicket.Terminado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(TicketMantenimiento.EstadosTicket.Aceptado)), TicketMantenimiento.EstadosTicket.Aceptado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(TicketMantenimiento.EstadosTicket.NoAceptado)), TicketMantenimiento.EstadosTicket.NoAceptado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(TicketMantenimiento.EstadosTicket.Cancelado)), TicketMantenimiento.EstadosTicket.Cancelado);
        }

        public void BindEstadosPresupuesto(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.SinPresupuesto)), TicketMantenimiento.EstadosPresupuesto.SinPresupuesto);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.Presupuestado)), TicketMantenimiento.EstadosPresupuesto.Presupuestado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.Recotizado)), TicketMantenimiento.EstadosPresupuesto.Recotizado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.Aprobado)), TicketMantenimiento.EstadosPresupuesto.Aprobado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.Terminado)), TicketMantenimiento.EstadosPresupuesto.Terminado);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.VerificadoSinAprobar)), TicketMantenimiento.EstadosPresupuesto.VerificadoSinAprobar);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.AceptadoCliente)), TicketMantenimiento.EstadosPresupuesto.AceptadoCliente);
            autoBindeable.AddItem(CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(TicketMantenimiento.EstadosPresupuesto.Cancelado)), TicketMantenimiento.EstadosPresupuesto.Cancelado);
        }
        
        public void BindTalleres(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var items = DaoFactory.TallerDAO.GetList().OrderBy(c => c.Descripcion);

            foreach (var item in items) autoBindeable.AddItem(item.Descripcion, item.Id);
        }

        public void BindTotalizador(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();

            AddDefaultItems(autoBindeable);

            var totalizadores = new List<short>();

            var autUser = WebSecurity.AuthenticatedUser;
            if (autUser != null)
            {
                var usuario = DaoFactory.UsuarioDAO.FindById(autUser.Id);
                if (usuario != null && usuario.PorEmpresa)
                {
                    foreach (Empresa empresa in usuario.Empresas)
                    {
                        totalizadores.AddRange(empresa.Totalizadores);
                    }
                }
                else
                {
                    totalizadores = Coche.Totalizador.Totalizadores;
                }
            }

            foreach (var totalizador in totalizadores.Distinct())
            {
                autoBindeable.AddItem(CultureManager.GetLabel(Coche.Totalizador.GetLabelVariableName(totalizador)), totalizador);
            }
        }

        public void BindTurnos(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var lineas = autoBindeable.ParentSelectedValues<Linea>();

            var items = DaoFactory.ShiftDAO.GetList(empresas, lineas).OrderBy(t => t.Descripcion);

            foreach (var item in items) autoBindeable.AddItem(item.Descripcion, item.Id);
        }

        public void BindViajesProgramados(IAutoBindeable autoBindeable)
        {
            autoBindeable.ClearItems();
            AddDefaultItems(autoBindeable);

            var empresas = autoBindeable.ParentSelectedValues<Empresa>();
            var transportistas = autoBindeable.ParentSelectedValues<Transportista>();

            var items = DaoFactory.ViajeProgramadoDAO.GetList(empresas, transportistas).OrderBy(t => t.Codigo);

            foreach (var item in items) autoBindeable.AddItem(item.Codigo, item.Id);
        }

        #endregion
        
        #region Private Methods

        private static void AddDefaultItems(IAutoBindeable autoBindeable)
        {
            if (autoBindeable.AddAllItem) autoBindeable.AddItem(autoBindeable.AllItemsName, autoBindeable.AllValue.ToString("#0"));
            if (autoBindeable.AddNoneItem) autoBindeable.AddItem(autoBindeable.NoneItemsName, autoBindeable.NoneValue.ToString("#0"));
        }

        private static OptionGroupConfiguration GetMobileGroupDescription(MovilOptionGroupValue optionGroupProperty, Coche movil)
        {
            var groupDesc = new OptionGroupConfiguration();

            switch (optionGroupProperty)
            {
                case MovilOptionGroupValue.TipoVehiculo: groupDesc.OptionGroupDescription = movil.TipoCoche != null ? movil.TipoCoche.Descripcion : String.Empty; break;
                case MovilOptionGroupValue.Estado: groupDesc = (OptionGroupConfiguration)EstadoCoches.ElementAt(movil.Estado).Second; break;
            }

            return groupDesc;
        }

        private static OptionGroupConfiguration GetSubEntidadGroupDescription(SubEntidadOptionGroupValue optionGroupProperty, SubEntidad subentidad)
        {
            var groupDesc = new OptionGroupConfiguration();

            switch (optionGroupProperty)
            {
                case SubEntidadOptionGroupValue.Entidad: groupDesc.OptionGroupDescription = subentidad.Entidad != null ? subentidad.Entidad.Descripcion : String.Empty; break;
            }

            return groupDesc;
        }

        private static List<Coche> OrderMobilesByOptionGroupProperty(MovilOptionGroupValue optionGroupProperty, List<Coche> coches)
        {
            switch (optionGroupProperty)
            {
                case MovilOptionGroupValue.TipoVehiculo: coches = (from Coche coche in coches orderby coche.TipoCoche.Descripcion select coche).ToList(); break;
                case MovilOptionGroupValue.Estado: coches = (from Coche coche in coches orderby coche.Estado select coche).ToList(); break;
            }

            return coches;
        }

        private static IEnumerable<SubEntidad> OrderSubEntidadByOptionGroupProperty(SubEntidadOptionGroupValue optionGroupProperty, IEnumerable<SubEntidad> subentidades)
        {
            switch (optionGroupProperty)
            {
                case SubEntidadOptionGroupValue.Entidad: subentidades = (from SubEntidad subentidad in subentidades orderby subentidad.Entidad.Descripcion select subentidad).ToList(); break;
            }

            return subentidades;
        }

        #endregion

        
    }
}
