namespace Logictracker.Model
{
    public interface IMessageHook
    {
        void HookUpMessage(IMessage message, INode device);
    }
}
