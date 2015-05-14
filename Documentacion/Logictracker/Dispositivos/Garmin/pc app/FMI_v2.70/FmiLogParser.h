/*********************************************************************
*
*   HEADER NAME:
*       FmiLogParser.h
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef FmiLogParser_H
#define FmiLogParser_H

#include "LogParser.h"
#include "fmi.h"

//----------------------------------------------------------------------
//! \brief LogParser for packets sent/received by the
//!     FmiApplicationLayer
//! \details Parses and resends FMI packets, plus PVT and the command
//!     packets defined in the FMI Interface Control Specification.
//----------------------------------------------------------------------
class FmiLogParser : public LogParser
{
public:
    FmiLogParser();
    virtual ~FmiLogParser();

    virtual CString getPacketDetail
        (
        int aLineNumber
        );

    virtual CString getPacketTitle
        (
        int aLineNumber
        );

    virtual void resendPacket
        (
        int aLineNumber
        );

private:
    CString formatBoolean
        (
        boolean aBool
        );

    CString formatFmiPacket
        (
        uint16   aFmiPacketId,
        uint8  * aFmiPayload,
        uint8    aFmiPayloadSize
        );

    CString FmiLogParser::formatLatitude
        (
        sint32  aSemicircles
        );

    CString FmiLogParser::formatLongitude
        (
        sint32  aSemicircles
        );

    CString formatMessageId
        (
        const uint8 * aMessageId,
        uint8         aMessageIdSize
        );

    CString formatText
        (
        const char * aText,
        int          aMaxLength
        );

    CString FmiLogParser::formatTime
        (
        time_type aTimestamp
        );

    CString getFmiPacketName
        (
        uint16 aPacketId
        );

    CString getGarminPacketName
        (
        uint8 aPacketId
        );

    CString getGarminCommandName
        (
        uint16 aCommandId
        );

    void initGarminCommandNames();
    void initGarminPacketNames();

#if( FMI_SUPPORT_A602 )
    void initFmiPacketNames();
#endif

#if( FMI_SUPPORT_A607 )
    void initFmiFeatureNames();
#endif

    //! Map of Garmin command IDs to display names for formatting
    std::map<uint16, CString> mGarminCommandNames;

    //! Map of Garmin packet IDs to display names for formatting
    std::map<uint8, CString>  mGarminPacketNames;

#if( FMI_SUPPORT_A602 )
    //! Map of FMI packet IDs to display names for formatting
    std::map<uint16, CString> mFmiPacketNames;
#endif

#if( FMI_SUPPORT_A607 )
    //! Map of FMI feature IDs to display names for formatting
    std::map<uint16, CString> mFmiFeatureNames;
#endif

};

#endif
