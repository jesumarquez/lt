using System.Collections.Generic;

namespace Logictracker.Scheduler.Tasks.Logiclink2
{
    public static class Properties
    {
        public class DistribucionFemsa
        {
            public const int Centro = 0;
            public const int Ruta = 1;
            public const int Entrega = 2;
            public const int CodigoCliente = 3;
            public const int DescripcionCliente = 4;
            public const int Latitud = 5;
            public const int Longitud = 6;
            public const int Fecha = 7;
            public const int Hora = 8;
            public const int Cajas = 9;
            public const int Patente = 10;
            public const int Legajo = 11;
        }
        public class DistribucionQuilmes
        {
            public const int Ruta = 0;
            public const int Viaje = 1;
            public const int Patente = 2;
            public const int Fecha = 3;
            public const int Orden = 4;
            public const int Campo2 = 5;
            public const int Hora = 6;
            public const int CodigoCliente = 7;
            public const int DescripcionCliente = 8;
            public const int Latitud = 9;
            public const int Longitud = 10;
            public const int Kms = 11;
        }
        public class DistribucionSos
        {
            public const int Fecha = 0;
            public const int Base = 1;
            public const int Entrega = 2;
            public const int Patente = 3;
            public const int LatitudOrigen = 4;
            public const int LongitudOrigen = 5;
            public const int HoraProgramada = 6;
            public const int LatitudDestino = 7;
            public const int LongitudDestino = 8;
        }
        public class DistribucionMusimundo
        {
            public const int Factura = 0;
            public const int Patente = 1;
            public const int TM = 2;
            public const int Prioridad = 3;
            public const int Fecha = 4;
            public const int Km = 5;
            public const int Coord2 = 6;
            public const int Coord1 = 7;
            public const int Cliente = 8;
            public const int Rango = 9;
            public const int Importe = 10;
            public const int Direccion = 11;
        }
        public class DistribucionCCU
        {
            public const int CodigoCliente = 0;
            public const int Planilla = 1;
            public const int Fecha = 2;
            public const int Factura = 3;
            public const int Hectolitros = 4;
            public const int ImporteTotal = 5;
        }
        public class ClienteCCU
        {
            public const int CodigoCliente = 0;
            public const int Nombre = 1;
            public const int Direccion = 2;
            public const int Provincia = 3;
            public const int Localidad = 4;
            public const int Territorio = 5;
            public const int Ramo = 6;
            public const int Latitud = 7;
            public const int Longitud = 8;
        }
        public class AsignacionCCU
        {
            public const int Planilla = 0;
            public const int Patente = 1;
        }

        public class DistribucionReginaldLee
        {
            public const int CodigoCliente = 0;
            public const int CodigoPedido = 1;
            public const int Packs = 2;
            public const int Pallets = 3;
            public const int Kilos = 4;
            public const int RutaOrigen = 5;
            public const int CodigoRuta = 6;
            public const int Orden = 7;
            public const int Interno = 8;
            public const int Latitud = 9;
            public const int OrientacionNorthSouth = 10;
            public const int Longitud = 11;
            public const int OrientacionEastWest = 12;
            public const int Fecha = 13;
            public const int DescripcionCliente = 14;

            public static Dictionary<int, int> Anchos
            {
                get
                {
                    return new Dictionary<int, int>
                    {
                        {CodigoCliente, 15},
                        {CodigoPedido, 15},
                        {Packs, 10},
                        {Pallets, 10},
                        {Kilos, 10},
                        {RutaOrigen, 4},
                        {CodigoRuta, 21},
                        {Orden, 4},
                        {Interno, 3},
                        {Latitud, 11},
                        {OrientacionNorthSouth, 1},
                        {Longitud, 9},
                        {OrientacionEastWest, 1},
                        {Fecha, 16},
                        {DescripcionCliente, 30}
                    };
                }
            }
        }

        public class PedidosPetrobras
        {
            public const int Cero = 0;
            public const int CodigoCliente = 1;
            public const int CodigoPlanta = 2;
            public const int NombreCliente = 3;
            public const int NombrePlanta = 4;
            public const int CodigoTransportista = 5;
            public const int DescripcionTransportista = 6;
            public const int FechaSolicitud = 7;
            public const int FechaLiberacion = 8;
            public const int CodigoEntrega = 9;
            public const int CodigoPedido = 10;
            public const int Condicion = 11;
            public const int PL = 12;
            public const int LugarConsumo = 13;
            public const int NaftaSuper = 14;
            public const int NaftaPremium = 15;
            public const int GasoilMenor500 = 16;
            public const int GasoilPremium = 17;
            public const int GasoilMayor500 = 18;
            public const int Observaciones = 19;

        }
    }
}
