#region Usings

using System;

#endregion

namespace Logictracker.Types.ImportadorObjects
{
    [Serializable]
    public class EmpleadoImportador
    {
        public string Mail { get; set; }
        public string Legajo { get; set; }
        public string NombreApellido { get; set; }
        public string NroDoc { get; set; }
        public string Patente { get; set; }
        public string Transportista { get; set; }
        public string Categoria { get; set; }

    }
}
