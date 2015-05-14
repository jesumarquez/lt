/*********************************************************************
*
*   HEADER NAME:
*       CTxtMsgNewDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CTxtMsgNewDlg_H
#define CTxtMsgNewDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to send a text message to the
//!     client.
//! \details This dialog allows the user to send a text message using
//!     any of the supported text message protocols.  The Message ID
//!     and Display Immediately controls are enabled or disabled
//!     depending on the selected protocol.  Most of the text message
//!     protocol are initiated immediately when the Send button is
//!     clicked; the Canned Response Text Message protocol is the
//!     exception because additional information is needed.  If the
//!     user selects this protocol, clicking Send causes a second
//!     dialog to be opened; the protocol is initiated after the user
//!     selects the canned responses.
//----------------------------------------------------------------------
class CTxtMsgNewDlg : public CDialog
{
    DECLARE_DYNAMIC( CTxtMsgNewDlg )
    DECLARE_MESSAGE_MAP()

public:
    CTxtMsgNewDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CTxtMsgNewDlg();

protected:
    //! \brief Typedef for message protocols listed in the combo box
    //! \see MessageProtocolEnum for valid values
    typedef int MessageProtocolType;

    //! \brief Enum for message protocols
    //! \note Must follow the sequence and order in OnInitDialog()
    enum MessageProtocolEnum
    {
#if( FMI_SUPPORT_A604 )
        A604_OPEN_MESSAGE_PROTOCOL,
        A604_CANNED_RESPONSE_MESSAGE_PROTOCOL,
#endif
#if( FMI_SUPPORT_A602 )
        A602_OPEN_MESSAGE_PROTOCOL,
        A602_OK_ACK_MESSAGE_PROTOCOL,
        A602_YES_NO_ACK_MESSAGE_PROTOCOL,
#endif
#if( FMI_SUPPORT_LEGACY )
        LEGACY_TEXT_MESSAGE_PROTOCOL,
#endif

        MESSAGE_PROTOCOL_CNT        // must be last
    };

    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );
    BOOL OnInitDialog();
    afx_msg void OnEnChangeEditFields();
    afx_msg void OnCbnSelChangeMsgProtocol();
    afx_msg void OnBnClickedOk();
    void updateDlgFields
        (
        int aSelectedProtocol
        );

    //! Pointer to the main app dialog
    FmiApplicationLayer & mCom;

    //! Contents of the Message Text edit box
    CString mMessageText;

    //! Index of the selected message protocol
    MessageProtocolType mMessageProtocol;

#if FMI_SUPPORT_A602
    //! Contents of the Message ID edit box
    CString mMessageId;
#endif

#if FMI_SUPPORT_A604
    //! If TRUE, the "Display Immediately" box is checked
    BOOL mDisplayImmediately;

    //! Value to use on the message_type member of the A604 text message
    uint8 mMessageType;
#endif
};

#endif
