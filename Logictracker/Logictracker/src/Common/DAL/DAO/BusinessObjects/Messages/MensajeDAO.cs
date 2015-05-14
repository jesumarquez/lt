#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ValueObject.Messages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Linq;
using NHibernate.SqlCommand;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    /// <summary>
    /// Database access class for Mensaje.
    /// </summary>
    public class MensajeDAO : GenericDAO<Mensaje>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public MensajeDAO(ISession session) : base(session) { }

        #endregion

        #region Private Methods
        private DetachedCriteria GetMensajeDetachedCriteria()
        {
            return GetMensajeDetachedCriteria(false, new int[] { }, new int[] { });
        }

        private DetachedCriteria GetMensajeDetachedCriteria(int[] empresas, int[] lineas)
        {
            return GetMensajeDetachedCriteria(false, empresas, lineas);
        }

        private DetachedCriteria GetMensajeDetachedCriteria(bool esBaja, int[] empresas, int[] lineas)
        {
            var dc = DetachedCriteria.For<Mensaje>("dm")
                .Add(Restrictions.Eq("EsBaja", esBaja))
                .Add(Restrictions.EqProperty("dm.Id", "m.Id"))
                .SetProjection(Projections.Property("Id"));

            if (empresas.Count() == 0)
                dc.Add(Restrictions.IsNull("Empresa"));
            else if (empresas.Count() == 1 && empresas.First() == -1)
                dc.Add(Restrictions.Not(Restrictions.IsNull("Empresa")));
            else                
                dc.CreateAlias("Empresa", "e", JoinType.InnerJoin).Add(Restrictions.In("e.Id", empresas));

            if (lineas.Count() == 0)
                dc.Add(Restrictions.IsNull("Linea"));
            else if (lineas.Count() == 1 && lineas.First() == -1)
                dc.Add(Restrictions.Not(Restrictions.IsNull("Linea")));
            else
                dc.CreateAlias("Linea", "l", JoinType.InnerJoin).Add(Restrictions.In("l.Id", lineas));

            return dc;
        }

        private ICriteria GetMensajeCriteria(int top, DetachedCriteria dc, Order order)
        {
            var crit = Session.CreateCriteria<Mensaje>("m")
                .Add(Subqueries.Exists(dc));

            if (top > 0)
                crit.SetMaxResults(top);

            if (order != null)
                crit.AddOrder(order);

            return crit;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Find all messages for the givenn location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<Mensaje> FindByEmpresaYLineaAndUser(Empresa empresa, Linea linea, Usuario user)
        {
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;
            var lin = linea != null ? linea.Id : -1;

            return FindByEmpresaYLinea(emp, lin, user);
        }

        /// <summary>
        /// Find all messages for the givenn location, base and message type.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<Mensaje> FindByTipo(TipoMensaje tipo, Empresa empresa, Linea linea, Usuario user)
        {
            var messages = FindByEmpresaYLineaAndUser(empresa, linea,user).ToList();

            messages = tipo == null ? messages : (from Mensaje m in messages where m.TipoMensaje != null && m.TipoMensaje.Codigo.Equals(tipo.Codigo) select m).ToList();

            return (from Mensaje m in messages where m.Acceso <= user.Tipo select m).ToList();
        }

        /// <summary>
        /// Find all messages asociated to the logistic cycle of the givenn location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        public IEnumerable<Mensaje> FindCicloLogistico(Empresa empresa, Linea linea)
        {
            var messages = FindByEmpresaYLineaAndUser(empresa, linea, null).ToList();

            return (from Mensaje mensaje in messages where !mensaje.EsSoloDeRespuesta && mensaje.TipoMensaje != null && mensaje.TipoMensaje.DeEstadoLogistico select mensaje).ToList();
        }

		/// <summary>
		/// Finds the message with the givenn code for the specified location and base.
		/// </summary>
		/// <param name="codigo"></param>
		/// <param name="empresa"></param>
		/// <param name="linea"></param>
		/// <returns></returns>
		public MensajeVO GetByCodigo(String codigo, Empresa empresa, Linea linea)
		{
			if (String.IsNullOrEmpty(codigo)) return null;

			var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa != null ? linea.Empresa.Id : -1 : -1;
			var lin = linea != null ? linea.Id : -1;

			var key = String.Format("message:{0}:{1}:{2}", emp, lin, codigo);

			if (!LogicCache.KeyExists(typeof(MensajeVO), key))
			{
				var result = FindByEmpresaYLineaAndUser(empresa, linea, null).FirstOrDefault(m => Convert.ToInt32(m.Codigo) == Convert.ToInt32(codigo));

				var mensajeVo = result != null ? new MensajeVO(result) : null;

				LogicCache.Store(typeof(MensajeVO), key, mensajeVo);

				return mensajeVo;
			}

			return LogicCache.Retrieve<MensajeVO>(typeof(MensajeVO), key);
		}

        public IEnumerable<Mensaje> FindByCodes(IEnumerable<string> codes)
        {
            var mensajes = Query.Where(m => codes.Contains(m.Codigo) && !m.EsBaja).ToList();

            return mensajes;
        }

        /// <summary>
        /// Determines if a message with the givenn code for the specified location and base already exists.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        public bool Exists(String codigo, Empresa empresa, Linea linea)
        {
            var message = GetByCodigo(codigo, empresa, linea);

            if (message == null) return false;

            if (empresa == null && linea == null) return message.Empresa == null && message.Linea == null;

            if (linea == null) return message.Empresa.HasValue && message.Empresa.Value.Equals(empresa.Id);

            return message.Linea.HasValue && message.Linea.Value.Equals(linea.Id);
        }

        public override void Delete(Mensaje obj)
        {
            if (obj == null) return;

            obj.EsBaja = true;

            SaveOrUpdate(obj);
        }

        public override void SaveOrUpdate(Mensaje obj)
        {
            obj.Revision = GetNextRevisionValue();

            base.SaveOrUpdate(obj);

            UpdateCache(obj);

            UpdateChildsValues(obj);
        }

        /// <summary>
        /// Updates the cache to store the message being created.
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateCache(Mensaje obj)
        {
            const string keyformat = "message:{0}:{1}:{2}";

            var key = String.Format(keyformat, -1, -1, obj.Codigo);
            DeleteCache(key);
            
            var empresa = obj.Empresa != null ? obj.Empresa.Id : -1;
            var linea = obj.Linea != null ? obj.Linea.Id : -1;

            if (empresa == -1)
            {
                foreach (var lin in new LineaDAO().FindAll())
                {
                    key = String.Format(keyformat, lin.Empresa.Id, lin.Id, obj.Codigo);
                    DeleteCache(key);

                    key = String.Format(keyformat, lin.Empresa.Id, -1, obj.Codigo);
                    DeleteCache(key);
                }
            }
            else if (linea == -1)
            {
                key = String.Format(keyformat, empresa, -1, obj.Codigo);
                DeleteCache(key);

                foreach (var lin in new LineaDAO().FindList(new[] { empresa }))
                {
                    key = String.Format(keyformat, empresa, lin.Id, obj.Codigo);
                    DeleteCache(key);
                }
            }
            else
            {
                key = String.Format(keyformat, empresa, linea, obj.Codigo);
                DeleteCache(key);
            } 

            LogicCache.Store(typeof(MensajeVO), key, new MensajeVO(obj));
        }

        private void DeleteCache(string key)
        {
            if (LogicCache.KeyExists(typeof(MensajeVO), key)) LogicCache.Delete(typeof(MensajeVO), key);
        }

        /// <summary>
        /// Determines if the specified message has a parent message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public bool HasParent(int message, string codigo)
        {
            var dc = GetMensajeDetachedCriteria()
                .Add(Restrictions.Eq("dm.Codigo", codigo))
                .Add(Restrictions.Not(Restrictions.Eq("dm.Id", message)));

            var crit = GetMensajeCriteria(0, dc, null);
            crit.SetProjection(Projections.Count("Id"));
            var result = crit.UniqueResult<int>();
            return result > 0;
        }

    	/// <summary>
    	/// 
    	/// </summary>
    	/// <param name="device"></param>
    	/// <param name="revision"></param>
    	/// <returns></returns>
    	public List<Mensaje> GetCannedMessagesTable(int device, int revision)
		{
			var vehicle = new CocheDAO().FindMobileByDevice(device);
			if (vehicle == null) return null;
			

			var emp = vehicle.Empresa != null ? vehicle.Empresa.Id : -1;
			var lin = vehicle.Linea != null ? vehicle.Linea.Id : -1;

    		try
    		{
				var res = Query
					.FilterEmpresa(Session, new[] { emp })
					.FilterLinea(Session, new[] { emp }, new[] { lin })
					.Where(m => (!m.EsBaja) && m.Revision > revision && !m.EsSoloDeRespuesta && (m.TipoMensaje.DeUsuario || m.TipoMensaje.DeEstadoLogistico))
					.Cacheable()
					.ToList()
					.Select(m => new { Value = (m.Empresa != null ? 1 : 0) + (m.Linea != null ? 2 : 0), Mensaje = m })
					.GroupBy(m => m.Mensaje.Codigo, n => n)
					.Select(m => m.FirstOrDefault(j => j.Value == m.Max(h => h.Value)).Mensaje)
					.ToList();

				foreach (var r in res) Session.Refresh(r);
					
				return res;

			}
			catch (ArgumentNullException)
			{
				return null;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        public List<Mensaje> GetResponsesMessagesTable(int device, int revision)
        {
            var vehicle = new CocheDAO().FindMobileByDevice(device);
            if (vehicle == null) return null;


            var emp = vehicle.Empresa != null ? vehicle.Empresa.Id : -1;
            var lin = vehicle.Linea != null ? vehicle.Linea.Id : -1;

            try
            {
                var res = Query
                    .FilterEmpresa(Session, new[] { emp })
                    .FilterLinea(Session, new[] { emp }, new[] { lin })
                    .Where(m => (!m.EsBaja) && m.Revision > revision && (m.TipoMensaje.DeConfirmacion || m.TipoMensaje.DeRechazo))
                    .Cacheable()
                    .ToList()
                    .Select(m => new { Value = (m.Empresa != null ? 1 : 0) + (m.Linea != null ? 2 : 0), Mensaje = m })
                    .GroupBy(m => m.Mensaje.Codigo, n => n)
                    .Select(m => m.FirstOrDefault(j => j.Value == m.Max(h => h.Value)).Mensaje)
                    .ToList();

                foreach (var r in res) Session.Refresh(r);

                return res;

            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

		/// <summary>
		/// Finds the message with the givenn code for the specified location and base.
		/// </summary>
		/// <param name="codigo"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public MensajeVO GetByCodigo(String codigo, int device)
		{
			if (device == 0) return null;
			if (String.IsNullOrEmpty(codigo)) return null;

			var vehicle = new CocheDAO().FindMobileByDevice(device);
			var emp = vehicle.Empresa != null ? vehicle.Empresa.Id : vehicle.Linea != null ? vehicle.Linea.Empresa != null ? vehicle.Linea.Empresa.Id : -1 : -1;
			var lin = vehicle.Linea != null ? vehicle.Linea.Id : -1;

			var key = String.Format("message:{0}:{1}:{2}", emp, lin, codigo);

			if (!LogicCache.KeyExists(typeof(MensajeVO), key))
			{
				var result = FindByEmpresaYLineaAndUser(vehicle.Empresa, vehicle.Linea, null).FirstOrDefault(m => Convert.ToInt32(m.Codigo) == Convert.ToInt32(codigo));

				var mensajeVo = result != null ? new MensajeVO(result) : null;

				LogicCache.Store(typeof(MensajeVO), key, mensajeVo);

				return mensajeVo;
			}

			return LogicCache.Retrieve<MensajeVO>(typeof(MensajeVO), key);
		}

        public IEnumerable<Mensaje> GetMensajesDeConfirmacion(int[] empresas, int[] lineas)
        {
            var dc = GetMensajeDetachedCriteria(false, empresas, lineas);
            //dc.Add(Restrictions.Eq("m.TipoMensaje.DeConfirmacion", true));

            var crit = GetMensajeCriteria(0, dc, null);
            var mensajes = crit.List<Mensaje>().Where(m => m.TipoMensaje.DeConfirmacion).ToList();
            return mensajes;
        }

        public IEnumerable<Mensaje> GetMensajesDeRechazo(int[] empresas, int[] lineas)
        {
            var dc = GetMensajeDetachedCriteria(false, empresas, lineas);
            //dc.Add(Restrictions.Eq("TipoMensaje.DeRechazo", true));

            var crit = GetMensajeCriteria(0, dc, null);
            var mensajes = crit.List<Mensaje>().Where(m => m.TipoMensaje.DeRechazo).ToList();
            return mensajes;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds all messages for the givenn location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEnumerable<Mensaje> FindByEmpresaYLinea(int empresa, int linea, Usuario user)
        {
            var parents = FindAllParentMessages();

            var mensajes = new List<Mensaje>();

            if (empresa > 0)
            {
                var locationParents = GetMensajeCriteria(0, GetMensajeDetachedCriteria(new[] {empresa}, new int[] {}), Order.Asc("Codigo")).List<Mensaje>();

				if (linea > 0)
                {
                    var locationChilds = GetMensajeCriteria(0, GetMensajeDetachedCriteria(new[] { empresa }, new [] { linea }), Order.Asc("Codigo")).List<Mensaje>();

                    mensajes.AddRange(MergeParentsAndChilds(parents, MergeParentsAndChilds(locationParents, locationChilds)));
                }
                else
                {
                    var locationChilds = GetMensajeCriteria(0, GetMensajeDetachedCriteria(new[] { empresa }, new [] { -1 }), Order.Asc("Codigo")).List<Mensaje>();

                    mensajes.AddRange(MergeParentsAndChilds(parents, MergeChildsIntoParents(locationParents, locationChilds)));
                }
            }

            var results = mensajes.Count.Equals(0) ? parents.OfType<Mensaje>().ToList() : MergeParentsAndChilds(parents, mensajes);

            return user == null ? results : FilterByUser(results, user);
        }

        /// <summary>
        /// Filter givenn messages by user.
        /// </summary>
        /// <param name="mensajes"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static List<Mensaje> FilterByUser(IEnumerable mensajes, Usuario user)
        {
            return (from Mensaje m in mensajes
                    where !m.EsBaja && ((m.Empresa == null && m.Linea == null)
                        || (m.Empresa != null && m.Linea == null && (user.Empresas.IsEmpty || user.Empresas.Contains(m.Empresa)))
                        || (m.Linea != null && (user.Lineas.IsEmpty || user.Lineas.Contains(m.Linea))))
                    select m).ToList();
        }

        /// <summary>
        /// Merges all unredefined childs with their parents.
        /// </summary>
        /// <param name="parents"></param>
        /// <param name="childs"></param>
        /// <returns></returns>
        private static List<Mensaje> MergeChildsIntoParents(ICollection<Mensaje> parents, IList<Mensaje> childs)
        {
            if (childs.Count.Equals(0)) return parents.ToList();

            return parents.Count.Equals(0) ? childs.ToList() : MergeParents(parents, childs);
        }

        /// <summary>
        /// Merges the non redefined child messages with their parents.
        /// </summary>
        /// <param name="parents"></param>
        /// <param name="childs"></param>
        /// <returns></returns>
        private static List<Mensaje> MergeParents(ICollection<Mensaje> parents, IEnumerable<Mensaje> childs)
        {
            var merged = new List<Mensaje>(parents.Count);

            merged.AddRange(parents);

            foreach (var child in from Mensaje child in childs let isChild = merged.Any(message => message.Codigo.Equals(child.Codigo)) where !isChild select child) merged.Add(child);

            return merged;
        }

        /// <summary>
        /// Merge all parent message with its redifined childs.
        /// </summary>
        /// <param name="parents"></param>
        /// <param name="childs"></param>
        /// <returns></returns>
        private static List<Mensaje> MergeParentsAndChilds(IList<Mensaje> parents, IList<Mensaje> childs)
        {
            if (childs.Count.Equals(0)) return parents.ToList();

            if (parents.Count.Equals(0)) return childs.ToList();

            var merged = new List<Mensaje>(parents.Count);

            MergeChilds(childs, parents, merged);

            AddNonGenericChilds(childs, merged);

            return merged;
        }

        /// <summary>
        /// Adds non generic childs to the merged results set.
        /// </summary>
        /// <param name="childs"></param>
        /// <param name="merged"></param>
        private static void AddNonGenericChilds(IEnumerable childs, ICollection<Mensaje> merged)
        {
            foreach (var child in from Mensaje child in childs where !merged.Any(message => message.Codigo.Equals(child.Codigo)) select child) merged.Add(child);
        }

        /// <summary>
        /// Merge generich childs with their parents.
        /// </summary>
        /// <param name="childs"></param>
        /// <param name="parents"></param>
        /// <param name="merged"></param>
        private static void MergeChilds(IList<Mensaje> childs, IEnumerable<Mensaje> parents, ICollection<Mensaje> merged)
        {
            foreach (Mensaje parent in parents)
            {
                var hasChild = false;

                var father = parent;

                foreach (var child in childs.Where(child => child.Codigo.Equals(father.Codigo)))
                {
                    hasChild = true;

                    merged.Add(child);

                    break;
                }

                if (!hasChild) merged.Add(parent);
            }
        }


        /// <summary>
        /// Returns all parent messages. A parent message is the message defined for all locations and bases.
        /// </summary>
        /// <returns></returns>
        private IList<Mensaje> FindAllParentMessages()
        {
            var dc = GetMensajeDetachedCriteria();

            var crit = GetMensajeCriteria(0, dc, Order.Asc("Codigo"));

            return crit.List<Mensaje>();
        }

        /// <summary>
        /// Updates values for the childs of the currently edited message.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateChildsValues(Mensaje message)
        {
            if (!message.TipoMensaje.EsGenerico) return;

            if (message.Linea != null || message.Empresa != null) return;

            var childs = FindChilds(message);

            foreach (var child in childs)
            {
                child.TipoMensaje = message.TipoMensaje;
                child.Origen = message.Origen;
                child.Destino = message.Destino;
                child.EsAlarma = message.EsAlarma;
                child.Codigo = message.Codigo;

                SaveOrUpdate(child);
            }
        }

        /// <summary>
        /// Gets all childs messages for the current message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private IEnumerable<Mensaje> FindChilds(Mensaje message)
        {
            var dc = GetMensajeDetachedCriteria()
                .Add(Restrictions.Not(Restrictions.Eq("Id", message.Id)))
                .Add(Restrictions.Eq("Codigo", message.Codigo));

            return GetMensajeCriteria(0, dc, Order.Asc("Codigo")).List<Mensaje>();
        }

        /// <summary>
        /// Gets the next value for a message revision.
        /// </summary>
        /// <returns></returns>
        private int GetNextRevisionValue()
        {
            var p = Projections.SqlFunction(new VarArgsSQLFunction("(", "+", ")"), NHibernateUtil.Int32, Projections.Max("Revision"),
                Projections.Constant(1));

            var crit = Session.CreateCriteria<Mensaje>()
                .SetProjection(p);

            return crit.UniqueResult<int>();
        }

        #endregion
    }
}