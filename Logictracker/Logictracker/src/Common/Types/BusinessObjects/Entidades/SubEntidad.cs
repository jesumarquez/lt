using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class SubEntidad : IAuditable, ISecurable, IHasSensor, IHasEntidadPadre
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool Baja { get; set; }
        public virtual double X { get; set; }
        public virtual double Y { get; set; }
        public virtual bool ControlaMaximo { get; set; }
        public virtual bool ControlaMinimo { get; set; }
        public virtual double Maximo { get; set; }
        public virtual double Minimo { get; set; }

        public virtual EntidadPadre Entidad { get; set; }

        public override string ToString() { return Descripcion; }

        // Cuando se cambia el sensor 
        // se guarda el valor viejo para poder actualizar la cache
        public virtual Sensor OldSensor { get; set; }

        private Sensor _sensor;
        public virtual Sensor Sensor
        {
            get { return _sensor; }
            set
            {
                if (OldSensor == null)
                {
                    OldSensor = _sensor;
                }
                _sensor = value;
            }
        }
    }
}