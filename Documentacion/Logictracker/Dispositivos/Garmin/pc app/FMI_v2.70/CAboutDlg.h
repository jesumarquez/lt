/*********************************************************************
*
*   HEADER NAME:
*       CAboutDlg.h
*
* Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CAboutDlg_H
#define CAboutDlg_H

#include "stdafx.h"
#include "resource.h"
#include "CStaticLink.h"

//----------------------------------------------------------------------
//! \brief About box for this application
//! \details Dialog displaying product name, version, and a link
//!    control that takes the user to the Garmin FMI web site.
//----------------------------------------------------------------------
class CAboutDlg : public CDialog
{
    DECLARE_MESSAGE_MAP()

public:
    CAboutDlg
        (
        CWnd * aParent
        );

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();

    //! \brief Control displaying the Garmin URL.
    CStaticLink mGarminLink;
};

#endif
