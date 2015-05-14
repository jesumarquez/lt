/*********************************************************************
*
*   HEADER NAME:
*       TimerManager.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef TimerManager_H
#define TimerManager_H

class TimerManager;

#include <list>
#include "TimerListener.h"

//----------------------------------------------------------------------
//! \brief Manages timer (periodic) events.
//! \details The TimerManager manages the list of TimerListener objects
//!     in the application, and periodically calls their onTimer
//!     function. The application is responsible for periodically
//!     calling tick().
//----------------------------------------------------------------------
class TimerManager
{
public:
    static void addListener
        (
        TimerListener * aListener
        );
    static void removeListener
        (
        TimerListener * aListener
        );
    static void tick();

protected:

private:
    static TimerManager * getInstance();
    static void destroyInstance();

    TimerManager();
    virtual ~TimerManager();

    void instanceAddListener
        (
        TimerListener * aListener
        );
    void instanceRemoveListener
        (
        TimerListener * aListener
        );
    void instanceTick();

    //! Pointer to the one and only TimerManager instance
    static TimerManager * sInstance;

    //! List of objects that will get periodic notifications
    std::list<TimerListener *> mListeners;
};

#endif
