#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Types.ValueObject.Messages
{
    /// <summary>
    /// Class that represents a cacheable
    /// </summary>
    [Serializable]
    public class MensajeVO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new message vo associated to the specified message.
        /// </summary>
        /// <param name="msj"></param>
        public MensajeVO (Mensaje msj)
        {
            Id = msj.Id;
            Descripcion = msj.Descripcion;
            Destino = msj.Destino;
            Origen = msj.Origen;
            Acceso = msj.Acceso;
            Codigo = msj.Codigo;
            TTL = msj.Ttl;
            IsParent = msj.Empresa == null && msj.Linea == null;
            IsGeneric = msj.TipoMensaje.EsGenerico;
            EsAlarma = msj.EsAlarma;
            Texto = msj.Texto;
            Empresa = msj.Empresa != null ? msj.Empresa.Id : msj.Linea != null ? msj.Linea.Empresa != null ? msj.Linea.Empresa.Id : (Int32?)null : null;
            Linea = msj.Linea != null ? msj.Linea.Id : (Int32?)null;
        }

        #endregion

        #region Public Properties

        public string Descripcion { get; set; }

        public short Destino { get; set; }

        public short Acceso { get; set; }

        public byte Origen { get; set; }

        public string Codigo { get; set; }

        public short TTL { get; set; }

        public string TipoDescripcion { get; set; }

        public int Id { get; set; }

        public bool IsParent { get; set; }

        public bool IsGeneric { get; set; }

        public bool EsAlarma { get; set; }

        public String Texto { get; set; }

        public Int32? Empresa { get; set; }

        public Int32? Linea { get; set; }

        #endregion
    }
}