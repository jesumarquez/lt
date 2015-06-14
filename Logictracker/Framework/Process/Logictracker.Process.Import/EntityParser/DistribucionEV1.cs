using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.Process.Import.EntityParser
{
    public class DistribucionEV1 : EntityParserBase
    {
        // EDESUR
        protected override string EntityName { get { return "Distribucion"; } }

        public DistribucionEV1() {}
        public DistribucionEV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var oEmpresa = DaoFactory.EmpresaDAO.FindById(empresa);
            
            var tipo = data[Properties.DistribucionE.Tipo].Trim();
            var nroDoc = data[Properties.DistribucionE.NroDocumento].Trim();
            var despacho = data[Properties.DistribucionE.FechaDespacho].Trim();
            var generacion = data[Properties.DistribucionE.FechaGeneracion].Trim();
            var guardia = data[Properties.DistribucionE.Guardia].Trim();
            var latitud = data[Properties.DistribucionE.Latitud].Trim();
            var longitud = data[Properties.DistribucionE.Longitud].Trim();
            
            var codigoViaje = DateTime.Today.ToString("ddMMyy") + "|" + guardia;
            var codigoTarea = tipo + "|" + nroDoc;
            
            var empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, -1, guardia);
            if (empleado == null) ThrowProperty("EMPLEADO");

            var vehiculo = DaoFactory.CocheDAO.FindByChofer(empleado.Id);
            if (vehiculo == null) ThrowProperty("VEHICULO");

            var oLinea = vehiculo.Linea != null ? vehiculo.Linea : empleado.Linea;
            if (oLinea == null) ThrowProperty("LINEA");
            
            var oCliente = DaoFactory.ClienteDAO.GetList(new[] {empresa}, new[] {-1}).FirstOrDefault();
            if (oCliente == null) ThrowProperty("CLIENTE");
            const int vigencia = 24;
            
            var item = GetDistribucion(empresa, oLinea.Id, codigoViaje);
            if (data.Operation == (int) Operation.Delete) return item;

            var anioDespacho = Convert.ToInt32(despacho.Substring(0, 4));
            var mesDespacho = Convert.ToInt32(despacho.Substring(5, 2));
            var diaDespacho = Convert.ToInt32(despacho.Substring(8, 2));
            var horaDespacho = Convert.ToInt32(despacho.Substring(11, 2));
            var minDespacho = Convert.ToInt32(despacho.Substring(14, 2));
            var gmt = new TimeSpan(-3, 0, 0);
            var fechaDespacho = new DateTime(anioDespacho, mesDespacho, diaDespacho, horaDespacho, minDespacho, 0).Subtract(gmt);

            var anioGeneracion = Convert.ToInt32(generacion.Substring(0, 4));
            var mesGeneracion = Convert.ToInt32(generacion.Substring(5, 2));
            var diaGeneracion = Convert.ToInt32(generacion.Substring(8, 2));
            var horaGeneracion = Convert.ToInt32(generacion.Substring(11, 2));
            var minGeneracion = Convert.ToInt32(generacion.Substring(14, 2));
            var fechaGeneracion = new DateTime(anioGeneracion, mesGeneracion, diaGeneracion, horaGeneracion, minGeneracion, 0).Subtract(gmt);

            if (item.Id == 0)
            {
                item.Empresa = oEmpresa;
                item.Linea = oLinea;
                item.Vehiculo = vehiculo;
                item.Inicio = fechaDespacho;
                item.Fin = fechaDespacho;
                item.Tipo = ViajeDistribucion.Tipos.Desordenado;
                item.RegresoABase = false;
                item.Estado = ViajeDistribucion.Estados.Pendiente;
                item.Alta = DateTime.UtcNow;
                item.ProgramacionDinamica = false;
                item.NumeroViaje = 1;
                item.Empleado = empleado;
                item.CentroDeCostos = vehiculo.CentroDeCostos;
                item.SubCentroDeCostos = vehiculo.SubCentroDeCostos;
            }

            // Entregas
            if (item.Detalles.Count == 0)
            {
                //Si no existe, agrego la salida de base
                var origen = new EntregaDistribucion
                                 {
                                     Linea = oLinea,
                                     Descripcion = oLinea.Descripcion,
                                     Estado = EntregaDistribucion.Estados.Pendiente,
                                     Orden = 0,
                                     Programado = fechaDespacho,
                                     ProgramadoHasta = fechaDespacho,
                                     Viaje = item
                                 };
                item.Detalles.Add(origen);
            }
            
            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DaoFactory.TipoServicioCicloDAO.FindDefault(new[] {empresa}, new[] {oLinea.Id});
            if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

            var lat = Convert.ToDouble(latitud, CultureInfo.InvariantCulture);
            var lon = Convert.ToDouble(longitud, CultureInfo.InvariantCulture);

            var puntoEntrega = DaoFactory.PuntoEntregaDAO.GetByCode(new[] {empresa}, new[] {oLinea.Id}, new[] {oCliente.Id}, codigoTarea);

            if (puntoEntrega == null)
            {
                var empresaGeoRef = item.Vehiculo != null && item.Vehiculo.Empresa == null ? null : oCliente.Empresa == null ? null : oEmpresa;
                var lineaGeoRef = item.Vehiculo != null && item.Vehiculo.Linea == null ? null : oCliente.Linea == null ? null : oLinea;

                var puntoDeInteres = new ReferenciaGeografica
                                         {
                                             Codigo = codigoTarea,
                                             Descripcion = codigoTarea,
                                             Empresa = empresaGeoRef,
                                             Linea = lineaGeoRef,
                                             EsFin = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                                             EsInicio = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                                             EsIntermedio = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                                             InhibeAlarma = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                                             TipoReferenciaGeografica = oCliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                             Vigencia = new Vigencia {Inicio = DateTime.UtcNow, Fin = fechaDespacho.AddHours(vigencia)},
                                             Icono = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                                         };

                var posicion = GetNewDireccion(lat, lon);

                var poligono = new Poligono {Radio = 50, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                poligono.AddPoints(new[] {new PointF((float) lon, (float) lat)});

                puntoDeInteres.Historia.Add(new HistoriaGeoRef
                                                {
                                                    ReferenciaGeografica = puntoDeInteres,
                                                    Direccion = posicion,
                                                    Poligono = poligono,
                                                    Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                                });

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoDeInteres);
                STrace.Trace("QtreeReset", "DistribucionEV1 1");

                puntoEntrega = new PuntoEntrega
                                   {
                                       Cliente = oCliente,
                                       Codigo = codigoTarea,
                                       Descripcion = codigoTarea,
                                       Telefono = string.Empty,
                                       Baja = false,
                                       ReferenciaGeografica = puntoDeInteres,
                                       Nomenclado = true,
                                       DireccionNomenclada = string.Empty,
                                       Nombre = codigoTarea
                                   };
            }
            else
            {
                if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink && (puntoEntrega.ReferenciaGeografica.Latitude != lat || puntoEntrega.ReferenciaGeografica.Longitude != lon))
                {
                    puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                    puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                    var posicion = GetNewDireccion(lat, lon);
                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)lon, (float)lat) });

                    puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                }

                var end = fechaDespacho.AddHours(vigencia);
                if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                    puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "DistribucionEV1 2");
            }

            DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);

            var entrega = new EntregaDistribucion
                              {
                                  Cliente = oCliente,
                                  PuntoEntrega = puntoEntrega,
                                  Descripcion = codigoTarea,
                                  Estado = EntregaDistribucion.Estados.Pendiente,
                                  Orden = item.Detalles.Count,
                                  Programado = fechaDespacho,
                                  ProgramadoHasta = fechaDespacho,
                                  TipoServicio = tipoServicio,
                                  Viaje = item
                              };

            item.Detalles.Add(entrega);

            var lastDetail = item.Detalles.LastOrDefault();
            item.Fin = lastDetail == null ? item.Inicio : lastDetail.Programado;
            return item;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateSaveOrUpdate(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateDelete(tipo)) DaoFactory.ViajeDistribucionDAO.Delete(tipo);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateSave(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateUpdate(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        #endregion

        protected virtual ViajeDistribucion GetDistribucion(int empresa, int linea, string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) ThrowCodigo();

            var sameCode = DaoFactory.ViajeDistribucionDAO.FindByCodigo(empresa, linea, codigo);
            return sameCode ?? new ViajeDistribucion {Codigo = codigo};
        }

        private static Direccion GetNewDireccion(double latitud, double longitud)
        {
            return new Direccion
                       {
                           Altura = -1,
                           IdMapa = -1,
                           Provincia = string.Empty,
                           IdCalle = -1,
                           IdEsquina = -1,
                           IdEntrecalle = -1,
                           Latitud = latitud,
                           Longitud = longitud,
                           Partido = string.Empty,
                           Pais = string.Empty,
                           Calle = string.Empty,
                           Descripcion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture), longitud.ToString(CultureInfo.InvariantCulture)),
                           Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                       }
                ;
        }
    }
}
