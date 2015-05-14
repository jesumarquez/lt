/*********************************************************************
*
*   MODULE NAME:
*       CFmiApplication.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CFmiPcAppDlg.h"
#include "FmiApplicationLayer.h"
#include "Logger.h"

BEGIN_MESSAGE_MAP( CFmiApplication, CWinApp )
    ON_COMMAND( ID_HELP, CWinApp::OnHelp )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
CFmiApplication::CFmiApplication()
{
}

CFmiApplication::~CFmiApplication()
{
    Logger::closeLog();
}

//----------------------------------------------------------------------
//! \brief Initialize the application instance
//! \return FALSE to exit, always
//----------------------------------------------------------------------
BOOL CFmiApplication::InitInstance()
{
    // InitCommonControls() is required on Windows XP if an application
    // manifest specifies use of ComCtl32.dll version 6 or later to enable
    // visual styles.  Otherwise, any window creation will fail.
    InitCommonControls();
    CWinApp::InitInstance();
    AfxEnableControlContainer();

    CFmiPcAppDlg dlg;
    m_pMainWnd = &dlg;
    dlg.DoModal();

    // Since the dialog has been closed, return FALSE so that we exit the
    //  application, rather than start the application's message pump
    return FALSE;
}
