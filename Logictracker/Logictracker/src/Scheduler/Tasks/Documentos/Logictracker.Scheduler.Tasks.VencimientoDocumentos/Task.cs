using System;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Scheduler.Tasks.VencimientoDocumentos
{
    /// <summary>
    /// Task for analizing if any document has expired.
    /// </summary>
    public class Task : BaseTask
    {
        #region Protected Methods

        /// <summary>
        /// Analice all documents to determine if their are expired.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
			STrace.Trace(GetType().FullName, "Checking documents due date.");

			var documentos = DaoFactory.DocumentoDAO.FindByVencimiento();

            foreach (Documento documento in documentos)
            {
                try
                {
                    AnalizeDocument(documento);
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    STrace.Exception(GetType().FullName, ex);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Analizes the specified documents due date.
        /// </summary>
        /// <param name="documento"></param>
        private void AnalizeDocument(Documento documento)
        {
            STrace.Trace(GetType().FullName, string.Format("Analizing document: {0}", documento.Codigo));

            if (!documento.Vencimiento.HasValue) return;

            var coche = documento.TipoDocumento.AplicarAVehiculo 
                ? documento.Vehiculo 
                : documento.TipoDocumento.AplicarATransportista 
                    ? DaoFactory.CocheDAO.GetGenerico(documento.Empresa, documento.Transportista)
                    : DaoFactory.CocheDAO.GetGenerico(documento.Empresa);
            var empleado = documento.TipoDocumento.AplicarAEmpleado ? documento.Empleado : null;

            var now = DateTime.UtcNow;
            var primerVenc = documento.Vencimiento.Value.AddDays(-documento.TipoDocumento.PrimerAviso);
            var segundoVenc = documento.Vencimiento.Value.AddDays(-documento.TipoDocumento.SegundoAviso);
            var venc = documento.Vencimiento.Value;

            var primerAviso = now > primerVenc;
            var segundoAviso = now > segundoVenc;
            var vencido = now > venc;

            if (!documento.EnviadoAviso1 && primerAviso)
            {
                STrace.Trace(GetType().FullName, "Primer Aviso Cumplido: " + now.ToString("dd/MM/yyyy HH:mm") + " > " + primerVenc.ToString("dd/MM/yyyy HH:mm"));
                FirstWarn(documento, coche, empleado);
            }
            else if(!documento.EnviadoAviso2 && segundoAviso)
            {
                STrace.Trace(GetType().FullName, "Segundo Aviso Cumplido: " + now.ToString("dd/MM/yyyy HH:mm") + " > " + segundoVenc.ToString("dd/MM/yyyy HH:mm"));
                SecondWarn(documento, coche, empleado);
            }
            else if(!documento.EnviadoAviso3 && vencido)
            {
                STrace.Trace(GetType().FullName, "Vencimiento Cumplido: " + now.ToString("dd/MM/yyyy HH:mm") + " > " + venc.ToString("dd/MM/yyyy HH:mm"));
                Expired(documento, coche, empleado);
            }
            else STrace.Trace(GetType().FullName, string.Format("Valid document: {0}", documento.Codigo));
        }

        /// <summary>
        /// Notifies that the specified document has expired.
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="coche"></param>
        /// <param name="empleado"> </param>
        private void Expired(Documento documento, Coche coche, Empleado empleado)
        {
            STrace.Trace(GetType().FullName, string.Format("Document expired: {0}", documento.Codigo));

            GenerateMessage(documento, coche, empleado, MessageCode.DocumentExpired.GetMessageCode());
            documento.EnviadoAviso3 = true;
            DaoFactory.DocumentoDAO.SaveOrUpdate(documento);
        }

        /// <summary>
        /// Triggers the document first warning alarm.
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="coche"></param>
        /// <param name="empleado"> </param>
        private void FirstWarn(Documento documento, Coche coche, Empleado empleado)
        {
            STrace.Trace(GetType().FullName, string.Format("Document first warning reached: {0}", documento.Codigo));

            GenerateMessage(documento, coche, empleado, MessageCode.DocumentFirstWarning.GetMessageCode());
            documento.EnviadoAviso1 = true;
            DaoFactory.DocumentoDAO.SaveOrUpdate(documento);
        }

        /// <summary>
        /// Triggers the document second warning alarm.
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="coche"></param>
        /// <param name="empleado"> </param>
        private void SecondWarn(Documento documento, Coche coche, Empleado empleado)
        {
            STrace.Trace(GetType().FullName, string.Format("Document second warning reached: {0}", documento.Codigo));

            GenerateMessage(documento, coche, empleado, MessageCode.DocumentSecondWarning.GetMessageCode());
            documento.EnviadoAviso2 = true;
            DaoFactory.DocumentoDAO.SaveOrUpdate(documento);
        }

        /// <summary>
        /// Saves the specified event.
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="coche"></param>
        /// <param name="empleado"> </param>
        /// <param name="codigo"></param>
        private void GenerateMessage(Documento documento, Coche coche, Empleado empleado, string codigo)
        {
            var text = GetMessageText(documento);

            MessageSaver.Save(codigo, coche, empleado, DateTime.UtcNow, null, text);
        }

        /// <summary>
        /// Gets the text associated to the message
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        private static string GetMessageText(Documento documento)
        {
            return documento.Vencimiento != null
                       ? string.Format(" {0} - {1} ({2})", documento.TipoDocumento.Nombre, documento.Codigo, documento.Vencimiento.Value.ToString("dd/MM/yyyy"))
                       : string.Empty;
        }

        #endregion
    }
}
