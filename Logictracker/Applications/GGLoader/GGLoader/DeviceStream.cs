using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace GGLoader
{
	public class SDManagement : IDisposable
	{
		#region Members

		public SDManagement(String RootDir)
		{
			var de1 = new DiskExtents();
			var bytesReturned = 0;

			try
			{
				driveLetter = RootDir;
				var path = @"\\.\" + driveLetter;
				using (var sfh = CreateFile(path, EFileAccess.GenericRead | EFileAccess.GenericWrite, EFileShare.Read | EFileShare.Write, IntPtr.Zero, ECreationDisposition.OpenExisting, 0, IntPtr.Zero))
				{
					if (sfh.IsInvalid) throw new Win32Exception(Marshal.GetLastWin32Error(), "Handle.IsInvalid");

					using (new FileStream(sfh, FileAccess.Read))
					{
						if (DeviceIoControl(sfh, DeviceIoControlCode.IoctlVolumeGetVolumeDiskExtents, IntPtr.Zero, 0, ref de1, Marshal.SizeOf(de1), ref bytesReturned, IntPtr.Zero))
						{
							SDPhysicalName = @"\\.\PhysicalDrive" + de1.first.DiskNumber.ToString(CultureInfo.InvariantCulture);

							var dg1 = new DISK_GEOMETRY();
							if (!DeviceIoControl(sfh, DeviceIoControlCode.IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0, ref dg1, Marshal.SizeOf(dg1), ref bytesReturned, IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error(), "IOCTL_DISK_GET_DRIVE_GEOMETRY");
							SDSize = dg1.DiskSize;
							if (!DeviceIoControl(sfh, DeviceIoControlCode.FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, ref de1, Marshal.SizeOf(de1), ref bytesReturned, IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error(), "FSCTL_LOCK_VOLUME");
							if (!DeviceIoControl(sfh, DeviceIoControlCode.FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, ref de1, Marshal.SizeOf(de1), ref bytesReturned, IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error(), "FSCTL_DISMOUNT_VOLUME");
							//if (IsWinVistaOrHigher())
							{
								deviceName = GetDeviceName(driveLetter);
								if (!String.IsNullOrEmpty(deviceName) && !DefineDosDevice(EParam.DDD_RAW_TARGET_PATH | EParam.DDD_REMOVE_DEFINITION | EParam.DDD_EXACT_MATCH_ON_REMOVE, driveLetter, deviceName)) throw new Win32Exception(Marshal.GetLastWin32Error(), "DefineDosDevice");
							}
						}
						var lerror = Marshal.GetLastWin32Error();

						if (lerror != 0)
						{
							if (lerror == 1) throw new Win32Exception(lerror, String.Format(@"IncorrectFunction The drive ""{0}"" is removable and removed, like a CDRom with nothing in it.)", driveLetter));
							if (lerror == 122) throw new Win32Exception(lerror, "ErrorInsufficientBuffer");
							throw new NotSupportedException("Multiple Disc Volume");
						}
					}
				}
			}
			catch (Exception e)
			{
				if (!e.Message.StartsWith("External component has thrown an exception.")) throw;
			}

			diskHandle = CreateFile(SDPhysicalName, EFileAccess.GenericRead | EFileAccess.GenericWrite, EFileShare.Read | EFileShare.Write, IntPtr.Zero, ECreationDisposition.OpenExisting, EFileAttributes.Device | EFileAttributes.NoBuffering | EFileAttributes.Write_Through, IntPtr.Zero);
			if (diskHandle.IsInvalid) throw new Win32Exception(Marshal.GetLastWin32Error(), "Handle.IsInvalid");
			if (!DeviceIoControl(diskHandle, DeviceIoControlCode.FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, ref de1, Marshal.SizeOf(de1), ref bytesReturned, IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error(), "FSCTL_LOCK_VOLUME");
			fileStream = new FileStream(diskHandle, FileAccess.ReadWrite);
		}

		private readonly SafeFileHandle diskHandle;
		private readonly String deviceName;
		private readonly String driveLetter;
		private readonly String SDPhysicalName;

		public readonly long SDSize;
		public readonly FileStream fileStream;

		public void Dispose()
		{
			try
			{
				var de1 = new DiskExtents();
				var bytesReturned = 0;
				if (!DeviceIoControl(diskHandle, DeviceIoControlCode.FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, ref de1, Marshal.SizeOf(de1), ref bytesReturned, IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error(), "FSCTL_LOCK_VOLUME");

				//fileStream.Dispose();
				diskHandle.Dispose();
			}
			catch (Exception e)
			{
				if (!e.Message.StartsWith("External component has thrown an exception.")) throw;
			}
		}

		private static string GetDeviceName(string strDriveLetterAndColon)
		{
			const int MAX_PATH = 260;
			var mem = Marshal.AllocHGlobal(MAX_PATH);
			var ret = QueryDosDevice(strDriveLetterAndColon, mem, MAX_PATH);
			return ret != 0 ? Marshal.PtrToStringAnsi(mem, ret) : null;
		}

		private static bool IsWinVistaOrHigher()
		{
			var OS = Environment.OSVersion;
			return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
		}

		#endregion

		#region DllImports

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern SafeFileHandle CreateFile(String lpFileName, EFileAccess dwDesiredAccess, EFileShare dwShareMode, IntPtr lpSecurityAttributes, ECreationDisposition dwCreationDisposition, EFileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeviceIoControl(SafeFileHandle hVol, DeviceIoControlCode controlCode, IntPtr inBuffer, int inBufferSize, ref DiskExtents outBuffer, int outBufferSize, ref int bytesReturned, IntPtr overlapped);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool DeviceIoControl(SafeFileHandle hVol, DeviceIoControlCode controlCode, IntPtr inBuffer, int nInBufferSize, ref DISK_GEOMETRY outBuffer, int outBufferSize, ref int bytesReturned, IntPtr overlapped);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool DefineDosDevice(EParam dwFlags, string lpDeviceName, String lpTargetPath);

		[DllImport("kernel32", SetLastError = true)]
		private static extern int QueryDosDevice(string lpDeviceName, IntPtr lpTargetPath, uint ucchMax);

		#endregion

		#region Enums

		[Flags]
		private enum EFileAccess : uint
		{
			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
		}

		[Flags]
		private enum EFileShare : uint
		{
			Read = 0x00000001,
			Write = 0x00000002,
		}

		private enum ECreationDisposition : uint
		{
			OpenExisting = 3,
		}

		[Flags]
		private enum EFileAttributes : uint
		{
			Device = 0x00000040,
			Write_Through = 0x80000000,
			NoBuffering = 0x20000000,
		}

		[Flags]
		private enum EParam : uint
		{
			DDD_RAW_TARGET_PATH = 0x00000001,
			DDD_REMOVE_DEFINITION = 0x00000002,
			DDD_EXACT_MATCH_ON_REMOVE = 0x00000004,
		}


		private enum DeviceIoControlCode
		{
			IoctlVolumeGetVolumeDiskExtents = 0x560000,
			FSCTL_DISMOUNT_VOLUME = 0x00090020,
			FSCTL_LOCK_VOLUME = 0x90018,
			IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x70000,
			FSCTL_UNLOCK_VOLUME = 0x0009001C,
		}

		#endregion

		#region Structs

		[StructLayout(LayoutKind.Sequential)]
		private struct DiskExtent
		{
			public readonly int DiskNumber;
			private readonly long StartingOffset;
			private readonly long ExtentLength;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct DISK_GEOMETRY
		{
			private readonly long Cylinders;
			private readonly int MediaType;
			private readonly int TracksPerCylinder;
			private readonly int SectorsPerTrack;
			private readonly int BytesPerSector;

			public long DiskSize
			{
				get
				{
					return Cylinders * TracksPerCylinder * SectorsPerTrack * BytesPerSector;
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct DiskExtents
		{
			private readonly int numberOfExtents;
			public DiskExtent first;
		}

		#endregion
	}
}