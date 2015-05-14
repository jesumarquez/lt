#region Usings

using System;

#endregion

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    /// <summary>
    /// Class that represents the assigment of a shift.
    /// </summary>
    public class MovShiftsAssignments
    {
        #region Public Properties

        public virtual Int32 Id { get; set; }

        public virtual Shift Shift { get; set; }
        public virtual TipoCoche VehicleType { get; set; }
        public virtual CentroDeCostos CostCenter { get; set; }
        
        #endregion
    }
}
