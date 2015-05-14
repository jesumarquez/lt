/*********************************************************************
*
*   MODULE NAME:
*       TimerManager.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include <cassert>
#include "TimerManager.h"

TimerManager * TimerManager::sInstance = NULL;

//----------------------------------------------------------------------
//! \brief Get the TimerManager instance
//! \details Get the instance of the TimerManager, constructing it if
//!     necessary.
//----------------------------------------------------------------------
TimerManager * TimerManager::getInstance()
{
    if( sInstance == NULL )
    {
        sInstance = new TimerManager;
        atexit( TimerManager::destroyInstance );
    }
    return sInstance;
}

//----------------------------------------------------------------------
//! \brief Destroy the TimerManager instance
//! \details Destroy the one and only TimerManager.
//----------------------------------------------------------------------
void TimerManager::destroyInstance()
{
    delete sInstance;
    sInstance = NULL;
}

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
TimerManager::TimerManager()
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
TimerManager::~TimerManager()
{
}

//----------------------------------------------------------------------
//! \brief Register a listener to receive periodic callbacks
//! \param aListener The listener object to add
//----------------------------------------------------------------------
void TimerManager::addListener
    (
    TimerListener * aListener
    )
{
    getInstance()->instanceAddListener( aListener );
}

//----------------------------------------------------------------------
//! \brief Register a listener to receive periodic callbacks
//! \param aListener The listener object to add
//----------------------------------------------------------------------
void TimerManager::instanceAddListener
    (
    TimerListener * aListener
    )
{
    assert( aListener != NULL );
    mListeners.push_back( aListener );
}

//----------------------------------------------------------------------
//! \brief Remove a TimerListener from the list that is called
//! \param aListener The aListener to remove
//----------------------------------------------------------------------
void TimerManager::removeListener
    (
    TimerListener * aListener
    )
{
    getInstance()->instanceRemoveListener( aListener );
}

//----------------------------------------------------------------------
//! \brief Remove a TimerListener from the list that is called
//! \param  aListener The listener to remove
//----------------------------------------------------------------------
void TimerManager::instanceRemoveListener
    (
    TimerListener * aListener
    )
{
    assert( aListener != NULL );
    mListeners.remove( aListener );
}

//----------------------------------------------------------------------
//! \brief Timer tick
//! \details This function is called periodically and distributes the
//!    timer event to all TimerListnener objects.
//----------------------------------------------------------------------
void TimerManager::tick()
{
    getInstance()->instanceTick();
}

//----------------------------------------------------------------------
//! \brief Timer tick
//! \details This function is called periodically and distributes the
//!    timer event to all TimerListnener objects.
//----------------------------------------------------------------------
void TimerManager::instanceTick()
{
    std::list<TimerListener *>::iterator iter;

    for( iter = mListeners.begin(); iter != mListeners.end(); iter++ )
    {
        (*iter)->onTimer();
    }
}
