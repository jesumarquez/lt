#region Usings

using System;
using System.ComponentModel;
using System.Threading;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Utils
{
    public class Task
    {
        #region Delegates

        public delegate void TaskResultHandler(Task task, TaskResults result);

        #endregion

        #region TaskResults enum

        public enum TaskResults
        {
            Success,
            Failure,
            Canceled
        }

        #endregion

        #region TaskStates enum

        public enum TaskStates
        {
            [Description("No Construida")] WF_BUILD,
            [Description("No iniciada")] WF_START,
            [Description("Iniciando")] STARTING,
            [Description("Ejecutandose")] RUNNING,
            [Description("Durmiendo")] SLEEPING,
            [Description("Terminando")] STOPING,
            [Description("Terminada: Normal/Voluntario")] STOP_NORMAL,
            [Description("Terminada: Error/Voluntario")] STOP_FAILURE,
            [Description("Termianda: Normal/Externo")] STOP_JOINT,
            [Description("Terminada: Aborto/Externo")] STOP_ABORTED,
            [Description("Terminada: Excepcion no manejada!")] STOP_UNHANDLED_EXCEPTION
        };

        #endregion

        private readonly Thread thread;

        protected Task(string taskname)
        {
			JoinTimeout = Config.Task.JoinTimeout;
            TaskState = TaskStates.WF_START;
            thread = new Thread(Body) {Name = taskname, IsBackground = true };
        }

        public TaskStates TaskState { get; private set; }

        public int JoinTimeout { get; protected set; }

        /// <summary>
        /// Inicializa la tarea.
        /// </summary>
        public void Start()
        {
            if (TaskState != TaskStates.WF_START)
                throw new InvalidOperationException(String.Format("TASK[{0}]: La tarea que intenta iniciar ya fue inicada.", thread.Name));
            // empezamos...
            TaskState = TaskStates.STARTING;
            thread.Start();
        }

        /// <summary>
        /// Detiene la tarea, vencido un plazo si no finalizo, la aborta.
        /// </summary>
        public void Stop()
        {
            if (TaskState != TaskStates.RUNNING && TaskState != TaskStates.SLEEPING)
            {
                STrace.Debug(GetType().FullName,string.Format("TASK[{0}]: el estado es invalido para cerrar.", thread.Name));
                return;
            }

            // paro cualquier sleep que este corriendo o apunto de correrse de manera 
            // incluso si no esta SLEEPING pues quiza esta haciendo un Sleep internamente.
            thread.Interrupt();
            
            // trato de unirme 
            if (!thread.Join(JoinTimeout))
            {
                // si no pude, aborto.
                thread.Abort();
                TaskState = TaskStates.STOP_ABORTED;
            } 
			else
            {
                // cerro correctamente.
                TaskState = TaskStates.STOP_JOINT;
            }
        }

        /// <summary>
        /// Interrumpe a una tarea si esta se encuentra dentro de un Sleep. Si la 
        /// tarea no esta durmiendo, se ignora el llamado.
        /// </summary>
        public void Wakeup()
        {
            if (TaskState != TaskStates.SLEEPING) return;
            thread.Interrupt();
        }

        private void Body()
        {
            ulong ticks = 0;
            try
            {
                // inicializamos la tarea
                BeforeStart();
                TaskState = TaskStates.RUNNING;
                while (TaskState == TaskStates.SLEEPING || TaskState == TaskStates.RUNNING)
                {
                    TaskState = TaskStates.RUNNING;
                    var result = DoWork(ticks++);
                    if (result < 0)
                    {
                        TaskState = TaskStates.STOP_FAILURE;
                    }
					else if (result == 0)
                    {
                        TaskState = TaskStates.STOP_NORMAL;
                    }
					else
                    {
                        Sleep(result);
                    }
                }
            }
			catch (ThreadAbortException e)
            {
                STrace.Debug(GetType().FullName, String.Format("TASK[{0}]: ABORTED source={1}", thread.Name, String.IsNullOrEmpty(e.Source) ? "(desconocido)" : e.Source));
                TaskState = TaskStates.STOP_ABORTED;
            }
			catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e, String.Format("TASK[{0}]: *********  TASK UNHANDLED EXCEPTION ********** ", thread.Name));
                TaskState = TaskStates.STOP_UNHANDLED_EXCEPTION;
				Environment.Exit(3);
            }
			STrace.Debug(GetType().FullName, String.Format("TASK[{0}]: Finalizo con Estado: '{1:G}'", thread.Name, TaskState));
            AfterStop();
        }

        public void Sleep(int milliseconds)
        {
            try
            {
                if (TaskState != TaskStates.RUNNING)
                    throw new InvalidOperationException(string.Format("TASK[{0}]:No se puede llamar a Sleep dentro de una tarea que no este en estado RUNNING.", thread.Name));
                TaskState = TaskStates.SLEEPING;
                Thread.Sleep(milliseconds);
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        /// <summary>
        /// Es llamada cuando se transiciona a STARTING para que la implementacion
        /// pueda inicializar su contexto personalizado.
        /// </summary>
        protected virtual bool BeforeStart()
        {
            return true;
        }

        /// <summary>
        /// Es llamada cada vez que se transiciona a RUNNING
        /// </summary>
        /// <param name="tick"></param>
        /// <returns>
        /// si se retorna 0, la tarea termina exitosamente.
        /// si se retorna un numero menor que cero, la tarea termina con error.
        /// si se retorna un numero mayor que cero, la tarea transiciona a SLEEPING 
        /// y por la cantidad milisegundos del valor retornado espera asincronicamente. 
        /// Una vez transcurrido el intervalo, transicion a RUNNING y vuelve a llamar a 
        /// DoWork. 
        /// </returns>
        protected virtual int DoWork(ulong tick)
        {
            return 0; // por defecto la tarea termina inmediatamente.
        }

        /// <summary>
        /// Es llamada luego de que haya finalizado la tarea, ya sea por que DoWork
        /// la finalizo retornando 0 o menor. O por que fue finalizada desde otro
        /// contexto. 
        /// </summary>
        protected virtual void AfterStop()
        {
            
        }
    }
}
