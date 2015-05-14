#region Usings

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.QuadTree.Data
{
    public class GR2 //: ObservableCollection<QuadTreeFile>
        : ICustomIndex
    {
        public enum States{
            WAITING_FOR_INITIALIZE,
            LOCKED_AND_SYNCING,            
            INSERVICE,
            SHOCK_GUARD,
            FAILURE,
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Gr2Headers
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
	        public string Signature;
            [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
            public char Status;
            [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
            public int Revision;
            [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
            public short base_lat;
            [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
            public short base_lon;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct M2Sector
        {
	        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
            public char Status;
	        [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
            public int Revision;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 450)]
            public byte[] Data;
                       
        };

        public void Transition(States newState)
        {
            lock (syncLock) 
            {
                if (State == newState) return;
                STrace.Debug(typeof(GR2).FullName, String.Format("QT/GR2: Store File Transition. {0} -> {1}", State.ToString(), newState.ToString()));
                State = newState;
            }
        }

        public States State { get; private set; }

        private readonly object syncLock = new object();

        private Gr2Headers Headers;

        public Repository Repository { get; set; }

        public short Latitude { get; private set; }

        public short Longitude { get; private set; }
        
        public string FileName { get { return GetFileName(Latitude, Longitude, "GR2"); } }
        
        public GR2()
        {
            Transition(States.WAITING_FOR_INITIALIZE); 
            SectorCached = -1;
        }
 
        public IEnumerable Elements(float lat, float lon)
        {
            return new [] { GetPositionClass(lat, lon) };
        }

        public string GetFileName(float lat, float lon, string ext)
        {
            var gr_lat = (int)Math.Abs(lat);
            var gr_lon = (int)Math.Abs(lon);
            return Repository.BaseFolderPath + "\\M2-" + gr_lat + "-" + gr_lon + "." + ext;
        }

        #region Geolocalizacion por Referencia con nombre.
        public TYPE GetReference<TYPE>(float lat, float lon, string name)
        {
            if (State != States.INSERVICE) throw new ApplicationException("El indice esta fuera de servicio");
            //RefreshCache(sector);
            var file = ReferencePath(lat,lon,name);
            var data = LoadExtended(file);
            
            try
            {
                unsafe
                {
                    fixed (byte* ptrBuffer = &data[0])
                    {
                        return (TYPE) Marshal.PtrToStructure(new IntPtr(ptrBuffer),
                                                       typeof(TYPE));
                    }
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(GR2).FullName,e);
                return default(TYPE);
            }

        }

        public string ReferencePath(float lat, float lon, string name)
        {
            var gr_lat = (int)Math.Abs(lat);
            var gr_lon = (int)Math.Abs(lon);
            int sector;
            int xbyte;
            bool low_bits;
            GetFileSector(lat, lon, out sector, out xbyte, out low_bits);
            return Repository.BaseFolderPath + "\\Lat" + gr_lat + "\\Lon" + gr_lon + "\\" + sector + "\\" + xbyte + "-" + (low_bits ? "Lb" : "Hb") + ".QTX";
        }

        public void SetReference<TYPE>(float lat, float lon, string name, TYPE value)
        {
            if (State != States.INSERVICE) throw new ApplicationException("El indice esta fuera de servicio");
            try
            {
                var data = new byte[512];
                unsafe
                {
                    fixed (byte* ptrBuffer = &data[0])
                    {
                        Marshal.StructureToPtr(value, new IntPtr(ptrBuffer), true);
                        StoreExtended(ReferencePath(lat,lon,name), data);
                    }
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(GR2).FullName,e);
            }
        }
        #endregion

        #region Cache de nivel 1

        private int SectorCached;

        private bool CommitCacheRequired;

        private M2Sector Cache;

        public void LoadLevel1Cache(float lat, float lon)
        {
            if (!File.Exists(GetFileName(lat,lon,"GR2")))
            {
                Create(lat, lon);
            } else {
                if (string.IsNullOrEmpty(Headers.Signature))
                    Open(lat, lon);
            }
        }

        public void CommitLevel1Cache()
        {
	        if (!CommitCacheRequired) return;
	        RefreshCache(SectorCached != 0 ? 0 : 1);
        }

	    private void RefreshCache(int SectorNumber)
        {
            if (SectorCached == SectorNumber) return;
            if (SectorCached != -1 && CommitCacheRequired)
            {
                CommitCacheRequired = false;
                StoreM2(SectorCached, Cache);
            }
            byte b = 0;
            var data = GetSector(SectorNumber, ref b);
            try
            {
                unsafe
                {
                    fixed (byte* ptrBuffer = &data[0])
                    {
                        Cache = (M2Sector)
                                Marshal.PtrToStructure(new IntPtr(ptrBuffer),
                                                       typeof(M2Sector));
                    }
                }
                SectorCached = SectorNumber;
            }
            catch (Exception e)
            {
                STrace.Debug(typeof(GR2).FullName, e.ToString());
                SectorCached = -1;
            }
        }
        #endregion

        #region Funciones de la version 1
        public int GetPositionClass(float lat, float lon)
        {
            if (State != States.INSERVICE) throw new ApplicationException("El indice esta fuera de servicio");
            int sector;
            int xbyte;
            bool low_bits;
            GetFileSector(lat, lon, out sector, out xbyte, out low_bits);
            RefreshCache(sector);
            var active_byte = Cache.Data[xbyte];
            if (low_bits)
            {
                return active_byte & 0x0F;
            }
            return (active_byte >> 4) & 0x0F;
        }

        public void SetPositionClass(float lat, float lon, int value)
        {
            if (State != States.INSERVICE) throw new ApplicationException("El indice esta fuera de servicio");
            int sector;
            int xbyte;
            bool low_bits;
            GetFileSector(lat, lon, out sector, out xbyte, out low_bits);
            RefreshCache(sector);
            CommitCacheRequired = true;
            if (low_bits) {
		        var src_class = (byte) (value & 0xF);
		        src_class |= (byte) (Cache.Data[xbyte] & 0xF0);
		        Cache.Data[xbyte] = src_class;
	        } else {
                var src_class = (byte) ((value & 0xF) << 4);
		        src_class |= (byte) (Cache.Data[xbyte] & 0x0F);
		        Cache.Data[xbyte] = src_class;
	        }
        }
        #endregion

        

        private byte[] LoadExtended(string extension)
        {
            var dir = Path.GetDirectoryName(extension);
            if (!Directory.Exists(dir)) return null;
            var ds = File.OpenRead(extension);            
            var buffer = new byte[512];
            ds.Read(buffer, 0, 512);            
            return buffer;
        }

        private void StoreExtended(string extension, byte[] data)
        {
            var dir = Path.GetDirectoryName(extension);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var sw = File.OpenWrite(extension))
            {
                sw.Write(data, 0, data.GetLength(0));
                sw.Close();
            }            
        }


        private byte[] GetSector(int sector, ref byte repeated_byte)
        {
            using(var ds = File.OpenRead(FileName))
            {
                long offset = (sector * 512) + 512;
                ds.Seek(offset, SeekOrigin.Begin);
                var buffer = new byte[512];
                ds.Read(buffer, 0, 512);
                repeated_byte = buffer[5];
                var counter = 0;
                for (var i = 5; i < 455; ++i)
                    if (buffer[i] == repeated_byte) counter++;
                if (counter != 450)
                    repeated_byte = 0;
                return buffer;    
            }
        }

        private void StoreM2(int sector, M2Sector M2)
        {
            using (var sw = File.OpenWrite(FileName))
            {
                var data = new byte[512];
                sw.Seek(sector*512 + 512, SeekOrigin.Begin);
                unsafe
                {
                    fixed (byte* ptrBuffer = &data[0])
                    {
                        Marshal.StructureToPtr(M2, new IntPtr(ptrBuffer), true);
                    }
                }
                sw.Write(data, 0, data.GetLength(0));
                sw.Close();
                Repository.TransactionLog.SectorTouch(FileName,sector);
            }
        }

        private void Create(float lat, float lon)
        {
            CommitCacheRequired = false;
            SectorCached = -1;
            Latitude = (short)lat;
            Longitude = (short)lon;
            Headers = new Gr2Headers { 
                Signature = "QF-v0.5",
                base_lat = (short)lat,
                base_lon = (short)lon,
                Revision = 0,
                Status = (char)1
            };

            lock (syncLock)
            {
                Transition(States.LOCKED_AND_SYNCING);
                if (!File.Exists(FileName))
                {
                    //STrace.Trace(typeof(GR2).FullName,2, "QTREE/GR2: Creando archivo de tamaño y layout.");
                    using (var sw = File.Create(FileName))
                    {
                        var data = new byte[512];
                        unsafe
                        {
                            fixed (byte* ptrBuffer = &data[0])
                            {
                                Marshal.StructureToPtr(Headers, new IntPtr(ptrBuffer), true);
                            }
                        }
                        sw.Write(data, 0, 512);
                        data = new byte[512];
                        for(var y=0; y < 60; y++) {
		                    for(var x=0; x < 60; x++) {
			                    sw.Write(data, 0, 512);
                            }
                        }

                        sw.Close();
                    }
                    Transition(States.INSERVICE);
                    return;
                }
                STrace.Debug(typeof(GR2).FullName, String.Format("QT0002: El archivo ya existe. Ruta: {0}", FileName));
                Transition(States.FAILURE);
            }
        }

        private void Open(float lat, float lon)
        {
            CommitCacheRequired = false;
            SectorCached = -1;
            Latitude = (short)lat;
            Longitude = (short)lon;
            lock (syncLock)
            {
                Transition(States.LOCKED_AND_SYNCING);
                if (File.Exists(FileName))
                {
                    //STrace.Trace(typeof(GR2).FullName,2, "QTREE/GR2: Abriendo archivo " + FileName);
                    using (var sw = File.OpenRead(FileName))
                    {
                        var data = new byte[512];
                        sw.Read(data, 0, 512);
                        sw.Close();
                        unsafe
                        {
                            fixed (byte* ptrBuffer = &data[0])
                            {
                                Headers = (Gr2Headers) 
                                        Marshal.PtrToStructure(new IntPtr(ptrBuffer),
                                            typeof(Gr2Headers));
                            }
                        }                        
                    }
                    Transition(States.INSERVICE);
                    return;
                }
                STrace.Debug(typeof(GR2).FullName, String.Format("QT0003: El archivo no existe. Ruta: {0}", FileName));
                Transition(States.FAILURE);
            }
        }

        private static void GetFileSector(float lat, float lon, out int sector, out int xbyte, out bool low_bits)
        {
            lat = Math.Abs(lat);
            lon = Math.Abs(lon);

            STrace.Debug(typeof(GR2).FullName, String.Format("calculando sector de posicion {0} {1}", lat, lon));

            var gr_lat = (int)lat;
            var gr_lon = (int)lon;

            var min_lat = (int)((lat - gr_lat) * 60);
            var min_lon = (int)((lon - gr_lon) * 60);

            var sec_lat = (int)((lat - gr_lat - (min_lat / 60.0)) * 3600);
            var sec_lon = (int)((lon - gr_lon - (min_lon / 60.0)) * 3600);

            var norm_slat = sec_lat >> 1; // div 2
            var norm_slon = sec_lon >> 1; // div 2

            sector = min_lat * 60 + min_lon;
            xbyte = norm_slat * 15 + (norm_slon / 2);
            low_bits = (norm_slon % 2) == 0;

            STrace.Debug(typeof(GR2).FullName, String.Format("Latitude {0}º {1}' {2}\" base={3}", gr_lat, min_lat, sec_lat, min_lat * 60));
            STrace.Debug(typeof(GR2).FullName, String.Format("Longitude {0}º {1}' {2}\" base={3}", gr_lon, min_lon, sec_lon, min_lon));
            STrace.Debug(typeof(GR2).FullName, String.Format("sector = {0} / byte = {1} / en los 4 bits {2}", sector, xbyte, (low_bits ? "bajos" : "altos")));

        }

    }

}
