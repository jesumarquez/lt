using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class DeliverStatusReportGenerator   
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.DeliverStatusReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //																	

        //columnas
        private const int Ruta = 0;
        private const int TipoVehIculo = 1;
        private const int Vehiculo = 2;
        private const int Empleado = 3;
        private const int Fecha = 4;
        private const int Orden = 5;
        private const int OrdenReal = 6;
        private const int Cliente = 7;
        private const int Descripcion = 8;
        private const int Manual = 9;
        private const int Entrada = 10;
        private const int Salida = 11;
        private const int Duracion = 12;
        private const int Km = 13;
        private const int Estado = 14;
        private const int Confirmacion = 15;
        private const int Horario = 16;
        private const int Recepcion = 17;
        private const int Lectura = 18;

        public static Stream GenerateReport(List<ReporteDistribucionVo> results, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(TemplateName))
            {
                var templateWorkbook = new HSSFWorkbook(stream, true);
                var dataSheetInfo = templateWorkbook.GetSheet(DataSheet1) as HSSFSheet;
                //var dataSheetData = templateWorkbook.GetSheet(DataSheet2) as HSSFSheet;

                if (dataSheetInfo == null)
                    throw new RuntimeException(
                        string.Format("Cannot generate report datasheet {0} not found in template {1}", DataSheet1,
                            TemplateName));

                //distrito
                var row = dataSheetInfo.GetRow(Distrito) ?? dataSheetInfo.CreateRow(Distrito);
                row.CreateCell(1).SetCellValue(customer.RazonSocial);
                //base
                row = dataSheetInfo.GetRow(Base) ?? dataSheetInfo.CreateRow(Base);
                row.CreateCell(1).SetCellValue(baseName);
                //desde
                row = dataSheetInfo.GetRow(Desde) ?? dataSheetInfo.CreateRow(Desde);
                row.CreateCell(3).SetCellValue(initialDate);
                //hasta
                row = dataSheetInfo.GetRow(Hasta) ?? dataSheetInfo.CreateRow(Hasta);
                row.CreateCell(3).SetCellValue(finalDate);

                int rowCounter = 5; //fila de inicio de los datos
                foreach (var data in results)
                {
                    row = dataSheetInfo.GetRow(rowCounter) ?? dataSheetInfo.CreateRow(rowCounter);

                    row.CreateCell(Ruta).SetCellValue(data.Ruta);
                    row.CreateCell(TipoVehIculo).SetCellValue(data.TipoVehiculo);
                    row.CreateCell(Vehiculo).SetCellValue(data.Vehiculo);
                    row.CreateCell(Empleado).SetCellValue(data.Empleado);
                    row.CreateCell(Fecha).SetCellValue(data.Fecha);
                    row.CreateCell(Orden).SetCellValue(data.Orden);
                    row.CreateCell(OrdenReal).SetCellValue(data.OrdenReal);
                    row.CreateCell(Cliente).SetCellValue("");
                    row.CreateCell(Descripcion).SetCellValue(data.Descripcion);
                    row.CreateCell(Manual).SetCellValue(data.Manual);
                    row.CreateCell(Entrada).SetCellValue(data.Entrada);
                    row.CreateCell(Salida).SetCellValue(data.Salida);
                    row.CreateCell(Duracion).SetCellValue(data.Duracion); 
                    row.CreateCell(Km).SetCellValue(data.Km);
                    row.CreateCell(Estado).SetCellValue(data.Estado);
                    row.CreateCell(Confirmacion).SetCellValue(data.Confirmacion);
                    row.CreateCell(Horario).SetCellValue(data.Horario.ToString());
                    row.CreateCell(Recepcion).SetCellValue(data.DateManual.ToString());
                    row.CreateCell(Lectura).SetCellValue(data.DateConfirmacion.ToString());
                    
                    //
                    rowCounter++;
                }

                var memoryStream = new MemoryStream();
                templateWorkbook.Write(memoryStream);
                memoryStream.Position = 0;

                //dataSheetData.ForceFormulaRecalculation = true;
                dataSheetInfo.ForceFormulaRecalculation = true;

                return memoryStream;
            }
        }
    }
}