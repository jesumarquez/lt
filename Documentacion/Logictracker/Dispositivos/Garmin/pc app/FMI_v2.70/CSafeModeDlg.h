/*********************************************************************
*
*   HEADER NAME:
*       CSafeModeDlg.h
*
*   Copyright 2010 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CSafeModeDlg_H
#define CSafeModeDlg_H

#include "FmiApplicationLayer.h"

//---------------------------------------------------------------------
//! \brief Modal dialog allowing the user to set FMI Safe Mode and the
//!     threshold speed.
//! \since Protocol A606
//---------------------------------------------------------------------
class CSafeModeDlg : public CDialog
{
    DECLARE_DYNAMIC( CSafeModeDlg )
    DECLARE_MESSAGE_MAP()

public:
    CSafeModeDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CSafeModeDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange* aDataExchange
        );
    afx_msg void OnEnChangeSafeModeSpeed();
    afx_msg void OnBnClickedOk();
    BOOL OnInitDialog();

    //! Reference to the FMI communication controller that this dialog uses
    FmiApplicationLayer & mCom;

    //! Speed entered by the user
    CString speed;
};

#endif
