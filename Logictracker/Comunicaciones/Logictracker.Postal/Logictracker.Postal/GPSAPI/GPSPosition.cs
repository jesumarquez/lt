using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Urbetrack.Postal.GPSAPI
{

    #region " Enums "
    public enum GPSFixQuality
    {
        Unknown = 0,
        Gps,
        DGps
    }

    public enum GPSFixType
    {
        Unknown = 0,
        XyD,
        XyzD
    }

    public enum GPSFixSelection
    {
        Unknown = 0,
        Automatic,
        Manual
    }
    #endregion

    public struct SYSTEMTIME
    {
        public Int16 wYear;
        public Int16 wMonth;
        public Int16 wDayOfWeek;
        public Int16 wDay;
        public Int16 wHour;
        public Int16 wMinute;
        public Int16 wSecond;
        public Int16 wMilliseconds;
    }
    //SystemTime

    public struct TIME_ZONE_INFORMATION
    {
        public long Bias;
        public int[] StandardName;
        public SYSTEMTIME StandardDate;
        public long StandardBias;
        public int[] DaylightName;
        public SYSTEMTIME DaylightDate;
        public long DaylightBias;
    }



    public struct SatelliteArray
    {
        public Int32 a;
        public Int32 b;
        public Int32 c;
        public Int32 d;
        public Int32 e;
        public Int32 f;
        public Int32 g;
        public Int32 h;
        public Int32 i;
        public Int32 j;
        public Int32 k;
        public Int32 l;
    }

    public class GPSPosition
    {
        public enum GPS_VALID_FIELDS
        {
            GPS_VALID_UTC_TIME = 0x1,
            //If set, the stUTCTime field is valid. 
            GPS_VALID_LATITUDE = 0x2,
            //If set, the dblLatitude field is valid. 
            GPS_VALID_LONGITUDE = 0x4,
            //If set, the dblLongitude field is valid. 
            GPS_VALID_SPEED = 0x8,
            //If set, the flSpeed field is valid. 
            GPS_VALID_HEADING = 0x10,
            //If set, the flHeading field is valid. 
            GPS_VALID_MAGNETIC_VARIATION = 0x20,
            //If set, the dblMagneticVariation field is valid. 
            GPS_VALID_ALTITUDE_WRT_SEA_LEVEL = 0x40,
            //If set, the flAltitudeWRTSeaLevel field is valid. 
            GPS_VALID_ALTITUDE_WRT_ELLIPSOID = 0x80,
            //If set, the flAltitudeWRTEllipsoid field is valid. 
            GPS_VALID_POSITION_DILUTION_OF_PRECISION = 0x100,
            //If set, the flPositionDilutionOfPrecision field is valid. 
            GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION = 0x200,
            //If set, the flHorizontalDilutionOfPrecision field is valid. 
            GPS_VALID_VERTICAL_DILUTION_OF_PRECISION = 0x400,
            //If set, the flVerticalDilutionOfPrecision field is valid. 
            GPS_VALID_SATELLITE_COUNT = 0x800,
            //If set, the dwSatelliteCount field is valid. 
            GPS_VALID_SATELLITES_USED_PRNS = 0x1000,
            //If set, the rgdwSatellitesUsedPRNs field is valid. 
            GPS_VALID_SATELLITES_IN_VIEW = 0x2000,
            //If set, the dwSatellitesInView field is valid. 
            GPS_VALID_SATELLITES_IN_VIEW_PRNS = 0x4000,
            //If set, the rgdwSatellitesInViewPRNs field is valid. 
            GPS_VALID_SATELLITES_IN_VIEW_ELEVATION = 0x8000,
            //If set, the rgdwSatellitesInViewElevation field is valid. 
            GPS_VALID_SATELLITES_IN_VIEW_AZIMUTH = 0x10000,
            //If set, the rgdwSatellitesInViewAzimuth field is valid. 
            GPS_VALID_SATELLITES_IN_VIEW_SIGNAL_TO_NOISE_RATIO = 0x20000
        }

        public static System.DateTime systemTime2Date(SYSTEMTIME f)
        {
            return new DateTime(f.wYear, f.wMonth, f.wDay, f.wHour, f.wMinute, f.wSecond, f.wMilliseconds, DateTimeKind.Utc).ToLocalTime();
        }


        // Current version of GPSID client is using.
        public Int32 dwVersion = 1;
        // sizeof(_GPS_POSITION)
        public Int32 dwSize = 0;

        // Not all fields in the structure below are guaranteed to be valid.  
        // Which fields are valid depend on GPS device being used, how stale the API allows
        // the data to be, and current signal.
        // Valid fields are specified in dwValidFields, based on GPS_VALID_XXX flags.

        public Int32 dwValidFields = 0;
        // Additional information about this location structure (GPS_DATA_FLAGS_XXX)

        public Int32 dwFlags = 0;
        //** Time related
        //  UTC according to GPS clock.
        public SYSTEMTIME stUTCTime = new SYSTEMTIME();

        //** Position + heading related
        // Degrees latitude.  North is positive
        public double dblLatitude = 0.0;
        // Degrees longitude.  East is positive
        public double dblLongitude = 0.0;
        // Speed in knots
        public float flSpeed = 0f;
        // Degrees heading (course made good).  True North=0
        public float flHeading = 0f;
        // Magnetic variation.  East is positive
        public double dblMagneticVariation = 0.0;
        // Altitute with regards to sea level, in meters
        public float flAltitudeWRTSeaLevel = 0f;
        // Altitude with regards to ellipsoid, in meters
        public float flAltitudeWRTEllipsoid = 0f;

        //** Quality of this fix
        // Where did we get fix from?
        public GPSFixQuality fixQuality = GPSFixQuality.Unknown;
        // Is this 2d or 3d fix?
        public GPSFixType fixType = GPSFixType.Unknown;
        // Auto or manual selection between 2d or 3d mode
        public GPSFixSelection selectionType = GPSFixSelection.Unknown;
        // Position Dilution Of Precision
        public float flPositionDilutionOfPrecision = 0f;
        // Horizontal Dilution Of Precision
        public float flHorizontalDilutionOfPrecision = 0f;
        // Vertical Dilution Of Precision

        public float flVerticalDilutionOfPrecision = 0f;
        //** Satellite information
        // Number of satellites used in solution
        public Int32 dwSatelliteCount = 0;
        // PRN numbers of satellites used in the solution
        public SatelliteArray rgdwSatellitesUsedPRNs = new SatelliteArray();
        // Number of satellites in view.  From 0-GPS_MAX_SATELLITES
        public Int32 dwSatellitesInView = 0;
        // PRN numbers of satellites in view
        public SatelliteArray rgdwSatellitesInViewPRNs = new SatelliteArray();
        // Elevation of each satellite in view
        public SatelliteArray rgdwSatellitesInViewElevation = new SatelliteArray();
        // Azimuth of each satellite in view
        public SatelliteArray rgdwSatellitesInViewAzimuth = new SatelliteArray();
        // Signal to noise ratio of each satellite in view

        public SatelliteArray rgdwSatellitesInViewSignalToNoiseRatio = new SatelliteArray();
        public bool isValidField(GPS_VALID_FIELDS fieldEnum)
        {
            return (dwValidFields & (int)fieldEnum) > 0;
        }

        public System.DateTime Time
        {
            get
            {
                DateTime t = new DateTime(stUTCTime.wYear, stUTCTime.wMonth, stUTCTime.wDay, stUTCTime.wHour, stUTCTime.wMinute, stUTCTime.wSecond, stUTCTime.wMilliseconds);

                return t;
            }
        }
    }
}
