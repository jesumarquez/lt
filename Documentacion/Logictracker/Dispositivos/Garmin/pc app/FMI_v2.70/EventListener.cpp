/*********************************************************************
*
*   MODULE NAME:
*       EventListener.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "EventListener.h"

//----------------------------------------------------------------------
//! \brief Construct a new EventListener
//! \details Automatically registers this object with the Event
//!    manager.
//----------------------------------------------------------------------
EventListener::EventListener()
{
    Event::addListener( this );
}

//----------------------------------------------------------------------
//! \brief Destructor
//! \details Automatically unregisters this object from the Event
//!   manager.
//----------------------------------------------------------------------
EventListener::~EventListener()
{
    Event::removeListener( this );
}
