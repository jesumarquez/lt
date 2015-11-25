using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los m�todos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class PuntoEntregaDAO : GenericDAO<PuntoEntrega>
    {
//        public PuntoEntregaDAO(ISession session) : base(session) { }


        #region Find Methods

        public List<PuntoEntrega> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes, null)
                .Where(p => !p.Baja)
                .Cacheable()
                .ToList();
        }
        public PuntoEntrega FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, string code)
        {
            var q = Query.FilterCliente(Session, empresas, lineas, clientes, null)
                        .Where(c => !c.Baja)
                        .Where(c => c.Codigo == code);

            q = q.Cacheable();

            return q.SafeFirstOrDefault();
        }

        public List<PuntoEntrega> FindByEmpresaAndCodes(int empresaId, IEnumerable<string> codes)
        {
            return Query.Where(c => c.Cliente.Empresa.Id == empresaId && !c.Baja && codes.Contains(c.Codigo))
                        .Cacheable()
                        .ToList();
        }

        #endregion

        #region Get Methods

        public PuntoEntrega GetByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, string code)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes)
                .Where(p => !p.Baja)
                .Where(p => p.Codigo == code)
                .Cacheable()
				.SafeFirstOrDefault();
        }

        public List<PuntoEntrega> FindByCodes(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<string> codes)
        {
            return Query.FilterCliente(Session, empresas, lineas, clientes)
                .Where(p => !p.Baja)
                .Where(p => codes.Contains(p.Codigo))
                .Cacheable()
                .ToList();
        }

        public List<PuntoEntrega> GetByCliente(int idCliente, int page, int pageSize, ref int totalRows, bool reCount, string SearchString)
        {
            if (string.IsNullOrEmpty(SearchString))
            {
                if (reCount)
                {
                    int count = Query.Where(p => p.Cliente.Id == idCliente
                        && !p.Baja).Count();
                    if (!totalRows.Equals(count))
                    {
                        totalRows = count;
                    }
                }
                return Query.Where(p => p.Cliente.Id == idCliente
                        && !p.Baja).OrderBy(x => x.Nombre)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Cacheable()
                            .ToList();
            }
            else
            {
                if (reCount)
                {
                    int count = Query.Where(p => p.Cliente.Id == idCliente
                        && !p.Baja && (p.Nombre.ToUpper().Contains(SearchString.ToUpper()) || p.Codigo.ToUpper().Contains(SearchString.ToUpper()))).Count();
                    if (!totalRows.Equals(count))
                    {
                        totalRows = count;
                    }
                }
                return Query.Where(p => p.Cliente.Id == idCliente
                        && !p.Baja && (p.Nombre.ToUpper().Contains(SearchString.ToUpper()) || p.Codigo.ToUpper().Contains(SearchString.ToUpper()))).OrderBy(x => x.Nombre)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Cacheable()
                            .ToList();
            }
        }

        public List<PuntoEntrega> GetByCliente(int idCliente)
        {
            return Query.Where(p => p.Cliente.Id == idCliente
                    && !p.Baja)
                        .Cacheable()
                        .ToList();
        }

        public List<PuntoEntrega> GetByCliente(int idCliente, string SearchString)
        {
           if (string.IsNullOrEmpty(SearchString))
           { 

            return Query.Where(p => p.Cliente.Id == idCliente
                    && !p.Baja)
                        .Cacheable()
                        .ToList();
           }
            else
           {
               return Query.Where(p => p.Cliente.Id == idCliente
                    && !p.Baja && (p.Nombre.ToUpper().Contains(SearchString.ToUpper()) || p.Codigo.ToUpper().Contains(SearchString.ToUpper())))
                        .Cacheable()
                        .ToList();
           }
        }
        

        public List<PuntoEntrega> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes)
        {
            var q = Query.FilterCliente(Session, empresas, lineas, clientes)
                         .Where(p => !p.Baja);
            
            return q.Cacheable().ToList();
        }

        public List<PuntoEntrega> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, string prefixText)
        {
            var q = Query.FilterCliente(Session, empresas, lineas, clientes)
                         .Where(p => p.Descripcion.ToLower().Contains(prefixText))
                         .Where(p => !p.Baja);

            return q.Cacheable().ToList();
        }

        #endregion

        #region Override Methods

        public override void Delete(PuntoEntrega p)
        {
            if (p == null) return;
            p.Baja = true;
            SaveOrUpdate(p);
            if (p.ReferenciaGeografica != null)
            {
                var dao = new DAOFactory();
                dao.ReferenciaGeograficaDAO.DeleteGeoRef(p.ReferenciaGeografica.Id);
            }
        }

        #endregion

        #region Other Methods

        public void SaveOrUpdateWithoutTransaction(PuntoEntrega obj)
        {
            base.SaveOrUpdateWithoutTransaction(obj);
        }

        public void DeleteByCliente(int id)
        {
            foreach (PuntoEntrega punto in FindList(new[]{-1}, new[]{-1}, new[]{id}))
            {
                punto.Baja = true;
                Delete(punto);
            }
        }
        #endregion
    }
}
