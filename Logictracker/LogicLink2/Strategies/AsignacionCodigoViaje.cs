using LinqToExcel;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class AsignacionCodigoViaje : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Linea Linea { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<ViajeDistribucion> _viajesBuffer = new List<ViajeDistribucion>();

        public static void Parse(LogicLinkFile file, out int viajes, out string observaciones)
        {
            new AsignacionCodigoViaje(file).Parse(out viajes, out observaciones);
        }

        public AsignacionCodigoViaje(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Linea = file.Linea;
        }

        public void Parse(out int viajes, out string observaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRows(rows);            
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));
            
            var listViajes = new List<ViajeDistribucion>(rows.Count);
            viajes = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var codigo = row[Properties.AsignacionCodigoViaje.Codigo].ToString().Trim();
                var viaje = row[Properties.AsignacionCodigoViaje.Viaje].ToString().Trim();
                var oViaje = _viajesBuffer.SingleOrDefault(v => v.Codigo == viaje);

                if (oViaje == null)
                {
                    observaciones = "Valor inválido para el campo VIAJE";
                    continue;
                }

                oViaje.Codigo = codigo;
                listViajes.Add(oViaje);
                viajes++;                
            }

            STrace.Trace(Component, "Guardando viajes: " + listViajes.Count);
            te.Restart();
            var i = 1;
            foreach (var viaje in listViajes)
            {
                STrace.Trace(Component, string.Format("Guardando: {0}/{1}", i++, listViajes.Count()));
                DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
            }
            STrace.Trace(Component, string.Format("Viajes guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        private void PreBufferRows(IEnumerable<Row> rows)
        {
            var lastCodViaje = string.Empty;
            var codViajeStrList = new List<string>();

            for (var i = 0; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);
                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i + 1, rows.Count()));

                #region Buffer Viajes

                try
                {
                    var codigoViaje = row[Properties.AsignacionCodigoViaje.Viaje].ToString().Trim();

                    if (lastCodViaje != codigoViaje && codigoViaje != string.Empty)
                    {
                        if (!codViajeStrList.Contains(codigoViaje))
                            codViajeStrList.Add(codigoViaje);

                        lastCodViaje = codigoViaje;
                    }                    
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Viaje ({0})", row[Properties.AsignacionCodigoViaje.Viaje]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codViajeStrList.Any())
            {
                var cant = 0;

                foreach (var l in codViajeStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering Viajes: {0}/{1}", cant, codViajeStrList.Count()));

                    var viajes = DaoFactory.ViajeDistribucionDAO.FindByEmpresaAndCodes(Empresa.Id, l);
                    if (viajes != null && viajes.Any())
                    {
                        _viajesBuffer.AddRange(viajes);
                    }
                }
            }
        }
    }
}
