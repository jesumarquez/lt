using System;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class ParametroEmpresa
    {
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Valor { get; set; }
    }
}
