using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Organizacion;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class PerfilDAO : GenericDAO<Perfil>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public PerfilDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public List<Perfil> FindPerfilesByUsuario(Usuario user)
        {
            var results = new List<Perfil>();

            IEnumerable<Perfil> list;

            if (user.Perfiles.IsEmpty())
            {
                list = FindAll();
            }
            else
            {
                list = (from Perfil p in user.Perfiles where !p.FechaBaja.HasValue orderby p.Descripcion select p).ToList();
            }           

            if (list.ToList().Count > 1)
                results.Add(new Perfil {Id = (-1), Descripcion = CultureManager.GetLabel("TODOS")});

            results.AddRange(list);
            
            return results;
        }

        public IList<MovMenu> FindMovMenuBySistema(List<int> perfiles)
        {
            MovMenu mv = null;

            var dc = DetachedCriteria.For<MovMenu>()
                .CreateAlias("Perfil", "p", JoinType.InnerJoin)
                .CreateAlias("Funcion", "f", JoinType.InnerJoin)
                .Add(Restrictions.IsNull("p.FechaBaja"))
                .Add(Restrictions.IsNull("f.FechaBaja"));

            if (perfiles.Any(p => p != -1))
            {
                dc.Add(Restrictions.In("p.Id", perfiles));
            }

            dc.SetProjection(Projections.Property("Id"));            

            var projectionList = Projections.ProjectionList().Add(
                Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property<MovMenu>(c => c.Alta))).WithAlias(() => mv.Alta))
                .Add(Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property<MovMenu>(c => c.Baja))).WithAlias(() => mv.Baja))
                .Add(Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property<MovMenu>(c => c.Consulta))).WithAlias(() => mv.Consulta))
                .Add(Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property<MovMenu>(c => c.Modificacion))).WithAlias(() => mv.Modificacion))
                .Add(Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property<MovMenu>(c => c.VerMapa))).WithAlias(() => mv.VerMapa))
                .Add(Projections.Max(Projections.Cast(NHibernateUtil.Int32, Projections.Property<MovMenu>(c => c.Reporte))).WithAlias(() => mv.Reporte))
                .Add(Projections.Min(Projections.Property<MovMenu>(c => c.Orden)).WithAlias(() => mv.Orden))
                .Add(Projections.Group<MovMenu>(c => c.Funcion).WithAlias(() => mv.Funcion));


            var preresult = Session.CreateCriteria<MovMenu>()
                .Add(Subqueries.PropertyIn("Id", dc))
                .SetFetchMode("Funcion", FetchMode.Select)
                .SetProjection(projectionList);

            var result = preresult.SetResultTransformer(Transformers.AliasToBean<tmpMovMenu>())
                .List<tmpMovMenu>();

            return result.Select(m => new MovMenu
                                          {
                                              Alta = m.Alta == 1,
                                              Baja = m.Baja == 1,
                                              Consulta = m.Consulta == 1,
                                              Modificacion = m.Modificacion == 1,
                                              VerMapa = m.VerMapa == 1,
                                              Reporte = m.Reporte == 1,
                                              Orden = m.Orden,
                                              Funcion = m.Funcion
                                          })
                .OrderBy(m => m.Funcion.Sistema.Orden)
                .ThenBy(m => m.Funcion.Sistema.Descripcion)
                .ThenBy(m => m.Orden)
                .ThenBy(m => m.Funcion.Descripcion)
                .ToList();
        }

        public List<Asegurable> GetAsegurables(IEnumerable<Perfil> perfiles)
        {
            return perfiles.SelectMany(p => p.Asegurados).Where(ap => ap.Permitido).Select(ap => ap.Asegurable).Distinct().ToList();
        }

        public List<Asegurable> GetAsegurables(List<int> perfiles)
        {
            var allPerfiles = perfiles.Contains(-1) 
                ? FindAll()
                : perfiles.Select(p => FindById(p));
            return GetAsegurables(allPerfiles);
        }
        public override void Delete(Perfil perfil)
        {
            if (perfil == null) return;

            perfil.FechaBaja = DateTime.UtcNow;

            SaveOrUpdate(perfil);
        }

        public IEnumerable<Perfil> FindAllWithFunciones()
        {
            var dc = DetachedCriteria.For<Perfil>()
                .Add(Restrictions.IsNull(Projections.Property<Perfil>(p => p.FechaBaja)))
                .SetProjection(Projections.Property<Perfil>(p => p.Id));
            
            var perfiles =
                Session.CreateCriteria<Perfil>()
                    .Add(Subqueries.PropertyIn("Id", dc))
                    .SetFetchMode("Funciones", FetchMode.Select)
                    .AddOrder(Order.Asc(Projections.Property<Perfil>(p => p.Descripcion)))
                    .SetCacheable(true);
            return perfiles.List<Perfil>().ToList(); /* TODO: use IList everywhere */
            
        }

        public override IEnumerable<Perfil> FindAll()
        {
            var dc = DetachedCriteria.For<Perfil>()
                .Add(Restrictions.IsNull(Projections.Property<Perfil>(p=> p.FechaBaja)))
                .SetProjection(Projections.Property<Perfil>(p=>p.Id));

            var perfiles =
                Session.CreateCriteria<Perfil>()
                    .Add(Subqueries.PropertyIn("Id", dc))
                    .AddOrder(Order.Asc(Projections.Property<Perfil>(p=> p.Descripcion)))
                    .SetCacheable(true);
            return perfiles.List<Perfil>().ToList(); /* TODO: use IList everywhere */
        }

        public IEnumerable<int> GetProfileAccess(Usuario usuario, int selectedProfile, out IEnumerable<MovMenu> modules, out IEnumerable<Asegurable> securables)
        {
            var perfiles = selectedProfile > 0
                    ? new List<int> { selectedProfile }
                    : usuario.Perfiles.IsEmpty()
                            ? new List<int> { -1 }
                            : (from Perfil perfil in usuario.Perfiles select perfil.Id).ToList();

            modules = FindMovMenuBySistema(perfiles);
            securables = GetAsegurables(perfiles);
            return perfiles;
        }

        #endregion

        private class tmpMovMenu
        {
            public int Alta { get; set; }
            public int Baja { get; set; }
            public int Consulta { get; set; }
            public int Modificacion { get; set; }
            public int Reporte { get; set; }
            public int VerMapa { get; set; }
            public short Orden { get; set; }
            public Funcion Funcion { get; set; }
        }
    }
}