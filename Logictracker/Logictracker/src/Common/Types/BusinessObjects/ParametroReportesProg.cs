namespace Logictracker.Types.BusinessObjects
{
    public class ParametroReportesProg
    {
        public virtual int Id { get; set; }
        public virtual ProgramacionReporte ProgramacionReporte{ get; set; }
        public virtual ParameterType ParameterType{ get; set; }
        public virtual int EntityId { get; set; }
    }
}