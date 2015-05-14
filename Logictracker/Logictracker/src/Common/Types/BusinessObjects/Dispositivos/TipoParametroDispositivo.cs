#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    [Serializable]
    public class TipoParametroDispositivo : IAuditable
    {
        #region Private Properties

        private ISet _detallesDispositivo;

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Nombre { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string TipoDato { get; set; }
        public virtual string Consumidor { get; set; }
        public virtual string ValorInicial { get; set; }
        public virtual bool Editable { get; set; }
        public virtual string Metadata { get; set; }
        public virtual bool RequiereReset { get; set; }
        
        public virtual TipoDispositivo DispositivoTipo { get; set; }

        public virtual ISet DispositivoDetalle { get { return _detallesDispositivo ?? (_detallesDispositivo = new ListSet()); } }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as TipoParametroDispositivo;

            return castObj != null && castObj.Nombre.Equals(Nombre) && castObj.DispositivoTipo.Equals(DispositivoTipo);
        }

        public override int GetHashCode()
        {
            return Nombre.GetHashCode() + DispositivoTipo.GetHashCode();
        }

        #endregion
    }
}
