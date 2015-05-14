#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Documentos.Partes;

#endregion

namespace Logictracker.Web.Documentos.ParteComparativo.Entities
{
    public class ParteComparativoMovil
    {
        private Equipo equipo;

        public string TipoServicio { get; set; }
        public int TipoServicioId { get; set; }

        public int Id
        {
            get { return Equipo.Id; }
        }
        public Equipo Equipo
        {
            get { return equipo; }
            set { equipo = value; }
        }

        public string Descripcion
        {
            get { return TipoServicio == ParteCampos.ListaTipoServicios[0] ? equipo.Descripcion : TipoServicio; }
        }

        public TimeSpan Horas { get; set; }

        public int Kilometraje { get; set; }

        public string TiempoTrabajado
        {
            get
            {
                return string.Format("{0} dias {1} hs {2} min",
                                     (int)Math.Floor(Horas.TotalDays), Horas.Hours, Horas.Minutes);

            }
        }

        public int Partes { get; set; }
    }
}