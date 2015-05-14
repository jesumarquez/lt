using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class ProgramacionReporte : IAuditable, IHasEmpresa, IHasLinea
    {
        #region IAuditable Members

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Parametros { get; set; }
        public virtual string Reporte { get; set; }
        public virtual char Periodicidad { get; set; }
        public virtual string Mail { get; set; }
        public virtual bool Baja { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string ParametrosCsv { get; set; }
    }
}
