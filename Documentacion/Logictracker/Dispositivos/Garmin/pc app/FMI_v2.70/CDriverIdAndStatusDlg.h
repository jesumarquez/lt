/*********************************************************************
*
*   HEADER NAME:
*       CDriverIdAndStatusDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CDriverIdAndStatusDlg_H
#define CDriverIdAndStatusDlg_H

#include <vector>

class CDriverIdAndStatusDlg;

#include "CWndEventListener.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modeless dialog allowing the user to query and update
//!     Driver ID and Status on the client.
//! \since Protocol A604
//----------------------------------------------------------------------
class CDriverIdAndStatusDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CDriverIdAndStatusDlg )
    DECLARE_MESSAGE_MAP()

public:
    CDriverIdAndStatusDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CDriverIdAndStatusDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange* aDataExchange
        );

    BOOL         OnInitDialog();
    void         updateListBox();
    void         PostNcDestroy();
    afx_msg void OnEnChangeEditDriverId();
    afx_msg void OnBnClickedSendDriverStatus();
    afx_msg void OnBnClickedSendDriverId();
    afx_msg void OnBnClickedRefreshDriverId();
    afx_msg void OnBnClickedRefreshDriverStatus();
    afx_msg void OnEnChangeEditSet();
    afx_msg void OnBnClickedDelete();
    afx_msg void OnBnClickedSendDriverStatusItem();
    afx_msg void OnBnClickedOk();
    afx_msg void OnIndexChanged();
    afx_msg void OnCancel();
    afx_msg void OnEnSetfocusDriverIdEdit();
    afx_msg void OnEnKillfocusDriverIdEdit();
    afx_msg void OnEnSetfocusStatusEdit();
    afx_msg void OnEnKillfocusStatusEdit();
    afx_msg void OnLbnSelchangeStatuslist();
    afx_msg LPARAM OnDriverStatusListChanged( WPARAM, LPARAM );
    afx_msg LPARAM OnDriverIdChanged( WPARAM aIndex, LPARAM );
    afx_msg LPARAM OnDriverStatusChanged( WPARAM aIndex, LPARAM );

#if( FMI_SUPPORT_A607 )
    afx_msg void OnBnClickedEditLogins();
#endif
    //! Reference to the FMI communication controller
    FmiApplicationLayer& mCom;

    //! Contents of the "current driver ID" text box
    CString mCurrentDriverId;

    //! Contents of the "current driver status" text box
    CString mCurrentDriverStatus;

    //! Contents of the "update driver ID" edit box
    CString mNewDriverId;

    //! The list box containing the driver statuses on the client
    CListBox mDriverStatusList;

    //! Index of the selected item in the mDriverStatusList (-1 if no item is selected)
    int mSelectedListIndex;

    //! Contents of the ID edit box in the Set Driver Status List Item group
    CString mNewDriverStatusId;

    //! Contents of the status text edit box in the Set Driver Status List Item group
    CString mNewDriverStatusText;

    //! Selected driver index
    //! \since Protocol A607
    int mIndex;
};

#endif
