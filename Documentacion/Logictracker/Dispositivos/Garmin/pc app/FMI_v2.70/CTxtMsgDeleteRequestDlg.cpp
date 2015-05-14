/*********************************************************************
*
*   MODULE NAME:
*       CTxtMsgDeleteRequestDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CTxtMsgDeleteRequestDlg.h"
#include "util.h"

IMPLEMENT_DYNAMIC( CTxtMsgDeleteRequestDlg, CDialog )

BEGIN_MESSAGE_MAP( CTxtMsgDeleteRequestDlg, CDialog )
    ON_EN_CHANGE( IDC_MSGDEL_EDIT_MSG_ID, OnEnChangeEditMsgId )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CTxtMsgDeleteRequestDlg::CTxtMsgDeleteRequestDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_TXT_MSG_DELETE_REQUEST, aParent )
    , mCom( aCom )
    , mMessageId( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CTxtMsgDeleteRequestDlg::~CTxtMsgDeleteRequestDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CTxtMsgDeleteRequestDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_MSGDEL_EDIT_MSG_ID, mMessageId );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CTxtMsgDeleteRequestDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Edit Change handler for Message ID text box
//! \details Enables OK button if the message ID is not empty; disables
//!     OK button otherwise.
//----------------------------------------------------------------------
void CTxtMsgDeleteRequestDlg::OnEnChangeEditMsgId()
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
//!     the input entered by the user.  If the message ID entered
//!     begins with 0x, the MessageId constructor will interpret it as
//!     hexadecimal.
//----------------------------------------------------------------------
void CTxtMsgDeleteRequestDlg::OnBnClickedOk()
{
    UpdateData( TRUE );

    mCom.sendMessageDeleteRequest( MessageId( mMessageId, mCom.mClientCodepage ) );
    OnOK();
}    /* OnBnClickedOk() */
