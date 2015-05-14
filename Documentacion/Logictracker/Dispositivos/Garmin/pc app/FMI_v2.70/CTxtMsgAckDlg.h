/*********************************************************************
*
*   HEADER NAME:
*       CTxtMsgAckDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CTxtMsgAckDlg_H
#define CTxtMsgAckDlg_H

#include "stdafx.h"
#include "EventId.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal pop up which displays a text message ack received from
//!     the client.
//! \details Displays the message ID as-is if it is an ASCII string,
//!     or in hexadecimal otherwise.  Both the message ID and response
//!     text are retrieved from the text_message_ack_event_type that is
//!     passed in on the constructor.
//----------------------------------------------------------------------
class CTxtMsgAckDlg : public CDialog
{
    DECLARE_DYNAMIC( CTxtMsgAckDlg )
    DECLARE_MESSAGE_MAP()

public:
    CTxtMsgAckDlg
        (
        CWnd                          * aParent,
        const FmiApplicationLayer     & aCom,
        const text_msg_ack_event_type * aAckEvent
        );
    virtual ~CTxtMsgAckDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();

    //! The reference to the communication controller
    const FmiApplicationLayer & mCom;

    //! The text message ack event that is being displayed
    text_msg_ack_event_type mAckEvent;

    //! Text representation of the text message ID
    CString mMessageId;

    //! Text representation of the response from the client
    CString mResponseText;
};

#endif
