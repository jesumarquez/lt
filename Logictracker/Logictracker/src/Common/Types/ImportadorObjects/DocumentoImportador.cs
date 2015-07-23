#region Usings

using System;

#endregion

namespace Logictracker.Types.ImportadorObjects
{
    [Serializable]
    public class DocumentoImportador
    {
        public string CodigoDocumento { get; set; }
        public string DescripcionDocumento{ get; set; }
        public DateTime? VencimientoDocumento { get; set; }
        public DateTime? PresentacionDocumento { get; set; }
        public string PatenteVehiculo { get; set; }

    }
}
