/*********************************************************************
*
*   HEADER NAME:
*       CTxtMsgStatusRequestDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CTxtMsgStatusRequestDlg_H
#define CTxtMsgStatusRequestDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to request the status of a
//!     server to client text message.
//! \since Protocol A604
//----------------------------------------------------------------------
class CTxtMsgStatusRequestDlg : public CDialog
{
    DECLARE_DYNAMIC( CTxtMsgStatusRequestDlg )
    DECLARE_MESSAGE_MAP()

public:
    CTxtMsgStatusRequestDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CTxtMsgStatusRequestDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );
    afx_msg void OnEnChangeEditMsgId();
    afx_msg void OnBnClickedOk();
    BOOL OnInitDialog();

    //! Reference to the FMI communication controller that this dialog uses
    FmiApplicationLayer & mCom;

    //! Message ID entered by the user
    CString mMessageId;
};

#endif
