using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class ColaMensajes : IAuditable
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual DateTime UltimaActualizacion { get; set; }
    }
}