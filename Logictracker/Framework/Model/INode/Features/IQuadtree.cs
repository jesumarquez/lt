namespace Logictracker.Model
{
    /// <summary>
    /// Agrega la caracteristica de soportar quadtree
    /// </summary>
	public interface IQuadtree : INode
    {
        /// <summary>
        /// Actualiza el Qtree en el dispositivo si procede.
        /// </summary>
        /// <param name="messageId">id de seguimiento de operacion.</param>
		/// <param name="full">indicacion de empezar desde la revision 0</param>
		/// <param name="baseRevision">revision que tiene el dispositivo</param>
		bool SyncronizeQuadtree(ulong messageId, bool full, int baseRevision);
    }
}