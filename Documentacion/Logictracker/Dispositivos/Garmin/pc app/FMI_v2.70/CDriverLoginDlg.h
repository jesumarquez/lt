/*********************************************************************
*
*   HEADER NAME:
*       CDriverLoginDlg.h
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CDriverLoginDlg_H
#define CDriverLoginDlg_H

class CDriverLoginDlg;

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to manage canned responses.
//! \details This dialog allows the user to manage the global list of
//!     canned responses available for messages.  Particular responses
//!     for a message are selected in the CSelectCannedResponseDlg.
//! \note This dialog must always be created modeless.
//! \since Protocol A607
//----------------------------------------------------------------------
class CDriverLoginDlg : public CDialog
{
    DECLARE_DYNAMIC( CDriverLoginDlg )
    DECLARE_MESSAGE_MAP()

public:
    CDriverLoginDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CDriverLoginDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    void updateListBox();
    void PostNcDestroy();
    afx_msg void OnBnClickedDelete();
    afx_msg void OnBnClickedSet();
    afx_msg void OnEnChangeEditBoxes();
    afx_msg void OnBnClickedOk();
    afx_msg void OnCancel();
    afx_msg void OnEnSetfocusLoginEdit();
    afx_msg void OnEnKillfocusLoginEdit();
    afx_msg void OnLbnSelchangeDriverList();
    afx_msg void OnLbnSetfocusDriverList();
    afx_msg void OnLbnKillfocusDriverList();

protected:
    //! Reference to the FMI communication controller
    FmiApplicationLayer& mCom;

    //! \brief List box control containing the canned responses
    CListBox mListBox;

    //! \brief Index of the list box item currently selected
    int mSelectedIndex;

    //! \brief DDX member: the contents of the Driver ID edit box
    CString mDriverId;

    //! \brief DDX member: the contents of the Driver Password edit box
    CString mDriverPassword;
};

#endif
