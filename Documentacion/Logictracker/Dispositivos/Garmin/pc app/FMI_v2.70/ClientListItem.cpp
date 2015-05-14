/*********************************************************************
*
*   MODULE NAME:
*       ClientListItem.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "ClientListItem.h"

//----------------------------------------------------------------------
//! \brief Constructor
//! \details Create a ClientListItem without a name or parent
//----------------------------------------------------------------------
ClientListItem::ClientListItem()
{
    mCurrentName = _T("");
    mUpdateName = _T("");
    mIsValid = FALSE;
    mParent = NULL;
}

//----------------------------------------------------------------------
//! \brief Commit the item name
//! \details Commit the item name and set the ClientListItem as valid
//----------------------------------------------------------------------
void ClientListItem::commitName()
{
    mCurrentName = mUpdateName;
    setValid();
}

//----------------------------------------------------------------------
//! \brief Get the current name of this item
//! \return The current (committed) name
//----------------------------------------------------------------------
CString ClientListItem::getCurrentName() const
{
    return mCurrentName;
}

//----------------------------------------------------------------------
//! \brief Get item's ID
//! \details Get the unique ID of this item
//! \return The item's ID
//----------------------------------------------------------------------
const ClientListItem::key_type & ClientListItem::getId() const
{
    return mId;
}

//----------------------------------------------------------------------
//! \brief Check whether this item is valid
//! \return TRUE if the item is valid, FALSE otherwise
//----------------------------------------------------------------------
BOOL ClientListItem::isValid() const
{
    return mIsValid;
}

//----------------------------------------------------------------------
//! \brief Read a ClientListItem from an input stream
//! \details Read id and current name from the input stream, and
//!     updates the member variables appropriately.
//! \param aStream The stream to read from
//----------------------------------------------------------------------
void ClientListItem::readFromStream
    (
    std::istream &aStream
    )
{
    char utf8Name[200];
    TCHAR tcharName[200];
    int idx = 0;
    char temp;

    memset( utf8Name, 0, sizeof( utf8Name ) );

    aStream >> mId;
    aStream.get();    // consume '-' inserted between id and name

    while( aStream.peek() != '\n' && !aStream.eof() )
    {
        aStream.read( &temp, 1 );
        utf8Name[idx++] = temp;
    }
    aStream.get();    // consume '\n' inserted after name
    MultiByteToWideChar( CP_UTF8, 0, utf8Name, -1, tcharName, 200 );
    mCurrentName.SetString( tcharName );
    mUpdateName.SetString( _T("") );
    mIsValid = TRUE;
}

//----------------------------------------------------------------------
//! \brief Set item's ID
//! \param aId The item's ID
//----------------------------------------------------------------------
void ClientListItem::setId
    ( 
    const ClientListItem::key_type & aId
    )
{
    mId = aId;
}

//----------------------------------------------------------------------
//! \brief Set the parent
//! \param aParent The FileBackedMap that contains this item
//----------------------------------------------------------------------
void ClientListItem::setParent
    (
    FileBackedMap<ClientListItem>* aParent
    )
{
    mParent = aParent;
}

//----------------------------------------------------------------------
//! \brief Set pending name
//! \details Set the pending name of this ClientListItem.  This does
//!     not become the item's name until commitName is called.
//! \param aName The pending name to set
//----------------------------------------------------------------------
void ClientListItem::setUpdateName
    (
    CString aName
    )
{
    mUpdateName = aName;
}

//----------------------------------------------------------------------
//! \brief Set this item as valid
//! \details Set the valid flag.  If this item is part of a
//!    FileBackedMap, save the map.
//! \param aValid If TRUE, the item is valid.
//----------------------------------------------------------------------
void ClientListItem::setValid
    (
    BOOL aValid
    )
{
    mIsValid = aValid;
    save();
}

//----------------------------------------------------------------------
//! \brief Write a ClientListItem to an output stream
//! \details Append id and current name to the output stream.
//! \param aStream The stream to write to
//----------------------------------------------------------------------
void ClientListItem::writeToStream
    (
    std::ofstream &aStream
    ) const
{
    int nameLength = WideCharToMultiByte( CP_UTF8, 0, mCurrentName, -1, NULL, 0, NULL, NULL );
    char* name = new char[nameLength];

    WideCharToMultiByte( CP_UTF8, 0, mCurrentName, -1, name, nameLength, NULL, NULL );
    aStream << mId << "-" << name << '\n';

    delete [] name;
}

//----------------------------------------------------------------------
//! \brief Save this item
//! \details Invokes parent to save all items
//----------------------------------------------------------------------
void ClientListItem::save()
{
    if( mParent )
    {
        mParent->save();
    }
}
