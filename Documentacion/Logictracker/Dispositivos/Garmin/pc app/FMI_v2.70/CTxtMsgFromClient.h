/*********************************************************************
*
*   HEADER NAME:
*       CTxtMsgFromClient.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CTxtMsgFromClient_H
#define CTxtMsgFromClient_H

#include "EventId.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal pop up dialog displaying a client to server open text
//!     message.
//! \details Displays the last client to server open text message in
//!     a dialog box.  The mOriginationTime, message text, and message
//!     ID are retrieved from the event passed to the constructor.
//! \since Protocol A603
//----------------------------------------------------------------------
class CTxtMsgFromClient : public CDialog
{
    DECLARE_DYNAMIC( CTxtMsgFromClient )
    DECLARE_MESSAGE_MAP()

public:
    CTxtMsgFromClient
        (
        CWnd                                  * aParent,
        FmiApplicationLayer                   & aCom,
        const text_msg_from_client_event_type * aEvent
        );

    virtual ~CTxtMsgFromClient();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! The text message event that caused this dialog to be displayed
    text_msg_from_client_event_type  mTextMessageEvent;

    //! String representation of the origination time on the client
    CString mOriginationTime;

    //! String representation of the text message
    CString mMessageText;

    //! String representation of the message ID.
    CString mMessageId;

#if( FMI_SUPPORT_A607 )
    //! String representation of the link ID (corresponding server to client message ID)
    CString mLinkId;
#endif
};

#endif
