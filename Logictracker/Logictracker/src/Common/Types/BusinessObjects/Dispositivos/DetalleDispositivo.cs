#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    [Serializable]
    public class DetalleDispositivo : IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Valor { get; set; }
        public virtual int Revision{ get; set; }
        public virtual TipoParametroDispositivo TipoParametro { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as DetalleDispositivo;

            return castObj != null && castObj.TipoParametro.Equals(TipoParametro) && Dispositivo.Equals(castObj.Dispositivo);
        }

        public override int GetHashCode() { return TipoParametro != null ? TipoParametro.GetHashCode() : 0; }

        #endregion
    }

	public static class DetalleDispositivoX
	{
		public static Boolean AsBoolean(this DetalleDispositivo obj, Boolean defaultValue)
		{
			return obj != null ? Convert.ToBoolean(obj.Valor) : defaultValue;
		}

		public static T As<T>(this DetalleDispositivo obj, T defaultValue)
		{
			if (obj == null) return defaultValue;

			if (typeof(T) == typeof(String)) return (T)(Object)obj.Valor;

			return (T) Convert.ChangeType(obj.Valor, typeof (T));
		}

	}
}
