/*********************************************************************
*
*   MODULE NAME:
*       CTxtMsgAckDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CTxtMsgAckDlg.h"
#include "CFmiPcAppDlg.h"

IMPLEMENT_DYNAMIC( CTxtMsgAckDlg, CDialog )

BEGIN_MESSAGE_MAP( CTxtMsgAckDlg, CDialog )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//! \param aAckEvent The text message ack that was received
//----------------------------------------------------------------------
CTxtMsgAckDlg::CTxtMsgAckDlg
    (
    CWnd                          * aParent,
    const FmiApplicationLayer     & aCom,
    const text_msg_ack_event_type * aAckEvent
    )
    : CDialog( IDD_TXT_MSG_ACK, aParent )
    , mCom( aCom )
    , mAckEvent( *aAckEvent )
    , mMessageId( _T("") )
    , mResponseText( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CTxtMsgAckDlg::~CTxtMsgAckDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CTxtMsgAckDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_MSGACK_TXT_MSG_ID, mMessageId );
    DDX_Text( aDataExchange, IDC_MSGACK_TXT_RESPONSE, mResponseText );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the text boxes from the ack event passed to the
//!     constructor.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CTxtMsgAckDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    TCHAR  stringBuffer[50];

    mMessageId.Format( _T("                %s"), mAckEvent.message_id.toCString( mCom.mClientCodepage ) );

    MultiByteToWideChar( mCom.mClientCodepage, 0, mAckEvent.ack_text, -1, stringBuffer, 50 );
    stringBuffer[49] = '\0';
    mResponseText.Format( _T("     %s"), stringBuffer );

    UpdateData( FALSE );

    return TRUE;
}    /* OnInitDialog() */
