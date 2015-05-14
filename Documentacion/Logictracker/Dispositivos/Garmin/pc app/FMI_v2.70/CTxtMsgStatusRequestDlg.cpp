/*********************************************************************
*
*   MODULE NAME:
*       CTxtMsgStatusRequestDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CTxtMsgStatusRequestDlg.h"
#include "util.h"

IMPLEMENT_DYNAMIC( CTxtMsgStatusRequestDlg, CDialog )

BEGIN_MESSAGE_MAP( CTxtMsgStatusRequestDlg, CDialog )
    ON_EN_CHANGE( IDC_STATUSREQ_EDIT_MSG_ID, OnEnChangeEditMsgId )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CTxtMsgStatusRequestDlg::CTxtMsgStatusRequestDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_TXT_MSG_STATUS_REQUEST, aParent )
    , mCom( aCom )
    , mMessageId( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CTxtMsgStatusRequestDlg::~CTxtMsgStatusRequestDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CTxtMsgStatusRequestDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_STATUSREQ_EDIT_MSG_ID, mMessageId );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CTxtMsgStatusRequestDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Edit Change handler for Message ID text box
//! \details Enables OK button if the message ID is not empty; disables
//!     OK button otherwise.
//----------------------------------------------------------------------
void CTxtMsgStatusRequestDlg::OnEnChangeEditMsgId()
{
    UpdateData( TRUE );
    if( mMessageId != "" )
    {
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDOK )->EnableWindow( FALSE );
    }
}    /* OnEnChangeEditMsgId() */

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Sends a Text Message Status Request to the client based on
//!     the input entered by the user.
//----------------------------------------------------------------------
void CTxtMsgStatusRequestDlg::OnBnClickedOk()
{
    UpdateData( TRUE );

    mCom.sendTextMessageStatusRequest( MessageId( mMessageId, mCom.mClientCodepage ) );

    OnOK();
}    /* OnBnClickedOk() */
