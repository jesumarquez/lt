#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Interfaces.Combustible
{
    public class PozoMessage : IDisposable
    {
        #region Private Properties

        private readonly List<Movimiento> movements = new List<Movimiento>();

        private readonly List<VolumenHistorico> historicalVolumes = new List<VolumenHistorico>();

        private readonly List<Movimiento> movementsLog = new List<Movimiento>();

        private readonly List<VolumenHistorico> historicalVolumesLog = new List<VolumenHistorico>();

        #endregion

        #region Public Methods

        public void addMovimiento(Movimiento mov) { movements.Add(mov); }

        public void addHistoricalVolume(VolumenHistorico vol) { historicalVolumes.Add(vol); }

        public void addMovimientoLog(Movimiento mov) { movementsLog.Add(mov); }

        public void addHistoricalVolumeLog(VolumenHistorico vol) { historicalVolumesLog.Add(vol); }

        public List<Movimiento> getMovimientos() { return movements; }

        public List<VolumenHistorico> getHistoricalVolumes() { return historicalVolumes; }

        public List<Movimiento> getMovimientosLog() { return movementsLog; }

        public List<VolumenHistorico> getHistoricalVolumesLog() { return historicalVolumesLog; }

        public void Dispose()
        {
            movements.Clear();

            historicalVolumes.Clear();

            movementsLog.Clear();

            historicalVolumesLog.Clear();
        }

        #endregion
    }
}
