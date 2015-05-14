using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Entidades;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public partial class MedicionDAO
    {
        public ValoresAgregados GetAggregates(int dispositivo, string sensorCode, DateTime desde, DateTime hasta)
        {
            var sensorDao = new SensorDAO();
            var sensor = sensorDao.FindByCode(dispositivo, sensorCode);
            if (sensor == null) return null;

            ValoresAgregados valoresAgregados = null;

            return Session.QueryOver<Medicion>()
                .Where(m => m.Dispositivo.Id == dispositivo && m.Sensor.Id == sensor.Id)
                .And(m => m.FechaMedicion >= desde && m.FechaMedicion <= hasta)
                .Select(Projections.Max<Medicion>(m => m.ValorNum1).WithAlias(() => valoresAgregados.Max),
                            Projections.Min<Medicion>(m => m.ValorNum1).WithAlias(() => valoresAgregados.Min),
                            Projections.Avg<Medicion>(m => m.ValorNum1).WithAlias(() => valoresAgregados.Avg)
                )
                .TransformUsing(Transformers.AliasToBean<ValoresAgregados>())
                .List<ValoresAgregados>()
                .FirstOrDefault();
        }
        public IEnumerable<ValoresDiarios> GetAggregatesDiarios(int dispositivo, string sensorCode, DateTime desde, DateTime hasta)
        {
            var sensorDao = new SensorDAO();
            var sensor = sensorDao.FindByCode(dispositivo, sensorCode);
            if (sensor == null) return new List<ValoresDiarios>(0);

            ValoresDiarios valoresDiarios = null;
            var byDate = Projections.SqlFunction("date", NHibernateUtil.Date,
                                                        Projections.Group<Medicion>(a => a.FechaMedicion));
            return Session.QueryOver<Medicion>()
                .Where(m => m.Dispositivo.Id == dispositivo && m.Sensor.Id == sensor.Id)
                .And(m => m.FechaMedicion >= desde && m.FechaMedicion <= hasta)
                .SelectList(x => x
                    .Select(Projections.Max<Medicion>(m => m.ValorNum1).WithAlias(() => valoresDiarios.Max))
                    .Select(Projections.Min<Medicion>(m => m.ValorNum1).WithAlias(() => valoresDiarios.Min))
                    .Select(Projections.Avg<Medicion>(m => m.ValorNum1).WithAlias(() => valoresDiarios.Avg))
                    .Select(Projections.GroupProperty(byDate).WithAlias(() => valoresDiarios.Date))
                    )
                .OrderBy(byDate).Asc
                .TransformUsing(Transformers.AliasToBean<ValoresDiarios>())
                .List<ValoresDiarios>();
        }
        //public IEnumerable<ValoresDiarios> GetMaximaDiaria(int dispositivo, string sensorCode, DateTime desde, DateTime hasta)
        //{
        //    var sensorDao = new SensorDAO();
        //    var sensor = sensorDao.FindByCode(dispositivo, sensorCode);
        //    if (sensor == null) return new List<ValoresDiarios>(0);

        //    ValoresDiarios valoresDiarios = null;
        //    var byDate = Projections.SqlFunction("date", NHibernateUtil.Date,
        //                                                Projections.Group<Medicion>(a => a.FechaMedicion));
        //    return Session.QueryOver<Medicion>()
        //        .Where(m => m.Dispositivo.Id == dispositivo && m.Sensor.Id == sensor.Id)
        //        .And(m => m.FechaMedicion >= desde && m.FechaMedicion <= hasta)
        //        .SelectList(x => x
        //            .Select(Projections.Max<Medicion>(m=>m.ValorNum1).WithAlias(() => valoresDiarios.Valor))
        //            .Select(Projections.GroupProperty(byDate).WithAlias(()=>valoresDiarios.Date))
        //            )
        //        .OrderBy(byDate).Asc
        //        .TransformUsing(Transformers.AliasToBean<ValoresDiarios>())
        //        .List<ValoresDiarios>();
        //}
    }
    public class ValoresDiarios
    {
        public DateTime Date { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Avg { get; set; }
    }
    public class ValoresAgregados
    {
        public double Max { get; set; }
        public double Min { get; set; }
        public double Avg { get; set; }
    }
}
