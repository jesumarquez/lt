Imports System.Runtime.InteropServices
Imports System


#Region " Enums "
Public Enum GPSFixQuality
    Unknown = 0
    Gps
    DGps
End Enum

Public Enum GPSFixType
    Unknown = 0
    XyD
    XyzD
End Enum

Public Enum GPSFixSelection
    Unknown = 0
    Automatic
    Manual
End Enum
#End Region

Public Structure SYSTEMTIME
    Public wYear As Int16
    Public wMonth As Int16
    Public wDayOfWeek As Int16
    Public wDay As Int16
    Public wHour As Int16
    Public wMinute As Int16
    Public wSecond As Int16
    Public wMilliseconds As Int16
End Structure 'SystemTime

Public Structure TIME_ZONE_INFORMATION
    Public Bias As Long
    Public StandardName() As Integer
    Public StandardDate As SYSTEMTIME
    Public StandardBias As Long
    Public DaylightName() As Integer
    Public DaylightDate As SYSTEMTIME
    Public DaylightBias As Long
End Structure



Public Structure SatelliteArray
    Public a, b, c, d, e, f, g, h, i, j, k, l As Int32
End Structure

Public Class GPSPosition
    Public Enum GPS_VALID_FIELDS
        GPS_VALID_UTC_TIME = &H1 'If set, the stUTCTime field is valid. 
        GPS_VALID_LATITUDE = &H2 'If set, the dblLatitude field is valid. 
        GPS_VALID_LONGITUDE = &H4 'If set, the dblLongitude field is valid. 
        GPS_VALID_SPEED = &H8 'If set, the flSpeed field is valid. 
        GPS_VALID_HEADING = &H10 'If set, the flHeading field is valid. 
        GPS_VALID_MAGNETIC_VARIATION = &H20 'If set, the dblMagneticVariation field is valid. 
        GPS_VALID_ALTITUDE_WRT_SEA_LEVEL = &H40 'If set, the flAltitudeWRTSeaLevel field is valid. 
        GPS_VALID_ALTITUDE_WRT_ELLIPSOID = &H80 'If set, the flAltitudeWRTEllipsoid field is valid. 
        GPS_VALID_POSITION_DILUTION_OF_PRECISION = &H100 'If set, the flPositionDilutionOfPrecision field is valid. 
        GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION = &H200 'If set, the flHorizontalDilutionOfPrecision field is valid. 
        GPS_VALID_VERTICAL_DILUTION_OF_PRECISION = &H400 'If set, the flVerticalDilutionOfPrecision field is valid. 
        GPS_VALID_SATELLITE_COUNT = &H800 'If set, the dwSatelliteCount field is valid. 
        GPS_VALID_SATELLITES_USED_PRNS = &H1000 'If set, the rgdwSatellitesUsedPRNs field is valid. 
        GPS_VALID_SATELLITES_IN_VIEW = &H2000 'If set, the dwSatellitesInView field is valid. 
        GPS_VALID_SATELLITES_IN_VIEW_PRNS = &H4000 'If set, the rgdwSatellitesInViewPRNs field is valid. 
        GPS_VALID_SATELLITES_IN_VIEW_ELEVATION = &H8000 'If set, the rgdwSatellitesInViewElevation field is valid. 
        GPS_VALID_SATELLITES_IN_VIEW_AZIMUTH = &H10000 'If set, the rgdwSatellitesInViewAzimuth field is valid. 
        GPS_VALID_SATELLITES_IN_VIEW_SIGNAL_TO_NOISE_RATIO = &H20000
    End Enum

    Public Shared Function systemTime2Date(ByVal f As SYSTEMTIME) As Date
        Return New DateTime(f.wYear, f.wMonth, _
            f.wDay, f.wHour, f.wMinute, f.wSecond, _
            f.wMilliseconds, DateTimeKind.Utc).ToLocalTime
    End Function


    Public dwVersion As Int32 = 1             ' Current version of GPSID client is using.
    Public dwSize As Int32 = 0                ' sizeof(_GPS_POSITION)

    ' Not all fields in the structure below are guaranteed to be valid.  
    ' Which fields are valid depend on GPS device being used, how stale the API allows
    ' the data to be, and current signal.
    ' Valid fields are specified in dwValidFields, based on GPS_VALID_XXX flags.
    Public dwValidFields As Int32 = 0

    ' Additional information about this location structure (GPS_DATA_FLAGS_XXX)
    Public dwFlags As Int32 = 0

    '** Time related
    Public stUTCTime As SYSTEMTIME = New SYSTEMTIME()  '  UTC according to GPS clock.

    '** Position + heading related
    Public dblLatitude As Double = 0.0            ' Degrees latitude.  North is positive
    Public dblLongitude As Double = 0.0           ' Degrees longitude.  East is positive
    Public flSpeed As Single = 0.0F                ' Speed in knots
    Public flHeading As Single = 0.0F              ' Degrees heading (course made good).  True North=0
    Public dblMagneticVariation As Double = 0.0   ' Magnetic variation.  East is positive
    Public flAltitudeWRTSeaLevel As Single = 0.0F  ' Altitute with regards to sea level, in meters
    Public flAltitudeWRTEllipsoid As Single = 0.0F ' Altitude with regards to ellipsoid, in meters

    '** Quality of this fix
    ' Where did we get fix from?
    Public fixQuality As GPSFixQuality = fixQuality.Unknown
    ' Is this 2d or 3d fix?
    Public fixType As GPSFixType = fixType.Unknown
    ' Auto or manual selection between 2d or 3d mode
    Public selectionType As GPSFixSelection = GPSFixSelection.Unknown
    ' Position Dilution Of Precision
    Public flPositionDilutionOfPrecision As Single = 0.0F
    ' Horizontal Dilution Of Precision
    Public flHorizontalDilutionOfPrecision As Single = 0.0F
    ' Vertical Dilution Of Precision
    Public flVerticalDilutionOfPrecision As Single = 0.0F

    '** Satellite information
    ' Number of satellites used in solution
    Public dwSatelliteCount As Int32 = 0
    ' PRN numbers of satellites used in the solution
    Public rgdwSatellitesUsedPRNs As SatelliteArray = New SatelliteArray()
    ' Number of satellites in view.  From 0-GPS_MAX_SATELLITES
    Public dwSatellitesInView As Int32 = 0
    ' PRN numbers of satellites in view
    Public rgdwSatellitesInViewPRNs As SatelliteArray = New SatelliteArray()
    ' Elevation of each satellite in view
    Public rgdwSatellitesInViewElevation As SatelliteArray = New SatelliteArray()
    ' Azimuth of each satellite in view
    Public rgdwSatellitesInViewAzimuth As SatelliteArray = New SatelliteArray()
    ' Signal to noise ratio of each satellite in view
    Public rgdwSatellitesInViewSignalToNoiseRatio As SatelliteArray = New SatelliteArray()

    Function isValidField(ByVal fieldEnum As GPS_VALID_FIELDS)
        Return (dwValidFields And fieldEnum) > 0
    End Function

    Public ReadOnly Property Time() As DateTime
        Get
            Dim t As DateTime = New DateTime(stUTCTime.wYear, stUTCTime.wMonth, _
            stUTCTime.wDay, stUTCTime.wHour, stUTCTime.wMinute, stUTCTime.wSecond, _
            stUTCTime.wMilliseconds)

            Return t
        End Get
    End Property
    '    {
    '        get
    '        {
    '            DateTime time = new DateTime(stUTCTime.year, stUTCTime.month, stUTCTime.day, stUTCTime.hour, stUTCTime.minute, stUTCTime.second, stUTCTime.millisecond)
    '            return time
    '        }


    ''/ <summary>
    ''/ UTC according to GPS clock.
    ''/ </summary>
    '    public DateTime Time
    '    {
    '        get
    '        {
    '            DateTime time = new DateTime(stUTCTime.year, stUTCTime.month, stUTCTime.day, stUTCTime.hour, stUTCTime.minute, stUTCTime.second, stUTCTime.millisecond)
    '            return time
    '        }

    '    }
    ''/ <summary>
    ''/ True if the Time property is valid, false if invalid
    ''/ </summary>
    '    public bool TimeValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_UTC_TIME) != 0 }
    '    }


    ''/ <summary>
    ''/ Satellites used in the solution
    ''/ </summary>
    ''/ <returns>Array of Satellites</returns>
    '    public Satellite[] GetSatellitesInSolution()
    '    {
    '        Satellite[] inViewSatellites = GetSatellitesInView()
    '        ArrayList list = new ArrayList()
    '        for (int index = 0 index < dwSatelliteCount index++)
    '        {
    '            Satellite found = null
    '            for (int viewIndex = 0 viewIndex < inViewSatellites.Length && found == null viewIndex++)
    '            {
    '                if (rgdwSatellitesUsedPRNs[index] == inViewSatellites[viewIndex].Id)
    '                {
    '                    found = inViewSatellites[viewIndex]
    '                    list.Add(found)
    '                }
    '            }
    '        }

    '        return (Satellite[])list.ToArray(typeof(Satellite))
    '    }
    ''/ <summary>
    ''/ True if the SatellitesInSolution property is valid, false if invalid
    ''/ </summary>
    '    public bool SatellitesInSolutionValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_SATELLITES_USED_PRNS) != 0 }
    '    }



    ''/ <summary>
    ''/ Satellites in view
    ''/ </summary>
    ''/ <returns>Array of Satellites</returns>
    '    public Satellite[] GetSatellitesInView()
    '    {
    '        Satellite[] satellites = null
    '        if (dwSatellitesInView != 0)
    '        {
    '            satellites = new Satellite[dwSatellitesInView]
    '            for (int index = 0 index < satellites.Length index++)
    '            {
    '                satellites[index] = new Satellite()
    '                satellites[index].Azimuth = rgdwSatellitesInViewAzimuth[index]
    '                satellites[index].Elevation = rgdwSatellitesInViewElevation[index]
    '                satellites[index].Id = rgdwSatellitesInViewPRNs[index]
    '                satellites[index].SignalStrength = rgdwSatellitesInViewSignalToNoiseRatio[index]
    '            }
    '        }

    '        return satellites
    '    }
    ''/ <summary>
    ''/ True if the SatellitesInView property is valid, false if invalid
    ''/ </summary>
    '    public bool SatellitesInViewValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_SATELLITES_IN_VIEW) != 0 }
    '    }


    ''/ <summary>
    ''/ Number of satellites used in solution
    ''/ </summary>
    '    public int SatelliteCount
    '    {
    '        get { return dwSatelliteCount }
    '    }
    ''/ <summary>
    ''/ True if the SatelliteCount property is valid, false if invalid
    ''/ </summary>
    '    public bool SatelliteCountValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_SATELLITE_COUNT) != 0 }
    '    }

    ''/ <summary>
    ''/ Number of satellites in view.  
    ''/ </summary>
    '    public int SatellitesInViewCount
    '    {
    '        get { return dwSatellitesInView }
    '    }
    ''/ <summary>
    ''/ True if the SatellitesInViewCount property is valid, false if invalid
    ''/ </summary>
    '    public bool SatellitesInViewCountValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_SATELLITES_IN_VIEW) != 0 }
    '    }

    ''/ <summary>
    ''/ Speed in knots
    ''/ </summary>
    '    public float Speed
    '    {
    '        get { return flSpeed }
    '    }
    ''/ <summary>
    ''/ True if the Speed property is valid, false if invalid
    ''/ </summary>
    '    public bool SpeedValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_SPEED) != 0 }
    '    }

    ''/ <summary>
    ''/ Altitude with regards to ellipsoid, in meters
    ''/ </summary>
    '    public float EllipsoidAltitude
    '    {
    '        get { return flAltitudeWRTEllipsoid }
    '    }
    ''/ <summary>
    ''/ True if the EllipsoidAltitude property is valid, false if invalid
    ''/ </summary>
    '    public bool EllipsoidAltitudeValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_ALTITUDE_WRT_ELLIPSOID) != 0 }
    '    }

    ''/ <summary>
    ''/ Altitute with regards to sea level, in meters
    ''/ </summary>
    '    public float SeaLevelAltitude
    '    {
    '        get { return flAltitudeWRTSeaLevel }
    '    }
    ''/ <summary>
    ''/ True if the SeaLevelAltitude property is valid, false if invalid
    ''/ </summary>
    '    public bool SeaLevelAltitudeValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_ALTITUDE_WRT_SEA_LEVEL) != 0 }
    '    }

    ''/ <summary>
    ''/ Latitude in decimal degrees.  North is positive
    ''/ </summary>
    '    public double Latitude
    '    {
    '        get { return ParseDegreesMinutesSeconds(dblLatitude).ToDecimalDegrees() }
    '    }
    ''/ <summary>
    ''/ Latitude in degrees, minutes, seconds.  North is positive
    ''/ </summary>
    '    public DegreesMinutesSeconds LatitudeInDegreesMinutesSeconds
    '    {
    '        get { return ParseDegreesMinutesSeconds(dblLatitude) }
    '    }

    ''/ <summary>
    ''/ True if the Latitude property is valid, false if invalid
    ''/ </summary>
    '    public bool LatitudeValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_LATITUDE) != 0 }
    '    }

    ''/ <summary>
    ''/ Longitude in decimal degrees.  East is positive
    ''/ </summary>
    '    public double Longitude
    '    {
    '        get { return ParseDegreesMinutesSeconds(dblLongitude).ToDecimalDegrees() }
    '    }

    ''/ <summary>
    ''/ Longitude in degrees, minutes, seconds.  East is positive
    ''/ </summary>
    '    public DegreesMinutesSeconds LongitudeInDegreesMinutesSeconds
    '    {
    '        get { return ParseDegreesMinutesSeconds(dblLongitude) }
    '    }
    ''/ <summary>
    ''/ True if the Longitude property is valid, false if invalid
    ''/ </summary>
    '    public bool LongitudeValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_LONGITUDE) != 0 }
    '    }

    ''/ <summary>
    ''/ Degrees heading (course made good).  True North=0
    ''/ </summary>
    '    public float Heading
    '    {
    '        get { return flHeading }
    '    }
    ''/ <summary>
    ''/ True if the Heading property is valid, false if invalid
    ''/ </summary>
    '    public bool HeadingValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_HEADING) != 0 }
    '    }

    ''/ <summary>
    ''/ Position Dilution Of Precision
    ''/ </summary>
    '    public float PositionDilutionOfPrecision
    '    {
    '        get { return flPositionDilutionOfPrecision }
    '    }
    ''/ <summary>
    ''/ True if the PositionDilutionOfPrecision property is valid, false if invalid
    ''/ </summary>
    '    public bool PositionDilutionOfPrecisionValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_POSITION_DILUTION_OF_PRECISION) != 0 }
    '    }

    ''/ <summary>
    ''/ Horizontal Dilution Of Precision
    ''/ </summary>
    '    public float HorizontalDilutionOfPrecision
    '    {
    '        get { return flHorizontalDilutionOfPrecision }
    '    }
    ''/ <summary>
    ''/ True if the HorizontalDilutionOfPrecision property is valid, false if invalid
    ''/ </summary>
    '    public bool HorizontalDilutionOfPrecisionValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION) != 0 }
    '    }

    ''/ <summary>
    ''/ Vertical Dilution Of Precision
    ''/ </summary>
    '    public float VerticalDilutionOfPrecision
    '    {
    '        get { return flVerticalDilutionOfPrecision }
    '    }
    ''/ <summary>
    ''/ True if the VerticalDilutionOfPrecision property is valid, false if invalid
    ''/ </summary>
    '    public bool VerticalDilutionOfPrecisionValid
    '    {
    '        get { return (dwValidFields & GPS_VALID_VERTICAL_DILUTION_OF_PRECISION) != 0 }
    '    }

    ''/ <summary>
    ''/ Parses out the degrees, minutes, seconds from the double format returned by
    ''/ the NMEA GPS device
    ''/ </summary>
    ''/ <param name="val">degrees, minutes, seconds as a double</param>
    ''/ <returns>DegreesMinutesSeconds structure</returns>
    '    private DegreesMinutesSeconds ParseDegreesMinutesSeconds(double val)
    '    {
    '        double degrees = (val / 100.0)
    '        double minutes = (Math.Abs(degrees) - Math.Abs((double)(int)(degrees))) * 100
    '        double seconds = (Math.Abs(val) - Math.Abs((double)(int)val)) * 60.0

    '        return new DegreesMinutesSeconds((int)degrees, (int)minutes, seconds)
    '    }
    '}

End Class
