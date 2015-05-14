namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects
{
    public interface ITipoDocumentoAutoBindeable : IAutoBindeable
    {
        bool OnlyForVehicles { get; set; }
        bool OnlyForEmployees { get; set; }
        bool OnlyForEquipment { get; set; }
        bool OnlyForTransporter { get; set; }
    }
}
