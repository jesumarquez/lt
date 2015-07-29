using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ReportObjects.RankingDeOperadores;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class DocumentsExpirationReportGenerator 
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.DocumentsExpirationReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Documento = 0;
        private const int Vehiculo = 1; //A
        private const int Codigo= 2;
        private const int Descripcion = 3;
        private const int Vencimiento = 4;
        private const int DiasVencido = 5;

        public static Stream GenerateReport(IList<Documento> results, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
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
                foreach (var doc in results)
                {
                    row = dataSheetInfo.GetRow(rowCounter) ?? dataSheetInfo.CreateRow(rowCounter);

                    row.CreateCell(Documento).SetCellValue(doc.TipoDocumento.Descripcion);
                    row.CreateCell(Vehiculo).SetCellValue(doc.Vehiculo != null ? doc.Vehiculo.Interno : string.Empty);
                    row.CreateCell(Codigo).SetCellValue(doc.Codigo);
                    row.CreateCell(Descripcion).SetCellValue(doc.Descripcion);

                    if (doc.Vencimiento.HasValue)
                    {
                        row.CreateCell(Vencimiento).SetCellValue(doc.Vencimiento.Value);
                        row.CreateCell(DiasVencido).SetCellValue(Convert.ToInt32(doc.Vencimiento.Value.Subtract(DateTime.Now).TotalDays));
                    }
                    else
                    {
                        row.CreateCell(Vencimiento).SetCellValue(DateTime.MaxValue);
                        row.CreateCell(DiasVencido).SetCellValue(9999);
                    }


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