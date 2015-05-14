/*********************************************************************
*
*   MODULE NAME:
*       CCommErrorDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CCommErrorDlg.h"
#include "CSelectCommPortDlg.h"
#include "CFmiPcAppDlg.h"

IMPLEMENT_DYNAMIC( CCommErrorDlg, CDialog )

BEGIN_MESSAGE_MAP( CCommErrorDlg, CDialog )
    ON_BN_CLICKED( IDC_ERROR_BTN_EXIT, OnBnClickedExit )
    ON_BN_CLICKED( IDC_ERROR_BTN_CHANGE_PORT, OnBnClickedChangeCom )
    ON_BN_CLICKED( IDC_ERROR_BTN_RETRY, OnBnClickedRetry )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aErrorMessage The error message to display
//! \param aIsPortDown If true, indicates that the com port is down,
//!     and the OK button is replaced by Exit, Retry, and Change Port
//!     buttons.
//! \param  aParent The parent window
//! \param aCom The FmiApplicationLayer that should be notified when
//!     communication resumes
//----------------------------------------------------------------------
CCommErrorDlg::CCommErrorDlg
    (
    TCHAR               * aErrorMessage,
    BOOL                  aIsPortDown,
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_ERROR, aParent )
    , mMessageText( _T("") )
    , mCom( aCom )
{
    mMessageText.Format( _T("%s"), aErrorMessage );
    mComPortDown = aIsPortDown;
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CCommErrorDlg::~CCommErrorDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CCommErrorDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_ERROR_TXT_MESSAGE, mMessageText );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer.  It also hides the OK button and shows
//!     the Exit, Retry, and Change Port buttons if the com port is
//!     down (as indicated in the constructor).
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CCommErrorDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    if( mComPortDown )
    {
        GetDlgItem( IDOK )->ShowWindow( SW_HIDE );
        GetDlgItem( IDC_ERROR_BTN_EXIT )->ShowWindow( SW_SHOW );
        GetDlgItem( IDC_ERROR_BTN_RETRY )->ShowWindow( SW_SHOW );
        GetDlgItem( IDC_ERROR_BTN_CHANGE_PORT )->ShowWindow( SW_SHOW );
    }
    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Click handler for the Exit button
//! \details Exit the application by sending WM_CLOSE to the main
//!     window.
//----------------------------------------------------------------------
void CCommErrorDlg::OnBnClickedExit()
{
    AfxGetMainWnd()->SendMessage( WM_CLOSE );
    mComPortDown = FALSE;    //change so we can call OnCancel
    OnCancel();
}

//----------------------------------------------------------------------
//! \brief Cancel action handler
//! \details Dismiss the dialog if the com port is not down (otherwise
//!     com state would be violated).
//----------------------------------------------------------------------
void CCommErrorDlg::OnCancel()
{
    if( !mComPortDown )
    {
        CDialog::OnCancel();
    }
}

//----------------------------------------------------------------------
//! \brief Click handler for the Retry button
//! \details Re-enable the communication link and make sure an enable
//!    packet is sent if appropriate.
//----------------------------------------------------------------------
void CCommErrorDlg::OnBnClickedRetry()
{
#if( FMI_SUPPORT_A602 )
    mCom.sendEnable();
#else
    mCom.clearError();
#endif

    OnOK();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Change Port button
//! \details Display the Select Port dialog, then (if a port was
//!     selected) retry the communication.
//----------------------------------------------------------------------
void CCommErrorDlg::OnBnClickedChangeCom()
{
    CSelectCommPortDlg dlg( this );
    if( IDOK == dlg.DoModal() )
    {
        OnBnClickedRetry();
    }
}

//----------------------------------------------------------------------
//! \brief Called by MFC after the window has been destroyed; performs
//!     final termination activities.
//! \details This dialog is displayed in response to particular events;
//!     to prevent its message loop from interrupting another Windows
//!     message that may be in progress, this dialog is created
//!     modeless. PostNcDestroy sends an event to notify its creator
//!     that the dialog is safe to delete.
//----------------------------------------------------------------------
void CCommErrorDlg::PostNcDestroy()
{
    Event::post( EVENT_COMM_ERROR_DLG_CLOSED );
    CDialog::PostNcDestroy();
}

