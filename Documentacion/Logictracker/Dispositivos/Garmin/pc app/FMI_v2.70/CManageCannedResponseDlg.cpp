/*********************************************************************
*
*   MODULE NAME:
*       CManageCannedResponseDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CManageCannedResponseDlg.h"
#include "Event.h"
using namespace std;

IMPLEMENT_DYNAMIC( CManageCannedResponseDlg, CDialog )

BEGIN_MESSAGE_MAP( CManageCannedResponseDlg, CDialog )
    ON_BN_CLICKED( IDC_CANRESP_BTN_DELETE, OnBnClickedDelete )
    ON_BN_CLICKED( IDC_CANRESP_BTN_SEND, OnBnClickedSend )
    ON_EN_CHANGE( IDC_CANRESP_EDIT_ID, OnEnChangeRspBoxes )
    ON_EN_CHANGE( IDC_CANRESP_EDIT_TEXT, OnEnChangeRspBoxes )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_EN_SETFOCUS( IDC_CANRESP_EDIT_TEXT, OnEnSetfocusResponseEdit )
    ON_EN_KILLFOCUS( IDC_CANRESP_EDIT_TEXT, OnEnKillfocusResponseEdit )
    ON_EN_SETFOCUS( IDC_CANRESP_EDIT_ID, OnEnSetfocusResponseEdit )
    ON_EN_KILLFOCUS( IDC_CANRESP_EDIT_ID, OnEnKillfocusResponseEdit )
    ON_LBN_SELCHANGE( IDC_CANRESP_LST_RESPONSES, OnLbnSelchangeResponselist )
    ON_LBN_SETFOCUS( IDC_CANRESP_LST_RESPONSES, OnLbnSetfocusResponselist )
    ON_LBN_KILLFOCUS( IDC_CANRESP_LST_RESPONSES, OnLbnKillfocusResponselist )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_CANNED_RESP_LIST_CHANGED ), OnCannedRespListChanged )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param  aParent The parent window.
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CManageCannedResponseDlg::CManageCannedResponseDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_CANNED_RESPONSE, aParent )
    , mCom( aCom )
    , mSelectedResponseIndex( 0 )
    , mResponseId( _T("") )
    , mResponseText( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CManageCannedResponseDlg::~CManageCannedResponseDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CManageCannedResponseDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Control( aDataExchange, IDC_CANRESP_LST_RESPONSES, mListBox );
    DDX_LBIndex( aDataExchange, IDC_CANRESP_LBL_RESPONSES, mSelectedResponseIndex );
    DDX_Text( aDataExchange, IDC_CANRESP_EDIT_ID, mResponseId );
    DDX_Text( aDataExchange, IDC_CANRESP_EDIT_TEXT, mResponseText );
}

//----------------------------------------------------------------------
//! \brief This function is called when the window is created.
//! \details This function is called when the window is created. It
//!     initializes the canned response list, and sets the initial
//!     position of the window.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CManageCannedResponseDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    updateListBox();

    SetWindowPos( NULL, 700, 350, 0, 0, SWP_NOSIZE | SWP_NOZORDER );

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Update the canned response list box from the canned response
//!     map owned by FmiApplicationLayer.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::updateListBox()
{
    CString listItem;

    //must keep track of where the list was scrolled to
    //since we reset content we must reinitialize these
    int selectedIndex = mListBox.GetCurSel();
    int topIndex = mListBox.GetTopIndex();

    //reset content and then add current canned responses
    mListBox.ResetContent();
    FileBackedMap<ClientListItem>::const_iterator iter = mCom.mCannedResponses.begin();
    for( ; iter != mCom.mCannedResponses.end(); iter++ )
        {
        if( iter->second.isValid() )
            {
            listItem.Format( _T("%d - %s"), iter->first, iter->second.getCurrentName() );
            mListBox.AddString( listItem );
            }
        }
    //reset scroll and selection
    mListBox.SetCurSel( selectedIndex );
    mListBox.SetTopIndex( topIndex );
}   /* updateListBox() */

//----------------------------------------------------------------------
//! \brief Button handler for the Delete button
//! \details Calls FmiApplicationLayer to initiate the Delete Canned
//!     Response protocol.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnBnClickedDelete()
{
    UpdateData( TRUE );
    if( mSelectedResponseIndex >= 0 )
        {
        mCom.sendDeleteCannedResponseRequest( mCom.mCannedResponses.getKeyAt( mSelectedResponseIndex) );
        }
}   /* OnBnClickedDelete() */

//----------------------------------------------------------------------
//! \brief Handler for FMI_EVENT_CANNED_RESP_LIST_CHANGED event
//! \details Updates the list box
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LPARAM CManageCannedResponseDlg::OnCannedRespListChanged( WPARAM, LPARAM )
{
    updateListBox();
    return 0;
}   /* OnCannedRespListChanged() */

//----------------------------------------------------------------------
//! \brief Button handler for the Send button
//! \details Calls FmiApplicationLayer to initiate the Set Canned
//!     Response protocol, using the data in the Response ID and
//!     Response Text edit boxes.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnBnClickedSend()
{
    UpdateData( TRUE );
    uint32 responseId = _ttoi( mResponseId.GetBuffer() );

    mCom.sendCannedResponse( responseId, mResponseText );
}   /* OnBnClickedSend() */

//----------------------------------------------------------------------
//! \brief Edit handler for the Response ID and Response Text boxes.
//! \details If either box is empty, disables the Send button; if both
//!     ID and text are specified, enables the Send button.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnEnChangeRspBoxes()
{
    UpdateData( TRUE );
    if( mResponseText != "" && mResponseId != "" )
    {
        GetDlgItem( IDC_CANRESP_BTN_SEND )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_CANRESP_BTN_SEND )->EnableWindow( FALSE );
    }
}   /* OnEnChangeRspBoxes() */


//----------------------------------------------------------------------
//! \brief Selection Changed handler for the Response List box.
//! \details Fills in the Response ID and Response Text fields of the
//!     dialog with the information from the selected list item, for
//!     easy editing.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnLbnSelchangeResponselist()
{
    CListBox * responseListBox = (CListBox*) GetDlgItem( IDC_CANRESP_LST_RESPONSES );
    int selectedIndex = responseListBox->GetCurSel();

    if( selectedIndex >= 0 && selectedIndex < responseListBox->GetCount() )
    {
        int index = 0;
        CString itemText;
        responseListBox->GetText( selectedIndex, itemText );
        index = itemText.Find( _T("-"), 0 );
        if( index > 0 )
        {
            TCHAR * itemBuffer = itemText.GetBuffer();

            TCHAR * responseId = new TCHAR[index];
            memcpy( responseId, itemBuffer, ( index - 1 ) * sizeof( TCHAR ) );
            responseId[index-1] = '\0';
            mResponseId.Format( _T("%s"), responseId );

            int responseTextLength = itemText.GetLength()-index-1;
            TCHAR * responseText = new TCHAR[responseTextLength];
            memcpy( responseText, &itemBuffer[index + 2], ( responseTextLength - 1 ) * sizeof( TCHAR ) );
            responseText[responseTextLength - 1] = '\0';
            mResponseText.Format( _T("%s"), responseText );

            itemText.ReleaseBuffer();
            delete[] responseId;
            delete[] responseText;
            UpdateData( FALSE );
        }
    }

    responseListBox->SetCurSel( selectedIndex );
}


//----------------------------------------------------------------------
//! \brief Button handler for the OK button
//! \details Closes the window.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnBnClickedOk()
{
    DestroyWindow();
    //not modal so don't call OnOK()
}   /* OnBnClickedOk */

//----------------------------------------------------------------------
//! \brief Handler for the Cancel action
//! \details Closes the window.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnCancel()
{
    DestroyWindow();
}   /* OnCancel */

//----------------------------------------------------------------------
//! \brief Called by MFC after the window has been destroyed; performs
//!     final termination activities.
//! \details This dialog is a "monitor", so it is modeless.  When it
//!     gets the destroy message it must re-enable the main button to
//!     open this dialog (the button is disabled when the dialog is
//!     opened to prevent more than one from being created)
//----------------------------------------------------------------------
void CManageCannedResponseDlg::PostNcDestroy()
{
    CDialog::PostNcDestroy();
    Event::post( EVENT_FMI_CANNED_RESPONSE_DLG_CLOSED );
}   /* PostNcDestroy() */

//----------------------------------------------------------------------
//! \brief Handles the set focus event for the Response ID and Response
//!     Text edit boxes.
//! \details Sets the default control to the Send button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnEnSetfocusResponseEdit()
{
    SendMessage( DM_SETDEFID, IDC_CANRESP_BTN_SEND );
}   /* OnEnSetfocusResponseEdit */

//----------------------------------------------------------------------
//! \brief Handles the kill focus event for the Response ID and
//!     Response Text edit boxes.
//! \details Sets the default control to the OK button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnEnKillfocusResponseEdit()
{
    SendMessage( DM_SETDEFID, IDOK );
}   /* OnEnKillfocusResponseEdit */

//----------------------------------------------------------------------
//! \brief Handles the set focus event for the Response List.
//! \details Sets the default control to the Delete button so that it
//!     is activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnLbnSetfocusResponselist()
{
    SendMessage( DM_SETDEFID, IDC_CANRESP_BTN_DELETE );
}   /* OnLbnSetfocusResponselist */

//----------------------------------------------------------------------
//! \brief Handles the kill focus event for the Response List box.
//! \details Sets the default control to the OK button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CManageCannedResponseDlg::OnLbnKillfocusResponselist()
{
    SendMessage( DM_SETDEFID, IDOK );
}   /* OnLbnKillfocusResponselist */
