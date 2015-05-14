/*********************************************************************
*
*   MODULE NAME:
*       CCannedTxtMsgDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CCannedTxtMsgDlg.h"
#include "Event.h"

using namespace std;

IMPLEMENT_DYNAMIC( CCannedTxtMsgDlg, CDialog )

BEGIN_MESSAGE_MAP( CCannedTxtMsgDlg, CDialog )
    ON_BN_CLICKED( IDC_CANMSG_BTN_DELETE, OnBnClickedDelete )
    ON_BN_CLICKED( IDC_CANMSG_BTN_SEND, OnBnClickedSend )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_EN_CHANGE( IDC_CANMSG_EDIT_ID, OnEnChangeEditBoxes )
    ON_EN_CHANGE( IDC_CANMSG_EDIT_TEXT, OnEnChangeEditBoxes )
    ON_LBN_SELCHANGE( IDC_CANMSG_LST_MESSAGES, OnLbnSelchangeMsglist )
    ON_LBN_SETFOCUS( IDC_CANMSG_LST_MESSAGES, OnLbnSetfocusList )
    ON_LBN_KILLFOCUS( IDC_CANMSG_LST_MESSAGES, OnLbnKillfocusMsgList )
    ON_EN_SETFOCUS( IDC_CANMSG_EDIT_TEXT, OnEnSetfocusMessageEdit )
    ON_EN_KILLFOCUS( IDC_CANMSG_EDIT_TEXT, OnEnKillfocusMessageEdit )
    ON_EN_SETFOCUS( IDC_CANMSG_EDIT_ID, OnEnSetfocusMessageEdit )
    ON_EN_KILLFOCUS( IDC_CANMSG_EDIT_ID, OnEnKillfocusMessageEdit )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_CANNED_MSG_LIST_CHANGED ), OnCannedMsgListChanged )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CCannedTxtMsgDlg::CCannedTxtMsgDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_CANNED_TXT_MSG, aParent )
    , mCom( aCom )
    , mMessageId( _T("") )
    , mMessageText( _T("") )
    , mSelectedIndex( 0 )
{
}

//----------------------------------------------------------------------
//! \brief [Brief description of the method]
//! \details [A longer description of the method]
//----------------------------------------------------------------------
CCannedTxtMsgDlg::~CCannedTxtMsgDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Control( aDataExchange, IDC_CANMSG_LST_MESSAGES, mCannedMessageList );
    DDX_Text( aDataExchange, IDC_CANMSG_EDIT_ID, mMessageId );
    DDX_Text( aDataExchange, IDC_CANMSG_EDIT_TEXT, mMessageText );
    DDX_LBIndex( aDataExchange, IDC_CANMSG_LST_MESSAGES, mSelectedIndex );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the response list and sets the window position.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CCannedTxtMsgDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    updateListBox();
    SetWindowPos( NULL, 700, 625, 0, 0, SWP_NOSIZE | SWP_NOZORDER );
    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Update the canned message list box from the canned message
//!     map owned by FmiApplicationLayer.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::updateListBox()
{
    CString listItem;

    //must keep track of where the list was scrolled to
    //since we reset content we must reinitialize these
    int selectedIndex = mCannedMessageList.GetCurSel();
    int topIndex = mCannedMessageList.GetTopIndex();

    //reset content and add current canned messages to list
    mCannedMessageList.ResetContent();
    FileBackedMap<ClientListItem>::const_iterator iter;

    for( iter = mCom.mCannedMessages.begin();
         iter != mCom.mCannedMessages.end();
         iter++ )
    {
        if( iter->second.isValid() )
        {
            listItem.Format( _T("%d - %s"), iter->first, iter->second.getCurrentName() );
            mCannedMessageList.AddString( listItem );
        }
    }
    //reset scroll and selection
    mCannedMessageList.SetCurSel( selectedIndex );
    mCannedMessageList.SetTopIndex( topIndex );
}    /* updateListBox() */

//----------------------------------------------------------------------
//! \brief Handler for the FMI_EVENT_CANNED_MSG_LIST_CHANGED event.
//! \details Updates the list box.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LPARAM CCannedTxtMsgDlg::OnCannedMsgListChanged( WPARAM, LPARAM )
{
    updateListBox();
    return 0;
}    /* OnCannedMsgListChanged() */

//----------------------------------------------------------------------
//! \brief Edit handler for the Message ID and Message Text boxes.
//! \details If either box is empty, disables the Send button; if both
//!     ID and text are specified, enables the Send button.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnEnChangeEditBoxes()
{
    UpdateData( TRUE );
    if( mMessageText != "" && mMessageId != "" )
    {
        GetDlgItem( IDC_CANMSG_BTN_SEND )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_CANMSG_BTN_SEND )->EnableWindow( FALSE );
    }
}    /* OnEnChangeEditBoxes */

//----------------------------------------------------------------------
//! \brief Selection Changed handler for the Message List box.
//! \details Fills in the Message ID and Message Text fields of the
//!     dialog with the information from the selected list item, for
//!     easy editing.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnLbnSelchangeMsglist()
{
    CListBox * messageListBox = (CListBox *)GetDlgItem( IDC_CANMSG_LST_MESSAGES );
    int selectedIndex = messageListBox->GetCurSel();
    if( selectedIndex >= 0 && selectedIndex < messageListBox->GetCount() )
    {
        int index = 0;
        CString str;
        messageListBox->GetText( selectedIndex, str );
        index = str.Find( _T("-"), 0 );
        if( index > 0 )
        {
            TCHAR * itemBuffer = str.GetBuffer();
            TCHAR * messageId = new TCHAR[index];
            memcpy( messageId, itemBuffer, ( index - 1 ) * sizeof( TCHAR ) );
            messageId[index-1] = '\0';
            mMessageId.Format( _T("%s"), messageId );
            int messageTextLength = str.GetLength()-index-1;
            TCHAR * messageText = new TCHAR[messageTextLength];
            memcpy( messageText, itemBuffer + index + 2, ( messageTextLength - 1 ) * sizeof( TCHAR ) );
            messageText[messageTextLength - 1] = '\0';
            mMessageText.Format( _T("%s"), messageText );
            str.ReleaseBuffer();
            delete[] messageId;
            delete[] messageText;
            UpdateData( FALSE );
        }
    }
    messageListBox->SetCurSel( selectedIndex );
}

//----------------------------------------------------------------------
//! \brief Handles the Delete button clicked event.
//! \details Initiates a Delete Canned Message protocol using via the
//!     FmiApplicationLayer.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnBnClickedDelete()
{
    UpdateData( TRUE );
    if( mSelectedIndex >= 0 )
    {
        mCom.sendDeleteCannedMessageRequest( mCom.mCannedMessages.getKeyAt( mSelectedIndex ) );
    }
}    /* OnBnClickedDelete() */

//----------------------------------------------------------------------
//! \brief Button handler for the Send button
//! \details Calls FmiApplicationLayer to initiate the Set Canned
//!     Message protocol, using the data in the Message ID and Message
//!     Text edit boxes.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnBnClickedSend()
{
    UpdateData( TRUE );
    uint32 messageId = _ttoi( mMessageId.GetBuffer() );

    mCom.sendCannedMessage( messageId, mMessageText );
}    /* OnBnClickedSend() */

//----------------------------------------------------------------------
//! \brief Button handler for the OK button
//! \details Closes the window.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnBnClickedOk()
{
    DestroyWindow();
}    /* OnBnClickedOk () */

//----------------------------------------------------------------------
//! \brief Handler for the Cancel action
//! \details Closes the window.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnCancel()
{
    DestroyWindow();
}    /* OnCancel() */

//----------------------------------------------------------------------
//! \brief Called by MFC after the window has been destroyed; performs
//!     final termination activities.
//! \details This dialog is a "monitor", so it is modeless.  When it
//!     gets the destroy message it must re-enable the main button to
//!     open this dialogs (the button is disabled when the dialog is
//!     opened to prevent several of the same type from being created),
//!     and delete itself since the pointer to it is not maintained
//!     by the parent.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::PostNcDestroy()
{
    Event::post( EVENT_FMI_CANNED_MESSAGE_DLG_CLOSED );
    CDialog::PostNcDestroy();
}    /* PostNcDestroy() */

//----------------------------------------------------------------------
//! \brief Handles the set focus event for the Message ID and Message
//!     Text edit boxes.
//! \details Sets the default ID to the Send button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnEnSetfocusMessageEdit()
{
    SendMessage( DM_SETDEFID, IDC_CANMSG_BTN_SEND );
}    /* OnEnSetfocusMessageEdit */

//----------------------------------------------------------------------
//! \brief Handles the kill focus event for the Message ID and Message
//!     Text edit boxes.
//! \details Restores the default ID to the OK button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnEnKillfocusMessageEdit()
{
    SendMessage( DM_SETDEFID, IDOK );
}    /* OnEnKillfocusMessageEdit */

//----------------------------------------------------------------------
//! \brief Handles the set focus event for the Message List box.
//! \details Sets the default ID to the Delete button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnLbnSetfocusList()
{
    SendMessage( DM_SETDEFID, IDC_CANMSG_BTN_DELETE );
}    /* OnLbnSetfocusList */

//----------------------------------------------------------------------
//! \brief Handles the kill focus event for the Message list box.
//! \details Restores the default ID to the OK button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CCannedTxtMsgDlg::OnLbnKillfocusMsgList()
{
    SendMessage( DM_SETDEFID, IDOK );
}    /* OnLbnKillfocusList */
