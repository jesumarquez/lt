#region Usings

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.QuadTree.Data
{
	public class GeoGrillas : ICustomIndex
	{
		#region Constructor

		public GeoGrillas()
		{
			SectorCached = -1;
		}

		#endregion

		#region ICustomIndex

		public void LoadLevel1Cache(float lat, float lon)
		{
			long sector;
			long xbyte;
			bool low_bits;
			var fileName = GetFileNameAndIndexes(lat, lon, out sector, out xbyte, out low_bits);
			if (string.IsNullOrEmpty(fileName)) return;

			if (!File.Exists(fileName))
			{
				Create(fileName);
			}
			else
			{
				//if (string.IsNullOrEmpty(Headers.Signature))
				Open(fileName);
			}
		}

		public void CommitLevel1Cache()
		{
			if (!CommitCacheRequired) return;
			RefreshCache(SectorCached != 0 ? 0 : 1);
		}

		#region Geolocalizacion por Referencia con nombre.

		public T GetReference<T>(float lat, float lon, string name)
		{
			//TO DO
			return default(T);
		}

		public void SetReference<T>(float lat, float lon, string name, T value)
		{
			//TO DO
		}

		#endregion

		#region Funciones de la version 1

		public int GetPositionClass(float lat, float lon)
		{
			if (State != GR2.States.INSERVICE) throw new ApplicationException("El indice esta fuera de servicio");
			long sector;
			long xbyte;
			bool low_bits;
			var fileName = GetFileNameAndIndexes(lat, lon, out sector, out xbyte, out low_bits);
			if (string.IsNullOrEmpty(fileName)) return 0;
			//Debug.WriteLine(String.Format("GGTXXT: GET sector = {0} / byte = {1} / en los 4 bits {2}", sector, xbyte, (low_bits ? "bajos" : "altos")));
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
			if (State != GR2.States.INSERVICE) throw new ApplicationException("El indice esta fuera de servicio");
			long sector;
			long xbyte;
			bool low_bits;
			var fileName = GetFileNameAndIndexes(lat, lon, out sector, out xbyte, out low_bits);
			if (string.IsNullOrEmpty(fileName)) return;
			//Debug.WriteLine(String.Format("GGTXXT: SET sector = {0} / byte = {1} / en los 4 bits {2}", sector, xbyte, (low_bits ? "bajos" : "altos")));
			RefreshCache(sector);
			CommitCacheRequired = true;
			if (low_bits)
			{
				var src_class = (byte)(value & 0xF);
				src_class |= (byte)(Cache.Data[xbyte] & 0xF0);
				Cache.Data[xbyte] = src_class;
			}
			else
			{
				var src_class = (byte)((value & 0xF) << 4);
				src_class |= (byte)(Cache.Data[xbyte] & 0x0F);
				Cache.Data[xbyte] = src_class;
			}
		}

		#endregion

		public IEnumerable Elements(float lat, float lon)
		{
			return new[] { GetPositionClass(lat, lon) };
		}

		#endregion

		#region Private Implementation

		public string GetFileNameAndIndexes(float latF, float lonF, out long numSector, out long numByte, out bool low_bits)
		{
			var gg = Repository.Headers.GridStructure;
			var ggLatGridCount = Convert.ToInt64(gg.Lat_GridCount);
			var ggLonGridCount = Convert.ToInt64(gg.Lon_GridCount);

			var ggLatGrid = Convert.ToInt64(gg.Lat_Grid);
			var ggLonGrid = Convert.ToInt64(gg.Lon_Grid);

			var ggLatLenght = ggLatGrid * ggLatGridCount;
			var ggLonLenght = ggLonGrid * ggLonGridCount;

			var ggLatEnd = gg.Lat_OffSet - ggLatLenght;
			var ggLonEnd = gg.Lon_OffSet + ggLonLenght;

			var ggFileSectorCount = Convert.ToInt64(gg.FileSectorCount);

			var lat = Convert.ToInt64(latF * 10000000);
			var lon = Convert.ToInt64(lonF * 10000000);

			if ((lat > gg.Lat_OffSet) || (lat < ggLatEnd))
			{
				numSector = 0;
				numByte = 0;
				low_bits = true;
				return null;
			}
			if ((lon < gg.Lon_OffSet) || (lon > ggLonEnd))
			{
				numSector = 0;
				numByte = 0;
				low_bits = true;
				return null;
			}

			var scLatLenght = Convert.ToInt64(gg.Lat_Grid) * 32;
			var scLonLenght = Convert.ToInt64(gg.Lon_Grid) * 32;

			var despScLat = Convert.ToInt64(Math.Abs(lat - gg.Lat_OffSet)) / scLatLenght;
			var despScLon = Convert.ToInt64(Math.Abs(lon - gg.Lon_OffSet)) / scLonLenght;

			numSector = despScLat * ggFileSectorCount + despScLon;

			var despByteLat = Math.Abs(lat - gg.Lat_OffSet) / ggLatGrid % 32;
			var despByteLon = Math.Abs(lon - gg.Lon_OffSet) / ggLonGrid % 32;

			numByte = despByteLat * 16 + despByteLon / 2;
			low_bits = despByteLon % 2 == 0;

			return GetFileName();
		}

		private string GetFileName()
		{
			return String.Format(@"{0}\GG-000000.DAT", Repository.BaseFolderPath);
		}

		private string _headerFileName;

		private string HeaderFileName
		{
			get
			{
				if (string.IsNullOrEmpty(_headerFileName))
				{
					_headerFileName = String.Format(@"{0}\GRIDSTRUCTURE.DAT", Repository.BaseFolderPath);
				}
				//STrace.Debug(GetType().FullName,"GG: header file name={0}", _headerFileName);
				return _headerFileName;
			}
		}

		private void Create(string FileName)
		{
			CommitCacheRequired = false;
			SectorCached = -1;
            var countSectors =
                Math.Ceiling(Repository.Headers.GridStructure.Lat_GridCount / 32.0) * // Cantidad de Grillas en Latitud
                Math.Ceiling(Repository.Headers.GridStructure.Lon_GridCount / 32.0); // Cantidad de Grillas en Longitud

			lock (syncLock)
			{
				Transition(GR2.States.LOCKED_AND_SYNCING);
				if (!File.Exists(FileName))
				{
					FileManager.CreateFile(FileName, (int) countSectors + 1);
					Transition(GR2.States.INSERVICE);
					return;
				}
				STrace.Debug(GetType().FullName, String.Format("GG0002: El archivo ya existe. Ruta: {0}", FileName));
				Transition(GR2.States.FAILURE);
			}
		}

		private void Open(string FileName)
		{
			CommitCacheRequired = false;
			SectorCached = -1;
			lock (syncLock)
			{
				Transition(GR2.States.LOCKED_AND_SYNCING);
				if (File.Exists(FileName))
				{
					using (var sw = File.OpenRead(FileName))
					{
						var data = new byte[512];
						sw.Read(data, 0, 512);
						sw.Close();
						unsafe
						{
							fixed (byte* ptrBuffer = &data[0])
							{
								Marshal.PtrToStructure(new IntPtr(ptrBuffer), typeof(GridStructure));
							}
						}
					}
					Transition(GR2.States.INSERVICE);
					return;
				}
				STrace.Debug(GetType().FullName, String.Format("GG0003: El archivo no existe. Ruta: {0}", FileName));
				Transition(GR2.States.FAILURE);
			}
		}

		private void Transition(GR2.States newState)
		{
			lock (syncLock)
			{
				if (State == newState) return;
				STrace.Debug(GetType().FullName, String.Format("GG/GR2: Store File Transition. {0} -> {1}", State.ToString(), newState.ToString()));
				State = newState;
			}
		}

		public Repository Repository { get; set; }

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct GGSector
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
			public byte[] Data;
		};

		private readonly object syncLock = new object();
		private GR2.States State { get; set; }
		private long SectorCached;
		private bool CommitCacheRequired;
		private GGSector Cache;

		#endregion

		#region Cache de nivel 1

		private void RefreshCache(long SectorNumber)
		{
			if (SectorCached == SectorNumber) return;
			if (SectorCached != -1 && CommitCacheRequired)
			{
				CommitCacheRequired = false;
				StoreGGSector(SectorCached, Cache);
			}
			var data = LoadGGSector(SectorNumber);
			try
			{
				unsafe
				{
					fixed (byte* ptrBuffer = &data[0])
					{
						Cache = (GGSector)Marshal.PtrToStructure(new IntPtr(ptrBuffer), typeof(GGSector));
					}
				}
				SectorCached = SectorNumber;
			}
			catch
			{
				SectorCached = -1;
			}
		}

		private void StoreGGSector(long sectorNumber, GGSector sector)
		{
			var fileName = GetFileName();
			using (var sw = File.OpenWrite(fileName))
			{
				var data = new byte[512];
				var fileSector = sectorNumber;// % SectorsPerFile;
				sw.Seek(fileSector * 512 + 512, SeekOrigin.Begin);
				unsafe
				{
					fixed (byte* ptrBuffer = &data[0])
					{
						Marshal.StructureToPtr(sector, new IntPtr(ptrBuffer), true);
					}
				}
				sw.Write(data, 0, data.GetLength(0));
				sw.Close();
				Repository.TransactionLog.SectorTouch(fileName, (int)fileSector);
			}
		}

		private byte[] LoadGGSector(long SectorNumber)
		{
			var fileSector = SectorNumber;// % SectorsPerFile;
			using (var ds = File.OpenRead(GetFileName()))
			{
				ds.Seek(fileSector * 512 + 512, SeekOrigin.Begin);
				var buffer = new byte[512];
				ds.Read(buffer, 0, 512);
				return buffer;
			}
		}

		#endregion

		#region SaneImplementation

		public void getSector(long SectorIndex, byte[] buffer)
		{
			var fileSector = SectorIndex;// % SectorsPerFile;
			using (var ds = File.OpenRead(GetFileName()))
			{
				ds.Seek(fileSector * 512 + 512, SeekOrigin.Begin);
				ds.Read(buffer, 0, 512);
			}
		}

		public void get2Sectors(long SectorIndex, byte[] buffer)
		{
			var fileSector = SectorIndex;// % SectorsPerFile;
			using (var ds = File.OpenRead(GetFileName()))
			{
				ds.Seek(fileSector * 512 + 512, SeekOrigin.Begin);
				ds.Read(buffer, 0, 1024);
			}
		}

		public GridStructure GridStructure
		{
			get
			{
				if (File.Exists(HeaderFileName))
				{
					using (var sw = File.OpenRead(HeaderFileName))
					{
						sw.Seek(0, SeekOrigin.Begin);
						var data = new byte[512];
						sw.Read(data, 0, 512);
						sw.Close();
						unsafe
						{
							fixed (byte* ptrBuffer = &data[0])
							{
								return (GridStructure)Marshal.PtrToStructure(new IntPtr(ptrBuffer), typeof(GridStructure));
							}
						}
					}
				}
				throw new FileNotFoundException(HeaderFileName);
			}
		}

		#endregion
	}
}