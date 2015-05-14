namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    #region Public Enums

    public enum MovilOptionGroupValue { TipoVehiculo, Estado } ;

    #endregion

    public interface IMovilAutoBindeable : IAutoBindeable
    {
        bool ShowDriverName { get; }
        bool ShowOnlyAccessControl { get; }
        bool HideWithNoDevice { get; }
        bool HideInactive { get; }

        /// <summary>
        /// Determines the property to group by.
        /// </summary>
        MovilOptionGroupValue OptionGroupProperty { get; }

        int Coche { get; }
    }
}