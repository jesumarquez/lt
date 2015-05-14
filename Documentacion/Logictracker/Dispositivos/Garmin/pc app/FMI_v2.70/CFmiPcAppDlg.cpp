/*********************************************************************
*
*   MODULE NAME:
*       CFmiPcAppDlg.cpp
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CAboutDlg.h"
#include "CFmiApplication.h"
#include "CFmiPcAppDlg.h"
#include "CCommErrorDlg.h"
#include "CSelectCommPortDlg.h"
#include "CStopListDlg.h"
#include "CTxtMsgNewDlg.h"
#include "CLogViewerDlg.h"
#include "FmiLogParser.h"

#if( FMI_SUPPORT_A602 )
#include "CTxtMsgAckDlg.h"
#endif

#if( FMI_SUPPORT_A603 )
#include "CAutoArrivalDlg.h"
#include "CDeleteDataDlg.h"
#include "CTxtMsgFromClient.h"
#endif

#if( FMI_SUPPORT_A604 )
#include "CManageCannedResponseDlg.h"
#include "CCannedTxtMsgDlg.h"
#include "CDriverIdAndStatusDlg.h"
#include "CGpiQueryDlg.h"
#include "CGpiTransferDlg.h"
#include "CMsgThrottlingDlg.h"
#include "CPingStatusDlg.h"
#include "CTxtMsgStatusDlg.h"
#include "CTxtMsgStatusRequestDlg.h"
#include "CUITextChangeDlg.h"
#endif

#if( FMI_SUPPORT_A606 )
#include "CSafeModeDlg.h"
#endif

#if( FMI_SUPPORT_A607 )
#include "CFeatureDlg.h"
#include "CTxtMsgDeleteRequestDlg.h"
#endif

#if( FMI_SUPPORT_A608 )
#include "CSpeedLimitAlertsDlg.h"
#endif

#include "Event.h"
#include "Logger.h"
#include "TimerManager.h"

//! RX polling interval, in milliseconds
#define MAIN_RX_TIMER_INTERVAL   ( 150 )

//! Interval between automatic ETA requests, in milliseconds
#define MAIN_ETA_TIMER_INTERVAL  ( 60 * 1000 )

//! Timer ID values used in the application.
//! \see CFmiPcAppDlg::OnTimer() for the handler
//! \see CFmiPcAppDlg::OnInitDialog() for use of the MAIN_TIMER_ID
//! \see CFmiPcAppDlg::OnBnClickedAutoETA() for use of MAIN_ETA_TIMER_ID
enum TimerIdType
{
    MAIN_TIMER_ID,
#if FMI_SUPPORT_A603
    MAIN_ETA_TIMER_ID,
#endif

    TIMER_ID_COUNT
};

using namespace std;

BEGIN_MESSAGE_MAP( CFmiPcAppDlg, CDialog )
    ON_WM_SYSCOMMAND()
    ON_WM_PAINT()
    ON_WM_QUERYDRAGICON()
    ON_WM_TIMER()
    ON_BN_CLICKED( IDC_MAIN_BTN_CHG_COM_PORT, OnBnClickedChgComPort )
    ON_BN_CLICKED( IDC_MAIN_BTN_SEND_ENABLE, OnBnClickedEnable )
    ON_BN_CLICKED( IDC_MAIN_BTN_SEND_TXT_MSG, OnBnClickedTxt )
    ON_BN_CLICKED( IDC_MAIN_BTN_VIEW_LOG, OnBnClickedViewlog )
    ON_BN_CLICKED( IDC_MAIN_BTN_VIEW_STOPS, OnBnClickedManageStops )
    ON_BN_CLICKED( IDC_MAIN_CHK_PVT_ENABLE, OnBnClickedCheckPVT )
    ON_COMMAND( ID_FILE_CLEARPACKETLOG, OnFileClearPacketLog )
    ON_COMMAND( ID_FILE_CLOSE, OnFileClose )
    ON_COMMAND( ID_FILE_VIEWLOG, OnBnClickedViewlog )
    ON_COMMAND( ID_HELP_ABOUT, OnHelpAbout )
    ON_UPDATE_COMMAND_UI( ID_FILE_VIEWLOG, OnUpdateFileViewlog )
    ON_MESSAGE( WM_EVENT( EVENT_MAIN_DLG_INIT ), OnEventMainDlgInit )
    ON_MESSAGE( WM_EVENT( EVENT_COMM_TIMEOUT ), OnEventCommTimeout )
    ON_MESSAGE( WM_EVENT( EVENT_COMM_ERROR_DLG_CLOSED ), OnEventCommErrorDlgClosed )
    ON_MESSAGE( WM_EVENT( EVENT_ESN_RECEIVED ), OnEventEsnReceived )
    ON_MESSAGE( WM_EVENT( EVENT_PRODUCT_ID_RECEIVED ), OnEventProductIdReceived )
    ON_MESSAGE( WM_EVENT( EVENT_PROTOCOLS_RECEIVED ), OnEventProtocolsReceived )
    ON_MESSAGE( WM_EVENT( EVENT_PVT_RECEIVED ), OnEventPvtReceived )
    ON_MESSAGE( WM_EVENT( EVENT_LOG_VIEWER_CLOSED ), OnEventLogViewerClosed )
    ON_MESSAGE( WM_EVENT( EVENT_STOP_LIST_DLG_CLOSED ), OnEventStopListDlgClosed )
#if( FMI_SUPPORT_A602 )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_TXT_MSG_ACK ), OnEventTextMsgAck )
    ON_BN_CLICKED( IDC_MAIN_BTN_FMI_SEND, OnBnClickedSend )
#endif
#if( FMI_SUPPORT_A603 )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_TXT_MSG_FROM_CLIENT ), OnEventTxtMsgFromClient )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_ETA_RECEIVED ), OnEventEtaReceived )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_STOP_STATUS_CHANGED ), OnEventStopDone )
    ON_BN_CLICKED( IDC_MAIN_BTN_CHG_AUTO_ARVL, OnBnClickedChgAutoArvl )
    ON_BN_CLICKED( IDC_MAIN_BTN_DEL_DATA, OnBnClickedDelData )
    ON_BN_CLICKED( IDC_MAIN_BTN_REQ_ETA, OnBnClickedReqETA )
    ON_BN_CLICKED( IDC_MAIN_CHK_ETA_AUTO_UPDATE, OnBnClickedAutoETA )
#endif
#if( FMI_SUPPORT_A604 )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_UI_UPDATE_ERROR ), OnEventUserInterfaceTextChangeFailed )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_CANNED_RESP_LIST_RCPT_ERROR ), OnEventCannedRespListReceiptError )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_A604_TXT_MSG_ERROR ), OnEventOpenTextMsgError )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_TXT_MSG_STATUS_RECEIVED ), OnEventTxtMsgStatus )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_CANNED_MESSAGE_DLG_CLOSED ), OnEventCannedMessageDlgClosed )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DRIVER_ID_AND_STATUS_DLG_CLOSED ), OnEventDriverIdAndStatusDlgClosed )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DRIVER_STATUS_LIST_DELETE_FAILED ), OnEventDriverStatusListDeleteFailed )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_CANNED_RESPONSE_DLG_CLOSED ), OnEventCannedResponseDlgClosed )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DISABLE_COMPLETE ), OnEventFmiDisabled )
    ON_BN_CLICKED( IDC_MAIN_BTN_GPI_FILE_TRANS, OnBnClickedGpiFileTrans )
    ON_BN_CLICKED( IDC_MAIN_BTN_CANNED_RESPONSES, OnBnClickedCannedResponses )
    ON_BN_CLICKED( IDC_MAIN_BTN_PING, OnBnClickedPing )
    ON_BN_CLICKED( IDC_MAIN_BTN_GPI_QUERY, OnBnClickedGpiQuery )
    ON_BN_CLICKED( IDC_MAIN_BTN_DRIVER_INFO, OnBnClickedIdStatus )
    ON_BN_CLICKED( IDC_MAIN_BTN_CANNED_MESSAGES, OnBnClickedManageCannedMsg )
    ON_BN_CLICKED( IDC_MAIN_BTN_MSG_STATUS, OnBnClickedMsgStatus )
    ON_BN_CLICKED( IDC_MAIN_BTN_UI_TXT_CHG, OnBnClickedUiTxtChg )
    ON_BN_CLICKED( IDC_MAIN_BTN_MSG_THROTTLING, OnBnClickedMsgThrottling )
#endif
#if( FMI_SUPPORT_A606 )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_SAFE_MODE_ERROR ), OnEventFmiSafeModeError )
    ON_BN_CLICKED(IDC_MAIN_BTN_SET_SAFE_MODE, OnBnClickedSafeMode)
#endif
#if FMI_SUPPORT_A607
    ON_BN_CLICKED( IDC_MAIN_BTN_MSG_DELETE, OnBnClickedMsgDelete )
    ON_BN_CLICKED( IDC_MAIN_BTN_WAYPOINTS, OnBnClickedWaypoints )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_WAYPOINT_DLG_CLOSED ), OnEventWaypointDlgClosed )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DELETE_TEXT_MESSAGE_STATUS ), OnEventDeleteTextMessageStatus )
#endif
#if( FMI_SUPPORT_A608 )
    ON_BN_CLICKED( IDC_MAIN_BTN_SPEED_LIMIT_ALERTS, OnBnClickedSpeedLimit )
#endif

END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//----------------------------------------------------------------------
CFmiPcAppDlg::CFmiPcAppDlg
    (
    CWnd * aParent
    )
    : CDialog( IDD_FMI_PC_APP_DIALOG, aParent )
    , mPvtChecked( FALSE )
    , mUnitId( _T("") )
    , mGpsFix( _T("") )
    , mPvtDate( _T("") )
    , mPvtTime( _T("") )
    , mPvtLatitude( _T("") )
    , mPvtLongitude( _T("") )
    , mAltitude( _T("") )
    , mEastWestVelocity( _T("") )
    , mNorthSouthVelocity( _T("") )
    , mUpDownVelocity( _T("") )
    , m2DVelocity( _T("") )
    , mCommErrorDlg( NULL )
    , mLogViewerDlg( NULL )
    , mSupportedProtocols( _T("") )
    , mProductId( _T("") )
    , mSoftwareVersion( _T("") )
    , mStopListDlg( NULL )
#if FMI_SUPPORT_A602
    , mPacketId( _T("") )
    , mPacketData( _T("") )
#endif
#if FMI_SUPPORT_A603
    , mAutoEtaChecked( FALSE )
    , mEtaTimerEnabled( FALSE )
    , mEtaTime( _T("") )
    , mEtaDistance( _T("") )
    , mEtaLatitude( _T("") )
    , mEtaLongitude( _T("") )
#endif
#if FMI_SUPPORT_A604
    , mCannedResponseDlg( NULL )
    , mCannedTxtMsgDlg( NULL )
    , mDriverIdAndStatusDlg( NULL )
    , mTxtMsgStatusDlg( NULL )
#endif
#if FMI_SUPPORT_A607
    , mWaypointDlg( NULL )
#endif
{
    mIconHandle = AfxGetApp()->LoadIcon( IDR_MAINFRAME );
    mLogParser = new FmiLogParser;
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CFmiPcAppDlg::~CFmiPcAppDlg()
{
    delete mLogParser;
    delete mCommErrorDlg;
    delete mLogViewerDlg;
    delete mStopListDlg;

#if FMI_SUPPORT_A604
    delete mCannedResponseDlg;
    delete mDriverIdAndStatusDlg;
    delete mCannedTxtMsgDlg;
#endif

#if FMI_SUPPORT_A607
    delete mWaypointDlg;
#endif
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CFmiPcAppDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Check( aDataExchange, IDC_MAIN_CHK_PVT_ENABLE, mPvtChecked );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_ESN, mUnitId );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_GPS_FIX, mGpsFix );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_DATE, mPvtDate );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_TIME, mPvtTime );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_LAT, mPvtLatitude );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_LON, mPvtLongitude );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_ALTITUDE, mAltitude );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_EW_VEL, mEastWestVelocity );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_NS_VEL, mNorthSouthVelocity );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_UD_VEL, mUpDownVelocity );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PVT_2D_VEL, m2DVelocity );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_PRODUCT_ID, mProductId );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_SW_VERSION, mSoftwareVersion );
#if( FMI_SUPPORT_A602 )
    DDX_Text( aDataExchange, IDC_MAIN_EDIT_FMI_ID, mPacketId );
    DDX_Text( aDataExchange, IDC_MAIN_EDIT_FMI_DATA, mPacketData );
#endif
#if( FMI_SUPPORT_A603 )
    DDX_Check( aDataExchange, IDC_MAIN_CHK_ETA_AUTO_UPDATE, mAutoEtaChecked );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_ETA_TIME, mEtaTime );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_ETA_DISTANCE, mEtaDistance );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_ETA_LAT, mEtaLatitude );
    DDX_Text( aDataExchange, IDC_MAIN_TXT_ETA_LON, mEtaLongitude );
#endif
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to Com.  It also registers to receive events from Com, and
//!     initializes the data structures for canned messages, canned
//!     responses, and driver ID and status.  The data structures for
//!     stops are only partially initialized at this point (stop names
//!     and IDs), since the client can update and delete stops when it
//!     is disconnected from the server.  The status of each stop, and
//!     removal of deleted stops, will be initialized on the first
//!     timer cycle which enables FMI and sends update requests for all
//!     stops.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CFmiPcAppDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    // Add "About..." menu item to system menu.
    // IDM_ABOUTBOX must be in the system command range.
    ASSERT( ( IDM_ABOUTBOX & 0xFFF0 ) == IDM_ABOUTBOX );
    ASSERT( IDM_ABOUTBOX < 0xF000 );

    CMenu * pSysMenu = GetSystemMenu( FALSE );
    if( pSysMenu != NULL )
    {
        CString strAboutMenu;
        strAboutMenu.LoadString( IDS_ABOUTBOX );
        if( !strAboutMenu.IsEmpty() )
        {
            pSysMenu->AppendMenu( MF_SEPARATOR );
            pSysMenu->AppendMenu( MF_STRING, IDM_ABOUTBOX, strAboutMenu );
        }
    }

    // Set the icon for this dialog.  The framework does this automatically
    //  when the application's main window is not a dialog
    SetIcon( mIconHandle, TRUE );            // Set big icon
    SetIcon( mIconHandle, FALSE );           // Set small icon

    //used on the first timer cycle to send an enable to device
    //this is done so the app pops up before stalling if there
    //is no device to enable
    mComPortSelected = FALSE;

    mIgnoreTimer = FALSE;

    initProductNames();

    SetTimer( MAIN_TIMER_ID, MAIN_RX_TIMER_INTERVAL, NULL );

    SetWindowPos( NULL, 120, 175, 0, 0, SWP_NOSIZE | SWP_NOZORDER );

#if( !FMI_SUPPORT_A602 )
    CButton * enableButton = (CButton *)GetDlgItem( IDC_MAIN_BTN_SEND_ENABLE );
    enableButton->SetWindowText( _T("Request PID/ESN") );
    GetDlgItem( IDC_MAIN_GRP_FMI_PACKET )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_LBL_FMI_ID )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_LBL_FMI_DATA )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_EDIT_FMI_ID )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_EDIT_FMI_DATA )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_FMI_SEND )->EnableWindow( FALSE );
#endif

#if( !FMI_SUPPORT_A603 )
    GetDlgItem( IDC_MAIN_BTN_DEL_DATA )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_REQ_ETA )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_CHG_AUTO_ARVL )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_CHK_ETA_AUTO_UPDATE )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_GRP_ETA_DATA )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_LBL_ETA_TIME )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_LBL_ETA_DISTANCE )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_LBL_ETA_LAT )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_LBL_ETA_LON )->EnableWindow( FALSE );
#endif

#if( !FMI_SUPPORT_A604 )
    // Disable the controls specific to A604 support
    GetDlgItem( IDC_MAIN_BTN_GPI_FILE_TRANS )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_CANNED_RESPONSES )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_PING )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_GPI_QUERY )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_DRIVER_INFO )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_CANNED_MESSAGES )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_MSG_STATUS )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_UI_TXT_CHG )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_MSG_THROTTLING )->EnableWindow( FALSE );
#endif

#if( !FMI_SUPPORT_A606 )
    // Disable the controls specific to A606 support
    GetDlgItem( IDC_MAIN_BTN_SET_SAFE_MODE )->EnableWindow( FALSE );
#endif

#if( !FMI_SUPPORT_A607 )
    GetDlgItem( IDC_MAIN_BTN_MSG_DELETE )->EnableWindow( FALSE );
    GetDlgItem( IDC_MAIN_BTN_WAYPOINTS )->EnableWindow( FALSE );
#endif

    switch( FMI_PROTOCOL_LEVEL )
    {
    case 0:
        this->SetWindowText( _T("Fleet Management Controller - Legacy Support Mode") );
        break;
    case 602:
        this->SetWindowText( _T("Fleet Management Controller - A602 Support Mode") );
        break;
    case 603:
        this->SetWindowText( _T("Fleet Management Controller - A603 Support Mode") );
        break;
    case 604:
        this->SetWindowText( _T("Fleet Management Controller - A604 Support Mode") );
        break;
    case 605:
        this->SetWindowText( _T("Fleet Management Controller - A605 Support Mode") );
        break;
    case 607:
        this->SetWindowText( _T("Fleet Management Controller - A607 Support Mode") );
        break;
    case 608:
        this->SetWindowText( _T("Fleet Management Controller") );
        break;
    default:
        this->SetWindowText( _T("Fleet Management Controller - Unknown Protocol Support Level") );
        break;
    }

    Event::post( EVENT_MAIN_DLG_INIT );
    return TRUE;  // return TRUE  unless you set the focus to a control
}

//----------------------------------------------------------------------
//! \brief Click handler for the Enable PVT check box
//! \details Send an Enable PVT or Disable PVT command as appropriate
//!     for the new state of the check box.  If disabling PVT, clear
//!     the corresponding text boxes.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedCheckPVT()
{
    UpdateData( TRUE );
    if( mPvtChecked )
    {
        mCom.sendEnablePvtCommand( TRUE );
    }
    else
    {
        mCom.sendEnablePvtCommand( FALSE );

        mGpsFix.Format( _T("") );
        mPvtDate.Format( _T("") );
        mPvtTime.Format( _T("") );
        mPvtLatitude.Format( _T("") );
        mPvtLongitude.Format( _T("") );
        mAltitude.Format( _T("") );
        mNorthSouthVelocity.Format( _T("") );
        mEastWestVelocity.Format( _T("") );
        mUpDownVelocity.Format( _T("") );
        m2DVelocity.Format( _T("") );
        UpdateData( FALSE );
    }
}

//----------------------------------------------------------------------
//! \brief Click handler for the Change Com Port button
//! \details Show the Select Comm Port dialog; if a mCom port was
//!     selected, send the enable.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedChgComPort()
{
    if( selectComPort() )
    {
        OnBnClickedEnable();
    }
}

//----------------------------------------------------------------------
//! \brief Click handler for the Send Enable button.
//! \details Initiates the Enable FMI protocol, requests the ESN,
//!     requests updated status for all A603 stops, and enables PVT
//!     data and the automatic ETA request timer.
//!
//!     Deleting stops normally requires the data structures to be
//!     valid, but they can't possibly be since this function
//!     initializes communication.  These updates will be used to clear
//!     all deleted stops out of the save file and stop id_hex list.
//!     When the user wants to view the stops, another wave of updates
//!     will be requested that will set up their position in the list.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedEnable()
{
#if( FMI_SUPPORT_A607 )
    CFeatureDlg featureDlg( this, mCom );
    if( IDOK != featureDlg.DoModal() )
        {
        return;
        }
#elif( FMI_SUPPORT_A602 )
    mCom.sendEnable();
#endif

#if( !( MINIMAL_ENABLE ) )

#if( FMI_SUPPORT_A602 )
    mCom.sendProductRequest();
#else
    mCom.sendLegacyProductRequest();
#endif

    mCom.sendUnitIdRequest();

#if( FMI_SUPPORT_A603 )
    //every time we enable the client, we must refresh all stops
    //since the client can delete stops and change status without
    //the servers knowledge
    FileBackedMap<StopListItem>::const_iterator iter = mCom.mA603Stops.begin();
    for( ; iter != mCom.mA603Stops.end(); iter++ )
    {
        mCom.sendStopStatusRequest( iter->first, REQUEST_STOP_STATUS );
    }
    mAutoEtaChecked = TRUE;
    OnBnClickedAutoETA();
#endif // FMI_SUPPORT_A603

    mPvtChecked = TRUE;
    UpdateData( FALSE );
    OnBnClickedCheckPVT();

#endif // !( MINIMAL_ENABLE )
}

//----------------------------------------------------------------------
//! \brief Click handler for the Manage Stops button.
//! \details Display the monitor for stops; this is implemented as a
//!     modeless dialog (i.e., does not stop input to the main window.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedManageStops()
{
    //we use a save file to keep track of stops when the client gets disconnected
    //from the server--everything works fine if no stops are deleted; however, we
    //have to assume some are.  This requires 2 waves of updates.  The first wave
    //is sent when FMI is enabled.  All updates are ignored except deleted stop updates.
    //Once all delete stops are removed, we can continue as normal and send a second
    //wave of updates when the user wants to view stops (now)
    if( !mCom.mStopListInitialized )
        initStopList();

    delete mStopListDlg;
    mStopListDlg = new CStopListDlg( this, mCom );
    mStopListDlg->Create( IDD_STOP_LIST );
    mStopListDlg->ShowWindow( SW_SHOW );
    GetDlgItem( IDC_MAIN_BTN_VIEW_STOPS )->EnableWindow( FALSE );
}

#if FMI_SUPPORT_A607
//----------------------------------------------------------------------
//! \brief Click handler for the Waypoints button.
//! \details Display the monitor for waypoints. This is implemented as
//!     a modeless dialog so that it does not stop input to the main
//!     window.
//! \since Protocol A607
//----------------------------------------------------------------------
afx_msg void CFmiPcAppDlg::OnBnClickedWaypoints()
{
    delete mWaypointDlg;
    mWaypointDlg = new CWaypointDlg( this, mCom );
    mWaypointDlg->Create( IDD_WAYPOINT );
    mWaypointDlg->ShowWindow( SW_SHOW );
    GetDlgItem( IDC_MAIN_BTN_WAYPOINTS )->EnableWindow( FALSE );
}
#endif

//----------------------------------------------------------------------
//! \brief Click handler for the OK button.  Close the app.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedOk()
{
    OnOK();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Send Text Message button
//! \details Display a modal dialog allowing the user to send a text
//!     message to the client.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedTxt()
{
    CTxtMsgNewDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the View Packet Log button.
//! \details Display the log viewer; this is implemented as a modeless
//!     dialog (i.e., does not stop input to the main window.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedViewlog()
{
    OnViewlog();
}

//----------------------------------------------------------------------
//! \brief Display the Com Time Out error dialog.
//! \details Display the CommError dialog.  If OK/Retry was clicked,
//!     send an enable (if not in legacy mode) and continue
//!     communication.  If Cancel/Exit was clicked, do nothing more;
//!     the CommErrorDlg may have sent a close message to the app
//!     dialog.
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventCommTimeout( WPARAM, LPARAM )
{
    delete mCommErrorDlg;
    mCommErrorDlg = new CCommErrorDlg( _T("Communication Time Out!  Please check all connections."), TRUE, this, mCom );
    mCommErrorDlg->Create( IDD_ERROR );
    mCommErrorDlg->ShowWindow( SW_SHOW );

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handle the Comm Error Dlg Closed event.
//! \details Delete the dialog.
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventCommErrorDlgClosed( WPARAM, LPARAM )
{
    delete mCommErrorDlg;
    mCommErrorDlg = NULL;

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Unit ID (ESN) Received event from Com.
//! \details Update the Unit ID (ESN) text box.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventEsnReceived( WPARAM, LPARAM )
{
    TCHAR unitId[11];

    UpdateData( TRUE );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mClientUnitId, -1, unitId, 11 );
    unitId[10] = '\0';
    mUnitId.Format( _T(" %s"), unitId );
    UpdateData( FALSE );

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for Log Viewer Closed event
//! \details Re-enable the log viewer button, and delete the log viewer
//!     object.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventLogViewerClosed( WPARAM, LPARAM )
{
    GetDlgItem( IDC_MAIN_BTN_VIEW_LOG )->EnableWindow( TRUE );
    delete mLogViewerDlg;
    mLogViewerDlg = NULL;

    // If a mCom port has not been selected, we are not initialized yet.
    if( !mComPortSelected )
    {
        doComPortQuestion();
    }
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for Main Dialog Init question
//! \details Flow in main dialog is doOpenLogFileQuestion ->
//!     doComPortQuestion -> Run.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventMainDlgInit( WPARAM, LPARAM )
{
    doOpenLogFileQuestion();

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Product ID Received event from Com.
//! \details Update the product name and software version text boxes.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventProductIdReceived( WPARAM, LPARAM )
{
    UpdateData( TRUE );
    mProductId = getProductName( mCom.mClientProductId );
    mSoftwareVersion.Format( _T("%1.2f"), (float)mCom.mClientSoftwareVersion / 100 );
    UpdateData( FALSE );
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Protocols Received event from Com.
//! \details Update the Supported Protocols text box.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventProtocolsReceived( WPARAM, LPARAM )
{
    TCHAR protocols[PROTOCOL_SIZE];
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mProtocols, -1, protocols, PROTOCOL_SIZE );
    protocols[PROTOCOL_SIZE - 1] = '\0';
    mSupportedProtocols.Format( _T("%s"), protocols );
    GetDlgItem( IDC_MAIN_TXT_PROTOCOLS )->SetWindowText( mSupportedProtocols );

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the PVT Data Received event from Com.
//! \details Update the text boxes in the PVT group.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventPvtReceived( WPARAM, LPARAM )
{
    TCHAR str[15];

    UpdateData( TRUE );
    memset( str, 0, sizeof( str ) );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPVTFixType, -1, str, 9 );
    mGpsFix.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtDate, -1, str, 11 );
    mPvtDate.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtTime, -1, str, 13 );
    mPvtTime.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtLatitude, -1, str, 14 );
    mPvtLatitude.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtLongitude, -1, str, 14 );
    mPvtLongitude.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtAltitude, -1, str, 13 );
    mAltitude.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtNorthSouthVelocity, -1, str, 15 );
    mNorthSouthVelocity.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtEastWestVelocity, -1, str, 15 );
    mEastWestVelocity.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mPvtUpDownVelocity, -1, str, 15 );
    mUpDownVelocity.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mHorizontalVelocity, -1, str, 15 );
    m2DVelocity.Format( _T(" %s"), str );
    UpdateData( FALSE );

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for Stop List Dialog Closed event
//! \details Re-enable the button, and delete the dialog object.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventStopListDlgClosed( WPARAM, LPARAM )
{
    GetDlgItem( IDC_MAIN_BTN_VIEW_STOPS )->EnableWindow( TRUE );
    delete mStopListDlg;
    mStopListDlg = NULL;

    return 0;
}

#if FMI_SUPPORT_A607
//----------------------------------------------------------------------
//! \brief Handler for Stop List Dialog Closed event
//! \details Re-enable the button, and delete the dialog object.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventWaypointDlgClosed( WPARAM, LPARAM )
{
    GetDlgItem( IDC_MAIN_BTN_WAYPOINTS )->EnableWindow( TRUE );
    delete mWaypointDlg;
    mWaypointDlg = NULL;

    return 0;
}
#endif

//----------------------------------------------------------------------
//! \brief Handler for the File > Close action.  Close the app.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnFileClose()
{
    OnOK();
}

//----------------------------------------------------------------------
//! \brief Handler for the File > Clear Packet Log action.
//! \details Clear the packet log.  If the Log Viewer is open, close
//!     it beforehand, and open it again afterward.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnFileClearPacketLog()
{
    bool logWasOpen = false;

    if( mLogViewerDlg != NULL )
    {
        logWasOpen = true;
        mLogViewerDlg->DestroyWindow();
    }

    Logger::clearLog();

    if( logWasOpen )
    {
        OnViewlog();
    }
}

//----------------------------------------------------------------------
//! \brief Opens the About Dialog box
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnHelpAbout()
{
    CAboutDlg dlg( this );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Handler for WM_PAINT message.  If the dialog is minimized,
//!     draw the icon centered in the client rectangle, else call the
//!     base class implementation of OnPaint.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnPaint()
{
    if( IsIconic() )
    {
        CPaintDC dc( this ); // device context for painting

        SendMessage( WM_ICONERASEBKGND, reinterpret_cast<WPARAM>( dc.GetSafeHdc() ), 0 );

        // Center icon in client rectangle
        int cxIcon = GetSystemMetrics( SM_CXICON );
        int cyIcon = GetSystemMetrics( SM_CYICON );
        CRect rect;
        GetClientRect( &rect );
        int x = ( rect.Width() - cxIcon + 1 ) / 2;
        int y = ( rect.Height() - cyIcon + 1 ) / 2;

        // Draw the icon
        dc.DrawIcon( x, y, mIconHandle );
    }
    else
    {
        CDialog::OnPaint();
    }
}

//----------------------------------------------------------------------
//! \brief The system calls this function to obtain the cursor to
//!     display while the user drags the minimized window.
//! \return A handle to the cursor
//----------------------------------------------------------------------
HCURSOR CFmiPcAppDlg::OnQueryDragIcon()
{
    return static_cast<HCURSOR>( mIconHandle );
}

//----------------------------------------------------------------------
//! \brief Called when the user selects a command from the Control
//!     menu, or when the user selects the Maximize or Minimize button.
//! \details Display the about box, or delegate the message to the base
//!     class.
//! \param aSystemCommandId Specifies the type of system command
//!     requested.
//! \param aParam Unused, but passed to base class OnSysCommand.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnSysCommand
    (
    UINT   aSystemCommandId,
    LPARAM aParam
    )
{
    if( ( aSystemCommandId & 0xFFF0 ) == IDM_ABOUTBOX )
    {
        CAboutDlg dlgAbout( this );
        dlgAbout.DoModal();
    }
    else
    {
        CDialog::OnSysCommand( aSystemCommandId, aParam );
    }
}

//----------------------------------------------------------------------
//! \brief WM_TIMER event handler.
//! \details The main dialog has two timers: a comm timer which polls
//!     Com frequently to drive receiving of packets, and an ETA timer,
//!     which, if enabled, requests ETA data each mPvtTime it fires.
//!
//!     The comm timer is required because this is a single-threaded
//!     application; if rx were to block, the UI would be unresponsive.
//! \param aTimerId The ID of the timer that was fired.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnTimer
    (
    UINT aTimerId
    )
{
    if( !mIgnoreTimer )
    {
        switch( aTimerId )
        {
        case MAIN_TIMER_ID:
            {
                TimerManager::tick();
            } // end of case MAIN_COMM_TIMER
            break;
#if FMI_SUPPORT_A603
        case MAIN_ETA_TIMER_ID:
            {
                mCom.sendEtaRequest();
            }
            break;
#endif
        } //end of switch( nIdEvent )
    } // end of if(!mIgnoreTimer...)
} // end of CFmiPcAppDlg::OnTimer()

//----------------------------------------------------------------------
//! \brief ON_UPDATE_COMMAND_UI handler for File > View Log menu item.
//! \details Called by MFC just before the File menu is displayed to
//!     determine whether View Log should be enabled or disabled.
//!     File > View Log is given the same state as the View Log button:
//!     enabled when there is no log viewer visible, and disabled when
//!     the log viewer is shown.
//! \param aCmdUI Pointer to the CCmdUI object representing the File >
//!     View Log menu item.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnUpdateFileViewlog
    (
    CCmdUI * aCmdUI
    )
{
    if( mLogViewerDlg != NULL )
    {
        aCmdUI->Enable( FALSE );
    }
    else
    {
        aCmdUI->Enable( TRUE );
    }
}

//----------------------------------------------------------------------
//! \brief Display the log viewer.
//! \details Display the log viewer; this is implemented as a modeless
//!     dialog (i.e., does not stop input to the main window).
//! \param aOpenOther If TRUE, the log viewer will start by prompting
//!     the user to open an existing packet log.
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnViewlog
    (
    BOOL aOpenOther
    )
{
    delete mLogViewerDlg;

    mLogViewerDlg = new CLogViewerDlg( mLogParser, this, aOpenOther );
    mLogViewerDlg->Create( IDD_LOG );
    mLogViewerDlg->ShowWindow( SW_SHOW );

    //disable button so multiple dialogs cannot be opened
    GetDlgItem( IDC_MAIN_BTN_VIEW_LOG )->EnableWindow( FALSE );
}

//----------------------------------------------------------------------
//! \brief Translate a product ID into a display name.
//! \details Display an error message pop up.
//! \param aProductId A numeric product ID returned in the Product ID
//!     and Support protocol.
//! \return A string representation of the product name
//! \since Protocol A604
//! \note The only product IDs that are translated are those that are
//!     known as of the date this app was published; the list is not
//!     guaranteed to be exhaustive.  If this function returns Unknown
//!     as the product name, add the product ID and corresponding name
//!     to the list in initProductNames.
//----------------------------------------------------------------------
CString CFmiPcAppDlg::getProductName
    (
    uint16 aProductId
    )
{
    CString result;
    map<uint16, CString>::iterator iter = mProductNames.find( aProductId );
    if( iter == mProductNames.end() )
    {
        result.Format( _T("Unknown (%u)"), aProductId );
    }
    else
    {
        result = iter->second;
    }

    return result;
}

//----------------------------------------------------------------------
//! \brief Initialize the list of product names
//----------------------------------------------------------------------
void CFmiPcAppDlg::initProductNames()
{
    mProductNames.clear();
    mProductNames[ 404 ] = CString("StreetPilot 2720");
    mProductNames[ 412 ] = CString("StreetPilot 7000 Series");
    mProductNames[ 481 ] = CString("StreetPilot c340");
    mProductNames[ 520 ] = CString("StreetPilot 2820");
    mProductNames[ 539 ] = CString("StreetPilot c500 Series");
    mProductNames[ 566 ] = CString("nuvi 310/360/370");
    mProductNames[ 580 ] = CString("zumo 500 Series");
    mProductNames[ 596 ] = CString("nuvi 600 Series");
    mProductNames[ 640 ] = CString("nuvi 300/350 Chinese");
    mProductNames[ 641 ] = CString("nuvi 300/350 Japanese");
    mProductNames[ 642 ] = CString("nuvi 300/350 Thai");
    mProductNames[ 643 ] = CString("nuvi 310/360 Chinese");
    mProductNames[ 644 ] = CString("nuvi 310/360 Japanese");
    mProductNames[ 645 ] = CString("nuvi 310/360 Thai");
    mProductNames[ 655 ] = CString("nuvi 310/360 Taiwanese");
    mProductNames[ 656 ] = CString("nuvi 310/360 Russian");
    mProductNames[ 657 ] = CString("nuvi 310/360 Arabic");
    mProductNames[ 706 ] = CString("zumo 500 Series Taiwanese");
    mProductNames[ 722 ] = CString("zumo 500 Series Japanese");
    mProductNames[ 723 ] = CString("nuvi 500 Series");
    mProductNames[ 726 ] = CString("nuvi 800 Series");
    mProductNames[ 743 ] = CString("nuvi 5000 Series");
    mProductNames[ 754 ] = CString("nuvi 700 Series");
    mProductNames[ 827 ] = CString("nuvi 205W Series");
    mProductNames[ 836 ] = CString("nuvi 700 Series Taiwanese");
    mProductNames[ 844 ] = CString("nuvi 700 Series Chinese");
    mProductNames[ 851 ] = CString("nuvi 205 Series");
    mProductNames[ 855 ] = CString("nuvi 300/350");
    mProductNames[ 856 ] = CString("nuvi 310/360/370");
    mProductNames[ 870 ] = CString("nuvi 705 Series");
    mProductNames[ 905 ] = CString("nuvi 700 Series Sing/Malay");
    mProductNames[ 906 ] = CString("nuvi 700 Series Thai");
    mProductNames[ 925 ] = CString("nuvi 700 Series Indonesian");
    mProductNames[ 926 ] = CString("nuvi 205 Series Indonesian");
    mProductNames[ 927 ] = CString("nuvi 205 Series Sing/Malay");
    mProductNames[ 928 ] = CString("nuvi 205 Series Chinese");
    mProductNames[ 929 ] = CString("nuvi 205W Series Indonesian");
    mProductNames[ 930 ] = CString("nuvi 205W Series Sing/Malay");
    mProductNames[ 931 ] = CString("nuvi 205W Series Chinese");
    mProductNames[ 932 ] = CString("nuvi 205W Series Taiwanese");
    mProductNames[ 933 ] = CString("nuvi 205W Series Japanese");
    mProductNames[ 943 ] = CString("nuvi 465T");
    mProductNames[ 958 ] = CString("nuvi 5000 Taiwanese");
    mProductNames[ 959 ] = CString("nuvi 5000 Chinese");
    mProductNames[ 971 ] = CString("nuvi 1200 Series");
    mProductNames[ 972 ] = CString("nuvi 1300/1400 Series");
    mProductNames[ 1001 ] = CString("nuvi 205W Series Thai");
    mProductNames[ 1002 ] = CString("nuvi 205 Series India");
    mProductNames[ 1007 ] = CString("nuvi 705 Series Taiwanese");
    mProductNames[ 1074 ] = CString("nuvi 205 Series MT");
    mProductNames[ 1077 ] = CString("nuvi 1480 Series Japanese");
    mProductNames[ 1091 ] = CString("nuvi 205W Series MT");
    mProductNames[ 1104 ] = CString("nuvi 1300 Series MT");
    mProductNames[ 1106 ] = CString("nuvi 1100/1200 Series MT");
    mProductNames[ 1186 ] = CString("nuvi 2200 Series");
    mProductNames[ 1187 ] = CString("nuvi 2300 Series");
    mProductNames[ 1269 ] = CString("dezl 560 Series");
    mProductNames[ 1273 ] = CString("nuvi 2400 Series");
}

//----------------------------------------------------------------------
//! \brief Initialize the stop list.
//! \details Send the second wave of update stop requests that actually
//!     initialize the index in the stop list.
//! \since Protocol A603
//----------------------------------------------------------------------
void CFmiPcAppDlg::initStopList()
{
    mCom.mStopListInitialized = TRUE;

#if( FMI_SUPPORT_A603 )
    FileBackedMap<StopListItem>::const_iterator iter;
    for( iter = mCom.mA603Stops.begin(); iter != mCom.mA603Stops.end(); iter++ )
    {
        mCom.sendStopStatusRequest( iter->first, REQUEST_STOP_STATUS );
    }
#endif
}

//----------------------------------------------------------------------
//! \brief Show the Select Comm Port dialog.
//! \return TRUE if OK was selected, FALSE otherwise.
//----------------------------------------------------------------------
BOOL CFmiPcAppDlg::selectComPort()
{
    CSelectCommPortDlg dlg( this );
    if( dlg.DoModal() == IDOK )
        return TRUE;
    else
        return FALSE;
}

//----------------------------------------------------------------------
//! \brief Ask the user whether to open a log file.
//! \details Display a message box with the question; if the user
//!    selects Yes open the log viewer, else ask the user to select a
//!    mCom port.
//----------------------------------------------------------------------
void CFmiPcAppDlg::doOpenLogFileQuestion()
{
    if( MessageBox( _T("Would you like to open a log file without connecting to a unit first?"), _T("Question?"), MB_YESNO ) == IDYES )
    {
        OnViewlog( TRUE );
        mIgnoreTimer = FALSE;
    }
    else
    {
        doComPortQuestion();
    }
}

//----------------------------------------------------------------------
//! \brief Ask the user to select a mCom port.
//! \details Call selectComPort, if a port was selected send the FMI
//!    enable and start processing timer events.  If the user canceled
//!    the Select Com Port dialog, close the app.
//----------------------------------------------------------------------
void CFmiPcAppDlg::doComPortQuestion()
{
    mIgnoreTimer = TRUE;
    if( selectComPort() )
    {
        OnBnClickedEnable();
        mComPortSelected = TRUE;
        mIgnoreTimer = FALSE;
    }
    else
    {
        AfxGetMainWnd()->SendMessage( WM_CLOSE );
    }
}

#if( FMI_SUPPORT_A602 )
//----------------------------------------------------------------------
//! \brief Click handler for the Send (FMI Packet) button
//! \details Sends an FMI packet containing the FMI packet ID and FMI
//!     payload entered by the user in the corresponding edit boxes.
//! \warning This is intended for debugging purposes only.  The user is
//!     responsible for assuring that the packet is well-formed.  The
//!     app will respond to any packets sent by the client.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedSend()
{
    char   fmiPayloadHex[MAX_PAYLOAD_SIZE * 2];
    uint8  fmiPayload[MAX_PAYLOAD_SIZE];
    uint8  fmiPayloadSize = 0;
    char   fmiPacketIdHex[6];
    uint16 fmiPacketId = 0;

    //get data
    UpdateData( TRUE );
    WideCharToMultiByte( mCom.mClientCodepage, 0, mPacketData, -1, fmiPayloadHex, sizeof( fmiPayloadHex ), NULL, NULL );
    WideCharToMultiByte( mCom.mClientCodepage, 0, mPacketId, -1, fmiPacketIdHex, sizeof( fmiPacketIdHex ), NULL, NULL );
    fmiPayloadHex[MAX_PAYLOAD_SIZE * 2 - 1] = '\0';
    fmiPayload[0] = '\0';

    if( !strncmp( fmiPacketIdHex, "0x", 2 ) )
        fmiPayloadSize = UTIL_hex_to_uint16( fmiPacketIdHex + 2, &fmiPacketId, 1 );
    else
        fmiPayloadSize = UTIL_hex_to_uint16( fmiPacketIdHex, &fmiPacketId, 1 );
    if( fmiPayloadSize == 0 )
    {
        MessageBox( _T(" Invalid Hex String in ID field!"), _T("Error!") );
        return;
    }

    fmiPayloadSize = 0;
    if( mPacketData != "" )
    {
        if( !strncmp( fmiPayloadHex, "0x", 2 ) )
        {
            fmiPayloadSize = (uint8)UTIL_hex_to_uint8( fmiPayloadHex + 2, (uint8*)fmiPayload, MAX_PAYLOAD_SIZE );
        }
        else
        {
            fmiPayloadSize = (uint8)UTIL_hex_to_uint8( fmiPayloadHex, (uint8*)fmiPayload, MAX_PAYLOAD_SIZE );
        }
        if( fmiPayloadSize == 0 )
        {
            MessageBox( _T(" Invalid Hex String in Data Field!"), _T("Error!") );
            return;
        }
    }

    mCom.sendFmiPacket( fmiPacketId, fmiPayload, fmiPayloadSize );
}

//----------------------------------------------------------------------
//! \brief Handler for the Text Message Ack Received event from Com.
//! \details Show a pop up containing the ack.
//! \param aEventDataPtr The ack text.
//! \return 0, always
//! \since Protocol A602
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventTextMsgAck
    (
    WPARAM,
    LPARAM aEventDataPtr
    )
{
    const text_msg_ack_event_type * ackEvent = (const text_msg_ack_event_type *)aEventDataPtr;
    CTxtMsgAckDlg dlg( this, mCom, ackEvent );
    dlg.DoModal();

    delete ackEvent;
    return 0;
}
#endif

#if( FMI_SUPPORT_A603 )
//----------------------------------------------------------------------
//! \brief Event handler for Auto ETA Request check box.
//! \details Set or kill the MAIN_ETA_TIMER_ID as appropriate.
//----------------------------------------------------------------------
afx_msg void CFmiPcAppDlg::OnBnClickedAutoETA()
{
    UpdateData( TRUE );
    if( mAutoEtaChecked )
    {
        if( !mEtaTimerEnabled )
        {
            mEtaTimer = SetTimer( MAIN_ETA_TIMER_ID, MAIN_ETA_TIMER_INTERVAL, NULL );
            if( mEtaTimer )
            {
                mEtaTimerEnabled = true;
            }
            else
            {
                ::MessageBox( m_hWnd, _T("Failed to set periodic ETA timer!"), _T("Error"), MB_OK );
                mAutoEtaChecked = FALSE;
                UpdateData( FALSE );
            }
        }
    }
    else
    {
        if( mEtaTimerEnabled )
        {
            KillTimer( mEtaTimer );
            mEtaTimerEnabled = false;
        }
    }
}

//----------------------------------------------------------------------
//! \brief Click handler for the Change Auto-Arrival button
//! \details Display a modal dialog allowing the user to edit the
//!     auto-arrival options.
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg void CFmiPcAppDlg::OnBnClickedChgAutoArvl()
{
    CAutoArrivalDlg dlg( this, mCom );
    dlg.DoModal();
}    /* OnBnClickedChgAutoArvl() */

//----------------------------------------------------------------------
//! \brief Click handler for the Delete Data button
//! \details Display a modal dialog allowing the user to delete FMI
//!     data on the client.
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg void CFmiPcAppDlg::OnBnClickedDelData()
{
    CDeleteDataDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Request ETA button
//! \details Initiate an ETA Request protocol.  The results will be
//!     displayed in the ETA group of text boxes when the ETA is
//!     received from the client.
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg void CFmiPcAppDlg::OnBnClickedReqETA()
{
    mCom.sendEtaRequest();
}    /* OnBnClickedReqETA() */

//----------------------------------------------------------------------
//! \brief Handler for the ETA Received event from Com.
//! \details Update the ETA group of text boxes.
//! \return 0, always
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventEtaReceived( WPARAM, LPARAM )
{
    //eta data will be transmitted when an active route starts
    //On start up, the server requests the eta to make sure there
    //wasn't one active already
    TCHAR str[14];
    UpdateData( TRUE );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mEtaTime, -1, str, 13 );
    str[12] = '\0';
    mEtaTime.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mEtaLatitude, -1, str, 14 );
    str[13] = '\0';
    mEtaLatitude.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mEtaLongitude, -1, str, 14 );
    str[13] = '\0';
    mEtaLongitude.Format( _T(" %s"), str );
    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mEtaDistance, -1, str, 14 );
    str[13] = '\0';
    mEtaDistance.Format( _T(" %s"), str );
    UpdateData( FALSE );
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Stop Done event from Com.
//! \details If there is no active route, clear the ETA text boxes.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventStopDone( WPARAM, LPARAM )
{
    if( !mCom.mActiveRoute )
    {
        UpdateData( TRUE );
        mEtaTime.Format( _T("") );
        mEtaLatitude.Format( _T("") );
        mEtaLongitude.Format( _T("") );
        mEtaDistance.Format( _T("") );
        UpdateData( FALSE );
    }
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Text Message Received event from Com.
//! \details Show a pop up containing the message from the client.
//! \param aEventDataPtr The text message from the client.
//! \return 0, always
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventTxtMsgFromClient
    (
    WPARAM,
    LPARAM aEventDataPtr
    )
{
    text_msg_from_client_event_type * messageEvent = (text_msg_from_client_event_type *)aEventDataPtr;

    CTxtMsgFromClient dlg( this, mCom, messageEvent );

    dlg.DoModal();
    delete messageEvent;

    return 0;
}
#endif

#if( FMI_SUPPORT_A604 )
//----------------------------------------------------------------------
//! \brief Click handler for the Canned Responses button.
//! \details Display the monitor for canned responses; this is
//!     implemented as a modeless dialog (i.e., does not stop input to
//!     the main window.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedCannedResponses()
{
    delete mCannedResponseDlg;
    mCannedResponseDlg = new CManageCannedResponseDlg( this, mCom );
    mCannedResponseDlg->Create( IDD_CANNED_RESPONSE );
    mCannedResponseDlg->ShowWindow( SW_SHOW );

    GetDlgItem( IDC_MAIN_BTN_CANNED_RESPONSES )->EnableWindow( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the GPI Transfer File button
//! \details Display a modal dialog allowing the user to select and
//!     transfer a GPI file to the client.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedGpiFileTrans()
{
    CGpiTransferDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the GPI Query button
//! \details Display a modal dialog allowing the user to display the
//!     information associated with the FMI GPI file on the client.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedGpiQuery()
{
    CGpiQueryDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Driver ID and Status button.
//! \details Display the monitor for Driver ID and Status; this is
//!     implemented as a modeless dialog (i.e., does not stop input to
//!     the main window.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedIdStatus()
{
    delete mDriverIdAndStatusDlg;

    mDriverIdAndStatusDlg = new CDriverIdAndStatusDlg( this, mCom );
    mDriverIdAndStatusDlg->Create( IDD_DRIVER_ID_STATUS );
    mDriverIdAndStatusDlg->ShowWindow( SW_SHOW );

    GetDlgItem( IDC_MAIN_BTN_DRIVER_INFO )->EnableWindow( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the Canned Messages button.
//! \details Display the monitor for canned messages; this is
//!     implemented as a modeless dialog (i.e., does not stop input to
//!     the main window.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedManageCannedMsg()
{
    delete mCannedTxtMsgDlg;

    mCannedTxtMsgDlg = new CCannedTxtMsgDlg( this, mCom );
    mCannedTxtMsgDlg->Create( IDD_CANNED_TXT_MSG );
    mCannedTxtMsgDlg->ShowWindow( SW_SHOW );

    GetDlgItem( IDC_MAIN_BTN_CANNED_MESSAGES )->EnableWindow( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the Message Status button
//! \details Display a modal dialog allowing the user to request
//!     message status for a particular message.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedMsgStatus()
{
    CTxtMsgStatusRequestDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Message Throttling button
//! \details Display a modal dialog allowing the user to change which
//!     protocols are throttled (disabled unless initiated by the
//!     server).
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedMsgThrottling()
{
    CMsgThrottlingDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Ping button
//! \details Display a modal dialog allowing the user to view and reset
//!     ping counts and last ping times.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedPing()
{
    CPingStatusDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Change UI Text button
//! \details Display a modal dialog allowing the user to change the
//!     text of certain UI elements on the client.
//! \since Protocol A604
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedUiTxtChg()
{
    CUITextChangeDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Handler for Canned Text Message Dialog Closed event
//! \details Re-enable the button, and delete the dialog object.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventCannedMessageDlgClosed( WPARAM, LPARAM )
{
    GetDlgItem( IDC_MAIN_BTN_CANNED_MESSAGES )->EnableWindow( TRUE );
    delete mCannedTxtMsgDlg;
    mCannedTxtMsgDlg = NULL;

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Canned Response List Rcpt Error event.
//! \details Show a pop up containing the error.
//! \param aResultCode The result code sent by the client
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventCannedRespListReceiptError
    (
    WPARAM aResultCode,
    LPARAM
    )
{
    if( aResultCode == CANNED_RESP_LIST_INVALID_COUNT )
    {
        AfxGetMainWnd()->MessageBox
            (
            _T("Invalid Response Count! Retry with a Canned Response List packet with 1-50 Response IDs."),
            _T("Canned Response Text Message Failure")
            );
    }
    else if( aResultCode == CANNED_RESP_LIST_INVALID_MSG_ID )
    {
        AfxGetMainWnd()->MessageBox
            (
            _T("Invalid Response ID! Retry after ensuring all canned responses are on the client."),
            _T("Canned Response Text Message Failure")
            );
    }
    else if( aResultCode == CANNED_RESP_LIST_DUPLICATE_MSG_ID )
    {
        AfxGetMainWnd()->MessageBox
            (
            _T("Duplicate Message ID! Retry using a message ID that is not on the client."),
            _T("Canned Response Text Message Failure")
            );
    }
    else if( aResultCode == CANNED_RESP_LIST_FULL )
    {
        AfxGetMainWnd()->MessageBox
            (
            _T("Canned Response List Database Full! Retry after receiving an acknowledgment for a previous message."),
            _T("Canned Response Text Message Failure")
            );
    }
    else
    {
        AfxGetMainWnd()->MessageBox
            (
            _T("Unknown error.  Please retry."),
            _T("Canned Response Text Message Failure")
            );
    }

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for Canned Response Dialog Closed event
//! \details Re-enable the button, and delete the dialog object.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventCannedResponseDlgClosed( WPARAM, LPARAM )
{
    GetDlgItem( IDC_MAIN_BTN_CANNED_RESPONSES )->EnableWindow( TRUE );
    delete mCannedResponseDlg;
    mCannedResponseDlg = NULL;

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for Driver ID and Status Dialog Closed event
//! \details Re-enable the button, and delete the dialog object.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventDriverIdAndStatusDlgClosed( WPARAM, LPARAM )
{
    GetDlgItem( IDC_MAIN_BTN_DRIVER_INFO )->EnableWindow( TRUE );
    delete mDriverIdAndStatusDlg;
    mDriverIdAndStatusDlg = NULL;

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Delete Driver Status Failed event from Com.
//! \details Show a pop up containing the error.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventDriverStatusListDeleteFailed( WPARAM, LPARAM )
{
    AfxGetMainWnd()->MessageBox
        (
        _T("Unable to delete driver status item."),
        _T("Delete Driver Status Error")
        );
    return 0;
}

//----------------------------------------------------------------------
//! \brief Set FMI as disabled
//! \details Once FMI is disabled, the app should not try to do
//!    anything else with the client.  This flag is checked once the
//!    Delete Data dialog is closed, and is used to cause the app to
//!    exit.
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventFmiDisabled( WPARAM, LPARAM )
{
    OnOK();

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Message Throttle Failed event from Com.
//! \details Show a pop up containing the error.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventMsgThrottleFailed( WPARAM, LPARAM )
{
    AfxGetMainWnd()->MessageBox
        (
        _T("Invalid protocol ID or state."),
        _T("Message Throttling Error")
        );
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the Server to Client Text Message Error event
//!     from Com.
//! \details Display an error message pop up.
//! \param aEventDataPtr The message ID.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventOpenTextMsgError
    (
    WPARAM,
    LPARAM aEventDataPtr
    )
{
    CString errorString;

    MessageId * messageId = (MessageId*)aEventDataPtr;

    errorString.Format( _T("Text Message with id %s already exists!"), messageId->toCString( mCom.mClientCodepage ) );
    AfxGetMainWnd()->MessageBox( errorString, _T("Open Text Message Failure" ) );
    delete messageId;

    return 0;
}
//----------------------------------------------------------------------
//! \brief Handler for the Text Message Status Received event from Com.
//! \details Show a pop up containing the message status.
//! \param aEventDataPtr The text message status details
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventTxtMsgStatus
    (
    WPARAM,
    LPARAM aEventDataPtr
    )
{
    text_msg_status_event_type* statusEvent = (text_msg_status_event_type*)aEventDataPtr;
    if( mTxtMsgStatusDlg != NULL )
    {
        mTxtMsgStatusDlg->EndDialog( IDCANCEL );
        mTxtMsgStatusDlg = NULL;
    }
    CTxtMsgStatusDlg dlg( this, mCom, statusEvent );    //same as above
    mTxtMsgStatusDlg = &dlg;  //keep track of this dialog so if another comes along
    dlg.DoModal();            //we can close this one
    mTxtMsgStatusDlg = NULL;

    delete statusEvent;

    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for the UI Text Change Failed event from Com.
//! \details Show a pop up containing the CCommErrorDlg.
//! \return 0, always
//! \since Protocol A604
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventUserInterfaceTextChangeFailed( WPARAM, LPARAM )
{
    AfxGetMainWnd()->MessageBox( _T("Updating UI Text Failed!"), _T("Error!") );
    return 0;
}
#endif

#if (FMI_SUPPORT_A606)
//---------------------------------------------------------------------
//! \brief Click handler for the FMI Safe Mode button
//! \details Display a modal dialog allowing the user to set
//!     FMI Safe Mode Speed.
//! \since Protocol A606
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedSafeMode()
{
    CSafeModeDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Handler for the FMI Safe Mode event from Com.
//! \details Display an error message pop up.
//! \return 0, always
//! \since Protocol A606
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventFmiSafeModeError( WPARAM, LPARAM )
{
    CString *errorString = new CString( "Error setting FMI Safe Mode!" );

    AfxGetMainWnd()->MessageBox( *errorString, _T( "FMI Safe Mode Failure" ) );

    return 0;
}
#endif

#if( FMI_SUPPORT_A607 )
//----------------------------------------------------------------------
//! \brief Click handler for the Message Delete button
//! \details Display a modal dialog allowing the user to request that a
//!     particular text message be deleted.
//! \since A607
//----------------------------------------------------------------------
void CFmiPcAppDlg::OnBnClickedMsgDelete()
{
    CTxtMsgDeleteRequestDlg dlg( this, mCom );
    dlg.DoModal();
}

//----------------------------------------------------------------------
//! \brief Handler for the Delete Text Message Status event from Com.
//! \details Show a pop up containing the CCommErrorDlg.
//! \param aEventData Boolean indicating whether the delete succeeded
//! \return 0, always
//! \since Protocol A607
//----------------------------------------------------------------------
afx_msg LRESULT CFmiPcAppDlg::OnEventDeleteTextMessageStatus
    (
    WPARAM aEventData,
    LPARAM
    )
{
    boolean deleteSuccessful = (boolean)aEventData;

    if( deleteSuccessful )
    {
        AfxGetMainWnd()->MessageBox( _T("Text Message Deleted"), _T("Error!") );
    }
    else
    {
        AfxGetMainWnd()->MessageBox( _T("Text Message Delete failed.  Check Message ID and try again"), _T("Error!") );
    }

    return 0;
}

#endif

#if( FMI_SUPPORT_A608 )
//---------------------------------------------------------------------
//! \brief Click handler for the FMI Speed Limit Alerts button
//! \details Display a modal dialog allowing the user to configure
//!     FMI speed limit alerts
//! \since Protocol A608
//----------------------------------------------------------------------
afx_msg void CFmiPcAppDlg::OnBnClickedSpeedLimit()
{
    CSpeedLimitAlertsDlg dlg( this, mCom );
    dlg.DoModal();
}
#endif

