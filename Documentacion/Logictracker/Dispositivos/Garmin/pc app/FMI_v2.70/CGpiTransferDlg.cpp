/*********************************************************************
*
*   MODULE NAME:
*       CGpiTransferDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"

#include <fstream>

#include "CGpiTransferDlg.h"
#include "CGpiTransferProgressDlg.h"
#include "util.h"

using namespace std;

IMPLEMENT_DYNAMIC( CGpiTransferDlg, CDialog )

BEGIN_MESSAGE_MAP( CGpiTransferDlg, CDialog )
    ON_EN_CHANGE( IDC_GPIXFER_EDIT_FILENAME, OnEnChangeGpiFile )
    ON_EN_CHANGE( IDC_GPIXFER_EDIT_FILE_VERSION, OnEnChangeGpiFile )
    ON_BN_CLICKED( IDC_GPIXFER_BTN_FIND, OnBnClickedFind )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CGpiTransferDlg::CGpiTransferDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_GPI_TRANSFER, aParent )
    , mCom( aCom )
    , mGpiFilePath( _T("") )
    , mVersion( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CGpiTransferDlg::~CGpiTransferDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param aDataExchange The DDX context
//----------------------------------------------------------------------
void CGpiTransferDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_GPIXFER_EDIT_FILENAME, mGpiFilePath );
    DDX_Text( aDataExchange, IDC_GPIXFER_EDIT_FILE_VERSION, mVersion );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CGpiTransferDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Edit Change handler for all text boxes.
//! \details Enables the OK button if both a file name and mVersion are
//!     specified, and the mVersion is of the appropriate length
//----------------------------------------------------------------------
void CGpiTransferDlg::OnEnChangeGpiFile()
{
    BOOL isValid = TRUE;

    UpdateData( TRUE );

    if( mGpiFilePath == "" || mVersion == "" )
    {
        isValid = FALSE;
    }

    if( mVersion.Left( 2 ) == "0x" )
    {
        if( mVersion.GetLength() > 34 )
            isValid = FALSE;
    }
    else
    {
        if( mVersion.GetLength() > 16 )
            isValid = FALSE;
    }

    if( isValid )
    {
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDOK )->EnableWindow( FALSE );
    }
}

//----------------------------------------------------------------------
//! \brief Click handler for Find (file to send) button.
//! \details Displays a File..Open dialog allowing the user to select
//!     the file to send.
//----------------------------------------------------------------------
void CGpiTransferDlg::OnBnClickedFind()
{
    TCHAR workingDirectory[200];
    // opening a file in another directory changes the current
    // directory, which will cause problems because the log and data
    // files are opened relative to the current directory.  So, get the
    // directory now so that it can be restored when the user is done
    // picking a file.
    DWORD result = GetCurrentDirectory( 200, workingDirectory );
    if( result == 0 || result > 200 )
    {
        MessageBox( _T("Unable to get current directory"), _T("Severe Error") );
        OnCancel();
        return;
    }
    CFileDialog dlg
        (
        TRUE,
        _T("gpi"),
        NULL,
        OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
        _T("GPI Files (*.gpi)|*.gpi||")
        );
    if( dlg.DoModal() == IDOK )
    {
        mGpiFilePath = dlg.GetPathName();
        UpdateData( FALSE );
        OnEnChangeGpiFile();
    }
    if( !SetCurrentDirectory( workingDirectory ) )
    {
        MessageBox( _T("Unable to set current directory"), _T("Severe Error") );
        OnCancel();
        return;
    }
}    /* OnBnClickedFind() */

//----------------------------------------------------------------------
//! \brief Click handler for OK button.
//! \details Initiates the file transfer process.  Actual packets are
//!     sent by the mCom itself, but the file entered is checked before
//!     sending the message.  If the file exists, a GPI File Progress
//!     dialog box will appear that will not let the user continue
//!     until the file transfer completes or the user cancels it. If
//!     the file doesn't exist, an error dialog will appear.
//----------------------------------------------------------------------
void CGpiTransferDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
    char file[200];

    char    versionString[35];
    uint8   version[16];
    uint8   versionLength;

    memset( version, 0, sizeof( version ) );
    WideCharToMultiByte( mCom.mClientCodepage, 0, mVersion, -1, versionString, 34, NULL, NULL );
    versionString[34] = '\0';

    if( strncmp( versionString, "0x", 2 ) == 0 )
    {
        versionLength = (uint8)UTIL_hex_to_uint8( versionString + 2, version, 16 );
    }
    else
    {
        versionLength = (uint8)minval( 16, strlen( versionString ) );
        memmove( version, versionString, versionLength );
    }

    WideCharToMultiByte( CP_ACP, 0, mGpiFilePath.GetBuffer(), -1, file, 200, NULL, NULL );
    file[199] = '\0';
    fstream open_file( file, ios_base::binary | ios_base::in );
    if( open_file.good() )
    {
        open_file.close();
        mCom.sendGpiFile( file, versionLength, version );

        CGpiTransferProgressDlg sending_dlg( this, mCom );
        sending_dlg.DoModal();
    }
    else
    {
        mGpiFilePath += _T(" could not be opened ");
        MessageBox( mGpiFilePath, _T("Error!") );
    }
    open_file.close();
    OnOK();
}    /* OnBnClickedOk */
