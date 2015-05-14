#region Usings

using System;
using System.Collections.Generic;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;

#endregion

namespace Logictracker.Layers
{
    [FrameworkElement(XName = "FiltrarRepetidos", IsContainer = false)]
	public class FiltrarRepetidos : FrameworkElement, IDispatcherLayer
    {
		/// <summary>
		/// Flag que indica si se debe utilizar la base para persistir los datos
		/// </summary>
		[ElementAttribute(XName = "Persistente", DefaultValue = false)]
		public bool PersisteFlag { get; set; }

		#region IDispatcherLayer

		public BackwardReply Dispatch(IMessage msg)
		{
			try
			{
				return Procesar(msg);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(FiltrarRepetidos).FullName, e);
				return BackwardReply.None;
			}
		}

    	public bool ServiceStart()
		{
			return true;
		}

		public bool ServiceStop()
		{
			return true;
		}

		public bool StackBind(ILayer bottom, ILayer top)
		{
			if (top is IDispatcherLayer)
			{
				_dispatcher = top as IDispatcherLayer;
				return true;
			}
			STrace.Error(GetType().FullName, "Falta IDispatcherLayer!");
			return false;
		}

		#endregion

		#region Members

		[ThreadStatic]
		private static DAOFactory _daoFactory;
		private static DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }

    	private readonly Dictionary<int, List<String>> _lista = new Dictionary<int, List<String>>();

		private IDispatcherLayer _dispatcher;

		private BackwardReply Procesar(IMessage msg)
		{
			if (msg == null) return BackwardReply.None;
			var msgType = msg.GetType();
			if (msgType.FullName == null) return BackwardReply.None;

			var ev = msg as Event;
			var hash = ev != null
				? String.Format("{0}:{1:yyyy/MM/dd HH:mm:ss}:{2} {3} {4}:{5}", ev.DeviceId, ev.GetDateTime(), ev.Code, ev.GetData(), ev.GetData2(), msgType.Name)
				: String.Format("{0}:{1:yyyy/MM/dd HH:mm:ss}:{2}", msg.DeviceId, msg.GetDateTime(), msgType.Name);

			if (PersisteFlag)
			{
				var controlaRepetidosFlag = DaoFactory.DetalleDispositivoDAO.GetDeviceDetail(msg.DeviceId, "GTE_FILTRAR_REPETIDOS").AsBoolean(false);
				if (controlaRepetidosFlag)
				{
					if (DaoFactory.ReportsCacheDAO.ExistsValue(msg.DeviceId, hash))
					{
						STrace.Debug(typeof(FiltrarRepetidos).FullName, msg.DeviceId, String.Format("Ignorando reporte repetido (Base): {0}", hash));
						return BackwardReply.None;
					}
					var cache = new ReportsCache { Dispositivo = msg.DeviceId, DateTime = msg.GetDateTime(), Value = hash };
					DaoFactory.ReportsCacheDAO.Save(cache);
				}
			}
			else
			{
				lock (_lista)
				{
					if (!_lista.ContainsKey(msg.DeviceId))
					{
						_lista.Add(msg.DeviceId, new List<string>());
					}
					else if (_lista[msg.DeviceId].Contains(hash))
					{
						STrace.Debug(typeof(FiltrarRepetidos).FullName, msg.DeviceId, String.Format("Ignorando reporte repetido (Memoria): {0}", hash));
						return BackwardReply.None;
					}

					var cache = _lista[msg.DeviceId];
					cache.Insert(0, hash);
					if (cache.Count > 40) cache.RemoveRange(25, 15);
				}
			}

			//STrace.Debug(typeof(FiltrarRepetidos).FullName, msg.DeviceId, "Despachando: {0}", hash);
			return _dispatcher.Dispatch(msg);
		}

		public bool IsRepetido(IMessage msg)
		{
			if (msg == null) return false;
			var msgType = msg.GetType();
			if (msgType.FullName == null) return false;

			var ev = msg as Event;
			var hash = ev != null
				? String.Format("{0}:{1:yyyy/MM/dd HH:mm:ss}:{2} {3} {4}:{5}", ev.DeviceId, ev.GetDateTime(), ev.Code, ev.GetData(), ev.GetData2(), msgType.Name)
				: String.Format("{0}:{1:yyyy/MM/dd HH:mm:ss}:{2}", msg.DeviceId, msg.GetDateTime(), msgType.Name);

			lock (_lista)
			{
				if (!_lista.ContainsKey(msg.DeviceId))
				{
					_lista.Add(msg.DeviceId, new List<string>());
				}
				else if (_lista[msg.DeviceId].Contains(hash))
				{
					STrace.Debug(typeof (FiltrarRepetidos).FullName, msg.DeviceId, String.Format("Ignorando reporte repetido (Memoria): {0}", hash));
					return true;
				}

				var cache = _lista[msg.DeviceId];
				cache.Insert(0, hash);
				if (cache.Count > 40) cache.RemoveRange(25, 15);
			}

			STrace.Debug(typeof(FiltrarRepetidos).FullName, msg.DeviceId, String.Format("Despachando: {0}", hash));
			return false;
		}

		#endregion
    }
}
