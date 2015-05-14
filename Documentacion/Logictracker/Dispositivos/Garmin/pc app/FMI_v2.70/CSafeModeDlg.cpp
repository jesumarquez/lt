/*********************************************************************
*
*   MODULE NAME:
*       CSafeModeDlg.cpp
*
*   Copyright 2010-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include <stdlib.h>
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CSafeModeDlg.h"

IMPLEMENT_DYNAMIC( CSafeModeDlg, CDialog )

BEGIN_MESSAGE_MAP( CSafeModeDlg, CDialog )
    ON_EN_CHANGE( IDC_SAFE_MODE_SPEED, OnEnChangeSafeModeSpeed )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//--------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//--------------------------------------------------------------------
CSafeModeDlg::CSafeModeDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_SAFE_MODE, aParent )
    , mCom( aCom )
    , speed( _T("") )
{
}

//--------------------------------------------------------------------
//! \brief Destructor
//--------------------------------------------------------------------
CSafeModeDlg::~CSafeModeDlg()
{
}

//---------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param aDataExchange The DDX context
//---------------------------------------------------------------------
void CSafeModeDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_SAFE_MODE_SPEED, speed );
}

//---------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to Com.
//! \return TRUE, since this function does not set focus to a control
//---------------------------------------------------------------------
BOOL CSafeModeDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    return TRUE;
} /* OnInitDialog() */

//---------------------------------------------------------------------
//! \brief Edit Change handler for Safe Mode Speed text box
//! \details Enables OK button if the speed is not empty; disables
//!     OK button otherwise.
//---------------------------------------------------------------------
void CSafeModeDlg::OnEnChangeSafeModeSpeed()
{
    UpdateData( TRUE );
    if( speed != "" )
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    else
        GetDlgItem( IDOK )->EnableWindow( FALSE );
}    /* OnEnChangeSafeModeSpeed() */

//---------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Sends a speed to the client based on
//!     the input entered by the user.
//---------------------------------------------------------------------
void CSafeModeDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
    char    str[35];

    WideCharToMultiByte( mCom.mClientCodepage, 0, speed, -1, str, 34, NULL, NULL );
    str[34] = '\0';

    mCom.sendFmiSafeModeSpeed( (float)atof( str ) );

    OnOK();
}    /* OnBnClickedOk() */
