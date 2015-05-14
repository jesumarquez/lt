using Logictracker.Utils;

namespace Logictracker.Model
{
    ///<summary>
    /// Tiene sistema de aceptacion de lista de stops para cumplir.
    ///</summary>
	public interface IRoutable : INode
    {
		bool LoadRoute(ulong messageId, Destination[] route, bool sort, int routeId);

        bool ReloadRoute(ulong messageId, Destination[] route, bool sort, int routeId);

        bool UnloadStop(ulong messageId, Destination[] route);

        bool UnloadRoute(ulong messageId, Destination[] route, int routeId);

		bool UnloadRoute(ulong messageId, int routeId);
	}

	public class Destination
	{
		public int Code;
		public GPSPoint Point;
		public string Text;
        public string Name;
        public string Address;

		public Destination(int code, GPSPoint point, string text, string name, string address)
		{
			Code = code;
			Point = point;
			Text = text;
		    Name = name;
		    Address = address;
		}

		public Destination()
		{
		}
	}
}