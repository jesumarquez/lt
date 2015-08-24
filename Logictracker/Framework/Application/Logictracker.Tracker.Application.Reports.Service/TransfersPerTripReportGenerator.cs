using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class TransfersPerTripReportGenerator
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.TransfersPerTripReport.xls";
        private const string DataSheet1 = "Informe";
        
        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Transportista = 0; //A
        private const int Vehiculo = 1;
        private const int Fecha = 2;
        private const int Viaje = 3;
        private const int KmReales = 4;
        private const int Productivos = 5;
        private const int Improductivos = 6;
        private const int Programado = 7;
        private const int DifKm	 = 8;
        private const int PorcentajeKm = 9;
        private const int TiempoReal = 10;
        private const int TiempoProgramado = 11;
        private const int DiferenciaTiempo = 12;
        private const int PorcentajeMin = 13;


        public static Stream GenerateReport(List<ReporteTrasladoVo> results, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
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

                    row.CreateCell(Transportista).SetCellValue(data.Transportista);
                    row.CreateCell(Vehiculo).SetCellValue(data.Vehiculo);
                    row.CreateCell(Fecha).SetCellValue(data.Fecha);
                    row.CreateCell(Viaje).SetCellValue(data.Viaje);
                    row.CreateCell(KmReales).SetCellValue(data.KmReales);
                    row.CreateCell(Productivos).SetCellValue(data.KmProductivos);
                    row.CreateCell(Improductivos).SetCellValue(data.KmImproductivos);
                    row.CreateCell(Programado).SetCellValue(data.KmProgramado);
                    row.CreateCell(DifKm).SetCellValue(data.DiferenciaKm);
                    row.CreateCell(PorcentajeKm).SetCellValue(data.PorcDiferenciaKm);
                    row.CreateCell(TiempoReal).SetCellValue(data.TiempoReal.ToString());
                    row.CreateCell(TiempoProgramado).SetCellValue(data.TiempoProgramado.ToString());
                    row.CreateCell(DiferenciaTiempo).SetCellValue(data.DiferenciaTiempo.ToString()); 
                    row.CreateCell(PorcentajeMin).SetCellValue(data.PorcDiferenciaTiempo);
                    
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