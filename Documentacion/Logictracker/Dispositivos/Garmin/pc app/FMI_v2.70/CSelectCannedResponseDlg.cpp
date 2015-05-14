/*********************************************************************
*
*   MODULE NAME:
*       CSelectCannedResponseDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "CFmiApplication.h"
#include "CSelectCannedResponseDlg.h"

using namespace std;

IMPLEMENT_DYNAMIC( CSelectCannedResponseDlg, CDialog )

BEGIN_MESSAGE_MAP( CSelectCannedResponseDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aMessageId The message ID of the message to send
//! \param aMessageText The message text to send; must be
//!     null-terminated
//! \param aMessageType The message_type; see the a604_message_type
//!     enum
//! \param aParent Reference to the parent window
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CSelectCannedResponseDlg::CSelectCannedResponseDlg
    (
    const MessageId     & aMessageId,
    char                * aMessageText,
    uint8                 aMessageType,
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_CANNED_RESPONSE_SELECT, aParent )
    , mCom( aCom )
    , mMessageId( aMessageId )
{
    strcpy( mMessageText, aMessageText );
    mMessageType = aMessageType;
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CSelectCannedResponseDlg::~CSelectCannedResponseDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CSelectCannedResponseDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Control( aDataExchange, IDC_RESPSEL_LST_RESPONSES, mListBox );
}

//----------------------------------------------------------------------
//! \brief This function is called when the window is created.
//! \details Builds the list box of canned responses that the user can
//!     select from.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CSelectCannedResponseDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CString listItem;

    FileBackedMap<ClientListItem>::const_iterator iter = mCom.mCannedResponses.begin();
    for( ; iter != mCom.mCannedResponses.end(); iter++ )
    {
        if( iter->second.isValid() )
        {
            listItem.Format( _T("%d - %s"), iter->first, iter->second.getCurrentName() );
            mListBox.AddString( listItem );
        }
    }
    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief OK button handler
//! \details Extracts data from the form and sends a canned response
//!     text message via FmiApplicationLayer.
//----------------------------------------------------------------------
void CSelectCannedResponseDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
    uint32 * selectedResponseIds = new uint32[ mListBox.GetSelCount() ];
    uint8 selectedIdCount = 0;
    for( int i = 0; i < mListBox.GetCount(); i++ )
    {
        if( mListBox.GetSel( i ) > 0 )
        {
            selectedResponseIds[selectedIdCount++] = mCom.mCannedResponses.getKeyAt( i );
        }
    }

    mCom.sendCannedResponseTextMessage
        (
        mMessageText,
        mMessageId,
        selectedIdCount,
        selectedResponseIds,
        mMessageType
        );

    delete [] selectedResponseIds;
    OnOK();
}    /* OnBnClickedOk() */
