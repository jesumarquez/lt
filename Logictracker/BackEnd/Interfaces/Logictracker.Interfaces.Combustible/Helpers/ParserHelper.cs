#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Interfaces.Combustible.Helpers
{
    public class ParserHelper
    {
        #region Constants

        private const Double MaxCaudalValue = 500.00;

        #endregion

        #region Private Properties

		private static readonly Dictionary<String, Movimiento> LastMovements = new Dictionary<String, Movimiento>();
		private static readonly Dictionary<String, VolumenHistorico> LastTankNivel = new Dictionary<String, VolumenHistorico>();

        #endregion

        #region Public Methods

		public static DateTime ParseDate(String dateStr)
        {
            var str = dateStr.Split(' ');
            var date = str[0].Split('/');
            var time = str[1].Split(':');
            var year = Convert.ToInt32(date[2]);
            var month = Convert.ToInt32(date[1]);
            var day = Convert.ToInt32(date[0]);
            var hour = Convert.ToInt32(time[0]);
            var minutes = Convert.ToInt32(time[1]);
            var seconds = Convert.ToInt32(time[2]);

            return new DateTime(year, month, day, hour, minutes, seconds);
        }

        #region Motor Methods

        /// <summary>
        /// Parses a Motor Type Message.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="alias"></param>
        /// <param name="volumen"></param>
        /// <param name="fecha"></param>
        /// <param name="caudal"></param>
        /// <param name="hsEnMarcha"></param>
        /// <param name="temperatura"></param>
        /// <param name="rpm"></param>
        /// <param name="entries"></param>
        /// <param name="events"></param>
        /// <returns></returns>
		public Movimiento ProcessMotor(DAOFactory daoFactory, String center, String id, String alias, Double volumen, DateTime fecha, Double caudal, Double hsEnMarcha, Double temperatura, Double rpm, String entries, String events)
        {
            var mov = new Movimiento
                          {
                              Fecha = fecha,
                              FechaIngresoABase = DateTime.Now,
                              Volumen = volumen,
                              Tanque = null,
                              Coche = null,
                              Observacion = null,
                              Estado = 0,
                              TipoMovimiento = daoFactory.TipoMovimientoDAO.GetByCode("M"),
                              Caudalimetro = daoFactory.CaudalimetroDAO.GetByCode(id),
                              Caudal = caudal,
                              HsEnMarcha = hsEnMarcha,
                              Temperatura = temperatura,
                              RPM = rpm,
                              Procesado = false
                          };

            if (mov.Caudalimetro == null) mov.Caudalimetro = CreateNewMotor(daoFactory, center, id, alias);

            if (!mov.Caudalimetro.Descripcion.Equals(alias)) UpdateMotorDescription(daoFactory, mov.Caudalimetro, alias);

            if (mov.Caudal > MaxCaudalValue)
            {
                var message = String.Format("The engine {0} ({1}) reported a value greater than {2}", id, alias, MaxCaudalValue);

				STrace.Error(GetType().FullName, message);

                return null;
            }

            try
            {
                EventHelper.EvaluteMaximumCaudal(mov.Caudal, mov.Caudalimetro, fecha);
                EventHelper.ProcessMotorEntries(center, id, entries, fecha);
                EventHelper.ProcessMotorAndMedidorEvents(center, id, null, events, fecha);
            }
            catch (Exception)
            {
				STrace.Error(GetType().FullName, String.Format("Error on type M: \nENTRIES={0} \nEVENTS={1}", entries, events));

                throw;
            }

            if (MovementHasChanged(mov, mov.Caudalimetro.Codigo))
            {
                SetLastMovimientoChange(mov, mov.Caudalimetro.Codigo);

                return mov;
            }

            return null;
        }

        #endregion

        #region MedidorMethods

        /// <summary>
        /// Parses a Medidor Type Message.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="alias"></param>
        /// <param name="volumen"></param>
        /// <param name="nroRemito"></param>
        /// <param name="fecha"></param>
        /// <param name="events"></param>
        /// <returns></returns>
		public Movimiento ProcessMedidor(DAOFactory daoFactory, String center, String id, String alias, Double volumen, String nroRemito, DateTime fecha, String events)
        {
            var mov = new Movimiento
                          {
                              Fecha = fecha,
                              FechaIngresoABase = DateTime.Now,
                              Volumen = volumen,
                              Observacion = nroRemito,
                              Estado = 0,
                              Coche = null,
                              Caudalimetro = daoFactory.CaudalimetroDAO.GetByCode(id),
                              Tanque = null,
                              Procesado = false,
                              TipoMovimiento = daoFactory.TipoMovimientoDAO.GetByCode("I"),
                          };

            if (mov.Caudalimetro == null) mov.Caudalimetro = CreateNewCaudalimetroDeEntrada(daoFactory, center, id, alias);

            if (!mov.Caudalimetro.Descripcion.Equals(alias)) UpdateMotorDescription(daoFactory, mov.Caudalimetro, alias);

            try { EventHelper.ProcessMotorAndMedidorEvents(center, id, null, events, fecha); }
            catch (Exception)
            {
                STrace.Error(GetType().FullName, String.Format("Error on type V: \nEVENTS={0}", events));

                throw;
            }

            if (MovementHasChanged(mov, mov.Caudalimetro.Codigo))
            {
                SetLastMovimientoChange(mov, mov.Caudalimetro.Codigo);

                return mov;
            }

            return null;
        }

        #endregion

        #region Tank Methods

        /// <summary>
        /// Parses a T type Message.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="center"></param>
        /// <param name="code"></param>
        /// <param name="alias"></param>
        /// <param name="volumen"></param>
        /// <param name="volumenAgua"></param>
        /// <param name="fecha"></param>
        /// <param name="events"></param>
        /// <returns></returns>
		public VolumenHistorico ProcessTanque(DAOFactory daoFactory, String center, String code, String alias, Double volumen, Double volumenAgua, DateTime fecha, String events)
        {
            var vol = new VolumenHistorico
                          {
                              Fecha = fecha,
                              Volumen = volumen,
                              EsTeorico = false,
                              Tanque = daoFactory.TanqueDAO.FindByCode(code),
                              VolumenAgua = volumenAgua
                          };

            if (vol.Tanque == null) vol.Tanque = CreateTanque(daoFactory, center, code, alias);

            if (!vol.Tanque.Descripcion.Equals(alias)) UpdateTankDescription(daoFactory, vol.Tanque, alias);

            try
            {
                EventHelper.ProcessTankEvents(center, code, events, fecha);
                EventHelper.EvaluateStockCriticoAndReposicion(vol.Volumen, vol.Tanque, fecha);
                EventHelper.EvaluteWaterLevel(vol.VolumenAgua, vol.Tanque, fecha);
            }
            catch (Exception)
            {
                STrace.Error(GetType().FullName, String.Format("Events: {0}", events));

                throw;
            }

            if (TankVolumeHasChanged(vol))
            {
                SetLastTankHistoricalVolumeChanged(vol);

                return vol;
            }

            return null;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        ///  Creates a new Tank Object.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="center"></param>
        /// <param name="id"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
		private static Tanque CreateTanque(DAOFactory daoFactory, String center, String id, String alias)
        {
                var t = new Tanque
                {
                    Codigo = id,
                    Descripcion = alias,
                    VolReal = 0,
                    VolTeorico = 0,
                    Equipo = daoFactory.EquipoDAO.GetByCode("PI-" + center),
                    Linea = null
                };

                daoFactory.TanqueDAO.SaveOrUpdate(t);

                return daoFactory.TanqueDAO.FindByCode(id);
        }

        /// <summary>
        /// Creates a new Motor Object.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="center"></param>
        /// <param name="codigo"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
		private static Caudalimetro CreateNewMotor(DAOFactory daoFactory, String center, String codigo, String alias)
        {
            var m = new Caudalimetro
            {
                Codigo = codigo,
                Tanque = daoFactory.TanqueDAO.FindByCode(center),
                Equipo = daoFactory.EquipoDAO.GetByCode("PI-" + center),
                Descripcion = alias,
                EsDeEntrada = false
            };

            daoFactory.CaudalimetroDAO.SaveOrUpdate(m);

            return daoFactory.CaudalimetroDAO.GetByCode(codigo);
        }

        /// <summary>
        /// Creates a new Caudalimetro Object.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="center"></param>
        /// <param name="codigo"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
		private static Caudalimetro CreateNewCaudalimetroDeEntrada(DAOFactory daoFactory, String center, String codigo, String alias)
        {
            var m = new Caudalimetro
            {
                Codigo = codigo,
                Tanque = daoFactory.TanqueDAO.FindByCode(center),
                Equipo = daoFactory.EquipoDAO.GetByCode("PI-" + center),
                Descripcion = alias,
                EsDeEntrada = true
            };

            daoFactory.CaudalimetroDAO.SaveOrUpdate(m);

            return daoFactory.CaudalimetroDAO.GetByCode(codigo);
        }

        /// <summary>
        /// Updates a Tanque Description
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="tanque"></param>
        /// <param name="alias"></param>
		private static void UpdateTankDescription(DAOFactory daoFactory, Tanque tanque, String alias)
        {
            tanque.Descripcion = alias;

            daoFactory.TanqueDAO.SaveOrUpdate(tanque);
        }

        /// <summary>
        /// Updates the description of the Motor.
        /// </summary>
        /// <param name="daoFactory"></param>
        /// <param name="motor"></param>
        /// <param name="alias"></param>
		private static void UpdateMotorDescription(DAOFactory daoFactory, Caudalimetro motor, String alias)
        {
            motor.Descripcion = alias;

            daoFactory.CaudalimetroDAO.SaveOrUpdate(motor);
        }

        /// <summary>
        /// Sets the last value changed in dictionary.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="key"></param>
		private static void SetLastMovimientoChange(Movimiento m, String key)
        {
            if (LastMovements.ContainsKey(key)) LastMovements.Remove(key);

            LastMovements.Add(key, m);
        }

        /// <summary>
        /// Cheks if the movement values have changed compared with the last value inserted. 
        /// </summary>
        /// <param name="mov"></param>
        /// <param name="key"></param>
        /// <returns></returns>
		private static bool MovementHasChanged(Movimiento mov, String key)
        {
            Movimiento m;

            LastMovements.TryGetValue(key, out m);

            return !mov.Equals(m);
        }

        /// <summary>
        /// Sets the last value changed in dictionary.
        /// </summary>
        /// <param name="vol"></param>
        private static void SetLastTankHistoricalVolumeChanged(VolumenHistorico vol)
        {
            if (LastTankNivel.ContainsKey(vol.Tanque.Codigo)) LastTankNivel.Remove(vol.Tanque.Codigo);

            LastTankNivel.Add(vol.Tanque.Codigo, vol);
        }

        /// <summary>
        /// Cheks if the HistoricalVolume of a tank values have changed compared with the last value inserted 
        /// </summary>
        /// <param name="vol"></param>
        /// <returns></returns>
        private static bool TankVolumeHasChanged(VolumenHistorico vol)
        {
            VolumenHistorico m;

            LastTankNivel.TryGetValue(vol.Tanque.Codigo, out m);

            return !vol.Equals(m);
        }

        #endregion
    }
}
