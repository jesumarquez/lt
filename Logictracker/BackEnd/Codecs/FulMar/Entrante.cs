using Urbetrack.Common.Messaging;
using Urbetrack.Common.Utils;

namespace Urbetrack.FulMarV1.v1
{
	public static class Entrante
	{
		public static MessageIdentifier GiroCode(byte data)
		{
			var onFlag = StringHelper.AreBitsSet(data, 0x1) ? 1 : 0;
			onFlag += StringHelper.AreBitsSet(data, 0x2) ? 2 : 0;
			onFlag += StringHelper.AreBitsSet(data, 0x4) ? 4 : 0;
			switch (onFlag)
			{
				case 7: return MessageIdentifier.SpinStop2; //EVT_SPIN_NO_SENSOR
				case 6: return MessageIdentifier.MixerClockwiseFast;
				case 5: return MessageIdentifier.MixerCounterClockwiseFast;
				case 4:
				case 3: return MessageIdentifier.SpinStop2; //EVT_SPIN_FAIL
				case 2: return MessageIdentifier.MixerClockwiseSlow;
				case 1: return MessageIdentifier.MixerCounterClockwiseSlow;
				//case 0:
				default: return MessageIdentifier.MixerStopped;
			}
		}

		public static MessageIdentifier TolvaCode(byte data)
		{
			return StringHelper.AreBitsSet(data, 0x2) ? MessageIdentifier.TolvaDeactivated : MessageIdentifier.TolvaActivated;
		}
	}
}