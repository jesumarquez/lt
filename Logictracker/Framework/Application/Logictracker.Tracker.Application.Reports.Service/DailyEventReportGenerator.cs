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
    public class DailyEventReportGenerator  
    {

        private const string TemplateName = "Logictracker.Tracker.Application.Reports.Templates.EventReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Grafico";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Vehiculo = 0; //A
        private const int Chofer = 1;
        private const int Responsable = 2;
        private const int Fecha = 3;
        private const int Recepcion = 4;
        private const int Duracion = 5;
        private const int Mensaje = 6;
        private const int Atendido = 7;
        private const int Usuario = 8;
        private const int FechaAtencion = 9;
        private const int MensajeAtencion = 10;
        private const int Observacion = 11;
        private const int EsquinaInicial= 12;
        private const int EsquinaFinal = 13;


        public static Stream GenerateReport(List<MobileEvent> mobileEvents, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(TemplateName))
            {
                var templateWorkbook = new HSSFWorkbook(stream, true);
                var dataSheet1 = templateWorkbook.GetSheet(DataSheet1) as HSSFSheet;
                //var dataSheet2 = templateWorkbook.GetSheet(DataSheet2) as HSSFSheet;

                if (dataSheet1 == null)
                    throw new RuntimeException(string.Format("Cannot generate report datasheet {0} not found in template {1}", DataSheet1, TemplateName));

                //if (dataSheet2 == null)
                //    throw new RuntimeException(string.Format("Cannot generate report datasheet {0} not found in template {1}", DataSheet2, TemplateName));

                //distrito
                var row = dataSheet1.GetRow(Distrito) ?? dataSheet1.CreateRow(Distrito);
                row.CreateCell(1).SetCellValue(customer.RazonSocial);
                //base
                row = dataSheet1.GetRow(Base) ?? dataSheet1.CreateRow(Base);
                row.CreateCell(1).SetCellValue(baseName);
                //desde
                row = dataSheet1.GetRow(Desde) ?? dataSheet1.CreateRow(Desde);
                row.CreateCell(3).SetCellValue(initialDate);
                //hasta
                row = dataSheet1.GetRow(Hasta) ?? dataSheet1.CreateRow(Hasta);
                row.CreateCell(3).SetCellValue(finalDate);

                int rowCounter = 5; //fila de inicio de los datos
                foreach(var eventMobile in mobileEvents)
                {
                    row = dataSheet1.GetRow(rowCounter) ?? dataSheet1.CreateRow(rowCounter);

                    row.CreateCell(Vehiculo).SetCellValue(eventMobile.MobileType + " " + eventMobile.Intern);
                    row.CreateCell(Chofer).SetCellValue(eventMobile.Driver);
                    row.CreateCell(Responsable).SetCellValue(eventMobile.Responsable);
                    row.CreateCell(Fecha).SetCellValue(eventMobile.EventTime.ToString());
                    if (eventMobile.Reception != null)
                        row.CreateCell(Recepcion).SetCellValue(eventMobile.Reception.Value);
                    row.CreateCell(Duracion).SetCellValue(eventMobile.Duration.ToString());
                    row.CreateCell(Mensaje).SetCellValue(eventMobile.Message);
                    row.CreateCell(Atendido).SetCellValue(eventMobile.Atendido ? "Si" : "No");
                    if (eventMobile.Usuario != null)
                        row.CreateCell(Usuario).SetCellValue(eventMobile.Usuario.NombreUsuario);
                    if (eventMobile.AtencionEvento != null)
                    {
                        row.CreateCell(FechaAtencion).SetCellValue(eventMobile.AtencionEvento.Fecha);
                        row.CreateCell(MensajeAtencion).SetCellValue(eventMobile.AtencionEvento.Mensaje.Descripcion);
                        row.CreateCell(Observacion).SetCellValue(eventMobile.AtencionEvento.Observacion);
                    }
                    else
                    {
                        row.CreateCell(MensajeAtencion).SetCellValue("Sin mensaje");                        
                    }
                    //if (eventMobile.InitialCross != null)
                    //    row.CreateCell(EsquinaInicial).SetCellValue(eventMobile.InitialCross);
                    //if (eventMobile.FinalCross != null)
                    //    row.CreateCell(EsquinaFinal).SetCellValue(eventMobile.FinalCross);
                    rowCounter++;
                }

                dataSheet1.ForceFormulaRecalculation = true;
                //dataSheet2.ForceFormulaRecalculation = true;
                var memoryStream = new MemoryStream();
                templateWorkbook.Write(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}