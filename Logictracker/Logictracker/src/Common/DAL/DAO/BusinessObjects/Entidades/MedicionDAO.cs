using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Entidades;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public partial class MedicionDAO : GenericDAO<Medicion>
    {
//        public MedicionDAO(ISession session) : base(session) { }

        public Medicion FindLast(int sensor, DateTime fechaHasta)
        {
            var sen = new SensorDAO().FindById(sensor);
            return Query
                .Where(m => m.Sensor.Id == sensor && m.Dispositivo.Id == sen.Dispositivo.Id)
                .Where(m => m.FechaMedicion < fechaHasta)
                .OrderByDescending(m => m.FechaMedicion)
                .FirstOrDefault();
        }

        public List<Medicion> FindList(int sensor, DateTime fechaDesde, DateTime fechaHasta)
        {
            var sen = new SensorDAO().FindById(sensor);
            return Query
                .Where(m => m.Sensor.Id == sensor && m.Dispositivo.Id == sen.Dispositivo.Id)
                .Where(m => m.FechaMedicion < fechaHasta && m.FechaMedicion > fechaDesde)
                .OrderBy(m => m.FechaMedicion)
                .ToList();
        }

        public List<Medicion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> sensores, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> subentidades, IEnumerable<int> tiposMedicion, DateTime fechaDesde, DateTime fechaHasta)
        {
            var q = Query.FilterTipoMedicion(Session, tiposMedicion);

            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(dispositivos))
                q = q.FilterDispositivo(Session, empresas, lineas, dispositivos);
            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(dispositivos) || !QueryExtensions.IncludesAll(sensores))
                q = q.FilterSensor(Session, empresas, lineas, dispositivos, sensores);
            if (!QueryExtensions.IncludesAll(empresas) || !QueryExtensions.IncludesAll(lineas) || !QueryExtensions.IncludesAll(tiposEntidad)
             || !QueryExtensions.IncludesAll(entidades) || !QueryExtensions.IncludesAll(subentidades))
                q = q.FilterSubEntidad(Session, empresas, lineas, tiposEntidad, entidades, subentidades);
            
            return q.Where(m => m.FechaMedicion < fechaHasta && m.FechaMedicion > fechaDesde)
                    .OrderBy(m => m.FechaMedicion)
                    .ToList();
        }
        
        public Medicion GetUltimaMedicionHasta(int idDispositivo, int idSensor, DateTime hasta)
        {
            var q = Query;

            if (idDispositivo != -1)
                q = q.FilterDispositivo(Session, new[] { -1 }, new[] { -1 }, new[] { idDispositivo });
            if (idDispositivo != -1 || idSensor != -1)
                q = q.FilterSensor(Session, new[] { -1 }, new[] { -1 }, new[] { idDispositivo }, new[] { idSensor });

            return q.Where(m => m.FechaMedicion < hasta)
                    .OrderByDescending(m => m.FechaMedicion)
                    .FirstOrDefault();
        }
    }
}