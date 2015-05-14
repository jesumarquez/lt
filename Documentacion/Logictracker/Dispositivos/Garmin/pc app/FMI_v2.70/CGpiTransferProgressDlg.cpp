/*********************************************************************
*
*   MODULE NAME:
*       CGpiTransferProgressDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CGpiTransferProgressDlg.h"
#include "CFmiPcAppDlg.h"
#include "Event.h"

IMPLEMENT_DYNAMIC( CGpiTransferProgressDlg, CDialog )

BEGIN_MESSAGE_MAP( CGpiTransferProgressDlg, CDialog )
    ON_BN_CLICKED( IDC_GPIPROGRESS_BTN_STOP, OnBnClickedStop )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_GPI_TRANSFER_PROGRESS ), OnGpiTransferProgress )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED ), OnGpiTransferStateChange )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CGpiTransferProgressDlg::CGpiTransferProgressDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_GPI_TRANSFER_PROGRESS, aParent )
    , mCom( aCom )
    , mTransferStatus( _T("") )
{
    mStopButtonClicked = FALSE;
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CGpiTransferProgressDlg::~CGpiTransferProgressDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CGpiTransferProgressDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_GPIPROGRESS_TXT_STATUS, mTransferStatus );
    DDX_Control( aDataExchange, IDC_GPIPROGRESS_PROGRESS_BAR, mProgressBar );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the progress bar and text box.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CGpiTransferProgressDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    // display the current mTransferStatus
    OnGpiTransferStateChange( 0, NULL );

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Handle a GPI Transfer State Change event
//! \details Updates the transfer mTransferStatus text box (based on
//!     the current state known to FmiApplicationLayer) and, if the
//!     transfer is in progress, the progress bar.
//----------------------------------------------------------------------
afx_msg LRESULT CGpiTransferProgressDlg::OnGpiTransferStateChange( WPARAM, LPARAM )
{
    switch ( mCom.mGpiTransferState )
    {
    //set mTransferStatus
    case FmiApplicationLayer::TRANSFER_NOT_STARTED:
        GetDlgItem( IDOK )->EnableWindow( TRUE );
        mTransferStatus.Format( _T("No Transfer") );
        break;
    case FmiApplicationLayer::TRANSFER_STARTED:
        GetDlgItem( IDOK )->EnableWindow( FALSE );
        mTransferStatus.Format( _T("Starting Transfer") );
        break;
    case FmiApplicationLayer::TRANSFER_IN_PROGRESS:
        GetDlgItem( IDOK )->EnableWindow( FALSE );
        mProgressBar.SetRange32( 0, mCom.mGpiFileTransferSize );
        mProgressBar.SetPos( 0 );
        break;
    case FmiApplicationLayer::TRANSFER_COMPLETED:
        GetDlgItem( IDOK )->EnableWindow( TRUE );
        GetDlgItem( IDC_GPIPROGRESS_BTN_STOP )->EnableWindow( FALSE );
        mTransferStatus.Format( _T("Done") );
        break;
    case FmiApplicationLayer::TRANSFER_FAILED:
        GetDlgItem( IDOK )->EnableWindow( TRUE );
        GetDlgItem( IDC_GPIPROGRESS_BTN_STOP )->EnableWindow( FALSE );
        if( mStopButtonClicked )
            mTransferStatus.Format( _T("Canceled at user request") );
        else
            mTransferStatus.Format( _T("Failed") );
        break;
    default:
        break;
    }
    UpdateData( FALSE );
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for GPI Transfer Progress event.
//! \details Updates the progress bar and mTransferStatus text with the
//!     progress.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CGpiTransferProgressDlg::OnGpiTransferProgress( WPARAM, LPARAM )
{
    //update progress bar
    mProgressBar.SetPos( mCom.mGpiTransferBytesDone );

    //update label
    mTransferStatus.Format
        (
        _T("Sent %u/%u bytes"),
        mCom.mGpiTransferBytesDone,
        mCom.mGpiFileTransferSize
        );
    UpdateData( FALSE );

    return 0;

}    /* OnGpiTransferStateChange() */

//----------------------------------------------------------------------
//! \brief Click handler for the Stop button
//! \details Tells FmiApplicationLayer to stop the transfer
//----------------------------------------------------------------------
void CGpiTransferProgressDlg::OnBnClickedStop()
{
    mStopButtonClicked = TRUE;
    mCom.stopGpiFileTransfer();
}    /* OnBnClickedStop() */


//----------------------------------------------------------------------
//! \brief Handles the Cancel action
//! \details Does absolutely nothing because the user must wait until
//!     transfer is done, or explicitly stop the transfer.
//----------------------------------------------------------------------
void CGpiTransferProgressDlg::OnCancel()
{
}    /* OnCancel */
