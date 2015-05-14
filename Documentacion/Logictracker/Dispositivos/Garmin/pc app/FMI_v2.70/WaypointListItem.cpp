/*********************************************************************
*
*   MODULE NAME:
*       WaypointListItem.cpp
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "WaypointListItem.h"

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
WaypointListItem::WaypointListItem() : ClientListItem()
{

}

//----------------------------------------------------------------------
//! \brief Get categories that this waypoint is part of
//! \return The categories that this waypoint is part of
//----------------------------------------------------------------------
uint16 WaypointListItem::getCategories() const
{
    return mCategories;
}

void WaypointListItem::readFromStream
    (
    std::istream &aStream
    )
{
    aStream >> mCategories;
    aStream.get();    // ignore '-' inserted after mCategories

    ClientListItem::readFromStream( aStream );
}

//----------------------------------------------------------------------
//! \brief Set the waypoint's categories
//! \param aCategories The categories that this waypoint is part of
//----------------------------------------------------------------------
void WaypointListItem::setCategories
    (
    uint16 aCategories
    )
{
    mCategories = aCategories;
}

//----------------------------------------------------------------------
//! \brief Set the parent
//! \param aParent The FileBackedMap that contains this item
//----------------------------------------------------------------------
void WaypointListItem::setParent
    (
    FileBackedMap<WaypointListItem> * aParent
    )
{
    mParent = aParent;
}

void WaypointListItem::writeToStream
    (
    std::ofstream &aStream
    ) const
{
    aStream << mCategories << "-";
    ClientListItem::writeToStream( aStream );
}

//----------------------------------------------------------------------
//! \brief Save this item
//! \details Invokes parent to save all items
//----------------------------------------------------------------------
void WaypointListItem::save()
{
    if( mParent )
    {
        mParent->save();
    }
}
