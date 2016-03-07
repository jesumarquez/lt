using System;

namespace Logictracker.Model
{
    public interface IFoteable : INode
    {
		/// <summary>
		/// </summary>
		IDataTransportLayer DataTransportLayer { get; set; }

        String FotaFolder { get;}

		/// <summary>
        /// </summary>
		Boolean ReloadFirmware(ulong messageId);

        Boolean ReloadMessages(ulong messageId);

        Boolean ResetFMIOnGarmin(ulong messageId);

        Boolean ClearDeviceQueue(ulong messageId);
        /// <summary>
        /// </summary>
		Boolean ReloadConfiguration(ulong messageId);

    	/// <summary>
    	/// </summary>
		/// <param name="line">mensaje que esta pendiente de ser enviado al dispositivo tal y como aparece en el archivo</param>
    	/// <returns>el mismo mensaje pero en el formato para enviar al dispositivo</returns>
		Boolean ContainsMessage(String line);

	    ulong GetMessageId(String line);

        Boolean? IsGarminConnected { get; set; }
        INodeMessage LastSent { get; set; }
	}

    #region Nested type: MessagingDevice

    public abstract class MessagingDevice
    {
        public const String Sms = "SMS";
        public const String Mpx01 = "MPX01";
        public const String Garmin = "GARMIN";
    }

    #endregion

	public class INodeMessage
	{
		public readonly ulong Id;
		public String Text;
		public DateTime Time;
		public bool IsOnTheFly;
	    public DateTime? ExpiresOn;

        public INodeMessage(ulong id, String text, DateTime time) : this(id, text, time, null)
        {
            
        }

        public INodeMessage(ulong id, String text, DateTime time, DateTime? expiresOn)
		{
			Id = id;
			Text = text;
			Time = time;
			IsOnTheFly = false;
		    ExpiresOn = expiresOn ?? DateTime.UtcNow.AddSeconds(10);
		}
	}

	public static class INodeMessageX
	{
		public static ulong GetId(this INodeMessage me)
		{
			return me == null ? 0 : me.Id;
		}

		public static String GetText(this INodeMessage me, String defaultvalue)
		{
			return me == null ? defaultvalue : me.Text;
		}

		public static DateTime GetTime(this INodeMessage me, DateTime defaultvalue)
		{
			return me == null ? defaultvalue : me.Time;
		}

		public static bool IsOnTheFly(this INodeMessage me)
		{
			return me != null && me.IsOnTheFly;
		}

        public static bool IsExpired(this INodeMessage me)
        {
            if (me == null || me.ExpiresOn == null)
            {
                return false;
            }
            var result = DateTime.UtcNow > me.ExpiresOn;

            return result;
        }
	}
}