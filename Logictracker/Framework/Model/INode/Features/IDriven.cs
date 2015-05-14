namespace Logictracker.Model
{
    ///<summary>
    /// El INode tiene sistema de identifiacion de chofer.
    ///</summary>
	public interface IDriven : INode
    {
        ///<summary>
        /// Obtiene o establece la identificacion electronica del
        /// chofer del vehiculo.
        ///</summary>
        string DriverIdentifier { get; set; }
    }
}