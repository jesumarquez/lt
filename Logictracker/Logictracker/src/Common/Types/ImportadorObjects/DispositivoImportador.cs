#region Usings

using System;

#endregion

namespace Logictracker.Types.ImportadorObjects
{
    [Serializable]
    public class DispositivoImportador
    {
        public string Codigo { get; set; }
        public short PollInterval { get; set; }
        public string IpAdress { get; set; }
        public int Port { get; set; }
        public string Imei { get; set; }
        public string Clave { get; set; }
        public string Telefono { get; set; }
        public string Interno { get; set; }
    }
}
