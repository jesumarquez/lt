namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    public interface ICaudalimetroAutoBindeable : IAutoBindeable
    {
        
        #region Properties

        /// <summary>
        /// Determines wither to shgow or not DeEntrada caudalimeters
        /// </summary>
        bool ShowDeIngreso { get; }
               
        #endregion
    }
}
