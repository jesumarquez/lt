using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class ConsumoImport : SecuredImportPage
    {
        private const string Reporte = "Procesado: {0} de {1}<br/> Importados: {2} ({3}%)<br/>Erroneos: {4} ({5}%)<br/>Sin Importar: {6} ({7}%)<br/>Tiempo: {8}<br/>";
        private const string Result = "{0};{1};{2};{3};{4};{5}<br/>";

        protected override bool Redirect { get { return false; } }
        protected override string RedirectUrl { get { return "ConsumosLista.aspx"; } }
        protected override string VariableName { get { return "MAN_CONSUMOS"; } }
        protected override string GetRefference() { return "MAN_CONSUMOS"; }

        protected override List<FieldValue> GetMappingFields()
        {
            switch (CurrentImportMode)
            {
                case Modes.Default: return Fields.Default;
                case Modes.EssoCard: return Fields.EssoCard;
                case Modes.YpfEnRuta: return Fields.YpfEnRuta;
                case Modes.Edenred: return Fields.Edenred;
                default: return Fields.Default;
            }
        }
        private List<FieldValue> GetFieldsObligatorios()
        {
            switch (CurrentImportMode)
            {
                case Modes.Default: return Fields.ObligatoriosDefault;
                case Modes.EssoCard: return Fields.ObligatoriosEssoCard;
                case Modes.YpfEnRuta: return Fields.ObligatoriosDefault;
                case Modes.Edenred: return Fields.ObligatoriosDefault;
                default: return Fields.Default;
            }
        }
        
        private static class Modes
        {
            public const string Default = "Default";
            public const string EssoCard = "ESSO CARD";
            public const string YpfEnRuta = "YPF EN RUTA";
            public const string Edenred = "EDENRED";
            
        }

        protected override string[] ImportModes
        {
            get { return new[] { Modes.Default, Modes.EssoCard, Modes.YpfEnRuta, Modes.Edenred }; }
        }

        protected override void ValidateMapping()
        {
            var fields = GetFieldsObligatorios();
            for(var i = 1; i < fields.Count; i++)
            {
                var field = fields[i];
                if(!IsMapped(field.Value) && !HasDefault(field.Value)) throw new ApplicationException("Falta mapear " + field.Name);
            }

            if (CurrentImportMode != Modes.EssoCard)
            {
                if (!IsMapped(Fields.Vehiculo.Value) && !HasDefault(Fields.Vehiculo.Value)
                    && !IsMapped(Fields.DepositoDestino.Value) && !HasDefault(Fields.DepositoDestino.Value))
                    throw new ApplicationException("Falta mapear " + Fields.Vehiculo.Name + " o " +
                                                   Fields.DepositoDestino.Name);
                if (!IsMapped(Fields.Proveedor.Value) && !HasDefault(Fields.Proveedor.Value)
                    && !IsMapped(Fields.Deposito.Value) && !HasDefault(Fields.Deposito.Value))
                    throw new ApplicationException("Falta mapear " + Fields.Proveedor.Name + " o " +
                                                   Fields.Deposito.Name);
            }
        }

        protected override bool ValidateRow(ImportRow row)
        {
            ValidateDouble(GetValue(row, Fields.Cantidad.Value), "CANTIDAD");

            if (CurrentImportMode == Modes.EssoCard)
            {
                ValidateDouble(GetValue(row, Fields.Valor.Value), "VALOR");
            }
            else
            {
                ValidateDouble(GetValue(row, Fields.ImporteTotal.Value), "IMPORTE_TOTAL");
                ValidateDouble(GetValue(row, Fields.ImporteUnitario.Value), "IMPORTE_UNITARIO");
                if (IsMapped(Fields.Vehiculo.Value) || HasDefault(Fields.Vehiculo.Value))
                    ValidateDouble(GetValue(row, Fields.KmDeclarados.Value), "KM_REC");
            }

            return true;
        }

        protected override void Import(List<ImportRow> rows)
        {
            switch (CurrentImportMode)
            {
                case Modes.Default: ImportDefault(rows); break;
                case Modes.EssoCard: ImportEssoCard(rows); break;
                case Modes.YpfEnRuta: ImportYpfEnRuta(rows); break;
                case Modes.Edenred: ImportEdenred(rows); break;
                default: ImportDefault(rows); break;
            }
        }

        protected void ImportDefault(List<ImportRow> rows)
        {
            var procesados = 0;
            var erroneos = 0;
            var sinProcesar = 0;
            var index = 0;
            var start = DateTime.Now.Ticks;
            lblResult.Text = "";

            foreach (var row in rows)
            {
                using (var transaction = SmartTransaction.BeginTransaction())
                {

                    try
                    {
                        index++;
                        var fecha = DateTime.Parse(GetValue(row, Fields.Fecha.Value)).ToDataBaseDateTime();
                        var patente = GetValue(row, Fields.Vehiculo.Value);
                        var kmDeclarados = ValidateDouble(GetValue(row, Fields.KmDeclarados.Value), "");
                        var factura = GetValue(row, Fields.Factura.Value);
                        var proveedorDescripcion = GetValue(row, Fields.Proveedor.Value).Trim();
                        var depositoDescripcion = GetValue(row, Fields.Deposito.Value).Trim();
                        var depositoDestinoDescripcion = GetValue(row, Fields.DepositoDestino.Value).Trim();

                        Coche vehiculo = null;
                        Proveedor proveedor = null;
                        Deposito deposito = null;
                        Deposito depositoDestino = null;

                        if (patente != string.Empty)
                            vehiculo = DAOFactory.CocheDAO.GetByPatente(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, patente);
                        if (proveedorDescripcion != string.Empty)
                            proveedor = DAOFactory.ProveedorDAO.GetByDescripcion(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, proveedorDescripcion);
                        if (depositoDescripcion != string.Empty)
                            deposito = DAOFactory.DepositoDAO.GetByDescripcion(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, depositoDescripcion);
                        if (depositoDestinoDescripcion != string.Empty)
                            depositoDestino = DAOFactory.DepositoDAO.GetByDescripcion(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected},
                                depositoDestinoDescripcion);

                        var consumo = DAOFactory.ConsumoCabeceraDAO.FindByDatos(fecha, kmDeclarados, factura, vehiculo != null ? vehiculo.Id : -1,
                            proveedor != null ? proveedor.Id : -1, deposito != null ? deposito.Id : -1, depositoDestino != null ? depositoDestino.Id : -1);

                        if (consumo == null ||
                            consumo.Id == 0)
                        {
                            short tipoMovimiento = 0;

                            if (deposito != null &&
                                depositoDestino != null) tipoMovimiento = ConsumoCabecera.TiposMovimiento.DepositoADeposito;
                            if (deposito != null &&
                                vehiculo != null) tipoMovimiento = ConsumoCabecera.TiposMovimiento.DepositoAVehiculo;
                            if (proveedor != null &&
                                depositoDestino != null) tipoMovimiento = ConsumoCabecera.TiposMovimiento.ProveedorADeposito;
                            if (proveedor != null &&
                                vehiculo != null) tipoMovimiento = ConsumoCabecera.TiposMovimiento.ProveedorAVehiculo;

                            consumo = new ConsumoCabecera
                                      {
                                          Fecha = fecha,
                                          KmDeclarados = kmDeclarados,
                                          NumeroFactura = factura,
                                          ImporteTotal = 0.0,
                                          Estado = ConsumoCabecera.Estados.Pagado,
                                          Vehiculo = vehiculo,
                                          Proveedor = proveedor,
                                          Deposito = deposito,
                                          DepositoDestino = depositoDestino,
                                          TipoMovimiento = tipoMovimiento
                                      };

                            if (FaltanDatosCabecera(consumo))
                            {
                                sinProcesar++;
                                Log("FALTAN DATOS", index, consumo);
                                continue;
                            }

                            if (SobranDatosCabecera(consumo))
                            {
                                sinProcesar++;
                                Log("SOBRAN DATOS", index, consumo);
                                continue;
                            }

                            if (
                                !DAOFactory.ConsumoCabeceraDAO.IsFacturaUnique(consumo.NumeroFactura, consumo.Proveedor != null ? consumo.Proveedor.Id : -1,
                                    consumo.Deposito != null ? consumo.Deposito.Id : -1, consumo.Id))
                            {
                                sinProcesar++;
                                Log("FACTURA EXISTENTE", index, consumo);
                                continue;
                            }
                        }

                        var cantidad = GetValue(row, Fields.Cantidad.Value);
                        var importeUnitario = GetValue(row, Fields.ImporteUnitario.Value);
                        var importeTotal = GetValue(row, Fields.ImporteTotal.Value);
                        var insumoDescripcion = GetValue(row, Fields.Insumo.Value).Trim();

                        var insumo = DAOFactory.InsumoDAO.GetByDescripcion(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {-1}, insumoDescripcion);

                        double cant, unit, tot;

                        if (double.TryParse(cantidad, out cant) &&
                            double.TryParse(importeUnitario, out unit) &&
                            double.TryParse(importeTotal, out tot))
                        {
                            if (tot > 0 &&
                                cant > 0) unit = tot/cant;
                            else if (tot == 0 &&
                                     unit > 0 &&
                                     cant > 0) tot = unit*cant;

                            var detalle = new ConsumoDetalle {Cantidad = cant, ImporteUnitario = unit, ImporteTotal = tot, Insumo = insumo};
                            consumo.ImporteTotal = consumo.ImporteTotal + detalle.ImporteTotal;


                            DAOFactory.ConsumoCabeceraDAO.SaveOrUpdate(consumo);

                            if (FaltanDatosDetalle(detalle))
                            {
                                sinProcesar++;
                                Log("FALTAN DATOS", index, consumo);
                                transaction.Rollback();
                                continue;
                            }

                            if (deposito != null &&
                                SinStock(deposito.Id, insumo.Id, cant))
                            {
                                sinProcesar++;
                                Log("SIN STOCK", index, consumo);
                                transaction.Rollback();
                                continue;
                            }

                            try
                            {
                                detalle.ConsumoCabecera = consumo;
                                consumo.Detalles.Add(detalle);
                                DAOFactory.ConsumoCabeceraDAO.SaveOrUpdate(consumo);

                                var oInsumo = detalle.Insumo;
                                oInsumo.ValorReferencia = detalle.ImporteUnitario;
                                DAOFactory.InsumoDAO.SaveOrUpdate(oInsumo);

                                if (consumo.Deposito != null)
                                {
                                    var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(consumo.Deposito.Id, detalle.Insumo.Id);
                                    stock.Cantidad -= detalle.Cantidad;

                                    DAOFactory.StockDAO.SaveOrUpdate(stock);

                                    if (stock.AlarmaActiva)
                                    {
                                        var msgSaver = new MessageSaver(DAOFactory);

                                        if (stock.Cantidad < stock.StockCritico)
                                        {
                                            msgSaver.Save(MessageIdentifier.StockCritic.ToString("d"), consumo.Vehiculo, consumo.Fecha, null,
                                                "Alarma Stock Crítico");
                                        }
                                        else if (stock.Cantidad < stock.PuntoReposicion)
                                        {
                                            msgSaver.Save(MessageIdentifier.StockReposition.ToString("d"), consumo.Vehiculo, consumo.Fecha, null,
                                                "Alarma Reposición de Stock");
                                        }
                                    }
                                }
                                if (consumo.DepositoDestino != null)
                                {
                                    var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(consumo.DepositoDestino.Id, detalle.Insumo.Id) ??
                                                new Stock {Deposito = consumo.DepositoDestino, Insumo = detalle.Insumo};
                                    stock.Cantidad += detalle.Cantidad;

                                    DAOFactory.StockDAO.SaveOrUpdate(stock);
                                }

                                procesados++;

                                if (vehiculo != null) DAOFactory.MovOdometroVehiculoDAO.ResetByVehicleAndInsumo(vehiculo, detalle.Insumo);

                                transaction.Commit();
                            }
                            catch (Exception)
                            {
                                erroneos++;
                                Log("FILA ERRONEA", index, consumo);
                                transaction.Rollback();
                            }

                        }
                        else
                        {
                            erroneos++;
                            Log("FILA ERRONEA", index, consumo);
                        }
                    }
                    catch (Exception)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, null);
                    }
                }
            }
            const int totalWidth = 200;
            var percent = index*100/rows.Count;
            litProgress.Text = string.Format(@"<div style='margin: auto; border: solid 1px #999999; background-color: #FFFFFF; width: {0}px; height: 10px;'>
                <div style='background-color: #0000AA; width: {1}px; height: 10px; font-size: 8px; color: #CCCCCC;'>{2}%</div>
                </div>", totalWidth, percent*totalWidth/100, percent);
            lblDirs.Text = string.Format(Reporte, procesados + erroneos, rows.Count, procesados,
                rows.Count > 0 ? (procesados*100.0/rows.Count).ToString("0.00") : "0.00", erroneos,
                rows.Count > 0 ? (erroneos*100.0/rows.Count).ToString("0.00") : "0.00", sinProcesar,
                rows.Count > 0 ? (sinProcesar*100.0/rows.Count).ToString("0.00") : "0.00", TimeSpan.FromTicks(DateTime.Now.Ticks - start));
            panelProgress.Visible = true;
        }

        protected void ImportEssoCard(List<ImportRow> rows)
        {
            var procesados = 0;
            var erroneos = 0;
            var sinProcesar = 0;
            var index = 0;
            var start = DateTime.Now.Ticks;
            lblResult.Text = "";

            foreach (var row in rows)
            {
                using (var transaction = SmartTransaction.BeginTransaction())
                {
                    try
                    {
                        index++;
                        var fecha = DateTime.Parse(GetValue(row, Fields.Fecha.Value));
                        var horario = GetValue(row, Fields.Hora.Value);
                        if (horario.Length == 5) horario = string.Concat("0", horario);
                        var hora = int.Parse(horario.Substring(0, 2));
                        var min = int.Parse(horario.Substring(2, 2));
                        fecha = fecha.AddHours(hora).AddMinutes(min).ToDataBaseDateTime();

                        var patente = GetValue(row, Fields.Driver.Value);
                        var kmDeclarados = ValidateDouble(GetValue(row, Fields.Odometro.Value), "");
                        var factura = GetValue(row, Fields.NroAut.Value);

                        var vehiculo = DAOFactory.CocheDAO.GetByPatente(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, patente);
                        var proveedor = DAOFactory.ProveedorDAO.GetByDescripcion(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, "Generico");

                        var consumo = DAOFactory.ConsumoCabeceraDAO.FindByDatos(fecha, kmDeclarados, factura, vehiculo != null ? vehiculo.Id : -1,
                            proveedor != null ? proveedor.Id : -1, -1, -1);

                        if (consumo == null ||
                            consumo.Id == 0)
                        {
                            consumo = new ConsumoCabecera
                                      {
                                          Fecha = fecha,
                                          KmDeclarados = kmDeclarados,
                                          NumeroFactura = factura,
                                          ImporteTotal = 0.0,
                                          Estado = ConsumoCabecera.Estados.Pagado,
                                          Vehiculo = vehiculo,
                                          Proveedor = proveedor,
                                          TipoMovimiento = ConsumoCabecera.TiposMovimiento.ProveedorAVehiculo
                                      };

                            if (FaltanDatosCabecera(consumo))
                            {
                                sinProcesar++;
                                Log("FALTAN DATOS", index, consumo);
                                continue;
                            }

                            if (SobranDatosCabecera(consumo))
                            {
                                sinProcesar++;
                                Log("SOBRAN DATOS", index, consumo);
                                continue;
                            }

                            if (!DAOFactory.ConsumoCabeceraDAO.IsFacturaUnique(consumo.NumeroFactura, consumo.Proveedor.Id, -1, consumo.Id))
                            {
                                sinProcesar++;
                                Log("FACTURA EXISTENTE", index, consumo);
                                continue;
                            }
                        }

                        var cantidad = GetValue(row, Fields.Cantidad.Value);
                        var importeTotal = GetValue(row, Fields.Valor.Value);
                        var insumoDescripcion = GetValue(row, Fields.Prod.Value).Trim();

                        var insumo = DAOFactory.InsumoDAO.GetByDescripcion(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {-1}, insumoDescripcion);

                        double cant, tot;

                        if (double.TryParse(cantidad, out cant) &&
                            double.TryParse(importeTotal, out tot))
                        {
                            var unit = tot/cant;

                            var detalle = new ConsumoDetalle {Cantidad = cant, ImporteUnitario = unit, ImporteTotal = tot, Insumo = insumo};
                            consumo.ImporteTotal = consumo.ImporteTotal + detalle.ImporteTotal;


                            DAOFactory.ConsumoCabeceraDAO.SaveOrUpdate(consumo);

                            if (FaltanDatosDetalle(detalle))
                            {
                                sinProcesar++;
                                Log("FALTAN DATOS", index, consumo);
                                transaction.Rollback();
                                continue;
                            }

                            try
                            {
                                detalle.ConsumoCabecera = consumo;
                                consumo.Detalles.Add(detalle);
                                DAOFactory.ConsumoCabeceraDAO.SaveOrUpdate(consumo);

                                var oInsumo = detalle.Insumo;
                                oInsumo.ValorReferencia = detalle.ImporteUnitario;
                                DAOFactory.InsumoDAO.SaveOrUpdate(oInsumo);

                                procesados++;

                                if (vehiculo != null) DAOFactory.MovOdometroVehiculoDAO.ResetByVehicleAndInsumo(vehiculo, detalle.Insumo);

                                transaction.Commit();
                            }
                            catch (Exception)
                            {
                                erroneos++;
                                Log("FILA ERRONEA", index, consumo);
                                transaction.Rollback();
                            }
                        }
                        else
                        {
                            erroneos++;
                            Log("FILA ERRONEA", index, consumo);
                        }
                    }
                    catch (Exception)
                    {
                        erroneos++;
                        Log("FILA ERRONEA", index, null);
                    }
                }
            }

            const int totalWidth = 200;
            var percent = index * 100 / rows.Count;
            litProgress.Text = string.Format(@"<div style='margin: auto; border: solid 1px #999999; background-color: #FFFFFF; width: {0}px; height: 10px;'>
                <div style='background-color: #0000AA; width: {1}px; height: 10px; font-size: 8px; color: #CCCCCC;'>{2}%</div>
                </div>", totalWidth, percent * totalWidth / 100, percent);
            lblDirs.Text = string.Format(Reporte,
                                         procesados + erroneos,
                                         rows.Count,
                                         procesados,
                                         rows.Count > 0 ? (procesados * 100.0 / rows.Count).ToString("0.00") : "0.00",
                                         erroneos,
                                         rows.Count > 0 ? (erroneos * 100.0 / rows.Count).ToString("0.00") : "0.00",
                                         sinProcesar,
                                         rows.Count > 0 ? (sinProcesar * 100.0 / rows.Count).ToString("0.00") : "0.00",
                                         TimeSpan.FromTicks(DateTime.Now.Ticks - start));
            panelProgress.Visible = true;
        }

        protected void ImportYpfEnRuta(List<ImportRow> rows)
        {
            ImportDefault(rows);
        }

        protected void ImportEdenred(List<ImportRow> rows)
        {
            ImportDefault(rows);
        }

        private bool SinStock(int idDeposito, int idInsumo, double cant)
        {
            var stock = DAOFactory.StockDAO.GetByDepositoAndInsumo(idDeposito, idInsumo);

            return (stock == null || stock.Cantidad < cant);
        }

        private static bool FaltanDatosCabecera(ConsumoCabecera consumo)
        {
            return (consumo.Deposito == null && consumo.Proveedor == null) || (consumo.Vehiculo == null && consumo.DepositoDestino == null);
        }

        private static bool SobranDatosCabecera(ConsumoCabecera consumo)
        {
            return (consumo.Deposito != null && consumo.Proveedor != null) || (consumo.Vehiculo != null && consumo.DepositoDestino != null);
        }

        private static bool FaltanDatosDetalle(ConsumoDetalle detalle)
        {
            return (detalle.Insumo == null);
        }

        private void Log(string status, int index, ConsumoCabecera cons)
        {
            lblResult.Text += string.Format(Result, 
                                            new[] { status, 
                                                    " INDICE: " + index, 
                                                    " Vehiculo = " + ((cons != null && cons.Vehiculo != null) ? cons.Vehiculo.ToString() : "VACIO"), 
                                                    " Proveedor = " + (( cons != null && cons.Proveedor != null) ? cons.Proveedor.ToString() : "VACIO"),
                                                    " Deposito = " + (( cons != null && cons.Deposito != null) ? cons.Deposito.ToString() : "VACIO"),
                                                    " Deposito Destino = " + (( cons != null && cons.DepositoDestino != null) ? cons.DepositoDestino.ToString() : "VACIO")
                                                  });
        }

        #region SubClasses

        private static class Fields
        {
            public static readonly FieldValue Fecha = new FieldValue("Fecha");
            public static readonly FieldValue KmDeclarados = new FieldValue("Km. Declarados");
            public static readonly FieldValue Factura = new FieldValue("Factura");
            public static readonly FieldValue Vehiculo = new FieldValue("Vehículo");
            public static readonly FieldValue Proveedor = new FieldValue("Proveedor");
            public static readonly FieldValue Insumo = new FieldValue("Insumo");
            public static readonly FieldValue Cantidad = new FieldValue("Cantidad");
            public static readonly FieldValue ImporteUnitario = new FieldValue("Importe Unitario");
            public static readonly FieldValue ImporteTotal = new FieldValue("Importe Total");
            public static readonly FieldValue Deposito = new FieldValue("Depósito");
            public static readonly FieldValue DepositoDestino = new FieldValue("Depósito Destino");

            public static readonly FieldValue Site = new FieldValue("Site");
            public static readonly FieldValue Nombre = new FieldValue("Nombre");
            public static readonly FieldValue Hora = new FieldValue("Hora");
            public static readonly FieldValue Tarjeta = new FieldValue("Tarjeta");
            public static readonly FieldValue TotalCupon = new FieldValue("Total Cupón");
            public static readonly FieldValue NroAut = new FieldValue("Nro. Aut.");
            public static readonly FieldValue Cupon = new FieldValue("Cupón");
            public static readonly FieldValue Odometro = new FieldValue("Odómetro");
            public static readonly FieldValue Driver = new FieldValue("Driver");
            public static readonly FieldValue Prod = new FieldValue("Prod");
            public static readonly FieldValue Tax = new FieldValue("Tax");
            public static readonly FieldValue Valor = new FieldValue("Valor");
            public static readonly FieldValue CodServ = new FieldValue("Cod. Serv.");

            public static List<FieldValue> Default
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Fecha,
                                   KmDeclarados,
                                   Factura,
                                   Vehiculo,
                                   Proveedor,
                                   Insumo,
                                   Cantidad,
                                   ImporteUnitario,
                                   ImporteTotal,
                                   Deposito,
                                   DepositoDestino
                               };
                }
            }

            public static List<FieldValue> EssoCard
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Site,
                                   Nombre,
                                   Fecha,
                                   Hora,
                                   Tarjeta,
                                   TotalCupon,
                                   NroAut,
                                   Cupon,
                                   Odometro,
                                   Driver,
                                   Vehiculo,
                                   Prod,
                                   Tax,
                                   Cantidad,
                                   Valor,
                                   CodServ
                               };
                }
            }

            public static List<FieldValue> YpfEnRuta
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Fecha,
                                   KmDeclarados,
                                   Factura,
                                   Vehiculo,
                                   Proveedor,
                                   Insumo,
                                   Cantidad,
                                   ImporteUnitario,
                                   ImporteTotal,
                                   Deposito,
                                   DepositoDestino
                               };
                }
            }

            public static List<FieldValue> Edenred
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Fecha,
                                   KmDeclarados,
                                   Factura,
                                   Vehiculo,
                                   Proveedor,
                                   Insumo,
                                   Cantidad,
                                   ImporteUnitario,
                                   ImporteTotal,
                                   Deposito,
                                   DepositoDestino
                               };
                }
            }

            public static List<FieldValue> ObligatoriosDefault
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Fecha,
                                   KmDeclarados,
                                   Factura,
                                   Insumo,
                                   Cantidad,
                                   ImporteUnitario,
                                   ImporteTotal
                               };
                }
            }

            public static List<FieldValue> ObligatoriosEssoCard
            {
                get
                {
                    return new List<FieldValue>
                               {
                                   Fecha,
                                   Hora,
                                   Driver,
                                   Odometro,
                                   NroAut,
                                   Cantidad,
                                   Valor,
                                   Prod
                               };
                }
            }
        }

        #endregion
    }
}