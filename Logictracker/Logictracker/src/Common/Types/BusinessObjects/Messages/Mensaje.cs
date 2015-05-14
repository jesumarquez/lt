#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class Mensaje : IAuditable, ISecurable, IDisposable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual short Destino { get; set; }
        public virtual short Acceso { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Texto { get; set; }
        public virtual byte Origen { get; set; }
        public virtual short Ttl { get; set; }
        public virtual TipoMensaje TipoMensaje { get; set; }
        public virtual bool EsAlarma { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual bool EsBaja { get; set; }
        public virtual Icono Icono { get; set; }
        public virtual int Revision { get; set; }
        public virtual bool EsSoloDeRespuesta { get; set; }

        private ISet _respuestas;

        /// <summary>
        /// A list of possible responses to the message.
        /// </summary>
        public virtual ISet Respuestas { get { return _respuestas ?? (_respuestas = new ListSet()); } }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as Mensaje;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        public void Dispose()
        {
            TipoMensaje = null;
            Empresa = null;
            Linea = null;
            Icono = null;
            _respuestas = null;            
        }

        #endregion
    }
}