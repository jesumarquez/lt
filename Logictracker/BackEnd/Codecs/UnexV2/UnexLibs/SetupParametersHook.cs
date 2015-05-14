#region Usings

using System;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v2.UnexLibs
{
	[FrameworkElement(XName = "SetupParametersHook", IsContainer = false)]
	public class SetupParametersHook : FrameworkElement, IMessageHook
	{
		[ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
		public IDataProvider DataProvider
		{
			get { return (IDataProvider)GetValue("DataProvider"); }
			set { SetValue("DataProvider", value); }
		}

		public void HookUpMessage(IMessage message, INode parser)
		{
			if (!(message is ConfigRequest)) return;
			var cfm = message as ConfigRequest;

			var config = 0;
			var messages = 0;
			var firmware = "undefined";
			if (cfm.IntegerParameters.ContainsKey(Indication.indicatedConfigurationRevision)) config = cfm.IntegerParameters[Indication.indicatedConfigurationRevision];
			if (cfm.IntegerParameters.ContainsKey(Indication.indicatedCannedMessagesTableRevision)) messages = cfm.IntegerParameters[Indication.indicatedCannedMessagesTableRevision];
			if (cfm.StringParameters.ContainsKey(Indication.indicatedFirmwareSignature)) firmware = cfm.StringParameters[Indication.indicatedFirmwareSignature];

			var target = DataProvider.Get(message.DeviceId, parser) as IProvisioned;
			if (target == null) return;

			try
			{
				DetalleDispositivo[] parametros;
				var hash = target.GetHashFromRevisions(out parametros);
				STrace.Debug(GetType().FullName, message.DeviceId, String.Format("SetupParametersHook: VCNF={0}/{1}; VMSG={2}; VFIRM={3}", config, hash, messages, firmware));

				if (hash == config) return;
				var convertsKnots = DataProvider.GetDetalleDispositivo(message.DeviceId, "converts_knots").AsBoolean(true);
				foreach (var pp in parametros)
				{
					var p = pp;
					parser.ExecuteOnGuard(() => target.SetParameter(0, p.TipoParametro.Nombre, DeviceFormatted(convertsKnots, p.TipoParametro.TipoDato, p.Valor), p.Revision, hash), "SetupParametersHook.HookUpMessage", "SetParameter:" + p.As(default(String)));
				}
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
			}
		}

		private static String DeviceFormatted(Boolean convertsKnots, String type, String value)
		{
			if (type == "km/h" && convertsKnots) return ((int)Speed.KmToKnot(Convert.ToSingle(value))).ToString(CultureInfo.InvariantCulture);

			if (type == "knots" && (!convertsKnots)) return ((int)Speed.KnotToKm(Convert.ToSingle(value))).ToString(CultureInfo.InvariantCulture);

			return value;
		}
	}
}