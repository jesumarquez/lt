using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Tickets;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class PedidoDAO: GenericDAO<Pedido>
    {
//        public PedidoDAO(ISession session) : base(session) { }

        #region Find Methods

        public int FindNextId()
        {
            const string cacheKey = "AutoGenCode";
            int maxId;
            if (LogicCache.KeyExists(typeof(Pedido), cacheKey))
            {
                maxId = (int)LogicCache.Retrieve<object>(typeof(Pedido), cacheKey);
            }
            else
            {
                maxId = Session.Query<Pedido>().Any()
                    ? Session.Query<Pedido>().Max(p => p.Id)
                    : 1;
            }

            while (FindByCode(-1, maxId.ToString()) != null) maxId++;

            LogicCache.Store(typeof(Pedido), cacheKey, maxId, DateTime.Now.AddMinutes(5));

            return maxId;
        }

        public Pedido FindByCode(int empresa, string codigo)
        {
            return Query.FilterEmpresa(Session, new[]{empresa})
                .Where(b => !b.Baja).FirstOrDefault(b => b.Codigo == codigo);
        }

        #endregion

        #region Get Methods

        public List<Pedido> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<int> puntoentregas, IEnumerable<int> bocasDeCarga, IEnumerable<int> estados, IEnumerable<int> productos, DateTime? desde, DateTime? hasta)
        {
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas)
                         .FilterBocaDeCarga(Session, empresas, lineas, bocasDeCarga)
                         .Where(p => !p.Baja);


            if (!estados.Any(e => e < 0)) q = q.Where(p => estados.ToList().Contains(p.Estado));
            if (desde.HasValue) q = q.Where(b => b.FechaEnObra >= desde.Value);
            if (hasta.HasValue) q = q.Where(b => b.FechaEnObra < hasta.Value);

            IEnumerable<Pedido> list = q.ToList();
            if(!QueryExtensions.IncludesAll(clientes))list = list.FilterCliente(Session, empresas, lineas, clientes);
            if (!QueryExtensions.IncludesAll(puntoentregas)) list = list.FilterPuntoEntrega(Session, empresas, lineas, clientes, puntoentregas);
            if(!QueryExtensions.IncludesAll(bocasDeCarga)) list = list.FilterBocaDeCarga(Session, empresas, lineas, bocasDeCarga);
            if(!QueryExtensions.IncludesAll(productos)) list = list.FilterProducto(Session, empresas, lineas, productos);
            
            return list.ToList();
        }

        public List<Pedido> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga, IEnumerable<int> estados, DateTime? desde, DateTime? hasta)
        {
            var q = Query.FilterEmpresa(Session, empresas)
                         .FilterLinea(Session, empresas, lineas)
                         .FilterBocaDeCarga(Session, empresas, lineas, bocasDeCarga)
                         .Where(p => !p.Baja);

            if (!QueryExtensions.IncludesAll(estados)) q = q.Where(p => estados.ToList().Contains(p.Estado));
            if (desde.HasValue) q = q.Where(b => b.FechaEnObra >= desde.Value);
            if (hasta.HasValue) q = q.Where(b => b.FechaEnObra < hasta.Value);

            return q.ToList();
        }

        #endregion

        #region Override Methods

        public override void Delete(Pedido obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        } 

        #endregion

    }
}
