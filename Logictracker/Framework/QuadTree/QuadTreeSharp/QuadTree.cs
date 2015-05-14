#region Usings

using System;
using System.Runtime.InteropServices;
using Logictracker.DatabaseTracer.Core;
using Logictracker.QuadTree.Data;

#endregion

namespace Logictracker.QuadTree
{
    /// <summary>
    /// QuadTree Interop
    /// </summary>
    /// <remarks>El lock es a lo bestia </remarks>
    public class QuadTree : IDisposable
    {
        #region States enum

        public enum States
        {
            WF_INIT,
            CREATED,
            READY,
            DLLMISSING,
            ERROR
        } ;

        #endregion

        private static QuadTree Instance;

        private IntPtr native = IntPtr.Zero;

        private QuadTree()
        {
            try
            {
                State = States.WF_INIT;
                Q43_Create(out native);
                State = States.CREATED;
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
        }

        public States State { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Close();
                Q43_Destroy(native);
                GC.SuppressFinalize(this);
                native = IntPtr.Zero;
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
        }

        #endregion

        #region DllImport

        [DllImport("QuadTree.dll")]
        private static extern int Q43_Create(out IntPtr handle);

        [DllImport("QuadTree.dll")]
        private static extern int Q43_Destroy(IntPtr handle);

        [DllImport("QuadTree.dll")]
        private static extern int Q43_Init(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string basedir);

        [DllImport("QuadTree.dll")]
        private static extern int Q43_Open(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string basedir);

        [DllImport("QuadTree.dll")]
        private static extern void Q43_Close(IntPtr handle);

        [DllImport("QuadTree.dll")]
        private static extern int Q43_GetPositionClass(IntPtr handle, float lat, float lon);

        [DllImport("QuadTree.dll")]
        private static extern char Q43_GetPositionZone(IntPtr handle, float lat, float lon);

        [DllImport("QuadTree.dll")]
        private static extern int Q43_SetPositionClass(IntPtr handle, float lat, float lon, int _class, char zona);

        [DllImport("QuadTree.dll")]
        private static extern void Q43_TrLog(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string action,
                                             [MarshalAs(UnmanagedType.LPStr)] string data);

        #endregion

        public static QuadTree i()
        {
        	return Instance ?? (Instance = new QuadTree());
        }

    	public int InitEx(string basedir, GridStructure gridStructure)
        {
            return 0;
        }

        public int OpenEx(string basedir, out GridStructure gridStructure)
        {
            gridStructure = new GridStructure();
            return 0;
        }

        public int Init(string basedir)
        {
            try
            {
                if (State != States.CREATED) return -1;
                return Q43_Init(native, basedir);
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
            return -1;
        }

        public int Open(string basedir)
        {
            try
            {
                if (State != States.CREATED) return -1;
                var result = Q43_Open(native, basedir);
                State = result == 0 ? States.READY : States.ERROR;
                if (State == States.ERROR)
                {
                    STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                    STrace.Debug(GetType().FullName,"QTREE: debido a un error al abrir el repositorio.");
                    return -1;
                }
                return result;
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
            return -1;
        }

        public void Close()
        {
            try
            {
                if (State == States.CREATED || State == States.READY)
                    Q43_Close(native);
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
        }

        public int GetPositionClass(float lat, float lon)
        {
            try
            {
                return State != States.READY ? -1 : Q43_GetPositionClass(native, lat, lon);
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
            return -1;
        }

        public char GetPositionZone(float lat, float lon)
        {
            try
            {
                return State != States.READY ? Convert.ToChar(0xFF) : Q43_GetPositionZone(native, lat, lon);
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName, e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
            return Convert.ToChar(0xFF);
        }

        public int GetPositionClass(double lat, double lon)
        {
            return GetPositionClass((float) lat, (float) lon);
        }

        public int SetPositionClass(float lat, float lon, int _class, char zona)
        {
            try
            {
                return State != States.READY ? -1 : Q43_SetPositionClass(native, lat, lon, _class, zona);
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
            return -1;
        }

        public void TrLog(string action, string data)
        {
            try
            {
                if (State != States.READY) return;
                Q43_TrLog(native, action, data);
            }
            catch (DllNotFoundException e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a que falta el archivo QuadTree.DLL o una de sus dependencias.");
                STrace.Exception(GetType().FullName,e);
                State = States.DLLMISSING;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName,"QTREE: la interface de acceso y edicion de datos (de clase y extendidos) queda deshabilitada.");
                STrace.Debug(GetType().FullName,"QTREE: debido a un error no especificado.");
                STrace.Exception(GetType().FullName,e);
                State = States.ERROR;
            }
        }
    }
}