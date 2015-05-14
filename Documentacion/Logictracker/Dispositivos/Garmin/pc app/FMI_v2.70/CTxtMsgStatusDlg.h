/*********************************************************************
*
*   HEADER NAME:
*       CTxtMsgStatusDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CTxtMsgStatusDlg_H
#define CTxtMsgStatusDlg_H

class CTxtMsgStatusDlg;

#include "CFmiPcAppDlg.h"

//----------------------------------------------------------------------
//! \brief Modal pop-up dialog displaying the status of a server to
//!     client text message.
//! \details Dialog displaying the text message status received from
//!     the client using the Text Message Status protocol.  This
//!     will be received from the client unsolicited when the status
//!     of a text message changes; if the protocol is throttled the
//!     user will only see this after requesting the status of a
//!     particular text message from the client.
//! \since Protocol A604
//----------------------------------------------------------------------
class CTxtMsgStatusDlg : public CDialog
{
    DECLARE_DYNAMIC( CTxtMsgStatusDlg )
    DECLARE_MESSAGE_MAP()

public:
    CTxtMsgStatusDlg
        (
        CWnd                             * aParent,
        FmiApplicationLayer              & aCom,
        const text_msg_status_event_type * aStatusEvent
        );

    virtual ~CTxtMsgStatusDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();

    //! Reference to the communication layer
    FmiApplicationLayer & mCom;

    //! The text message status event
    text_msg_status_event_type mStatusEvent;

    //! The text message ID.  This will be in hexadecimal if the message
    //! ID is not displayable ASCII
    CString mTextMessageId;

    //! String representation of the text message status ("Unread",
    //! "Not Found", etc)
    CString mTextMessageStatus;
};

#endif
