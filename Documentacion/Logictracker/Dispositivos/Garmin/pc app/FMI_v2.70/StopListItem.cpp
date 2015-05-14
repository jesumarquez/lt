/*********************************************************************
*
*   MODULE NAME:
*       StopListItem.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "StopListItem.h"

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
StopListItem::StopListItem() : ClientListItem()
{
    mStopStatus = INVALID_STOP_STATUS;
}

void StopListItem::readFromStream( std::istream &aStream )
{
    mStopStatus = INVALID_STOP_STATUS;
    ClientListItem::readFromStream( aStream );
}

//----------------------------------------------------------------------
//! \brief Get the stop status
//! \return the status of the stop
//----------------------------------------------------------------------
stop_status_status_type StopListItem::getStopStatus() const
{
    return mStopStatus;
}

//----------------------------------------------------------------------
//! \brief Set the stop status
//! \param aStatus The new stop status
//----------------------------------------------------------------------
void StopListItem::setStopStatus
    (
    stop_status_status_type aStatus
    )
{
    mStopStatus = aStatus;
}

//----------------------------------------------------------------------
//! \brief Set the parent
//! \param aParent The FileBackedMap that contains this item
//----------------------------------------------------------------------
void StopListItem::setParent
    (
    FileBackedMap<StopListItem> * aParent
    )
{
    mParent = aParent;
}

//----------------------------------------------------------------------
//! \brief Set the current name of the stop
//! \param aName The new name of the stop
//----------------------------------------------------------------------
void StopListItem::setCurrentName
    (
    CString aName
    )
{
    mCurrentName = aName;
}
