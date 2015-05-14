/*********************************************************************
*
*   MODULE NAME:
*       CMsgThrottlingDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CFmiPcAppDlg.h"
#include "CMsgThrottlingDlg.h"
#include "Event.h"

IMPLEMENT_DYNAMIC( CMsgThrottlingDlg, CDialog )

BEGIN_MESSAGE_MAP( CMsgThrottlingDlg, CDialog )
    ON_BN_CLICKED( IDC_THROTTLE_BTN_CANCEL, OnBnClickedBack )
    ON_BN_CLICKED( IDC_THROTTLE_BTN_UPDATE, OnBnClickedUpdate )
    ON_BN_CLICKED( IDC_THROTTLE_CHK_ALL, OnBnClickedCheckAll )
#if( FMI_SUPPORT_A605 )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_MSG_THROTTLE_QUERY_RESP_RECEIVED ), OnThrottleQueryResponse )
#endif
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CMsgThrottlingDlg::CMsgThrottlingDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_MSG_THROTTLING, aParent )
    , mCom( aCom )
    , mMessageStatus( FALSE )
    , mRefreshCannedResponseList( FALSE )
    , mRefreshCannedMessageList( FALSE )
    , mClientToServerTextMessage( FALSE )
    , mStopStatus( FALSE )
    , mEta( FALSE )
    , mDriverIdUpdate( FALSE )
    , mDriverStatusList( FALSE )
    , mDriverStatusUpdate( FALSE )
    , mPing( FALSE )
    , mWaypointDeleted( FALSE )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CMsgThrottlingDlg::~CMsgThrottlingDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CMsgThrottlingDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_MESSAGE_STATUS, mMessageStatus );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_REFRESH_CAN_RESP, mRefreshCannedResponseList );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_REFRESH_CAN_MSG_LIST, mRefreshCannedMessageList );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_CLIENT_TO_SERVER_OPEN_TXT_MSG, mClientToServerTextMessage );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_STOP_STATUS, mStopStatus );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_ETA, mEta );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_DRIVER_ID_UPDATE, mDriverIdUpdate );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_DRIVER_STATUS_LIST_REFRESH, mDriverStatusList );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_DRIVER_STATUS_UPDATE, mDriverStatusUpdate );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_PING, mPing );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_WAYPOINT_DELETED, mWaypointDeleted );
    DDX_Check( aDataExchange, IDC_THROTTLE_CHK_ALL, mSelectAll );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the lookup table to relate protocols to check boxes,
//!     then initiates the Message Throttling Query Protocol to get the
//!     current throttle status of each protocol.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CMsgThrottlingDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    mPacketIdLookup[0] = FMI_ID_TEXT_MSG_STATUS;
    mPacketIdLookup[1] = FMI_ID_REFRESH_CANNED_RESP_LIST;
    mPacketIdLookup[2] = FMI_ID_REFRESH_CANNED_MSG_LIST;
    mPacketIdLookup[3] = FMI_ID_CLIENT_OPEN_TXT_MSG;
    mPacketIdLookup[4] = FMI_ID_STOP_STATUS;
    mPacketIdLookup[5] = FMI_ID_ETA_DATA;
    mPacketIdLookup[6] = FMI_ID_DRIVER_ID_UPDATE;
    mPacketIdLookup[7] = FMI_ID_DRIVER_STATUS_LIST_REFRESH;
    mPacketIdLookup[8] = FMI_ID_DRIVER_STATUS_UPDATE;
    mPacketIdLookup[9] = FMI_ID_PING;
#if FMI_SUPPORT_A607
    mPacketIdLookup[10] = FMI_ID_WAYPOINT_DELETED;
#endif

    mProtocolStateLookup[0] = &mMessageStatus;
    mProtocolStateLookup[1] = &mRefreshCannedResponseList;
    mProtocolStateLookup[2] = &mRefreshCannedMessageList;
    mProtocolStateLookup[3] = &mClientToServerTextMessage;
    mProtocolStateLookup[4] = &mStopStatus;
    mProtocolStateLookup[5] = &mEta;
    mProtocolStateLookup[6] = &mDriverIdUpdate;
    mProtocolStateLookup[7] = &mDriverStatusList;
    mProtocolStateLookup[8] = &mDriverStatusUpdate;
    mProtocolStateLookup[9] = &mPing;
#if FMI_SUPPORT_A607
    mProtocolStateLookup[10] = &mWaypointDeleted;
#endif

    // set all to false for now
    for( int i = 0; i < NUM_THROTTLED_PROTOCOLS; i++)
    {
        *( mProtocolStateLookup[i] ) = FALSE;
        mOriginalValues[i] = FALSE;
    }

#if( FMI_SUPPORT_A605 )
    // Query the throttling to get the real check values
    mCom.sendMessageThrottlingQuery();

    UpdateData( FALSE );
#endif
    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Click handler for the Back button; closes the dialog
//----------------------------------------------------------------------
void CMsgThrottlingDlg::OnBnClickedBack()
{
    OnCancel();
}    /* OnBnClickedBack() */

//----------------------------------------------------------------------
//! \brief Click handler for the Update button
//! \details Compare the requested message throttling state for each
//!     protocol to the original state received from the client. If
//!     a protocol's requested throttle state has changed, send a
//!     Message Throttling request to the client to set the new state.
//!     Once all requests are ACKed, close the dialog.
//----------------------------------------------------------------------
void CMsgThrottlingDlg::OnBnClickedUpdate()
{
    int i;

    UpdateData( TRUE );

    // Check to see which protocols actually changed
    for( i = 0; i < NUM_THROTTLED_PROTOCOLS; ++i )
    {
        if( *( mProtocolStateLookup[i] ) != mOriginalValues[i] )
        {
            mCom.sendMessageThrottlingUpdate
                    (
                    mPacketIdLookup[i],
                    ( *mProtocolStateLookup[i] ) ? MESSAGE_THROTTLE_STATE_DISABLE : MESSAGE_THROTTLE_STATE_ENABLE
                    );
        }
        mOriginalValues[i] = *mProtocolStateLookup[i];
    }

    OnOK();
}    /* OnBnClickedUpdate() */

//----------------------------------------------------------------------
//! \brief Click handler for the Select All check box
//! \details Select or deselect all boxes on the dialog
//----------------------------------------------------------------------
void CMsgThrottlingDlg::OnBnClickedCheckAll()
{
    UpdateData( TRUE );
    int i;

    for( i = 0; i < NUM_THROTTLED_PROTOCOLS; i++ )
    {
        *mProtocolStateLookup[i] = mSelectAll;
    }

    UpdateData( FALSE );
}

#if( FMI_SUPPORT_A605 )
//----------------------------------------------------------------------
//! \brief Event handler that processes a throttling query response
//!     event
//! \details For each protocol in the list received, find the
//!     corresponding local state value and update it.
//! \param aResponseCount The number of protocols in the query response
//! \param aResponseList Pointer to a message_throttling_data_type
//!     containing the response
//! \return 0, always
//! \since Protocol A605
//----------------------------------------------------------------------
afx_msg LRESULT CMsgThrottlingDlg::OnThrottleQueryResponse
    (
    WPARAM aResponseCount,
    LPARAM aResponseList
    )
{
    message_throttling_data_type * list = (message_throttling_data_type *)aResponseList;
    uint32 listCount = (uint32)aResponseCount;

    UpdateData( TRUE );
    for( uint32 listIdx = 0; listIdx < listCount; ++listIdx )
    {
        for( uint32 localIdx = 0; localIdx < NUM_THROTTLED_PROTOCOLS; ++localIdx )
        {
            if( mPacketIdLookup[localIdx] == list[listIdx].packet_id )
            {
                *mProtocolStateLookup[localIdx] = !( (BOOL) list[listIdx].new_state );
                mOriginalValues[localIdx] = *mProtocolStateLookup[localIdx];
            }
        }
    }
    UpdateData( FALSE );

    return 0;
}
#endif
