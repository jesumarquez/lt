/*********************************************************************
*
*   HEADER NAME:
*       EventListener.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef EventListener_H
#define EventListener_H

class EventListener;

#include "Event.h"

//----------------------------------------------------------------------
//! \brief Base class for objects that take action in response to
//!    an Event being posted.
//! \details Abstract base class for objects that need to be notified
//!    of Event posting.  An EventListener registers itself with the
//!    event system, and onEvent() is called when an event is posted.
//----------------------------------------------------------------------
class EventListener
{
public:
    EventListener();
    virtual ~EventListener();

    //----------------------------------------------------------------------
    //! \brief Callback for when an event is posted.
    //! \details This function is called when any event has occurred.
    //! \param aEventId The Event ID.
    //! \param aEventData An integer; meaning is event-specific.
    //! \param aEventDataPtr A pointer; meaning is event-specific.
    //----------------------------------------------------------------------
    virtual void onEvent
        (
        EventId aEventId,
        uint32  aEventData,
        void*   aEventDataPtr
        ) = 0;
};

#endif
