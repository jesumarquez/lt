/*********************************************************************
*
*   HEADER NAME:
*       CGpiTransferProgressDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CGpiTransferProgressDlg_H
#define CGpiTransferProgressDlg_H

#include "CWndEventListener.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal dialog displaying the status of the GPI file transfer.
//! \details This dialog is displayed when a GPI file transfer begins.
//!     When FmiApplicationLayer receives a file or packet receipt, it
//!     sends a status update message which this dialog receives and
//!     processes to update the text box and progress bar.  Canceling
//!     the file transfer sets a flag in the FmiApplicationLayer
//!     instance which instructs the server to end the transfer (by
//!     not sending any more data).
//! \since Protocol A604
//----------------------------------------------------------------------
class CGpiTransferProgressDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CGpiTransferProgressDlg )
    DECLARE_MESSAGE_MAP()

public:
    CGpiTransferProgressDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CGpiTransferProgressDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange* aDataExchange
        );

    BOOL OnInitDialog();
    void OnCancel();
    afx_msg void OnBnClickedStop();
    afx_msg LRESULT OnGpiTransferStateChange( WPARAM, LPARAM );
    afx_msg LRESULT OnGpiTransferProgress( WPARAM, LPARAM );

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! Current status of the transfer, as a string
    //! \details This may be a generic string ("Started", "Cancelled")
    //!     or, for an in progress transfer, include the bytes
    //!     transferred and total
    CString mTransferStatus;

    //! The progress bar.
    CProgressCtrl mProgressBar;

    //! If TRUE, the Stop button was clicked.
    BOOL mStopButtonClicked;

};

#endif
