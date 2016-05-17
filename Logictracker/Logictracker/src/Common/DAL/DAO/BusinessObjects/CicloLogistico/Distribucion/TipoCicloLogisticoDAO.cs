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
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion
{
    public class TipoCicloLogisticoDAO : GenericDAO<TipoCicloLogistico>
    {
        public List<TipoCicloLogistico> GetByEmpresa(int empresa)
        {
            return Session.Query<TipoCicloLogistico>().Where(e => e.Empresa.Id == empresa)
                                                   .OrderBy(e => e.Descripcion)
                                                   .ToList();
        }

        public TipoCicloLogistico FindByCodigo(int empresa, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .Where(q => !q.Baja && q.Codigo == codigo)
                        .SafeFirstOrDefault();
        }
        public override void Delete(TipoCicloLogistico obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}