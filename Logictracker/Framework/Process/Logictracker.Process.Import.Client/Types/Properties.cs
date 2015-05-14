using System;

namespace Logictracker.Process.Import.Client.Types
{
    public static class Properties
    {
        public class Cliente
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Comentario1 = 2;
            public const int Comentario2 = 3;
            public const int Comentario3 = 4;
            public const int Descripcion = 5;
            public const int DescripcionCorta = 6;
            public const int Telefono = 7;
            public const int Linea = 8;
            public const int ReferenciaGeografica = 9;
        }
        public class ReferenciaGeografica
        {
            public const int None = 0;
            public  const  int Codigo = 1;
            public  const  int Descripcion = 2;
            public  const  int Linea = 3;
            public  const  int VigenciaDesde = 4;
            public  const  int VigenciaHasta = 5;
            public  const  int Color = 6;

            public const int Latitud = 100;
            public const int Longitud = 101;

            public const int Direccion = 102;

            public const int Pais = 103;
            public const int Provincia = 104;
            public const int Partido = 105;
            public const int Localidad = 106;
            public const int Calle = 107;
            public const int Esquina = 108;
            public const int EntreCalle = 109;
            public const int Altura = 110;

            public const int Radio = 111;
            public const int TipoReferenciaGeografica = 112;
        }
        public class PuntoEntrega
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Cliente = 3;
            public const int Telefono = 4;
            public const int Comentario1 = 5;
            public const int Comentario2 = 6;
            public const int Comentario3 = 7;
            public const int ReferenciaGeografica = 8;
            public const int Linea = 9;
        }
        public class Ticket
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Linea = 2;
            public const int Pedido = 3;
            public const int Cliente = 4;
            public const int PuntoEntrega = 5;
            public const int Vehiculo = 6;
            public const int Chofer = 7;
            public const int FechaInicio = 8;
            public const int FechaEnObra = 9;
            public const int OrdenDiario = 10;

            public const int Producto = 11;
            public const int CantidadPedido = 12;
            public const int CantidadCarga = 13;
            public const int CantidadAcumulada = 14;
            public const int Unidad = 15;

            public const int Comentario1 = 16;
            public const int Comentario2 = 17;
            public const int Comentario3 = 18;

            public const int Gmt = 19;

            public const int Iniciar = 20;

            public const int LineaDestino = 21;
        }
        public class Proveedor
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Linea = 3;
            public const int TipoProveedor = 4;
        }
        public class TipoProveedor
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Linea = 3;
        }
        public class TipoInsumo
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Linea = 3;
            public const int DeCombustible = 4;
        }
        public class UnidadMedida
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Simbolo = 3;
        }
        public class Insumo
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Linea = 3;
            public const int TipoInsumo = 4;
            public const int UnidadMedida = 5;
        }
        public class Consumo
        {
            public const int None = 0;
            public const int NroFactura = 1;
            public const int Linea = 2;
            public const int Vehiculo = 3;
            public const int Empleado = 4;
            public const int Fecha = 5;
            public const int Km = 6;
            public const int Proveedor = 7;
            public const int Gmt = 8;

            public const int Insumo = 50;
            public const int Cantidad = 51;
            public const int ImporteUnitario = 52;
        }

        public class Distribucion
        {
            public const int None = 0;
            public const int Linea = 1;
            public const int Vehiculo = 2;
            public const int Empleado = 3;
            public const int Codigo = 4;
            public const int Fecha = 5;
            public const int TipoCiclo = 6;
            public const int RegresaABase = 7;
            public const int Orden = 8;
            public const int Cliente = 9;
            public const int PuntoEntrega = 10;
            public const int Programado = 11;
            public const int TipoServicio = 12;
            public const int Gmt = 13;
            public const int ModificaCabecera = 14;
        }
        public class Empleado
        {
            public const int None = 0;
            public const int Linea = 2;
            public const int Departamento = 3;
            public const int Transportista = 4;
            public const int CentroDeCosto = 5;
            public const int TipoEmpleado = 6;
            public const int Reporta1 = 7;
            public const int Reporta2 = 8;
            public const int Reporta3 = 9;
            public const int Categoria = 10;
            public const int Tarjeta = 11;
            public const int Legajo = 12;
            public const int Antiguedad = 13;
            public const int Art = 14;
            public const int Licencia = 15;
            public const int Mail = 16;
            public const int EsResponsable = 17;
            public const int Nombre = 18;
            public const int Apellido = 19;
            public const int TipoDocumento = 20;
            public const int NroDocumento = 21;
            public const int Cuil = 22;
        }
        public class Documento
        {
            public const int None = 0;
            public const int Linea = 1;
            public const int Codigo = 2;
            public const int Descripcion = 3;
            public const int Fecha = 4;
            public const int FechaPresentacion = 5;
            public const int FechaVencimiento = 6;
            public const int FechaCierre = 7;
            public const int Vehiculo = 8;
            public const int Empleado = 9;
            public const int Transportista = 10;
            public const int Equipo = 11;
            public const int TipoDocumento = 12;
            public const int Gmt = 13;
        }
        public class Distrito
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Gmt = 3;
        }
        public class Base
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int Gmt = 3;
            public const int Telefono = 4;
            public const int Mail = 5;
            public const int ReferenciaGeografica = 6;
        }
        public class Dispositivo
        {
            public const int None = 0;
            public const int Base = 1;
            public const int TipoDispositivo = 2;
            public const int Codigo = 3;
            public const int Imei = 4;
            public const int Estado = 5;
            public const int LineaTelefonica = 6;
        }
        public class Vehiculo
        {
            public const int None = 0;
            public const int Base = 1;
            public const int TipoVehiculo = 2;
            public const int Interno = 3;
            public const int Patente = 4;
            public const int CentroCostos = 5;
            public const int Transportista = 6;
            public const int Referencia = 7;
            public const int Responsable = 8;
            public const int Dispositivo = 9;
            public const int Estado = 10;
            public const int Marca = 11;
            public const int Modelo = 12;
            public const int Ano = 13;
            public const int Chasis = 14;
            public const int Motor = 15;
            public const int Poliza = 16;
            public const int VencimientoPoliza = 17;
        }
        public class LineaTelefonica
        {
            public const int None = 0;
            public const int Numero = 1;
            public const int Sim = 2;
        }
        public class TipoVehiculo
        {
            public const int None = 0;
            public const int Codigo = 1;
            public const int Descripcion = 2;
            public const int VelocidadAlcanzable = 3;
            public const int KmDiarios = 4;
            public const int VelocidadPromedio = 5;
            public const int Capacidad = 6;
            public const int DesvioCombustibleMin = 7;
            public const int DesvioCombustibleMax = 8;
            public const int ControlaKilometraje = 9;
            public const int ControlaTurno = 10;
            public const int ControlaConsumos = 11;
            public const int SeguimientoPersonas = 12;
            public const int NoVehiculo = 13;
            public const int Base = 14;
        }

        public class Marca
        {
            public const int None = 0;
            public const int Nombre = 1;
            public const int Base = 2;
        }
        public class Modelo
        {
            public const int None = 0;
            public const int Marca = 1;
            public const int Codigo = 2;
            public const int Rendimiento = 3;
            public const int Capacidad = 4;
            public const int Costo = 5;
            public const int VidaUtil = 6;
            public const int Descripcion = 7;
        }

        public class Distribution
        {
            public const int None = 0;
            public const int Coche = 1;
            public const int Codigo = 2;
            public const int Fecha = 3;
            public const int Horario = 4;
            public const int Latitud = 5;
            public const int Longitud = 6;
            public const int NroViaje = 7;
            public const int Km = 8;
            public const int PuntoEntrega = 9;
            public const int Nombre = 10;
            public const int Orden = 11;
        }
        public class DistribucionF
        {
            public const int Centro = 1;
            public const int Ruta = 2;
            public const int Entrega = 3;
            public const int CodigoCliente = 4;
            public const int DescripcionCliente = 5;
            public const int Latitud = 6;
            public const int Longitud = 7;
            public const int Fecha = 8;
            public const int Hora = 9;
            public const int Cajas = 10;
            public const int Patente = 11;
            public const int Legajo = 12;
        }
        public class DistribucionB
        {
            public const int CodigoRuta = 1;
            public const int Orden = 2;
            public const int CodigoEntrega = 3;
            public const int Fecha = 4;
            public const int InicioBanda1 = 5;
            public const int FinBanda1 = 6;
            public const int InicioBanda2 = 7;
            public const int FinBanda2 = 8;
            public const int HoraProgramada = 9;
            public const int Interno = 10;
            public const int CodigoPuntoEntrega = 11;
            public const int Estado = 12;
        }
        public class DistribucionE
        {
            public const int Tipo = 1;
            public const int NroDocumento = 2;
            public const int FechaGeneracion = 3;
            public const int FechaDespacho = 4;
            public const int Guardia = 5;
            public const int Latitud = 6;
            public const int Longitud = 7;
        }
        public class PuntoEntregaB
        {
            public const int CodigoCliente = 1;
            public const int NombreCliente = 2;
            public const int CodigoPuntoEntrega = 3;
            public const int NombrePuntoEntrega = 4;
            public const int Estado = 5;
            public const int Tipo = 6;
            public const int Latitud = 7;
            public const int Longitud = 8;
        }

        public static int GetAs(string prop, Type type)
        {
            try
            {
                var fieldInfo = type.GetField(prop);
                return (int) fieldInfo.GetValue(null);
            }
            catch
            {
                return 0;
                //var props = type.GetFields().Select(p => p.Name).ToArray();
                //var values = string.Join("|", props);
                //throw new ApplicationException("Error obteniendo propiedad " + prop + ". Los valores válidos para " + type.Name + " son: " + values);
            }
        }
    }
}
