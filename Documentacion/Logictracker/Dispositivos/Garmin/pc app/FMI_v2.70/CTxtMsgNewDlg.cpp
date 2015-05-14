/*********************************************************************
*
*   MODULE NAME:
*       CTxtMsgNewDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CTxtMsgNewDlg.h"
#include "CSelectCannedResponseDlg.h"

IMPLEMENT_DYNAMIC( CTxtMsgNewDlg, CDialog )

BEGIN_MESSAGE_MAP( CTxtMsgNewDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_CBN_SELCHANGE( IDC_MSGNEW_CBO_PROTOCOL, OnCbnSelChangeMsgProtocol )
    ON_EN_CHANGE( IDC_MSGNEW_EDIT_MSG_ID, OnEnChangeEditFields )
    ON_EN_CHANGE( IDC_MSGNEW_EDIT_MESSAGE_TEXT, OnEnChangeEditFields )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CTxtMsgNewDlg::CTxtMsgNewDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_TXT_MSG_NEW, aParent )
    , mCom( aCom )
    , mMessageText( _T("") )
    , mMessageProtocol( 0 )
#if FMI_SUPPORT_A602
    , mMessageId( _T("") )
#endif
#if FMI_SUPPORT_A604
    , mMessageType( A604_MESSAGE_TYPE_NORMAL )
    , mDisplayImmediately( FALSE )
#endif
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CTxtMsgNewDlg::~CTxtMsgNewDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CTxtMsgNewDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_MSGNEW_EDIT_MESSAGE_TEXT, mMessageText );
    DDX_CBIndex( aDataExchange, IDC_MSGNEW_CBO_PROTOCOL, mMessageProtocol );
#if FMI_SUPPORT_A602
    DDX_Text( aDataExchange, IDC_MSGNEW_EDIT_MSG_ID, mMessageId );
#endif
#if( FMI_SUPPORT_A604 )
    DDX_Check( aDataExchange, IDC_MSGNEW_CHK_DISP_IMMED, mDisplayImmediately );
#endif
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the list of protocols and other dialog fields.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CTxtMsgNewDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CComboBox * comboBox = (CComboBox *)GetDlgItem( IDC_MSGNEW_CBO_PROTOCOL );

// This must match the MessageProtocolType private enum.
#if( FMI_SUPPORT_A604 )
    comboBox->AddString( _T("A604 - Open Text Message") );
    comboBox->AddString( _T("A604 - Canned Response Text Message") );
#endif
#if( FMI_SUPPORT_A602 )
    comboBox->AddString( _T("A602 - Open Text Message") );
    comboBox->AddString( _T("A602 - Simple Okay Acknowledge") );
    comboBox->AddString( _T("A602 - Yes/No Confirmation") );
#endif
#if( FMI_SUPPORT_LEGACY )
    comboBox->AddString( _T("Legacy Text Message") );
#endif

    ASSERT( comboBox->GetCount() == MESSAGE_PROTOCOL_CNT );
    comboBox->SetCurSel( 0 );
    updateDlgFields( 0 );

return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details If the selected protocol is a canned response text
//!     message, display a dialog allowing the user to select the
//!     allowed responses; otherwise, call FmiApplicationLayer to send
//!     the text message using the protocol and details entered by the
//!     user.
//----------------------------------------------------------------------
void CTxtMsgNewDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
    char  messageText[200];

#if( FMI_SUPPORT_A602 )
    MessageId messageId;
#endif

    WideCharToMultiByte( mCom.mClientCodepage, 0, mMessageText.GetBuffer(), -1, messageText, 200, NULL, NULL );
    messageText[199] = '\0';

#if( FMI_SUPPORT_A602 )
    messageId = MessageId( mMessageId, mCom.mClientCodepage );
#endif

#if( FMI_SUPPORT_A604 )
    // if message ID was left blank for a protocol that has a
    // message ID, ask the user to confirm that a blank message
    // ID is really desired; if the user selects No, don't send
    // the message
    if( 0 == messageId.getIdSize() &&
        ( A604_OPEN_MESSAGE_PROTOCOL       == mMessageProtocol ||
          A602_OK_ACK_MESSAGE_PROTOCOL     == mMessageProtocol ||
          A602_YES_NO_ACK_MESSAGE_PROTOCOL == mMessageProtocol ) )
    {
        int response;
        response = MessageBox
                    (
                    _T("The message ID field is empty.  If you continue, message status will not be available for this message.  Do you want to continue?"),
                    _T("Message ID Is Empty"),
                    MB_YESNO
                    );
        if( IDNO == response )
        {
            return;
        }
    }
#endif

#if FMI_SUPPORT_A604
    //set the mMessageType based on the user options
    if( mDisplayImmediately )
        mMessageType = A604_MESSAGE_TYPE_DISP_IMMEDIATE;
    else
        mMessageType = A604_MESSAGE_TYPE_NORMAL;
#endif

    //see which protocol was selected and send appropriately
    switch( mMessageProtocol )
        {
#if( FMI_SUPPORT_A604 )
        case A604_OPEN_MESSAGE_PROTOCOL:    //A604 open
            mCom.sendA604TextMessage( messageText, messageId, mMessageType );
            break;
        case A604_CANNED_RESPONSE_MESSAGE_PROTOCOL:    // Canned Response
            {
            CSelectCannedResponseDlg dlg( messageId, messageText, mMessageType, this, mCom );
            if( dlg.DoModal() == IDCANCEL )
                return;
            }
            break;
#endif
#if( FMI_SUPPORT_A602 )
        case A602_OPEN_MESSAGE_PROTOCOL:    //A602 Open
            mCom.sendA602TextMessage( FMI_ID_SERVER_OPEN_TXT_MSG, messageText, messageId );
            break;
        case A602_OK_ACK_MESSAGE_PROTOCOL:    //Okay Ack
            mCom.sendA602TextMessage( FMI_ID_SERVER_OK_ACK_TXT_MSG, messageText, messageId );
            break;
        case A602_YES_NO_ACK_MESSAGE_PROTOCOL:    //Yes/No Ack
            mCom.sendA602TextMessage( FMI_ID_SERVER_YES_NO_CONFIRM_MSG, messageText, messageId );
            break;
#endif
#if( FMI_SUPPORT_LEGACY )
        case LEGACY_TEXT_MESSAGE_PROTOCOL: // Legacy text message
            mCom.sendLegacyTextMessage( messageText );
            break;
#endif
        default:
            MessageBox
                (
                _T("The message type you have selected is not available."),
                _T("Unsupported"), MB_OK
                );
            return;
            break;
        }

    OnOK();
}    /* OnBnClickedOk() */

//----------------------------------------------------------------------
//! \brief Selection Changed handler for the Protocol combo box
//! \details Enable the text boxes that are valid for the newly
//!     selected protocol, and disable the rest.
//----------------------------------------------------------------------
void CTxtMsgNewDlg::OnCbnSelChangeMsgProtocol()
{
    UpdateData( TRUE );
    updateDlgFields( mMessageProtocol );
}

//----------------------------------------------------------------------
//! \brief Enable/disable controls as appropriate for the selected
//!     protocol.
//! \details Enable the controls that are valid for the newly
//!     selected protocol, and disable the rest.
//! \param aSelectedProtocol The index of the selected protocol in the
//!     combo box.
//----------------------------------------------------------------------
void CTxtMsgNewDlg::updateDlgFields
    (
    int aSelectedProtocol
    )
{
    switch( aSelectedProtocol )
    {
#if( FMI_SUPPORT_A604 )
    case A604_OPEN_MESSAGE_PROTOCOL:
        GetDlgItem( IDC_MSGNEW_CHK_DISP_IMMED )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_LBL_CANRSP_HINT )->ShowWindow( SW_HIDE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID_HINT )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_EDIT_MSG_ID )->EnableWindow( TRUE );
        break;
    case A604_CANNED_RESPONSE_MESSAGE_PROTOCOL:
        GetDlgItem( IDC_MSGNEW_CHK_DISP_IMMED )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_LBL_CANRSP_HINT )->ShowWindow( SW_SHOW );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID_HINT )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_EDIT_MSG_ID )->EnableWindow( TRUE );
        break;
#endif
#if( FMI_SUPPORT_A602 )
    case A602_OPEN_MESSAGE_PROTOCOL:
        GetDlgItem( IDC_MSGNEW_CHK_DISP_IMMED )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_LBL_CANRSP_HINT )->ShowWindow( SW_HIDE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID_HINT )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_EDIT_MSG_ID )->EnableWindow( FALSE );
        break;
    case A602_OK_ACK_MESSAGE_PROTOCOL:
        GetDlgItem( IDC_MSGNEW_CHK_DISP_IMMED )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_LBL_CANRSP_HINT )->ShowWindow( SW_HIDE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID_HINT )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_EDIT_MSG_ID )->EnableWindow( TRUE );
        break;
    case A602_YES_NO_ACK_MESSAGE_PROTOCOL:
        GetDlgItem( IDC_MSGNEW_CHK_DISP_IMMED )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_LBL_CANRSP_HINT )->ShowWindow( SW_HIDE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID_HINT )->EnableWindow( TRUE );
        GetDlgItem( IDC_MSGNEW_EDIT_MSG_ID )->EnableWindow( TRUE );
        break;
#endif
#if( FMI_SUPPORT_LEGACY )
    case LEGACY_TEXT_MESSAGE_PROTOCOL:
        GetDlgItem( IDC_MSGNEW_CHK_DISP_IMMED )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_LBL_CANRSP_HINT )->ShowWindow( SW_HIDE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_LBL_MSG_ID_HINT )->EnableWindow( FALSE );
        GetDlgItem( IDC_MSGNEW_EDIT_MSG_ID )->EnableWindow( FALSE );
        break;
#endif
    }

    OnEnChangeEditFields();

}    /* updateDlgFields() */

//----------------------------------------------------------------------
//! \brief Edit Change handler for all edit boxes on the dialog.
//! \details Validates that all required fields have input. If they do,
//!     enable the OK button; otherwise, disable the OK button.
//! \since Protocol A604
//----------------------------------------------------------------------
void CTxtMsgNewDlg::OnEnChangeEditFields()
{
    BOOL formValid = TRUE;

#if( !( SKIP_VALIDATION ) )
    UpdateData( TRUE );
#if( FMI_SUPPORT_A604 )
    if( mMessageProtocol == A604_CANNED_RESPONSE_MESSAGE_PROTOCOL && mMessageId == "" )
    {
        formValid = FALSE;
    }
#endif
    if( ( mMessageText.GetLength() == 0   ) ||
        ( mMessageText.GetLength() >= 200 ) )
    {
        formValid = FALSE;
    }
#endif

    if( formValid )
    {
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDOK )->EnableWindow( FALSE );
    }

}    /* OnEnChangeEditFields */
