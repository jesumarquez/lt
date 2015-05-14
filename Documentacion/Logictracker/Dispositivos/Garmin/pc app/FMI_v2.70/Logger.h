/*********************************************************************
*
*   HEADER NAME:
*       Logger.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef Logger_H
#define Logger_H

class Logger;

#include <fstream>

#include "ApplicationLayer.h"
#include "Packet.h"

//----------------------------------------------------------------------
//! \brief Log writer
//! \details The logger writes the packet log.
//----------------------------------------------------------------------
class Logger
{
public:
    static void clearLog();
    static void closeLog();
    static bool isLogOpen();
    static void logRawData
        (
        Packet * aPacket,
        bool     aIsTx = true
        );


    static const char * LOG_FILE;
protected:

private:
    //! \brief Construct a new Logger
    Logger();

    //! \brief Destructor
    virtual ~Logger();

    //! Time when the packet log was started/cleared; used to compute
    //!     offsets
    static SYSTEMTIME           mLogStartTime;

    //! File stream that the packet log is written to
    static std::wofstream       mLogFile;
};

#endif
