/*********************************************************************
*
*   MODULE NAME:
*       CDeleteDataDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CDeleteDataDlg.h"
#include "Event.h"

using namespace std;

IMPLEMENT_DYNAMIC( CDeleteDataDlg, CDialog )

BEGIN_MESSAGE_MAP( CDeleteDataDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent window
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CDeleteDataDlg::CDeleteDataDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_DELETE_DATA, aParent )
    , mCom( aCom )
    , mDeleteMessagesChecked( FALSE )
    , mDeleteStopsChecked( FALSE )
#if( FMI_SUPPORT_A604 )
    , mDeleteActiveRouteChecked( FALSE )
    , mDeleteCannedMessagesChecked( FALSE )
    , mDeleteCannedResponsesChecked( FALSE )
    , mDeleteGpiFileChecked( FALSE )
    , mDeleteDriverInfoChecked( FALSE )
    , mDisableFmiChecked( FALSE )
#endif
#if( FMI_SUPPORT_A607 )
    , mWaypointsChecked( FALSE )
#endif
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CDeleteDataDlg::~CDeleteDataDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CDeleteDataDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_MESSAGES, mDeleteMessagesChecked );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_STOPS, mDeleteStopsChecked );
#if( FMI_SUPPORT_A604 )
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_ROUTE, mDeleteActiveRouteChecked );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_CANNED_MESSAGES, mDeleteCannedMessagesChecked );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_CANNED_RESPONSES, mDeleteCannedResponsesChecked );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_GPI_FILE, mDeleteGpiFileChecked );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_DRIVER_INFO, mDeleteDriverInfoChecked );
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_DISABLE_FMI, mDisableFmiChecked );
#endif
#if( FMI_SUPPORT_A607 )
    DDX_Check( aDataExchange, IDC_DELDATA_CHK_WAYPOINTS, mWaypointsChecked );
#endif
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created.  It
//!     disables all check boxes not supported by the server.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CDeleteDataDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

#if( !FMI_SUPPORT_A604 )
    // Disable the controls specific to A604 support
    GetDlgItem( IDC_DELDATA_CHK_ROUTE )->EnableWindow( FALSE );
    GetDlgItem( IDC_DELDATA_CHK_CANNED_MESSAGES )->EnableWindow( FALSE );
    GetDlgItem( IDC_DELDATA_CHK_CANNED_RESPONSES )->EnableWindow( FALSE );
    GetDlgItem( IDC_DELDATA_CHK_GPI_FILE )->EnableWindow( FALSE );
    GetDlgItem( IDC_DELDATA_CHK_DRIVER_INFO )->EnableWindow( FALSE );
    GetDlgItem( IDC_DELDATA_CHK_DISABLE_FMI )->EnableWindow( FALSE );
#endif

#if( !FMI_SUPPORT_A607 )
    // Disable the controls specific to A607 support
    GetDlgItem( IDC_DELDATA_CHK_WAYPOINTS )->EnableWindow( FALSE );
#endif

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Determines which check boxes are checked.  For each box
//!     that is checked, initiate the Data Deletion protocol.
//----------------------------------------------------------------------
void CDeleteDataDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
#if( FMI_SUPPORT_A604 )
    //check to see if they want to delete all data first
    //if so, we don't have to worry about deleting other stuff
    if( mDisableFmiChecked )
    {
        mCom.sendDataDeletionRequest( DISABLE_FMI );
    }
    else
#endif
    {
        //not deleting all data
        //need to process all individual check boxes
        if( mDeleteMessagesChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_ALL_MESSAGES );
        }

        if( mDeleteStopsChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_ALL_STOPS );
        }

#if( FMI_SUPPORT_A604 )
        if( mDeleteActiveRouteChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_ACTIVE_ROUTE );
        }

        if( mDeleteCannedMessagesChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_CANNED_MESSAGES );
        }

        if( mDeleteCannedResponsesChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_CANNED_RESPONSES );
        }

        if( mDeleteGpiFileChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_GPI_FILE );
        }

        if( mDeleteDriverInfoChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_DRIVER_ID_AND_STATUS );
        }
#endif
#if( FMI_SUPPORT_A607 )
        if( mWaypointsChecked )
        {
            mCom.sendDataDeletionRequest( DELETE_WAYPOINTS );
        }
#endif
    }

    OnOK();
}    /* OnBnClickedOK() */
