using System;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class DataMartsLogDAO : GenericDAO<DataMartsLog>
    {
        public IEnumerable<DataMartsLog> GetList(DateTime desde, DateTime hasta)
        {
            return Query.Where(f => f.FechaInicio >= desde
                                 && f.FechaFin <= hasta)
                        .ToList();
        }

        public void SaveNewLog(DateTime inicio, DateTime fin, double duracion, short modulo, string mensaje)
        {
            var log = new DataMartsLog
            {
                FechaInicio = inicio,
                FechaFin = fin,
                Duracion = duracion,
                Modulo = modulo,
                Mensaje = mensaje
            };

            SaveOrUpdate(log);
        }
    }
}