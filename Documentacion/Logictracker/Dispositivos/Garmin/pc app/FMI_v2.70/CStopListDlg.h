/*********************************************************************
*
*   HEADER NAME:
*       CStopListDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CStopListDlg_H
#define CStopListDlg_H

class CStopListDlg;

#include "CWndEventListener.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modeless dialog allowing the user to manage the A603 stops
//!     on the client device.
//! \details CStopListDlg allows the user to view and manipulate the
//!     A603 stops on the client device.  The user also has the option
//!     to create a new stop and to sort the stop list.
//----------------------------------------------------------------------
class CStopListDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CStopListDlg )
    DECLARE_MESSAGE_MAP()

public:
    CStopListDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CStopListDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    void PostNcDestroy();
    BOOL OnInitDialog();
    afx_msg void OnBnClickedNewStop();
    afx_msg void OnBnClickedOk();
    afx_msg void OnCancel();

#if( FMI_SUPPORT_A603 )
    afx_msg void OnEnChangeMoveTo();
    afx_msg void OnBnClickedSend();
    afx_msg void OnBnClickedSort();
    afx_msg void OnCbnSelChangeUpdateOption();
    afx_msg LRESULT OnEventStopListChanged( WPARAM, LPARAM );
    afx_msg LRESULT OnEventEtaReceived( WPARAM, LPARAM );
#endif

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

#if( FMI_SUPPORT_A603 )
    //! Index of the selected stop in the stop list
    int mSelectedStopIndex;

    //! The list box control containing the A603 stops
    CListBox mStopListBox;

    //! Text description of the selected stop's status
    CString mSelectedStopStatus;

    //! Index of the selected item in the Update Stop combo box
    //! \see stop_status_status_enum (valid values are the REQUEST_* enums)
    int mSelectedUpdateIndex;

    //! Text in the "Move To" edit box
    CString mMoveTo;

    //! Text in the ETA edit box
    CString mEta;
#endif
};

#endif
