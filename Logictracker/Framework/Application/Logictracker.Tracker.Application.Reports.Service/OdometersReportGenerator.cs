using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using NPOI.HSSF.UserModel;
using NPOI.Util;

namespace Logictracker.Tracker.Application.Reports
{
    public class OdometersReportGenerator
    {
        private const string TemplateName = "Logictracker.Tracker.Application.Reports.Templates.OdometersReport.xls";
        private const string DataSheet1 = "Informe";
        //private const string DataSheet2 = "Datos";

        //cabecera
        private const int Distrito = 2;
        private const int Base = 3;
        private const int Desde = 2;
        private const int Hasta = 3;

        //columnas
        private const int Interno = 0; //A
        //private const int TipoVehiculo = 1;
        private const int CentroCostos = 1;
        private const int Responsable = 2;
        private const int Referencia = 3;
        private const int Tipo = 4;
        private const int KilometrosReferencia = 5;
        private const int HorasReferencia = 6;
        private const int KilometrosFaltantes = 7;
        private const int DiasFaltantes = 8;
        //private const int HorasFaltantes = 8;
        //private const int KmTotales = 11;
        private const int UltimaActualizacion = 9;

        public static Stream GenerateReport(IEnumerable<OdometroStatus> odometers, Empresa customer, DateTime initialDate, DateTime finalDate, string baseName)
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
                foreach (var odom in odometers)
                {
                    row = dataSheetInfo.GetRow(rowCounter) ?? dataSheetInfo.CreateRow(rowCounter);

                    //row.CreateCell(TipoVehiculo).SetCellValue("Movil");
                    row.CreateCell(CentroCostos).SetCellValue(odom.CentroDeCosto);
                    row.CreateCell(Interno).SetCellValue(odom.Tipo + " " + odom.Interno);
                    row.CreateCell(Responsable).SetCellValue(odom.Responsable);
                    row.CreateCell(Referencia).SetCellValue(odom.Referencia);
                    row.CreateCell(Tipo).SetCellValue(odom.Odometro);
                    if (odom.KilometrosReferencia.HasValue)
                        row.CreateCell(KilometrosReferencia).SetCellValue(odom.KilometrosReferencia.Value);
                    //row.CreateCell(DiasReferencia).SetCellValue("0");
                    //if (odom.HorasFaltantes.HasValue)
                       // row.CreateCell(HorasFaltantes).SetCellValue(odom.HorasFaltantes.Value);
                    if (odom.KilometrosFaltantes.HasValue)
                        row.CreateCell(KilometrosFaltantes).SetCellValue(odom.KilometrosFaltantes.Value);
                    if (odom.HorasReferencia.HasValue)
                        row.CreateCell(HorasReferencia).SetCellValue(odom.HorasReferencia.Value);
                    if (odom.TiempoFaltante.HasValue)
                        row.CreateCell(DiasFaltantes).SetCellValue(odom.TiempoFaltante.Value);
                    //row.CreateCell(KmTotales).SetCellValue("0"); 
                    row.CreateCell(UltimaActualizacion).SetCellValue(odom.UltimoUpdate);
                    
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