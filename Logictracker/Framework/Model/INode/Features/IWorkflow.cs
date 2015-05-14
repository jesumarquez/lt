using System;

namespace Logictracker.Model
{
    /// <summary>
    /// Soporta ciclos logisticos.
    /// </summary>
	public interface IWorkflow : INode
    {
    	/// <summary>
    	/// Establece el nuevo estado del ciclo logistico.
    	/// </summary>
		/// <param name="messageId"></param>
    	/// <param name="state">Nuevo estado a establecer.</param>
    	/// <param name="messages"></param>
		bool SetWorkflowState(ulong messageId, int state, WorkflowMessage[] messages);
    }

	[Serializable]
	public class WorkflowMessage
	{
		public int Code { get; set; }
		public string Text { get; set; }
		public WorkflowMessage(int code, string text)
		{
			Code = code;
			Text = text;
		}
		public WorkflowMessage() {}
	}
}