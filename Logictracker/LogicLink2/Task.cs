using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Scheduler.Tasks.Logiclink2.Strategies;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Logiclink2
{
    public class Task : BaseTask
    {
        private Dictionary<int, List<int>> _empresasLineas = new Dictionary<int, List<int>>();
        private const string Component = "Logiclink2";

        private int IdEmpresa
        {
            get
            {
                var empresa = GetInt32("Empresa");
                return empresa.HasValue ? empresa.Value : -1;
            }
        }

        protected override void OnExecute(Timer timer)
        {
            if (IdEmpresa <= 0) return;

            var archivoPendiente = DaoFactory.LogicLinkFileDAO.GetNextPendiente(IdEmpresa);
            if (archivoPendiente != null)
            {
                STrace.Trace(Component, "Archivo a procesar: " + archivoPendiente.FilePath);

                var te = new TimeElapsed();
                ProcessArchivo(archivoPendiente);
                STrace.Trace(Component, "Archivo procesado en: " + te.getTimeElapsed().TotalSeconds + " segundos.");
            }
        }

        public void ProcessArchivo(LogicLinkFile archivo)
        {
            archivo.Status = LogicLinkFile.Estados.Procesando;
            DaoFactory.LogicLinkFileDAO.SaveOrUpdate(archivo);

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    STrace.Trace(Component, "Procesando con Estrategia: " + archivo.Strategy);
                    var rutas = 0;
                    var entregas = 0;
                    ViajeDistribucion viaje = null;

                    switch (archivo.Strategy)
                    {
                        case LogicLinkFile.Estrategias.DistribucionFemsa:
                            _empresasLineas = DistribucionFemsa.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionQuilmes:
                            _empresasLineas = DistribucionQuilmes.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionMusimundo:
                            //EmpresasLineas = DistribucionMusimundo.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionBrinks:
                            //EmpresasLineas = DistribucionBrinks.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionSos:
                            viaje = DistribucionSos.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionReginaldLee:
                            _empresasLineas = DistribucionReginaldLee.Parse(archivo, out rutas, out entregas);
                            break;
                    }

                    archivo.Status = LogicLinkFile.Estados.Procesado;
                    archivo.DateProcessed = DateTime.UtcNow;
                    var result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                    archivo.Result = result;
                    DaoFactory.LogicLinkFileDAO.SaveOrUpdate(archivo);

                    transaction.Commit();

                    if (archivo.Strategy == LogicLinkFile.Estrategias.DistribucionSos)
                    {
                        if (viaje != null)
                        {
                            foreach (var detalle in viaje.Detalles)
                            {
                                if (detalle.PuntoEntrega != null && detalle.PuntoEntrega.ReferenciaGeografica != null)
                                    AddReferenciasGeograficas(detalle.ReferenciaGeografica);
                            }

                            if (viaje.Vehiculo != null && viaje.Estado == ViajeDistribucion.Estados.Pendiente)
                            {
                                var tieneViajeEnCurso = DaoFactory.ViajeDistribucionDAO.FindEnCurso(viaje.Vehiculo) != null;
                                if (!tieneViajeEnCurso)
                                {
                                    var ciclo = new CicloLogisticoDistribucion(viaje, DaoFactory,
                                        new MessageSaver(DaoFactory));
                                    var evento = new InitEvent(DateTime.UtcNow);
                                    ciclo.ProcessEvent(evento);
                                }
                            }
                        }
                    }

                    if (_empresasLineas.Count > 0)
                    {
                        DaoFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                        _empresasLineas.Clear();
                    }

                    STrace.Trace(Component, result);
                }
                catch (Exception ex)
                {
                    AddError(ex);
                    transaction.Rollback();
                    ClearData();

                    archivo.Status = LogicLinkFile.Estados.Cancelado;
                    var result = "Archivo procesado erroneamente. Exception: " + ex.Message;
                    archivo.Result = result;
                    DaoFactory.LogicLinkFileDAO.SaveOrUpdate(archivo);

                    STrace.Trace(Component, result);
                }
                finally
                {
                    ClearData();
                }
            }
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }

        private void AddReferenciasGeograficas(ReferenciaGeografica rg)
        {
            if (rg == null)
                STrace.Error(Component, "AddReferenciasGeograficas: rg is null");
            else if (rg.Empresa == null)
                STrace.Error(Component, "AddReferenciasGeograficas: rg.Empresa is null");
            else
            {
                if (!_empresasLineas.ContainsKey(rg.Empresa.Id))
                    _empresasLineas.Add(rg.Empresa.Id, new List<int> { -1 });

                if (rg.Linea != null)
                {
                    if (!_empresasLineas[rg.Empresa.Id].Contains(rg.Linea.Id))
                        _empresasLineas[rg.Empresa.Id].Add(rg.Linea.Id);
                }
                else
                {
                    var todaslaslineas = DaoFactory.LineaDAO.GetList(new[] { rg.Empresa.Id });
                    foreach (var linea in todaslaslineas)
                    {
                        if (!_empresasLineas.ContainsKey(linea.Id))
                            _empresasLineas[rg.Empresa.Id].Add(linea.Id);
                    }
                }
            }
        }
    }
}
