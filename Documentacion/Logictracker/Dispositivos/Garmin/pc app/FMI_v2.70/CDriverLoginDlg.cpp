/*********************************************************************
*
*   MODULE NAME:
*       CDriverLoginDlg.cpp
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CDriverLoginDlg.h"

using namespace std;

IMPLEMENT_DYNAMIC( CDriverLoginDlg, CDialog )

BEGIN_MESSAGE_MAP( CDriverLoginDlg, CDialog )
    ON_BN_CLICKED( IDC_DRIVERS_BTN_DELETE, OnBnClickedDelete )
    ON_BN_CLICKED( IDC_DRIVERS_BTN_SET, OnBnClickedSet )
    ON_EN_CHANGE( IDC_DRIVERS_EDIT_ID, OnEnChangeEditBoxes )
    ON_EN_CHANGE( IDC_DRIVERS_EDIT_PASSWORD, OnEnChangeEditBoxes )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_EN_SETFOCUS( IDC_DRIVERS_EDIT_PASSWORD, OnEnSetfocusLoginEdit )
    ON_EN_KILLFOCUS( IDC_DRIVERS_EDIT_PASSWORD, OnEnKillfocusLoginEdit )
    ON_EN_SETFOCUS( IDC_DRIVERS_EDIT_ID, OnEnSetfocusLoginEdit )
    ON_EN_KILLFOCUS( IDC_DRIVERS_EDIT_ID, OnEnKillfocusLoginEdit )
    ON_LBN_SELCHANGE( IDC_DRIVERS_LST_DRIVERS, OnLbnSelchangeDriverList )
    ON_LBN_SETFOCUS( IDC_DRIVERS_LST_DRIVERS, OnLbnSetfocusDriverList )
    ON_LBN_KILLFOCUS( IDC_DRIVERS_LST_DRIVERS, OnLbnKillfocusDriverList )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param  aParent The parent window.
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CDriverLoginDlg::CDriverLoginDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_DRIVER_LOGINS, aParent )
    , mCom( aCom )
    , mSelectedIndex( -1 )
    , mDriverId( _T("") )
    , mDriverPassword( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CDriverLoginDlg::~CDriverLoginDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CDriverLoginDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Control( aDataExchange, IDC_DRIVERS_LST_DRIVERS, mListBox );
    DDX_Text( aDataExchange, IDC_DRIVERS_EDIT_ID, mDriverId );
    DDX_Text( aDataExchange, IDC_DRIVERS_EDIT_PASSWORD, mDriverPassword );
}

//----------------------------------------------------------------------
//! \brief This function is called when the window is created.
//! \details This function is called when the window is created. It
//!     initializes the canned response list, and sets the initial
//!     position of the window.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CDriverLoginDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    updateListBox();

    GetDlgItem( IDC_DRIVERS_BTN_DELETE )->EnableWindow( FALSE );

    SetWindowPos( NULL, 700, 350, 0, 0, SWP_NOSIZE | SWP_NOZORDER );

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Update the canned response list box from the canned response
//!     map owned by FmiApplicationLayer.
//----------------------------------------------------------------------
void CDriverLoginDlg::updateListBox()
{
    CString listItem;

    //must keep track of where the list was scrolled to
    //since we reset content we must reinitialize these
    mSelectedIndex = mListBox.GetCurSel();
    int topIndex = mListBox.GetTopIndex();

    //reset content and then add current canned responses
    mListBox.ResetContent();
    FileBackedMap<DriverLoginItem>::const_iterator iter = mCom.mDriverLogins.begin();
    for( ; iter != mCom.mDriverLogins.end(); iter++ )
    {
        if( iter->second.isValid() )
        {
            listItem.Format( _T("%s"), iter->second.getDriverId() );
            mListBox.AddString( listItem );
        }
    }

    //reset scroll and selection
    mSelectedIndex = -1;

    //reset scroll and selection
    mListBox.SetCurSel( mSelectedIndex );
    mListBox.SetTopIndex( topIndex );

}   /* updateListBox() */

//----------------------------------------------------------------------
//! \brief Button handler for the Delete button
//! \details Remove the selected driver from the allowed logins.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnBnClickedDelete()
{
    UpdateData( TRUE );
    if( mSelectedIndex >= 0 )
        {
        mCom.mDriverLogins.remove( mCom.mDriverLogins.getKeyAt( mSelectedIndex ) );
        updateListBox();
        }

    if( mSelectedIndex < 0 )
    {
        GetDlgItem( IDC_DRIVERS_BTN_DELETE )->EnableWindow( FALSE );
    }

}   /* OnBnClickedDelete() */

//----------------------------------------------------------------------
//! \brief Button handler for the Set button
//! \details Saves the new driver password.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnBnClickedSet()
{
    UpdateData( TRUE );
    char driverId[50];
    char driverPassword[20];

    WideCharToMultiByte( CP_UTF8, 0, mDriverId, -1, driverId, sizeof( driverId ), NULL, NULL );
    WideCharToMultiByte( CP_UTF8, 0, mDriverPassword, -1, driverPassword, sizeof( driverPassword ), NULL, NULL );

    DriverLoginItem & login = mCom.mDriverLogins.get( driverId );
    login.setPassword( driverPassword );

    mCom.mDriverLogins.put( login );

    updateListBox();

}   /* OnBnClickedSet() */

//----------------------------------------------------------------------
//! \brief Edit handler for the Driver ID and Driver Password boxes.
//! \details If either box is empty, disables the Set button; if both
//!     ID and password are specified, enables the Set button.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnEnChangeEditBoxes()
{
    UpdateData( TRUE );
    if( mDriverId != "" && mDriverPassword != "" )
    {
        GetDlgItem( IDC_DRIVERS_BTN_SET )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_DRIVERS_BTN_SET )->EnableWindow( FALSE );
    }
}   /* OnEnChangeEditBoxes() */


//----------------------------------------------------------------------
//! \brief Selection Changed handler for the Driver List box.
//! \details Fills in the driver ID and password fields of the
//!     dialog with the information from the selected list item, for
//!     easy editing.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnLbnSelchangeDriverList()
{
    CListBox * driverListBox = (CListBox*) GetDlgItem( IDC_DRIVERS_LST_DRIVERS );
    mSelectedIndex = driverListBox->GetCurSel();

    if( mSelectedIndex >= 0 && mSelectedIndex < driverListBox->GetCount() )
    {
        DriverLoginItem& item = mCom.mDriverLogins.get( mCom.mDriverLogins.getKeyAt( mSelectedIndex ) );

        mDriverId = item.getDriverId();
        mDriverPassword = item.getPassword();

        UpdateData( FALSE );
        GetDlgItem( IDC_DRIVERS_BTN_DELETE )->EnableWindow( TRUE );

        OnEnChangeEditBoxes();
    }
    else
    {
        GetDlgItem( IDC_DRIVERS_BTN_DELETE )->EnableWindow( FALSE );
    }

    driverListBox->SetCurSel( mSelectedIndex );
}


//----------------------------------------------------------------------
//! \brief Button handler for the OK button
//! \details Closes the window.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnBnClickedOk()
{
    DestroyWindow();
    //not modal so don't call OnOK()
}   /* OnBnClickedOk */

//----------------------------------------------------------------------
//! \brief Handler for the Cancel action
//! \details Closes the window.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnCancel()
{
    DestroyWindow();
}   /* OnCancel */

//----------------------------------------------------------------------
//! \brief Called by MFC after the window has been destroyed; performs
//!     final termination activities.
//----------------------------------------------------------------------
void CDriverLoginDlg::PostNcDestroy()
{
    CDialog::PostNcDestroy();
}   /* PostNcDestroy() */

//----------------------------------------------------------------------
//! \brief Handles the set focus event for the driver ID and password
//!     edit boxes.
//! \details Sets the default control to the Set button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnEnSetfocusLoginEdit()
{
    SendMessage( DM_SETDEFID, IDC_DRIVERS_BTN_SET );
}   /* OnEnSetfocusLoginEdit */

//----------------------------------------------------------------------
//! \brief Handles the kill focus event for the driver ID and password
//!     edit boxes.
//! \details Sets the default control to the OK button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnEnKillfocusLoginEdit()
{
    SendMessage( DM_SETDEFID, IDOK );
}   /* OnEnKillfocusLoginEdit */

//----------------------------------------------------------------------
//! \brief Handles the set focus event for the driver list.
//! \details Sets the default control to the Delete button so that it
//!     is activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnLbnSetfocusDriverList()
{
    SendMessage( DM_SETDEFID, IDC_DRIVERS_BTN_DELETE );
}   /* OnLbnSetfocusDriverList */

//----------------------------------------------------------------------
//! \brief Handles the kill focus event for the driver list.
//! \details Sets the default control to the OK button so that it is
//!     activated if the user presses the Enter key.
//----------------------------------------------------------------------
void CDriverLoginDlg::OnLbnKillfocusDriverList()
{
    SendMessage( DM_SETDEFID, IDOK );
}   /* OnLbnKillfocusDriverList */
