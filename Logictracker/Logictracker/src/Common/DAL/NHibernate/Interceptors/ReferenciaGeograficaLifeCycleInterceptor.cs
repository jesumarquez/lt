using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Type;

namespace Logictracker.DAL.NHibernate.Interceptors
{
    public class ReferenciaGeograficaLifeCycleInterceptor : BaseInterceptor
    {  
        private Dictionary<int, List<int>> _empresasLineas = new Dictionary<int, List<int>>();

        private void AddReferenciasGeograficas(object entity)
        {
            var rg = entity as ReferenciaGeografica;
            

            if (rg == null)
                STrace.Error("Logictracker Interceptor", "entity is null");
            else if (rg.Empresa == null)
                STrace.Error("Logictracker Interceptor", "rg.Empresa is null");
            else
            {
                var empresaId = rg.Empresa.Id;

                if (!_empresasLineas.ContainsKey(empresaId))
                    _empresasLineas.Add(rg.Empresa.Id, new List<int> { -1 });

                if (rg.Linea != null)
                {
                    if (!_empresasLineas[rg.Empresa.Id].Contains(rg.Linea.Id))
                        _empresasLineas[rg.Empresa.Id].Add(rg.Linea.Id);
                }
                else
                {
                    var todaslaslineas = new DAOFactory().LineaDAO.GetList(new[] { rg.Empresa.Id });
                    foreach (var linea in todaslaslineas)
                    {
                        if (!_empresasLineas.ContainsKey(linea.Id))
                            _empresasLineas[rg.Empresa.Id].Add(linea.Id);
                    }
                }
            }
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            if (entity != null && entity.GetType() == typeof(ReferenciaGeografica))
                AddReferenciasGeograficas(entity);

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity != null && entity.GetType() == typeof(ReferenciaGeografica))
                AddReferenciasGeograficas(entity);
            
            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity != null && entity.GetType() == typeof(ReferenciaGeografica))
                AddReferenciasGeograficas(entity);
        }                

        public override void AfterTransactionCompletion(ITransaction tx)
        {
            if (_empresasLineas.Count > 0)
            {
                new DAOFactory().ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                _empresasLineas.Clear();
            }
        }
    }
}