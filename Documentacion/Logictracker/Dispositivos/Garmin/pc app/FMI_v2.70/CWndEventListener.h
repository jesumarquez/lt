/*********************************************************************
*
*   HEADER NAME:
*       CWndEventListener.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CWndEventListener_H
#define CWndEventListener_H

class CWndEventListener;

#include "EventListener.h"

//! Translation from an application event to the corresponding Windows
//!     message
#define WM_EVENT( _event )      ( _event + WM_APP )

//----------------------------------------------------------------------
//! \brief EventListener that dispatches a Windows message
//! \details This implementation of EventListener should be inherited
//!    by any CWnd that needs to process an Event.  The message map of
//!    the child class should contain ON_MESSAGE handlers for any
//!    events that need to be processed, for example:
//!    ON_MESSAGE( WM_EVENT( EVENT... ), On...() )
//----------------------------------------------------------------------
class CWndEventListener : public EventListener
{
public:
    void onEvent
        (
        EventId aEventId,
        uint32  aEventData,
        void*   aEventDataPtr
        );

protected:

private:

};

#endif
