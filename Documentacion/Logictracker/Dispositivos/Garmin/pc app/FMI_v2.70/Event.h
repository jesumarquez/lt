/*********************************************************************
*
*   HEADER NAME:
*       Event.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef Event_H
#define Event_H

#include "stdafx.h"
#include <list>

class Event;

#include "garmin_types.h"
#include "EventId.h"
#include "EventListener.h"

//----------------------------------------------------------------------
//! \brief Event dispatcher
//! \details The Event class manages event posting between objects. To
//!     post an event, call Event::post().  The event will then be
//!     dispatched to all EventListener objects that have registered
//!     themselves via the addListener interface.
//!
//!     An event dispatcher is not always strictly required (direct
//!     callbacks can usually be used instead) but use of an event
//!     system improves maintainability, as it decouples event senders
//!     from receivers, and allows code to be written more
//!     expressively, in terms of real-world things that have happened.
//----------------------------------------------------------------------
class Event
{
public:
    static void post
        (
        EventId aEventId,
        uint32  aEventData    = 0,
        void *  aEventDataPtr = NULL
        );

    static void addListener
        (
        EventListener * aListener
        );

    static void removeListener
        (
        EventListener * aListener
        );

private:
    //! List of listeners that should receive event notifications
    static std::list<EventListener*>  mListeners;
};

#endif
