using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Vehiculos;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Vehiculos
{
    public class MovOdometroVehiculoDAO : GenericDAO<MovOdometroVehiculo>
    {
        #region Constructor

//        public MovOdometroVehiculoDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public void ResetOdometer(int id)
        {
            var odometro = FindById(id);

            if (odometro != null && odometro.Odometro.EsReseteable)
            {
                odometro.ResetOdometerValues();
                SaveOrUpdate(odometro);
            }
        }

        public void UpdateOdometer(int id, double ajusteKm, int ajusteDias, double ajusteHoras)
        {
            var odometro = FindById(id);

            if (odometro != null)
            {
                odometro.AjusteKilometros = ajusteKm;
                odometro.AjusteDias = ajusteDias;
                odometro.AjusteHoras = ajusteHoras;
                SaveOrUpdate(odometro);
            }
        }

        public List<MovOdometroVehiculo> GetByOdometro(int odometro)
        {
            return Session.Query<MovOdometroVehiculo>()
                .Where(c => c.Odometro.Id == odometro)
                .ToList();
        }

        public IEnumerable<MovOdometroVehiculo> GetForVehicles(List<Int32> vehiculos, List<Int32> odometros, Boolean porVencer)
        {
            return Session.Query<MovOdometroVehiculo>()
                .Where(c => vehiculos.Contains(c.Vehiculo.Id) && odometros.Contains(c.Odometro.Id)
                    && (!porVencer || ((c.Odometro.PorTiempo && c.Dias >= c.Odometro.Alarma1Tiempo) 
                                    || (c.Odometro.PorKm && c.Kilometros >= c.Odometro.Alarma1Km) 
                                    || (c.Odometro.PorTiempo && c.Horas >= c.Odometro.Alarma1Horas))))
                .ToList();
        }

        #endregion

        public void ResetByVehicleAndInsumo(Coche vehiculo, Insumo insumo)
        {
            var odometros = GetByVehicleAndInsumo(vehiculo, insumo);

            foreach (var odometro in odometros)
                ResetOdometer(odometro.Id);
        }

        private IEnumerable<MovOdometroVehiculo> GetByVehicleAndInsumo(Coche vehiculo, Insumo insumo)
        {
            return Session.Query<MovOdometroVehiculo>()
                          .Where(o => o.Vehiculo == vehiculo
                                   && o.Odometro.Insumo != null
                                   && o.Odometro.Insumo == insumo);
        }
    }
}