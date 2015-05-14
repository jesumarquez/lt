using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Organizacion
{
    [Serializable]
    public class Asegurable : IAuditable
    {
        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }

        public virtual string Referencia { get; set; }
    }
}
