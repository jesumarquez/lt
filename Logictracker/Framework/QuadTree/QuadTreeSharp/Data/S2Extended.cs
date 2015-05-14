using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using Urbetrack.Toolkit;
using System.Runtime.InteropServices;

namespace Urbetrack.QuadTree
{
    public class S2Extended //: ObservableCollection<QuadTreeFile>
       
    {
        public enum States{
            WAITING_FOR_INITIALIZE,
            LOCKED_AND_SYNCING,            
            INSERVICE,
            SHOCK_GUARD,
            FAILURE,
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct S2Entry
        {
            [MarshalAs(UnmanagedType.U8, SizeConst = 8)]
            public ulong UniqueIdentifier;
            [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
            public short VerticesCount;
            [MarshalAs(UnmanagedType.I4, SizeParamIndex= 2)]
	        public int[] Vertices;
        };

        public void Transition(States newState)
        {
            lock (syncLock) 
            {
                if (State == newState) return;
                T.TRACE(3, "QT/SG2: Store File Transition. {0} -> {1}", State.ToString(), newState.ToString());
                State = newState;
            }
        }

        public States State { get; private set; }

        private readonly object syncLock = new object();
        
        private readonly Repository Repository;

        public string FileName { get; private set; }

        public S2Extended(Repository Repository, float lat, float lon)
        {
            Transition(States.WAITING_FOR_INITIALIZE); 
            this.Repository = Repository;
            var Gr2Cache = this.Repository.IndexCatalog.OfType<GR2>().FirstOrDefault();
            FileName = Gr2Cache.ReferencePath(lat, lon, "cmap");

            var dir = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(FileName))
            {
                var sw = File.CreateText(FileName);
                sw.Write("SG2v0.5({0})>", DateTime.Now);
                sw.Close();
            }
        }

        

        

    }

}
