/*********************************************************************
*
*   HEADER NAME:
*       CLogViewerDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CLogViewerDlg_H
#define CLogViewerDlg_H

#include <map>
#include <iostream>

#include "fmi.h"
#include "CColoredListBox.h"
#include "LogParser.h"
#include "CWndEventListener.h"

//----------------------------------------------------------------------
//! \brief Modeless log viewer dialog
//! \details This dialog reads, parses, and displays a packet log
//!     created by the Com class.  The list of packets is displayed on
//!     the left side; selecting a packet causes the selected packet
//!     to be displayed and formatted into a text box on the right
//!     side of the dialog.  The user can also search the packet list
//!     (header names only), mSaveButton the current log file, or open
//!     a previous log.
//----------------------------------------------------------------------
class CLogViewerDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CLogViewerDlg )
    DECLARE_MESSAGE_MAP()

public:
    CLogViewerDlg
        (
        LogParser * aLogParser,
        CWnd      * aParent    = NULL,
        BOOL        aOpenOther = FALSE
        );
    virtual ~CLogViewerDlg();

protected:
    //! Search direction.  Indexes must correspond to the radio buttons in
    //!     the IDC_LOG_RDO_UP radio group.
    enum SearchDirectionType
    {
        SEARCH_UP,
        SEARCH_DOWN
    };

    afx_msg void OnBnClickedClearLog();
    afx_msg void OnBnClickedFindNext();
    afx_msg void OnBnClickedOk();
    afx_msg void OnBnClickedSaveAs();
    afx_msg void OnBnClickedViewCurrent();
    afx_msg void OnBnClickedViewOther();
    afx_msg void OnCancel();
    afx_msg void OnGetMinMaxInfo
        (
        MINMAXINFO* aMinMaxInfo
        );

    afx_msg void OnLbnSelchangeLog();
    afx_msg void OnSize
        (
        UINT aType,
        int  aClientWidth,
        int  aClientHeight
        );

    void resendPacket();

    BOOL OnInitDialog();

    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    afx_msg LPARAM OnPacketLogged( WPARAM, LPARAM );

    void PostNcDestroy();

    void resetView();
    void UpdateLogDisplay();

    BOOL updateView();

    //! If TRUE, the Open File dialog should be presented (to aOpenOther another log)
    BOOL mOpenOtherLog;

    //! Reference to the Close button
    CButton mCloseButton;

    //! Reference to the View Other button
    CButton mViewOtherButton;

    //! Reference to the View Current button
    CButton mViewCurrentButton;

    //! Reference to the Save button
    CButton mSaveButton;

    //! Reference to the Clear button
    CButton mClearButton;

    //! Reference to the Find Next button
    CButton mFindNextButton;

    //! Reference to the Search Up radio button
    CButton mSearchUpRadioButton;

    //! Reference to the Search Down radio button
    CButton mSearchDownRadioButton;

    //! Reference to the Search group box
    CButton mSearchGroupBox;

    //! Reference to the Resend button
    CButton mResendButton;

    //! Reference to the Search Text edit box
    CEdit mSearchTextControl;

    //! Reference to the packet list box
    CColoredListBox mPacketListBox;

    //! The string describing the log being viewed ("Current execution's" or a file name)
    CString mLogNameText;

    //! Formatted representation of the current packet
    CString mSelectedPacketText;

    //! Contents of the Search Text edit box
    CString mSearchText;

    //! Reference to a control containing the currently selected packet, formatted
    CStatic mCurrentPacketControl;

    //! Label above the current packet text box
    CStatic mCurrentPacketTitleLabel;

    //! Label above the packet list
    CStatic mPacketListTitleLabel;

    //! Label for the log name control
    CStatic mLogNameTitleLabel;

    //! Control containing the log name
    CStatic mLogNameControl;

    //! Search direction
    //! \see SearchDirectionType for valid values
    int mSearchDirection;

    //! The log parser
    LogParser*  mLogParser;
};

#endif
