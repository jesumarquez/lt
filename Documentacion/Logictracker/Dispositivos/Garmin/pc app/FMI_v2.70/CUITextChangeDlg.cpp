/*********************************************************************
*
*   MODULE NAME:
*       CUITextChangeDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CUITextChangeDlg.h"

IMPLEMENT_DYNAMIC( CUITextChangeDlg, CDialog )

BEGIN_MESSAGE_MAP( CUITextChangeDlg, CDialog )
    ON_EN_CHANGE( IDC_UITEXT_EDIT_NEWTEXT, OnChange )
    ON_CBN_SELCHANGE( IDC_UITEXT_CBO_ITEM, OnChange )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CUITextChangeDlg::CUITextChangeDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_UI_TEXT_CHANGE, aParent )
    , mCom( aCom )
    , mListIndex( 0 )
    , mNewText( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CUITextChangeDlg::~CUITextChangeDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CUITextChangeDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_CBIndex( aDataExchange, IDC_UITEXT_CBO_ITEM, mListIndex );
    DDX_Text( aDataExchange, IDC_UITEXT_EDIT_NEWTEXT, mNewText );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CUITextChangeDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Change handler for all controls on the dialog.
//! \details Enable the OK button if an item is selected and the new
//!     text edit box is not blank
//----------------------------------------------------------------------
void CUITextChangeDlg::OnChange()
{
    BOOL formValid = TRUE;

#if !SKIP_VALIDATION
    UpdateData( TRUE );
    if( mListIndex == -1 || mNewText.GetLength() == 0 || mNewText.GetLength() >= 50 )
        formValid = FALSE;
#endif

    if( formValid )
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    else
        GetDlgItem( IDOK )->EnableWindow( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Initiate the User Interface Text Change protocol using the
//!     information entered on this dialog, then close this dialog box.
//----------------------------------------------------------------------
void CUITextChangeDlg::OnBnClickedOk()
{
    char    uiText[50];

    UpdateData( TRUE );

    WideCharToMultiByte( mCom.mClientCodepage, 0, mNewText, -1, uiText, 50, NULL, NULL );
    uiText[49] = '\0';

    mCom.sendUserInterfaceText( mListIndex, uiText );
    OnOK();
}
