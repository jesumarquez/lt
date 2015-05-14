using System;
using Urbetrack.Cache.Interfaces;
using Urbetrack.Types.InterfacesAndBaseClasses;

namespace Urbetrack.Types.BusinessObjects.Dispositivos
{
	[Serializable]
	public class TemperatureSensorEntropy : IDataIdentify, IAuditable
	{
		public virtual int Id { get; set; }
		public virtual Type TypeOf() { return GetType(); }

		//public virtual int Vehiculo { get; set; }
		public virtual String Serial { get; set; }
		public virtual DateTime DateTime { get; set; }
		public virtual float Temperature { get; set; }
		public virtual bool Connected { get; set; }
		public virtual bool ButtonPressed { get; set; }
	}
}