/*********************************************************************
*
*   HEADER NAME:
*       CCommErrorDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CCommErrorDlg_H
#define CCommErrorDlg_H

class CCommErrorDlg;

#include "CFmiPcAppDlg.h"

//----------------------------------------------------------------------
//! \brief Modal dialog displaying a communication error.
//! \details Gives the user the option to exit, retry the operation,
//!     or change the com port.
//----------------------------------------------------------------------
class CCommErrorDlg : public CDialog
{
    DECLARE_DYNAMIC( CCommErrorDlg )
    DECLARE_MESSAGE_MAP()

public:
    CCommErrorDlg
        (
        TCHAR               * aErrorMessage,
        BOOL                  aCommPortDown,
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CCommErrorDlg();

protected:
    BOOL OnInitDialog();
    afx_msg void OnBnClickedRetry();
    afx_msg void OnBnClickedChangeCom();
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    afx_msg void OnBnClickedExit();
    void OnCancel();
    void PostNcDestroy();

    //! The message to display to the user
    CString mMessageText;

    //! If TRUE, communication is down and the serial port will need to be
    //! reinitialized to continue
    BOOL mComPortDown;

    //! The FMI communication controller
    FmiApplicationLayer&  mCom;
};

#endif
