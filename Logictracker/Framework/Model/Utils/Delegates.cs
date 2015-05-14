#region Usings

using System.Net;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Model.Utils
{
    public delegate bool NetworkFrameReceivedDelegate(IUnderlayingNetworkLayer sender, ILink link, IFrame frame);

	public delegate bool NetworkChangeDelegate(IUnderlayingNetworkLayer sender, ILink link, IFrame frame);

	public delegate bool LinkStateDelegate(IDataLinkLayer sender, int deviceId, IMessage message);

    public delegate ILink LinkTranslationDelegate(ILayer sender, EndPoint path, IFrame frame);

	public delegate void LinkMessageReceivedDelegate(IDataLinkLayer sender, int deviceId, IMessage message);

	public delegate void PointChangeStateDelegate(IDataTransportLayer sender, int deviceId, IMessage message);

    public delegate void PointMessageReceivedDelegate(IDataTransportLayer sender, int deviceId, IMessage message);

    public delegate HandleResults MessageHandlerDelegate<TYPE>(TYPE message) where TYPE : IMessage;
}