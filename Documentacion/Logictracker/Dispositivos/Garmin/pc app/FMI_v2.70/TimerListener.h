/*********************************************************************
*
*   HEADER NAME:
*       TimerListener.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef TimerListener_H
#define TimerListener_H

//----------------------------------------------------------------------
//! \brief Abstract base class for objects that need to do something
//!     periodically.
//! \details The constructor and destructor register and unregister
//!     the TimerListener from the TimerManager.  The TimerManager
//!     will periodically call the onTimer method of all registered
//!     listeners.  The period is indeterminate.
//----------------------------------------------------------------------
class TimerListener
{
public:
    TimerListener();
    virtual ~TimerListener();

    //! \brief Periodic callback
    //! \details This function will be called periodically.
    virtual void onTimer() = 0;

protected:

private:

};

#endif
