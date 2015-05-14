#region Usings

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.QuadTree.Data
{
    public class Repository //: ObservableCollection<QuadTreeFile>
    {
        public string BaseFolderPath { get; set; }
        public int  Revision { get; internal set; }
        public TransactionLog TransactionLog { get; private set; }
        public Headers Headers { get; private set; }
        public ICustomIndex IndexFile { get; private set; }

        public int ZoomLevel { get { return 15; } }

        public double HorizontalResolution
        {
            get {
                if (IndexFile is GR2) return 1.0 / 60.0 / 30.0;
                return Headers.GridStructure.Lon_Grid/10000000.0; 
            }
        }

        public double VerticalResolution
        {
            get
            {
                if (IndexFile is GR2) return 1.0 / 60.0 / 30.0;
                return Headers.GridStructure.Lat_Grid / 10000000.0;
            }
        }


        private static Repository _instance;
        private static readonly object lockInstance = new object();

        public static Repository Instance
        {
            get
            {
                lock(lockInstance)
                {
                    if (_instance == null)
                    {
                        const string dirname = @"C:\GEOGRILLAS";
                        var so = new GridStructure();
                        _instance = new Repository();
                        _instance.Open<GeoGrillas>(dirname, ref so);
                    }
                    return _instance;
                }
            }
        }

        public bool Init<TYPE>(string baseFolderPath, GridStructure structure) where TYPE : ICustomIndex, new()
        {
            Revision = 0;
            BaseFolderPath = baseFolderPath;

            if (!Directory.Exists(BaseFolderPath))
            {                
                Directory.CreateDirectory(BaseFolderPath);
                STrace.Debug(GetType().FullName,String.Format("QT1000, El directorio especificado fue creado. ruta: {0}.", BaseFolderPath));
                // TODO: patchear permisos
            }
            else
            {
                if (Directory.GetFiles(BaseFolderPath).GetLength(0) > 0 ||
                    Directory.GetDirectories(BaseFolderPath).GetLength(0) > 0)
                {
                    STrace.Debug(GetType().FullName, String.Format("QT0001, El directorio especificado debe estar vacio para proceder a la inicializacion. La ruta: {0} tiene {1} archivos y {2} directorios.", BaseFolderPath, Directory.GetFiles(BaseFolderPath).GetLength(0), Directory.GetDirectories(BaseFolderPath).GetLength(0)));
                    return false;
                }
 
            }
            IndexFile = new TYPE { Repository = this};
            TransactionLog = new TransactionLog(this, 0);
            Headers = new Headers(this);
            
            Headers.Init(structure);
            return true;
        }

        public bool Open<TYPE>(string baseFolderPath, ref GridStructure structure)where TYPE : ICustomIndex, new()
        {
            Debug.Assert(baseFolderPath != null);

            Revision = 0;
            BaseFolderPath = baseFolderPath;

            if (!Directory.Exists(BaseFolderPath))
            {
                STrace.Debug(GetType().FullName, String.Format("QT0002, No se puede abrir el repositorio pues no existe el directorio especificado ({0}).", BaseFolderPath));
                return false;
            }
            
            TransactionLog = new TransactionLog(this, 0);
            Headers = new Headers(this);
            Headers.Open(ref structure);
            IndexFile = new TYPE { Repository = this };
            return true;
        }

        public void Close()
        {
            if (IndexFile != null) IndexFile.CommitLevel1Cache();
        }

        private void RefreshCache(float lat, float lon)
        {
            if (IndexFile != null) IndexFile.LoadLevel1Cache(lat, lon);
        }

        public IEnumerable Elements(float lat, float lon) 
        {
            RefreshCache(lat, lon);
            return IndexFile != null ? IndexFile.Elements(lat, lon) : null;
        }

        public int GetPositionClass(float lat, float lon)
        {
            RefreshCache(lat, lon);
            if (IndexFile == null) return 0;
            return IndexFile.GetPositionClass(lat, lon);
        }

        public TYPE GetReference<TYPE>(float lat, float lon, string name)
        {
            RefreshCache(lat, lon);
            if (IndexFile == null) return default(TYPE);
            return IndexFile.GetReference<TYPE>(lat, lon, name);
        }

        private readonly object spcLock = new object();
        public void SetPositionClass(float lat, float lon, int value)
        {
            lock(spcLock) {
                RefreshCache(lat, lon);
                if (IndexFile == null) return;
                IndexFile.SetPositionClass(lat, lon, value);
            }
        }

        public void SetReference<TYPE>(float lat, float lon, string name, TYPE value)
        {
            RefreshCache(lat, lon);
            if (IndexFile == null) return;
            IndexFile.SetReference(lat, lon, name, value);
        }

        /*public IEnumerable Elements(float lat, float lon, int margen_celdas, )
        {
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
        }*/


        public byte[] GetSector(string name, int sector, ref byte repeated_byte)
        {
            var ds = File.OpenRead(string.Format("{0}\\{1}", BaseFolderPath, name));
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
}
