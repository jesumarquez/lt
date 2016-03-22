using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.QuadTree.Data;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Alas
{
    public partial class Parser : IQuadtree
    {

        #region IQuadtree

        public bool SyncronizeQuadtree(ulong messageId, bool full, int baseRevision)
        {
            try
            {
                var QTreeFile = DataProvider.GetDetalleDispositivo(Id, "PARAM_QTREE_FILE").As("");

                if (String.IsNullOrEmpty(QTreeFile))
                {
                    STrace.Error(GetType().FullName, Id, @"No se fotea geogrilla por no estar el Detalle ""PARAM_QTREE_FILE""");
                    return false;
                }

                if (baseRevision == 0 && !full)
                {
                    STrace.Error(GetType().FullName, Id, @"No se fotea Geogrilla diferencial ya que el detalle ""known_qtree_revision"" es cero, si quiere enviar toda la geogrilla utilize el comando ""Enviar Full Qtree""");
                    return false;
                }
                var td = new TaskData
                {
                    Device = this,
                    Path = Path.Combine(Config.Qtree.QtreeGteDirectory, QTreeFile),
                    BaseRev = baseRevision,
                    Full = full,
                    MessageId = messageId,
                };
                ThreadPool.QueueUserWorkItem(CreateGeogrillaTxt, td);
                return true;
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e, Id);
                return false;
            }
        }

        #endregion

        #region Level 0

        private class TaskData
        {
			public Parser Device;
            public String Path;
            public int BaseRev;
            public Boolean Full;
            public ulong MessageId;
        }

        private DateTime LastGeogrillaSentTimestamp = DateTime.MinValue;

        private static void CreateGeogrillaTxt(Object obj)
        {
            var taskData = (TaskData)obj;
			taskData.Device.ExecuteOnGuard(() => CreateGeogrillaTxtInternal(taskData), "CreateGeogrillaTxt", "");
        }

        private static void CreateGeogrillaTxtInternal(TaskData td)
        {
            try
            {
				if (td.Device.LastGeogrillaSentTimestamp.AddMinutes(30) > DateTime.UtcNow)
                {
					STrace.Log(typeof(Mensaje).FullName, null, td.Device.Id, td.Full ? LogTypes.Error : LogTypes.Debug, null, String.Format("No Foteo Geogrilla {0} por tiempo de espera minimo entre actualizaciones no alcanzado, ultima actualizacion: {1}", td.Full ? "full" : "incremental", td.Device.LastGeogrillaSentTimestamp.ToLocalTime()));
                    return;
                }
				td.Device.LastGeogrillaSentTimestamp = DateTime.UtcNow;

                var gg = new GeoGrillas
                    {
                        Repository = new Repository
                            {
                                BaseFolderPath = td.Path,
                            }
                    };
                var headers = gg.GridStructure;
                //si no esta bien cargado el header fix it
                if (headers.ActiveKey != 0x55AA55AA)
                {
                    headers.ActiveKey = 0x55AA55AA;
                    headers.DataStart = 1;
                }

                //si hay en fota un envio de qtree usar esa revision como base
				var fotaRev = GetPendingFotaRevision(td.Device);
                if (fotaRev > td.BaseRev) td.BaseRev = fotaRev;

                int revision;
				var changedSectorsList = new TransactionLog(gg.Repository, td.Device.Id)
                    .GetChangedSectorsAndRevision(td.BaseRev, out revision)
                    .SelectMany(file => file.Value, (file, sectorindex) => (file.Key) + sectorindex)
                    .ToArray();

                if (td.Full)
                {
                    var countSectors = (int)(Math.Ceiling(headers.Lat_GridCount / 32.0) * Math.Ceiling(headers.Lon_GridCount / 32.0));
                    changedSectorsList = Enumerable.Range(0, countSectors).ToArray();
                }

                if (changedSectorsList.Length == 0)
                {
					STrace.Debug(typeof(Parser).FullName, td.Device.Id, String.Format("Cancelando envio de geogrilla por no haber sectores cambiados, BaseRev={0} LatestRev={1}", td.BaseRev, revision));
                    return;
                }

				var limit = td.Device.DataProvider.GetDetalleDispositivo(td.Device.Id, "LimiteGeogrillaIncremental").As(20480);
                if (!td.Full && changedSectorsList.Length > limit)
                {
                    //limite en el tamaño del diferencial generado de x sectores o megas, configurable en la plataforma con el detalle de dispositivo "LimiteGeogrillaIncremental".
					STrace.Trace(typeof(Parser).FullName, td.Device.Id, String.Format(@"Cancelando envio de geogrilla incremental por superar el limite de sectores (detalle de dispositivo ""LimiteGeogrillaIncremental""): limite={0} cantidad={1}", limit, changedSectorsList.Length));
                    return;
                }

                //seteo desde ya la revision en la base por si vuelven a enviar un qtree diferencial que se sume solo lo nuevo
				td.Device.DataProvider.SetDetalleDispositivo(td.Device.Id, "known_qtree_revision", revision.ToString("D4"), "int");

				STrace.Debug(typeof(Parser).FullName, td.Device.Id, String.Format("Generando Geogrilla, BaseRev={0} LatestRev={1} GGName={2}", td.BaseRev, revision, td.Path));

                if (td.Full)
                {
					var head = td.Device.DataProvider.GetDetalleDispositivo(td.Device.Id, "FullQtreePartitionsSd").AsBoolean(false) 
						? String.Format(
							">SUV29*<{0}>SUVU55AA<{0}>SSDF,|G1,{1},TestMap<{0}>SUV29AQSDF,14,3,|G1<{0}", 
							Environment.NewLine,
							td.Device.DataProvider.GetDetalleDispositivo(td.Device.Id, "QtreeSdPartitionSize").As("3800000"))
						: String.Empty;
					Fota.EnqueueLowPriority1(td.Device, 0, SetGGParams(headers, head));
                }

				Fota.EnqueueLowPriority1(td.Device, 0, Fota.VirtualMessageFactory(MessageIdentifier.QtreeStart, td.Device.NextSequence));

				ProccessSectors(td, changedSectorsList, gg, td.Device);

                var bytes = BitConverter.GetBytes(revision);
                var hexa = StringUtils.ByteArrayToHexString(bytes, 0, bytes.Length);
                var salida = String.Format("{1}>SSDG1,W0000008004{0}<{2}{1}>QSDG1,R0000008004<{1}", hexa, Environment.NewLine, Fota.VirtualMessageFactory(MessageIdentifier.QtreeSuccess, 0));
				Fota.EnqueueLowPriority2(td.Device, td.MessageId, salida);

				STrace.Trace(typeof(Parser).FullName, td.Device.Id, String.Format("geogrilla a revision: {0} desde: {1} cantidad de sectores: {2}", revision, td.BaseRev, changedSectorsList.Length));

            }
            catch (Exception e)
            {
				STrace.Exception(typeof(Parser).FullName, e, td.Device.Id, "Exception during CreateGeogrillaTxt");
            }
        }

        #endregion

        #region Level 1

        private static void ProccessSectors(TaskData td, int[] pendingSectorsList, GeoGrillas gg, Parser dev)
        {

            var enviarceros = true;
            if (td.Full)
            {
                enviarceros = dev.DataProvider.GetDetalleDispositivo(dev.Id, "SendZerosQTree").AsBoolean(false);
            }
            var buffer = new byte[BytesInSector * 2];
            var sbZeros = new StringBuilder();
            var sbNonZeros = new StringBuilder();
            var totales = pendingSectorsList.Length;
            for (var index = 0; index < totales; index++)
            {
                var act = pendingSectorsList[index];
                var nxt = (totales < (index + 2)) ? -1 : pendingSectorsList[index + 1];

                int pending;

                if (act + 1 == nxt)
                {
                    gg.get2Sectors(act, buffer);
                    pending = BytesInSectorTimes2;
                    index++;
                }
                else
                {
                    gg.getSector(act, buffer);
                    pending = BytesInSector;
                }

                Proccess1Or2Sectors(dev, act, buffer, sbZeros, sbNonZeros, totales, pending, enviarceros);
            }
        }

        private static void Proccess1Or2Sectors(Parser dev, int sectorNumber, byte[] sector, StringBuilder sbZeros, StringBuilder sbNonZeros, int totales, int pending, bool enviarceros)
        {
            var start = 0;

            while (pending > 0)
            {
                int count;
                bool zerosFlag;
                bool isRepeated;
                GetNextChunkCount(sector, start, pending, out count, out zerosFlag, out isRepeated);

                if (!zerosFlag)
                    STrace.Debug(typeof(Parser).FullName, dev.Id, String.Format("sec={0:D7}/{1} s={2:D3} c={3:D3}{4}", sectorNumber, (sectorNumber * 100) / totales, start, count, isRepeated ? " Repetidos" : ""));

                var sb = zerosFlag ? sbZeros : sbNonZeros;
                var offset = (UInt32)((sectorNumber + 1) * BytesInSector + start);
                var cmd = isRepeated ? SdRepeatedChunk(offset, sector[start], count) : SdChunk(offset, sector, start, count);

                if (sb.Length + cmd.Length > MaxCharsPerCommand)
                {
                    if (sb.Length > cmd.Length)
                    {
                        SendSdWriteCommand(dev, sb, zerosFlag, enviarceros);
                        sb.Append(cmd);
                    }
                    else
                    {
                        var sbt = new StringBuilder(cmd);
                        SendSdWriteCommand(dev, sbt, zerosFlag, enviarceros);
                    }
                }
                else
                {
                    sb.Append(cmd);
                }

                start += count;
                pending -= count;
            }
        }

        #endregion

        #region Level 2

        private static void GetNextChunkCount(byte[] sector, int start, int pending, out int count, out bool zerosFlag, out bool repeatedFlag)
        {
            if (IsRepeatedSeriesStart(sector, start, pending, MinimumRepeated1))
            {
                count = GetRepeatedChunkCount(sector, start, pending, MinimumRepeated1);
                zerosFlag = sector[start] == 0;
                repeatedFlag = true;
            }
            else
            {
                count = GetNonRepeatedChunkCount(sector, start, pending);
                zerosFlag = false;
                repeatedFlag = false;
            }
        }

        private static bool IsRepeatedSeriesStart(byte[] sector, int start, int pending, int minimum)
        {
            if (pending < minimum) return false;

            var compared = sector[start];
            var limit = start + minimum;
            for (var i = start + 1; i < limit; i++) if (compared != sector[i]) return false;
            return true;
        }

        private static int GetRepeatedChunkCount(byte[] buffer, int start, int pending, int minimum)
        {
            if (pending > MaxRepeatedBytesPerChunk) pending = MaxRepeatedBytesPerChunk;
            var limit = start + pending;
            var compared = buffer[start];
            var RepeatedCount = minimum;
            for (var i = start + minimum; i < limit; i++)
            {
                if (buffer[i] != compared) break;
                RepeatedCount++;
            }

            return RepeatedCount;
        }

        private static int GetNonRepeatedChunkCount(byte[] sector, int start, int limitcount)
        {
            if (limitcount > MaxBytesPerChunk) limitcount = MaxBytesPerChunk;

            //get count while not init repeated
            var limit = start + limitcount;
            var cant = 1;
            for (var i = start + 1; i < limit; i++)
            {
                if (IsRepeatedSeriesStart(sector, i, limit - i, MinimumRepeated2)) break;
                cant++;
            }
            return cant;
        }

        private const int MinimumRepeated1 = 5; //minima cantidad de repetidos en el *principio* de una cadena para que valga la pena comprimirlos
        private const int MinimumRepeated2 = 10; //minima cantidad de repetidos en el *medio* de una cadena para que valga la pena comprimirlos
        private const int MaxBytesPerChunk = 0x40;
        private const int MaxRepeatedBytesPerChunk = 0x80;
        private const int MaxCharsPerCommand = 0x84;
        private const int BytesInSector = 0x200;
        private const int BytesInSectorTimes2 = BytesInSector * 2;

        #endregion

        #region Level 3

        private static int GetPendingFotaRevision(IFoteable Device)
        {
            var line = Fota.FindLast(Device, ">SSDG1,W0000008004", true) ?? ">SSDG1,W000000800400000000<";
            line = line.Substring(line.IndexOf("04") + 2).TrimEnd('<');
            var bytes = StringUtils.HexStringToByteList(line, 0).ToArray();
            var value = BitConverter.ToInt32(bytes, 0);
            return value;
        }

        private static String SetGGParams(GridStructure gg, String head)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.ActiveKey));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.DataStart));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.FileSectorCount));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.Lat_OffSet));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.Lon_OffSet));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.Lat_Grid));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.Lon_Grid));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.Lat_GridCount));
            bytes.AddRange(BitConverter.GetBytes((UInt32)gg.Lon_GridCount));

            return String.Format("{0}>SSDG1,W{1}<{2}", head, SdChunk(0, bytes.ToArray(), 0, bytes.Count), Environment.NewLine);
        }

        private static void SendSdWriteCommand(Parser dev, StringBuilder sb, bool allzeros, bool enviarceros)
        {
            sb.Insert(0, ">SSDG1,W");
            sb.Append("<");
            var cmd = sb.ToString();
            sb.Remove(0, sb.Length);

            if (!allzeros)
                Fota.EnqueueLowPriority1(dev, 0, cmd);
            else if (enviarceros)
                Fota.EnqueueLowPriority2(dev, 0, cmd);
        }

        private static String SdChunk(UInt32 offset, byte[] bytes, int start, int count)
        {
            var hexa = StringUtils.ByteArrayToHexString(bytes, start, count);
            return String.Format("{0:X8}{1:X2}{2}", offset, count, hexa);
        }

        private static String SdRepeatedChunk(UInt32 offset, byte bytevalue, int count)
        {
            return String.Format("{0:X8}{1:X2}{2:X2}", offset, 0x7F + count, bytevalue);
        }

        #endregion
    }
}
