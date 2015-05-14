namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    public interface ICicloLogisticoAutoBindeable : IAutoBindeable
    {
        bool AddEstados { get; set; }
        bool AddCiclos { get; set; }
    }
}
