namespace Logictracker.Model
{
    public interface IDataTransportLayer : ILayer
    {
		void DispatchMessage(INode device, IMessage message);
    }
}
