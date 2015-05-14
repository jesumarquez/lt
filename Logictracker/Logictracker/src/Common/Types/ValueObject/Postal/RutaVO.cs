#region Usings

using System;

#endregion

namespace Logictracker.Types.ValueObject.Postal
{
    /// <summary>
    /// Represents a sqlite route.
    /// </summary>
    [Serializable]
    public class RutaVO
    {
        #region Public Properties

        public Int32 Id { get; set; }
        public String NumeroDeRuta { get; set; }
        public Int16 NumeroDeItem { get; set; }
        public String CodigoDistribuidor { get; set; }
        public Int32 Distribuidor { get; set; }
        public String CodigoTipoServicio { get; set; }
        public Int32 TipoServicio { get; set; }
        public Int32 Cliente { get; set; }
        public String Destinatario { get; set; }
        public String Direccion { get; set; }
        public Int32 IdPieza { get; set; }
        public String Pieza { get; set; }
        public Int32 Estado { get; set; }
        public Int32? Motivo { get; set; }
        public Double? Latitud { get; set; }
        public Double? Longitud { get; set; }
        public Byte[] Foto { get; set; }
        public DateTime? FechaFoto { get; set; }
        public DateTime? FechaMotivo { get; set; }
        public String Lateral2 { get; set; }
        public String Lateral1 { get; set; }
        public String Referencia { get; set; }
        public Int32? Dispositivo { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime FechaModificacion { get; set; }

        #endregion
    }
}