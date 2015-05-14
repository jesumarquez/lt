/*********************************************************************
*
*   HEADER NAME:
*       CGpiQueryDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CGpiQueryDlg_H
#define CGpiQueryDlg_H

#include "CFmiPcAppDlg.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to query the file version and
//!     size of the FMI GPI file on the client device.
//! \details When the dialog is displayed, and when the Refresh button
//!     is clicked, the dialog initiates the GPI File Information
//!     Protocol.  When the response is received from the client,
//!     FmiApplicationLayer sends an event; this dialog then displays
//!     the details received from the client.
//! \since Protocol A604
//----------------------------------------------------------------------
class CGpiQueryDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CGpiQueryDlg )
    DECLARE_MESSAGE_MAP()

public:
    CGpiQueryDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CGpiQueryDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void OnBnClickedUpdate();
    afx_msg LRESULT OnGpiInfoReceived( WPARAM, LPARAM );

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! Contents of the File Version text box
    CString mFileVersion;

    //! Contents of the File Size text box
    CString mFileSize;
};

#endif
