Imports System.Runtime.InteropServices

Public Module iPaqUtil
    Private Const DEFAULT_BATIDLE_SECONDS = 60
    Private m_BatIdle As Integer

    Public Enum IPAQ_ON_OFF As Integer
        IPAQ_OFF = 0
        IPAQ_ON = 1
    End Enum

    <DllImport("ipaqutil")> _
    Public Function iPAQGetSerialNumber(ByVal lpszSerialNumber As Char()) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQGetGSMRadioStatus(ByRef lpdwValue As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQSetGSMRadio(ByRef lpdwValue As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQSetBlueToothRadio(ByRef lpdwValue As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQSetWLANRadio(ByRef lpdwValue As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQGetWLANRadioStatus(ByRef lpdwValue As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQSetOnBatteryDeviceSuspendTimeOut(ByRef lpdwtimeout As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQGetOnBatteryDeviceSuspendTimeOut(ByRef lpdwtimeout As Integer) As Boolean
    End Function

    <DllImport("ipaqutil")> _
    Public Function iPAQGetMainBatteryLevel(ByRef lpdwValue As Integer) As Boolean
    End Function


    Public Function GetBatteryLevel() As Integer
        Try

            ' Variables 
            'DWORD dwLevel; 
            'BOOL  bRet; 
            Dim dwLevel As Int32 = 0
            Dim bRet As Boolean

            Try

                bRet = iPAQGetMainBatteryLevel(dwLevel)
                Return dwLevel

                '{ 
                '   // Function call was successful...is the battery above 50%? 
                '   if (dwLevel > 50) 
                '   { 
                '      // Yes... 
                '   } 
                '} 


            Catch ex As Exception

                Throw New Exception("No se puede obtener el nivel de la batería. " & ex.Message)

            End Try
            '// Get the main battery level 
            'bRet = iPAQGetMainBatteryLevel(&dwLevel); 
            'if (bRet == iPAQ_SUCCESS)  
            '{ 
            '   // Function call was successful...is the battery above 50%? 
            '   if (dwLevel > 50) 
            '   { 
            '      // Yes... 
            '   } 
            '} 

        Catch ex As Exception

        End Try

    End Function




    Public Function GetDeviceSN() As String

        Dim ret As String
        Dim c(19) As Char

        Try

            iPAQGetSerialNumber(c)
            Dim l As Integer = -1
            Do
                l += 1
            Loop Until c(l) = Chr(0)
            Dim s As New String(c, 0, l)
            s.Trim()
            ret = s

        Catch ex As Exception

            ' EJU
            ' Asumo que falló porque estamos corriendo en emulador
            ret = "(EMULADOR)"

        End Try


        Return ret
    End Function

    Public Sub FinalizeIPAQUtil()
        'Try
        'cm.RequestDisconnect()
        'cm = Nothing
        'Catch ex As Exception
        'End Try
    End Sub

    Sub EnableSuspendMode(ByVal canSuspend As Boolean)
        Dim r As Boolean

        Try

            If Not canSuspend Then
                r = iPAQGetOnBatteryDeviceSuspendTimeOut(m_BatIdle)
                r = iPAQSetOnBatteryDeviceSuspendTimeOut(0)
            Else
                r = iPAQSetOnBatteryDeviceSuspendTimeOut(DEFAULT_BATIDLE_SECONDS)
            End If
        Catch ex As Exception

        End Try
    End Sub

End Module
