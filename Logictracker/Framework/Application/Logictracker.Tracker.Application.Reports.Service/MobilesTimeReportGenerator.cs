using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class MobilesTimeReportGenerator
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.MobilesTimeReport.xls";
        private const string DataSheet1 = "Informe";
        private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Vehiculo = 0; //A
        private const int Horas = 1;

        public static Stream GenerateReport(List<MobilesTime> mobilesTime, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(TemplateName))
            {
                var templateWorkbook = new HSSFWorkbook(stream, true);
                var dataSheetInfo = templateWorkbook.GetSheet(DataSheet1) as HSSFSheet;
                var dataSheetData = templateWorkbook.GetSheet(DataSheet2) as HSSFSheet;

                if ((dataSheetData == null) || ((dataSheetInfo == null)))
                    throw new RuntimeException(
                        string.Format("Cannot generate report datasheet {0} not found in template {1}", DataSheet2,
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

                int rowCounter = 1; //fila de inicio de los datos
                foreach (var mt in mobilesTime)
                {
                    row = dataSheetData.GetRow(rowCounter) ?? dataSheetData.CreateRow(rowCounter);

                    row.CreateCell(Vehiculo).SetCellValue(mt.Intern);
                    row.CreateCell(Horas).SetCellValue(mt.ElapsedTime);

                    rowCounter++;
                }

                var memoryStream = new MemoryStream();
                templateWorkbook.Write(memoryStream);
                memoryStream.Position = 0;

                dataSheetData.ForceFormulaRecalculation = true;
                dataSheetInfo.ForceFormulaRecalculation = true;

                return memoryStream;
            }
        }
    }
}