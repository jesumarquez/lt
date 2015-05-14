/*********************************************************************
*
*   HEADER NAME:
*       CAutoArrivalDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CAutoArrivalDlg_H
#define CAutoArrivalDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to change the auto-arrival options.
//! \note This dialog must be created modal.
//! \since Protocol A603
//----------------------------------------------------------------------
class CAutoArrivalDlg : public CDialog
{
    DECLARE_DYNAMIC( CAutoArrivalDlg )
    DECLARE_MESSAGE_MAP()

public:
    CAutoArrivalDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CAutoArrivalDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void OnBnClickedEnabled();
    afx_msg void OnBnClickedOk();
    afx_msg void OnEnChangeEditBox();

    //! The state of the Enabled check box.
    BOOL mAutoArrivalEnabled;

    //! The minimum stop time for a stop to be considered done by the client.
    CString mMinimumStopTime;

    //! The minimum distance for a stop to be considered done by the client.
    CString mMinimumStopDistance;

    //! Reference to the FMI communication controller that this dialog uses
    FmiApplicationLayer& mCom;
};

#endif
