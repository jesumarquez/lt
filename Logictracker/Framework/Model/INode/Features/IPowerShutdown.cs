namespace Logictracker.Model
{
    /// <summary>
    /// El INode puede apagarse ordenadamente y cortar su entrada de energia.
    /// </summary>
	public interface IPowerShutdown : INode
    {
        /// <summary>
        /// Apaga el INode
        /// </summary>
        bool Shutdown();
    }
}
