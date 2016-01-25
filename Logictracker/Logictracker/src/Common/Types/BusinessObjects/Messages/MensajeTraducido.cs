using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class MensajeTraducido : IAuditable, IHasEmpresa
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual Empresa Empresa { get; set; }
        public virtual string CodigoOriginal { get; set; }
        public virtual string CodigoFinal { get; set; }
    }
}