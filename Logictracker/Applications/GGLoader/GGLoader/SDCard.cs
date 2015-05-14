using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;

namespace GGLoader
{
	public static class SDCard
	{
		public static String[] GetSDCards()
		{
			return DriveInfo
				.GetDrives()
				.Where(d => d.DriveType == DriveType.Removable)
				.Where(d => !(new[] { 'A', 'B', 'C' }).Contains(d.RootDirectory.ToString()[0]))
				.Select(di => di.RootDirectory.ToString().TrimEnd('\\'))
				.ToArray();
		}

		public static String[] GetAllDevices()
		{
			return DriveInfo
				.GetDrives()
				.Where(d => !(new[] { 'A', 'B', 'C' }).Contains(d.RootDirectory.ToString()[0]))
				.Select(di => di.RootDirectory.ToString().TrimEnd('\\'))
				.ToArray();
		}

		public static void LoadGeogrillaFile(String GGPath, String RootDir, ProgressBar progressBar1, Label label2, Panel panel1, bool formatFlag, String GGName, int bufsize)
		{
			var worker = new LoadGeogrillaFileDelegate(LoadGeogrillaFileWorker);
			worker.BeginInvoke(new WorkerData(GGPath, RootDir, progressBar1, label2, panel1, formatFlag, GGName, bufsize), LoadGeogrillaFileWorkerResult, null);
		}

		private delegate String LoadGeogrillaFileDelegate(WorkerData wd);

		private static String LoadGeogrillaFileWorker(WorkerData wd)
		{
			try
			{
				using (var sd = new SDManagement(wd.RootDir))
				{
					using (var fileStream2 = new FileStream(wd.GGPath, FileMode.Open, FileAccess.Read))
					{
						if (wd.formatFlag)
						{
							sd.fileStream.Seek(0, SeekOrigin.Begin);
							var sector = new SdInfo(wd.GGName, sd.SDSize).ToByteArray();
							sd.fileStream.Write(sector, 0, 512);
						}

						var bufferLength = wd.bufsize;
						var bytes = new Byte[bufferLength];
						sd.fileStream.Seek(512, SeekOrigin.Begin);
						var pending = (int) fileStream2.Length;
						var total = pending;
						wd.progressBar1.BeginInvoke((MethodInvoker) (() =>
						                                             	{
						                                             		wd.progressBar1.Minimum = 0;
						                                             		wd.progressBar1.Maximum = pending;
						                                             		wd.progressBar1.Value = 0;
						                                             	}));

						var startTime = DateTime.UtcNow;
						var acumulated = 0;

						var pasada = 0;
						while (pending > 0)
						{
							var readed = fileStream2.Read(bytes, 0, bufferLength);
							sd.fileStream.Write(bytes, 0, readed);
							pending -= readed;
							acumulated += readed;

							pasada++;

							if (pasada % 10 != 1) continue;

							wd.progressBar1.BeginInvoke((MethodInvoker)(() => { if (wd.progressBar1.Maximum >= wd.progressBar1.Value + acumulated) wd.progressBar1.Value += acumulated; acumulated = 0; }));

							var processed = total - pending;
							var time = TimeSpan.FromSeconds((DateTime.UtcNow - startTime).TotalSeconds * (pending * 1.0 / processed));
							var texto = String.Format("{0:00} horas {1}' {2}''", time.TotalHours, time.Minutes, time.Seconds);
							wd.label2.BeginInvoke((MethodInvoker)(() => wd.label2.Text = texto));
						}
					}
					return "Exito!";
				}
			}
			catch (Exception e)
			{
				return e.Message;
			}
			finally
			{
				wd.progressBar1.BeginInvoke((MethodInvoker)(() => { wd.progressBar1.Value = 0; }));
				wd.label2.BeginInvoke((MethodInvoker)(() => wd.label2.Text = "00 horas 0' 0''"));
				wd.panel1.BeginInvoke((MethodInvoker)(() => wd.panel1.Enabled = true));
			}
		}

		private static void LoadGeogrillaFileWorkerResult(IAsyncResult result)
		{
			var async = (AsyncResult)result;
			var loader = (LoadGeogrillaFileDelegate)async.AsyncDelegate;
			var res = loader.EndInvoke(result);
			MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Resultado: {0}", res), "Proceso Finalizado");
		}

		private class WorkerData
		{
			public readonly String GGPath;
			public readonly String RootDir;
			public readonly ProgressBar progressBar1;
			public readonly Label label2;
			public readonly Panel panel1;
			public readonly bool formatFlag;
			public readonly String GGName;
			public readonly int bufsize;

			public WorkerData(String _GGPath, String _RootDir, ProgressBar _progressBar1, Label _label2, Panel _panel1, bool _formatFlag, String _GGName, int _bufsize)
			{
				GGPath = _GGPath;
				RootDir = _RootDir;
				progressBar1 = _progressBar1;
				label2 = _label2;
				panel1 = _panel1;
				formatFlag = _formatFlag;
				GGName = _GGName;
				bufsize = _bufsize;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
		private struct SdInfo
		{
			//_SDFatInfo
			private readonly UInt32 ActiveKey_SDFatInfo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			private readonly Byte[] IdeKey;
			private readonly UInt32 CreationTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
			private readonly Byte[] Name_SDFatInfo_Serial;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			private readonly Byte[] nop;

			//_SDPartInfo
			private readonly UInt16 ActiveKey_SDPartInfo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			private readonly Byte[] PartType;
			private readonly UInt32 PartStart;
			private readonly UInt32 PartSize;
			private readonly UInt32 IndexSize;
			private readonly UInt32 PagCount;
			private readonly UInt32 PagSize;
			private readonly UInt32 ObjCount;
			private readonly UInt32 ObjSize;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			private readonly Byte[] Name_SDPartInfo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 440)]
			private readonly Byte[] ceros;

			// ReSharper disable UnusedParameter.Local
			public SdInfo(String GGName, long SDSectorsCount)
			// ReSharper restore UnusedParameter.Local
			{
				//_SDFatInfo
				ActiveKey_SDFatInfo = 0x55AA55AA;
				IdeKey = Encoding.ASCII.GetBytes("TRAX");
				CreationTime = 0x4FBC0D0B; //TODO: averiguar en que esta expresado y calcular con DateTime.UtcNow
				Name_SDFatInfo_Serial = Encoding.ASCII.GetBytes("0005040590\0\0");
				nop = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

				//_SDPartInfo
				ActiveKey_SDPartInfo = 0x55AA;
				PartType = Encoding.ASCII.GetBytes("G1");
				PartStart = 1; //Inicio de particion en sectores
				PartSize = 0x0039FBC1; //En cantidad de sectores //TODO: calcular en base al tamaño de la sd
				IndexSize = 1; //Tamaño del indice en sectores
				PagCount = 1; //Cantidad de PAGINAS dentro del indice
				PagSize = 0x0039FBC0; //Tamaño de la PAGINAS en sectores //TODO: calcular en base al tamaño de la sd
				ObjCount = 0x0039FBC0; //Cantidad de Objetos
				ObjSize = 512; //Tamaño de cada Objeto en BYTE
				Name_SDPartInfo = Encoding.ASCII.GetBytes(GGName.PadRight(8, '\0'));
				ceros = Enumerable.Repeat((Byte)0, 512 - 72).ToArray();
			}

			public byte[] ToByteArray()
			{
				var rawsize = Marshal.SizeOf(this);
				var buffer = Marshal.AllocHGlobal(rawsize);
				Marshal.StructureToPtr(this, buffer, false);
				var rawdata = new byte[rawsize];
				Marshal.Copy(buffer, rawdata, 0, rawsize);
				Marshal.FreeHGlobal(buffer);
				return rawdata;
			}
		}

	}
}
