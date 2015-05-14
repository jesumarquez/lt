#region Usings

using System;

#endregion

namespace Logictracker.Types.BusinessObjects.Components
{
    [Serializable]
    public class Owner
    {
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
    }
}