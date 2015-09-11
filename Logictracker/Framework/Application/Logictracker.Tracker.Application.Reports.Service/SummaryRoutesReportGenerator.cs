using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class SummaryRoutesReportGenerator
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.Templates.SummaryRoutesReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Estado= 0;
        private const int Ruta = 1;
        private const int Vehiculo = 2;
        private const int Inicio = 3;
        private const int Recepción = 4;
        private const int Entregas = 5;
        private const int Realizados = 6;
        private const int Visitados = 7;
        private const int Rechazados = 8;
        private const int EnSitio = 9;
        private const int EnZona = 10;
        private const int Porcentaje = 11;
        private const int Kms = 12;
        private const int Recorrido = 13;

        public static Stream GenerateReport(List<ResumenDeRutasVo> results, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
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

                    row.CreateCell(Estado).SetCellValue(data.Estado);
                    row.CreateCell(Ruta).SetCellValue(data.Ruta);
                    row.CreateCell(Vehiculo).SetCellValue(data.Vehiculo);
                    row.CreateCell(Inicio).SetCellValue(data.Inicio.ToString());
                    row.CreateCell(Recepción).SetCellValue(data.Recepcion.ToString());
                    row.CreateCell(Entregas).SetCellValue(data.Entregas);
                    row.CreateCell(Realizados).SetCellValue(data.Realizados);
                    row.CreateCell(Visitados).SetCellValue(data.Visitados);
                    row.CreateCell(Rechazados).SetCellValue(data.Rechazados);
                    row.CreateCell(EnSitio).SetCellValue(data.EnSitio);
                    row.CreateCell(EnZona).SetCellValue(data.EnZona);
                    row.CreateCell(Porcentaje).SetCellValue(data.PorcVisitados);
                    row.CreateCell(Kms).SetCellValue(data.Kms); 
                    row.CreateCell(Recorrido).SetCellValue(data.Recorrido.ToString());
                    
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