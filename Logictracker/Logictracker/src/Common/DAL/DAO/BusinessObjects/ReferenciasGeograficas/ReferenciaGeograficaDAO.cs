using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Positions;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObject;
using Logictracker.Utils;
using Logictracker.Utils.NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class ReferenciaGeograficaDAO: GenericDAO<ReferenciaGeografica>
    {
        protected object GeocercasLock = new object();

        #region Find Methods

        public IEnumerable<ReferenciaGeografica> FindByTipo(int idTipoGeoRef)
        {
            DetachedCriteria dc = DetachedCriteria.For<ReferenciaGeografica>("drg")
                .Add(Restrictions.Eq("Baja", false))
                .Add(Restrictions.Eq("TipoReferenciaGeografica.Id", idTipoGeoRef))
                .SetProjection(Projections.Property("Id"));

            return Session.CreateCriteria<ReferenciaGeografica>("rg")
                .Add(Subqueries.PropertyIn("Id", dc)).List<ReferenciaGeografica>();
        }

        public List<ReferenciaGeografica> FindByCodigoStartWith(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, string codigo)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef, null)
                .Where(r => !r.Baja && r.Codigo.StartsWith(codigo))
                .Cacheable()
                .ToList();
        }

        public ReferenciaGeografica FindByCodigo(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, string codigo)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef, null)
                .Where(r => !r.Baja && r.Codigo == codigo)
                .Cacheable()
                .FirstOrDefault();
        }

        public List<ReferenciaGeografica> FindInhibidores(int empresa, int linea)
        {
            var referencias = FindList(new[] { empresa }, new[] { linea }, new[]{-1});
            if (referencias == null || referencias.Count.Equals(0)) return new List<ReferenciaGeografica>();
            return referencias.Where(r => r.Poligono != null && r.InhibeAlarma).ToList();
        }

        public List<ReferenciaGeografica> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef, null)
                .Where(r => !r.Baja)
                .Cacheable()
                .ToList();
        }

        public List<ReferenciaGeografica> FindList(IEnumerable<int> geocercasIds)
        {
            return Query.Where(r => geocercasIds.Contains(r.Id) && !r.Baja)
                        .Cacheable()
                        .ToList();
        }

        #endregion

        #region Get Methods

        public List<ReferenciaGeografica> GetByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, string descripcion)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(r => !r.Baja)
                .ToList()// SQL
                .Where(r => r.Descripcion.ToLower().Contains(descripcion.Trim().ToLower()))//.NET (por el Contains)
                .ToList();
        }

        public ReferenciaGeografica GetByCodigo(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, string codigo)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef)
                .Where(r => !r.Baja && r.Codigo == codigo)
                .Cacheable()
                .FirstOrDefault();
        }

        public List<MobilePoi> GetVehiculosCercanos(IEnumerable<int> empresas, IEnumerable<int> lineas, int referencia)
        {
            var posDao = new LogPosicionDAO();
            var cocheDao = new CocheDAO();

            var r = FindById(referencia);

            var lat = r.Direccion != null ? r.Direccion.Latitud : r.Poligono.Centro.Y;
            var lon = r.Direccion != null ? r.Direccion.Longitud : r.Poligono.Centro.X;

            var coches = cocheDao.GetList(empresas, lineas);

            if (!coches.Any()) return new List<MobilePoi>();

            return posDao.GetLastVehiclesPositions(coches).Values
                .Where(position => position != null)
                .Select(lup => new MobilePoi
                                   {
                                       IdVehiculo = lup.IdCoche,
                                       PuntoDeInteres = r.Descripcion,
                                       Distancia = Distancias.Loxodromica(lup.Latitud, lup.Longitud, lat, lon),
                                       Interno = lup.Coche,
                                       Latitud = lup.Latitud,
                                       Longitud = lup.Longitud,
                                       TipoVehiculo = lup.TipoCoche,
                                       Velocidad = lup.Velocidad
                                   })
                .OrderBy(mp => mp.Distancia)
                .ToList();
        }

        public List<ReferenciaGeografica> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, string SearchString)
        {
            if (string.IsNullOrEmpty(SearchString))
            {
                var q = Query.FilterEmpresa(Session, empresas)
                             .FilterLinea(Session, empresas, lineas);

                if (!QueryExtensions.IncludesAll(tiposGeoRef))
                    q = q.FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef);

                return q.Where(r => !r.Baja)
                        .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow))
                        .Cacheable()
                        .ToList();
            }
            else
            {
                var q = Query.FilterEmpresa(Session, empresas)
                             .FilterLinea(Session, empresas, lineas);

                if (!QueryExtensions.IncludesAll(tiposGeoRef))
                    q = q.FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef);

                return q.Where(r => !r.Baja && (r.Codigo.ToUpper().Contains(SearchString.ToUpper()) || r.Descripcion.ToUpper().Contains(SearchString.ToUpper())))
                        .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow))
                        .Cacheable()
                        .ToList();
            }
        }

        public List<ReferenciaGeografica> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef)
        {
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas);

            if (!QueryExtensions.IncludesAll(tiposGeoRef))
                q = q.FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef);

            return q.Where(r => !r.Baja)
                    .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow))
                    .Cacheable()
                    .ToList();
        }

        public List<ReferenciaGeografica> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, int page, int pageSize, ref int totalRows, bool reCount, string SearchString)
        {

            if (string.IsNullOrEmpty(SearchString))
            {
                var q = Query.FilterEmpresa(Session, empresas)
                      .FilterLinea(Session, empresas, lineas);

                if (!QueryExtensions.IncludesAll(tiposGeoRef))
                    q = q.FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef);
                if (reCount)
                {
                    int count = q.Where(r => !r.Baja)
                          .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow))
                          .Count();

                    if (!totalRows.Equals(count))
                    {
                        totalRows = count;
                    }
                }

                return q.Where(r => !r.Baja)
                        .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Cacheable()
                        .ToList();
            }
            else
            {
                var q = Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas);

                if (!QueryExtensions.IncludesAll(tiposGeoRef))
                    q = q.FilterTipoReferenciaGeografica(Session, empresas, lineas, tiposGeoRef);
                if (reCount)
                {
                    int count = q.Where(r => !r.Baja)
                          .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow)
                          && (r.Codigo.ToUpper().Contains(SearchString.ToUpper()) || r.Descripcion.ToUpper().Contains(SearchString.ToUpper())))
                          .Count();

                    if (!totalRows.Equals(count))
                    {
                        totalRows = count;
                    }
                }

                return q.Where(r => !r.Baja)
                        .Where(r => !r.Vigencia.Fin.HasValue || (r.Vigencia.Fin.HasValue && r.Vigencia.Fin.Value > DateTime.UtcNow)
                        && (r.Codigo.ToUpper().Contains(SearchString.ToUpper()) || r.Descripcion.ToUpper().Contains(SearchString.ToUpper())))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Cacheable()
                        .ToList();
            }
        }        
        
        public IEnumerable<ReferenciaGeografica> GetListForVehicle(Coche vehiculo)
        {
            var empresaId = (vehiculo.Empresa != null ? vehiculo.Empresa.Id : -1);
            var lineaId = (vehiculo.Linea != null ? vehiculo.Linea.Id : -1);
            
            return GetListForEmpresaLinea(empresaId, lineaId);
        }

        public IEnumerable<ReferenciaGeografica> GetListForEmpresaLinea(int empresaId, int lineaId)
        {
            return GetListForEmpresaLinea(empresaId, lineaId, false);
        }

        public IEnumerable<ReferenciaGeografica> GetListForEmpresaLinea(int empresaId, int lineaId, bool conDependencias)
        {
            var dc = DetachedCriteria.For<ReferenciaGeografica>("drg").SetProjection(Projections.Property("drg.Id"));

            if (empresaId != -1)
            {
                dc.Add(Restrictions.Eq("Empresa.Id", empresaId));
            }
            if (lineaId != -1)
            {
                dc.Add(Restrictions.Or(Restrictions.IsNull("Linea.Id"), Restrictions.Eq("Linea.Id", lineaId)));
            }
            dc.Add(Restrictions.Eq("drg.Baja", false));
            dc.Add(Restrictions.Or(Restrictions.IsNull("drg.Vigencia.Inicio"), Restrictions.Lt("drg.Vigencia.Inicio", DateTime.UtcNow)));
            dc.Add(Restrictions.Or(Restrictions.IsNull("drg.Vigencia.Fin"), Restrictions.Gt("drg.Vigencia.Fin", DateTime.UtcNow)));

            var crit = Session.CreateCriteria<ReferenciaGeografica>("rg")
                              .Add(Subqueries.PropertyIn("Id", dc));

            if (conDependencias)
            {
                crit.CreateAlias("TipoReferenciaGeografica", "tr", JoinType.LeftOuterJoin)
                    .CreateAlias("TipoReferenciaGeografica._velocidades", "trv", JoinType.LeftOuterJoin)
                    .CreateAlias("_historia", "h", JoinType.LeftOuterJoin)
                    .CreateAlias("_velocidades", "v", JoinType.LeftOuterJoin)
                    .CreateAlias("_zonas", "z", JoinType.LeftOuterJoin)
                    .CreateAlias("_historia.Direccion", "dir", JoinType.LeftOuterJoin)
                    .CreateAlias("_historia.Poligono", "poly", JoinType.LeftOuterJoin)
                    .CreateAlias("_historia.Poligono._puntos", "points", JoinType.LeftOuterJoin);
            }

            var list = crit.List<ReferenciaGeografica>();

            return list;
        }

        public IEnumerable<Poligono> GetPoligonosVigentes(int empresa, int linea)
        {
            var now = DateTime.UtcNow;
            var result = Session.CreateSQLQuery("exec [dbo].[sp_getPoligonosVigentes] @today = :today, @company = :company, @branch = :branch")
                                .AddEntity("p", typeof(Poligono))
                                .AddJoin("pu", "p._puntos")
                                .AddEntity("p", typeof(Poligono))
                                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                                .SetDateTime("today", now)
                                .SetParameter("company", empresa)
                                .SetParameter("branch", linea)
                                .List<Poligono>();
            return result;
        }

        public IEnumerable<Direccion> GetDireccionesVigentes(int empresa, int linea)
        {
            var now = DateTime.UtcNow;
            var result = Session.CreateSQLQuery("exec [dbo].[sp_getDireccionesVigentes] @today = :today, @company = :company, @branch = :branch")
                                .AddEntity(typeof(Direccion))                
                                .SetDateTime("today", now)
                                .SetParameter("company", empresa)
                                .SetParameter("branch", linea)
                                .List<Direccion>();
            return result;
        }

        public IEnumerable<Geocerca> GetGeocercasFor(Coche vehiculo)
        {
            var empresaId = (vehiculo.Empresa != null ? vehiculo.Empresa.Id : -1);
            var lineaId = (vehiculo.Linea != null ? vehiculo.Linea.Id : -1);

            return GetGeocercasFor(empresaId, lineaId);
        }

        public IEnumerable<Geocerca> GetGeocercasFor(int empresaId, int lineaId)
        {            
            Dictionary<int, Poligono> poligonos;
            Dictionary<int, Direccion> direcciones;

            var t = new TimeElapsed();
            var poligonosList = GetPoligonosVigentes(empresaId, lineaId);
            var ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Trace("DispatcherLock", string.Format("GetPoligonosVigentes({0},{1}): {2} segundos", empresaId, lineaId, ts));

            t.Restart();
            if (poligonosList != null &&
                poligonosList.Any())
                poligonos = poligonosList.ToDictionary(x => x.Id, x => x);
            else
                poligonos = new Dictionary<int, Poligono>();

            ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Trace("DispatcherLock", string.Format("poligonosList.ToDictionary({0},{1}): {2} segundos", empresaId, lineaId, ts));

            t.Restart();
            var direccionesList = GetDireccionesVigentes(empresaId, lineaId);
            ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Trace("DispatcherLock", string.Format("GetDireccionesVigentes({0},{1}): {2} segundos", empresaId, lineaId, ts));

            t.Restart();
            if (direccionesList != null &&
                direccionesList.Any())
                direcciones = direccionesList.ToDictionary(x => x.Id, x => x);
            else
                direcciones = new Dictionary<int, Direccion>();

            ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Trace("DispatcherLock", string.Format("direccionesList.ToDictionary({0},{1}): {2} segundos", empresaId, lineaId, ts));

            return GetGeocercasFor(empresaId, lineaId, direcciones, poligonos);
        }

        public IEnumerable<Geocerca> GetGeocercasFor(int empresaId, int lineaId, Dictionary<int, Direccion> direcciones, Dictionary<int, Poligono> poligonos)
        {
            var t = new TimeElapsed();
            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_getReferenciasGeoVigentes] @company = :company, @branch = :branch;");
            sqlQ.SetInt32("company", empresaId);
            sqlQ.SetInt32("branch", lineaId);
            sqlQ.SetResultTransformer(Transformers.AliasToBean(typeof(Geocerca)));
            var results = sqlQ.List<Geocerca>();
            STrace.Debug("DispatcherLock", string.Format("sp_getReferenciasGeoVigentes {0} en {1} segundos", results.Count(), t.getTimeElapsed().TotalSeconds));

            t = new TimeElapsed();
            foreach (var geo in results)
            {
                Direccion direccion = null;
                if (geo.DireccionId != null)
                {
                    if (direcciones.ContainsKey(geo.DireccionId.Value))
                        direccion = direcciones[geo.DireccionId.Value];
                    else
                        STrace.Debug("DispatcherLock", string.Format("ERROR DIRECCION NO ENCONTRADA EN CACHE !!! {0} ({1},{2}) ", geo.DireccionId.Value, empresaId, lineaId));
                }

                Poligono poligono = null;
                if (geo.PoligonoId != null)
                {
                    if (poligonos.ContainsKey(geo.PoligonoId.Value))
                        poligono = poligonos[geo.PoligonoId.Value];
                    else
                        STrace.Debug("DispatcherLock", string.Format("ERROR POLIGONO NO ENCONTRADO EN CACHE !!! {0} ({1},{2}) ", geo.PoligonoId.Value, empresaId, lineaId));
                }
                
                if (direccion != null || poligono != null)
                    geo.Calculate(direccion, poligono);
            }
            STrace.Debug("DispatcherLock", string.Format("geo.Calculate {0} en {1} segundos", results.Count(), t.getTimeElapsed().TotalSeconds));
            return results;
        }

        public IEnumerable<ReferenciaGeografica> GetListByEmpresaLineaTipos(int idEmpresa, int idLinea, List<int> idsTipos)
        {   
            var tableTipos = Ids2DataTable(idsTipos);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_ReferenciaGeograficaDAO_GetListByEmpresaLineaTipos] @idEmpresa = :idEmpresa, @idLinea = :idLinea, @tiposIds = :tiposIds;")
                              .AddEntity(typeof(ReferenciaGeografica))
                              .SetInt32("idEmpresa", idEmpresa)
                              .SetInt32("idLinea", idLinea)
                              .SetStructured("tiposIds", tableTipos);
            var results = sqlQ.List<ReferenciaGeografica>();
            return results;
        }

        #endregion

        #region Override Methods

        public override void SaveOrUpdate(ReferenciaGeografica obj)
        {
            foreach (var historiaGeoRef in obj.Historia.Cast<HistoriaGeoRef>().Where(historiaGeoRef => historiaGeoRef.Poligono != null)) historiaGeoRef.Poligono.GenerateBounds();
            base.SaveOrUpdate(obj);
        }

        public void SingleSaveOrUpdate(ReferenciaGeografica obj)
        {   
            SaveOrUpdate(obj);
            UpdateGeocercas(obj);
        }

        public void Guardar(ReferenciaGeografica obj)
        {
            foreach (var historiaGeoRef in obj.Historia.Cast<HistoriaGeoRef>().Where(historiaGeoRef => historiaGeoRef.Poligono != null)) historiaGeoRef.Poligono.GenerateBounds();
            base.SaveOrUpdateWithoutTransaction(obj);
        }

        #endregion

        #region Other Methods

        public void DeleteGeoRef(Int32 geoRef)
        {
            if (!ValidateDelete(geoRef)) return;
            var geo = FindById(geoRef);
            if (geo == null) return;
            geo.Baja = true;
            geo.Vigencia.Fin = DateTime.UtcNow;
            if (geo.Poligono != null) geo.Poligono.Vigencia.Fin = DateTime.UtcNow;
            if (geo.Direccion != null) geo.Direccion.Vigencia.Fin = DateTime.UtcNow;
            SaveOrUpdate(geo);
        }

        public bool ValidateDelete(int geoRef)
        {
            if (Session.Query<Cliente>().Count(cliente => cliente.Baja == false && cliente.ReferenciaGeografica != null && cliente.ReferenciaGeografica.Id == geoRef) > 0) return false;
            if (Session.Query<Equipo>().Count(equipo => equipo.Baja == false && equipo.ReferenciaGeografica != null && equipo.ReferenciaGeografica.Id == geoRef) > 0) return false;
            if (Session.Query<Linea>().Count(linea => linea.Baja == false && linea.ReferenciaGeografica != null && linea.ReferenciaGeografica.Id == geoRef) > 0) return false;
            if (Session.Query<PuntoEntrega>().Count(punto => punto.Baja == false && punto.ReferenciaGeografica != null && punto.ReferenciaGeografica.Id == geoRef) > 0) return false;
            if (Session.Query<Taller>().Count(taller => taller.Baja == false && taller.ReferenciaGeografica != null && taller.ReferenciaGeografica.Id == geoRef) > 0) return false;
            if (Session.Query<Ticket>().Count(ticket => ticket.PuntoEntrega != null && ticket.PuntoEntrega.Id == geoRef) > 0) return false;
            if (Session.Query<Transportista>().Count(transportista => transportista.Baja == false && transportista.ReferenciaGeografica != null && transportista.ReferenciaGeografica.Id == geoRef) > 0) return false;
            return true;
        }

        #endregion

        #region Geocercas Cache

        public void UpdateGeocercas(Dictionary<int, List<int>> empresasLineas)
        {
            // recibo un diccionario (_empresasLineas), pero armo uno nuevo (leDict)
            if (empresasLineas.Count > 0)
            {
                foreach (var k in empresasLineas.Keys)
                {
                    foreach (var v in empresasLineas[k])
                    {
                        ResetLastModQtree(k, v);
                        STrace.Error("ResetQtree", "qtree RESET ---> Empresa: " + k + " - Linea: " + v);
                    }
                }
            }           
        }

        public void UpdateGeocercas(ReferenciaGeografica rg)
        {
            if (rg.Poligono == null) return;

            lock (GeocercasLock)
            {
                var i = DateTime.UtcNow;

                #region Delete Cached Lists & Qtree

                ResetLastModQtree(-1, -1);
                //UpdateCacheList(-1, -1, rg.Id);
                //DeleteCacheList(-1, -1);
                //SetLastListTimeStamp(-1, -1);

                var empresa = rg.Empresa != null ? rg.Empresa.Id : -1;
                var linea = rg.Linea != null ? rg.Linea.Id : -1;

                //if (empresa == -1)
                //{
                //    foreach (var lin in new LineaDAO().FindAll())
                //    {
                //        var empresaId = lin.Empresa.Id;
                //        var lineaId = lin.Id;

                //        UpdateCacheList(lin.Empresa.Id, lin.Id, rg.Id);
                //        UpdateCacheList(lin.Empresa.Id, -1, rg.Id);

                //        DeleteCacheList(lin.Empresa.Id, lin.Id);
                //        DeleteCacheList(lin.Empresa.Id, -1);
                                                
                //        SetLastListTimeStamp(empresaId, lineaId);
                //        SetLastListTimeStamp(empresaId, -1);
                //    }
                //}
                //else 
                if (linea == -1)
                {
                    ResetLastModQtree(empresa, -1);
                    //UpdateCacheList(empresa, -1, rg.Id);
                    //DeleteCacheList(empresa, -1);
                    //SetLastListTimeStamp(empresa, -1);

                    foreach (var lin in new LineaDAO().FindList(new[] { empresa }))
                    {
                        ResetLastModQtree(empresa, lin.Id);
                        //UpdateCacheList(empresa, lin.Id, rg.Id);
                        //DeleteCacheList(empresa, lin.Id);
                        //SetLastListTimeStamp(empresa, lin.Id);
                    }
                }
                else
                {
                    ResetLastModQtree(empresa, linea);
                    ResetLastModQtree(empresa, -1);
                    //UpdateCacheList(empresa, linea, rg.Id);
                    //UpdateCacheList(empresa, -1, rg.Id);
                    //DeleteCacheList(empresa, linea);
                    //DeleteCacheList(empresa, -1);
                    //SetLastListTimeStamp(empresa, linea);
                    //SetLastListTimeStamp(empresa, -1);
                } 
                #endregion

                var f = DateTime.UtcNow;
                if (f.Subtract(i).TotalSeconds > 1)
                    STrace.Trace("DispatcherTest", "UpdateLock: " + f.Subtract(i).TotalSeconds);
            }
        }

        public void ClearGeocerca(int id)
        {
            LogicCache.Delete(typeof(Geocerca), GetGeocercaByIdKey(id));
        }

        public Geocerca FindGeocerca(int id)
        {
            var geo = LogicCache.Retrieve<Geocerca>(typeof(Geocerca), GetGeocercaByIdKey(id));

            return geo ?? GetGeocercaById(id);
        }
        public Geocerca GetGeocercaById(int id)
        {
            var rg = Session.CreateCriteria<ReferenciaGeografica>("rg")
                            .Add(Restrictions.Eq("Id", id))
                            .UniqueResult<ReferenciaGeografica>();

            var geo = new Geocerca(rg);
            LogicCache.Store(typeof(Geocerca), GetGeocercaByIdKey(rg.Id), geo, DateTime.UtcNow.AddMinutes(Config.Dispatcher.DispatcherGeocercasRefreshRate));
            
            return geo;
        }

        public Geocerca FindGeocerca(ReferenciaGeografica rg)
        {
            var geo = new Geocerca(rg);
            LogicCache.Store(typeof(Geocerca), GetGeocercaByIdKey(rg.Id), geo, DateTime.UtcNow.AddMinutes(Config.Dispatcher.DispatcherGeocercasRefreshRate));
            return geo;
        }

        public static DateTime GetLastUpdate(int empresa, int linea)
        {
            var key = GetGeocercaQtreeKey(empresa, linea);
            if (!LogicCache.KeyExists(typeof(Geocerca), key)) return DateTime.MinValue;

            return (DateTime)LogicCache.Retrieve<object>(typeof(Geocerca), key);
        }

        public void ResetLastModQtree(int empresa, int linea)
        {
            var key = GetGeocercaQtreeKey(empresa, linea);
            LogicCache.Store(typeof (Geocerca), key, DateTime.UtcNow);
            STrace.Trace("QtreeReset", string.Format("Reset para ({0}, {1})", empresa, linea));
        }
        private static string GetGeocercaByIdKey(int id)
        {
            return string.Format("geocercaById:{0}", id);
        }
        public static string GetGeocercaQtreeKey(int empresa, int linea)
        {
            return string.Format("geocercasRefreshQtree:{0}:{1}", empresa, linea);
        }

        #endregion

        
    }
}
