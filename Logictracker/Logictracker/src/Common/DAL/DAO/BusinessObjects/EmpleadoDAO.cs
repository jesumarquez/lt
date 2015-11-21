using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Sync;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class EmpleadoDAO : GenericDAO<Empleado>
    {
        private const string LoggedInDriverCacheKey = "LoggedInDriver";
        private const string LastLogCacheKey = "LastLog";

//        public EmpleadoDAO(ISession session) : base(session) { }

        #region Find Methods

        /// <summary>
        /// Finds all active employees with cards
        /// </summary>
        /// <returns></returns>
        public List<Empleado> FindActivosConTarjetas(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(emp => !emp.Baja && emp.Tarjeta != null)
                .ToList();
        }

        /// <summary>
        /// Gets an employee based on its rfid card.
        /// </summary>
        /// <param name="empresa"> </param>
        /// <param name="rfid">Rfid code.</param>
        /// <returns>The associated employee or null if any.</returns>
        public Empleado FindByRfid(int empresa, string rfid)
        {
            return Query.FilterEmpresa(Session, new[]{empresa}, null)
                        .Where(e => e.Tarjeta != null && e.Tarjeta.Pin == rfid)
                        .Cacheable()
                        .SafeFirstOrDefault()
                   ?? 
                   Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .Where(e => e.Tarjeta != null && e.Tarjeta.PinHexa == rfid)
                        .Cacheable()
                        .SafeFirstOrDefault();
        }

        public Empleado FindByTarjeta(int idTarjeta)
        {
            return Query.Where(e => e.Tarjeta != null && e.Tarjeta.Id == idTarjeta)
                        .Cacheable()
						.SafeFirstOrDefault();
        }

        public Empleado FindByCodigoAcceso(int accessCode)
        {
            return Query.Where(e => e.Tarjeta != null && e.Tarjeta.CodigoAcceso != null && e.Tarjeta.CodigoAcceso == accessCode)
                        .Cacheable()
						.SafeFirstOrDefault();
        }

        public Empleado FindByLegajo(int empresa, int linea, string legajo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .Where(q => !q.Baja)
                        .Where(q => q.Legajo == legajo)
						.SafeFirstOrDefault();
        }

        public List<Empleado> FindByLegajos(int empresa, int linea, IEnumerable<string> legajos)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .Where(q => !q.Baja)
                        .Where(q => legajos.Contains(q.Legajo))
                        .ToList();
        }

        /// <summary>
        /// Gets the employee associated to the specified name.
        /// Returns NULL if not found.
        /// </summary>
        /// <param name="upcode"></param>
        /// <returns></returns>
        public Empleado FindByUpcode(string upcode)
        {
            return Query.Where( e => e.Tarjeta != null && e.Tarjeta.Numero == upcode.Replace(" ", "").Replace(",", ""))
						.SafeFirstOrDefault();
        }

        #endregion

        #region Get Methods

        public Empleado GetByLegajo(int empresa, int linea, string legajo)
        {
            return Query.FilterEmpresa(Session, new[]{empresa})
                        .FilterLinea(Session, new[] { empresa }, new[] { linea })
                        .Where(q => !q.Baja)
                        .Where(q => q.Legajo == legajo)
						.SafeFirstOrDefault();
        }

        public Empleado GetById(int empresa, int linea, int id)
        {
            return Query.FilterEmpresa(Session, new[] { empresa })
                        .FilterLinea(Session, new[] { empresa }, new[] { linea })
                        .Where(q => !q.Baja)
                        .Where(q => q.Id == id)
                        .SafeFirstOrDefault();
        }

        public List<Empleado> GetReporta(int empresa, int linea, int id)
        {
            var reporta = new List<Empleado>();
            var empleado = GetById(empresa, linea, id);
            if (empleado.Reporta1 != null) reporta.Add(empleado.Reporta1);
            if (empleado.Reporta2 != null) reporta.Add(empleado.Reporta2);
            if (empleado.Reporta3 != null) reporta.Add(empleado.Reporta3);
            return reporta;
        }

        public List<Empleado> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado, IEnumerable<int> transportistas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTipoEmpleado(Session, empresas, lineas, tiposEmpleado)
                        .FilterTransportista(Session, empresas, lineas, transportistas)
                        .Where(q => !q.Baja)
                        .ToList();
        }
        public List<Empleado> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado, IEnumerable<int> transportistas, IEnumerable<int> categoriasAcceso)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTipoEmpleado(Session, empresas, lineas, tiposEmpleado)
                        .FilterTransportista(Session, empresas, lineas, transportistas)
                        .FilterCategoriaAcceso(Session, empresas, lineas, categoriasAcceso)
                        .Where(q => !q.Baja)
                        .ToList();
        }
        public List<Empleado> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado, IEnumerable<int> transportistas, IEnumerable<int> centrosDeCosto, IEnumerable<int> departamentos)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTipoEmpleado(Session, empresas, lineas, tiposEmpleado)
                        .FilterTransportista(Session, empresas, lineas, transportistas)
                        .FilterCentroDeCostos(Session, empresas, lineas, departamentos, centrosDeCosto)
                        .FilterDepartamento(Session, empresas, lineas, departamentos)
                        .Where(q => !q.Baja)
                        .ToList();
        }
        public List<Empleado> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado, IEnumerable<int> transportistas, IEnumerable<int> centrosDeCosto, IEnumerable<int> departamentos, IEnumerable<int> categoriasAcceso, string legajo)
        {
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas)
                         .FilterTipoEmpleado(Session, empresas, lineas, tiposEmpleado)
                         .FilterTransportista(Session, empresas, lineas, transportistas)
                         .FilterCentroDeCostos(Session, empresas, lineas, departamentos, centrosDeCosto)
                         .FilterDepartamento(Session, empresas, lineas, departamentos)
                         .FilterCategoriaAcceso(Session, empresas, lineas, categoriasAcceso)
                         .Where(emp => !emp.Baja);

            if (legajo != string.Empty)
                q = q.Where(emp => emp.Legajo.Equals(legajo));

            return q.ToList();
        }

        #endregion

        #region Current Driver Cache Methods

        public void SetLoggedInDriver(Coche coche, int? empleado)
        {
            if (!empleado.HasValue) coche.Delete(LoggedInDriverCacheKey);
            else coche.Store(LoggedInDriverCacheKey, empleado.Value);
        }

        public Empleado GetLoggedInDriver(Coche coche)
        {
            if (coche == null) return null;

            if (coche.KeyExists(LoggedInDriverCacheKey))
            {
                var emp = coche.Retrieve<object>(LoggedInDriverCacheKey);
                return emp == null ? !coche.IdentificaChoferes ? coche.Chofer : null : FindById((int)emp);
            }

            var lastLog = new LastVehicleEventDAO().GetEventByVehicleAndType(coche, Coche.Totalizador.UltimoLogin);

            Empleado chofer = null;
            if (lastLog != null)
            {
                chofer = lastLog.Mensaje.Codigo == MessageCode.RfidDriverLogin.GetMessageCode()
                                 ? lastLog.Chofer
                                 : null;
            }

            if (chofer == null && !coche.IdentificaChoferes) chofer = coche.Chofer;

            coche.Store(LoggedInDriverCacheKey, chofer != null ? (object)chofer.Id : null);

            return chofer;
        } 

        #endregion

        #region Last Log in Cache

        public void SetLastLog(Empleado empleado, Empleado.VerificadorEmpleado verificadorEmpleado)
        {
            if (verificadorEmpleado == null) empleado.Delete(LastLogCacheKey);
            else empleado.Store(LastLogCacheKey, verificadorEmpleado);
        }

        public Empleado.VerificadorEmpleado GetLastLog(Empleado empleado)
        {
            if (empleado == null) return null;

            if (empleado.KeyExists(LastLogCacheKey))
            {
                var verificadorEmpleado = empleado.Retrieve<object>(LastLogCacheKey);
                if (verificadorEmpleado != null) return (Empleado.VerificadorEmpleado)verificadorEmpleado;
            }
            
            var mensajes = Session.Query<Mensaje>()
                                  .Where(msg => msg.Codigo == MessageCode.RfidEmployeeLogin.GetMessageCode() 
                                             || msg.Codigo == MessageCode.RfidEmployeeLogout.GetMessageCode())
                                  .Select(msg => msg.Id)
                                  .ToList();

            var lastLog = Session.Query<LogMensaje>()
                                 .Where(message => message.Chofer.Id == empleado.Id
                                                && mensajes.Contains(message.Mensaje.Id))
                                 .OrderByDescending(message => message.Fecha)
                                 .FirstOrDefault();

            var verif = new Empleado.VerificadorEmpleado { Empleado = empleado, TipoFichada = Empleado.VerificadorEmpleado.TipoDeFichada.SinFichar };

            if (lastLog != null)
            {
                verif.Fecha = lastLog.Fecha;

                if (lastLog.Coche != null)
                {
                    var puertaDao = new PuertaAccesoDAO();
                    var puerta = puertaDao.FindByVehiculo(lastLog.Coche.Id);

                    if (puerta != null)
                    {
                        verif.PuertaAcceso = puerta;

                        if (lastLog.Mensaje.Codigo == MessageCode.RfidEmployeeLogin.GetMessageCode())
                        {
                            verif.TipoFichada = Empleado.VerificadorEmpleado.TipoDeFichada.Entrada;
                            if (puerta.ZonaAccesoEntrada != null) verif.ZonaAcceso = puerta.ZonaAccesoEntrada; 
                        }
                        else if (lastLog.Mensaje.Codigo == MessageCode.RfidEmployeeLogout.GetMessageCode())
                        {
                            verif.TipoFichada = Empleado.VerificadorEmpleado.TipoDeFichada.Salida;
                            if (puerta.ZonaAccesoSalida != null) verif.ZonaAcceso = puerta.ZonaAccesoSalida;
                        }
                    }
                }
            }

            empleado.Store(LastLogCacheKey, verif);

            return verif;
        }
        
        // TODO: Devuelve 1? es el correcto?
        public virtual Empleado FindEmpleadoByDevice(Dispositivo dispositivo)
        {
            var dc =
                DetachedCriteria.For<Empleado>("de")
                    .Add(Restrictions.Eq("Dispositivo", dispositivo))
                    .Add(Restrictions.EqProperty("de.Id", "e.Id"))
                    .SetProjection(Projections.Property("Id"));
            var crit = Session.CreateCriteria<Empleado>("e")
                .Add(Subqueries.Exists(dc))
                .SetMaxResults(1);
            return crit.UniqueResult<Empleado>();
        }


        #endregion

        #region Override Methods

        public override void Delete(Empleado chofer)
        {
            if (chofer == null) return;
            
            chofer.Baja = true;
            chofer.Tarjeta = null;
            SaveOrUpdate(chofer);
        }

        public override void SaveOrUpdate(Empleado obj)
        {
            base.SaveOrUpdate(obj);
            EnqueueSync(obj, OutQueue.Queries.Molinete, OutQueue.Operations.Empleado);
        }
        #endregion
    }
}