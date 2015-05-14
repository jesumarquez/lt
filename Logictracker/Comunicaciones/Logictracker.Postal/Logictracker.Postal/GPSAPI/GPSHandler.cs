using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Urbetrack.Postal.GPSAPI
{
    public class GPSHandler
    {
        	#region "API declaration"

	[DllImport("gpsapi.dll")]
	private static extern IntPtr GPSOpenDevice(IntPtr hNewLocationData, IntPtr hDeviceStateChange, string szDeviceName, int dwFlags);

	[DllImport("gpsapi.dll")]
	private static extern Int32 GPSCloseDevice(IntPtr hGPSDevice);

	[DllImport("gpsapi.dll")]
	private static extern Int32 GPSGetPosition(IntPtr hGPSDevice, IntPtr pGPSPosition, Int32 dwMaximumAge, Int32 dwFlags);

	[DllImport("gpsapi.dll")]
	private static extern Int32 GPSGetDeviceState(ref IntPtr pGPSDevice);

	[DllImport("coredll.dll", SetLastError = true)]
	private static extern IntPtr LocalAlloc(int uFlags, int uBytes);

	[DllImport("coredll.dll", SetLastError = true)]
	private static extern IntPtr LocalFree(IntPtr hMem);

	private const int LMEM_FIXED = 0;
	private const int LMEM_MOVEABLE = 2;
	private const int LMEM_ZEROINIT = 0x40;
		#endregion
	private const int LPTR  = (LMEM_FIXED | LMEM_ZEROINIT);

	private IntPtr hGPSDevice = IntPtr.Zero;
	private IntPtr pGPSPosition = IntPtr.Zero;
	private int mMaximunAge = 0;

	private int mUsageCount = 0;
	public event PositionReceivedEventHandler PositionReceived;
	public delegate void PositionReceivedEventHandler(GPSPosition position);

	public GPSHandler(int MaximunAgeSec)
	{
		mMaximunAge = MaximunAgeSec * 1000;

	}

	public bool Open()
	{
		lock (this) {
			if (mUsageCount == 0) {
				hGPSDevice = GPSOpenDevice(IntPtr.Zero, IntPtr.Zero, null, 0);
				if (hGPSDevice.ToInt32() != IntPtr.Zero.ToInt32()) {
					mUsageCount = 1;
				}
			} else {
				mUsageCount += 1;
			}
		}
		return IsOpen();
	}

	public bool Close()
	{
		Int32 rc = default(Int32);
		bool ret = false;

		lock (this) {
			if (mUsageCount >= 1) {
				if (mUsageCount == 1) {
					rc = GPSCloseDevice(hGPSDevice);
					hGPSDevice = IntPtr.Zero;
					mUsageCount = 0;
				} else {
					mUsageCount -= 1;
				}
			} else {
				ret = false;
			}
		}
		return ret;
	}

	public GPSPosition GetPosition()
	{
		GPSPosition ret = null;
		bool posAcquired = false;

		lock (this) {

			if (IsOpen()) {
				int cb = Marshal.SizeOf(typeof(GPSPosition));
				IntPtr ptr = LocalAlloc(LPTR, cb);

				ret = new GPSPosition();
				ret.dwVersion = 1;
				ret.dwSize = Marshal.SizeOf(typeof(GPSPosition));

				// Marshal our data to the native pointer we allocated.
				Marshal.StructureToPtr(ret, ptr, false);

				Int32 i = GPSGetPosition(hGPSDevice, ptr, mMaximunAge, 0);

				if (i == 0) {
					Marshal.PtrToStructure(ptr, ret);
					posAcquired = true;

				// COMENTADO.
				// Ahora la fecha la sacamos del SQL Server.
				//' '' Seteo la hora actual en el sistema con la que tienen los satélites
				// ''If ret.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_UTC_TIME) Then
				// ''    If ret.stUTCTime.wYear >= 2007 Then
				// ''        'Si la diferencia de tiempo es de más de 2 minutos, actualiza la hora
				// ''        If Math.Abs(ret.Time.Subtract(DateTime.UtcNow).Minutes) > 2 Then
				// ''            Dim rc As UInteger
				// ''            rc = SetSystemTime(ret.stUTCTime)
				// ''        End If
				// ''    End If
				// ''End If

				} else {
                    MessageBox.Show("Error# " + i.ToString());
				}
				LocalFree(ptr);
			}
		}

		if (posAcquired == false) {
			if (PositionReceived != null) {
				PositionReceived(ret);
			}
		}

		return ret;
	}

	public bool IsOpen()
	{
		return hGPSDevice.ToInt32() != IntPtr.Zero.ToInt32();
	}

	public IntPtr GetDeviceState()
	{
		IntPtr pGPSDevice = IntPtr.Zero;
		if (IsOpen()) {
			GPSGetDeviceState(ref pGPSDevice);
		}

		return pGPSDevice;
	}

    public bool IsPositionValid(GPSPosition position )
	{
		bool posOk = false;
        bool ValidateFix = false;
        float MAXDOP = -1;

		if ((position != null)) {
			if (position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_UTC_TIME) && position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION) && position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LATITUDE) && position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LONGITUDE) && (position.fixType >= GPSFixType.XyD || ValidateFix == false)) {
				if ((MAXDOP >= position.flHorizontalDilutionOfPrecision) | MAXDOP == -1) {  
					posOk = true;
				}
			}
		}
		return posOk;
	}

	public static bool IsPositionValid(GPSPosition position, bool ValidateFix, float MAXDOP )
	{
		bool posOk = false;

		if ((position != null)) {
			if (position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_UTC_TIME) & position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION) & position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LATITUDE) & position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LONGITUDE) & (position.fixType >= GPSFixType.XyD | ValidateFix == false)) {
				if ((MAXDOP >= position.flHorizontalDilutionOfPrecision) | MAXDOP == -1) {
					posOk = true;
				}
			}
		}
		return posOk;
	}
    }

}
