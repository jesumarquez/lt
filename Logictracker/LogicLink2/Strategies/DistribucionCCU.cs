using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public class DistribucionCCU : Strategy
    {
        private static Dictionary<int, List<int>> EmpresasLineas = new Dictionary<int, List<int>>();
        private const string Component = "Logiclink2";

        private Empresa Empresa { get; set; }
        private Cliente Cliente { get; set; }
        private LogicLinkFile Llfile { get; set; }
        private DAOFactory DaoFactory { get; set; }

        public static Dictionary<int, List<int>> ParseRutas(LogicLinkFile file, out int rutas, out int entregas)
        {
            new DistribucionCCU(file).ParseRutas(out rutas, out entregas);
            return EmpresasLineas;
        }

        public static void ParseClientes(LogicLinkFile file, out int clientes)
        {
            new DistribucionCCU(file).ParseClientes(out clientes);            
        }

        public DistribucionCCU(LogicLinkFile file)
        {
            Llfile = file;
            DaoFactory = new DAOFactory();
            Empresa = file.Empresa;
            Cliente = DaoFactory.ClienteDAO.GetList(new[] { Empresa.Id }, new[] { -1 }).FirstOrDefault();
        }

        public void ParseRutas(out int rutas, out int entregas)
        {
            const int vigencia = 12;

            var te = new TimeElapsed();
            var rows = ParseFile(Llfile.FilePath).Rows;
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));
            
            var listPuntos = new List<PuntoEntrega>();
            var listReferencias = new List<ReferenciaGeografica>();

            rutas = 0;
            entregas = 0;            
        }

        public void ParseClientes(out int clientes)
        {
            var te = new TimeElapsed();
            var rows = ParseFile(Llfile.FilePath).Rows;
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));

            var listPuntos = new List<PuntoEntrega>();
            var listReferencias = new List<ReferenciaGeografica>();

            clientes = 0;
        }

        public static void ParseAsignaciones(LogicLinkFile file, out int asignaciones)
        {
            var te = new TimeElapsed();
            var rows = ParseFile(file.FilePath).Rows;
            STrace.Trace(Component, string.Format("Archivo parseado en {0} segundos", te.getTimeElapsed().TotalSeconds));

            asignaciones = 0;
        }
        
    }
}
