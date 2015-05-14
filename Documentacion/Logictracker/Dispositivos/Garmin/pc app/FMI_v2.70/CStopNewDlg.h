/*********************************************************************
*
*   HEADER NAME:
*       CStopNewDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CStopNewDlg_H
#define CStopNewDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Modal dialog allowing the user to send a new stop to the
//!     client
//----------------------------------------------------------------------
class CStopNewDlg : public CDialog
{
    DECLARE_DYNAMIC( CStopNewDlg )
    DECLARE_MESSAGE_MAP()

public:
    CStopNewDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CStopNewDlg();

protected:
    //! Index of the selected item in the mStopProtocol radio button group.
    enum StopProtocolType
    {
        STOP_PROTOCOL_A603,
        STOP_PROTOCOL_A602,
        STOP_PROTOCOL_LEGACY
    };

    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );
    BOOL OnInitDialog();
    afx_msg void OnFormChanged();
    afx_msg void OnBnClickedOk();

    //! Reference to the FMI communication controller
    FmiApplicationLayer & mCom;

    //! Text in the Latitude edit box
    CString mLatitudeStr;

    //! Text in the Longitude edit box
    CString mLongitudeStr;

    //! Text in the Destination Name/Message edit box
    CString mMessageStr;

    //! Text in the Stop ID edit box
    CString mStopId;

    //! Index of the selected mStopProtocol
    //! \see StopProtocolType
    int mStopProtocol;
};

#endif
