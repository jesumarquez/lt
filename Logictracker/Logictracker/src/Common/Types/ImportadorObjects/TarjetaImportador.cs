#region Usings

using System;

#endregion

namespace Logictracker.Types.ImportadorObjects
{
    [Serializable]
    public class TarjetaImportador
    {
        public string Numero { get; set; }
        public string Pin { get; set; }
        public string Legajo { get; set; }
    }

}
