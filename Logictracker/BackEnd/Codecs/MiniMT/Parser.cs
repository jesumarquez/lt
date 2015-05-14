using System;
using System.Text;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Model;

namespace Logictracker.MiniMT.v1
{
    [FrameworkElement(XName = "MiniMTParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Minimt; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2003)]
		public override int Port { get; set; }

		#endregion

		#region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

		public override INode Factory(IFrame frame, int formerId)
        {
			return DataProvider.FindByIMEI(GetImei(frame.Payload), this);
        }

		public override IMessage Decode(IFrame frame)
		{
            var buffer = Encoding.ASCII.GetString(frame.Payload, 4, frame.Payload.Length - 4);
			if (buffer.Length < 4)
			{
				return null;
			}

			//var api = Convert.ToInt16(buffer[0]) * 256 + Convert.ToInt16(buffer[1]);
			//var type = Convert.ToInt16(buffer[2]);

    		return PositionReport.IsPositionReport(buffer) ? PositionReport.Factory(buffer, Id) : null;
		}

		#endregion

        #region Properties

		private static String GetImei(byte[] p)
        {
            var buffer = Encoding.ASCII.GetString(p, 4, p.Length - 4);
            return buffer.Split(',')[0].Substring(4).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];
        }
        
        #endregion
    }
}