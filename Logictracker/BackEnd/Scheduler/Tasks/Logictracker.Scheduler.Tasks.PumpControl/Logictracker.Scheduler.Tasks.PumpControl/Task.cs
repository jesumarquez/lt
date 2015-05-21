using System;
using System.Globalization;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Interfaces.PumpControl;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Interfaces.PumpControl.PumpControlService;

namespace Logictracker.Scheduler.Tasks.PumpControl
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Pump Control Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var service = new Service();
            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                if (empresa.PumpHabilitado)
                {
                    var user = empresa.PumpUsuario;
                    var password = empresa.PumpPassword;
                    var company = empresa.PumpCompany;

                    STrace.Trace(ComponentName, string.Format("Pump Habilitado para: {0}", empresa.RazonSocial));
                    
                    getFirstPendingTransactionResponse despacho;
                    try
                    {
                        despacho = service.GetFirstPendingTransaction(user, password, company);

                        if (despacho == null) STrace.Trace(ComponentName, "Primer despacho == null");
                        if (despacho != null && despacho.trx_id == null) STrace.Trace(ComponentName, "Primer despacho con id nulo");
                    }
                    catch (Exception ex) { STrace.Exception(ComponentName, ex); continue; }

                    while (despacho != null && despacho.trx_id != null && Convert.ToInt32(despacho.trx_id) > 0) // CONTROLAR QUE MANDA CUANDO NO HAY MAS DESPACHOS PENDIENTES
                    {
                        using (var transaction = SmartTransaction.BeginTransaction())
                        {

                            try
                            {
                                if (despacho.lastinformed_error_msg != null) throw new ApplicationException(despacho.lastinformed_error_msg);

                                var patente = despacho.car_plate;
                                if (patente == string.Empty)
                                {
                                    switch (company)
                                    {
                                        case "PAV":
                                            patente = "EXTRAS";
                                            break;
                                        case "TO":
                                            patente = "PLAYERO";
                                            break;
                                    }
                                }

                                var vehiculo = DaoFactory.CocheDAO.FindByPatente(-1, patente);
                                if (vehiculo == null) throw new ApplicationException(string.Format("Vehículo no encontrado: Patente: {0}", patente));

                                var deposito = DaoFactory.DepositoDAO.FindByCode(new[] {vehiculo.Empresa.Id},
                                    new[] {vehiculo.Linea != null ? vehiculo.Linea.Id : -1}, despacho.store);
                                if (deposito == null)
                                    throw new ApplicationException(string.Format("Depósito no encontrado: Codigo {0}", despacho.store));

                                var insumo = DaoFactory.InsumoDAO.FindByCode(new[] {vehiculo.Empresa.Id},
                                    new[] {vehiculo.Linea != null ? vehiculo.Linea.Id : -1}, /*despacho.prod_id*/ "1");
                                if (insumo == null) throw new ApplicationException(string.Format("Insumo no encontrado: Codigo {0}", despacho.prod_id));

                                var nroFactura = despacho.trx_id;
                                var consumoExistente = DaoFactory.ConsumoCabeceraDAO.FindByNroFactura(new[] {vehiculo.Empresa.Id},
                                    new[] {vehiculo.Linea != null ? vehiculo.Linea.Id : -1}, nroFactura);
                                if (consumoExistente != null)
                                {
                                    STrace.Trace(ComponentName, string.Format("Despacho ya procesado: Factura Nro {0}", nroFactura));
                                    continue;
                                }

                                var stock = DaoFactory.StockDAO.GetByDepositoAndInsumo(deposito.Id, insumo.Id) ??
                                            new Stock {Deposito = deposito, Insumo = insumo};
                                var fecha = Convert.ToDateTime(despacho.trx_date);
                                var precioUnitario = Convert.ToDouble(despacho.ppu, CultureInfo.InvariantCulture);
                                var cantidad = Convert.ToDouble(despacho.trx_volume, CultureInfo.InvariantCulture);
                                var precioTotal = Convert.ToDouble(despacho.amount, CultureInfo.InvariantCulture);
                                var km = Convert.ToDouble(despacho.car_kilometer, CultureInfo.InvariantCulture);

                                var consumoCabecera = new ConsumoCabecera
                                                      {
                                                          Deposito = deposito,
                                                          Estado = ConsumoCabecera.Estados.Pagado,
                                                          Fecha = fecha.AddHours(3),
                                                          ImporteTotal = precioTotal,
                                                          KmDeclarados = km,
                                                          NumeroFactura = nroFactura,
                                                          TipoMovimiento = ConsumoCabecera.TiposMovimiento.DepositoAVehiculo,
                                                          Vehiculo = vehiculo
                                                      };

                                DaoFactory.ConsumoCabeceraDAO.SaveOrUpdate(consumoCabecera);

                                var consumoDetalle = new ConsumoDetalle
                                                     {
                                                         Cantidad = cantidad,
                                                         ConsumoCabecera = consumoCabecera,
                                                         ImporteTotal = precioTotal,
                                                         ImporteUnitario = precioUnitario,
                                                         Insumo = insumo
                                                     };

                                stock.Cantidad -= consumoDetalle.Cantidad;

                                DaoFactory.ConsumoDetalleDAO.SaveOrUpdate(consumoDetalle);
                                DaoFactory.StockDAO.SaveOrUpdate(stock);

                                if (stock.AlarmaActiva)
                                {
                                    if (stock.Cantidad < stock.StockCritico)
                                    {
                                        MessageSaver.Save(MessageIdentifier.StockCritic.ToString("d"), consumoCabecera.Vehiculo, consumoCabecera.Fecha, null,
                                            "Alarma Stock Crítico");
                                    }
                                    else if (stock.Cantidad < stock.PuntoReposicion)
                                    {
                                        MessageSaver.Save(MessageIdentifier.StockReposition.ToString("d"), consumoCabecera.Vehiculo, consumoCabecera.Fecha, null,
                                            "Alarma Reposición de Stock");
                                    }
                                }

                                transaction.Commit();

                                STrace.Trace(ComponentName, consumoCabecera.Vehiculo.Dispositivo != null ? consumoCabecera.Vehiculo.Dispositivo.Id : 0,
                                    string.Format("Despacho importado - Fecha: {0:dd/MM/yyyy HH:mm} - Factura: {1}", consumoCabecera.Fecha.AddHours(-3),
                                        consumoCabecera.NumeroFactura));

                                var confirmacion = service.SetLastInformedTransaction(user, password, company, despacho.trx_id);
                                if (confirmacion.lastinformed_error_msg != null)
                                    STrace.Trace(ComponentName, consumoCabecera.Vehiculo.Dispositivo != null ? consumoCabecera.Vehiculo.Dispositivo.Id : 0,
                                        string.Format("Mensaje de confirmación: {0}", confirmacion.lastinformed_error_msg));
                            }
                            catch (Exception ex)
                            {
                                STrace.Exception(ComponentName, ex);
                                transaction.Rollback();
                            }
                            finally
                            {
                                despacho = service.GetFirstPendingTransaction(user, password, company, despacho.trx_id);
                            }
                        }
                    }
                }
                else { STrace.Trace(ComponentName, string.Format("Pump No Habilitado para: {0}", empresa.RazonSocial)); }
            }
        }
    }
}