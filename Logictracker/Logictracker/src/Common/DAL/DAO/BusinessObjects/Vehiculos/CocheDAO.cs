using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects.Positions;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects.Vehiculos
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametros
    /// </remarks>
    public class CocheDAO : GenericDAO<Coche>
    {
        private static readonly Dictionary<int, object> LocksByDevice = new Dictionary<int, object>();

        private static object GetLockByDevice(int vehicle)
        {
            lock (LocksByDevice)
            {
                if (!LocksByDevice.ContainsKey(vehicle)) LocksByDevice.Add(vehicle, new object());
                return LocksByDevice[vehicle];
            }
        }

//        public CocheDAO(ISession session) : base(session) { }

        #region Private Methods
        private DetachedCriteria GetDetachedCriteria(bool withDevice)
        {
            var dc = DetachedCriteria.For<Coche>()
                .SetProjection(Projections.Property("Id"));

            if (withDevice)
                dc.CreateAlias("Dispositivo", "d", JoinType.InnerJoin);

            return dc;
        }

        private DetachedCriteria GetDetachedCriteriaByDevice(int top, int[] deviceIds)
        {
            var dc = GetDetachedCriteria(true)                
                .Add(Restrictions.In("d.Id", deviceIds));

            if (top > 0)
                dc.SetMaxResults(top);

            return dc;
        }

        private DetachedCriteria GetDetachedCriteria(int top, int[] cochesIds)
        {
            var dc = GetDetachedCriteria(true)
                .Add(Restrictions.In("Id", cochesIds));

            if (top > 0)
                dc.SetMaxResults(top);

            return dc;
        }

        private DetachedCriteria GetDetachedCriteriaForChofer(int top, int choferId)
        {
            var dc = GetDetachedCriteria(false)
                .CreateAlias("Chofer", "ch", JoinType.InnerJoin)
                .Add(Restrictions.Eq("ch.Id", choferId));

            if (top > 0)
                dc.SetMaxResults(top);

            return dc;
        }

        private DetachedCriteria GetDetachedCriteriaForModelo(int top, int modeloId)
        {
            var dc = GetDetachedCriteria(false)
                .CreateAlias("Modelo", "m", JoinType.InnerJoin)
                .Add(Restrictions.Eq("m.Id", modeloId));

            if (top > 0)
                dc.SetMaxResults(top);

            return dc;
        }

        private ICriteria GetCriteria(DetachedCriteria dc, Order order)
        {
            var c = Session.CreateCriteria<Coche>()
                .Add(Subqueries.PropertyIn("Id", dc))
                .SetCacheable(true);

            if (order != null)
                c.AddOrder(order);

            return c;
        }

        #endregion

        #region Find Methods

        /// <summary>
        /// Finds all active vehicles: Activos o En Mantenimiento.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Coche> FindAllActivos()
        {
            var dc = GetDetachedCriteria(false);
            dc.Add(Restrictions.Lt("Estado", Coche.Estados.Inactivo));
            var c = GetCriteria(dc, null);
            return c.List<Coche>();
        }

        /// <summary>
        /// Finds all active vehicles: Activos o En Mantenimiento con Dispositivo Asignado.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Coche> FindAllAssigned()
        {
            var dc = GetDetachedCriteria(true);
            dc.Add(Restrictions.Lt("Estado", Coche.Estados.Inactivo));
            var c = GetCriteria(dc, null);
            return c.List<Coche>();
        }

        /// <summary>
        /// Gets the vehicle associated to the specified plate.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="patente"></param>
        /// <returns></returns>
        public Coche FindByPatente(int empresa, string patente)
        {
            return Query.FilterEmpresa(Session, new[]{ empresa }).FirstOrDefault(c => c.Patente == patente);
        }

        public IEnumerable<Coche> FindByPatentes(int empresa, IEnumerable<string> patentes)
        {
            return Query.FilterEmpresa(Session, new[] { empresa })
                        .Where(v => patentes.Contains(v.Patente))
                        .ToList();
        }

        public Coche FindByInterno(IEnumerable<int> empresas, IEnumerable<int> lineas, string interno)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .Where(c => c.Interno.ToUpperInvariant() == interno.ToUpperInvariant())
                        .FirstOrDefault(c => c.Estado != Coche.Estados.Inactivo);
        }

        public Coche FindByChofer(int chofer)
        {
            var dc = GetDetachedCriteriaForChofer(1, chofer)
                .Add(Restrictions.Not(Restrictions.Eq("Estado", Coche.Estados.Inactivo)));

            return GetCriteria(dc, null).UniqueResult<Coche>();
        }
        /// <summary>
        /// Gets the vehicle assigned to the specified device id.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public Coche FindMobileByDevice(int deviceId)
        {
            lock (GetLockByDevice(deviceId))
            {
            	var devVeh = RetrieveDevKey(deviceId);
                if (!string.IsNullOrEmpty(devVeh))
                {
                    var cocheId = Convert.ToInt32(devVeh);
                    var coche = cocheId == 0 ? null : FindById(cocheId);
                    if (coche == null)
                    {
                        STrace.Error("FindMobileByDevice", deviceId, "coche == null");
                        return null;
                    }
                    if (coche.Dispositivo == null)
                    {
                        STrace.Error("FindMobileByDevice", deviceId, "coche.Dispositivo == null");
                        return null;
                    }
                    if (coche.Dispositivo.Id != deviceId)
                    {
                        STrace.Error("FindMobileByDevice", deviceId, "coche.Dispositivo.Id != deviceId");
                        return null;
                    }
                    return coche;
                }

                try
                {
                    var dc = GetDetachedCriteriaByDevice(0, new[] { deviceId });
                    var c = GetCriteria(dc, null);

                    var cocheObjs = c.List<Coche>();
                    if (cocheObjs.Count() > 1)
                    {
                    	throw new ApplicationException("Dispositivo asignado a mas de un Vehiculo");
                    }

                    var cocheObj = cocheObjs.FirstOrDefault();
                    StoreDevKey(deviceId, cocheObj);
                    return cocheObj;
                }
                catch (Exception e)
                {
					STrace.Exception(typeof(CocheDAO).FullName, e, deviceId);
                    return null;
                }
            }
        }

        public IQueryable<Coche> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return FindList(empresas, lineas, new[] {-1});
        }

        public IQueryable<Coche> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo)
        {
            return FindList(empresas, lineas, tipoVehiculo, null);
        }
        public IQueryable<Coche> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, Usuario user)
        {
            return Query.FilterEmpresa(Session, empresas, user)
                        .FilterLinea(Session, empresas, lineas, user)
                        .FilterTipoVehiculo(Session, empresas, lineas, tipoVehiculo, user);
        }

        public List<Coche> FindByModelo(int modelo)
        {
            var dc = GetDetachedCriteriaForModelo(0, modelo);
            return GetCriteria(dc, null).List<Coche>().ToList();
        }

        public List<Coche> FindByTipo(int tipo)
        {
            return Query.FilterTipoVehiculo(Session, new[]{-1}, new[]{-1}, new[]{tipo}).ToList();
        }

        #endregion

        #region Get Methods

        public Coche GetGenerico(Empresa empresa)
        {
            return GetGenerico(empresa, null);
        }

        public Coche GetGenerico(Empresa empresa, Transportista transportista)
        {
            var generico = "(Generico)";
            if (transportista != null)
            {
                var descripcion = transportista.Descripcion.Length > 19 
                                    ? transportista.Descripcion.Substring(0, 19) 
                                    : transportista.Descripcion;
                generico = "(Generico - " + descripcion + ")";
            }
                
            var idEmpresa = empresa != null ? empresa.Id : -1;
            var coche = FindByInterno(new[]{idEmpresa}, new[]{-1}, generico);

            if (coche != null && coche.Dispositivo != null)
            {
                return coche;
            }
            if (coche == null)
            {
                var modeloDAO = new ModeloDAO();
                var tipoCocheDAO = new TipoCocheDAO();
                coche = new Coche
                            {
                                Empresa = empresa,
                                Linea = null,
                                Transportista = transportista,
                                Interno = generico,
                                IdentificaChoferes = true,
                                AnioPatente = 2000,
                                Modelo = modeloDAO.GetList(new[] {idEmpresa}, new[] {-1}, new[] {-1}).First()
                            };
                coche.Marca = coche.Modelo.Marca;
                coche.ModeloDescripcion = coche.Modelo.Descripcion;
                coche.NroChasis = generico;
                coche.NroMotor = generico;
                coche.Patente = generico;
                coche.Poliza = generico.Length > 16 ? generico.Substring(0, 16) : generico;
                coche.TipoCoche = tipoCocheDAO.FindByEmpresasAndLineas(new List<int> {idEmpresa}, new List<int> {-1}, null)
                                              .Cast<TipoCoche>().First();
            }
            if (coche.Dispositivo == null)
            {
                var dispositivoDAO = new DispositivoDAO();
                coche.Dispositivo = dispositivoDAO.GetGenericDevice(empresa, transportista);
            }

            coche.Estado = Coche.Estados.Activo;
            SaveOrUpdate(coche);
            return coche;
        }

        public Coche GetByInterno(IEnumerable<int> empresas, IEnumerable<int> lineas,  string interno)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(c => c.Interno == interno)
                        .FirstOrDefault(c => c.Estado != Coche.Estados.Inactivo);
        }

        public List<Coche> GetByInternos(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<string> internos)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(c => internos.Contains(c.Interno))
                        .Where(c => c.Estado != Coche.Estados.Inactivo)
                        .ToList();
        }

        public Coche GetByInternoEndsWith(IEnumerable<int> empresas, IEnumerable<int> lineas, string interno)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(c => c.Interno.EndsWith(interno))
                        .FirstOrDefault(c => c.Estado != Coche.Estados.Inactivo);
        }

    	/// <summary>
    	/// Gets the vehicle associated to the specified plate.
    	/// </summary>
    	/// <param name="lineas"></param>
    	/// <param name="patente"></param>
    	/// <param name="empresas"></param>
    	/// <returns></returns>
    	public Coche GetByPatente(IEnumerable<int> empresas, IEnumerable<int> lineas, string patente)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(c => c.Patente == patente)
                        .FirstOrDefault(c => c.Estado != Coche.Estados.Inactivo);
        }

        /// <summary>
        /// Gets the vehicles associated to the specified plate list.
        /// </summary>
        /// <param name="lineas"></param>
        /// <param name="patentes"></param>
        /// <param name="empresas"></param>
        /// <returns></returns>
        public List<Coche> GetByPatentes(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<string> patentes)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(c => patentes.Contains(c.Patente))
                        .Where(c => c.Estado != Coche.Estados.Inactivo)
                        .ToList();
        }

        /// <summary>
        /// Gets the vehicles associated to the specified Ids list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<Coche> GetByIds(IEnumerable<int> ids)
        {
            var table = Ids2DataTable(ids);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_CocheDAO_GetByIds] @vehiculosIds = :ids, @estadoInactivo = :estadoInactivo;")
                              .AddEntity(typeof (Coche))
                              .SetStructured("ids", table)
                              .SetInt32("estadoInactivo", Coche.Estados.Inactivo);            
            var results = sqlQ.List<Coche>();
            return results;
        }
        
        public List<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<short> estados)
        {
            var listEstados = estados.ToList();
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(c => listEstados.Contains(c.Estado))
                        .ToList();
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return GetList(empresas, lineas, new[] {-1});
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo)
        {
            return GetList(empresas, lineas, tipoVehiculo, new[] { -1 });
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, new[] {-1});
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, new[] { -1 });
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, new[] { -1 });
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, costSubCenters, new[] { -1 });
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters, bool soloConDispositivo)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, costSubCenters, soloConDispositivo, false);
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters, bool soloConDispositivo, bool soloConGarmin)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, costSubCenters, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, soloConDispositivo, soloConGarmin);
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters, IEnumerable<int> clientes)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, costSubCenters, clientes, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, false, false);
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters, IEnumerable<int> clientes, IEnumerable<int> marcas, IEnumerable<int> modelos, IEnumerable<int> empleados)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, costSubCenters, clientes, marcas, modelos, new[]{-1},empleados);        
        }
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters, IEnumerable<int> clientes, IEnumerable<int> marcas, IEnumerable<int> modelos, IEnumerable<int> tipoEmpleados, IEnumerable<int> empleados)
        {
            return GetList(empresas, lineas, tipoVehiculo, transportistas, departamentos, costCenters, costSubCenters, clientes, marcas, modelos, tipoEmpleados, empleados, false , false );
        }
         
        public IQueryable<Coche> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipoVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> costCenters, IEnumerable<int> costSubCenters, IEnumerable<int> clientes, IEnumerable<int> marcas, IEnumerable<int> modelos, IEnumerable<int> tipoEmpleados, IEnumerable<int> empleados, bool soloConDispositivo, bool soloConGarmin)
        {
            var dc = DetachedCriteria.For<Coche>("c").SetProjection(Projections.Property("c.Id"));

            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var includesAllEmpresas = QueryExtensions.IncludesAll(empresas);
            var includesAllLineas = QueryExtensions.IncludesAll(lineas);
            var includesAllCostCenters = QueryExtensions.IncludesAll(costCenters);
            var includesAllDepartamentos = QueryExtensions.IncludesAll(departamentos);
            var includesAllCostSubCenters = QueryExtensions.IncludesAll(costSubCenters);
            var includesAllTransportistas = QueryExtensions.IncludesAll(transportistas);
            var includesAllTipoEmpleados = QueryExtensions.IncludesAll(tipoEmpleados);
            var includesAllEmpleados = QueryExtensions.IncludesAll(empleados);
            var includesAllTipoVehiculo = QueryExtensions.IncludesAll(tipoVehiculo);
            var includesAllMarcas = QueryExtensions.IncludesAll(marcas);
            var includesAllModelos = QueryExtensions.IncludesAll(modelos);

            #region Security Stuff
            
            if (user != null && user.PorEmpresa)
            {
                var permittedEmpresasList = user.Empresas.Cast<Empresa>().Select(e => e.Id).ToList();
                empresas = includesAllEmpresas ? permittedEmpresasList : empresas.Intersect(permittedEmpresasList);
            }

            if (user != null && user.PorLinea)
            {
                var permittedLineasList = user.Lineas.Cast<Linea>().Select(e => e.Id).ToList();
                lineas = includesAllLineas ? permittedLineasList : lineas.Intersect(permittedLineasList);
            }

            if (user != null && (user.PorTransportista || !user.MostrarSinTransportista))
            {
                var permittedTransportistasList =  user.Transportistas.Cast<Transportista>().Select(e => e.Id).ToList();
                transportistas = includesAllTransportistas ? permittedTransportistasList : transportistas.Intersect(permittedTransportistasList);
            }

            if (user != null && user.PorCentroCostos)
            {
                var permittedCentroDeCostosList = user.CentrosCostos.Cast<CentroDeCostos>().Select(e => e.Id).ToList();
                costCenters = includesAllCostCenters ? permittedCentroDeCostosList : costCenters.Intersect(permittedCentroDeCostosList);
            }

            if (user != null && user.PorCoche)
            {
                var permittedCochesList = user.Coches.Cast<Coche>().Select(e => e.Id).ToList();
                dc.Add(Restrictions.In("c.Id", permittedCochesList));
            }

            #endregion Security Stuff

            if (!includesAllEmpresas || (user != null && user.PorEmpresa))
                dc.CreateAlias("Empresa", "e").Add(Restrictions.In("e.Id", empresas.ToArray()));

            if (!includesAllLineas || (user != null && user.PorLinea))
                dc.CreateAlias("Linea", "l").Add(Restrictions.In("l.Id", lineas.ToArray()));

            if (!includesAllTipoVehiculo)
                dc.CreateAlias("TipoCoche", "tc").Add(Restrictions.In("tc.Id", tipoVehiculo.ToArray()));

            if (!includesAllDepartamentos)
                dc.CreateAlias("Departamento", "dep").Add(Restrictions.In("dep.Id", departamentos.ToArray()));

            if (!includesAllCostCenters || !includesAllDepartamentos || (user != null && user.PorCentroCostos))
            {
                dc.CreateAlias("CentroDeCostos", "cdc");
                //if (!includesAllDepartamentos)
                //{
                //    var dcD = DetachedCriteria.For<CentroDeCostos>("dcdc")
                //                              .CreateAlias("Departamento", "d")
                //                              .Add(Restrictions.In("d.Id", departamentos.ToArray()))
                //                              .SetProjection(Projections.Property("dcdc.Id"));
                //    dc.Add(Subqueries.PropertyIn("cdc.Id", dcD));
                //}

                if (!includesAllCostCenters || user.PorCentroCostos)
                    dc.Add(Restrictions.In("cdc.Id", costCenters.ToArray()));
            }

            if (!includesAllCostSubCenters)
                dc.CreateAlias("SubCentroDeCostos", "scdc").Add(Restrictions.In("scdc.Id", costSubCenters.ToArray()));

            if (!includesAllTransportistas || (!includesAllTipoEmpleados && !includesAllEmpleados) || (user != null && user.PorTransportista))
            {
                if (includesAllTipoEmpleados && includesAllEmpleados)
                {
                    dc.CreateAlias("Transportista", "t");
                    dc.Add(Restrictions.In("t.Id", transportistas.ToArray()));
                }
                else
                {
                    var dcE = DetachedCriteria.For<Empleado>("emp");
                    if (!includesAllTransportistas || user.PorTransportista)
                    {
                        dcE.CreateAlias("Transportista", "empt")
                           .Add(Restrictions.In("empt.Id", transportistas.ToArray()));
                    }

                    if (!includesAllEmpresas || user.PorEmpresa)
                    {
                        dcE.CreateAlias("Empresa", "empe")
                           .Add(Restrictions.In("empe.Id", empresas.ToArray()));
                    }

                    if (!includesAllLineas || user.PorLinea)
                    {
                        dcE.CreateAlias("Linea", "empl")
                           .Add(Restrictions.In("empl.Id", lineas.ToArray()));
                    }

                    if (!includesAllCostCenters || user.PorCentroCostos)
                    {
                        dcE.CreateAlias("CentroDeCostos", "empcdc")
                           .Add(Restrictions.In("empcdc.Id", costCenters.ToArray()));
                    }

                    if (!includesAllDepartamentos)
                    {
                        dcE.CreateAlias("Departamento", "empd")
                           .Add(Restrictions.In("empd.Id", departamentos.ToArray()));
                    }

                    if (!includesAllTipoEmpleados)
                    {
                        dcE.CreateAlias("TipoEmpleado", "temp")
                           .Add(Restrictions.In("temp.Id", tipoEmpleados.ToArray()));
                    }

                    dcE.SetProjection(Projections.Property("emp.Id"));

                    dc.CreateAlias("Chofer", "ch");
                    dc.Add(Subqueries.PropertyIn("ch.Id", dcE));
                }
            }
            
            if (!includesAllMarcas)
            {
                dc.CreateAlias("Marca", "m")
                  .Add(Restrictions.In("m.Id", marcas.ToArray()));
            }

            if (!includesAllModelos)
            {
                dc.CreateAlias("Modelo", "m2")
                  .Add(Restrictions.In("m2.Id", modelos.ToArray()));
            }

            if (soloConDispositivo || soloConGarmin)
            {
                dc.CreateAlias("Dispositivo", "disp");
                if (!soloConGarmin)
                    dc.Add(Restrictions.IsNotNull("disp.Id"));
                else
                {
                    var dddc = DetachedCriteria.For<DetalleDispositivo>("dd")
                                               .CreateAlias("Dispositivo", "ddd")
                                               .CreateAlias("TipoParametro", "ddtp")
                                               .Add(Restrictions.Eq("ddtp.Nombre", "GTE_MESSAGING_DEVICE"))
                                               .Add(Restrictions.Eq("dd.Valor", "GARMIN"))
                                               .SetProjection(Projections.Property("ddd.Id"));
                    dc.Add(Subqueries.PropertyIn("disp.Id", dddc));
                }
            }

            var coches = GetCriteria(dc, null).Future<Coche>().AsQueryable();
            
            if (!QueryExtensions.IncludesAll(clientes))
            {
                var client = clientes.ToList();
                coches = coches.Where(c => c.IsAssignedToAnyClient(client));
            }

            return coches;
        }

        public List<Coche> GetCochesPermitidosPorUsuario(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCostos, IEnumerable<int> subCentrosDeCostos, IEnumerable<int> marcas, IEnumerable<int> modelos, IEnumerable<int> tiposEmpleado, IEnumerable<int> empleados)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var userId = sessionUser != null ? sessionUser.Id : 0;

            var empresasIds = Ids2DataTable(empresas);
            var lineasIds = Ids2DataTable(lineas);
            var tiposVehiculoIds = Ids2DataTable(tiposVehiculo);
            var transportistasIds = Ids2DataTable(transportistas);
            var departamentosIds = Ids2DataTable(departamentos);
            var centrosDeCostosIds = Ids2DataTable(centrosDeCostos);
            var subCentrosDeCostosIds = Ids2DataTable(subCentrosDeCostos);
            var marcasIds = Ids2DataTable(marcas);
            var modelosIds = Ids2DataTable(modelos);
            var tiposEmpleadoIds = Ids2DataTable(tiposEmpleado);
            var empleadoIds = Ids2DataTable(empleados);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_CocheDAO_GetCochesPermitidosPorUsuario] " +
                                              "@empresasIds = :empresasIds, " +
                                              "@lineasIds = :lineasIds, " +
                                              "@tiposVehiculoIds = :tiposVehiculoIds, " +
                                              "@transportistasIds = :transportistasIds, " +
                                              "@departamentosIds = :departamentosIds, " +
                                              "@centrosDeCostosIds = :centrosDeCostosIds, " +
                                              "@subCentrosDeCostosIds = :subCentrosDeCostosIds, " +
                                              "@marcasIds = :marcasIds, " +
                                              "@modelosIds = :modelosIds, " +
                                              "@tiposEmpleadoIds = :tiposEmpleadoIds, " +
                                              "@empleadoIds = :empleadoIds, " +
                                              "@userId = :userId;")
                              .AddEntity(typeof(Coche))
                              .SetStructured("empresasIds", empresasIds)
                              .SetStructured("lineasIds", lineasIds)
                              .SetStructured("tiposVehiculoIds", tiposVehiculoIds)
                              .SetStructured("transportistasIds", transportistasIds)
                              .SetStructured("departamentosIds", departamentosIds)
                              .SetStructured("centrosDeCostosIds", centrosDeCostosIds)
                              .SetStructured("subCentrosDeCostosIds", subCentrosDeCostosIds)
                              .SetStructured("marcasIds", marcasIds)
                              .SetStructured("modelosIds", modelosIds)
                              .SetStructured("tiposEmpleadoIds", tiposEmpleadoIds)
                              .SetStructured("empleadoIds", empleadoIds)
                              .SetInt32("userId", userId);
            var coches = sqlQ.List<Coche>();
            
            return coches.ToList();
        }

        #endregion

        #region OtherMethods

        /// <summary>
        /// Gets the distance traveled by the mobile in the specified time span.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        public double GetDistance(int coche, DateTime inicio, DateTime fin)
        {


            var distance = 0.0;

            if (fin < DateTime.Today)
            {
                var dmDAO = new DatamartDAO();
                var dm = dmDAO.GetMobilesKilometers(inicio, fin, new List<int> {coche}).FirstOrDefault();
                return dm != null ? dm.Kilometers : 0.0;
            }

            //var sqlQ = Session.CreateSQLQuery("SELECT dbo.fn_getVehicleKm(?, ?, ?);");
            //sqlQ.SetInt32(0, coche);
            //sqlQ.SetDateTime(1, inicio);
            //sqlQ.SetDateTime(2, fin);
            //distance = sqlQ.UniqueResult<double>();
            //return distance;

            var lpDAO = new LogPosicionDAO();
            var results = lpDAO.GetPositionsBetweenDates(coche, inicio, fin);

            //if (results.Count.Equals(0))
            //    results = Session.Query<LogPosicionHistorica>()
            //        .Where(position => position.Coche.Id == coche && position.FechaMensaje >= inicio && position.FechaMensaje <= fin)
            //        .Cast<LogPosicionBase>()
            //        .ToList();

            for (var i = 0; i < results.Count - 1; i++)
            {
                var x = results[i];
                var y = results[i + 1];

                distance += Distancias.Loxodromica(x.Latitud, x.Longitud, y.Latitud, y.Longitud);
            }

            return distance/1000.0;
        }

        public double GetRunningHours(int coche, DateTime inicio, DateTime fin)
        {
            var time = 0.0;

            var lpDAO = new LogPosicionDAO();
            var results = lpDAO.GetPositionsBetweenDates(coche, inicio, fin);

            //if (results.Count.Equals(0))
            //    results = Session.Query<LogPosicionHistorica>()
            //        .Where(position => position.Coche.Id == coche && position.FechaMensaje >= inicio && position.FechaMensaje <= fin)
            //        .Cast<LogPosicionBase>()
            //        .ToList();

            for (var i = 0; i < results.Count - 1; i++)
            {
                var x = results[i];
                var y = results[i + 1];

                if (x.MotorOn.HasValue && x.MotorOn.Value)
                    time += y.FechaMensaje.Subtract(x.FechaMensaje).TotalHours; 
            }

            return time;
        }

        #endregion

        #region Override Methods

        public override void Delete(Coche coche)
        {
            if (coche == null) return;
            coche.Estado = Coche.Estados.Inactivo;
            coche.DtCambioEstado = DateTime.UtcNow;
            SaveOrUpdate(coche);
        }

        public override void SaveOrUpdate(Coche obj)
        {
            ValidateDevice(obj);
            SetLastPositionData(obj);

            var oldDev = obj.OldDispositivo;
            base.SaveOrUpdate(obj);
            if (oldDev != null) DeleteDevKey(oldDev.Id);
            if (obj.Dispositivo != null) StoreDevKey(obj.Dispositivo.Id, obj);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates that the specified device is not assigned to another vehicle.
        /// </summary>
        /// <param name="obj"></param>
        private void ValidateDevice(Coche obj)
        {
            if (obj.Dispositivo == null) return;
            var vehicles = Query.Count(vehicle => vehicle.Dispositivo != null && vehicle.Dispositivo.Id == obj.Dispositivo.Id && vehicle.Id != obj.Id);
            if (vehicles > 0) throw new Exception("The specified device is already assigned to another vehicle.");
            obj.Dispositivo.Empresa = obj.Empresa;
        }

        /// <summary>
        /// When data is saved, updates cache info.
        /// </summary>
        /// <param name="vehicle">Vehicle to update</param>
        private static void SetLastPositionData(Coche vehicle)
        {
            if (!vehicle.IsLastPositionInCache()) return;
            var lastPosition = vehicle.RetrieveLastPosition();
            if (lastPosition == null) return;
            lastPosition.ApplyVehicleData(vehicle);
            vehicle.StoreLastPosition(lastPosition);
        }

		private static void StoreDevKey(int deviceId, Coche cocheObj)
        {
			if (cocheObj == null) return; //si estan configurando un dispositivo nuevo y reporta antes de darlo de alta es mejor no guardar nada y esperar a que lo den de alta
            var id =  cocheObj.Id.ToString("#0");
			LogicCache.Store(typeof(String), "DeviceVehicle:" + deviceId, id);
        }

		private static void DeleteDevKey(int deviceId)
        {
			LogicCache.Delete(typeof(String), "DeviceVehicle:" + deviceId);
		}

		private static String RetrieveDevKey(int deviceId)
        {
			return LogicCache.Retrieve<String>(typeof(String), "DeviceVehicle:" + deviceId);
        }

        #endregion
    }
}