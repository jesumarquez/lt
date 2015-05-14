/*********************************************************************
*
*   HEADER NAME:
*      CFmiPcAppDlg.h
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CFmiPcAppDlg_H
#define CFmiPcAppDlg_H

class CFmiPcAppDlg;

#include "FmiApplicationLayer.h"
#include "util.h"
#include "CCommErrorDlg.h"
#include "CLogViewerDlg.h"
#include "CWndEventListener.h"

#include "CStopListDlg.h"

#if FMI_SUPPORT_A604
#include "CTxtMsgStatusDlg.h"
#include "CManageCannedResponseDlg.h"
#include "CCannedTxtMsgDlg.h"
#include "CDriverIdAndStatusDlg.h"
#endif

#if FMI_SUPPORT_A607
#include "CWaypointDlg.h"
#endif

//----------------------------------------------------------------------
//! \brief Main application dialog
//! \details This is the main application dialog; it allows the user
//!     to select any FMI operation to perform.  Displays show the
//!     details of the client device that is connected as well as the
//!     last ETA and PVT information received.
//----------------------------------------------------------------------
class CFmiPcAppDlg : public CDialog, public CWndEventListener
{
    DECLARE_MESSAGE_MAP()

public:
    CFmiPcAppDlg
        (
        CWnd * aParent = NULL
        );
    ~CFmiPcAppDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );
    virtual BOOL OnInitDialog();

    afx_msg void OnBnClickedCheckPVT();
    afx_msg void OnBnClickedChgComPort();
    afx_msg void OnBnClickedEnable();
    afx_msg void OnBnClickedManageStops();
    afx_msg void OnBnClickedOk();
    afx_msg void OnBnClickedTxt();
    afx_msg void OnBnClickedViewlog();

    afx_msg LRESULT OnEventCommTimeout( WPARAM, LPARAM );
    afx_msg LRESULT OnEventCommErrorDlgClosed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventEsnReceived( WPARAM, LPARAM );
    afx_msg LRESULT OnEventLogViewerClosed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventMainDlgInit( WPARAM, LPARAM );
    afx_msg LRESULT OnEventProductIdReceived( WPARAM, LPARAM );
    afx_msg LRESULT OnEventProtocolsReceived( WPARAM, LPARAM );
    afx_msg LRESULT OnEventPvtReceived( WPARAM, LPARAM );

    afx_msg void OnFileClearPacketLog();
    afx_msg void OnFileClose();
    afx_msg void OnHelpAbout();
    afx_msg void OnPaint();
    afx_msg HCURSOR OnQueryDragIcon();
    afx_msg void OnSysCommand
        (
        UINT   nID,
        LPARAM lParam
        );
    afx_msg void OnTimer
        (
        UINT aTimerId
        );
    afx_msg void OnUpdateFileViewlog
        (
        CCmdUI * aCmdUI
        );
    afx_msg void OnViewlog
        (
        BOOL aOpenOther = FALSE
        );

    void doComPortQuestion();
    void doOpenLogFileQuestion();

    CString getProductName
        (
        uint16 aProductId
        );
    void initProductNames();
    void initStopList();
    BOOL selectComPort();

    afx_msg LRESULT OnEventStopListDlgClosed( WPARAM, LPARAM );

#if FMI_SUPPORT_A602
    afx_msg void OnBnClickedSend();
    afx_msg LRESULT OnEventTextMsgAck
        (
        WPARAM,
        LPARAM aEventDataPtr
        );
#endif

#if FMI_SUPPORT_A603
    afx_msg void OnBnClickedAutoETA();
    afx_msg void OnBnClickedChgAutoArvl();
    afx_msg void OnBnClickedDelData();
    afx_msg void OnBnClickedReqETA();

    afx_msg LRESULT OnEventEtaReceived( WPARAM, LPARAM );
    afx_msg LRESULT OnEventStopDone( WPARAM, LPARAM );
    afx_msg LRESULT OnEventTxtMsgFromClient
        (
        WPARAM,
        LPARAM aEventDataPtr
        );
#endif

#if FMI_SUPPORT_A604
    afx_msg void OnBnClickedCannedResponses();
    afx_msg void OnBnClickedIdStatus();
    afx_msg void OnBnClickedGpiFileTrans();
    afx_msg void OnBnClickedGpiQuery();
    afx_msg void OnBnClickedManageCannedMsg();
    afx_msg void OnBnClickedMsgStatus();
    afx_msg void OnBnClickedMsgThrottling();
    afx_msg void OnBnClickedPing();
    afx_msg void OnBnClickedUiTxtChg();

    afx_msg LRESULT OnEventCannedMessageDlgClosed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventCannedResponseDlgClosed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventCannedRespListReceiptError
        (
        WPARAM aResultCode,
        LPARAM
        );
    afx_msg LRESULT OnEventDriverIdAndStatusDlgClosed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventDriverStatusListDeleteFailed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventFmiDisabled( WPARAM, LPARAM );
    afx_msg LRESULT OnEventMsgThrottleFailed( WPARAM, LPARAM );
    afx_msg LRESULT OnEventOpenTextMsgError
        (
        WPARAM,
        LPARAM aEventDataPtr
        );
    afx_msg LRESULT OnEventTxtMsgStatus( WPARAM, LPARAM );
    afx_msg LRESULT OnEventUserInterfaceTextChangeFailed( WPARAM, LPARAM );
#endif
#if (FMI_SUPPORT_A606)
    afx_msg LRESULT OnEventFmiSafeModeError( WPARAM, LPARAM );
    afx_msg void OnBnClickedSafeMode();
#endif
#if FMI_SUPPORT_A607
    afx_msg void OnBnClickedMsgDelete();
    afx_msg void OnBnClickedWaypoints();

    afx_msg LRESULT OnEventDeleteTextMessageStatus
        (
        WPARAM aEventData,
        LPARAM
        );
    afx_msg LRESULT OnEventWaypointDlgClosed( WPARAM, LPARAM );
#endif
#if( FMI_SUPPORT_A608 )
    afx_msg void OnBnClickedSpeedLimit();
#endif

    //! The FmiApplicationLayer object used to communicate with the client
    FmiApplicationLayer mCom;

    //! The log parser to use
    LogParser * mLogParser;

    //! Handle to the dialog's icon
    HICON mIconHandle;

    //! If true, the Enable PVT check box is selected.
    BOOL mPvtChecked;

    //! If TRUE, the user has selected a FmiApplicationLayer port
    BOOL mComPortSelected;
    //! If TRUE, all timers should be ignored
    BOOL mIgnoreTimer;

    //! Contents of the Unit ID (ESN) text box
    CString mUnitId;

    //! Contents of PVT text box: GPS fix
    CString mGpsFix;

    //! Contents of PVT text box: Date
    CString mPvtDate;

    //! Contents of PVT text box: Time
    CString mPvtTime;

    //! Contents of PVT text box: Latitude
    CString mPvtLatitude;

    //! Contents of PVT text box: Longitude
    CString mPvtLongitude;

    //! Contents of PVT text box: Altitude
    CString mAltitude;

    //! Contents of PVT text box: East-West velocity
    CString mEastWestVelocity;

    //! Contents of PVT text box: North-South velocity
    CString mNorthSouthVelocity;

    //! Contents of PVT text box: Up-Down velocity
    CString mUpDownVelocity;

    //! Contents of PVT text box: 2-D velocity
    CString m2DVelocity;

    //! Map of numeric product IDs to displayable names
    std::map<uint16, CString> mProductNames;

    //! Contents of Product ID text box
    CString mProductId;

    //! Contents of Software Version text box
    CString mSoftwareVersion;

    //! Contents of Protocols text box
    CString mSupportedProtocols;

    //! Pointer to the comm error dialog, or NULL if the dialog is not open
    CCommErrorDlg * mCommErrorDlg;

    //! Pointer to the log viewer dialog, or NULL if the dialog is not open
    CLogViewerDlg * mLogViewerDlg;

    //! Pointer to the stop list dialog, or NULL if the dialog is not open
    CStopListDlg * mStopListDlg;

#if FMI_SUPPORT_A602
    //! Contents of the FMI Packet ID edit box
    //! \since Protocol A602
    CString mPacketId;

    //! Contents of the FMI Packet (payload) edit box
    //! \since Protocol A602
    CString mPacketData;
#endif

#if FMI_SUPPORT_A603
    //! If TRUE, the Auto ETA Request text box is checked
    //! \since Protocol A603
    BOOL    mAutoEtaChecked;

    //! If true, mEtaTimer is a valid timer ID.
    //! \since Protocol A603
    bool    mEtaTimerEnabled;

    //! Pointer to the timer ID for the ETA timer (used to disable the timer
    //!     when needed)
    //! \since Protocol A603
    UINT_PTR mEtaTimer;

    //! Contents of the ETA Time text box (arrival time at destination)
    //! \since Protocol A603
    CString mEtaTime;

    //! Contents of the ETA Distance text box (distance to destination)
    //! \since Protocol A603
    CString mEtaDistance;

    //! Contents of the ETA Latitude text box (latitude of destination)
    //! \since Protocol A603
    CString mEtaLatitude;

    //! Contents of the ETA Longitude text box (longitude of destination)
    //! \since Protocol A603
    CString mEtaLongitude;
#endif

#if FMI_SUPPORT_A604
    //! Pointer to the message status dialog, or NULL if no text message
    //! status dialog is displayed
    //! \since Protocol A604
    CTxtMsgStatusDlg * mTxtMsgStatusDlg;

    //! Pointer to the Canned Response dialog, or NULL if no such dialog
    //! is displayed
    //! \since Protocol A604
    CManageCannedResponseDlg * mCannedResponseDlg;

    //! Pointer to the Canned Text Message dialog, or NULL if no such dialog
    //! is displayed
    //! \since Protocol A604
    CCannedTxtMsgDlg * mCannedTxtMsgDlg;

    //! Pointer to the Driver ID and Status dialog, or NULL if no such dialog
    //! is displayed
    //! \since Protocol A604
    CDriverIdAndStatusDlg * mDriverIdAndStatusDlg;
#endif

#if FMI_SUPPORT_A607
    //! Pointer to the Waypoints dialog, or NULL if no such dialog is displayed
    //! \since Protocol A607
    CWaypointDlg * mWaypointDlg;
#endif
};

#endif
