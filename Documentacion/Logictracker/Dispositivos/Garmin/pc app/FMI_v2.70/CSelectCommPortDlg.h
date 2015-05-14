/*********************************************************************
*
*   HEADER NAME:
*       CSelectCommPortDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CSelectCommPortDlg_H
#define CSelectCommPortDlg_H

#include "CFmiPcAppDlg.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to select the port to use to
//!     communicate with the client.
//----------------------------------------------------------------------
class CSelectCommPortDlg : public CDialog
{
    DECLARE_DYNAMIC( CSelectCommPortDlg )
    DECLARE_MESSAGE_MAP()

public:
    CSelectCommPortDlg
        (
        CWnd * aParent = NULL
        );

    virtual ~CSelectCommPortDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();

    afx_msg void OnBnClickedCancel();
    afx_msg void OnBnClickedOk();

protected:
    //! Index of the selected port in the drop down.
    int mSelectedPortIndex;
};

#endif
