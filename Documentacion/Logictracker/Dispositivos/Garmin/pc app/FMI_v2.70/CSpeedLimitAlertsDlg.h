/*********************************************************************
*
*   HEADER NAME:
*       CSpeedLimitAlertsDlg.h
*
*   Copyright 2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CSpeedLimitAlertsDlg_H
#define CSpeedLimitAlertsDlg_H

#include "CWndEventListener.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to change speed limit alerts settings
//! \note This dialog must be created modal.
//! \since Protocol A608
//----------------------------------------------------------------------
class CSpeedLimitAlertsDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CSpeedLimitAlertsDlg )
    DECLARE_MESSAGE_MAP()

public:
    CSpeedLimitAlertsDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CSpeedLimitAlertsDlg();

private:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    void EnableFields
        (
        bool aValue
        );

    afx_msg void OnBnClickedSend();

    afx_msg void OnCbnSelchangeSpeedLimitCboMode();

    afx_msg LRESULT OnEventSpeedLimitSetResultFromClient
        (
        WPARAM aResultCode,
        LPARAM
        );

    BOOL OnInitDialog();

    void SetResult
        (
        uint8 aResultCode
        );

    //! Reference to the FMI communication controller that this dialog uses
    FmiApplicationLayer& mCom;

    //! Selection of the mode combo box
    uint8   mMode;

    //! Value of the time over edit box
    uint8   mTimeOver;

    //! Value of the time under edit box
    uint8   mTimeUnder;

    //! Selection value of the alert user combo box
    boolean mAlertUser;

    //! Value of the threshold edit box
    float   mThreshold;
};

#endif
