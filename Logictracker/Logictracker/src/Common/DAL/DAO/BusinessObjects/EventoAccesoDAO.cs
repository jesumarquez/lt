#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class EventoAccesoDAO : GenericDAO<EventoAcceso>
    {
//        public EventoAccesoDAO(ISession session) : base(session) {}

        public List<EventoAcceso> FindByEmpresaLineaEmpleadosAndFecha(Int32 empresa, Int32 linea, List<Int32> empleados, DateTime desde, DateTime hasta)
        {
            return Session.Query<EventoAcceso>().Where(e => e.Fecha >= desde && e.Fecha <= hasta && e.Puerta.Empresa.Id == empresa 
                && (linea <= 0 || e.Puerta.Linea == null || e.Puerta.Linea.Id == linea) && (e.Empleado == null || empleados.Contains(e.Empleado.Id))).ToList();
        }

        public List<EventoAcceso> FindByEmpleadosAndFecha(List<Int32> empleados, DateTime desde, DateTime hasta)
        {
            return Session.Query<EventoAcceso>().Where(e => e.Fecha >= desde && e.Fecha <= hasta && e.Empleado != null && empleados.Contains(e.Empleado.Id)).ToList();
        }

        public EventoAcceso FindLastEventForEmployee(Int32 employee, DateTime date)
        {
            return Session.Query<EventoAcceso>()
                          .Where(acces => acces.Fecha < date 
                                       && acces.Empleado != null 
                                       && acces.Empleado.Id == employee)
                          .OrderByDescending(access => access.Fecha)
                          .FirstOrDefault();
        }

        public EventoAcceso FindLastEventForCompanyAndBase(Int32 empresa, Int32 linea, DateTime date)
        {
            return Session.Query<EventoAcceso>()
                          .Where(acces => acces.Fecha < date
                                       && acces.Empleado != null
                                       && acces.Empleado.Empresa != null
                                       && acces.Empleado.Empresa.Id == empresa
                                       && (linea == -1 || acces.Empleado.Linea == null || acces.Empleado.Linea.Id == linea))
                          .OrderByDescending(access => access.Fecha)
                          .FirstOrDefault();
        }

        public IList<EventoAcceso> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> centros, IEnumerable<int> tiposEmpleados, IEnumerable<int> empleados, IEnumerable<int> puertas, DateTime desde, DateTime hasta)
        {
            Empleado ch = null;
            EventoAcceso ea = null;

            var empQ = QueryOver.Of(() => ch)
                                .Where(Restrictions.EqProperty(Projections.Property(() => ch.Id), Projections.Property(() => ea.Empleado.Id)));

            // Empresas
            if (empresas.Any(e=>e>0))
            {
                empQ = empQ.And(Restrictions.Or(
                        Restrictions.IsNull(Projections.Property<Empleado>(p => ch.Empresa))
                        , Restrictions.InG(Projections.Property<Empleado>(p => ch.Empresa.Id), empresas)));
            }

            // Lineas
            if (lineas.Any(e => e > 0))
            {
                empQ = empQ.And(Restrictions.Or(
                        Restrictions.IsNull(Projections.Property<Empleado>(p => ch.Linea))
                        , Restrictions.InG(Projections.Property<Empleado>(p => ch.Linea.Id), lineas)));
            }

            // Centros de Costo
            if (centros.Any(e => e > 0))
            {
                empQ = empQ.And(Restrictions.Or(
                        Restrictions.IsNull(Projections.Property<Empleado>(p => ch.CentroDeCostos))
                        , Restrictions.InG(Projections.Property<Empleado>(p => ch.CentroDeCostos.Id), centros)));
            }

            // Tipo de Empleado
            var tipoNone = tiposEmpleados.Contains(-2);
            var tipoOther = tiposEmpleados.Any(t => t > 0);
            var tipoAll = tiposEmpleados.Contains(-1);

            if (!tipoAll)
            {
                if (tipoNone && tipoOther)
                {
                    empQ = empQ.And(Restrictions.Or(
                        Restrictions.IsNull(Projections.Property<Empleado>(p => ch.TipoEmpleado))
                        , Restrictions.InG(Projections.Property<Empleado>(p => ch.TipoEmpleado.Id), tiposEmpleados)));
                }
                else
                {
                    if (tipoNone) empQ = empQ.And(Restrictions.IsNull(Projections.Property<Empleado>(p => ch.TipoEmpleado)));
                    if (tipoOther)
                        empQ = empQ.And(Restrictions.InG(Projections.Property<Empleado>(p => ch.TipoEmpleado.Id), tiposEmpleados));
                }
            }

            // Empleados
            if (empleados.Any(e => e > 0))
            {
                empQ = empQ.And(Restrictions.InG(Projections.Property<Empleado>(p => ch.Id), empleados));
            }

            

            var q = Session.QueryOver(() => ea)
                .Where(dm => dm.Fecha >= desde && dm.Fecha <= hasta);

            // Puertas
            if (puertas.Any(e => e > 0))
            {
                q = q.And(Restrictions.InG(Projections.Property<EventoAcceso>(p => p.Puerta.Id), puertas));
            }

            return q.WithSubquery.WhereExists(empQ.Select(x=>x.Id))
                .OrderBy(Projections.Property<EventoAcceso>(p => p.Empleado)).Asc
                .ThenBy(Projections.Property<EventoAcceso>(p => p.Fecha)).Asc
                .List();
           

        }
    }
}
