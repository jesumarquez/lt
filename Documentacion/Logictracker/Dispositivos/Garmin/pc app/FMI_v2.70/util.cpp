/*********************************************************************
*
*   MODULE NAME:
*       util.cpp - Utility Procedures
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <time.h>
#include <math.h>
//#include <windows.h>

#include "util.h"

//----------------------------------------------------------------------
//! \brief Number of days in each month in a non-leap year
//----------------------------------------------------------------------
static uint8        const sMonthDays[ 12 ] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

//----------------------------------------------------------------------
//! \brief Cumulative number of days in prior months in a non-leap year
//----------------------------------------------------------------------
static uint32       const sYearDays[ 12 ]  = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };

//----------------------------------------------------------------------
//! \brief Converts a Garmin date to a structure containing year,
//!     month, and day.
//! \param aSeconds Number of seconds since Dec 31, 1989
//! \param aDateTime Structure containing year, month, and day
//----------------------------------------------------------------------
void UTIL_convert_seconds_to_date_type
    (
    const time_type     * aSeconds,
    date_time_data_type * aDateTime
    )
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    boolean         isLeapYear;
    time_type       days;
    sint32          fourYearCount;

    days            = *aSeconds / SECONDS_PER_DAY;
    fourYearCount   = (sint32)( days / DAYS_IN_4_YEARS );
    days           -= fourYearCount * DAYS_IN_4_YEARS;
    aDateTime->date.year      = (uint16)( fourYearCount * 4 ) + BASE_YEAR;

    while( days > DAYS_IN_1_YEAR )
    {
        days -= DAYS_IN_1_YEAR;

        if( ( aDateTime->date.year % 4 ) == 0 )
        {
            days--;
        }

        ( aDateTime->date.year )++;
    }

    if( days < 1 )
    {
        aDateTime->date.year--;
        aDateTime->date.month = DECEMBER;
        aDateTime->date.day   = sMonthDays[ DECEMBER - 1 ];
    }
    else
    {
        isLeapYear = FALSE;

        /*--------------------------------------------------
        Adjust for the extra day that happens during leap
        years (February 29).  This adjustment needs to
        happen any time after the beginning of March.
        --------------------------------------------------*/
        if( ( days > sYearDays[ MARCH - 1 ] ) &&
            ( ( aDateTime->date.year % 4 )  == 0 ) )
        {
            days--;
            isLeapYear = TRUE;
        }

        for( aDateTime->date.month = DECEMBER;
            days <= sYearDays[ aDateTime->date.month -1 ];
            aDateTime->date.month-- );

        aDateTime->date.day = (uint8)( days - sYearDays[ aDateTime->date.month - 1 ] );

        if( ( isLeapYear     == TRUE                          ) &&
            ( aDateTime->date.month   == FEBRUARY                      ) &&
            ( aDateTime->date.day     == sMonthDays[ FEBRUARY - 1 ] ) )
        {
            aDateTime->date.day++;
        }
    }
}

//----------------------------------------------------------------------
//! \brief Converts a date from from a structure to a Garmin date.
//! \param aDateTime Structure containing year, month, and day
//! \param aSeconds Number of seconds since Dec 31, 1989
//! \return TRUE if the conversion was successful, FALSE if
//!     the date_type does not represent a valid date between
//!     Dec 31, 1989 and Feb 5, 2126.
//----------------------------------------------------------------------
boolean UTIL_convert_date_time_to_seconds
    (
    const date_time_data_type * aDateTime,
    time_type * aSeconds
    )
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    time_type       days;
    sint32          fourYearCount;
    int             i;

    returnif_v( ( aDateTime->date.year  <  1989 ),        FALSE );
    returnif_v( ( aDateTime->date.year  == 1989 ) &&
        ( aDateTime->date.month <  12 ),          FALSE );
    returnif_v( ( aDateTime->date.year  == 1989 ) &&
        ( aDateTime->date.month == 12 ) &&
        ( aDateTime->date.day < 31 ),             FALSE );

    returnif_v( ( aDateTime->date.year >  2126 ),         FALSE );
    returnif_v( ( aDateTime->date.year == 2126 ) &&
        ( aDateTime->date.month >  2 ),           FALSE );
    returnif_v( ( aDateTime->date.year == 2126 ) &&
        ( aDateTime->date.month == 2 ) &&
        ( aDateTime->date.day > 5 ),              FALSE );

    returnif_v( ( aDateTime->date.month == 0 ) ||
        ( aDateTime->date.month > 12 ),           FALSE );

    fourYearCount = ( aDateTime->date.year - BASE_YEAR ) / 4;
    days          = (sint32)fourYearCount * DAYS_IN_4_YEARS;

    for( i = ( ( fourYearCount * 4 ) + BASE_YEAR ); i < (int)aDateTime->date.year; i++ )
    {
        days += DAYS_IN_1_YEAR;
        if( (i % 4) == 0 )
        {
            days++;
        }
    }

    days += sYearDays[ aDateTime->date.month - 1 ];
    if( ( aDateTime->date.month      >  FEBRUARY ) &&
        ( ( aDateTime->date.year % 4 ) == 0        ) )
    {
        days++;
    }

    days += aDateTime->date.day;

    /*------------------------------------------------------
    This accounts for the fact that internal time really
    starts one day before the base year.  The above
    conversion does not work for December 31, 1989.
    ------------------------------------------------------*/
    if( ( aDateTime->date.year  == 1989 ) &&
        ( aDateTime->date.month == 12   ) &&
        ( aDateTime->date.day   == 31   ) )
    {
        days = 0;
    }

    *aSeconds = days * SECONDS_PER_DAY;
    return TRUE;
}


//----------------------------------------------------------------------
//! \brief Converts a latitude/longitude from degrees to radians.
//! \param aDegrees The latitude/longitude in degrees
//! \return The latitude/longitude in radians.
//----------------------------------------------------------------------
double UTIL_convert_degrees_to_radians
    (
    double      aDegrees
    )
{
return aDegrees * (double)( PI / 180 );
}   /* UTIL_convert_degrees_to_radians() */


//----------------------------------------------------------------------
//! \brief Converts a latitude/longitude from degrees to semicircles.
//! \details 2^31 semicircles equal 180 degrees.  The conversion uses
//!     the equivalent 2^30 semicircles per 90 degrees to avoid
//!     overflow.
//! \param aDegrees The latitude/longitude in degrees
//! \return The latitude/longitude in semicircles.
//----------------------------------------------------------------------
sint32 UTIL_convert_degrees_to_semicircles
    (
    double      aDegrees
    )
{
return (sint32)( aDegrees * (double)( ( 1 << 30 ) / 90 ) );
}   /* UTIL_convert_degrees_to_semicircles() */

//----------------------------------------------------------------------
//! \brief Converts Garmin time to a structure containing GPS time.
//! \param aSeconds The Garmin time (seconds since Dec. 31, 1989 UTC)
//! \param aGpsTime Structure containing week number and time of week
//! \return TRUE, always.
//----------------------------------------------------------------------
void UTIL_convert_seconds_to_gps_time
    (
    const time_type * aSeconds,
    gps_time_type * aGpsTime
    )
{
    aGpsTime->week_number_days = (sint32)( *aSeconds / SECONDS_PER_DAY );
    aGpsTime->time_of_week     = *aSeconds - ( aGpsTime->week_number_days * SECONDS_PER_DAY );
}

//----------------------------------------------------------------------
//! \brief Converts from gps_time to Garmin time
//! \param aGpsTime Structure containing week number and time of week
//! \param aSeconds The Garmin time (seconds since Dec. 31, 1989 UTC)
//----------------------------------------------------------------------
void UTIL_convert_gps_time_to_seconds
    (
    const gps_time_type * aGpsTime,
    time_type * aSeconds
    )
{
    *aSeconds = ( aGpsTime->week_number_days * SECONDS_PER_DAY ) + aGpsTime->time_of_week;
}

//----------------------------------------------------------------------
//! \brief Converts a latitude/longitude from radians to degrees.
//! \param aRadians The latitude/longitude in radians
//! \return The latitude/longitude in degrees
//----------------------------------------------------------------------
double UTIL_convert_radians_to_degrees
    (
    double      aRadians
    )
{
return aRadians * (double)( 180 / PI );
}   /* UTIL_convert_radians_to_degrees() */

//----------------------------------------------------------------------
//! \brief Converts a latitude/longitude from semicircles to degrees.
//! \param aSemicircles The latitude/longitude in semicircles
//! \return The latitude/longitude in degrees
//----------------------------------------------------------------------
double UTIL_convert_semicircles_to_degrees
    (
    sint32      aSemicircles
    )
{
return (double)aSemicircles * 90 / ( 1 << 30 );
}   /* UTIL_convert_semicircles_to_degrees() */

//----------------------------------------------------------------------
//! \brief Converts from a Garmin time to a structure containing
//!     separate members for hour, minute, and second (time_type).
//! \param aSeconds The Garmin time to convert
//! \param aDateTime The output structure
//----------------------------------------------------------------------
void UTIL_convert_seconds_to_time_type
    (
    const time_type * aSeconds,
    date_time_data_type * aDateTime
    )
{
    time_type       secondsSinceMidnight;

    secondsSinceMidnight = *aSeconds % SECONDS_PER_DAY;

    aDateTime->time.hour   = (sint16)( secondsSinceMidnight / SECONDS_PER_HOUR );
    secondsSinceMidnight             -= aDateTime->time.hour * SECONDS_PER_HOUR;
    aDateTime->time.minute = (uint8)( secondsSinceMidnight / SECONDS_PER_MINUTE );
    aDateTime->time.second = (uint8)( secondsSinceMidnight - aDateTime->time.minute * SECONDS_PER_MINUTE );
}

//----------------------------------------------------------------------
//! \brief Converts a time_type to seconds since midnight.
//! \param aDateTime The structure containing hours, minutes, seconds
//! \param aSeconds The number of seconds since midnight.
//----------------------------------------------------------------------
void UTIL_convert_time_type_to_seconds
    (
    const date_time_data_type * aDateTime,
    time_type                 * aSeconds
    )
{
    *aSeconds = ( aDateTime->time.hour   * SECONDS_PER_HOUR )
              + ( aDateTime->time.minute * SECONDS_PER_MINUTE )
              +   aDateTime->time.second;
}


//----------------------------------------------------------------------
//! \brief Convert a hexadecimal ASCII string to an array of uint8
//! \param[in] aHexString The hexadecimal string to convert. May be
//!     null-terminated.  aHexString should not contain any prefix;
//!     only the digits 0-9 and a-f may be present.
//! \param[out] aBinaryData An array of uint8 to receive the output.
//!     This array's size must be at least max_bytes.
//! \param[in] aMaxBytes The maximum number of bytes to convert. If
//!     aHexString is not null-terminated its length must be at least
//!     2 * max_bytes.
//! \return The number of bytes actually converted.  This may be less
//!     than max_bytes if a null terminator was found, and will be 0
//!     if a character that is not a hexadecimal digit was encountered
//!     or if the length of aHexString is not even.
//! \note aBinaryData may be modified even if 0 is returned.
//----------------------------------------------------------------------
uint16 UTIL_hex_to_uint8
    (
    const char *  aHexString,
    uint8      *  aBinaryData,
    uint16        aMaxBytes
    )
{
    static const char hex[] = "0123456789abcdef";
    const char * currentChar;
    const char * hexPos;
    bool         highNibble = true;
    uint16       binaryIndex = 0;
    uint8        workByte = 0;

    currentChar = aHexString;

    while( binaryIndex < aMaxBytes && *currentChar != '\0' )
    {
        hexPos = strchr( hex, tolower( *currentChar ) );
        if( hexPos != NULL )
        {
            if( highNibble )
            {
                workByte = (uint8)( ( hexPos - hex ) << 4 );
            }
            else
            {
                workByte += (uint8)( hexPos - hex );
                aBinaryData[binaryIndex++] = workByte;
            }
            highNibble = !highNibble;
        }
        else return 0;
        currentChar++;
    }
    if( !highNibble )
        binaryIndex = 0;

    return binaryIndex;
}

//----------------------------------------------------------------------
//! \brief Convert from binary to a hexadecimal string
//! \details Converts an array of uint8 values to a displayable string,
//!     one octet (uint8) at a time.  No prefix is included in the
//!     output.  For example, given a null-terminated ASCII string
//!     "ABC" and aNumBytes = 4, the output would be the string
//!     "41424300".
//! \param[in] aData The binary data.
//! \param[out] aOutput The null-terminated output string.  This must
//!     be a buffer of at least (2 * aNumBytes + 1) characters.
//! \param[in] aNumBytes The number of bytes to convert.
//----------------------------------------------------------------------
void UTIL_uint8_to_hex
    (
    const uint8         * aData,
    char                * aOutput,
    uint8                 aNumBytes
    )
{
    static const char hex[] = "0123456789abcdef";

    for( int i = 0; i < aNumBytes; i++ )
    {
        aOutput[2 * i]   = hex[ aData[i] / 16 ];
        aOutput[2 * i + 1] = hex[ aData[i] % 16 ];
    }
    aOutput[2 * aNumBytes] = '\0';
}

//----------------------------------------------------------------------
//! \brief Converts a null-terminated string in hexadecimal format to
//!     an array of uint16 numbers, assuming natural byte ordering in
//!     the hex.  No prefix should be present in the hex string.
//! \details For example, given a hex string "01020304" and aMaxWords
//!     2, the output will be the equivalent of { 258, 772 } decimal.
//! \note Conversion will stop when an invalid hexadecimal digit is
//!     encountered in the input or when the null terminator is found,
//!     whichever comes first.
//! \param[in] aHexString The ASCII null-terminated string containing
//!     the hex to convert.
//! \param[out] aBinaryData Pointer to an array of uint16 to contain
//!     the output.
//! \param[in] aMaxWords The maximum number of words to convert.
//! \return The number of complete words converted, or 0 if either
//!     aHexString contains a character that is not a hex digit or
//!     the length of aHexString is not a multiple of 4.
//----------------------------------------------------------------------
uint8 UTIL_hex_to_uint16
    (
    const char * aHexString,
    uint16     * aBinaryData,
    uint8        aMaxWords
    )
{
    static const char hex[] = "0123456789abcdef";
    const char * currentChar;
    const char * hexPos;
    int          state = 0;
    uint8        binaryIndex = 0;
    uint16       work = 0;

    currentChar = aHexString;

    while( binaryIndex < aMaxWords && *currentChar != '\0' )
    {
        hexPos = strchr( hex, tolower( *currentChar ) );
        if( hexPos != NULL )
        {
            switch( state )
                {
                case 0: //highest byte
                    work = (uint16)( hexPos - hex ) << 12;
                    break;
                case 1: //middle high
                    work += (uint16)( hexPos - hex ) << 8;
                    break;
                case 2: //middle low
                    work += (uint16)( hexPos - hex ) << 4;
                    break;
                case 3: //lowest
                    work += (uint16)( hexPos - hex );
                    aBinaryData[binaryIndex++] = work;
                }
            state++;
            if( state > 3 ) state = 0;
        }
        else return 0;
        currentChar++;
    }

    if( state != 0 )
    {
        binaryIndex = 0;
    }

    return binaryIndex;
}


//----------------------------------------------------------------------
//! \brief Determine whether an array of characters consists only
//!     of printable ASCII.
//! \details Uses isprint().
//! \param[in] aData The character array to test.
//! \param[in] aLength The number of characters to test.
//! \return TRUE if all characters are printable, FALSE otherwise.
//----------------------------------------------------------------------
bool UTIL_data_is_printable
    (
    const char * aData,
    int          aLength
    )
{
    for( int i = 0; i < aLength; i++ )
    {
        if( aData[i] < 0 || !isprint( aData[i] ) )
        {
            return false;
        }
    }
    return true;
}

//----------------------------------------------------------------------
//! \brief Determine whether an array of characters consists only
//!     of numeric, and that the value when converted from a string
//!     is in the valid range for a uint32.
//! \param[in] aData The character array to test.
//! \return true if the resulting string is a valid uint32, false
//!     otherwise.
//----------------------------------------------------------------------
bool UTIL_data_is_uint32
    (
    const char * aData
    )
{
    if( strlen( aData ) == 0 )
    {
        return false;
    }

    if( _atoi64( aData ) > max_uint_val( uint32 ) ||
        _atoi64( aData ) < 0                      )
    {
        return false;
    }

    for( unsigned int i = 0; i < strlen( aData ); i++ )
    {
        if( aData[i] < '0' || aData[i] > '9' )
        {
            return false;
        }
    }
    return true;
}

//----------------------------------------------------------------------
//! \brief Converts a time_type from UTC to local time.
//! \param[in] aUtcTime The UTC time as a time_type.
//! \param[out] aLocalTime The local time as a time_type.
//----------------------------------------------------------------------
void UTIL_convert_UTC_to_local
    (
    const time_type * aUtcTime,
    time_type       * aLocalTime
    )
{
    TIME_ZONE_INFORMATION tzinfo;

    switch ( GetTimeZoneInformation( &tzinfo ) )
    {
    case TIME_ZONE_ID_DAYLIGHT:
        *aLocalTime = *aUtcTime - ( ( tzinfo.Bias + tzinfo.DaylightBias ) * SECONDS_PER_MINUTE );
        break;
    default:
        *aLocalTime = *aUtcTime - ( ( tzinfo.Bias + tzinfo.StandardBias ) * SECONDS_PER_MINUTE );
    }
}

//----------------------------------------------------------------------
//! \brief Converts a time structure (date_time_data_type) to a time
//!     string representation.
//! \param[in] aDateTime The structure containing the time.
//! \param[out] aResultString A null-terminated ASCII string of the
//!     form "HH:MM:SS AA" (hours, minutes, seconds, AM/PM,
//!     respectively).
//----------------------------------------------------------------------
void UTIL_format_time_string
    (
    const date_time_data_type * aDateTime,
    char                      * aResultString
    )
{
    boolean morning = TRUE;
    sint16  hour = aDateTime->time.hour;

    // 0 <= hour <= 23
    if( hour >= 12 )
    {
        hour -= 12;
        morning = FALSE;
    }
    // 0 <= hour <= 11
    if( hour == 0 ) hour = 12;

    if( morning )
        sprintf( aResultString, "%02d:%02d:%02d AM", hour, aDateTime->time.minute, aDateTime->time.second );
    else
        sprintf( aResultString, "%02d:%02d:%02d PM", hour, aDateTime->time.minute, aDateTime->time.second );
}

//----------------------------------------------------------------------
//! \brief Get the current server time in Garmin format
//! \return The current time as a time_type
//----------------------------------------------------------------------
time_type UTIL_get_current_garmin_time()
{
    time_t   aCurrentTime;
    tm     * aCurrentTm;

    time( &aCurrentTime );
    //convert to nice struct that separates into years since 1900, days, hours, minutes...
    aCurrentTm = gmtime( &aCurrentTime );

    //Garmin time counts seconds from Dec 31, 1989
    //First, find seconds from Jan 1 1990 to aCurrentTm, excluding leap days
    time_type seconds = ( ( aCurrentTm->tm_year - 90 ) * SECONDS_PER_DAY * DAYS_IN_1_YEAR )
                      + ( aCurrentTm->tm_yday          * SECONDS_PER_DAY    )
                      + ( aCurrentTm->tm_hour          * SECONDS_PER_HOUR   )
                      + ( aCurrentTm->tm_min           * SECONDS_PER_MINUTE )
                      + ( aCurrentTm->tm_sec                                );

    // Then add seconds from Dec 31 1989 to Jan 1 1990
    seconds += SECONDS_PER_DAY;

    //adjust for leap days
    // 88 last leap year prior to base year
    // (subtract 88 since time_t base year is 1900)
    int leapDays = ( aCurrentTm->tm_year - 88 ) / 4;

    // if in a leap year and on or before Feb 28
    //   -> leap day has not happened this year
    if( ( aCurrentTm->tm_year % 4 == 0 ) &&
        ( aCurrentTm->tm_yday <= sYearDays[2] ) )
    {
        leapDays--;
    }
    seconds += leapDays * SECONDS_PER_DAY;
    return seconds;
}

//----------------------------------------------------------------------
//! \brief Format a date as a string
//! \details Formats the date portion of aDateTime as a string, in
//!     MM/DD/YYYY format
//! \param aDateTime The date to format.
//! \param aResultString Pointer to a char[11] or longer to contain
//!    the formatted date.
//----------------------------------------------------------------------
void UTIL_format_date_string
    (
    const date_time_data_type * aDateTime,
    char                      * aResultString
    )
{
    sprintf( aResultString, "%02d/%02d/%04d", aDateTime->date.month, aDateTime->date.day, aDateTime->date.year );
}

//----------------------------------------------------------------------
//! \brief Determine the two-dimensional velocity
//! \param  aNorthVelocity The north velocity (negative is south)
//! \param  aEastVelocity  The east velocity (negative is west)
//! \return The 2d velocity, which is
//!     sqrt( aNorthVelocity^2 + aEastVelocity^2 ).
//----------------------------------------------------------------------
double UTIL_calc_2d_speed
    (
    float32 aNorthVelocity,
    float32 aEastVelocity
    )
{
    return sqrt( pow( (double)aNorthVelocity, 2.0 ) +
                 pow( (double)aEastVelocity,  2.0 ) );
}

//----------------------------------------------------------------------
//! \brief Determine the nearest cardinal aCardinalDirection
//! \param aNorthVelocity The north velocity (negative is south)
//! \param aEastVelocity  The aEastVelocity velocity (negative is west)
//! \param aCardinalDirection A buffer of at least 3 bytes to contain
//!     the cardinal direction, which is one of "N", "E", "S", "W",
//!     "NE", "NW", "SE", "SW", or ""
//----------------------------------------------------------------------
void UTIL_calc_2d_direction
    (
    float32   aNorthVelocity,
    float32   aEastVelocity,
    char    * aCardinalDirection
    )
{
    if( ( aNorthVelocity == 0 ) && ( aEastVelocity == 0 ) )
    {
        strcpy( aCardinalDirection, "" );
        return;
    }

    double angle = atan2( (double)aNorthVelocity, (double)aEastVelocity );

    //between -pi/8 and pi/8 = E
    if( angle > ( PI * -1/8 ) && angle < ( PI * 1/8 ) )
        strcpy( aCardinalDirection, "E" );
    //between pi/8 and 3pi/8 = NE
    else if( angle >= ( PI * 1/8 ) && angle < ( PI * 3/8 ) )
        strcpy( aCardinalDirection, "NE" );
    //between 3pi/8 and 5pi/8 = N
    else if( angle >= ( PI * 3/8 ) && angle < ( PI * 5/8 ) )
        strcpy( aCardinalDirection, "N" );
    //between 5pi/8 and 7pi/8 = NW
    else if( angle >= ( PI * 5/8 ) && angle < ( PI * 7/8 ) )
        strcpy( aCardinalDirection, "NW" );
    //greater than 7pi/8 OR less than -7pi/8 = W
    else if( angle >= ( PI * 7/8 ) || angle < ( PI * -7/8 ) )
        strcpy( aCardinalDirection, "W" );
    //between -7*pi/8 and -5pi/8 = SW
    else if( angle >= ( PI * -7/8 ) && angle < ( PI * -5/8 ) )
        strcpy( aCardinalDirection, "SW" );
    //between -5pi/8 and -3pi/8 = S
    else if( angle >= ( PI * -5/8 ) && angle < ( PI * -3/8 ) )
        strcpy( aCardinalDirection, "S" );
    //between -3pi/8 and -pi/8 = SE
    else if( angle >= ( PI * -3/8 ) && angle < ( PI * -1/8 ) )
        strcpy( aCardinalDirection, "SE" );
    else
        strcpy( aCardinalDirection, "??" );
}
