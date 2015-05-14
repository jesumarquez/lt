#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{   
    [Serializable]
    public class MobileMessage
    {
        private string _hora;

        public int Id { get; set; }

        public int MovilId { get; set; }

        public string Patente { get; set; }

        public string Interno { get; set; }

        public DateTime FechaYHora { get; set; }

        public string Mensaje { get; set; }

        public int? Velocidad { get; set; }

        public string Chofer { get; set; }

        public string Responsable { get; set; }

        public string Hora
        {
            get { return (_hora)?? string.Empty; }
            set { _hora = value; }
        }

        public int Indice { get; set; }
    }
}

