/*********************************************************************
*
*   MODULE NAME:
*       CPingStatusDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CPingStatusDlg.h"
#include "CFmiPcAppDlg.h"
#include "Event.h"

IMPLEMENT_DYNAMIC( CPingStatusDlg, CDialog )

BEGIN_MESSAGE_MAP( CPingStatusDlg, CDialog )
    ON_BN_CLICKED( IDC_PING_BTN_RESET_CLIENT, OnBnClickedResetClient )
    ON_BN_CLICKED( IDC_PING_BTN_RESET_SERVER, OnBnClickedResetServer )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_PING_RECEIVED ), OnPingEvent )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_PING_RESPONSE_RECEIVED ), OnPingEvent )
    ON_BN_CLICKED( IDC_PING_BTN_SEND, OnBnClickedSendPing )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CPingStatusDlg::CPingStatusDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_PING_STATUS, aParent )
    , mCom( aCom )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CPingStatusDlg::~CPingStatusDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CPingStatusDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

    DDX_Text( aDataExchange, IDC_PING_TXT_SERVER_PINGS, mServerPingCount );
    DDX_Text( aDataExchange, IDC_PING_TXT_SERVER_PING_TS, mServerPingTime );
    DDX_Text( aDataExchange, IDC_PING_TXT_CLIENT_PINGS, mClientPingCount );
    DDX_Text( aDataExchange, IDC_PING_TXT_CLIENT_PING_TS, mClientPingTime );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the text boxes with the current counts and times
//!     from FmiApplicationLayer.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CPingStatusDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    updateTextFields();
    return TRUE;
}    /* OnInitDialog() */


//----------------------------------------------------------------------
//! \brief Click handler for the Reset (Client) button
//! \details Reset the Client to Server Ping count and last ping time.
//----------------------------------------------------------------------
void CPingStatusDlg::OnBnClickedResetClient()
{
    mCom.mClientPingCount = 0;
    updateTextFields();
}    /* OnBnClickedResetClient() */

//----------------------------------------------------------------------
//! \brief Click handler for the Reset (Server) button
//! \details Reset the Server to Client Ping count and last ping time.
//----------------------------------------------------------------------
void CPingStatusDlg::OnBnClickedResetServer()
{
    mCom.mServerPingCount = 0;
    updateTextFields();
}    /* OnBnClickedResetServer() */

//----------------------------------------------------------------------
//! \brief Handler for the Ping event from FmiApplicationLayer
//! \details Update the dialog with the ping counts owned by
//!     FmiApplicationLayer.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CPingStatusDlg::OnPingEvent( WPARAM, LPARAM )
{
    updateTextFields();
    return 0;
}    /* OnPingEvent() */

//----------------------------------------------------------------------
//! \brief Update the text boxes
//! \details Update the text boxes with the ping counts and times that
//!     FmiApplicationLayer has.
//----------------------------------------------------------------------
void CPingStatusDlg::updateTextFields()
{
    mClientPingCount.Format( _T(" %d"), mCom.mClientPingCount );
    if( mCom.mClientPingCount > 0 )
    {
        mClientPingTime.Format( mCom.mLastClientPingTime.Format("%I:%M:%S %p") );
    }
    else
    {
        mClientPingTime.Format( _T("") );
    }

    mServerPingCount.Format( _T(" %d"), mCom.mServerPingCount );
    if( mCom.mServerPingCount > 0 )
    {
        mServerPingTime.Format( mCom.mLastServerPingTime.Format("%I:%M:%S %p") );
    }
    else
    {
        mServerPingTime.Format( _T("") );
    }
    UpdateData( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the Send Ping button
//! \details Initiate the Server to Client Ping protocol.  If a
//!     timeout occurs and the user does not retry, close the dialog.
//----------------------------------------------------------------------
void CPingStatusDlg::OnBnClickedSendPing()
{
    mCom.sendPing();
}    /* OnBnClickedSendPing() */
