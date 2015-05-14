/*********************************************************************
*
*   MODULE NAME:
*      CTxtMsgStatusDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CTxtMsgStatusDlg.h"

IMPLEMENT_DYNAMIC( CTxtMsgStatusDlg, CDialog )

BEGIN_MESSAGE_MAP( CTxtMsgStatusDlg, CDialog )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//! \param aStatusEvent The text message status received from the
//!     client
//----------------------------------------------------------------------
CTxtMsgStatusDlg::CTxtMsgStatusDlg
    (
    CWnd                             * aParent,
    FmiApplicationLayer              & aCom,
    const text_msg_status_event_type * aStatusEvent
    )
    : CDialog( IDD_TXT_MSG_STATUS, aParent )
    , mCom( aCom )
    , mStatusEvent( *aStatusEvent )
    , mTextMessageId( _T("") )
    , mTextMessageStatus( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CTxtMsgStatusDlg::~CTxtMsgStatusDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CTxtMsgStatusDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_MSGSTATUS_TXT_ID, mTextMessageId );
    DDX_Text( aDataExchange, IDC_MSGSTATUS_TXT_STATUS, mTextMessageStatus );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the text boxes from the text message status event
//!     passed via the constructor.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CTxtMsgStatusDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    mTextMessageId = mStatusEvent.msg_id.toCString( mCom.mClientCodepage );

    switch( mStatusEvent.message_status )
    {
    case MESSAGE_STATUS_UNREAD:
        mTextMessageStatus = _T("unread");
        break;
    case MESSAGE_STATUS_READ:
        mTextMessageStatus = _T("read");
        break;
    case MESSAGE_STATUS_NOT_FOUND:
        mTextMessageStatus = _T("not found");
        break;
    default:
        mTextMessageStatus = _T("invalid");
        break;
    }

    UpdateData( FALSE );
    return TRUE;
}    /* OnInitDialog */
