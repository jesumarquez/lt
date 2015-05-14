#region Usings

using System;
using System.Threading;
using Urbetrack.Mobile.Toolkit;

#endregion

namespace Urbetrack.Mobile.Toolkit
{
    public class Task
    {
        public enum TaskResults
        {
            Success,
            Failure,
            Canceled
        }

        public delegate void TaskResultHandler(Task task, TaskResults result);

        public enum TaskStates
        {
            WF_BUILD,
            WF_START,
            STARTING,
            RUNNING,
            SLEEPING,
            STOPING,
            STOP_NORMAL,
            STOP_FAILURE,
            STOP_JOINT,
            STOP_ABORTED,
            STOP_UNHANDLED_EXCEPTION
        };

        private readonly Thread thread;

        public TaskStates TaskState { get; private set; }

        public int JoinTimeout { get; protected set; }
        public string TaskName { get; private set; }

        protected Task(string taskname)
        {
            TaskName = taskname;
            JoinTimeout = 5000;
            TaskState = TaskStates.WF_START;
            thread = new Thread(Body);
        }


        /// <summary>
        /// Inicializa la tarea.
        /// </summary>
        public void Start()
        {
            if (TaskState != TaskStates.WF_START)
                throw new InvalidOperationException("TASK[" + TaskName + "]: La tarea que intenta iniciar ya fue inicada.");
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
                throw new InvalidOperationException("TASK[" + TaskName + "]:La tarea que intenta detener no esta en ejecucion.");

            // paro cualquier sleep que este corriendo o apunto de correrse de manera 
            // incluso si no esta SLEEPING pues quiza esta haciendo un Sleep internamente.
            
            // MOBILE NO
            // thread.Interrupt(); 

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
            //thread.Interrupt();
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
                    else
                        if (result == 0)
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
                T.ERROR("TASK[" + TaskName + "]: *********  TASK ABORTED ********** ");
                T.EXCEPTION(e,"TASKBODY");
                TaskState = TaskStates.STOP_ABORTED;
            }
            catch (Exception e)
            {
                T.ERROR("TASK[" + TaskName + "]: *********  TASK UNHANDLED EXCEPTION ********** ");
                T.EXCEPTION(e, "TASKBODY");
                TaskState = TaskStates.STOP_UNHANDLED_EXCEPTION;
                //Process.Abort("TASK[" + TaskName + "] no controlo la exception antedicha, el proceso se suicida.");
            }
            T.TRACE("TASK[" + TaskName + "]: Finalizo con Estado: " + TaskState);
            AfterStop();
        }

        public void Sleep(int milliseconds)
        {
            try
            {
                if (TaskState != TaskStates.RUNNING)
                    throw new InvalidOperationException(
                        "TASK[" + TaskName + "]:No se puede llamar a Sleep dentro de una tarea que no este en estado RUNNING.");
                TaskState = TaskStates.SLEEPING;
                Thread.Sleep(milliseconds);
            }
            catch (Exception e)
            {
                T.EXCEPTION(e,"TASKSLEEP");
                return;
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