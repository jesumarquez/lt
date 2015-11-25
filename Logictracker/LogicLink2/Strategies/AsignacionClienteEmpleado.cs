using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using LinqToExcel;
using Geocoder.Core.VO;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class AsignacionClienteEmpleado : Strategy
    {
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Linea Linea { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }
        private readonly List<PuntoEntrega> _puntosBuffer = new List<PuntoEntrega>();
        private readonly List<Empleado> _empleadosBuffer = new List<Empleado>();

        public static void Parse(LogicLinkFile file, out int puntos, out string observaciones)
        {
            new AsignacionClienteEmpleado(file).Parse(out puntos, out observaciones);
        }

        public AsignacionClienteEmpleado(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Linea = file.Linea;
        }

        public void Parse(out int puntos, out string observaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseExcelFile(Llfile.FilePath, true);
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            te.Restart();
            PreBufferRows(rows);            
            STrace.Trace(Component, string.Format("PreBufferRows en {0} segundos", te.getTimeElapsed().TotalSeconds));
            
            var listPuntos = new List<PuntoEntrega>(rows.Count);
            puntos = 0;
            observaciones = string.Empty;

            STrace.Trace(Component, "Cantidad de filas: " + rows.Count);
            var filas = 0;

            foreach (var row in rows)
            {
                filas++;
                STrace.Trace(Component, string.Format("Procesando fila: {0}/{1}", filas, rows.Count));

                var codigoCliente1 = row[Properties.AsignacionClienteEmpleado.CodigoCliente].ToString().Trim();
                var puntoEntrega1 = _puntosBuffer.SingleOrDefault(v => v.Codigo == codigoCliente1);

                var codigoCliente2 = "00000000000" + codigoCliente1;
                codigoCliente2 = codigoCliente2.Substring(codigoCliente2.Length-10, 10);
                var puntoEntrega2 = _puntosBuffer.SingleOrDefault(v => v.Codigo == codigoCliente2);

                var legajo = row[Properties.AsignacionClienteEmpleado.Legajo].ToString().Trim();
                var empleado = _empleadosBuffer.SingleOrDefault(v => v.Legajo == legajo);

                if (empleado == null)
                {
                    observaciones = "Valor inválido para el campo EMPLEADO";
                    continue;
                }

                if (puntoEntrega1 != null)
                {
                    puntoEntrega1.Responsable = empleado;
                    listPuntos.Add(puntoEntrega1);
                    puntos++;
                }
                if (puntoEntrega2 != null)
                {
                    puntoEntrega2.Responsable = empleado;
                    listPuntos.Add(puntoEntrega2);
                    puntos++;
                }
            }

            STrace.Trace(Component, "Guardando puntos: " + listPuntos.Count);
            te.Restart();
            var i = 1;
            foreach (var punto in listPuntos)
            {
                STrace.Trace(Component, string.Format("Guardando: {0}/{1}", i++, listPuntos.Count()));
                DaoFactory.PuntoEntregaDAO.SaveOrUpdate(punto);
            }
            STrace.Trace(Component, string.Format("Puntos guardados en {0} segundos", te.getTimeElapsed().TotalSeconds));
        }

        private void PreBufferRows(IEnumerable<Row> rows)
        {
            var lastCodPunto = string.Empty;
            var lastCodEmpleado = string.Empty;

            var codPuntoStrList = new List<string>();
            var codEmpleadoStrList = new List<string>();

            for (var i = 0; i < rows.Count(); i++)
            {
                var row = rows.ElementAt(i);
                STrace.Trace(Component, string.Format("Prebuffering: {0}/{1}", i + 1, rows.Count()));

                #region Buffer PuntoEntrega

                try
                {
                    var codigoPuntoEntrega = row[Properties.AsignacionClienteEmpleado.CodigoCliente].ToString().Trim();

                    if (lastCodPunto != codigoPuntoEntrega && codigoPuntoEntrega != string.Empty)
                    {
                        if (!codPuntoStrList.Contains(codigoPuntoEntrega))
                            codPuntoStrList.Add(codigoPuntoEntrega);

                        lastCodPunto = codigoPuntoEntrega;
                    }

                    codigoPuntoEntrega = "00000000000" + codigoPuntoEntrega;
                    codigoPuntoEntrega = codigoPuntoEntrega.Substring(codigoPuntoEntrega.Length - 10, 10);

                    if (lastCodPunto != codigoPuntoEntrega && codigoPuntoEntrega != string.Empty)
                    {
                        if (!codPuntoStrList.Contains(codigoPuntoEntrega))
                            codPuntoStrList.Add(codigoPuntoEntrega);

                        lastCodPunto = codigoPuntoEntrega;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex,
                        String.Format("Error Buffering Punto de Entrega ({0})", row[Properties.AsignacionClienteEmpleado.CodigoCliente]));
                }

                #endregion

                #region Buffer Empleados

                try
                {
                    var legajo = row[Properties.AsignacionClienteEmpleado.Legajo].ToString().Trim();

                    if (lastCodEmpleado != legajo)
                    {
                        if (!codEmpleadoStrList.Contains(legajo))
                            codEmpleadoStrList.Add(legajo);

                        lastCodEmpleado = legajo;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(Component, ex, String.Format("Error Buffering Empleado ({0})", row[Properties.AsignacionClienteEmpleado.Legajo]));
                }

                #endregion
            }

            const int batchSize = 1000;

            if (codPuntoStrList.Any())
            {
                var cant = 0;

                foreach (var l in codPuntoStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering Puntos: {0}/{1}", cant, codPuntoStrList.Count()));

                    var puntos = DaoFactory.PuntoEntregaDAO.FindByEmpresaAndCodes(Empresa.Id, l);
                    if (puntos != null && puntos.Any())
                    {
                        _puntosBuffer.AddRange(puntos);
                    }
                }
            }

            if (codEmpleadoStrList.Any())
            {
                var cant = 0;

                foreach (var l in codEmpleadoStrList.InSetsOf(batchSize))
                {
                    cant += l.Count();
                    STrace.Trace(Component, string.Format("Buffering Empleados: {0}/{1}", cant, codEmpleadoStrList.Count()));

                    var empleados = DaoFactory.EmpleadoDAO.FindByLegajos(Empresa.Id, -1, l);
                    if (empleados != null && empleados.Any())
                    {
                        _empleadosBuffer.AddRange(empleados);
                    }
                }
            }
        }
    }
}
