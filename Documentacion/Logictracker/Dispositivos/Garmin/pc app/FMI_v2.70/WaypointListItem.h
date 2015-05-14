/*********************************************************************
*
*   HEADER NAME:
*       WaypointListItem.h
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef _WAYPOINTLISTITEM_H
#define _WAYPOINTLISTITEM_H

#include "ClientListItem.h"

//----------------------------------------------------------------------
//! \brief Tracks a waypoint that has been sent to the client.
//! \since Protocol A607
//----------------------------------------------------------------------
class WaypointListItem : public ClientListItem
{
public:
    typedef uint32 key_type;

    WaypointListItem();

    virtual void readFromStream
        (
        std::istream &aStream
        );

    virtual void writeToStream
        (
        std::ofstream &aStream
        ) const;

    uint16 getCategories() const;

    void setCategories( uint16 val );

    void setParent
        (
        FileBackedMap<WaypointListItem> * aParent
        );

protected:
    virtual void save();

private:
    //! The map that this item belongs to, or NULL if none
    FileBackedMap<WaypointListItem> * mParent;

    //! The categories that this waypoint is part of. (bit mapped)
    uint16 mCategories;

};
#endif
