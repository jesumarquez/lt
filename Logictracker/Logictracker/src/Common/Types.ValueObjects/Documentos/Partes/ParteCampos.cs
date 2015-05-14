using System.Collections.Generic;

namespace Logictracker.Types.ValueObjects.Documentos.Partes
{
    public static class ParteCampos
    {
        public const string Empresa = "Empresa";
        public const string Equipo = "Equipo";
        public const string EstadoControl = "Estado Control";
        public const string Kilometraje = "Kilometraje";
        public const string KilometrajeControl = "Kilometraje V";
        public const string OdometroInicial = "Odometro Inicial";
        public const string OdometroFinal = "Odometro Final";

        public const string KilometrajeGps = "Kilometraje GPS";
        public const string KilometrajeTotal = "Kilometraje Total";
        public const string LlegadaAlPozo = "Hs Llegada Al Pozo";
        public const string LlegadaAlPozoControl = "Hs Llegada Al Pozo V";
        public const string LlegadaDelPozo = "Hs Llegada Del Pozo";
        public const string LlegadaDelPozoControl = "Hs Llegada Del Pozo V";
        public const string LugarLlegada = "Lugar Llegada";
        public const string LugarPartida = "Lugar Partida";
        public const string Observaciones = "Observaciones";
        public const string Responsable = "Responsable";
        public const string SalidaAlPozo = "Hs Salida Al Pozo";

        public const string SalidaAlPozoControl = "Hs Salida Al Pozo V";
        public const string SalidaDelPozo = "Hs Salida Del Pozo";
        public const string SalidaDelPozoControl = "Hs Salida Del Pozo V";
        public const string UsuarioControl = "Usuario V";
        public const string UsuarioVerificacion = "Usuario F";
        public const string Vehiculo = "Vehiculo";

        public const string TipoServicio = "Tipo Servicio";
        public const string CentroCostos = "Centro Costos";


        public static List<string> ListaTipoServicios = new List<string>
                                                {
                                                    "Equipos",
                                                    "Adicionales",
                                                    "Transporte de Agua",
                                                    "Relevo Personal Mantenimiento",
                                                    "Relevo Jefe de Equipo"
                                                };
        
    }
}