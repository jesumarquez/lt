/*********************************************************************
*
*   HEADER NAME:
*       ClientListItem.h
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef _CLIENTLISTITEM_H
#define _CLIENTLISTITEM_H

#include "stdafx.h"
#include <map>
#include <fstream>

class ClientListItem;

#include "garmin_types.h"
#include "FileBackedMap.h"

//! \brief Stores the name associated with a list item
//!     that is sent to the client.
//! \since Protocol A602
class ClientListItem
{
public:
    //! key type
    typedef uint32 key_type;

    ClientListItem();

    void commitName();

    CString getCurrentName() const;

    const key_type & getId() const;

    BOOL isValid() const;

    virtual void readFromStream
        (
        std::istream &aStream
        );

    void setId
        (
        const key_type & aId
        );

    void setParent
        (
        FileBackedMap<ClientListItem>* aParent
        );

    void setUpdateName
        (
        CString aName
        );

    void setValid
        (
        BOOL aValid = TRUE
        );

    virtual void writeToStream
        (
        std::ofstream &aStream
        ) const;

protected:
    virtual void save();

    //! The last name acknowledged by the client
    CString             mCurrentName;

    //! The last name entered by the user
    CString             mUpdateName;

    //! True if this list item is mIsValid
    BOOL                mIsValid;

    //! The unique ID of this item
    key_type            mId;
private:
    //! The map that this item belongs to, or NULL if none
    FileBackedMap<ClientListItem>* mParent;
};

#endif
