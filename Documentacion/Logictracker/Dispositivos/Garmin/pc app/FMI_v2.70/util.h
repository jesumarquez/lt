/*********************************************************************
*
*   HEADER NAME:
*       util.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef _UTIL_H_
#define _UTIL_H_

#include "garmin_types.h"

//! The ratio of a circle's circumference to its diameter
#define PI                      ( 3.14159265358979323846 )

//! An invalid value for time_type
#define INV_DATE_TIME           ( 0xFFFFFFFF )

//! \brief The base year for Garmin dates
#define BASE_YEAR               ( 1990 )

//! \brief Number of days in four years
#define DAYS_IN_4_YEARS         ( 365 * 4 + 1 )

//! \brief Number of days in a non-leap year
#define DAYS_IN_1_YEAR          ( 365 )

//! \brief Number of days in one week
#define DAYS_IN_1_WEEK          ( 7 )

//! \brief Number of seconds in one week
#define SECONDS_PER_WEEK        ( 60 * 60 * 24 * 7 )

//! \brief Number of seconds in one day
#define SECONDS_PER_DAY         ( 60 * 60 * 24 )

//! \brief Number of seconds in one hour
#define SECONDS_PER_HOUR        ( 60 * 60 )

//! \brief Number of seconds in one minute
#define SECONDS_PER_MINUTE      ( 60 )

//! \brief One-based month number for February
#define FEBRUARY                ( 2 )
//! \brief One-based month number for March
#define MARCH                   ( 3 )
//! \brief One-based month number for December
#define DECEMBER                ( 12 )

double UTIL_convert_degrees_to_radians
    (
    double      aDegrees
    );

sint32 UTIL_convert_degrees_to_semicircles
    (
    double      aDegrees
    );

double UTIL_convert_radians_to_degrees
    (
    double      aRadians
    );

double UTIL_convert_semicircles_to_degrees
    (
    sint32      aSemicircles
    );

uint16 UTIL_hex_to_uint8
    (
    const char *  aHexString,
    uint8      *  aBinaryData,
    uint16        aMaxBytes
    );

void UTIL_uint8_to_hex
    (
    const uint8         * aData,
    char                * aOutput,
    uint8                 aNumBytes
    );

uint8 UTIL_hex_to_uint16
    (
    const char * aHexString,
    uint16     * aBinaryData,
    uint8        aMaxWords
    );

bool UTIL_data_is_printable
    (
    const char * aData,
    int          aLength
    );

bool UTIL_data_is_uint32
    (
    const char * aData
    );

void UTIL_format_time_string
    (
    const date_time_data_type * aDateTime,
    char                      * aResultString
    );

void UTIL_format_date_string
    (
    const date_time_data_type * aDateTime,
    char                      * aResultString
    );

void UTIL_convert_UTC_to_local
    (
    const time_type * aUtcTime,
    time_type       * aLocalTime
    );

time_type UTIL_get_current_garmin_time
    (
    void
    );

void UTIL_convert_gps_time_to_seconds
    (
    const gps_time_type * aGpsTime,
    time_type           * aSeconds
    );

void UTIL_convert_time_type_to_seconds
    (
    const date_time_data_type * aDateTime,
    time_type                 * aSeconds
    );

void UTIL_convert_seconds_to_time_type
    (
    const time_type     * aSeconds,
    date_time_data_type * aDateTime
    );

void UTIL_convert_seconds_to_date_type
    (
    const time_type     * aSeconds,
    date_time_data_type * aDateTime
    );

double UTIL_calc_2d_speed
    (
    float32 aNorthVelocity,
    float32 aEastVelocity
    );

void UTIL_calc_2d_direction
    (
    float32   aNorthVelocity,
    float32   aEastVelocity,
    char    * aCardinalDirection
    );

#endif  /* _UTIL_H_ */
