/*********************************************************************
*
*   HEADER NAME:
*       CGpiTransferDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CGpiTransferDlg_H
#define CGpiTransferDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to select a GPI file to
//!     transfer to the client.
//! \since Protocol A604
//----------------------------------------------------------------------
class CGpiTransferDlg : public CDialog
{
    DECLARE_DYNAMIC( CGpiTransferDlg )
    DECLARE_MESSAGE_MAP()

public:
    CGpiTransferDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CGpiTransferDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange* aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void OnEnChangeGpiFile();
    afx_msg void OnBnClickedFind();
    afx_msg void OnBnClickedOk();

    //! Reference to the FMI communication controller
    FmiApplicationLayer& mCom;

    //! Path and file name of the GPI file to transfer, as specified
    //!     by the user
    CString mGpiFilePath;

    //! GPI mVersion string specified by the user.
    CString mVersion;
};

#endif
