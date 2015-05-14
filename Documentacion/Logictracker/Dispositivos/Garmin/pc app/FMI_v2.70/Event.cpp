/*********************************************************************
*
*   MODULE NAME:
*       Event.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include <algorithm>

#include "Event.h"
#include "EventListener.h"

std::list<EventListener*> Event::mListeners;

//----------------------------------------------------------------------
//! \brief Adds a window that is interested in receiving events
//! \param aTarget The object that is interested in events
//----------------------------------------------------------------------
void Event::addListener
    (
    EventListener * aTarget
    )
{
    // Precondition: aTarget must not be in the list
    ASSERT( aTarget != NULL );
    ASSERT( find( mListeners.begin(), mListeners.end(), aTarget ) == mListeners.end() );

    mListeners.push_back( aTarget );
}

//----------------------------------------------------------------------
//! \brief Remove a target window from the interested object list
//! \param  aTarget The object that is no longer interested
//----------------------------------------------------------------------
void Event::removeListener
    (
    EventListener * aTarget
    )
{
    ASSERT( aTarget != NULL );

    std::list<EventListener*>::iterator iter;

    // Precondition: aTarget must be in the event target list
    iter = find( mListeners.begin(), mListeners.end(), aTarget );
    ASSERT( iter != mListeners.end() );

    mListeners.erase( iter );
}

//----------------------------------------------------------------------
//! \brief Posts a message to all windows that have registered to
//!     get events
//! \details Calls the onEvent function for all registered listeners.
//! \param  aEventId The event ID that is posted.
//! \param  aEventData A small bit of event data, may be 0
//! \param  aEventDataPtr A pointer to a longer bit of event data, may
//!     be NULL if not used
//----------------------------------------------------------------------
void Event::post
    (
    EventId           aEventId,
    uint32            aEventData,
    void *            aEventDataPtr
    )
{
    std::list<EventListener*>::iterator iter;
    for( iter = mListeners.begin(); iter != mListeners.end(); ++iter )
    {
        (*iter)->onEvent( aEventId, aEventData, aEventDataPtr );
    }
}
