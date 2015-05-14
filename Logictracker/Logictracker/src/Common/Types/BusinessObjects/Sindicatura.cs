using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Sindicatura : IAuditable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf()
        {
            throw new NotImplementedException();
        }

        public virtual string PlanificacionGlobal { get; set; }
        public virtual string Objeto { get; set; }
        public virtual string Dependencias { get; set; }
        public virtual string TareasFiscalizacion { get; set; }
    }
}
