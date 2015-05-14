#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

#endregion

namespace Logictracker.Types.ValueObject.Vehiculos
{
    [Serializable]
    public class CocheVo
    {
        public CocheVo(Coche coche)
        {
            Id = coche.Id;
            Interno = coche.Interno;
            Transportista = coche.Transportista != null ? coche.Transportista.Descripcion : String.Empty;
            Dispositivo = coche.Dispositivo != null ? coche.Dispositivo.Codigo : String.Empty;
            Estado = coche.Estado;
            Tipo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : String.Empty;
            Icono = coche.TipoCoche != null ? coche.TipoCoche.IconoDefault.PathIcono : null;
            Patente = coche.Patente;
            CentroDeCostos = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : String.Empty;
            Responsable = coche.Chofer != null ? coche.Chofer.Entidad.Descripcion : String.Empty;
            Referencia = coche.Referencia;
        }

        public string Interno { get; set; }
        public string Patente { get; set; }
        public string Transportista { get; set; }
        public string Dispositivo { get; set; }
        public int Estado { get; set; }
        public string Tipo { get; set; }
        public int Id { get; set; }
        public string Icono { get; set; }
        public string CentroDeCostos { get; set; }
        public string Responsable { get; set; }
        public string Referencia { get; set; }
    }
}