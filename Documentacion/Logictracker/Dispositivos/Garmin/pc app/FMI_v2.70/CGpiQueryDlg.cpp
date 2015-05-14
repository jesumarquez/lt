/*********************************************************************
*
*   MODULE NAME:
*       CGpiQueryDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CGpiQueryDlg.h"
#include "Event.h"

IMPLEMENT_DYNAMIC( CGpiQueryDlg, CDialog )

BEGIN_MESSAGE_MAP( CGpiQueryDlg, CDialog )
    ON_BN_CLICKED( IDC_GPIQUERY_BTN_UPDATE, OnBnClickedUpdate )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_GPI_FILE_INFO_RECEIVED ), OnGpiInfoReceived )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CGpiQueryDlg::CGpiQueryDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_GPI_QUERY, aParent )
    , mCom( aCom )
    , mFileVersion( _T("") )
    , mFileSize( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CGpiQueryDlg::~CGpiQueryDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CGpiQueryDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_GPIQUERY_TXT_VERSION, mFileVersion );
    DDX_Text( aDataExchange, IDC_GPIQUERY_TXT_SIZE, mFileSize );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initiates a GPI File Information Request to get the current
//!     information from the client.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CGpiQueryDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    OnBnClickedUpdate();
    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Click handler for the Update button
//! \details Initiate the GPI File Information Request protocol to get
//!     the current information from the client.
//----------------------------------------------------------------------
void CGpiQueryDlg::OnBnClickedUpdate()
{
    mCom.sendGpiFileInfoRequest();
}    /* OnBnClickedUpdate() */

//----------------------------------------------------------------------
//! \brief Handler for the GPI Information Received event
//! \details Updates the fields in this dialog with the information
//!     received.  If the file version is printable, it is interpreted
//!     as a string; otherwise, it is displayed in hexadecimal.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LRESULT CGpiQueryDlg::OnGpiInfoReceived( WPARAM, LPARAM )
{
    TCHAR versionString[33];    // 2 * 16 bytes + null terminator
    char  versionHex[33];
    uint8 versionLength = minval( mCom.mGpiFileVersionLength, 16 );

    memset( versionString, 0, sizeof( versionString ) );
    mFileSize.Format( _T(" %d"), mCom.mGpiFileSize );
    if( UTIL_data_is_printable( (const char*)mCom.mGpiFileVersion, versionLength ) )
    {
        MultiByteToWideChar( mCom.mClientCodepage, 0, (char*)mCom.mGpiFileVersion, -1, versionString, versionLength );
        mFileVersion.Format( _T(" %s"), versionString );
    }
    else
    {
        memset( versionHex, 0, sizeof( versionHex ) );
        UTIL_uint8_to_hex( mCom.mGpiFileVersion, versionHex, versionLength );
        MultiByteToWideChar( mCom.mClientCodepage, 0, versionHex, -1, versionString, sizeof( versionString ) );
        mFileVersion.Format( _T(" 0x%s"), versionString );
    }
    UpdateData( FALSE );

    return 0;
}    /* OnGpiInfoReceived() */
