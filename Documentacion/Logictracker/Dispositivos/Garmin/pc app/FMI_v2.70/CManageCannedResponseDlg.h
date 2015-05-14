/*********************************************************************
*
*   HEADER NAME:
*       CManageCannedResponseDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CManageCannedResponseDlg_H
#define CManageCannedResponseDlg_H

class CManageCannedResponseDlg;

#include "CWndEventListener.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to manage canned responses.
//! \details This dialog allows the user to manage the global list of
//!     canned responses available for messages.  Particular responses
//!     for a message are selected in the CSelectCannedResponseDlg.
//! \note This dialog must always be created modeless.
//! \since Protocol A604
//----------------------------------------------------------------------
class CManageCannedResponseDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CManageCannedResponseDlg )
    DECLARE_MESSAGE_MAP()

public:
    CManageCannedResponseDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CManageCannedResponseDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    void updateListBox();
    void PostNcDestroy();
    afx_msg void OnBnClickedDelete();
    afx_msg LPARAM OnCannedRespListChanged( WPARAM, LPARAM );
    afx_msg void OnBnClickedSend();
    afx_msg void OnEnChangeRspBoxes();
    afx_msg void OnBnClickedOk();
    afx_msg void OnCancel();
    afx_msg void OnEnSetfocusResponseEdit();
    afx_msg void OnEnKillfocusResponseEdit();
    afx_msg void OnLbnSelchangeResponselist();
    afx_msg void OnLbnSetfocusResponselist();
    afx_msg void OnLbnKillfocusResponselist();

protected:
    //! Reference to the FMI communication controller
    FmiApplicationLayer& mCom;

    //! \brief List box control containing the canned responses
    CListBox mListBox;

    //! \brief Index of the list box item currently selected
    int mSelectedResponseIndex;

    //! \brief DDX member: the contents of the Response ID edit box
    CString mResponseId;

    //! \brief DDX member: the contents of the Response Text edit box
    CString mResponseText;
};

#endif
