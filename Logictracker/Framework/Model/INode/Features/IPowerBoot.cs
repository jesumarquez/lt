namespace Logictracker.Model
{
    /// <summary>
    /// El INode puede ser reinicializado.
    /// </summary>
	public interface IPowerBoot : INode
    {
        /// <summary>
        /// Reinicializa el INode
        /// </summary>
        bool Reboot(ulong messageId);
    }
}