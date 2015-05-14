#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Postal
{
    /// <summary>
    /// Class thar represent an item within a distributors route.
    /// </summary>
    [Serializable]
    public class Ruta : IAuditable
    {
        #region Public Properties

        public virtual Int32 Id { get; set; }
        public virtual String NumeroRuta { get; set; }
        public virtual Int32 Item { get; set; }
        public virtual String CodigoDistribuidor { get; set; }
        public virtual Distribuidor Distribuidor { get; set; }
        public virtual String CodigoTipoServicio { get; set; }
        public virtual TipoServicio TipoServicio { get; set; }
        public virtual Int32 Cliente { get; set; }
        public virtual String Destinatario { get; set; }
        public virtual String Direccion { get; set; }
        public virtual Int32 IdPieza { get; set; }
        public virtual String Pieza { get; set; }
        public virtual Int32 Estado { get; set; }
        public virtual Motivo Motivo { get; set; }
        public virtual Double? Latitud { get; set; }
        public virtual Double? Longitud { get; set; }
        public virtual Byte[] Foto { get; set; }
        public virtual DateTime? FechaFoto { get; set; }
        public virtual DateTime? FechaMotivo { get; set; }
        public virtual String Lateral1 { get; set; }
        public virtual String Lateral2 { get; set; }
        public virtual String Referencia { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
        public virtual DateTime? FechaModificacion { get; set; }

        #endregion

        #region Public Methods

        public virtual Type TypeOf() { return GetType(); }

        #endregion
    }
}