#region Usings

using System;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;

#endregion

namespace Logictracker.Unetel.v2.UnexLibs
{
	[FrameworkElement(XName = "CannedMessagesTableHook", IsContainer = false)]
	public class CannedMessagesTableHook : FrameworkElement, IMessageHook
	{
		#region Attributes

		[ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
		public IDataProvider DataProvider
		{
			get { return (IDataProvider)GetValue("DataProvider"); }
			set { SetValue("DataProvider", value); }
		}
		
		#endregion

		#region IMessageHook

		public void HookUpMessage(IMessage message, INode parser)
		{
			if (!(message is ConfigRequest)) return;
			var cfm = message as ConfigRequest;
			if (!cfm.IntegerParameters.ContainsKey(Indication.indicatedCannedMessagesTableRevision)) return;
			var rev = cfm.IntegerParameters[Indication.indicatedCannedMessagesTableRevision];
			if (rev == 99999) rev = 0;

			var ism = DataProvider.Get(message.DeviceId, parser) as IShortMessage;
			if (ism == null) return;


			var cmt = DataProvider.GetCannedMessagesTable(parser.Id, rev);
			if (cmt == null) return;
			var cms = cmt.Select(cm => new CannedMessage(Convert.ToInt16(cm.Codigo), cm.Texto, cm.EsBaja, cm.Revision));

			foreach (var cm in cms)
			{
				var cm1 = cm;
				if (cm1.EsBaja)
				{
					parser.ExecuteOnGuard(() => ism.DeleteCannedMessage(0, cm1.Codigo, cm1.Revision), "HookUpMessage", "DeleteCannedMessage");
				}
				else
				{
					parser.ExecuteOnGuard(() => ism.SetCannedMessage(0, cm1.Codigo, cm1.Texto, cm1.Revision), "HookUpMessage", "SetCannedMessage");
				}
			}
		}

		#endregion

		#region Private Members

		private class CannedMessage
		{
			public readonly Int16 Codigo;
			public readonly String Texto;
			public readonly bool EsBaja;
			public readonly Int32 Revision;
			public CannedMessage(Int16 c, String t, bool b, Int32 r)
			{
				Codigo = c;
				Texto = t;
				EsBaja = b;
				Revision = r;
			}
		}
 
		#endregion
	}
}