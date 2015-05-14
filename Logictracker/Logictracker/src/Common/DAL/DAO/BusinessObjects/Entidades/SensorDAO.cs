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
    public class SensorDAO : GenericDAO<Sensor>
    {
        private static readonly object CacheByCodeLock = new object();

        private static string GetByCodeCacheKey(int dispositivo, string code)
        {
            return string.Format("Sensor[{0}:{1}]", dispositivo, code);
        }

//        public SensorDAO(ISession session) : base(session) { }

        public List<Sensor> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> tiposMedicion)
        {
            var q = Query.FilterTipoMedicion(Session, tiposMedicion);

            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(dispositivos))
                q = q.FilterDispositivo(Session, empresas, lineas, dispositivos);
            
            return q.Where(m => !m.Baja)
                    .ToList();
        }

        public List<Sensor> GetSensoresLibres(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> tiposMedicion)
        {
            var subEntidadDao = new SubEntidadDAO();
            var assignedSensores = subEntidadDao.FindAll().Where(c => c.Sensor != null).Select(c => c.Sensor.Id);
            return GetList(empresas, lineas, dispositivos, tiposMedicion)
                          .Where(s => !assignedSensores.Contains(s.Id))
                          .ToList();
        }

        public Sensor FindByCode(int dispositivo, string code)
        {
            lock (CacheByCodeLock)
            {
                var key = GetByCodeCacheKey(dispositivo, code);
                var sensorCache = LogicCache.Retrieve<string>(typeof(string), key);
                if (!string.IsNullOrEmpty(sensorCache))
                {
                    var sensorId = Convert.ToInt32(sensorCache);
                    var sensor = sensorId == 0 ? null : FindById(sensorId);
                    return sensor == null || sensor.Dispositivo.Id != dispositivo ? null : sensor;
                }

                try
                {
                    var sensor = Query
						.Where(s => !s.Baja && s.Dispositivo.Id == dispositivo && s.Codigo == code)
						.SafeFirstOrDefault();
                    if (sensor != null)
                    {
                        LogicCache.Store(typeof(string), key, sensor.Id);
                    }
                    return sensor;
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(SensorDAO).FullName, e, dispositivo);
                    return null;
                }
            }
        }

        public Sensor FindByDispositivo(int idDispositivo)
        {
            return Query
				.Where(s => !s.Baja && s.Dispositivo.Id == idDispositivo)
				.SafeFirstOrDefault();
        }

        public Sensor FindByDispositivoAndTipoMedicion(int idDispositivo, string tipoMedicionCode)
        {
	        return Query.Where(s => !s.Baja && s.Dispositivo.Id == idDispositivo
	                                && s.TipoMedicion.Codigo == tipoMedicionCode)
		        .SafeFirstOrDefault();
        }

        public override void SaveOrUpdate(Sensor obj)
        {
            var oldDev = obj.OldDispositivo;
            base.SaveOrUpdate(obj);
            lock (CacheByCodeLock)
            {
                if (oldDev != null) // si cambia el dispo, borro la cache vieja
                {
                    LogicCache.Delete(typeof (string), GetByCodeCacheKey(oldDev.Id, obj.OldCodigo));
                }
                if (obj.Baja) // si se elimina el sensor, borro la cache
                {
                    LogicCache.Delete(typeof (string), GetByCodeCacheKey(obj.Dispositivo.Id, obj.Codigo));
                }
                if (!obj.Baja) // si no se elimino el sensor, agrego el id a la cache
                {
                    LogicCache.Store(typeof (string), GetByCodeCacheKey(obj.Dispositivo.Id, obj.Codigo), obj.Id);
                }
            }
        }

        public override void Delete(Sensor obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}