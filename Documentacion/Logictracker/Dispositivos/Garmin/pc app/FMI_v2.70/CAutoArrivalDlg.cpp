/*********************************************************************
*
*   MODULE NAME:
*       CAutoArrivalDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CAutoArrivalDlg.h"

IMPLEMENT_DYNAMIC( CAutoArrivalDlg, CDialog )

BEGIN_MESSAGE_MAP( CAutoArrivalDlg, CDialog )
    ON_BN_CLICKED( IDC_AUTOARR_CHK_ENABLED, OnBnClickedEnabled )
    ON_EN_CHANGE( IDC_AUTOARR_EDIT_TIME, OnEnChangeEditBox )
    ON_EN_CHANGE( IDC_AUTOARR_EDIT_DISTANCE, OnEnChangeEditBox )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CAutoArrivalDlg::CAutoArrivalDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_AUTO_ARRIVAL, aParent )
    , mCom( aCom )
    , mAutoArrivalEnabled( FALSE )
    , mMinimumStopTime( _T("") )
    , mMinimumStopDistance( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CAutoArrivalDlg::~CAutoArrivalDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CAutoArrivalDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

    DDX_Check( aDataExchange, IDC_AUTOARR_CHK_ENABLED, mAutoArrivalEnabled );
    DDX_Text( aDataExchange, IDC_AUTOARR_EDIT_TIME, mMinimumStopTime );
    DDX_Text( aDataExchange, IDC_AUTOARR_EDIT_DISTANCE, mMinimumStopDistance );
}

//----------------------------------------------------------------------
//! \brief This function is called when the window is created.
//! \details It sets up the parent, so when the ok button is clicked,
//!     we can access com.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CAutoArrivalDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    mAutoArrivalEnabled = TRUE;        //doesn't have to be checked, looks better though
    UpdateData( FALSE );

    return TRUE;
}  /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Button handler for the Enabled check box.
//! \details Makes sure a user can't send data unless they have
//!     entered all the data.  If they are disabling the option, new
//!     text fields are blanked out and the button is enabled.
//!     Otherwise, the button is disabled and the user is allowed to
//!     enter text.
//----------------------------------------------------------------------
void CAutoArrivalDlg::OnBnClickedEnabled()
{
    UpdateData( TRUE );
    if( mAutoArrivalEnabled )
    {
        if( mMinimumStopTime != "" && mMinimumStopDistance != "" )
        {
            GetDlgItem( IDOK )->EnableWindow( TRUE );
        }
        else
        {
            GetDlgItem( IDOK )->EnableWindow( FALSE );
        }
        GetDlgItem( IDC_AUTOARR_EDIT_TIME )->EnableWindow( TRUE );
        GetDlgItem( IDC_AUTOARR_EDIT_DISTANCE )->EnableWindow( TRUE );
    }
    else
    {
        mMinimumStopTime.Format( _T("") );
        mMinimumStopDistance.Format( _T("") );
        UpdateData( FALSE );
        GetDlgItem( IDC_AUTOARR_EDIT_TIME )->EnableWindow( FALSE );
        GetDlgItem( IDC_AUTOARR_EDIT_DISTANCE )->EnableWindow( FALSE );
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    }
} /* OnBnClickedEnable() */

//----------------------------------------------------------------------
//! \brief Edit handler for the edit boxes on this dialog.
//! \details Enables the OK button when there is text in all edit
//!     fields, but disable it as long as one is empty. This same
//!     function gets called no matter which edit box changes.
//----------------------------------------------------------------------
void CAutoArrivalDlg::OnEnChangeEditBox()
{
    UpdateData( TRUE );
    if( mMinimumStopTime != "" && mMinimumStopDistance != "" )
    {
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDOK )->EnableWindow( FALSE );
    }
} /* OnEnChangeEditBox() */

//----------------------------------------------------------------------
//! \brief Button handler for the OK button on this dialog.
//! \details If this button is able to be clicked, it means all
//!     relevant information has been entered.  If the user wants to
//!     enable Auto Arrival, it will send an update for the new time
//!     and distance.  Otherwise, it sends 0xFFFFFFFF for both
//!     arguments to turn auto arrival off.
//----------------------------------------------------------------------
void CAutoArrivalDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
    if( mAutoArrivalEnabled )
    {
        mCom.sendAutoArrival
            (
            _ttoi( mMinimumStopTime.GetBuffer() ),
            _ttoi( mMinimumStopDistance.GetBuffer() )
            );
    }
    else
    {
        mCom.sendAutoArrival( INVALID32, INVALID32 );
    }
    OnOK();
} /* OnBnClickedOk() */
