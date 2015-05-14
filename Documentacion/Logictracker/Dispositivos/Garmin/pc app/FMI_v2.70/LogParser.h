/*********************************************************************
*
*   HEADER NAME:
*       LogParser.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef LogParser_H
#define LogParser_H

#include "stdafx.h"
#include <string>
#include <map>
#include "garmin_types.h"

//----------------------------------------------------------------------
//! \brief Abstract base class for log item parsers
//! \details The LogParser is used by the LogViewer to perform the work
//!     associated with parsing the log.
//----------------------------------------------------------------------
class LogParser
{
public:
    LogParser();
    virtual ~LogParser();

    void init
        (
        const CString& aFilename
        );

    CString getFilename();

    //! \brief Resend the packet at this line number
    //! \param aLineNumber the line number of the packet to resend
    virtual void resendPacket
        (
        int aLineNumber
        ) = 0;

    //! \brief Return the packet detail as a formatted string
    //! \param aLineNumber the line number of the packet to parse
    virtual CString getPacketDetail
        (
        int aLineNumber
        ) = 0;

    //! \brief Return the packet title as a formatted string
    //! \param aLineNumber the line number of the packet to parse
    virtual CString getPacketTitle
        (
        int aLineNumber
        ) = 0;

    BOOL readLog();
    void reset();

    int getLineCount() const;
    void setRenderWidth
        (
        int aWidth
        );

protected:
    CString formatMultiLineHex
        (
        int     aSize,
        uint8 * aData
        );

    //! File name of the currently open log file
    CString mLogFilename;

    //! Hour of the time when the log file was created (from log header)
    int mLogStartHr;

    //! Minutes of the time when the log file was created (from log header)
    int mLogStartMin;

    //! Seconds of the time when the log file was created (from log header)
    int mLogStartSec;

    //! Milliseconds of the time when the log file was created (from log header)
    int mLogStartMillis;

    //! If TRUE, the log was started in the mIsMorning (computed from log header)
    BOOL mIsMorning;

    //! Offset of the log file where parsing stopped
    std::streamoff mParseEndOffset;

    //! Map of log items to the offset in the log file where the text line starts.
    std::map<int, std::streamoff> mLineOffset;

    //! Number of lines that have been parsed so far
    int mLineCount;

    //! Available width in pixels to render the packet detail
    int mRenderWidth;
};

#endif
