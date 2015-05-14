/*********************************************************************
*
*   HEADER NAME:
*       StopListItem.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef _STOPLISTITEM_H
#define _STOPLISTITEM_H

#include "ClientListItem.h"
#include "fmi.h"

//! \brief Data structure to hold details of an A603
//!     stop that the server needs to keep.
//! \since Protocol A603
class StopListItem : public ClientListItem
{
public:
    typedef uint32 key_type;

    StopListItem();

    stop_status_status_type getStopStatus() const;

    virtual void readFromStream
        (
        std::istream &aStream
        );

    void setCurrentName
        (
        CString aName
        );

    void setStopStatus
        (
        stop_status_status_type aStatus
        );

    void setParent
        (
        FileBackedMap<StopListItem> * aParent
        );

private:
    //! \brief The last known stop status for this stop.
    //! \note This may be out of date if the stop status protocol is
    //!     throttled or if the update process that occurs when the
    //!     server begins communicating with the client has not
    //!     completed.
    stop_status_status_type mStopStatus;

    //! The map that this item belongs to, or NULL if none
    FileBackedMap<StopListItem> * mParent;
};

#endif
