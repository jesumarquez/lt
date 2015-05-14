#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;

#endregion

namespace Logictracker.Types.ValueObject.Dispositivos
{
    [Serializable]
    public class DispositivoVo
    {
        public DispositivoVo(Dispositivo disp, Coche vehiculo)
        {
            if (disp == null) return;

            Id = disp.Id;
            Codigo = disp.Codigo ?? string.Empty;
            Imei = disp.Imei ?? string.Empty;
            //FullFirmwareVersion = disp.FullFirmwareVersion ?? string.Empty;
            //Qtree = disp.QtreeRevision;
            Tipo = disp.TipoDispositivo != null ? string.Concat(disp.TipoDispositivo.Fabricante, " - ", disp.TipoDispositivo.Modelo) : string.Empty;
            Telefono = disp.Telefono;
            Estado = disp.Estado;
            Ubicacion = "-";

            if (vehiculo == null) return;

            var linea = vehiculo.Linea != null ? " - " + vehiculo.Linea.Descripcion : "";
            var empresa = vehiculo.Empresa != null ? vehiculo.Empresa.RazonSocial : vehiculo.Linea != null ? vehiculo.Linea.Empresa.RazonSocial : "Todos";
            Ubicacion = empresa + linea;

            Vehiculo = vehiculo.TipoCoche != null ? string.Concat(vehiculo.Interno, " - ", vehiculo.TipoCoche.Descripcion) : vehiculo.Interno;
        }

        public string Vehiculo { get; set; }
        public string Imei { get; set; }
        public string Codigo { get; set; }
        public int Id { get; set; }
        //public string FullFirmwareVersion { get; set; }
        public string Tipo { get; set; }
        public string Telefono { get; set; }
        //public string Qtree { get; set; }
        public int Estado { get; set; }
        public string Ubicacion { get; set; }
    }
}