/*********************************************************************
*
*   HEADER NAME:
*       CFmiApplication.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CFmiApplication_H
#define CFmiApplication_H

#include "resource.h"        // main symbols

//----------------------------------------------------------------------
//! \brief Application class for this MFC application.
//----------------------------------------------------------------------
class CFmiApplication : public CWinApp
{
    DECLARE_MESSAGE_MAP()

public:
    CFmiApplication();
    virtual ~CFmiApplication();
    virtual BOOL InitInstance();
};

//! \brief The one and only application instance
extern CFmiApplication theApp;

#endif
