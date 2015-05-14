using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class LogProgramacionReporte : IAuditable
    {
        #region IAuditable Members

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime Inicio { get; set; }
        public virtual DateTime Fin { get; set; }
        public virtual int Filas { get; set; }
        public virtual bool Error { get; set; }
        public virtual ProgramacionReporte ProgramacionReporte { get; set; }
    }
}
