/*********************************************************************
*
*   MODULE NAME:
*       LogParser.cpp
*
* Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include <fstream>
#include "util.h"

#include "LogParser.h"

using namespace std;

//----------------------------------------------------------------------
//! \brief Constructor.
//! \details Does nothing special
//----------------------------------------------------------------------
LogParser::LogParser()
{
}

//----------------------------------------------------------------------
//! \brief Destructor.
//! \details Does nothing special
//----------------------------------------------------------------------
LogParser::~LogParser()
{
}

//----------------------------------------------------------------------
//! \brief Reads from the log file starting from the end of the last
//!     read position and adds it to the display.
//! \details The display text is normally the Garmin packet name, but
//!     for FMI packets the FMI packet name is used, and for Garmin
//!     commands the Garmin command name is used.
//! \return TRUE if the log file was updated, FALSE if not (or an error
//!     occurred)
//----------------------------------------------------------------------
BOOL LogParser::readLog()
{
    BOOL updated = FALSE;

    int filenameLength = WideCharToMultiByte( CP_ACP, 0, mLogFilename, -1, NULL, 0, NULL, NULL );
    char *filenameAnsi = new char[filenameLength];
    WideCharToMultiByte( CP_ACP, 0, mLogFilename, -1, filenameAnsi, filenameLength, NULL, NULL );
    ifstream logFile( filenameAnsi, ios_base::in );
    delete[] filenameAnsi;

    if( logFile.good() )
    {
        const char  * logLine;
        std::string   logLineString;
        logFile.seekg( mParseEndOffset, ios_base::beg );
        if( mParseEndOffset == 0 )
        {
            char * number;
            char   timeString[20];

            memset( timeString, 0, sizeof( timeString ) );
            getline( logFile, logLineString );
            logLine = logLineString.c_str();
            strncpy( timeString, logLine, sizeof( timeString ) - 1 );
            mParseEndOffset = logFile.tellg(); //doesn't count '\n'

            number = strtok( timeString, "," );
            if( number != NULL ) mLogStartHr = atoi( number );

            number = strtok( NULL, "," );
            if( number != NULL ) mLogStartMin = atoi( number );

            number = strtok( NULL, "," );
            if( number != NULL ) mLogStartSec = atoi( number );

            number = strtok( NULL, "," );
            if( number != NULL ) mLogStartMillis = atoi( number );

            if( mLogStartHr < 12 )
            {
                mIsMorning = TRUE;
                if( mLogStartHr == 0 )
                {
                    mLogStartHr = 12;
                }
            }
            else
            {
                mIsMorning = FALSE;
                if( mLogStartHr != 12 )
                {
                    mLogStartHr -= 12;
                }
            }
        }
        if( !logFile.eof() )
            updated = TRUE;
        while( !logFile.eof() )
        {
            std::streamoff packetStartOffset;

            packetStartOffset = logFile.tellg();
            getline( logFile, logLineString );
            logLine = logLineString.c_str();
            mParseEndOffset = logFile.tellg();
            if( logLine[0] != '\0' && mParseEndOffset != -1 )
            {
                // update the index
                mLineOffset[ mLineCount ] = packetStartOffset;
                mLineCount++;
            }
            else
            {    // at the end of the file
                mParseEndOffset = packetStartOffset;
                break;
            }
        }
    }
    logFile.close();
    return updated;
}    /* readLog() */

//----------------------------------------------------------------------
//! \brief Reset the log parser to initial state.
//----------------------------------------------------------------------
void LogParser::reset()
{
    mParseEndOffset = 0;
    mLineCount = 0;
    mLineOffset.clear();
}

//----------------------------------------------------------------------
//! \brief Return the number of lines parsed so far
//! \return The number of lines parsed.
//----------------------------------------------------------------------
int LogParser::getLineCount() const
{
    return mLineCount;
}

//----------------------------------------------------------------------
//! \brief Set the number of pixels available for rendering text
//! \param aWidth The width of the packet detail area, in pixels
//----------------------------------------------------------------------
void LogParser::setRenderWidth
    (
    int aWidth
    )
{
    mRenderWidth = aWidth;
}

//----------------------------------------------------------------------
//! \brief Format bytes into a multi logLine hex dump format
//! \param aSize The number of bytes to format
//! \param aData The bytes to format
//! \return A string containing the hex dump
//----------------------------------------------------------------------
CString LogParser::formatMultiLineHex
    (
    int     aSize,
    uint8 * aData
    )
{
    CString hexString;
    int charsPerLine = ( ( mRenderWidth - 86 ) / 15 );
    for( int i = 0; i < aSize; i++ )
    {
        //check screen size and format accordingly
        if( i != 0 && i % charsPerLine == 0 )
            hexString.AppendFormat( _T("\r\n+ x%03x\t\t"), i );

        //print the data
        hexString.AppendFormat
            (
            _T(" %02x"),
            aData[i]
            );
    }
    hexString.Append( _T("\r\n") );
    return hexString;
}

//----------------------------------------------------------------------
//! \brief Initialize the LogParser to read a particular file
//! \param aFilename The file to read
//----------------------------------------------------------------------
void LogParser::init( const CString& aFilename )
{
    mLogFilename = aFilename;
    reset();
    readLog();
}

//----------------------------------------------------------------------
//! \brief Get the path of the log file being parsed
//! \return The path of the log file being parsed
//----------------------------------------------------------------------
CString LogParser::getFilename()
{
    return mLogFilename;
}
