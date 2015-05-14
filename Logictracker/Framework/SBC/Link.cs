#region Usings

using System;
using System.Net;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Layers
{
    [Serializable]
    public sealed class Link : ILink
    {
	    public LinkStates State { get; set; }

		public INode Device
		{
			get { return _device; }
			set { _device = value; }
		}
	    private INode _device;

	    public EndPoint EndPoint
	    {
			get { return _endPoint; }
		    set { _endPoint = value; }
	    }
	    private EndPoint _endPoint;

	    public DateTime Age { get; set; }

        public object UserState { get; set; }

		public IUnderlayingNetworkLayer UnderlayingNetworkLayer { get; set; }
    }
}