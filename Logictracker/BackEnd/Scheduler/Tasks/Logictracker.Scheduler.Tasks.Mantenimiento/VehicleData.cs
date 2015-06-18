using System;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class VehicleData : BaseVehicleTask
    {
        private const String ComponentName = "Vehicle Data Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            base.OnExecute(timer);

            var left = Vehicles.Count;
            foreach (var vehicleId in Vehicles)
            {
                var vehicle = DaoFactory.CocheDAO.FindById(vehicleId);

                try
                {
                    if (vehicle.CocheOperacion == null)
                    {
                        STrace.Error(GetType().FullName, String.Format("El vehiculo id={0} no tiene datos operativos asociados(opeenti03)", vehicle.Id));
                        continue;
                    }

                    var today = DateTime.UtcNow.Date;
                    //var historicStart = vehicle.CocheOperacion.FechaInicio;
                    //var historicEnd = today;
                    //var historicTime = historicEnd.Subtract(historicStart);
                    //var historicDays = historicTime.TotalDays;
                    //var historicDm = DaoFactory.DatamartDAO.GetSummarizedDatamart(historicStart, historicEnd, vehicle.Id);
                    
                    var lastMonth = today.AddMonths(-1);
                    //lastMonth = lastMonth < historicStart ? historicStart : lastMonth;
                    var monthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                    var monthEnd = new DateTime(today.Year, today.Month, 1);
                    var monthDm = new DatamartDAO.SummarizedDatamart();

                    //var yearStart = monthEnd.AddYears(-1);
                    //yearStart = yearStart < historicStart ? historicStart : yearStart;
                    //var yearEnd = monthEnd;
                    //var yearDm = DaoFactory.DatamartDAO.GetSummarizedDatamart(yearStart, yearEnd, vehicle.Id);

                    var monthCost = 0.0;
                    //var historicCost = 0.0;
                    //var yearCost = 0.0;

                    switch (vehicle.Empresa.TipoCalculoCostoKm)
                    {
                        case "consumos":
                            monthCost = DaoFactory.ConsumoCabeceraDAO.FindCostoVehiculo(monthStart, monthEnd, vehicle.Id);
                            //historicCost = DaoFactory.ConsumoCabeceraDAO.FindCostoVehiculo(historicStart, historicEnd, vehicle.Id);
                            //yearCost = DaoFactory.ConsumoCabeceraDAO.FindCostoVehiculo(yearStart, yearEnd, vehicle.Id);
                            break;
                        case "caudalimetro":
                            if (vehicle.Modelo != null && vehicle.Modelo.Insumo != null && vehicle.Modelo.Insumo.ValorReferencia > 0.0)
                            {
                                monthDm = DaoFactory.DatamartDAO.GetSummarizedDatamart(monthStart, monthEnd, vehicle.Id);
                                monthCost = monthDm.Consumo * vehicle.Modelo.Insumo.ValorReferencia;
                                //yearCost = yearDm.Consumo * vehicle.Modelo.Insumo.ValorReferencia;
                                //historicCost = historicDm.Consumo * vehicle.Modelo.Insumo.ValorReferencia;
                            }
                            break;
                        default:
                            if (vehicle.Modelo != null && (vehicle.Modelo.Rendimiento > 0.0 || vehicle.Modelo.RendimientoRalenti > 0.0) && vehicle.Modelo.Insumo != null && vehicle.Modelo.Insumo.ValorReferencia > 0.0)
                            {
                                monthDm = DaoFactory.DatamartDAO.GetSummarizedDatamart(monthStart, monthEnd, vehicle.Id);
                                var hsRalentiMonth = monthDm.HsMarcha - monthDm.HsMovimiento;
                                if (hsRalentiMonth < 0) hsRalentiMonth = 0.00;
                                //var hsRalentiYear = yearDm.HsMarcha - yearDm.HsMovimiento;
                                //if (hsRalentiYear < 0) hsRalentiYear = 0.00;
                                //var hsRalentiHistoric = historicDm.HsMarcha - historicDm.HsMovimiento;
                                //if (hsRalentiHistoric < 0) hsRalentiHistoric = 0.00;

                                var litrosMonth = ((monthDm.Kilometros/100.0) * vehicle.Modelo.Rendimiento) + hsRalentiMonth * vehicle.Modelo.RendimientoRalenti;
                                //var litrosYear = ((yearDm.Kilometros / 100.0) * vehicle.Modelo.Rendimiento) + hsRalentiYear * vehicle.Modelo.RendimientoRalenti;
                                //var litrosHistoric = ((historicDm.Kilometros / 100.0) * vehicle.Modelo.Rendimiento) + hsRalentiHistoric * vehicle.Modelo.RendimientoRalenti;
                            
                                monthCost = litrosMonth * vehicle.Modelo.Insumo.ValorReferencia;
                                //yearCost = litrosYear * vehicle.Modelo.Insumo.ValorReferencia;
                                //historicCost = litrosHistoric * vehicle.Modelo.Insumo.ValorReferencia;
                            }
                            break;
                    }

                    //vehicle.CocheOperacion.CostoDiaHistorico = historicDays > 0 ? historicCost/historicDays : 0;
                    //vehicle.CocheOperacion.CostoKmHistorico = historicDm.Kilometros > 0 ? historicCost/historicDm.Kilometros : 0;
                    //vehicle.CocheOperacion.CostoMesHistorico = vehicle.CocheOperacion.CostoDiaHistorico * 30;
                    //vehicle.CocheOperacion.CostoKmUltimoAnio = yearDm.Kilometros > 0 ? yearCost/yearDm.Kilometros : 0;
                    vehicle.CocheOperacion.CostoKmUltimoMes = monthDm.Kilometros > 0 ? monthCost/monthDm.Kilometros : 0;
                    vehicle.CocheOperacion.CostoUltimoMes = monthCost;
                    vehicle.CocheOperacion.Rendimiento = monthDm.Consumo > 0 && monthDm.Kilometros > 0 ? (monthDm.Consumo / monthDm.Kilometros) * 100 : 0;
                    
                    try
                    {
                        // EVALUO SI EL TIPO DE COCHE CONTROLA ALARMAS DE CONSUMO Y SI TIENE AL MENOS UN MAXIMO O UN MINIMO DEFINIDO
                        if (vehicle.TipoCoche.AlarmaConsumo && (vehicle.TipoCoche.DesvioMaximo > 0 || vehicle.TipoCoche.DesvioMinimo > 0))
                        {
                            STrace.Trace(ComponentName, String.Format("Control de consumo para el vehiculo: {0}", vehicle.Interno));

                            var fecha = vehicle.CocheOperacion.UltimoControlConsumo.HasValue
                                            ? vehicle.CocheOperacion.UltimoControlConsumo.Value
                                            : DateTime.Today.ToDataBaseDateTime().AddDays(-1);

                            STrace.Trace(ComponentName, String.Format("Ultimo Control de Consumo: {0:dd/MM/yyyy}", fecha));

                            while (fecha < DateTime.Today.ToDataBaseDateTime())
                            {
                                // BUSCO EL 1ER DESPACHO REALIZADO LUEGO DE LA ULTIMA EVALUACION
                                var primerDespacho = DaoFactory.ConsumoDetalleDAO.GetPrimerDespacho(vehicle.Id, fecha, null);
                                
                                if (primerDespacho != null)
                                {
                                    STrace.Trace(ComponentName, String.Format("Primer despacho: {0:dd/MM/yyyy}, Cantidad: {1:#0.00}", primerDespacho.ConsumoCabecera.Fecha, primerDespacho.Cantidad));

                                    // BUSCO EL ULTIMO DESPACHO REALIZADO ANTES DE LA ULTIMA EVALUACION
                                    var ultimoDespacho = DaoFactory.ConsumoDetalleDAO.GetUltimoDespacho(vehicle.Id, null, primerDespacho.ConsumoCabecera.Fecha);

                                    if (ultimoDespacho != null)
                                    {
                                        STrace.Trace(ComponentName, String.Format("Despacho anterior: {0:dd/MM/yyyy}, Cantidad: {1:#0.00}", ultimoDespacho.ConsumoCabecera.Fecha, ultimoDespacho.Cantidad));
                                        var total = GetTotalDespacho(primerDespacho);
                                        var consumo = 0.0;
                                        if (DaoFactory.SensorDAO.FindByDispositivoAndTipoMedicion(vehicle.Id, "NU") != null)
                                        {
                                            consumo += DaoFactory.DatamartDAO.GetSummarizedDatamart(ultimoDespacho.ConsumoCabecera.Fecha,
                                                                                                    primerDespacho.ConsumoCabecera.Fecha, 
                                                                                                    vehicle.Id).Consumo;
                                            STrace.Trace(ComponentName, String.Format("Consumo por caudalimetro: {0:#0.00}", consumo));
                                        }
                                        else
                                        {    
                                            if (vehicle.Modelo.Rendimiento > 0.0)
                                                consumo += DaoFactory.DatamartDAO.GetSummarizedDatamart(ultimoDespacho.ConsumoCabecera.Fecha,
                                                                                                        primerDespacho.ConsumoCabecera.Fecha,
                                                                                                        vehicle.Id).Kilometros * vehicle.Modelo.Rendimiento / 100.0;
                                            
                                            if (vehicle.Modelo.RendimientoRalenti > 0.0)
                                            {
                                                var sdm = DaoFactory.DatamartDAO.GetSummarizedDatamart(ultimoDespacho.ConsumoCabecera.Fecha,
                                                                                                        primerDespacho.ConsumoCabecera.Fecha,
                                                                                                        vehicle.Id);
                                                var hsRalenti = sdm.HsMarcha - sdm.HsMovimiento;
                                                if (hsRalenti < 0.0) hsRalenti = 0.0;
                                                consumo += hsRalenti * vehicle.Modelo.RendimientoRalenti;
                                            }
                                            
                                            STrace.Trace(ComponentName, String.Format("Consumo por rendimiento: {0:#0.00}", consumo));
                                        }

                                        var max = 1 + (vehicle.TipoCoche.DesvioMaximo / 100);
                                        var min = 1 - (vehicle.TipoCoche.DesvioMinimo / 100);
                                        if (consumo > 0)
                                        {
                                            if (consumo > total * max)
                                                MessageSaver.Save(MessageIdentifier.ConsumptionDeviationHigh.ToString("d"), vehicle, primerDespacho.ConsumoCabecera.Fecha, null, "Alarma Desvío Máximo de Combustible");
                                            if (consumo < total * min)
                                                MessageSaver.Save(MessageIdentifier.ConsumptionDeviationLow.ToString("d"), vehicle, primerDespacho.ConsumoCabecera.Fecha, null, "Alarma Desvío Mínimo de Combustible");
                                        }
                                    }
                                    else
                                    {
                                        STrace.Trace(ComponentName, "Despacho anterior: null");
                                    }
                                    fecha = GetFechaDespacho(primerDespacho);
                                }
                                else
                                {
                                    STrace.Trace(ComponentName, "Primer despacho: null");
                                    fecha = DateTime.Today.ToDataBaseDateTime();
                                }
                            }
                            vehicle.CocheOperacion.UltimoControlConsumo = fecha;
                        }
                    }
                    catch (Exception)
                    {
                        STrace.Error(GetType().FullName, String.Format("El vehiculo id={0} produjo un error al evaluar alarmas por consumo de combustible.", vehicle.Id));
                    }

                    DaoFactory.CocheDAO.SaveOrUpdate(vehicle);

                    STrace.Trace(GetType().FullName, String.Format("Terminado Vehiculo Id={0}. Faltan: {1}.", vehicle.Id, --left));
                    DaoFactory.SessionClear();
                }
                catch(Exception ex)
                {
                    STrace.Exception(GetType().FullName, ex);
                }
            }
        }

        private DateTime GetFechaDespacho(ConsumoDetalle consumoDetalle)
        {
            var fecha = consumoDetalle.ConsumoCabecera.Fecha;
            var consumoSiguiente = DaoFactory.ConsumoDetalleDAO.GetPrimerDespacho(consumoDetalle.ConsumoCabecera.Vehiculo.Id, consumoDetalle.ConsumoCabecera.Fecha, consumoDetalle.ConsumoCabecera.Fecha.AddMinutes(5));

            while (consumoSiguiente != null)
            {
                fecha = consumoSiguiente.ConsumoCabecera.Fecha;
                consumoSiguiente = DaoFactory.ConsumoDetalleDAO.GetPrimerDespacho(consumoSiguiente.ConsumoCabecera.Vehiculo.Id, consumoSiguiente.ConsumoCabecera.Fecha, consumoSiguiente.ConsumoCabecera.Fecha.AddMinutes(5));
            }

            return fecha;
        }

        private double GetTotalDespacho(ConsumoDetalle consumoDetalle)
        {
            var total = consumoDetalle.Cantidad;
            var consumoSiguiente = DaoFactory.ConsumoDetalleDAO.GetPrimerDespacho(consumoDetalle.ConsumoCabecera.Vehiculo.Id, consumoDetalle.ConsumoCabecera.Fecha, consumoDetalle.ConsumoCabecera.Fecha.AddMinutes(5));

            while (consumoSiguiente != null)
            {
                total += consumoSiguiente.Cantidad;
                consumoSiguiente = DaoFactory.ConsumoDetalleDAO.GetUltimoDespacho(consumoSiguiente.ConsumoCabecera.Vehiculo.Id, consumoSiguiente.ConsumoCabecera.Fecha, consumoSiguiente.ConsumoCabecera.Fecha.AddMinutes(5));
            }

            return total;
        }
    }
}
