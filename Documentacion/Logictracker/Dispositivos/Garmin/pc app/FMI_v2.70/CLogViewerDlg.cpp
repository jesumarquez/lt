/*********************************************************************
*
*   MODULE NAME:
*       CLogViewerDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "CLogViewerDlg.h"
#include "Logger.h"
#include "SerialPort.h"

#define EDGE_SPACING 7

using namespace std;

IMPLEMENT_DYNAMIC( CLogViewerDlg, CDialog )

BEGIN_MESSAGE_MAP( CLogViewerDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_WM_SIZE()
    ON_WM_GETMINMAXINFO()
    ON_LBN_SELCHANGE( IDC_LOG_LST_PACKETS, OnLbnSelchangeLog )
    ON_BN_CLICKED( IDC_LOG_BTN_VIEW_OTHER, OnBnClickedViewOther )
    ON_BN_CLICKED( IDC_LOG_BTN_VIEW_CURRENT, OnBnClickedViewCurrent )
    ON_BN_CLICKED( IDC_LOG_BTN_CLEAR, OnBnClickedClearLog )
    ON_BN_CLICKED( IDC_LOG_BTN_SAVE_AS, OnBnClickedSaveAs )
    ON_BN_CLICKED( IDC_LOG_BTN_FIND_NEXT, OnBnClickedFindNext )
    ON_MESSAGE( WM_EVENT( EVENT_LOG_PACKET ), OnPacketLogged )
    #if( FMI_SUPPORT_A602 )
    ON_BN_CLICKED( IDC_LOG_BTN_RESEND, resendPacket )
    #endif
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aLogParser The LogParser which will format log entries
//! \param aParentWnd The parent of this dialog
//! \param aOpenOtherLog If TRUE, the dialog starts by presenting a
//!     File/Open dialog to allow the user to select another
//!     (i.e., saved, not the current) packet log to view.
//----------------------------------------------------------------------
CLogViewerDlg::CLogViewerDlg
    (
    LogParser * aLogParser,
    CWnd      * aParentWnd,
    BOOL        aOpenOtherLog
    )
    : CDialog( IDD_LOG, aParentWnd )
    , mSelectedPacketText( _T("") )
    , mSearchDirection( SEARCH_DOWN )
    , mLogParser( aLogParser )
{
mOpenOtherLog = aOpenOtherLog;
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CLogViewerDlg::~CLogViewerDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CLogViewerDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_LOG_TXT_SELECTED_PACKET, mSelectedPacketText );
    DDX_Text( aDataExchange, IDC_LOG_EDIT_SEARCH_TEXT, mSearchText );
    DDX_Control( aDataExchange, IDC_LOG_LST_PACKETS, mPacketListBox );
    DDX_Control( aDataExchange, IDC_LOG_TXT_SELECTED_PACKET, mCurrentPacketControl );
    DDX_Control( aDataExchange, IDOK, mCloseButton );
    DDX_Control( aDataExchange, IDC_LOG_BTN_VIEW_OTHER, mViewOtherButton );
    DDX_Control( aDataExchange, IDC_LOG_BTN_VIEW_CURRENT, mViewCurrentButton );
    DDX_Control( aDataExchange, IDC_LOG_LBL_SELECTED_PACKET, mCurrentPacketTitleLabel );
    DDX_Control( aDataExchange, IDC_LOG_LBL_PACKETS, mPacketListTitleLabel );
    DDX_Control( aDataExchange, IDC_LOG_LBL_LOG_NAME, mLogNameTitleLabel );
    DDX_Control( aDataExchange, IDC_LOG_TXT_LOG_NAME, mLogNameControl );
    DDX_Control( aDataExchange, IDC_LOG_BTN_SAVE_AS, mSaveButton );
    DDX_Control( aDataExchange, IDC_LOG_BTN_CLEAR, mClearButton );
    DDX_Control( aDataExchange, IDC_LOG_BTN_FIND_NEXT, mFindNextButton );
    DDX_Control( aDataExchange, IDC_LOG_RDO_UP, mSearchUpRadioButton );
    DDX_Control( aDataExchange, IDC_LOG_RDO_DOWN, mSearchDownRadioButton );
    DDX_Control( aDataExchange, IDC_LOG_EDIT_SEARCH_TEXT, mSearchTextControl );
    DDX_Control( aDataExchange, IDC_LOG_GRP_SEARCH, mSearchGroupBox );
    DDX_Control( aDataExchange, IDC_LOG_BTN_RESEND, mResendButton );
    DDX_Radio( aDataExchange, IDC_LOG_RDO_UP, mSearchDirection );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to Com.  It also registers to receive events from Com, and
//!     initializes the maps of packet and command IDs to names for
//!     simplified lookup.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CLogViewerDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    if( mOpenOtherLog )
    {
        mOpenOtherLog = FALSE;
        OnBnClickedViewOther();
    }
    else
    {
        mLogParser->init( CString( Logger::LOG_FILE ) );
        resetView();
        UpdateLogDisplay();
    }
    UpdateData( FALSE );
    SetWindowPos( NULL, 500, 350, 0, 0, SWP_NOZORDER );
    return TRUE;
}    /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Handles the Packet Logged event from Com; updates the
//!     packet list.
//! \return 0, always
//----------------------------------------------------------------------
afx_msg LPARAM CLogViewerDlg::OnPacketLogged( WPARAM, LPARAM )
{
    UpdateLogDisplay();
    return 0;
}    /* OnPacketLogged() */

//----------------------------------------------------------------------
//! \brief Reads the log file and updates the packet list.  If a
//!     packet was selected, keeps the packet selected and visible in
//!     the list.
//----------------------------------------------------------------------
void CLogViewerDlg::UpdateLogDisplay()
{
    BOOL scroll = FALSE;
    int maxScroll;
    int scrolledTo;

    scrolledTo = mPacketListBox.GetScrollPos( SB_VERT );
    maxScroll = mPacketListBox.GetScrollLimit( SB_VERT );

    if( scrolledTo == maxScroll )
    {
        scroll = TRUE;
    }

    if( updateView() && scroll )
    {
        mPacketListBox.SetTopIndex( mPacketListBox.GetCount() - 1 );
    }
}

//----------------------------------------------------------------------
//! \brief Update the log view
//! \details Retrieve any new packet titles from the log parser and
//!     add them to the list box.
//----------------------------------------------------------------------
BOOL CLogViewerDlg::updateView()
{
    UpdateData( TRUE );

    mLogParser->readLog();
    while( mPacketListBox.GetCount() < mLogParser->getLineCount() )
    {
        mPacketListBox.AddString( mLogParser->getPacketTitle( mPacketListBox.GetCount() ) );
    }

    UpdateData( FALSE );

    return TRUE;
}

//----------------------------------------------------------------------
//! \brief Click handler for the OK button; destroys the window.
//----------------------------------------------------------------------
void CLogViewerDlg::OnBnClickedOk()
{
    DestroyWindow();
    //not modal so don't call OnOK()
}    /* OnBnClickedOk */

//----------------------------------------------------------------------
//! \brief Handler for the Cancel action; destroys the window.
//----------------------------------------------------------------------
void CLogViewerDlg::OnCancel()
{
    DestroyWindow();
}    /* OnCancel */

//----------------------------------------------------------------------
//! \brief Perform final cleanup on the log viewer.
//! \details This is a "monitor", so it is modeless.  When it gets the
//!     destroy message it must re-enable the main button to open these
//!     dialogs (the button was disabled when this window opened to
//!     prevent several instances of the log viewer from being
//!     displayed.  Finally, this function deletes the CLogViewerDlg
//!     object since the pointer to it was lost after creation.
//----------------------------------------------------------------------
void CLogViewerDlg::PostNcDestroy()
{
    CDialog::PostNcDestroy();

    Event::post( EVENT_LOG_VIEWER_CLOSED );
}    /* PostNcDestroy() */

//----------------------------------------------------------------------
//! \brief Called after the dialog is resized; repositions the contents
//!     of the display.
//! \details Moves/sizes the contents as follows:
//!     - Moves the search box to the bottom-left corner, keeping its
//!       size
//!     - Moves the packet list to the top-left, expands vertically
//!     - Moves the packet detail to the right of the list, expands
//!       horizontally to fill the dialog and vertically to match the
//!       height of the packet list
//!     - Centers the buttons under the packet detail.
//! \param aType The type of resizing requested (maximized, etc.)
//! \param aClientWidth The new width of the client area
//! \param aClientHeight The new height of the client area
//----------------------------------------------------------------------
void CLogViewerDlg::OnSize
    (
    UINT aType,
    int  aClientWidth,
    int  aClientHeight
    )
{
    CDialog::OnSize( aType, aClientWidth, aClientHeight );

    //check to make sure dialog is initialized...
    //only need to check one field
    if( mPacketListTitleLabel.GetSafeHwnd() != NULL )
    {
        UpdateData( FALSE );
        //-----------------------------------------------------------------
        //move the search group box
        CRect searchGroupRect;
        mSearchGroupBox.GetClientRect( &searchGroupRect );
        searchGroupRect.MoveToX( EDGE_SPACING );
        searchGroupRect.MoveToY( aClientHeight - searchGroupRect.Height() - EDGE_SPACING );
        mSearchGroupBox.MoveWindow( searchGroupRect );

        //move radio buttons
        CRect upRadioRect;
        mSearchUpRadioButton.GetClientRect( &upRadioRect );
        upRadioRect.MoveToX( searchGroupRect.CenterPoint().x - upRadioRect.Width() - 2 );
        upRadioRect.MoveToY( searchGroupRect.top + 10 );
        mSearchUpRadioButton.MoveWindow( upRadioRect );

        CRect downRadioRect;
        mSearchDownRadioButton.GetClientRect( &downRadioRect );
        downRadioRect.MoveToX( searchGroupRect.CenterPoint().x + 2 );
        downRadioRect.MoveToY( upRadioRect.top );
        mSearchDownRadioButton.MoveWindow( downRadioRect );

        //move edit box
        CRect findEditRect;
        mSearchTextControl.GetWindowRect( &findEditRect );
        findEditRect.MoveToX( searchGroupRect.left + EDGE_SPACING );
        findEditRect.MoveToY( upRadioRect.bottom + 2 );
        mSearchTextControl.MoveWindow( findEditRect );

        //move 'find next' button
        CRect findNextRect;
        mFindNextButton.GetWindowRect( &findNextRect );
        findNextRect.MoveToXY( findEditRect.right + EDGE_SPACING, findEditRect.top );
        mFindNextButton.MoveWindow( findNextRect );

        //-----------------------------------------------------------------
        //resize the packet list
        CRect packetListTitleRect;
        mPacketListTitleLabel.GetWindowRect( &packetListTitleRect );
        packetListTitleRect.MoveToXY( EDGE_SPACING, EDGE_SPACING );
        mPacketListTitleLabel.MoveWindow( packetListTitleRect );

        CRect packetListRect;
        mPacketListBox.GetWindowRect( &packetListRect );
        packetListRect.MoveToXY( EDGE_SPACING, packetListTitleRect.bottom + 2 );
        packetListRect.bottom = searchGroupRect.top - 15;       // consume available height
        mPacketListBox.MoveWindow( packetListRect );

        //-----------------------------------------------------------------
        //move the "Viewing:" text to be bottom-aligned with the list box
        CRect logNameTitleRect;
        mLogNameTitleLabel.GetWindowRect( &logNameTitleRect );
        logNameTitleRect.MoveToXY
            (
            packetListRect.right + 15,
            packetListRect.bottom - logNameTitleRect.Height()
            );
        mLogNameTitleLabel.MoveWindow( logNameTitleRect );

        CRect logNameRect;
        mLogNameControl.GetWindowRect( &logNameRect );
        logNameRect.MoveToXY( logNameTitleRect.right + 4, logNameTitleRect.top );
        logNameRect.right = aClientWidth - EDGE_SPACING;                             // consume available space
        mLogNameControl.MoveWindow( logNameRect );

        //-----------------------------------------------------------------
        //move the text above the packet information window
        //resize and move the packet information window
        CRect currentPacketTitleRect;
        mCurrentPacketTitleLabel.GetWindowRect( &currentPacketTitleRect );
        currentPacketTitleRect.MoveToXY( packetListRect.right + 15, EDGE_SPACING );
        mCurrentPacketTitleLabel.MoveWindow( currentPacketTitleRect );

        CRect currentPacketRect;
        mCurrentPacketControl.GetWindowRect( &currentPacketRect );
        currentPacketRect.MoveToXY( currentPacketTitleRect.left, packetListRect.top );
        currentPacketRect.right = aClientWidth - EDGE_SPACING;
        currentPacketRect.bottom = logNameRect.top - EDGE_SPACING;
        mCurrentPacketControl.MoveWindow( currentPacketRect );

        mLogParser->setRenderWidth( currentPacketRect.Width() );

        //-----------------------------------------------------------------
        // move the buttons; these are in a 3x2 grid with half-button-width
        // padding on each side, and equal spacing
        CRect buttonGroupRect;
        buttonGroupRect.SetRect( currentPacketRect.left, searchGroupRect.top, aClientWidth - EDGE_SPACING, aClientHeight - EDGE_SPACING );

        CRect buttonRect;
        mCloseButton.GetWindowRect( &buttonRect );
        int buttonSpacing = ( buttonGroupRect.Width() - 4 * buttonRect.Width() ) / 4;

        buttonRect.MoveToXY
            (
            buttonGroupRect.CenterPoint().x - 3 * buttonRect.Width() / 2 - buttonSpacing,
            buttonGroupRect.top
            );
        mResendButton.MoveWindow( buttonRect );

        buttonRect.MoveToXY
            (
            buttonGroupRect.CenterPoint().x - buttonRect.Width() / 2,
            buttonGroupRect.top
            );
        mClearButton.MoveWindow( buttonRect );

        buttonRect.MoveToXY
            (
            buttonGroupRect.CenterPoint().x + buttonRect.Width() / 2  + buttonSpacing,
            buttonGroupRect.top
            );
        mSaveButton.MoveWindow( buttonRect );

        buttonRect.MoveToXY
            (
            buttonGroupRect.CenterPoint().x - 3 * buttonRect.Width() / 2 - buttonSpacing,
            buttonGroupRect.bottom - buttonRect.Height()
            );
        mViewOtherButton.MoveWindow( buttonRect );

        buttonRect.MoveToXY
            (
            buttonGroupRect.CenterPoint().x - buttonRect.Width() / 2,
            buttonGroupRect.bottom - buttonRect.Height()
            );
        mViewCurrentButton.MoveWindow( buttonRect );

        buttonRect.MoveToXY
            (
            buttonGroupRect.CenterPoint().x + buttonRect.Width() / 2  + buttonSpacing,
            buttonGroupRect.bottom - buttonRect.Height()
            );
        mCloseButton.MoveWindow( buttonRect );

        OnLbnSelchangeLog();    //packet window needs to be reformatted

        this->RedrawWindow();
    }
}

//----------------------------------------------------------------------
//! \brief Called on every resize to get the resize bounds.
//! \details Sets minTrackSize to the minimum dimensions the window can
//!     be resized to.
//! \param aMinMaxInfo Pointer to the MINMAXINFO data structure that
//!     this function updates
//----------------------------------------------------------------------
void CLogViewerDlg::OnGetMinMaxInfo
    (
    MINMAXINFO * aMinMaxInfo
    )
{
    aMinMaxInfo->ptMinTrackSize.x = 600;
    aMinMaxInfo->ptMinTrackSize.y = 400;
}

//----------------------------------------------------------------------
//! \brief Retransmit the selected packet for debugging purposes.
//! \note This is currently limited to the active log.
//----------------------------------------------------------------------
void CLogViewerDlg::resendPacket()
{
    int selectedIndex = mPacketListBox.GetCurSel();
    mLogParser->resendPacket( selectedIndex );
}

//----------------------------------------------------------------------
//! \brief Selection Changed handler for the packet list.
//! \details Gets the currently selected packet and formats it into the
//!     text box.  Enables the resend button if a packet is selected
//!     and the app is connected to a client.
//----------------------------------------------------------------------
void CLogViewerDlg::OnLbnSelchangeLog()
{
    UpdateData( TRUE );

    int selectedIndex = mPacketListBox.GetCurSel();
    if( selectedIndex >= 0 && selectedIndex < mPacketListBox.GetCount() )
    {
        mSelectedPacketText = mLogParser->getPacketDetail( selectedIndex );
        if( SerialPort::getInstance()->isOpen() )
        {
            mResendButton.EnableWindow( TRUE );
        }
    }

    UpdateData( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the View Other button.
//! \details Prompt the user (with a FileDialog) to select another
//!     packet log to view then update the log display.
//----------------------------------------------------------------------
void CLogViewerDlg::OnBnClickedViewOther()
{
    TCHAR workingDirectory[200];
    // opening a file in another directory changes the current
    // directory, which will cause problems because the log and data
    // files are opened relative to the current directory.  So, get the
    // directory now so that it can be restored when the user is done
    // picking a file.
    DWORD returnValue = GetCurrentDirectory( 200, workingDirectory );
    if( returnValue == 0 || returnValue > 200 )
    {
        MessageBox( _T("Unable to get current directory"), _T("Severe Error") );
        OnCancel();
        return;
    }
    CFileDialog dlg
        (
        TRUE,
        _T("log"),
        NULL,
        OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
        _T("Log Files (*.log)|*.log||")
        );
    if( dlg.DoModal() == IDOK )
    {
        mLogParser->init( dlg.GetPathName() );

        resetView();
        UpdateLogDisplay();
    }
    SetCurrentDirectory( workingDirectory );

}    /* OnBnClickedViewOther */

//----------------------------------------------------------------------
//! \brief Reset the log view
//! \details Clear the packet list and packet detail, and update the
//!     log name.
//----------------------------------------------------------------------
void CLogViewerDlg::resetView()
{
    UpdateData( TRUE );
    mPacketListBox.ResetContent();
    mPacketListBox.SetTopIndex( mPacketListBox.GetCount() - 1 );

    mSelectedPacketText = _T("");
    if( mLogParser->getFilename() == Logger::LOG_FILE )
    {
        mLogNameText.Format( _T(" Current Execution's Packet Log") );
        mLogNameControl.SetWindowText( mLogNameText );
        mClearButton.EnableWindow( TRUE );
    }
    else
    {
        mLogNameText.Format( _T(" %s"), mLogParser->getFilename() );
        mLogNameControl.SetWindowText( mLogNameText );
        mClearButton.EnableWindow( FALSE );
    }

    mResendButton.EnableWindow( FALSE );
    UpdateData( FALSE );
}

//----------------------------------------------------------------------
//! \brief Click handler for the View Current button.
//! \details Switch to the default (current) packet log and update the
//!     log display.
//----------------------------------------------------------------------
void CLogViewerDlg::OnBnClickedViewCurrent()
{
    if( mLogParser->getFilename() == Logger::LOG_FILE )
    {
        return; //already viewing current log
    }

    mLogParser->init( CString( Logger::LOG_FILE ) );

    resetView();
    UpdateLogDisplay();
}    /* OnBnClickedViewCurrent */

//----------------------------------------------------------------------
//! \brief Click handler for the Clear Log button.
//! \details Clear (remove all packets from) the log file; also resets
//!     the packet list in the UI.
//----------------------------------------------------------------------
void CLogViewerDlg::OnBnClickedClearLog()
{
    Logger::clearLog();
    mLogParser->init( CString( Logger::LOG_FILE ) );

    resetView();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Save As button.
//! \details Presents the user with a File Save dialog then saves the
//!     currently viewed log to the user-specified filename.
//----------------------------------------------------------------------
void CLogViewerDlg::OnBnClickedSaveAs()
{
    int filenameLength = WideCharToMultiByte( CP_ACP, 0, mLogParser->getFilename(), -1, NULL, 0, NULL, NULL );
    char *filenameAnsi = new char[filenameLength];
    WideCharToMultiByte( CP_ACP, 0, mLogParser->getFilename(), -1, filenameAnsi, filenameLength, NULL, NULL );
    ifstream logFile( filenameAnsi, ios_base::in );
    delete filenameAnsi;

    if( logFile.good() )
    {
        //log is open..request mSaveButton file and mSaveButton
        TCHAR workingDirectory[200];

        // opening a file in another directory changes the current
        // directory, which will cause problems because the log and data
        // files are opened relative to the current directory.  So, get the
        // directory now so that it can be restored when the user is done
        // picking a file.
        DWORD returnValue = GetCurrentDirectory( 200, workingDirectory );
        if( returnValue == 0 || returnValue > 200 )
        {
            MessageBox( _T("Unable to get current directory"), _T("Severe Error") );
            OnCancel();
            return;
        }

        CFileDialog dlg
            (
            FALSE,
            _T("log"),
            NULL,
            OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
            _T("Log Files (*.log)|*.log||")
            );

        if( dlg.DoModal() == IDOK && dlg.GetFileName() != mLogParser->getFilename() )
        {
            std::string line;

            int filenameLength = WideCharToMultiByte( CP_ACP, 0, dlg.GetPathName(), -1, NULL, 0, NULL, NULL );
            char *filename = new char[filenameLength];
            WideCharToMultiByte( CP_ACP, 0, dlg.GetPathName().GetBuffer(), -1, filename, filenameLength, NULL, NULL );
            ofstream destinationFile( filename, ios_base::out );
            delete filename;
            if( destinationFile.good() )
            {
                logFile.peek();
                while( !logFile.eof() )
                {
                    getline( logFile, line );
                    destinationFile << line << endl;
                }
            }
            destinationFile.close();
        }
        SetCurrentDirectory( workingDirectory );    //change back (see above)
    }
    logFile.close();
}

//----------------------------------------------------------------------
//! \brief Click handler for the Find Next button.
//! \details Finds the next occurrence of the search string supplied
//!     in the list of packets.  Note that this only searches the
//!     packet description in the list, not the parsed content.
//----------------------------------------------------------------------
void CLogViewerDlg::OnBnClickedFindNext()
{
    UpdateData( TRUE );

    if( mPacketListBox.GetCount() == 0 )
    {
        return;         // nothing to search, so exit
    }

    int selectedIndex = mPacketListBox.GetCurSel();
    if( selectedIndex < 0 )
    {
        selectedIndex = 0;
    }

    CString listItemText;
    CString searchText = mSearchText;

    searchText.MakeLower();    //don't want to edit the text entered
    if( searchText != _T("") )
        {
        mPacketListBox.setSearchString( searchText );
        int nextIndex;
        switch( mSearchDirection )
            {
            case SEARCH_UP: //up selected
                {
                for(nextIndex = selectedIndex - 1; nextIndex != selectedIndex; nextIndex-- )
                    {
                    if( nextIndex < 0 )
                    {
                        nextIndex = mPacketListBox.GetCount() - 1;  //count is not zero-based
                    }
                    mPacketListBox.GetText( nextIndex, listItemText );
                    listItemText.MakeLower();
                    if( listItemText.Find( searchText, 0 ) != -1 )
                        {
                        mPacketListBox.SetCurSel( nextIndex );
                        OnLbnSelchangeLog();    //update packet info window
                        break;
                        }
                    }
                break;
                }
            case SEARCH_DOWN: //down selected
                {
                for( nextIndex = selectedIndex + 1; nextIndex != selectedIndex; nextIndex++ )
                    {
                    if( nextIndex >= mPacketListBox.GetCount() )
                        {
                        nextIndex = 0;
                        if (nextIndex == selectedIndex )
                            break; //have to check condition since we are changing in middle
                        }
                    mPacketListBox.GetText( nextIndex, listItemText );
                    listItemText.MakeLower();
                    if( listItemText.Find( searchText, 0 ) != -1 )
                        {
                        mPacketListBox.SetCurSel( nextIndex );
                        OnLbnSelchangeLog();    //update packet info window
                        break;
                        }
                    }
                break;
                }
            default:
                return;
            }
        if( nextIndex == selectedIndex )    //either not found at all, or index highlighted is the only one
            {
            mPacketListBox.GetText( selectedIndex, listItemText );
            listItemText.MakeLower();
            CString error;
            if( listItemText.Find( searchText, 0 ) != -1 ) //no more occurrences of this
                {
                error.Format( _T("No more occurrences of '%s' were found"), mSearchText );
                }
            else                                    //not found at all
                {
                error.Format( _T("'%s' was not found "), mSearchText );
                }
            MessageBox( error, _T("End of Search") );
            }
        }
    else
        {
        mPacketListBox.setSearchString( _T("") );
        MessageBox( _T("Search box is empty"), _T("Unable to search") );
        }
    mPacketListBox.RedrawWindow( 0, 0, RDW_FRAME|RDW_INVALIDATE|RDW_UPDATENOW|RDW_ERASE );
}
