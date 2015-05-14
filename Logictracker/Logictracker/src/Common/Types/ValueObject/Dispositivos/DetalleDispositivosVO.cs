#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

#endregion

namespace Logictracker.Types.ValueObject.Dispositivos
{
    [Serializable]
    public class DetalleDispositivosVO
    {
        #region Public Properties

        /// <summary>
        /// Constructor for the VO
        /// </summary>
        /// <param name="detalle"></param>
        public DetalleDispositivosVO(DetalleDispositivo detalle)
        {
            Imei = (detalle.Dispositivo != null) ? detalle.Dispositivo.Imei : String.Empty;
            CodigoDispositivo = (detalle.Dispositivo != null) ? detalle.Dispositivo.Codigo : String.Empty;
            Firm = detalle.Dispositivo != null ? detalle.Dispositivo.Firmware != null ? detalle.Dispositivo.Firmware.Nombre : String.Empty : String.Empty;
            Consumidor = (detalle.TipoParametro != null) ? detalle.TipoParametro.Consumidor : String.Empty;
            TipoParam = (detalle.TipoParametro != null) ? detalle.TipoParametro.TipoDato : String.Empty;
            Descripcion = (detalle.TipoParametro != null) ? detalle.TipoParametro.Descripcion : String.Empty;
            Param = (detalle.TipoParametro != null) ? detalle.TipoParametro.Nombre: String.Empty;
            Valor = detalle.Valor;
            Revision = detalle.Revision;
            ValorDefault = (detalle.TipoParametro != null) ? detalle.TipoParametro.ValorInicial : String.Empty;
            IdDispositivo = (detalle.Dispositivo != null) ? detalle.Dispositivo.Id : -1;
            IDDetail = detalle.Id;
            ParamEditable = (detalle.TipoParametro != null) && detalle.TipoParametro.Editable;
        }

        public string Imei { get; set; }
        public string CodigoDispositivo { get; set; }
        public string Telefono { get; set; }
        public string Firm { get; set; }
        public string Consumidor { get; set; }
        public string TipoParam { get; set; }
        public string Param { get; set; }
        public string Descripcion { get; set; }
        public string Valor { get; set; }
        public int Revision { get; set; }
        public string ValorDefault { get; set; }
        public int IdDispositivo { get; set; }
        public int IDDetail{ get; set; }
        public bool ParamEditable { get; set; }

        #endregion
    }
}