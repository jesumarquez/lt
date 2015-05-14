using Microsoft.Win32;
using System;
using System.Collections;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
namespace Geocoder.Core.Security
{
	internal class HardwareInfo
	{
		[DllImport("kernel32.dll")]
		private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, uint VolumeNameSize, ref uint VolumeSerialNumber, ref uint MaximumComponentLength, ref uint FileSystemFlags, StringBuilder FileSystemNameBuffer, uint FileSystemNameSize);
		public void GetFirstHardDiskDriveProperty(out string HDType, out string HDModel, out string HDSerial)
		{
			ArrayList arrayList = new ArrayList();
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)enumerator.Current;
					arrayList.Add(new HardDrive
					{
						Model = managementObject["Model"].ToString(),
						Type = managementObject["InterfaceType"].ToString()
					});
				}
			}
			managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
			int num = 0;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)enumerator.Current;
					HardDrive hardDrive = (HardDrive)arrayList[num];
					if (managementObject["SerialNumber"] == null)
					{
						hardDrive.SerialNo = "None";
					}
					else
					{
						hardDrive.SerialNo = managementObject["SerialNumber"].ToString();
					}
					num++;
				}
			}
			HDType = ((HardDrive)arrayList[0]).Type;
			HDModel = ((HardDrive)arrayList[0]).Model;
			HDSerial = ((HardDrive)arrayList[0]).SerialNo;
		}
		public void GetIPAndCompName(out string IP, out string ComputerName)
		{
			ComputerName = Dns.GetHostName();
			IPHostEntry hostEntry = Dns.GetHostEntry(ComputerName);
			IP = hostEntry.AddressList[0].ToString();
		}
		public string GetDriveVolumeProperties(string strDriveLetter)
		{
			uint value = 0u;
			uint num = 0u;
			StringBuilder stringBuilder = new StringBuilder(256);
			uint num2 = 0u;
			StringBuilder stringBuilder2 = new StringBuilder(256);
			strDriveLetter += ":\\";
			long volumeInformation = HardwareInfo.GetVolumeInformation(strDriveLetter, stringBuilder, (uint)stringBuilder.Capacity, ref value, ref num, ref num2, stringBuilder2, (uint)stringBuilder2.Capacity);
			return Convert.ToString(value);
		}
		public string GetHardwareIdBytes()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\WPA", false))
			{
				string[] subKeyNames = registryKey.GetSubKeyNames();
				for (int i = 0; i < subKeyNames.Length; i++)
				{
					string text = subKeyNames[i];
					if (text.IndexOf("SigningHash") == 0)
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(text))
						{
							return BitConverter.ToString((byte[])registryKey2.GetValue("SigningHashData"));
						}
					}
				}
			}
			throw new Exception("There is no hardware id on this computer!");
		}
		public string GetUniqueGUID()
		{
			return Guid.NewGuid().ToString();
		}
		public string GetVolumeSerial(string strDriveLetter)
		{
			if (string.IsNullOrEmpty(strDriveLetter))
			{
				strDriveLetter = "C";
			}
			ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
			managementObject.Get();
			return managementObject["VolumeSerialNumber"].ToString();
		}
		public string GetMACAddress()
		{
			ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection instances = managementClass.GetInstances();
			string text = string.Empty;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)enumerator.Current;
					if (text == string.Empty)
					{
						if ((bool)managementObject["IPEnabled"])
						{
							text = managementObject["MacAddress"].ToString();
						}
					}
					managementObject.Dispose();
				}
			}
			text = text.Replace(":", "");
			return text;
		}
		public string GetCPUId()
		{
			string text = string.Empty;
			string empty = string.Empty;
			ManagementClass managementClass = new ManagementClass("Win32_Processor");
			ManagementObjectCollection instances = managementClass.GetInstances();
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)enumerator.Current;
					if (text == string.Empty)
					{
						text = managementObject.Properties["ProcessorId"].Value.ToString();
					}
				}
			}
			return text;
		}
	}
}
