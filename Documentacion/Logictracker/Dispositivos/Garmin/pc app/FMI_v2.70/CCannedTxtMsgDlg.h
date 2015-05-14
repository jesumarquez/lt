/*********************************************************************
*
*   HEADER NAME:
*       CCannedTxtMsgDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CCannedTxtMsgDlg_H
#define CCannedTxtMsgDlg_H

class CCannedTxtMsgDlg;

#include "FmiApplicationLayer.h"
#include "CWndEventListener.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to manage the list of canned
//! messages on the client.
//! \details This dialog must be created modal.
//! \since Protocol A604
//----------------------------------------------------------------------
class CCannedTxtMsgDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CCannedTxtMsgDlg )
    DECLARE_MESSAGE_MAP()

public:
    CCannedTxtMsgDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CCannedTxtMsgDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    void updateListBox();
    void PostNcDestroy();
    afx_msg void OnBnClickedDelete();
    afx_msg void OnBnClickedOk();
    afx_msg void OnBnClickedSend();
    afx_msg void OnCancel();
    afx_msg void OnEnChangeEditBoxes();
    afx_msg void OnEnKillfocusMessageEdit();
    afx_msg void OnEnSetfocusMessageEdit();
    afx_msg void OnLbnKillfocusMsgList();
    afx_msg void OnLbnSetfocusList();
    afx_msg LPARAM OnCannedMsgListChanged( WPARAM, LPARAM );
    afx_msg void OnLbnSelchangeMsglist();

    //! \brief Listbox containing the canned messages on the client
    CListBox mCannedMessageList;

    //! \brief The canned message ID entered by the user
    CString mMessageId;

    //! \brief The canned message text entered by the user
    CString mMessageText;

    //! \brief The index of the selected item in the list of messages
    int mSelectedIndex;

    //! \brief Reference to the communication layer
    FmiApplicationLayer& mCom;
};

#endif
