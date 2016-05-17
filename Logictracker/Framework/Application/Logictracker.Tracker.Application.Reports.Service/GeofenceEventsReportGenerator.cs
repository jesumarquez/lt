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
    public class GeofenceEventsReportGenerator  
    {
         private const string TemplateName = "Logictracker.Tracker.Application.Reports.Templates.GeofenceEventsReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Vehiculo = 0;
        private const int Geocerca = 1;
        private const int Tipo = 2; //A
        private const int Inicio = 3;
        private const int Fin = 4;
        private const int Duracion = 5;
        private const int ProximaGeocerca	 = 6;
        private const int Recorrido = 7;

        public static Stream GenerateReport(List<MobileGeocerca> results, Empresa customer, DateTime initialDate,
            DateTime finalDate, string baseName)
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
                foreach (var geo in results)
                {
                    row = dataSheetInfo.GetRow(rowCounter) ?? dataSheetInfo.CreateRow(rowCounter);

                    row.CreateCell(Vehiculo).SetCellValue(geo.Interno);
                    row.CreateCell(Geocerca).SetCellValue(geo.Geocerca);
                    row.CreateCell(Tipo).SetCellValue(geo.TipoGeocerca);
                    row.CreateCell(Inicio).SetCellValue(geo.Entrada.Equals(DateTime.MinValue)? string.Empty : string.Format("{0} {1}", geo.Entrada.ToShortDateString(),geo.Entrada.TimeOfDay));
                    row.CreateCell(Fin).SetCellValue(geo.Salida.Equals(DateTime.MinValue) ? string.Empty : string.Format("{0} {1}", geo.Salida.ToShortDateString(), geo.Salida.TimeOfDay));
                    row.CreateCell(Duracion).SetCellValue(geo.Duracion.Equals(TimeSpan.MinValue) ? new TimeSpan().ToString() : geo.Duracion.ToString());
                    row.CreateCell(ProximaGeocerca).SetCellValue(geo.ProximaGeocerca.Equals(TimeSpan.MinValue) || geo.ProximaGeocerca < TimeSpan.Zero ? new TimeSpan().ToString() : geo.ProximaGeocerca.ToString());
                    row.CreateCell(Recorrido).SetCellValue(geo.ProximaGeocerca.TotalSeconds > 0.0 ? geo.Recorrido : 0.0);

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