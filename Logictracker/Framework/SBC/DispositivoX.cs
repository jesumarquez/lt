#region Usings

using System;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Model;
using Urbetrack.Types.BusinessObjects.Dispositivos;

#endregion

namespace Urbetrack.SessionBorder
{
	public static class DispositivoX
	{
		public static String GetNodeConfig(this Dispositivo d, IDataProvider dp)
		{
			var classType = d.TipoDispositivo.Fabricante;
			var type = Type.GetType(classType);

			if (type == null)
			{
				TraceNodeTypeError(classType, d.Id);
				return null;
			}
			var constructor = type.GetConstructor(new Type[0]);
			if (constructor == null)
			{
				TraceNodeTypeError(classType + " - " + type.FullName, d.Id);
				return null;
			}
			var dev = (ACParser)constructor.Invoke(null);

			dev.DataProvider = dp;// voy a necesitar el DataProvider para sacar la configuracion de la base
			return dev is IFoteable ? (dev as IFoteable).GetConfig() : null;
		}

		private static void TraceNodeTypeError(string tipo, int iNodeCode)
		{
			STrace.Debug(typeof(DispositivoX).FullName, iNodeCode, "imposible cargar el tipo: {0}", tipo);
		}
	}
}
