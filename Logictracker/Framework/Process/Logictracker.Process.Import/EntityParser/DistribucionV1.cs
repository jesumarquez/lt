using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Process.Import.EntityParser
{
    public class DistribucionV1 : EntityParserBase
    {
        protected override string EntityName
        {
            get { return "Distribucion"; }
        }

         public DistribucionV1()
        {
        }

         public DistribucionV1(DAOFactory daoFactory)
            : base(daoFactory)
        {
        }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var item = GetDistribucion(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return item;

            var modificaCabecera = data.AsBool(Properties.Distribucion.ModificaCabecera) ?? true;
            var gmt = data.AsInt32(Properties.Distribucion.Gmt) ?? 0;

            if (item.Id == 0 || modificaCabecera)
            {
                if (item.Id == 0)
                {
                    item.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                    item.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
                    item.Alta = DateTime.UtcNow;
                }

                var codigoLinea = data.AsString(Properties.Distribucion.Linea, 8);
                if (codigoLinea != null)
                {
                    item.Linea = GetLinea(empresa, codigoLinea) ?? item.Linea;
                    linea = item.Linea != null ? item.Linea.Id : -1;
                }

                var interno = data.AsString(Properties.Distribucion.Vehiculo, 32);
                if (interno != null)
                {
                    var vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] { empresa }, new[] { linea }, interno);
                    if (vehiculo != null)
                    {
                        item.Vehiculo = vehiculo;
                    }
                }

                var legajo = data.AsString(Properties.Distribucion.Empleado, 10);
                if (legajo != null)
                {
                    var empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, legajo);
                    if (empleado != null)
                    {
                        item.Empleado = empleado;
                    }
                }

                var fecha = data.AsDateTime(Properties.Distribucion.Fecha, gmt);
                if (!fecha.HasValue)
                {
                    ThrowProperty("Fecha");
                }
                item.Inicio = fecha.Value;

                var tipo = data.AsString(Properties.Distribucion.TipoCiclo, 1) ?? "N";
                switch (tipo)
                {
                    case "R": item.Tipo = ViajeDistribucion.Tipos.RecorridoFijo; break;
                    case "D": item.Tipo = ViajeDistribucion.Tipos.Desordenado; break;
                    default: item.Tipo = ViajeDistribucion.Tipos.Ordenado; break;
                }

                var regresaABase = data.AsBool(Properties.Distribucion.RegresaABase);
                if (regresaABase.HasValue)
                {
                    item.RegresoABase = regresaABase.Value;
                }
            }

            // Entregas
            if(item.Detalles.Count == 0)
            {
                //Si no existe, agrego la slida de base
                var salidaBase = new EntregaDistribucion
                                     {
                                         Descripcion = item.Linea.Descripcion, 
                                         Linea = item.Linea, 
                                         Viaje = item, 
                                         Programado = item.Inicio,
                                         ProgramadoHasta = item.Inicio
                                     };
                item.Detalles.Add(salidaBase);
            }
            else if(item.Detalles.Count > 1)
            {
                //Si existe, elimino la llegada a base (despues la vuelvo a agregar si hace falta).
                var last = item.Detalles.Last();
                if(last.Linea != null)
                {
                    item.Detalles.RemoveAt(item.Detalles.Count-1);
                }
            }

            var codigoCliente = data.AsString(Properties.Distribucion.Cliente, 32);
            var codigoPuntoEntrega = data.AsString(Properties.Distribucion.PuntoEntrega, 32);
            var codigoTipoServicio = data.AsString(Properties.Distribucion.TipoServicio, 64);
            var programado = data.AsDateTime(Properties.Distribucion.Programado, gmt);
            if (programado.HasValue && item.Inicio.Subtract(programado.Value).TotalHours > 24)
                programado = item.Inicio.Date.AddHours(programado.Value.Hour).AddMinutes(programado.Value.Minute);
            var orden = data.AsInt32(Properties.Distribucion.Orden);

            if(string.IsNullOrEmpty(codigoCliente))
            {
                ThrowProperty("Cliente");
            }
            if(string.IsNullOrEmpty(codigoPuntoEntrega))
            {
                ThrowProperty("PuntoEntrega");
            }

            var cliente = DaoFactory.ClienteDAO.FindByCode(new[] {empresa}, new[] {linea}, codigoCliente);
            if(cliente == null)
            {
                ThrowProperty("Cliente");
            }
            var puntoEntrega = DaoFactory.PuntoEntregaDAO.FindByCode(new[] {empresa}, new[] {linea}, new[] {cliente.Id},codigoPuntoEntrega);
            if(puntoEntrega == null)
            {
                ThrowProperty("PuntoEntrega");
            }
            var tipoServicio = string.IsNullOrEmpty(codigoTipoServicio)
                                   ? null
                                   : DaoFactory.TipoServicioCicloDAO.FindByCode(new[] {empresa}, new[] {linea}, codigoTipoServicio);

            var entrega = new EntregaDistribucion
                              {
                                  Viaje = item,
                                  Cliente = cliente,
                                  PuntoEntrega = puntoEntrega,
                                  TipoServicio = tipoServicio,
                                  Orden = item.Detalles.Count,
                                  Descripcion = puntoEntrega.Descripcion
                              };

            item.Detalles.Add(entrega);
            if (programado.HasValue)
            {
                entrega.Programado = programado.Value;
                entrega.ProgramadoHasta = programado.Value;
            }
            else
            {
                CalcularHorarioPorDistancia(item);
            }

            if (item.RegresoABase)
            {
                // Si regresa a base, agrego la llegada
                var regresoBase = new EntregaDistribucion
                                      {
                                          Descripcion = item.Linea.Descripcion, 
                                          Linea = item.Linea, 
                                          Viaje = item, 
                                          Programado = item.Inicio, 
                                          ProgramadoHasta = item.Inicio,
                                          Orden = item.Detalles.Count
                                      };
                item.Detalles.Add(regresoBase);
                CalcularHorarioPorDistancia(item);
            }

            var lastDetail = item.Detalles.LastOrDefault();
            item.Fin = lastDetail == null ? item.Inicio : lastDetail.Programado;
            return item;
        }
        private void CalcularHorarioPorDistancia(ViajeDistribucion distribucion)
        {
            var index = distribucion.Detalles.Count - 1;
            if (index < 0) return;
            if (index == 0)
            {
                distribucion.Detalles[index].Programado = distribucion.Inicio;
                distribucion.Detalles[index].ProgramadoHasta = distribucion.Inicio;
                return;
            }

            var velocidadPromedio = GetVelocidad(distribucion);

            var prev = distribucion.Detalles[index - 1];
            var item = distribucion.Detalles[index];

            var prevDate = prev.Programado;

            var prevPunto = prev.PuntoEntrega != null
                                ? prev.PuntoEntrega.ReferenciaGeografica
                                : prev.Linea.ReferenciaGeografica;
            var punto = item.PuntoEntrega != null
                            ? item.PuntoEntrega.ReferenciaGeografica
                            : item.Linea.ReferenciaGeografica;

            var demora = prev.TipoServicio != null ? prev.TipoServicio.Demora : 0;

            var distancia = GeocoderHelper.CalcularDistacia(prevPunto.Latitude,
                                                     prevPunto.Longitude,
                                                     punto.Latitude,
                                                     punto.Longitude);
            var tiempoViaje = TimeSpan.FromHours(distancia / velocidadPromedio);

            var hora = prevDate.AddMinutes(demora).Add(tiempoViaje);

            distribucion.Detalles[index].Programado = hora;
            distribucion.Detalles[index].ProgramadoHasta = hora;
            distribucion.Fin = hora;
        }
        private int GetVelocidad(ViajeDistribucion distribucion)
        {
            if (distribucion.Vehiculo != null)
            {
                if (distribucion.Vehiculo.VelocidadPromedio > 0) return distribucion.Vehiculo.VelocidadPromedio;
                if (distribucion.Vehiculo.TipoCoche.VelocidadPromedio > 0) return distribucion.Vehiculo.TipoCoche.VelocidadPromedio;
            }
            return distribucion.Empresa != null ? distribucion.Empresa.VelocidadPromedio : 20;
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

        protected virtual ViajeDistribucion GetDistribucion(int empresa, int linea, IData data)
        {
            string codigo = data.AsString(Properties.Distribucion.Codigo,32);
            if (string.IsNullOrEmpty(codigo)) ThrowCodigo();

            var sameCode = DaoFactory.ViajeDistribucionDAO.FindByCodigo(empresa, linea, codigo);
            return sameCode ?? new ViajeDistribucion {Codigo = codigo};
        }
    }
}
