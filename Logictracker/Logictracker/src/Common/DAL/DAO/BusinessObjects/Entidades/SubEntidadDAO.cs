using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Utils;
using NHibernate;
using System.Collections.Generic;
using System;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public class SubEntidadDAO : GenericDAO<SubEntidad>
    {
        private static readonly object CacheBySensorLock = new object();

        private static string GetBySensorCacheKey(int sensor)
        {
            return string.Format("SubEntidad[{0}]", sensor);
        }

//        public SubEntidadDAO(ISession session) : base(session) { }

        public List<SubEntidad> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> dispositivos, IEnumerable<int> sensores)
        {
            var q = Query.FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);
            if (!QueryExtensions.IncludesAll(tiposEntidad) || !QueryExtensions.IncludesAll(entidades))
                q = q.FilterEntidad(Session, empresas, lineas, tiposEntidad, entidades);
            if (!QueryExtensions.IncludesAll(dispositivos) || !QueryExtensions.IncludesAll(sensores))
                q = q.FilterSensor(Session, empresas, lineas, dispositivos, sensores);

            return q.Where(m => !m.Baja)
                    .ToList();
        }

        public SubEntidad FindBySensor(int sensor)
        {
            lock (CacheBySensorLock)
            {
                var key = GetBySensorCacheKey(sensor);
                var subEntidadCache = LogicCache.Retrieve<string>(typeof(string), key);
                if (!string.IsNullOrEmpty(subEntidadCache))
                {
                    var subEntidadId = Convert.ToInt32(subEntidadCache);
                    var subEntidad = subEntidadId == 0 ? null : FindById(subEntidadId);
                    return subEntidad == null || subEntidad.Sensor.Id != sensor ? null : subEntidad;
                }

                try
                {
                    var subEntidad = Query.Where(s => !s.Baja && s.Sensor.Id == sensor).SafeFirstOrDefault();
                    if (subEntidad != null)
                    {
                        LogicCache.Store(typeof(string), key, subEntidad.Id);
                    }
                    return subEntidad;
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(SensorDAO).FullName, e);
                    return null;
                }
            }
        }

        public bool IsCodeUnique(int empresa, int linea, int idSubEntidad, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .Where(g => g.Id != idSubEntidad)
                        .Where(g => g.Codigo == code)
                        .FirstOrDefault() == null;
        }

        public List<SubEntidad> GetByDescripcion(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, string descripcion)
        {
            return GetList(empresas, lineas, tiposEntidad, entidades, new[] { -1 }, new[] { -1 })
                          .Where(e => e.Descripcion.ToLower().Contains(descripcion.ToLower()))
                          .ToList();
        }

        public override void SaveOrUpdate(SubEntidad obj)
        {
            var oldSensor = obj.OldSensor;
            base.SaveOrUpdate(obj);
            lock (CacheBySensorLock)
            {
                if (oldSensor != null) // si cambia el sensor, borro la cache vieja
                {
                    LogicCache.Delete(typeof(string), GetBySensorCacheKey(oldSensor.Id));
                }
                if (obj.Baja) // si se elimina la subEntidad, borro la cache
                {
                    LogicCache.Delete(typeof(string), GetBySensorCacheKey(obj.Sensor.Id));
                }
                if (!obj.Baja) // si no se elimino la subEntidad, agrego el id a la cache
                {
                    LogicCache.Store(typeof(string), GetBySensorCacheKey(obj.Sensor.Id), obj.Id);
                }
            }
        }
        public override void Delete(SubEntidad obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}