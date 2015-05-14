/*********************************************************************
*
*   MODULE NAME:
*       CDriverIdAndStatusDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CDriverIdAndStatusDlg.h"
#include "Event.h"
#include "util.h"
#include "CDriverLoginDlg.h"

using namespace std;

IMPLEMENT_DYNAMIC( CDriverIdAndStatusDlg, CDialog )

BEGIN_MESSAGE_MAP( CDriverIdAndStatusDlg, CDialog )
    ON_WM_TIMER()
    ON_EN_CHANGE( IDC_DRVRINFO_EDIT_NEW_ID, OnEnChangeEditDriverId )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_SEND_STATUS, OnBnClickedSendDriverStatus )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_SEND_ID, OnBnClickedSendDriverId )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_GET_ID, OnBnClickedRefreshDriverId )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_GET_STATUS, OnBnClickedRefreshDriverStatus )
    ON_EN_CHANGE( IDC_DRVRINFO_EDIT_STATUS_ID, OnEnChangeEditSet )
    ON_EN_CHANGE( IDC_DRVRINFO_EDIT_STATUS_TEXT, OnEnChangeEditSet )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_DELETE_STATUS, OnBnClickedDelete )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_ADD_STATUS, OnBnClickedSendDriverStatusItem )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_CBN_SELENDOK( IDC_DRVRINFO_CBO_IDX, OnIndexChanged )
    ON_EN_SETFOCUS( IDC_DRVRINFO_EDIT_NEW_ID, OnEnSetfocusDriverIdEdit )
    ON_EN_KILLFOCUS( IDC_DRVRINFO_EDIT_NEW_ID, OnEnKillfocusDriverIdEdit )
    ON_EN_SETFOCUS( IDC_DRVRINFO_EDIT_STATUS_TEXT, OnEnSetfocusStatusEdit )
    ON_EN_KILLFOCUS( IDC_DRVRINFO_EDIT_STATUS_TEXT, OnEnKillfocusStatusEdit )
    ON_EN_SETFOCUS( IDC_DRVRINFO_EDIT_STATUS_ID, OnEnSetfocusStatusEdit )
    ON_EN_KILLFOCUS( IDC_DRVRINFO_EDIT_STATUS_ID, OnEnKillfocusStatusEdit )
    ON_LBN_SELCHANGE( IDC_DRVRINFO_LST_STATUS, OnLbnSelchangeStatuslist )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DRIVER_ID_CHANGED ), OnDriverIdChanged )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DRIVER_STATUS_CHANGED ), OnDriverStatusChanged )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_DRIVER_STATUS_LIST_CHANGED ), OnDriverStatusListChanged )
#if( FMI_SUPPORT_A607 )
    ON_BN_CLICKED( IDC_DRVRINFO_BTN_EDIT_LOGINS, OnBnClickedEditLogins )
#endif
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent window
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CDriverIdAndStatusDlg::CDriverIdAndStatusDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_DRIVER_ID_STATUS, aParent )
    , mCom( aCom )
    , mCurrentDriverId( _T("") )
    , mCurrentDriverStatus( _T("") )
    , mNewDriverId( _T("") )
    , mSelectedListIndex( 0 )
    , mNewDriverStatusId( _T("") )
    , mNewDriverStatusText( _T("") )
    , mIndex( 0 )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CDriverIdAndStatusDlg::~CDriverIdAndStatusDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_DRVRINFO_TXT_CURRENT_ID, mCurrentDriverId );
    DDX_Text( aDataExchange, IDC_DRVRINFO_TXT_CURRENT_STATUS, mCurrentDriverStatus );
    DDX_Text( aDataExchange, IDC_DRVRINFO_EDIT_NEW_ID, mNewDriverId );
    DDX_CBIndex( aDataExchange, IDC_DRVRINFO_CBO_IDX, mIndex );
    DDX_Control( aDataExchange, IDC_DRVRINFO_LST_STATUS, mDriverStatusList );
    DDX_LBIndex( aDataExchange, IDC_DRVRINFO_LST_STATUS, mSelectedListIndex );
    DDX_Text( aDataExchange, IDC_DRVRINFO_EDIT_STATUS_ID, mNewDriverStatusId );
    DDX_Text( aDataExchange, IDC_DRVRINFO_EDIT_STATUS_TEXT, mNewDriverStatusText );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created.  It
//!     initializes the status list and requests the current driver id
//!     and status from the client.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CDriverIdAndStatusDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    updateListBox();
#if( FMI_SUPPORT_A607 )
    if( mCom.mUseMultipleDrivers )
    {
        GetDlgItem( IDC_DRVRINFO_LBL_IDX )->EnableWindow( TRUE );
        GetDlgItem( IDC_DRVRINFO_CBO_IDX )->EnableWindow( TRUE );

        for( uint8 i = 0; i < FMI_DRIVER_COUNT; i++ )
        {
            mCom.sendA607DriverIdRequest( i );
            mCom.sendA607DriverStatusRequest( i );
        }
    }
    else
#endif
    {
        GetDlgItem( IDC_DRVRINFO_LBL_IDX )->EnableWindow( FALSE );
        GetDlgItem( IDC_DRVRINFO_CBO_IDX )->EnableWindow( FALSE );
        mCom.sendDriverIdRequest();
        mCom.sendDriverStatusRequest();
    }

    SetWindowPos( NULL, 250, 600, 0, 0, SWP_NOSIZE | SWP_NOZORDER );
    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Update the driver status list box from the map of items
//!     owned by FmiApplicationLayer.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::updateListBox()
{
    FileBackedMap<ClientListItem>::const_iterator it = mCom.mDriverStatuses.begin();
    CString listItem;

    //must keep track of where the list was scrolled to
    //since we reset content we must reinitialize these
    int selectedIndex = mDriverStatusList.GetCurSel();
    int topIndex = mDriverStatusList.GetTopIndex();

    mDriverStatusList.ResetContent();
    //loop through ids and add to status list
    for(; it != mCom.mDriverStatuses.end(); it++ )
    {
        if( it->second.isValid() )
        {
            listItem.Format( _T("%d - %s"), it->first, it->second.getCurrentName() );
            mDriverStatusList.AddString( listItem );
        }
    }

    //reset scroll and selection
    mDriverStatusList.SetCurSel( selectedIndex );
    mDriverStatusList.SetTopIndex( topIndex );
}    /* updateListBox() */

//----------------------------------------------------------------------
//! \brief Event handler for the  FMI_EVENT_DRIVER_STATUS_LIST_CHANGED
//!     event generated by FmiApplicationLayer.
//! \details Updates the driver status list.
//----------------------------------------------------------------------
afx_msg LPARAM CDriverIdAndStatusDlg::OnDriverStatusListChanged( WPARAM, LPARAM )
{
    updateListBox();

    return 0;
}

//----------------------------------------------------------------------
//! \brief Event handler for the FMI_EVENT_DRIVER_ID_CHANGED
//!     event generated by FmiApplicationLayer.
//! \details Updates the driver ID text box.
//! \param aIndex The index of the driver
//----------------------------------------------------------------------
afx_msg LPARAM CDriverIdAndStatusDlg::OnDriverIdChanged
    (
    WPARAM aIndex,
    LPARAM
    )
{
    if( aIndex == mIndex )
    {
        //This displays the driver ID, but does
        //not request it.
        //When the dialog is initialized, it requests both.
        //The client should send messages to mCom if they are updated.
        TCHAR        driverId[50];
        MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mDriverId[ aIndex ], -1, driverId, 50 );
        driverId[49] = '\0';
        mCurrentDriverId.Format( _T("%s"), driverId );
        UpdateData( FALSE );
    }

    return 0;
}

//----------------------------------------------------------------------
//! \brief Event handler for the  FMI_EVENT_DRIVER_STATUS_CHANGED
//!     event generated by FmiApplicationLayer.
//! \details Updates the driver status text box.
//! \param aIndex The index of the driver
//----------------------------------------------------------------------
afx_msg LPARAM CDriverIdAndStatusDlg::OnDriverStatusChanged( WPARAM aIndex, LPARAM )
{
    if( aIndex == mIndex )
    {
        //This displays the driver status, but does
        //not request it.
        //When the dialog is initialized, it requests both.
        //The client should send messages to mCom if they are updated.
        TCHAR        driverStatus[50];
        MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mDriverStatus[ aIndex ], -1, driverStatus, 50 );
        driverStatus[49] = '\0';
        mCurrentDriverStatus.Format( _T(" %s"), driverStatus );
        UpdateData( FALSE );
    }

    return 0;
}    /* OnDriverStatusListChanged() */

//----------------------------------------------------------------------
//! \brief Edit event handler for the Driver ID edit box.
//! \details Enable or disable the Send (Driver ID) button.  If the
//!     Driver ID is specified, enable the Send button, otherwise,
//!     disable it.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnEnChangeEditDriverId()
{
    UpdateData( TRUE );
    if( mNewDriverId != "" )
        GetDlgItem( IDC_DRVRINFO_BTN_SEND_ID )->EnableWindow( TRUE );
    else
        GetDlgItem( IDC_DRVRINFO_BTN_SEND_ID )->EnableWindow( FALSE );
}    /* OnEnChangeEditDriverId() */

//----------------------------------------------------------------------
//! \brief Selection Changed event handler for the status list.
//! \details This function fills in the status id and status text
//!     fields of the dialog with the information of the selected
//!     message in the message list, for easy editing.  It also
//!     enables the Delete button when an item is selected.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnLbnSelchangeStatuslist()
{
    int selectedIndex = mDriverStatusList.GetCurSel();
    if( selectedIndex >= 0 && selectedIndex < mDriverStatusList.GetCount() )
    {
        uint32 driverStatusId = mCom.mDriverStatuses.getKeyAt( selectedIndex );
        ClientListItem& driverStatusItem = mCom.mDriverStatuses.get( driverStatusId );

        mNewDriverStatusId.Format( _T("%u"), driverStatusId );
        mNewDriverStatusText.Format( _T("%s"), driverStatusItem.getCurrentName() );
        UpdateData( FALSE );
        GetDlgItem( IDC_DRVRINFO_BTN_DELETE_STATUS )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_DRVRINFO_BTN_DELETE_STATUS )->EnableWindow( FALSE );
    }
    mDriverStatusList.SetCurSel( selectedIndex );
}

//----------------------------------------------------------------------
//! \brief Click handler for the Send (Driver Status) button.
//! \details Sends a Driver Status Update to the client
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedSendDriverStatus()
{
    UpdateData( TRUE );
    if( mSelectedListIndex >= 0 )
    {
        uint32 driverStatusId = mCom.mDriverStatuses.getKeyAt( mSelectedListIndex );
#if FMI_SUPPORT_A607
        mCom.sendA607DriverStatusUpdate( driverStatusId, mIndex );
#else
        mCom.sendDriverStatusUpdate( driverStatusId );
#endif
    }
}    /* OnBnClickedSendDriverStatus() */

//----------------------------------------------------------------------
//! \brief Click handler for the Send (Driver ID) button.
//! \details Sends a Driver ID update to the client.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedSendDriverId()
{
    char    driverId[50];

    UpdateData( TRUE );
    WideCharToMultiByte( mCom.mClientCodepage, 0, mNewDriverId, -1, driverId, 50, NULL, NULL );
    driverId[49] = '\0';
#if FMI_SUPPORT_A607
    mCom.sendA607DriverIdUpdate( driverId, mIndex );
#else
    mCom.sendDriverIdUpdate( driverId );
#endif
}    /* OnBnClickedSendDriverId */

//----------------------------------------------------------------------
//! \brief Click handler for the Refresh (Driver ID) button
//! \details Sends a Driver ID Request to the client
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedRefreshDriverId()
{
#if( FMI_SUPPORT_A607 )
    if( mCom.mUseMultipleDrivers )
    {
        mCom.sendA607DriverIdRequest( mIndex );
    }
    else
#endif
    {
        mCom.sendDriverIdRequest();
    }
}    /* OnBnClickedRefreshDriverId() */

//----------------------------------------------------------------------
//! \brief Click handler for the Refresh (Driver Status) button
//! \details Requests the current driver status from the client
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedRefreshDriverStatus()
{
#if( FMI_SUPPORT_A607 )
    if( mCom.mUseMultipleDrivers )
    {
        mCom.sendA607DriverStatusRequest( mIndex );
    }
    else
#endif
    {
        mCom.sendDriverStatusRequest();
    }
}    /* OnBnClickedRefreshDriverStatus() */

//----------------------------------------------------------------------
//! \brief Edit handler for the Add/Update Status group
//! \details This function is called when either of the two edit
//!     boxes in the Add/Update Status group (ID and Status) are
//!     modified.  Enables the Send button in this group if both edit
//!     boxes contain mIsValid data, disables the button otherwise.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnEnChangeEditSet()
{
    BOOL fieldsValid = TRUE;
#if !SKIP_VALIDATION
    char idBuffer[11];
    UpdateData( TRUE );

    memset( idBuffer, 0, sizeof( idBuffer ) );
    if( 0 == WideCharToMultiByte
                (
                CP_ACP,
                0,
                mNewDriverStatusId.GetBuffer(),
                mNewDriverStatusId.GetLength(),
                idBuffer,
                sizeof( idBuffer ),
                NULL, NULL
                ) )
    {
        fieldsValid = FALSE;
    }
    else if( !UTIL_data_is_uint32( idBuffer ) )
    {
        fieldsValid = FALSE;
    }

    if( mNewDriverStatusText.GetLength() == 0 ||
        mNewDriverStatusText.GetLength() >= 50   )
    {
        fieldsValid = FALSE;
    }
#endif
    GetDlgItem( IDC_DRVRINFO_BTN_ADD_STATUS )->EnableWindow( fieldsValid );
} /* OnEnChangeEditSet() */

//----------------------------------------------------------------------
//! \brief Click handler for the Delete (Driver Status Item) button
//! \details Sends a Delete Driver Status List Item request to the
//!     client.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedDelete()
{
    UpdateData( TRUE );
    if( mSelectedListIndex >= 0 && mSelectedListIndex < mCom.mDriverStatuses.validCount() )
    {
        uint32 driverStatusId = mCom.mDriverStatuses.getKeyAt( mSelectedListIndex );
        mCom.sendDeleteDriverStatusListItem( driverStatusId );
    }
} /* OnBnClickedDelete() */

//----------------------------------------------------------------------
//! \brief Click handler for the Send (Driver Status Item) button
//! \details Sends the driver status list item specified in the
//!     Add/Set Status group of edit boxes to the client using the
//!     Set Driver Status List Item protocol.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedSendDriverStatusItem()
{
    UpdateData( TRUE );
    uint32 id = _tstoi( mNewDriverStatusId.GetBuffer() );

    mCom.sendDriverStatusListItem( id, mNewDriverStatusText );
} /* OnBnClickedSendDriverStatusItem() */

//----------------------------------------------------------------------
//! \brief Click handler for the OK (Close) button
//! \details Destroy the window, since this is a modeless dialog.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedOk()
{
    DestroyWindow();
} /* OnBnClickedOk */

//----------------------------------------------------------------------
//! \brief Selection handler for the driver index combo
//! \details Changes the other controls to display appropriate info
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnIndexChanged()
{
    UpdateData( TRUE );
    OnDriverIdChanged( mIndex, NULL );
    OnDriverStatusChanged( mIndex, NULL );
} /* OnIndexChanged */

//----------------------------------------------------------------------
//! \brief Handles the Cancel action
//! \details Destroy the window, since this is a modeless dialog.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnCancel()
{
    DestroyWindow();
} /* OnCancel */

//----------------------------------------------------------------------
//! \brief Perform final cleanup after the dialog window is destroyed.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::PostNcDestroy()
{
    CDialog::PostNcDestroy();
    Event::post( EVENT_FMI_DRIVER_ID_AND_STATUS_DLG_CLOSED );
} /* PostNcDestroy() */

//----------------------------------------------------------------------
//! \brief Set Focus handler for the Driver ID edit box
//! \details Set the default button to the Send (Driver ID) button so
//!     that it will be activated when the user presses Enter.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnEnSetfocusDriverIdEdit()
{
    SendMessage( DM_SETDEFID, IDC_DRVRINFO_BTN_SEND_ID );
}

//----------------------------------------------------------------------
//! \brief Kill Focus handler for the Driver ID edit box
//! \details Set the default button back to the CLose button so
//!     that it will be activated when the user presses Enter.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnEnKillfocusDriverIdEdit()
{
    SendMessage( DM_SETDEFID, IDOK );
}

//----------------------------------------------------------------------
//! \brief Set Focus handler for the Add/Update Status edit boxes
//! \details Set the default button to the Send (Driver Status Item)
//!     button so that it will be activated when the user presses
//!     Enter.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnEnSetfocusStatusEdit()
{
    SendMessage( DM_SETDEFID, IDC_DRVRINFO_BTN_ADD_STATUS );
}

//----------------------------------------------------------------------
//! \brief Kill Focus handler for the Driver Status edit box
//! \details Set the default button back to the CLose button so
//!     that it will be activated when the user presses Enter.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnEnKillfocusStatusEdit()
{
    SendMessage( DM_SETDEFID, IDOK );
}

#if( FMI_SUPPORT_A607 )
//----------------------------------------------------------------------
//! \brief Click handler for the Driver Logins button
//! \details Display the driver login dialog.
//----------------------------------------------------------------------
void CDriverIdAndStatusDlg::OnBnClickedEditLogins()
{
    CDriverLoginDlg * dlg = new CDriverLoginDlg( this, mCom );
    dlg->Create( IDD_DRIVER_LOGINS );
    dlg->ShowWindow( SW_SHOW );
}
#endif
