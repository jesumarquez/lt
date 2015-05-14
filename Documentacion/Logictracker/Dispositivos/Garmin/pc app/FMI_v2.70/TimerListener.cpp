/*********************************************************************
*
*   MODULE NAME:
*       TimerListener.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "TimerListener.h"
#include "TimerManager.h"

TimerListener::TimerListener()
{
    TimerManager::addListener( this );
}

TimerListener::~TimerListener()
{
    TimerManager::removeListener( this );
}
