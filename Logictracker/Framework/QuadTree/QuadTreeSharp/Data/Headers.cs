#region Usings

using System;
using System.IO;
using System.Runtime.InteropServices;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.QuadTree.Data
{
    public class Headers //: ObservableCollection<QuadTreeFile>
    {
        public enum States{
            WF_INIT,
            LOADING,
            READY,
            BAD
        };
        public States State { get; private set; }

        private readonly object syncLock = new object();
        private readonly Repository Repository;
        public GridStructure GridStructure { get; private set; }
        public string FileName { get { return String.Format("{0}\\GRIDSTRUCTURE.DAT", Repository.BaseFolderPath); } }

        public Headers(Repository Repository)
        {
            State = States.WF_INIT;
            this.Repository = Repository;    
        }

        public void Init(GridStructure gridStructure)
        {
            lock (syncLock)
            {
                GridStructure = gridStructure;

                State = States.LOADING;
                if (!File.Exists(FileName))
                {
                    STrace.Debug(GetType().FullName, "QTREE/HEADERS: Creando archivo de tamaño y layout.");
                    using (var sw = File.Create(FileName))
                    {
                        var data = new byte[512];
                        unsafe
                        {
                            fixed (byte* ptrBuffer = &data[0])
                            {
                                Marshal.StructureToPtr(GridStructure, new IntPtr(ptrBuffer), true);
                            }
                        }
                        sw.Write(data, 0, 512);
                        sw.Close();
                    }
                    State = States.READY;
                    return;
                }
                STrace.Debug(GetType().FullName, String.Format("QT0002: El archivo ya existe. Ruta: {0}", FileName));
                State = States.BAD;
                return;
            }
        }


        public void Open(ref GridStructure gridStructure)
        {
            lock (syncLock)
            {
                State = States.LOADING;
                if (File.Exists(FileName))
                {
                    STrace.Debug(GetType().FullName, "QTREE/HEADERS: Creando archivo de tamaño y layout.");
                    using (var sw = File.OpenRead(FileName))
                    {
                        var data = new byte[512];
                        sw.Read(data, 0, 512);
                        sw.Close();
                        unsafe
                        {
                            fixed (byte* ptrBuffer = &data[0])
                            {
                                GridStructure = (GridStructure) 
                                        Marshal.PtrToStructure(new IntPtr(ptrBuffer), 
                                            typeof(GridStructure));
                            }
                        }                        
                    }
                    State = States.READY;
                    gridStructure = GridStructure;
                    return;
                }
                STrace.Debug(GetType().FullName, String.Format("QT0003: El archivo no existe. Ruta: {0}", FileName));
                State = States.BAD;
                return;
            }
        }      

    }

}
