/*********************************************************************
*
*   MODULE NAME:
*       CWaypointDlg.cpp
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "CFmiApplication.h"
#include "CWaypointDlg.h"
#include "Event.h"

IMPLEMENT_DYNAMIC( CWaypointDlg, CDialog )

BEGIN_MESSAGE_MAP( CWaypointDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_BN_CLICKED( IDC_WPT_BTN_SEND, OnBnClickedWptBtnSend )
    ON_EN_CHANGE( IDC_WPT_EDIT_WPT_ID, OnEnChangeWptEdit )
    ON_EN_CHANGE( IDC_WPT_EDIT_WPT_NAME, OnEnChangeWptEdit )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_WAYPOINT_LIST_CHANGED ), OnWaypointListChanged )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_CATEGORY_LIST_CHANGED ), OnCategoryListChanged )
    ON_BN_CLICKED( IDC_WPT_BTN_DELETE, OnBnClickedWptBtnDelete )
    ON_BN_CLICKED( IDC_WPT_BTN_DELETE_CAT, OnBnClickedWptBtnDeleteCat )
    ON_BN_CLICKED( IDC_WPT_BTN_CREATE_CAT, OnBnClickedWptBtnCreateCat )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CWaypointDlg::CWaypointDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_WAYPOINT, aParent )
      , mCom( aCom )
      , mWptId( 0 )
      , mLat( 0 )
      , mLon( 0 )
      , mSymbol( 0 )
      , mCat( _T("") )
      , mName( _T("") )
      , mComment( _T("") )
      , mCatId( 0 )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CWaypointDlg::~CWaypointDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CWaypointDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_WPT_ID, mWptId );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_WPT_NAME, mName );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_CAT_NAME, mCat );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_CAT_ID, mCatId );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_LAT, mLat );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_LON, mLon );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_SYM_ID, mSymbol );
    DDX_Text( aDataExchange, IDC_WPT_EDIT_WPT_COMMENT, mComment );
    DDX_Control( aDataExchange, IDC_WPT_LST_WAYPOINTS, mListBox );
    DDX_Control( aDataExchange, IDC_WPT_LST_CATEGORIES, mCatBox );
}

//----------------------------------------------------------------------
//! \brief Perform final cleanup.
//! \details Enable the Waypoints button on the main window.
//----------------------------------------------------------------------
void CWaypointDlg::PostNcDestroy()
{
    CDialog::PostNcDestroy();
    Event::post( EVENT_FMI_WAYPOINT_DLG_CLOSED );
}    /* PostNcDestroy() */

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets the window position, updates the Waypoints and Categories
//!     list boxes, and disables the appropriate controls.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CWaypointDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    SetWindowPos( NULL, 700, 35, 0, 0, SWP_NOSIZE | SWP_NOZORDER );

    updateCatBox();
    updateListBox();

    GetDlgItem( IDC_WPT_BTN_SEND )->EnableWindow( FALSE );

    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Update the waypoint list box from the map owned by Com.
//----------------------------------------------------------------------
void CWaypointDlg::updateListBox()
{
    CString temp;

    //must keep track of where the list was scrolled to
    //since we reset content we must reinitialize these
    int selectedIndex = mListBox.GetCurSel();
    int topIndex = mListBox.GetTopIndex();

    //reset content and add current canned messages to list
    mListBox.ResetContent();
    FileBackedMap<WaypointListItem>::const_iterator it;

    for( it = mCom.mWaypoints.begin();
         it != mCom.mWaypoints.end();
         it++ )
    {
        if( it->second.isValid() )
        {
            temp.Format( _T("%d - %s"), it->first, it->second.getCurrentName() );
            mListBox.AddString( temp );
        }
    }
    //reset scroll and selection
    mListBox.SetCurSel( selectedIndex );
    mListBox.SetTopIndex( topIndex );
}    /* updateListBox() */

//----------------------------------------------------------------------
//! \brief Update the waypoint list box from the map owned by Com.
//----------------------------------------------------------------------
void CWaypointDlg::updateCatBox()
{
    CString temp;

    //must keep track of where the list was scrolled to
    //since we reset content we must reinitialize these
    int selectedIndex = mCatBox.GetCurSel();
    int topIndex = mCatBox.GetTopIndex();

    //reset content and add current canned messages to list
    mCatBox.ResetContent();
    FileBackedMap<ClientListItem>::const_iterator it;

    for( it = mCom.mCategories.begin();
         it != mCom.mCategories.end();
         it++ )
    {
        if( it->second.isValid() )
        {
            temp.Format( _T("%d - %s"), it->first, it->second.getCurrentName() );
            mCatBox.AddString( temp );
        }
    }
    //reset scroll and selection
    mCatBox.SetCurSel( selectedIndex );
    mCatBox.SetTopIndex( topIndex );
}

//----------------------------------------------------------------------
//! \brief Handler for EVENT_FMI_WAYPOINT_LIST_CHANGED event
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LPARAM CWaypointDlg::OnWaypointListChanged( WPARAM, LPARAM )
{
    updateListBox();
    return 0;
}

//----------------------------------------------------------------------
//! \brief Handler for EVENT_FMI_CATEGORY_LIST_CHANGED event
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LPARAM CWaypointDlg::OnCategoryListChanged( WPARAM, LPARAM )
{
    updateCatBox();
    return 0;
}

//----------------------------------------------------------------------
//! \brief Get category bit field value for the selected category IDs
//! \details Form a bit field with bits set corresponding to the
//!     selected category IDs.  For example, if categories with IDs 0
//!     and 5, are selected, the bit field will have the lowest bit
//!     and the 6th lowest bit set to 1 for a value of 33 decimal.
//! \return The category bit field
//----------------------------------------------------------------------
uint16 CWaypointDlg::getCatIds()
{
    uint16 selIds = 0;
    int selList[16];
    int count;
    count = mCatBox.GetSelItems( 16, selList );

    for( int i = 0; i < count; ++i )
    {
        int selectedIndex = selList[i];
        uint8 catId = mCom.mCategories.getKeyAt( selectedIndex );
        selIds |= setbit( catId );
    }

    return selIds;
}

// CWaypointDlg message handlers

//----------------------------------------------------------------------
//! \brief Handler for the OK button
//! \details Destroy the window
//----------------------------------------------------------------------
void CWaypointDlg::OnBnClickedOk()
{
    DestroyWindow();
}

//----------------------------------------------------------------------
//! \brief Handler for the Send Waypoint button
//! \details Initiate the Create Waypoint Protocol
//----------------------------------------------------------------------
void CWaypointDlg::OnBnClickedWptBtnSend()
{
    UpdateData( TRUE );

    mCom.sendWaypoint( mWptId, mLat, mLon, mSymbol, mName, getCatIds(), mComment );
}

//----------------------------------------------------------------------
//! \brief Edit handler for the waypoint edit boxes on this dialog.
//! \details Enable the Send Waypoint button if the waypoint name is
//!     specified; disable it otherwise.
//----------------------------------------------------------------------
void CWaypointDlg::OnEnChangeWptEdit()
{
    UpdateData( TRUE );
    if( mName != "" )
    {
        GetDlgItem( IDC_WPT_BTN_SEND )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_WPT_BTN_SEND )->EnableWindow( FALSE );
    }
}

//----------------------------------------------------------------------
//! \brief Handler for the Delete Waypoint button
//! \details Initiate the Delete Waypoint Protocol
//----------------------------------------------------------------------
void CWaypointDlg::OnBnClickedWptBtnDelete()
{
    UpdateData( TRUE );
    int selIdx = mListBox.GetCurSel();
    if( selIdx >= 0 )
    {
        mCom.sendDeleteWaypoint( mCom.mWaypoints.getKeyAt( selIdx ) );
    }
}

//----------------------------------------------------------------------
//! \brief Handler for the Delete Category button
//! \details Initiate the Delete Waypoints by Category Protocol
//----------------------------------------------------------------------
void CWaypointDlg::OnBnClickedWptBtnDeleteCat()
{
    UpdateData( TRUE );

    mCom.sendDeleteWaypointCat( getCatIds() );
}

//----------------------------------------------------------------------
//! \brief Handler for the Create Category button
//! \details Initiate the Create Waypoint Category Protocol
//----------------------------------------------------------------------
void CWaypointDlg::OnBnClickedWptBtnCreateCat()
{
    UpdateData( TRUE );
    mCom.sendCreateWaypointCat( mCatId, mCat );
}
