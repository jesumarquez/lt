using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class ConsumoV1 : EntityParserBase
    {
        // TEL3
        protected override string EntityName { get { return "Consumo"; } }

        public ConsumoV1() { }

        public ConsumoV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var item = GetConsumo(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return item;

            var codigoLinea = data[Properties.Consumo.Linea];
            if (codigoLinea != null)
            {
                var l = GetLinea(empresa, data[Properties.Consumo.Linea]);
                if (l != null) linea = l.Id;
            }

            Coche vehiculo = null;
            var internoVehiculo = data[Properties.Consumo.Vehiculo];
            if (internoVehiculo != null || item.Vehiculo != null)
            {
                vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresa}, new[] {linea}, internoVehiculo);
                if (vehiculo == null && item.Vehiculo == null)
                {
                    vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresa}, new[] {linea}, "(Generico)");
                    if (vehiculo == null) throw new EntityParserException("No se encontró el vehículo con interno " + internoVehiculo);
                }
            }
            else
            {
                ThrowProperty("Vehiculo");
            }

            Proveedor proveedor = null;
            var codigoProveedor = data[Properties.Consumo.Proveedor];
            if (codigoProveedor != null || item.Proveedor != null)
            {
                if (codigoProveedor == string.Empty)
                {
                    proveedor = DaoFactory.ProveedorDAO.FindByCode(new[] {empresa}, new[] {linea}, "ST");
                    if (proveedor == null)
                    {
                        var tipoProveedor = DaoFactory.TipoProveedorDAO.FindByCode(new[] {empresa}, new[] {linea}, "G");
                        if (tipoProveedor == null)
                        {
                            tipoProveedor = new TipoProveedor
                                                {
                                                    Baja = false,
                                                    Codigo = "G",
                                                    Descripcion = "Generico",
                                                    Empresa = vehiculo.Empresa
                                                };

                            DaoFactory.TipoProveedorDAO.SaveOrUpdate(tipoProveedor);
                        }

                        proveedor = new Proveedor
                                        {
                                            Codigo = "ST",
                                            Descripcion = "Stock",
                                            Empresa = vehiculo.Empresa,
                                            TipoProveedor = tipoProveedor
                                        };

                        DaoFactory.ProveedorDAO.SaveOrUpdate(proveedor);
                    }
                }
                else
                    proveedor = DaoFactory.ProveedorDAO.FindByCode(new[] { empresa }, new[] { linea }, codigoProveedor);

                if (proveedor == null && item.Proveedor == null) 
                    throw new EntityParserException("No se encontro el Proveedor con código " + codigoProveedor);    
            }
            else
            {
                ThrowProperty("Proveedor");
            }

            Insumo insumo = null;
            var codigoInsumo = data[Properties.Consumo.Insumo];
            if (codigoInsumo != null)
            {
                if (codigoInsumo == string.Empty)
                {
                    insumo = DaoFactory.InsumoDAO.FindByCode(new[] { empresa }, new[] { linea }, "FF");
                    if (insumo == null)
                    {
                        var tipoInsumo = DaoFactory.TipoInsumoDAO.FindByCode(new[] { empresa }, new[] { linea }, "G");
                        var unidadMedida = DaoFactory.UnidadMedidaDAO.FindByCode("U");

                        if (tipoInsumo == null)
                        {
                            tipoInsumo = new TipoInsumo
                                             {
                                                 Codigo = "G",
                                                 DeCombustible = false,
                                                 Descripcion = "Generico",
                                                 Empresa = vehiculo.Empresa
                                             };

                            DaoFactory.TipoInsumoDAO.SaveOrUpdate(tipoInsumo);
                        }
                        if (unidadMedida == null)
                        {
                            unidadMedida = new UnidadMedida
                                               {
                                                   Codigo = "U", 
                                                   Descripcion = "Unidad", 
                                                   Simbolo = "Un"
                                               };

                            DaoFactory.UnidadMedidaDAO.SaveOrUpdate(unidadMedida);
                        }
                        
                        insumo = new Insumo
                                     {
                                         Codigo = "FF",
                                         Descripcion = "Fondo Fijo",
                                         Empresa = vehiculo.Empresa,
                                         Linea = vehiculo.Linea,
                                         TipoInsumo = tipoInsumo,
                                         UnidadMedida = unidadMedida
                                     };

                        DaoFactory.InsumoDAO.SaveOrUpdate(insumo);
                    }
                }
                else
                    insumo = DaoFactory.InsumoDAO.FindByCode(new[] { empresa }, new[] { linea }, codigoInsumo);
            }

            int igmt;
            int.TryParse(data[Properties.Consumo.Gmt], out igmt);

            var fecha = data[Properties.Consumo.Fecha];
            if (fecha != null || item.Fecha != default(DateTime))
            {
                if (!fecha.AsDateTime().HasValue && item.Fecha == default(DateTime))
                    throw new EntityParserException("La Fecha no es válida: " + fecha);
            }
            else
            {
                ThrowProperty("Fecha");
            }

            Empleado empleado = null;
            var legajoEmpleado = data[Properties.Consumo.Empleado];
            if (legajoEmpleado != null)
            {
                empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, legajoEmpleado);
            }
            
            item.NumeroFactura = data[Properties.Consumo.NroFactura].Truncate(64);
            if (vehiculo != null) item.Vehiculo = vehiculo;
            if (legajoEmpleado != null) item.Empleado = empleado;
            if (fecha.AsDateTime().HasValue) item.Fecha = fecha.AsDateTime().Value.AddHours(-igmt);
            if (data[Properties.Consumo.Km].AsDouble().HasValue) item.KmDeclarados = data[Properties.Consumo.Km].AsDouble() ?? 0;
            if (proveedor != null) item.Proveedor = proveedor;
            
            item.Deposito = null;
            item.DepositoDestino = null;
            item.Estado = ConsumoCabecera.Estados.Pagado;
            item.TipoMovimiento = ConsumoCabecera.TiposMovimiento.ProveedorAVehiculo;

            if (insumo != null)
            {
                var detalle = new ConsumoDetalle
                                  {
                                      ConsumoCabecera = item,
                                      Insumo = insumo,
                                      //Cambiar mapeo Tel3
                                      ImporteTotal = data[Properties.Consumo.ImporteUnitario].AsDouble() ?? 0,
                                      Cantidad = data[Properties.Consumo.Cantidad].AsDouble() ?? 1
                                  };
                detalle.ImporteUnitario = detalle.ImporteTotal / detalle.Cantidad;

                item.Detalles.Add(detalle);

                insumo.ValorReferencia = detalle.ImporteUnitario;
                DaoFactory.InsumoDAO.SaveOrUpdate(insumo);

                if (item.Vehiculo != null) DaoFactory.MovOdometroVehiculoDAO.ResetByVehicleAndInsumo(item.Vehiculo, insumo);
            }

            item.ImporteTotal = item.Detalles.Cast<ConsumoDetalle>().Select(d => d.ImporteTotal).Sum();

            return item;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ConsumoCabecera;
            if (ValidateSaveOrUpdate(item)) DaoFactory.ConsumoCabeceraDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ConsumoCabecera;
            if (ValidateDelete(item)) DaoFactory.ConsumoCabeceraDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ConsumoCabecera;
            if (ValidateSave(item)) DaoFactory.ConsumoCabeceraDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as ConsumoCabecera;
            if (ValidateUpdate(item)) DaoFactory.ConsumoCabeceraDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual ConsumoCabecera GetConsumo(int empresa, int linea, IData data)
        {
            var codigo = data[Properties.Consumo.NroFactura];
            if (codigo == null) ThrowProperty("NroFactura");

            if (codigo == string.Empty) return new ConsumoCabecera();

            var sameCode = DaoFactory.ConsumoCabeceraDAO.FindByNroFactura(new[] { empresa }, new[] { linea }, codigo);
            return sameCode ?? new ConsumoCabecera();
        }
    }
}
