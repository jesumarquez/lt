using System;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using NHibernate.Criterion;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class InfraccionDAO : GenericDAO<Infraccion>
    {
//        public InfraccionDAO(ISession session) : base(session) { }

        public IList<Infraccion> GetByEmpleados(IEnumerable<int> empleados, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<Infraccion>()
                .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(Restrictions.InG(Projections.Property<Infraccion>(p => p.Empleado.Id), empleados))
                .List<Infraccion>();
        }
        public IList<Infraccion> GetByVehiculos(IEnumerable<int> vehiculos, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<Infraccion>()
                .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(Restrictions.InG(Projections.Property<Infraccion>(p => p.Vehiculo.Id), vehiculos))
                .List<Infraccion>();
        }
        public IList<Infraccion> GetByVehiculos(IEnumerable<int> vehiculos, short codigo, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<Infraccion>()
                .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(m => m.CodigoInfraccion == codigo)
                .And(Restrictions.InG(Projections.Property<Infraccion>(p => p.Vehiculo.Id), vehiculos))
                .List<Infraccion>();
        }
        public IList<Infraccion> GetByVehiculo(int vehiculo, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<Infraccion>()
                .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(p => p.Vehiculo.Id == vehiculo)
                .List<Infraccion>();
        }
        public IList<Infraccion> GetByVehiculo(int vehiculo, short codigo, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<Infraccion>()
                .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(p => p.Vehiculo.Id == vehiculo)
                .And(m => m.CodigoInfraccion == codigo)
                .List<Infraccion>();
        }
        public int Count(int vehiculo, short codigo, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<Infraccion>()
                .Where(m => m.Vehiculo.Id == vehiculo)
                .And(m => m.Fecha >= desde && m.Fecha <= hasta)
                .And(m => m.CodigoInfraccion == codigo)
                .Select(Projections.RowCount())
                .SingleOrDefault<int>();
        }
    }
}