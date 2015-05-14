/*********************************************************************
*
*   HEADER NAME:
*       CTxtMsgDeleteRequestDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CTxtMsgDeleteRequestDlg_H
#define CTxtMsgDeleteRequestDlg_H

class CTxtMsgDeleteRequestDlg;

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to request the status of a
//!     server to client text message.
//! \since Protocol A604
//----------------------------------------------------------------------
class CTxtMsgDeleteRequestDlg : public CDialog
{
    DECLARE_DYNAMIC( CTxtMsgDeleteRequestDlg )
    DECLARE_MESSAGE_MAP()

public:
    CTxtMsgDeleteRequestDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CTxtMsgDeleteRequestDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    afx_msg void OnEnChangeEditMsgId();
    afx_msg void OnBnClickedOk();
    BOOL OnInitDialog();

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! Message ID entered by the user
    CString mMessageId;
};

#endif
