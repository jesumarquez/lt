/*********************************************************************
*
*   HEADER NAME:
*       CSelectCannedResponseDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CSelectCannedResponseDlg_H
#define CSelectCannedResponseDlg_H

#include "stdafx.h"
#include "CFmiPcAppDlg.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to select the canned responses
//!     that are allowed for a particular message.
//! \details Because the protocol cannot be started until the user has
//!     selected the message IDs, all required information to send
//!     the text message is passed from the CNewTxtMsgDlg via the
//!     constructor.  Then, when the user clicks OK in this dialog,
//!     all the information is passed to FmiApplicationLayer, which
//!     sends all of the packets.
//! \note The global list of canned responses is created using the
//!     CManageCannedResponseDlg.
//! \since Protocol A604
//----------------------------------------------------------------------
class CSelectCannedResponseDlg : public CDialog
{
    DECLARE_DYNAMIC( CSelectCannedResponseDlg )
    DECLARE_MESSAGE_MAP()

public:
    CSelectCannedResponseDlg
        (
        const MessageId     & aMessageId,
        char                * aMessageText,
        uint8                 aMessageType,
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CSelectCannedResponseDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );
    BOOL OnInitDialog();
    afx_msg void OnBnClickedOk();

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! \brief List box containing the canned responses.
    CListBox mListBox;

    //! \brief The message type passed in from the CNewTxtMsgDlg
    //! \see a604_message_type for valid values
    uint8    mMessageType;

    //! \brief The message ID passed in from the CNewTxtMsgDlg.
    MessageId mMessageId;

    //! \brief The message text passed in from the CNewTxtMsgDlg.
    char    mMessageText[200];
};

#endif
