/*********************************************************************
*
*   HEADER NAME:
*       CDeleteDataDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CDeleteDataDlg_H
#define CDeleteDataDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to delete specified FMI data from
//!     the client.
//! \details For each item selected by the user, the Data Deletion
//!     protocol is used to request that the client delete the
//!     corresponding data.
//! \note This dialog must be created modal.
//! \since Protocol A603
//----------------------------------------------------------------------
class CDeleteDataDlg : public CDialog
{
    DECLARE_DYNAMIC( CDeleteDataDlg )
    DECLARE_MESSAGE_MAP()

public:
    CDeleteDataDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );
    virtual ~CDeleteDataDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void OnBnClickedOk();

    //! Reference to the FMI communication controller
    FmiApplicationLayer& mCom;

    //! If true, the Delete Messages check box is selected
    BOOL mDeleteMessagesChecked;

    //! If true, the Delete Stops check box is selected
    BOOL mDeleteStopsChecked;

#if FMI_SUPPORT_A604
    //! If true, the Delete Active Route check box is selected
    //! \since Protocol A604
    BOOL mDeleteActiveRouteChecked;

    //! If true, the Delete Canned Messages check box is selected
    //! \since Protocol A604
    BOOL mDeleteCannedMessagesChecked;

    //! If true, the Delete Canned Responses check box is selected
    //! \since Protocol A604
    BOOL mDeleteCannedResponsesChecked;

    //! If true, the Delete GPI File check box is selected
    //! \since Protocol A604
    BOOL mDeleteGpiFileChecked;

    //! If true, the Delete Driver ID and Status check box is selected
    //! \since Protocol A604
    BOOL mDeleteDriverInfoChecked;

    //! If true, the Disable FMI check box is selected
    //! \since Protocol A604
    BOOL mDisableFmiChecked;
#endif

#if FMI_SUPPORT_A607
    //! If true, the Waypoints check box is selected
    //! \since Protocol A607
    BOOL mWaypointsChecked;
#endif
};

#endif
