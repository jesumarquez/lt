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
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class DistribucionFV1 : EntityParserBase
    {
        // FEMSA
        protected override string EntityName { get { return "DistribucionF"; } }

        public DistribucionFV1() {}

        public DistribucionFV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {   
            var oEmpresa = DaoFactory.EmpresaDAO.FindById(empresa);
            var oCliente = DaoFactory.ClienteDAO.FindById(4186); // RESTO DE MERCADO
            const int vigencia = 24;

            var codLinea = data.AsString(Properties.DistribucionF.Centro, 4).Trim();
            if (string.IsNullOrEmpty(codLinea)) ThrowProperty("LINEA");
            var oLinea = DaoFactory.LineaDAO.FindByCodigo(empresa, codLinea);
            if (oLinea == null) ThrowProperty("LINEA");

            var ruta = data.AsString(Properties.DistribucionF.Ruta, 8).Trim();
            if (string.IsNullOrEmpty(ruta)) ThrowProperty("RUTA");
            int nroViaje;
            if (!int.TryParse(ruta.Split(',')[1], out nroViaje)) ThrowProperty("NUMERO_VIAJE");

            var codEntrega = data.AsString(Properties.DistribucionF.Entrega, 10).Trim();
            if (string.IsNullOrEmpty(codEntrega)) ThrowProperty("ENTREGA");
            
            var codCliente = data.AsString(Properties.DistribucionF.CodigoCliente, 9).Trim();
            if (string.IsNullOrEmpty(codCliente)) ThrowProperty("CODIGO_CLIENTE");
            
            var latitud = data.AsDouble(Properties.DistribucionF.Latitud) / 1000000.0;
            if (!latitud.HasValue) ThrowProperty("LATITUD");
            var longitud = data.AsDouble(Properties.DistribucionF.Longitud) / 1000000.0;
            if (!longitud.HasValue) ThrowProperty("LONGITUD");
            
            var fecha = data.AsString(Properties.DistribucionF.Fecha, 10).Trim();
            if (string.IsNullOrEmpty(fecha)) ThrowProperty("FECHA");
            
            var hora = data.AsString(Properties.DistribucionF.Hora, 8).Trim();
            if (string.IsNullOrEmpty(hora)) ThrowProperty("HORA");

            int dia, mes, anio, hr, min, seg;
            if (!int.TryParse(fecha.Substring(0, 2), out dia)) ThrowProperty("DIA");
            if (!int.TryParse(fecha.Substring(3, 2), out mes)) ThrowProperty("MES");
            if (!int.TryParse(fecha.Substring(6, 4), out anio)) ThrowProperty("AÑO");
            if (!int.TryParse(hora.Substring(0, 2), out hr)) ThrowProperty("HORA");
            if (hr == 0) hr = nroViaje <= 1 ? 6 : 12;
            if (!int.TryParse(hora.Substring(3, 2), out min)) ThrowProperty("MINUTO");
            if (!int.TryParse(hora.Substring(6, 2), out seg)) ThrowProperty("SEGUNDO");

            var gmt = new TimeSpan(-3, 0, 0);
            var date = new DateTime(anio, mes, dia, hr, min, seg).Subtract(gmt);

            var cajas = data.AsInt32(Properties.DistribucionF.Cajas);
            if (!cajas.HasValue) ThrowProperty("CAJAS");
            
            var codigo = date.ToString("yyMMdd") + "|" + ruta;
            var item = GetDistribucion(empresa, linea, codigo);
            if (data.Operation == (int) Operation.Delete) return item;
            
            if (item.Id == 0)
            {
                Coche vehiculo = null;
                var patente = data.AsString(Properties.DistribucionF.Patente, 10).Trim();
                if (!string.IsNullOrEmpty(patente)) vehiculo = DaoFactory.CocheDAO.GetByPatente(new[] { empresa }, new[] { oLinea.Id }, patente);
                
                if (vehiculo == null)
                {
                    var asig = DaoFactory.PreasignacionViajeVehiculoDAO.FindByCodigo(empresa, oLinea.Id, -1, ruta);
                    if (asig != null) vehiculo = asig.Vehiculo;
                }

                Empleado empleado = null;
                var legajo = data.AsString(Properties.DistribucionF.Legajo, 10).Trim();
                if (!string.IsNullOrEmpty(legajo)) empleado = DaoFactory.EmpleadoDAO.GetByLegajo(empresa, oLinea.Id, legajo);

                item.Empresa = oEmpresa;
                item.Linea = oLinea;
                item.Vehiculo = vehiculo;
                item.Empleado = empleado;
                item.Inicio = date;
                item.Fin = date;
                item.Tipo = ViajeDistribucion.Tipos.Desordenado;
                item.RegresoABase = true;
                item.Estado = ViajeDistribucion.Estados.Pendiente;
                item.Alta = DateTime.UtcNow;
                item.NumeroViaje = Convert.ToInt32(nroViaje);
            }

            // Entregas
            if (item.Detalles.Count == 0)
            {   // Si no existe, agrego la salida de base
                var origen = new EntregaDistribucion
                                 {
                                     Linea = oLinea,
                                     Descripcion = oLinea.Descripcion,
                                     Estado = EntregaDistribucion.Estados.Pendiente,
                                     Orden = 0,
                                     Programado = date,
                                     ProgramadoHasta = date,
                                     Viaje = item
                                 };
                item.Detalles.Add(origen);

                var llegada = new EntregaDistribucion
                {
                    Linea = oLinea,
                    Descripcion = oLinea.Descripcion,
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Orden = item.Detalles.Count,
                    Programado = date,
                    ProgramadoHasta = date,
                    Viaje = item
                };
                item.Detalles.Add(llegada);
            }

            if (item.Detalles.Any(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codCliente))
            {
                var repetido = item.Detalles.FirstOrDefault(d => d.PuntoEntrega != null && d.PuntoEntrega.Codigo == codCliente);
                repetido.Bultos += cajas.Value;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(repetido);
                return item;
            }

            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DaoFactory.TipoServicioCicloDAO.FindDefault(new[] {empresa}, new[] {linea});
            if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

            var puntoEntrega = DaoFactory.PuntoEntregaDAO.GetByCode(new[] {empresa}, new[] {linea}, new[] {-1}, codCliente);
            if (puntoEntrega == null)
            {
                var descCliente = data.AsString(Properties.DistribucionF.DescripcionCliente, 128).Trim();
                if (string.IsNullOrEmpty(descCliente)) ThrowProperty("NOMBRE_CLIENTE");

                var puntoDeInteres = new ReferenciaGeografica
                {
                    Codigo = codCliente,
                    Descripcion = descCliente,
                    Empresa = oEmpresa,
                    Linea = oLinea,
                    EsFin = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                    EsInicio = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                    EsIntermedio = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                    InhibeAlarma = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                    TipoReferenciaGeografica = oCliente.ReferenciaGeografica.TipoReferenciaGeografica,
                    Vigencia = new Vigencia { Inicio = date.Date, Fin = date.AddHours(vigencia) },
                    Icono = oCliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                };

                var posicion = GetNewDireccion(latitud.Value, longitud.Value);

                var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = date.Date } };
                poligono.AddPoints(new[] { new PointF((float)longitud.Value, (float)latitud.Value) });

                puntoDeInteres.Historia.Add(new HistoriaGeoRef
                {
                    ReferenciaGeografica = puntoDeInteres,
                    Direccion = posicion,
                    Poligono = poligono,
                    Vigencia = new Vigencia { Inicio = date.Date }
                });

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoDeInteres);
                STrace.Trace("QtreeReset", "DistribucionFV1 1");

                puntoEntrega = new PuntoEntrega
                {
                    Cliente = oCliente,
                    Codigo = codCliente,
                    Descripcion = descCliente,
                    Telefono = string.Empty,
                    Baja = false,
                    ReferenciaGeografica = puntoDeInteres,
                    Nomenclado = true,
                    DireccionNomenclada = string.Empty,
                    Nombre = descCliente
                };
            }
            else
            {
                if (!puntoEntrega.ReferenciaGeografica.IgnoraLogiclink && (puntoEntrega.ReferenciaGeografica.Latitude != latitud.Value || puntoEntrega.ReferenciaGeografica.Longitude != longitud.Value))
                {
                    puntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = date.Date;
                    puntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = date.Date;

                    var posicion = GetNewDireccion(latitud.Value, longitud.Value);
                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = date.Date } };
                    poligono.AddPoints(new[] { new PointF((float)longitud.Value, (float)latitud.Value) });

                    puntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, date.Date);
                }

                puntoEntrega.ReferenciaGeografica.Vigencia.Inicio = date.Date;
                var end = date.AddHours(vigencia);
                if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                    puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "DistribucionFV1 2");
            }

            DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
            
            var entrega = new EntregaDistribucion
                              {
                                  Cliente = puntoEntrega.Cliente,
                                  PuntoEntrega = puntoEntrega,
                                  Descripcion = codEntrega,
                                  Estado = EntregaDistribucion.Estados.Pendiente,
                                  Orden = item.Detalles.Count - 1,
                                  Programado = date,
                                  ProgramadoHasta = date,
                                  TipoServicio = tipoServicio,
                                  Viaje = item,
                                  Bultos = cajas.Value
                              };

            item.Detalles.Add(entrega);

            var maxDate = item.Detalles.Max(d => d.Programado);
            item.Fin = maxDate;

            var ultimo = item.Detalles.Last(e => e.Linea != null);
            if (ultimo.Id > 0)
            {
                ultimo.Programado = maxDate;
                ultimo.ProgramadoHasta = maxDate;
                ultimo.Orden = item.Detalles.Count - 1;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(ultimo);
            }

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
                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
            };
        }
    }
}
