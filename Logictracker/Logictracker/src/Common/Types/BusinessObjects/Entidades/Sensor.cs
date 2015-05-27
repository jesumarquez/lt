using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class Sensor : IAuditable, IHasTipoMedicion, IHasDispositivo
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }

        public virtual TipoMedicion TipoMedicion { get; set; }

        public override string ToString() { return Descripcion; }

        // Guardo el codigo viejo si se está modificando para poder actualizar la cache
        public virtual string OldCodigo { get; set; }

        private string _codigo;
        public virtual string Codigo
        {
            get { return _codigo; }
            set
            {
                if (OldCodigo == null)
                {
                    OldCodigo = _codigo;
                }
                _codigo = value;
            }
        }

        // Cuando se cambia el dispositivo 
        // se guarda el valor viejo para poder actualizar la cache
        public virtual Dispositivo OldDispositivo { get;  set; }

        private Dispositivo _dispositivo;
        public virtual Dispositivo Dispositivo
        {
            get { return _dispositivo; }
            set
            {
                if (OldDispositivo == null)
                {
                    OldDispositivo = _dispositivo;
                }
                _dispositivo = value;
            }
        }
    }
}