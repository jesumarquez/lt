/*********************************************************************
*
*   MODULE NAME:
*       CSpeedLimitAlertsDlg.cpp
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CSpeedLimitAlertsDlg.h"

IMPLEMENT_DYNAMIC( CSpeedLimitAlertsDlg, CDialog )

BEGIN_MESSAGE_MAP( CSpeedLimitAlertsDlg, CDialog )
    ON_BN_CLICKED( IDC_SPEED_LIMIT_SEND, OnBnClickedSend )
    ON_CBN_SELCHANGE(IDC_SPEED_LIMIT_CBO_MODE, OnCbnSelchangeSpeedLimitCboMode)
    ON_MESSAGE( WM_EVENT( EVENT_FMI_SPEED_LIMIT_SET_RESULT ), OnEventSpeedLimitSetResultFromClient )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CSpeedLimitAlertsDlg::CSpeedLimitAlertsDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_SPEED_ALERTS, aParent )
    , mCom( aCom )
    , mMode( 0 )
    , mTimeOver( 10 )
    , mTimeUnder( 10 )
    , mAlertUser( TRUE )
    , mThreshold( 0.0 )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CSpeedLimitAlertsDlg::~CSpeedLimitAlertsDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  pDX The DDX context
//----------------------------------------------------------------------
void CSpeedLimitAlertsDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

    // Bind to needed edit boxes
    DDX_Text( aDataExchange, IDC_SPEED_LIMIT_EDIT_TIME_OVER, mTimeOver );
    DDX_Text( aDataExchange, IDC_SPEED_LIMIT_EDIT_TIME_UNDER, mTimeUnder );
    DDX_Text( aDataExchange, IDC_SPEED_LIMIT_EDIT_THRESHOLD, mThreshold );

    // make sure time over and under values fits into one byte
    DDV_MinMaxInt( aDataExchange, mTimeOver, 0x0, 0xff );
    DDV_MinMaxInt( aDataExchange, mTimeUnder, 0x0, 0xff );

    // DDX will round to nearest integer, make sure to update the fields
    CString text;
    CEdit*  pEdit;
    text.Format( _T("%d"), mTimeOver );
    pEdit = (CEdit *)GetDlgItem( IDC_SPEED_LIMIT_EDIT_TIME_OVER );
    pEdit->SetWindowText( text );

    text.Format( _T("%d"), mTimeUnder );
    pEdit = (CEdit *)GetDlgItem( IDC_SPEED_LIMIT_EDIT_TIME_UNDER );
    pEdit->SetWindowText( text );

    // Resolve Mode
    // Note: Mode is enumerated with speed_limit_alert_mode_type
    mMode = (uint8)
        ( (CComboBox *)GetDlgItem( IDC_SPEED_LIMIT_CBO_MODE ) )->GetCurSel();

    // Resolve AlertUser
    mAlertUser = !(boolean)
        ( (CComboBox *)GetDlgItem( IDC_SPEED_LIMIT_CBO_ALERT_USER ) )->GetCurSel();
}

//----------------------------------------------------------------------
//! \brief Enable/Disable dialog fields
//! \param  aValue Enable state
//----------------------------------------------------------------------
void CSpeedLimitAlertsDlg::EnableFields
    (
    bool aValue
    )
{
    GetDlgItem( IDC_SPEED_LIMIT_EDIT_TIME_OVER )->EnableWindow( aValue );
    GetDlgItem( IDC_SPEED_LIMIT_EDIT_TIME_UNDER )->EnableWindow( aValue );
    GetDlgItem( IDC_SPEED_LIMIT_CBO_ALERT_USER )->EnableWindow( aValue );
    GetDlgItem( IDC_SPEED_LIMIT_EDIT_THRESHOLD )->EnableWindow( aValue );
}

//----------------------------------------------------------------------
//! \brief Click handler for the Send button
//! \details Send speed limit alerts setup packet
//----------------------------------------------------------------------
void CSpeedLimitAlertsDlg::OnBnClickedSend()
{
    UpdateData();

    mCom.sendSetSpeedLimitAlerts
        ( mMode, mTimeOver, mTimeUnder, mAlertUser, mThreshold );
}

//----------------------------------------------------------------------
//! \brief Selection changed handler for the Mode combo box
//! \details Enables/Disables appropriate fields
//----------------------------------------------------------------------
void CSpeedLimitAlertsDlg::OnCbnSelchangeSpeedLimitCboMode()
{
    UpdateData();
    switch( mMode )
    {
    case SPEED_LIMIT_MODE_CAR:
        EnableFields( TRUE );
        break;
    case SPEED_LIMIT_MODE_OFF:
        EnableFields( FALSE );
        break;
    case SPEED_LIMIT_MODE_TRUCK:
        EnableFields( TRUE );
        break;
    default:
        break;
    }
}

//----------------------------------------------------------------------
//! \brief Handler for the receipt event from Com.
//! \details Display result code
//! \param aResultCode The result code from the client.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CSpeedLimitAlertsDlg::OnEventSpeedLimitSetResultFromClient
    (
    WPARAM aResultCode,
    LPARAM
    )
{
    SetResult( static_cast<uint8>( aResultCode ) );

    return 0;
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CSpeedLimitAlertsDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    ( (CComboBox *)GetDlgItem( IDC_SPEED_LIMIT_CBO_MODE ) )->SetCurSel( 0 );
    ( (CComboBox *)GetDlgItem( IDC_SPEED_LIMIT_CBO_ALERT_USER ) )->SetCurSel( 0 );

    return TRUE;
}

//----------------------------------------------------------------------
//! \brief Set REsult text on the dialog.
//! \details Display result code
//! \param aResultCode The result code from the client.
//----------------------------------------------------------------------
void CSpeedLimitAlertsDlg::SetResult
    (
    uint8 aResultCode
    )
{
    CString resultCodeText;
    switch( aResultCode )
    {
    case SPEED_LIMIT_RESULT_SUCCESS:
        resultCodeText.Format( _T("Success") );
        break;
    case SPEED_LIMIT_RESULT_ERROR:
        resultCodeText.Format( _T("Error") );
        break;
    case SPEED_LIMIT_RESULT_MODE_UNSUPPORTED:
        resultCodeText.Format( _T("Unsupported") );
        break;
    default:
        resultCodeText.Format( _T("Unknown") );
        break;
    }

    CString text;
    CEdit * pEdit;
    text.Format( _T("%d - %s"), aResultCode, resultCodeText );
    pEdit = (CEdit *)GetDlgItem( IDC_SPEED_LIMIT_EDIT_RESULT );
    pEdit->SetWindowText( text );
}
