#region Usings

using System;

#endregion

namespace Logictracker.Types.ValueObjects.Documentos.Partes
{
    [Serializable]
    public class TurnoPartePersonal
    {
        public DateTime AlPozoSalida { get; set; }

        public DateTime AlPozoLlegada { get; set; }

        public DateTime DelPozoSalida { get; set; }

        public DateTime DelPozoLlegada { get; set; }

        public int OdometroInicial { get; set; }

        public int OdometroFinal { get; set; }

        public int Km { get; set; }

        public string Responsable { get; set; }

        public DateTime AlPozoSalidaControl { get; set; }

        public DateTime AlPozoLlegadaControl { get; set; }

        public DateTime DelPozoSalidaControl { get; set; }

        public DateTime DelPozoLlegadaControl { get; set; }

        public int KmControl { get; set; }

        public int KmGps { get; set; }
    }
}