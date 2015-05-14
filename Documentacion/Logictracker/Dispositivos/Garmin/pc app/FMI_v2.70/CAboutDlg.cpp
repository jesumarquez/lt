/*********************************************************************
*
*   MODULE NAME:
*       CAboutDlg.cpp
*
* Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "CAboutDlg.h"

BEGIN_MESSAGE_MAP( CAboutDlg, CDialog )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent window of this dialog
//----------------------------------------------------------------------
CAboutDlg::CAboutDlg
    (
    CWnd * aParent
    )
    : CDialog( IDD_ABOUTBOX, aParent )
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation.  Unused.
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CAboutDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
}

//----------------------------------------------------------------------
//! \brief This function is called when the window is created.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CAboutDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    mGarminLink.SubclassDlgItem( IDC_ABOUT_LBL_URL, this );
    return TRUE;
}
