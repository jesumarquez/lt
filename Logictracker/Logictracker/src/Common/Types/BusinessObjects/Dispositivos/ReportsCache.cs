using System;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    [Serializable]
	public class ReportsCache : IDataIdentify, IAuditable
    {
        public virtual int Id { get; set; }
    	public virtual Type TypeOf() { return GetType(); }

		public virtual DateTime DateTime { get; set; }

		public virtual int Dispositivo { get; set; }

		/// <summary>
		/// Hash del objeto IMessage DeviceId:DateTime(yyyy/MM/dd HH:mm:ss) [codigo codigoextra codigoextra2]:NombreCompletoTipo:NombreTipo
		/// </summary>
        public virtual string Value { get; set; } 
    }
}