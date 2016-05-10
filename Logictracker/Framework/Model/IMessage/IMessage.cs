#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Logictracker.Model.Utils;
using Logictracker.Utils;

#endregion

namespace Logictracker.Model
{
	/// <summary>
	/// Interface esencial de mensajes entre procesos.
	/// </summary>
	/// <remarks>
	/// Esta interface se ubica en la raiz de la intercomunicacion entre procesos,
	/// y provee de la funcionalidad basica para el encaminamiento e identificacion
	/// de los mensajes.
	/// </remarks>
	public interface IMessage
	{
		/// <summary>
		/// Identificador univoco del mensaje.
		/// </summary>
		ulong UniqueIdentifier { get; }

		/// <summary>
		/// Identificador del dispositivo que origino el mensaje.
		/// </summary>
		int DeviceId { get; }

		/// <summary>
		/// Parametros de usuario
		/// </summary>
		Dictionary<String, String> UserSettings { get; }

		/// <summary>
		/// 
		/// </summary>
		DateTime Tiempo { get; set; }

		/// <summary>
		/// En esta propiedad se setea la respuesta a enviar para este reporte
		/// </summary>
		byte[] Response { get; set; }
	}

	public static class IMessageX
	{
		public static T AddStringToSend<T>(this T msg, String buffer) where T : class, IMessage
		{
			if (msg == null) return null;
			if (msg.HasUserSetting(ToSend))
				msg.UserSettings[ToSend] = msg.UserSettings[ToSend] + buffer;
			else
				msg.UserSettings.Add(ToSend, buffer);
			return msg;
		}

		public static T SetUserSetting<T>(this T msg, String key, String value) where T : class, IMessage
		{
			if (msg == null) return null;
			if (msg.UserSettings.ContainsKey(key))
			{
				msg.UserSettings[key] = value;
			}
			else
			{
				msg.UserSettings.Add(key, value);
			}
			return msg;
		}

        public static T AddStringToPostSend<T>(this T msg, String buffer) where T : class, IMessage
		{
			if (msg == null) return msg;
			if (msg.HasUserSetting(PostSend))
				msg.UserSettings[PostSend] = msg.UserSettings[PostSend] + buffer;
			else
				msg.UserSettings.Add(PostSend, buffer);
		    return msg;
		}

		public static bool IsPendingPost(this IMessage msg)
		{
			return msg.HasUserSetting(PostSend);
		}

		public static byte[] GetPendingPost(this IMessage msg)
		{
			return msg == null ? null : (!msg.HasUserSetting(PostSend) ? null : Encoding.ASCII.GetBytes(msg.GetUserSetting(PostSend)));
		}

		public static String GetPendingPostAsString(this IMessage msg)
		{
			return msg.GetUserSetting(PostSend);
		}

		public static void AddBytesToSend(this IMessage msg, byte[] bytes)
		{
			if (msg == null) return;
			msg.Response = bytes;
		}

		public static bool IsPending(this IMessage msg)
		{
			return msg != null && (msg.Response != null || msg.GetUserSetting(ToSend) != null);
		}

		public static byte[] GetPending(this IMessage msg)
		{
			if (msg == null) return null;
			if (msg.Response != null) return msg.Response;
			var pen = msg.GetUserSetting(ToSend);
			return pen == null ? null : Encoding.ASCII.GetBytes(pen);
		}

		public static String GetPendingAsString(this IMessage msg)
		{
			if (msg == null) return null;
			return msg.Response != null
				? StringUtils.ByteArrayToHexString(msg.Response, 0, msg.Response.Length)
				: msg.GetUserSetting(ToSend);
		}

		public static int GetDeviceId(this IMessage msg)
		{
			return msg == null ? 0 : msg.DeviceId;
		}

		public static Boolean IsInvalidDeviceId(this IMessage msg)
		{
			return (msg == null) || (msg.DeviceId == ParserUtils.CeroDeviceId) || (msg.DeviceId == ParserUtils.WithoutDeviceId);
		}

		public static bool HasUserSetting(this IMessage msg, String key)
		{
			return (msg != null && msg.UserSettings != null) && msg.UserSettings.ContainsKey(key) && msg.UserSettings[key] != null;
		}

		public static String GetUserSetting(this IMessage msg, String key)
		{
			if (msg == null || msg.UserSettings == null || !msg.UserSettings.ContainsKey(key)) return null;
			return msg.UserSettings[key];
		}

		public static DateTime GetDateTime(this IMessage msg)
		{
			return msg == null ? new DateTime(2000, 1, 1) : msg.Tiempo;
		}

		private const String ToSend = "ToSend";
		private const String PostSend = "PostSend";

	    public static byte[] BinarySerialized(this IMessage msg)
	    {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, msg);
                stream.Position = 0;
                return stream.ToArray();
            }
	    }
	}
}