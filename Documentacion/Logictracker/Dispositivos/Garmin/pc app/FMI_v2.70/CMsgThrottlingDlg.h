/*********************************************************************
*
*   HEADER NAME:
*       CMsgThrottlingDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CMsgThrottlingDlg_H
#define CMsgThrottlingDlg_H

#include "fmi.h"

//! Number of throttled protocols that are supported
#define NUM_THROTTLED_PROTOCOLS 11

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to throttle (disable) or
//!     un-throttle (enable) certain client-initiated protocols.
//! \details When this dialog is initialized, the Message Throttling
//!     Query protocol is used to determine the current throttle state
//!     of each supported protocol on the client; the result is used
//!     to set the initial state of each check box.  When the OK button
//!     is clicked, the current state of each check box is compared to
//!     the throttle state received in the Message Throttle Query, and
//!     the Message Throttling Command protocol is used to update the
//!     client state for any check boxes that have changed.
//! \note The Message Throttle Query protocol requires that the client
//!     have support for A605.
//! \since Protocol A604
//----------------------------------------------------------------------
class CMsgThrottlingDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CMsgThrottlingDlg )
    DECLARE_MESSAGE_MAP()

public:
    CMsgThrottlingDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CMsgThrottlingDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange* aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void OnBnClickedBack();
    afx_msg void OnBnClickedUpdate();
    afx_msg void OnBnClickedCheckAll();
#if( FMI_SUPPORT_A605 )
    afx_msg LRESULT OnThrottleQueryResponse
        (
        WPARAM aResponseCount,
        LPARAM aResponseList
        );
#endif

protected:
    //! Reference to the FMI communication controller
    FmiApplicationLayer& mCom;

    //! Lookup table from array indexes to protocol (packet) IDs
    //! \details mPacketIdLookup[i] is the packet id corresponding to
    //!     *(mProtocolStateLookup[i]) and mOriginalValues[i]
    uint16   mPacketIdLookup[NUM_THROTTLED_PROTOCOLS];

    //! Pointers to BOOLs indicating the requested throttle state
    //! \details *(boolLookup[i]) is the requested state for the protocol
    //!     at id_lookup[i].
    BOOL *   mProtocolStateLookup[NUM_THROTTLED_PROTOCOLS];

    //! The current throttle state on the client.
    //! \details mOriginalValues[i] is the current state for the protocol
    //!     at id_lookup[i].
    BOOL     mOriginalValues[NUM_THROTTLED_PROTOCOLS];

    //! If TRUE, the Message Status check box is selected
    BOOL mMessageStatus;

    //! If TRUE, the Refresh Canned Response List check box is selected
    BOOL mRefreshCannedResponseList;

    //! If TRUE, the Refresh Canned Message List check box is selected
    BOOL mRefreshCannedMessageList;

    //! If TRUE, the Client to Server Text Message check box is selected
    BOOL mClientToServerTextMessage;

    //! If TRUE, the Stop Status check box is selected
    BOOL mStopStatus;

    //! If TRUE, the ETA check box is selected
    BOOL mEta;

    //! If TRUE, the Driver ID Update check box is selected
    BOOL mDriverIdUpdate;

    //! If TRUE, the Driver Status List Refresh check box is selected
    BOOL mDriverStatusList;

    //! If TRUE, the Driver Status Update check box is selected
    BOOL mDriverStatusUpdate;

    //! If TRUE, the Client to Server Ping check box is selected
    BOOL mPing;

    //! If TRUE, the Waypoint Deleted check box is selected
    BOOL mWaypointDeleted;

    //! If TRUE, the Select All check box is selected
    BOOL mSelectAll;
};

#endif
