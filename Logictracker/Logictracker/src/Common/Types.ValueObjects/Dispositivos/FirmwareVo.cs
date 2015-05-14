using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class FirmwareVo
    {
        public const int IndexNombre = 0;
        public const int IndexDescripcion = 1;
        public const int IndexFirma = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexFirma, ResourceName = "Labels", VariableName = "FIRMA", AllowGroup = false, IncludeInSearch = true)]
        public string Firma { get; set;}

        public FirmwareVo(Firmware firm)
        {
            Id = firm.Id;
            Nombre = firm.Nombre;
            Descripcion = firm.Descripcion;
            Firma = firm.Firma;
        }
    }
}
