/*********************************************************************
*
*   HEADER NAME:
*       CWaypointDlg.h
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#ifndef CWaypointDlg_H
#define CWaypointDlg_H

#include "stdafx.h"
#include "FmiApplicationLayer.h"
#include "CWndEventListener.h"

//----------------------------------------------------------------------
//! \brief Modeless dialog allowing the user to manage waypoints.
//! \details CStopListDlg allows the user to view and manipulate the
//!     waypoints and waypoint categories on the client device.
//! \since Protocol A607
//----------------------------------------------------------------------
class CWaypointDlg : public CDialog, public CWndEventListener
{
    DECLARE_DYNAMIC( CWaypointDlg )
    DECLARE_MESSAGE_MAP()

public:
    CWaypointDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );   // standard constructor
    virtual ~CWaypointDlg();

    afx_msg void OnBnClickedOk();
    afx_msg void OnBnClickedWptBtnSend();
    afx_msg void OnEnChangeWptEdit();
    afx_msg void OnBnClickedWptBtnDelete();
    afx_msg void OnBnClickedWptBtnDeleteCat();
    afx_msg void OnBnClickedWptBtnCreateCat();

private:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );    // DDX/DDV support
    void PostNcDestroy();
    BOOL OnInitDialog();
    void updateListBox();
    void updateCatBox();
    afx_msg LPARAM OnWaypointListChanged( WPARAM, LPARAM );
    afx_msg LPARAM OnCategoryListChanged( WPARAM, LPARAM );
    uint16 getCatIds();

    //! Reference to the FMI communication controller
    FmiApplicationLayer&   mCom;

private:
    //! Waypoint ID entered by the user
    UINT mWptId;

    //! Waypoint latitude in decimal degrees
    double mLat;

    //! Waypoint longitude in decimal degrees
    double mLon;

    //! Waypoint symbol entered by the user
    UINT mSymbol;

    //! Waypoint category entered by the user
    CString mCat;

    //! Waypoint name entered by the user
    CString mName;

    //! Waypoint comment entered by the user
    CString mComment;

    //! List box containing waypoints sent to the client
    CListBox mListBox;

    //! List box containing waypoint categories sent to the client
    CListBox mCatBox;

    //! Waypoint category ID entered by the user
    unsigned int mCatId;
};

#endif
