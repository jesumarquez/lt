/*********************************************************************
*
*   HEADER NAME:
*       garmin_types.h - Data structures, types, and constants common
*               to Garmin protocols
*
* Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef GARMIN_TYPES_H
#define GARMIN_TYPES_H

#include "util_macros.h"
#include <limits.h>

//! \brief Minimum packet size:  DLE + Packet ID + Payload Size + Data + Checksum + DLE + ETX
#define MIN_PACKET_SIZE         ( 1  + 1         + 1            + 0    + 1        + 1   + 1  )

//! \brief Maximum payload size, before DLE stuffing
#define MAX_PAYLOAD_SIZE        ( 255 )

//! \brief Maximum packet size that can be transmitted
//! \details The longest packet that can be transmitted is (DLE +
//!     Packet ID + Payload Size + Payload + Checksum + DLE + ETX).
//!     The payload size, payload, and checksum can all be filled with
//!     16 decimal before stuffing, thus requiring each byte to be
//!     DLE-stuffed.  Although such a packet would have an incorrect
//!     checksum, it is nonetheless a possible packet that can be
//!     received.
#define MAX_PACKET_SIZE         ( 1 + 1 + 2 * 1 + 2 * MAX_PAYLOAD_SIZE + 2 * 1 + 1 + 1 )

//! Size of packet header, before DLE stuffing
#define SIZE_OF_HEADER          ( 1 + 1 + 1 )

//! Size of packet footer (after payload and checksum)
#define SIZE_OF_FOOTER          (1 + 1 )

#define ID_ETX_BYTE             3
#define ID_ACK_BYTE             6
#define ID_DLE_BYTE             16
#define ID_NAK_BYTE             21

#ifndef TRUE
#define TRUE                    1
#define FALSE                   0
#endif
/*--------------------------------------------------------------------
                          FUNDAMENTAL TYPES
--------------------------------------------------------------------*/
//! 8-bit value representing FALSE (0) or TRUE (nonzero)
typedef unsigned char           boolean;

//! 8-bit signed integer
typedef signed char             sint8;
//! 16-bit signed integer
typedef signed short int        sint16;
//! 32-bit signed integer
typedef signed long int         sint32;

//! 8-bit unsigned integer
typedef unsigned char           uint8;
//! 16-bit unsigned integer
typedef unsigned short int      uint16;
//! 32-bit unsigned integer
typedef unsigned long int       uint32;

//! \brief 32-bit IEEE-format floating point data.
//! (1 sign bit, 8 exponent bits, and 23 mantissa bits)
typedef float                   float32;

//! \brief 64-bit IEEE-format floating point data.
//! (1 sign bit, 11 exponent bits, and 52 mantissa bits)
typedef double                  float64;

// Verify that the primitive types have the correct size.  The C
// standard indicates that sizeof() returns units of char, so we must
// also verify that a char is 8 bits. If any of these asserts fail, the
// app would not be conforming to the protocol, and changes would be needed.
_compiler_assert( CHAR_BIT          == 8, _FMI_H_ );
_compiler_assert( sizeof( boolean ) == 1, _FMI_H_ );
_compiler_assert( sizeof( uint8 )   == 1, _FMI_H_ );
_compiler_assert( sizeof( sint8 )   == 1, _FMI_H_ );
_compiler_assert( sizeof( uint16 )  == 2, _FMI_H_ );
_compiler_assert( sizeof( sint16 )  == 2, _FMI_H_ );
_compiler_assert( sizeof( uint32 )  == 4, _FMI_H_ );
_compiler_assert( sizeof( sint32 )  == 4, _FMI_H_ );
_compiler_assert( sizeof( float32 ) == 4, _FMI_H_ );
_compiler_assert( sizeof( float64 ) == 8, _FMI_H_ );

//! \brief Absolute time (number of seconds since 12/31/1989 12:00 am UTC)
//!
//! \details The time_type is used to indicate an absolute time. It is an
//! unsigned 32-bit integer and its value is the number of seconds since
//! 12:00 am, December 31, 1989 UTC. A hex value of 0xFFFFFFFF represents
//! an invalid time, and the client will ignore the time.
typedef uint32 time_type;

//! \brief Garmin packet ID.
//! \see id_enum for supported values
typedef uint8 id_type;

//! The packet header (the bytes before the payload)
struct header_type
    {
    uint8           dle;     //!< DLE byte
    uint8           id;      //!< Garmin packet ID
    uint8           size;    //!< Size of payload
    };

//! The packet footer (the bytes after the payload and checksum)
struct footer_type
    {
    uint8           dle;     //!< DLE byte
    uint8           etx;     //!< ETX byte
    };

//! \brief Indicates a latitude and longitude in semicircles.
//! \details Indicates a 2D position in semicircles, where 2^31
//!     semicircles equal 180 degrees. North latitudes and East
//!     longitudes are indicated with positive numbers; South latitudes
//!     and West longitudes are indicated with negative numbers.  All
//!     positions are given in WGS-84.
struct sc_position_type
    {
    sint32      lat;         //!< latitude in semicircles
    sint32      lon;         //!< longitude in semicircles
    };

//! \brief Indicates a latitude and longitude in radians.
//! \details Indicates a 2D position in radians. North latitudes and
//!     East longitudes are indicated with positive numbers; South
//!     latitudes and West longitudes are indicated with negative
//!     numbers.  All positions are given in WGS-84.
struct double_position_type
    {
    float64     lat;         //!< latitude in radians, positive is north
    float64     lon;         //!< longitude in radians, positive is east
    };

//! Encapsulates the fields of a GPS time for conversion.
//! \details This is a UTC time.
struct gps_time_type
    {
    sint32      week_number_days;   //!< Days since December 31st, 1989 to beginning of week (i.e., this is a Sunday)
    sint32      time_of_week;       //!< Seconds since 12:00 AM Sunday
    };

//! Date & time data type with separate fields for month, day, year, hour, minute, and second
struct date_time_data_type
    {
    struct _date          //! Date portion of data type
        {
        uint8    month;   //!< month (1-12)
        uint8    day;     //!< day (1-31)
        uint16   year;    //!< Real year (1990 means 1990!)
        } date;
    struct _time          //! Time portion of data type
        {
        sint16   hour;    //!< hour (0-65535), range required for correct ETE conversion
        uint8    minute;  //!< minute (0-59)
        uint8    second;  //!< second (0-59)
        } time;
    };

#endif
