/*********************************************************************
*
*   MODULE NAME:
*       InboxListItem.cpp
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "InboxListItem.h"

//----------------------------------------------------------------------
//! \brief Set the ID associated with this item
//! \param aId The item's ID
//----------------------------------------------------------------------
void InboxListItem::setId
    (
    const key_type & aId
    )
{
    messageId = aId;
}

//----------------------------------------------------------------------
//! \brief Get the ID associated with this item
//! \return The item's ID
//----------------------------------------------------------------------
const InboxListItem::key_type & InboxListItem::getId() const
{
    return messageId;
}

//----------------------------------------------------------------------
//! \brief Set the parent of this item
//! \param aParent The item's parent
//----------------------------------------------------------------------
void InboxListItem::setParent
    (
    FileBackedMap<InboxListItem> * aParent
    )
{
    mParent = aParent;
}

//----------------------------------------------------------------------
//! \brief Read an InboxListItem from an istream
//! \param aStream The stream to read from
//----------------------------------------------------------------------
void InboxListItem::readFromStream
    (
    std::istream &aStream
    )
{
    int             i;
    int             num;
    char            dummy;
    uint8           idSize;
    uint8           id[16];

    i = 0;
    aStream >> num;
    idSize = (uint8)num;
    aStream >> dummy;    // ignore '-' inserted between id size and id
    memset( id, 0, 16 );
    while( aStream.peek() != '\n' && !aStream.eof() )
    {
        aStream >> num;
        aStream >> dummy; //ignore',' inserted between chars of id
        if( i < 16 )
            id[i++] = (uint8)num;
    }
    aStream >> dummy; // ignore '\n' inserted after item
    messageId = MessageId( idSize, id );
    mIsValid = TRUE;
}

//----------------------------------------------------------------------
//! \brief Write an InboxListItem to an output stream
//! \param aStream The stream to write to
//----------------------------------------------------------------------
void InboxListItem::writeToStream
    (
    std::ofstream & aStream
    )
{
    const uint8 * id = messageId.getId();

    aStream << messageId.getIdSize() << '-';
    for( int i = 0; i < messageId.getIdSize(); i++ )
    {
        aStream << id[i] << ',';
    }
    aStream << '\n';
}
