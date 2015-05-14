using System;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class LogicLinkFileDAO : GenericDAO<LogicLinkFile>
    {
//        public LogicLinkFileDAO(ISession session) : base(session) { }

        public IEnumerable<LogicLinkFile> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, DateTime desde, DateTime hasta)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(f => f.DateAdded >= desde
                                 && f.DateAdded <= hasta)
                        .ToList();
        }

        public LogicLinkFile GetNextPendiente(int idEmpresa)
        {
            return Query.Where(f => f.Status == LogicLinkFile.Estados.Pendiente && f.Empresa.Id == idEmpresa)
                        .OrderBy(f => f.DateAdded)
                        .FirstOrDefault();
        }
    }
}