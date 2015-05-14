/*********************************************************************
*
*   MODULE NAME:
*       CStopListDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CStopListDlg.h"
#include "CStopNewDlg.h"
#include "Event.h"

using namespace std;

IMPLEMENT_DYNAMIC( CStopListDlg, CDialog )

BEGIN_MESSAGE_MAP( CStopListDlg, CDialog )
    ON_BN_CLICKED( IDC_STOPLIST_BTN_NEW_STOP, OnBnClickedNewStop )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
#if( FMI_SUPPORT_A603 )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_STOP_STATUS_CHANGED ), OnEventStopListChanged )
    ON_MESSAGE( WM_EVENT( EVENT_FMI_ETA_RECEIVED ), OnEventEtaReceived )
    ON_CBN_SELCHANGE( IDC_STOPLIST_CBO_UPDATE_STOP, OnCbnSelChangeUpdateOption )
    ON_EN_CHANGE( IDC_STOPLIST_EDIT_MOVETO, OnEnChangeMoveTo )
    ON_BN_CLICKED( IDC_STOPLIST_BTN_SEND, OnBnClickedSend )
#endif
#if( FMI_SUPPORT_A604 )
    ON_BN_CLICKED( IDC_STOPLIST_BTN_SORT, OnBnClickedSort )
#endif
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CStopListDlg::CStopListDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom )
    : CDialog( IDD_STOP_LIST, aParent )
    , mCom( aCom )
#if( FMI_SUPPORT_A603 )
    , mSelectedStopIndex( 0 )
    , mSelectedStopStatus( _T("") )
    , mSelectedUpdateIndex( 0 )
    , mMoveTo( _T("") )
    , mEta( _T("") )
#endif
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CStopListDlg::~CStopListDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CStopListDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

#if( FMI_SUPPORT_A603 )
    DDX_LBIndex( aDataExchange, IDC_STOPLIST_LST_SELECT_STOP, mSelectedStopIndex );
    DDX_Control( aDataExchange, IDC_STOPLIST_LST_SELECT_STOP, mStopListBox );
    DDX_Text( aDataExchange, IDC_STOPLIST_TXT_STATUS, mSelectedStopStatus );
    DDX_CBIndex( aDataExchange, IDC_STOPLIST_CBO_UPDATE_STOP, mSelectedUpdateIndex );
    DDX_Text( aDataExchange, IDC_STOPLIST_EDIT_MOVETO, mMoveTo );
    DDX_Text( aDataExchange, IDC_STOPLIST_TXT_ETA, mEta );
#endif
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     initializes the stop list based on the map owned by the
//!     FmiApplicationLayer, and sets the initial window position.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CStopListDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    SetWindowPos( NULL, 700, 35, 0, 0, SWP_NOSIZE | SWP_NOZORDER );

#if FMI_SUPPORT_A603
    OnEventStopListChanged( 0, NULL );
#endif

#if( !FMI_SUPPORT_A603 )
    GetDlgItem( IDC_STOPLIST_LBL_ETA )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_LBL_SELECT_STOP )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_LBL_STATUS )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_LBL_MOVETO )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_LST_SELECT_STOP )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_BTN_SEND )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_GRP_UPDATE_STOP )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_TXT_ETA )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_TXT_STATUS )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_EDIT_MOVETO )->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPLIST_CBO_UPDATE_STOP )->EnableWindow( FALSE );
#endif

#if( !FMI_SUPPORT_A604 )
    GetDlgItem( IDC_STOPLIST_BTN_SORT )->EnableWindow( FALSE );
#endif

    return TRUE;
}    /* OnInitDialog() */

#if( FMI_SUPPORT_A603 )
//----------------------------------------------------------------------
//! \brief Handler for the Stop List Changed event
//! \details Update the stop list, the status of the selected stop,
//!     and, if the selected stop is active, show the ETA text box.
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg LRESULT CStopListDlg::OnEventStopListChanged( WPARAM, LPARAM )
{
    int i = 0;
    if( !mCom.mStopIndexInList.empty() )
        {
        //must keep track of where the list was scrolled to
        //since we reset content we must reinitialize these
        int selectedIndex = mStopListBox.GetCurSel();
        int topIndex = mStopListBox.GetTopIndex();
        if( selectedIndex == -1 )
            selectedIndex = 0;

        //reset content and then add back current stops
        mStopListBox.ResetContent();
        uint32 stopId = mCom.mStopIndexInList[i];
        while( stopId != INVALID32 )
            {
            mStopListBox.AddString( mCom.mA603Stops.get( stopId ).getCurrentName() );
            i++;
            stopId = mCom.mStopIndexInList[i];
            }

        //update data
        UpdateData( TRUE );
        //display status
        if( mCom.mA603Stops.contains( mCom.mStopIndexInList[selectedIndex] ) )
        {
            StopListItem& item = mCom.mA603Stops.get( mCom.mStopIndexInList[selectedIndex] );

            switch( item.getStopStatus() )
            {
                case STOP_STATUS_ACTIVE:
                    {
                    mSelectedStopStatus.Format( _T(" Active") );
                    OnEventEtaReceived( 0, NULL );
                    }
                    break;
                case STOP_STATUS_DONE:
                    mSelectedStopStatus.Format( _T(" Done") );
                    break;
                case STOP_STATUS_UNREAD:
                    mSelectedStopStatus.Format( _T(" Unread Inactive") );
                    break;
                case STOP_STATUS_READ:
                    mSelectedStopStatus.Format( _T(" Read Inactive") );
                    break;
                case STOP_STATUS_DELETED:
                    mSelectedStopStatus.Format( _T(" Deleted") );
                    break;
                default:
                    mSelectedStopStatus.Format( _T("") );
                    break;
            } // end of switch

            if( item.getStopStatus() == STOP_STATUS_ACTIVE )
                {
                //since the stop is active the ETA should be displayed
                GetDlgItem( IDC_STOPLIST_LBL_ETA )->ShowWindow( SW_SHOW );
                GetDlgItem( IDC_STOPLIST_TXT_ETA )->ShowWindow( SW_SHOW );
                }
            else
                {
                //all the rest aren't active...hide ETA stuff
                GetDlgItem( IDC_STOPLIST_LBL_ETA )->ShowWindow( SW_HIDE );
                GetDlgItem( IDC_STOPLIST_TXT_ETA )->ShowWindow( SW_HIDE );
                }
        }
        UpdateData( FALSE );
        //reset scroll and selection
        mStopListBox.SetCurSel( selectedIndex );
        mStopListBox.SetTopIndex( topIndex );
    }
    else
        mStopListBox.ResetContent();

    return 0;
}    /* OnEventStopListChanged() */

//----------------------------------------------------------------------
//! \brief Selection Change handler for the Update Stop combo box
//! \details If the option selected is Move To, enable the Move To edit
//!     box and Send button; otherwise disable them.
//! \since Protocol A603
//----------------------------------------------------------------------
void CStopListDlg::OnCbnSelChangeUpdateOption()
{
    UpdateData( TRUE );
    if( mSelectedUpdateIndex == REQUEST_MOVE_STOP )
    {
        GetDlgItem( IDC_STOPLIST_LBL_MOVETO )->EnableWindow( TRUE );
        GetDlgItem( IDC_STOPLIST_EDIT_MOVETO )->EnableWindow( TRUE );
        OnEnChangeMoveTo();
    }
    else
    {
        GetDlgItem( IDC_STOPLIST_BTN_SEND )->EnableWindow( TRUE );
        mMoveTo.Format( _T("") );
        GetDlgItem( IDC_STOPLIST_LBL_MOVETO )->EnableWindow( FALSE );
        GetDlgItem( IDC_STOPLIST_EDIT_MOVETO )->EnableWindow( FALSE );
        UpdateData( FALSE );
    }
}    /* OnCbnSelChangeUpdateOption() */

//----------------------------------------------------------------------
//! \brief Edit handler for the Move To text box
//! \details If a valid index is entered into the Move To edit box,
//!     enable the Send button.
//! \since Protocol A603
//----------------------------------------------------------------------
void CStopListDlg::OnEnChangeMoveTo()
{
#if !SKIP_VALIDATION
    UpdateData( TRUE );
    if( mMoveTo != ""                              &&
        _ttoi( mMoveTo ) < mStopListBox.GetCount() &&
        _ttoi( mMoveTo ) >= 0 )
    {
        GetDlgItem( IDC_STOPLIST_BTN_SEND )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_STOPLIST_BTN_SEND )->EnableWindow( FALSE );
    }
#else
    GetDlgItem( IDC_STOPLIST_BTN_SEND )->EnableWindow( TRUE );
#endif
}    /* OnEnChangeMoveTo() */

//----------------------------------------------------------------------
//! \brief Click handler for the Send button
//! \details Send a stop update or request using the Stop Status
//!     protocol.
//! \since Protocol A603
//----------------------------------------------------------------------
void CStopListDlg::OnBnClickedSend()
{
    UpdateData( TRUE );

    if( mStopListBox.GetCurSel() != -1 )
    {
        //If the update is not a move, just send the update. Even if
        //the stop status protocol is throttled, having an out of date
        //status is not a major issue at this point since it can be
        //requested by the user.
        if( mSelectedUpdateIndex != REQUEST_MOVE_STOP )
        {
            uint32 selectedStopId = mCom.mStopIndexInList[mSelectedStopIndex]; //in case it gets deleted
            mCom.sendStopStatusRequest( selectedStopId, (stop_status_status_type) mSelectedUpdateIndex, INVALID16 );
        }
        else
        {
            uint16 moveTo = (uint16)_ttoi( mMoveTo );
            mCom.sendStopMoveRequest( moveTo, (uint16)mSelectedStopIndex );
        }
    }
}    /* OnBnClickedSend() */
#endif

//----------------------------------------------------------------------
//! \brief Click handler for the New Stop button
//! \details Open the New Stop modal dialog.
//----------------------------------------------------------------------
void CStopListDlg::OnBnClickedNewStop()
{
    CStopNewDlg dlg( this, mCom );
    dlg.DoModal();
}    /* OnBnClickedNewStop */

#if( FMI_SUPPORT_A604 )
//----------------------------------------------------------------------
//! \brief Click handler for the Sort button
//! \details Initiate the Stop Sort Request protocol.
//! \since Protocol A604
//----------------------------------------------------------------------
void CStopListDlg::OnBnClickedSort()
{
    mCom.sendStopSortRequest();
}    /* OnBnClickedSort */
#endif

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Destroy the window, since this is a modeless dialog.
//----------------------------------------------------------------------
void CStopListDlg::OnBnClickedOk()
{
    DestroyWindow();
}    /* OnBnClickedOk */

//----------------------------------------------------------------------
//! \brief Handler for the Cancel action
//! \details Destroy the window, since this is a modeless dialog.
//----------------------------------------------------------------------
void CStopListDlg::OnCancel()
{
    DestroyWindow();
}    /* OnCancel() */

//----------------------------------------------------------------------
//! \brief Perform final cleanup.
//! \details Enable the View Stops button on the main window.
//----------------------------------------------------------------------
void CStopListDlg::PostNcDestroy()
{
    CDialog::PostNcDestroy();
    Event::post( EVENT_STOP_LIST_DLG_CLOSED );
}    /* PostNcDestroy() */

#if FMI_SUPPORT_A603
//----------------------------------------------------------------------
//! \brief Handle an ETA Received event from FmiApplicationLayer
//! \details Update the contents of the ETA text box with the ETA time
//!     received from the client.
//! \since Protocol A603
//----------------------------------------------------------------------
afx_msg LRESULT CStopListDlg::OnEventEtaReceived( WPARAM, LPARAM )
{
    TCHAR etaTime[13];

    MultiByteToWideChar( mCom.mClientCodepage, 0, mCom.mEtaTime, -1, etaTime, 13 );
    etaTime[12] = '\0';
    mEta.Format( _T(" %s"), etaTime );
    UpdateData( FALSE );

    return 0;
}
#endif
