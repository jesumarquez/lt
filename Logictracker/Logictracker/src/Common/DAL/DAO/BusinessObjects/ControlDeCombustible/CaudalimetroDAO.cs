#region Usings

using System;
using System.Collections;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    public class CaudalimetroDAO : GenericDAO<Caudalimetro>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public CaudalimetroDAO(ISession session) : base(session) { }

        #endregion

        public Caudalimetro GetByCode(string code)
        {
            var list = (from Caudalimetro c in Session.Query<Caudalimetro>().ToList() where c.Codigo == code orderby c.Id ascending select c).ToList();

            return list.Count > 0 ? list.First() : null;
        }

        public Caudalimetro FindCaudalimetroDeEntradaByTanque(Int32 tanque)
        {
            return Session.Query<Caudalimetro>().SingleOrDefault(cau => cau.Tanque != null && cau.Tanque.Id == tanque && cau.EsDeEntrada);
        }

        public IEnumerable FindMotoresByEquipo(Equipo equipo)
        {
            return (from Caudalimetro c in Session.Query<Caudalimetro>().ToList()
                    where !c.EsDeEntrada && equipo == null || (c.Equipo != null && c.Equipo.Equals(equipo))
                    select c).ToList();
        }

        public IEnumerable FindByEquipoEmpresaAndLinea(Equipo equipo, Empresa empresa, Linea linea, bool dontShowDeIngreso, Usuario user)
        {
            return (from Caudalimetro c in Session.Query<Caudalimetro>().ToList()
                    where
                        c.Equipo != null &&
                        (c.EsDeEntrada == false || c.EsDeEntrada == !dontShowDeIngreso)
                        && ((equipo == null || c.Equipo == equipo)
                                && (c.Equipo.Empresa == null
                                    || ((empresa == null || c.Equipo.Empresa == empresa)
                                        && (user.Empresas.IsEmpty || user.Empresas.Contains(c.Equipo.Empresa))))
                                && (c.Equipo.Linea == null
                                    || ((linea == null || c.Equipo.Linea == linea)
                                        && (user.Lineas.IsEmpty || user.Lineas.Contains(c.Equipo.Linea)))))
                    select c).ToList();
        }

        /// <summary>
        /// Gets the last message associated to the speficied code for the current engine.
        /// </summary>
        /// <param name="caudalimetro"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public EventoCombustible FindLastMessageByCode(int caudalimetro, string code)
        {
            return Session.Query<EventoCombustible>().Where(evento => evento.Motor.Id == caudalimetro && evento.Mensaje.Codigo == code)
                .OrderByDescending(evento => evento.Fecha).FirstOrDefault();
        }
    }
}