using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class EstadoLogisticoDAO : GenericDAO<EstadoLogistico>
    {
        public List<EstadoLogistico> GetByEmpresa(int empresa)
        {
            return Session.Query<EstadoLogistico>().Where(e => e.Empresa.Id == empresa)
                                                   .OrderBy(e => e.Descripcion)
                                                   .ToList();
        }

        public EstadoLogistico FindByDescripcion(int empresa, string descripcion)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .Where(q => !q.Baja && q.Descripcion == descripcion)
                        .SafeFirstOrDefault();
        }
        public override void Delete(EstadoLogistico obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}