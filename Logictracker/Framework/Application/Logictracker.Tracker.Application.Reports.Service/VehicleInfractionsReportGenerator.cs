using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ReportObjects.RankingDeOperadores;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class VehicleInfractionsReportGenerator
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.VehicleInfractionsReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Vehiculo = 0; //A
        private const int Calificacion = 1;
        private const int Chofer = 2;
        private const int Esquina = 3;
        private const int Inicio = 4;
        private const int Duracion	= 5;
        private const int Pico = 6;
        private const int Exceso	= 7;
        private const int Ponderacion = 8;

        public static Stream GenerateReport(List<InfractionDetail> results, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
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
                foreach (var inf in results)
                {
                    row = dataSheetInfo.GetRow(rowCounter) ?? dataSheetInfo.CreateRow(rowCounter);

                    row.CreateCell(Vehiculo).SetCellValue(inf.Vehiculo);
                    row.CreateCell(Calificacion).SetCellValue(inf.Calificacion);
                    row.CreateCell(Chofer).SetCellValue(inf.Operador);
                    //row.CreateCell(Esquina).SetCellValue(inf.CornerNearest);
                    row.CreateCell(Inicio).SetCellValue(inf.Inicio);
                    row.CreateCell(Duracion).SetCellValue(inf.DuracionSegundos); 
                    row.CreateCell(Pico).SetCellValue(inf.Pico);
                    row.CreateCell(Exceso).SetCellValue(inf.Exceso);
                    row.CreateCell(Ponderacion).SetCellValue(inf.Ponderacion);
                    
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