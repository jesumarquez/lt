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
    public class VehicleActivityReportGenerator 
    {
         private const string TemplateName = "Logictracker.Tracker.Application.Reports.Templates.VehicleActivityReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int TipoVehiculo = 0; //A
        private const int CentroCostos = 1;
        private const int Vehiculo = 2;
        private const int Patente = 3;
        private const int Kilometros = 4;
        private const int Costo	= 5;
        private const int Movimiento = 6;
        private const int Detencion	= 7;
        private const int Infracciones = 8;
        private const int Leves = 9;
        private const int Medias = 10;
        private const int Graves = 11;
        private const int TiempoInfraccion = 12;
        private const int VelocidadPromedio = 13;
        private const int VelocidadMaxima = 14;

        public static Stream GenerateReport(List<MobileActivity> mobileActivities, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
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
                foreach (var mobAct in mobileActivities)
                {
                    row = dataSheetInfo.GetRow(rowCounter) ?? dataSheetInfo.CreateRow(rowCounter);

                    row.CreateCell(TipoVehiculo).SetCellValue(mobAct.TipoVehiculo);
                    row.CreateCell(CentroCostos).SetCellValue(mobAct.CentroDeCostos);
                    row.CreateCell(Vehiculo).SetCellValue(mobAct.Movil);
                    row.CreateCell(Patente).SetCellValue(mobAct.Patente);
                    row.CreateCell(Kilometros).SetCellValue(mobAct.Recorrido);
                    row.CreateCell(Costo).SetCellValue("No disponible"); //HACER CONSULTA
                    row.CreateCell(Movimiento).SetCellValue(String.Format("Días:{0} - Horas {1}:{2}:{3}", mobAct.HorasActivo.Days, mobAct.HorasActivo.Hours, mobAct.HorasActivo.Minutes, mobAct.HorasActivo.Seconds));
                    row.CreateCell(Detencion).SetCellValue(String.Format("Días:{0} - Horas {1}:{2}:{3}", mobAct.HorasDetenido.Days, mobAct.HorasDetenido.Hours, mobAct.HorasDetenido.Minutes, mobAct.HorasDetenido.Seconds));
                    row.CreateCell(Infracciones).SetCellValue(mobAct.Infracciones);
                    row.CreateCell(Leves).SetCellValue("N/D"); //que es esto
                    row.CreateCell(Medias).SetCellValue("N/D"); //que es esto
                    row.CreateCell(Graves).SetCellValue("N/D"); //que es esto
                    row.CreateCell(TiempoInfraccion).SetCellValue(String.Format("Días:{0} - Horas {1}:{2}:{3}", mobAct.HorasInfraccion.Days, mobAct.HorasInfraccion.Hours, 	mobAct.HorasInfraccion.Minutes, mobAct.HorasInfraccion.Seconds));
                    row.CreateCell(VelocidadPromedio).SetCellValue(mobAct.VelocidadPromedio);
                    row.CreateCell(VelocidadMaxima).SetCellValue(mobAct.VelocidadMaxima);
                    
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