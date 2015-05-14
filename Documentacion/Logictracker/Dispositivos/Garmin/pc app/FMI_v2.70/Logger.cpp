/*********************************************************************
*
*   MODULE NAME:
*       Logger.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include <iomanip>

#include "Logger.h"
#include "Event.h"
#include "util.h"

SYSTEMTIME     Logger::mLogStartTime;
std::wofstream Logger::mLogFile;

//! The log file that this Logger writes to
const char * Logger::LOG_FILE = "fmi_pc_app.log";

//----------------------------------------------------------------------
//! \brief Logs the raw data of the packet passed in to a file
//! \param  aPacket The packet to log
//! \param  aIsTx If true, this packet was transmitted (if false, this
//!     packet was received)
//----------------------------------------------------------------------
void Logger::logRawData
    (
    Packet * aPacket,
    bool     aIsTx
    )
{
    ASSERT( mLogFile.is_open() );
    ASSERT( aPacket != NULL );
    ASSERT( aPacket->getRawSize() > 0 );

    const uint8 * frame     = aPacket->getRawBytes();
    uint32        frameSize = aPacket->getRawSize();

    SYSTEMTIME currentSystemTime;
    FILETIME   currentFileTime, startFileTime;
    ULARGE_INTEGER currentTimeUlarge, startTimeUlarge;
    GetLocalTime( &currentSystemTime );
    SystemTimeToFileTime( &currentSystemTime, &currentFileTime );
    SystemTimeToFileTime( &Logger::mLogStartTime, &startFileTime );

    currentTimeUlarge.HighPart = currentFileTime.dwHighDateTime;
    currentTimeUlarge.LowPart = currentFileTime.dwLowDateTime;
    startTimeUlarge.HighPart = startFileTime.dwHighDateTime;
    startTimeUlarge.LowPart = startFileTime.dwLowDateTime;

    ULONGLONG elapsedMs;
    // currentTimeUlarge and startTimeUlarge are in 100 nanosecond intervals
    // there are 10 000 of these intervals in 1 ms.
    elapsedMs = ( currentTimeUlarge.QuadPart - startTimeUlarge.QuadPart ) / 10000;

    // Log viewer makes the following assumption
    ASSERT( elapsedMs < max_uint_val( uint32 ) );

    if( aIsTx )
        mLogFile << 'T';
    else
        mLogFile << 'R';
    mLogFile << elapsedMs << '-' << std::hex;
    for( uint32 i = 0; i < frameSize; i++ )
    {
        mLogFile << std::setw( 2 ) << frame[i];
    }
    mLogFile << std::endl << std::dec;
    mLogFile.flush();
    Event::post( EVENT_LOG_PACKET );
}

//----------------------------------------------------------------------
//! \brief Empties the packet log
//! \details Closes and reopens the log file (removing any existing
//!    data), writes a start time entry with the current time, and
//!    sends a notification message that the log has changed.
//----------------------------------------------------------------------
void Logger::clearLog()
{
    if( mLogFile.is_open() )
        mLogFile.close();

    mLogFile.open( LOG_FILE, std::ios_base::out );
    mLogFile.fill('0');
    GetLocalTime( &mLogStartTime );
    mLogFile << mLogStartTime.wHour << ','
        << mLogStartTime.wMinute << ','
        << mLogStartTime.wSecond << ','
        << mLogStartTime.wMilliseconds
        << std::endl;
    Event::post( EVENT_LOG_PACKET );
}

//----------------------------------------------------------------------
//! \brief Close the log file.
//----------------------------------------------------------------------
void Logger::closeLog()
{
    if( mLogFile.is_open() )
        mLogFile.close();
}

//----------------------------------------------------------------------
//! \brief Returns true if the log file is open.
//----------------------------------------------------------------------
bool Logger::isLogOpen()
{
    return mLogFile.is_open();
}
