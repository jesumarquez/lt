Imports System.Runtime.InteropServices

Public Class GPS

#Region "API declaration"

    <DllImport("gpsapi.dll")> _
    Private Shared Function GPSOpenDevice(ByVal hNewLocationData As IntPtr, _
        ByVal hDeviceStateChange As IntPtr, ByVal szDeviceName As String, _
        ByVal dwFlags As Integer) As IntPtr
    End Function

    <DllImport("gpsapi.dll")> _
    Private Shared Function GPSCloseDevice(ByVal hGPSDevice As IntPtr) As Int32
    End Function

    <DllImport("gpsapi.dll")> _
    Private Shared Function GPSGetPosition(ByVal hGPSDevice As IntPtr, _
        ByVal pGPSPosition As IntPtr, ByVal dwMaximumAge As Int32, _
        ByVal dwFlags As Int32) As Int32
    End Function

    <DllImport("gpsapi.dll")> _
    Private Shared Function GPSGetDeviceState(ByRef pGPSDevice As IntPtr) As Int32
    End Function

    <DllImport("coredll.dll", SetLastError:=True)> _
    Private Shared Function LocalAlloc(ByVal uFlags As Integer, ByVal uBytes As Integer) As IntPtr
    End Function

    <DllImport("coredll.dll", SetLastError:=True)> _
    Private Shared Function LocalFree(ByVal hMem As IntPtr) As IntPtr
    End Function

    Private Const LMEM_FIXED As Integer = 0
    Private Const LMEM_MOVEABLE As Integer = 2
    Private Const LMEM_ZEROINIT As Integer = &H40
    Private Const LPTR = (LMEM_FIXED Or LMEM_ZEROINIT)
#End Region

    Private hGPSDevice As IntPtr = IntPtr.Zero
    Private pGPSPosition As IntPtr = IntPtr.Zero
    Private mMaximunAge As Integer = 0

    Public Sub New(ByVal MaximunAgeSec As Integer)
        mMaximunAge = MaximunAgeSec * 1000
    End Sub

    Public Function Open() As Boolean
        SyncLock Me
            hGPSDevice = GPSOpenDevice(IntPtr.Zero, IntPtr.Zero, Nothing, 0)
        End SyncLock
        Return IsOpen()
    End Function

    Public Function Close() As Boolean
        Dim rc As Int32
        Dim ret As Boolean

        SyncLock Me
            rc = GPSCloseDevice(hGPSDevice)
            hGPSDevice = IntPtr.Zero.ToInt32
        End SyncLock
        Return ret
    End Function

    Public Function GetPosition() As GPSPosition
        Dim ret As GPSPosition = Nothing

        SyncLock Me
            If IsOpen() Then

                Dim cb As Integer = Marshal.SizeOf(GetType(GPSPosition))
                Dim ptr As IntPtr = LocalAlloc(LPTR, cb)

                ret = New GPSPosition
                ret.dwVersion = 1
                ret.dwSize = Marshal.SizeOf(GetType(GPSPosition))

                ' Marshal our data to the native pointer we allocated.
                Marshal.StructureToPtr(ret, ptr, False)

                Dim i As Int32 = GPSGetPosition(hGPSDevice, ptr, mMaximunAge, 0)

                If i = 0 Then
                    Marshal.PtrToStructure(ptr, ret)
                Else
                    System.Diagnostics.Debug.WriteLine(String.Format("ERROR: GPSGetPosition fallo, {0}", i))
                End If
                LocalFree(ptr)
            End If
        End SyncLock

        Return ret
    End Function

    Public Function IsOpen() As Boolean
        Return hGPSDevice.ToInt32 <> IntPtr.Zero.ToInt32
    End Function

    Public Shared Function IsPositionValid(ByVal position As GPSPosition, Optional ByVal ValidateFix As Boolean = False, Optional ByVal min_pdop As Single = -1) As Boolean
        Dim posOk As Boolean = False

        If Not position Is Nothing Then
            If position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_UTC_TIME) And _
               position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION) And _
               position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LATITUDE) And _
               position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LONGITUDE) And _
               (position.fixType >= GPSFixType.XyD Or ValidateFix = False) Then
                If (min_pdop >= position.flPositionDilutionOfPrecision) Or min_pdop = -1 Then
                    posOk = True
                End If
            End If
        End If
        Return posOk
    End Function

End Class
