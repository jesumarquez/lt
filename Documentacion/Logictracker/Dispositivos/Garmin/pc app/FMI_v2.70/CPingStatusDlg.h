/*********************************************************************
*
*   HEADER NAME:
*       CPingStatusDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CPingStatusDlg_H
#define CPingStatusDlg_H

#include "CWndEventListener.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modeless dialog allowing the user to view ping status and
//!     send a ping
//! \details This dialog allows the user to view the number of client
//!     to server pings and the timestamp of the last ping received,
//!     the number of server to client pings sent (and last time sent).
//!     The user can also reset these statistics, and initiate a server
//!     to client ping.
//! \since Protocol A604
//----------------------------------------------------------------------
class CPingStatusDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CPingStatusDlg )
    DECLARE_MESSAGE_MAP()

public:
    CPingStatusDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CPingStatusDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void    OnBnClickedResetClient();
    afx_msg void    OnBnClickedResetServer();
    afx_msg void    OnBnClickedSendPing();
    afx_msg LRESULT OnPingEvent( WPARAM, LPARAM );

    void updateTextFields();

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! Number of client to server pings received
    CString       mClientPingCount;

    //! Number of server to client pings sent
    CString       mServerPingCount;

    //! Time when the last client to server ping was received, in the
    //!     form HH:MM:SS AA (AA is am or pm)
    CString       mClientPingTime;

    //! Time when the last server to client ping was sent, in the form
    //!     HH:MM:SS AA (AA is am or pm)
    CString       mServerPingTime;
};

#endif
