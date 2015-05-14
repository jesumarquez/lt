/*********************************************************************
*
*   MODULE NAME:
*       MessageId.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "MessageId.h"
#include "util.h"

//----------------------------------------------------------------------
//! \brief Default constructor
//! \details Construct a message ID with size zero.
//----------------------------------------------------------------------
MessageId::MessageId() : mIdSize(0)
{
}

//----------------------------------------------------------------------
//! \brief Copy constructor
//! \param aRightSide The MessageId to construct a copy of.
//----------------------------------------------------------------------
MessageId::MessageId
    (
    const MessageId & aRightSide
    )
{
    mIdSize = aRightSide.mIdSize;
    memcpy( mId, aRightSide.mId, mIdSize );
}


//----------------------------------------------------------------------
//! \brief Construct message ID from raw binary ID and length
//! \param aIdSize The length of the message ID in bytes.  If this is
//!     greater than 16, a length of 16 will be used.
//! \param aId The binary message ID.
//----------------------------------------------------------------------
MessageId::MessageId
    (
    const uint8   aIdSize,
    const uint8 * aId
    )
{
    mIdSize = minval( aIdSize, sizeof( mId ) );
    memcpy( mId, aId, mIdSize );
}

//----------------------------------------------------------------------
//! \brief Construct message ID from displayable string
//! \param aCString The displayable representation of the string.  If
//!     this begins with "0x" then the rest of the string will be
//!     interpreted as hexadecimal, otherwise the string will be
//!     converted to the specified codepage.
//! \param aCodePage The code page to use (ASCII or Unicode)
//----------------------------------------------------------------------
MessageId::MessageId
    (
    const CString & aCString,
    codepage_type   aCodePage
    )
{
    char  messageIdHex[35];
    //assume in hex
    WideCharToMultiByte( aCodePage, 0, aCString, -1, messageIdHex, 34, NULL, NULL );

    //check for '0x' in the message ID
    if( strncmp( messageIdHex, "0x", 2 ) == 0 )
    {
        mIdSize = (uint8)UTIL_hex_to_uint8( messageIdHex + 2, mId, 16 );
    }
    else
    {
        mIdSize = (uint8)minval( 16, strlen( messageIdHex ) );
        memmove( mId, messageIdHex, mIdSize );
    }
}

//----------------------------------------------------------------------
//! \brief Less-than operator
//! \details Less-than operator for sorting purposes.  A message ID is
//!     considered less than another if the mIdSize is smaller or if
//!     the mId array itself is smaller in a strict binary comparison.
//! \param aRightSide The right hand side of the expression.
//----------------------------------------------------------------------
bool MessageId::operator<
    (
    const MessageId & aRightSide
    ) const
{
    if( mIdSize != aRightSide.mIdSize )
        return mIdSize < aRightSide.mIdSize;
    else
        return memcmp( mId, aRightSide.mId, mIdSize ) < 0;
}

//----------------------------------------------------------------------
//! \brief Assignment operator
//! \param aRightSide The right hand side of the assignment expression.
//----------------------------------------------------------------------
const MessageId& MessageId::operator=
    (
    const MessageId & aRightSide
    )
{
    if( this != &aRightSide )
    {
        mIdSize = aRightSide.mIdSize;
        memset( mId, 0, sizeof( mId ) );
        memcpy( mId, aRightSide.mId, minval( mIdSize, sizeof( mId ) ) );
    }
    return *this;
}

//----------------------------------------------------------------------
//! \brief Equality operator
//! \details Equality operator for comparison purposes.  A message ID is
//!     considered equal to another if both the mIdSize and the
//!     significant bytes of the mId array itself are binary equal.
//! \param aRightSide The right hand side of the expression.
//----------------------------------------------------------------------
bool MessageId::operator==
    (
    const MessageId& aRightSide
    ) const
{
    if( this->mIdSize != aRightSide.mIdSize )
        return false;

    if( 0 != memcmp( mId, aRightSide.mId, mIdSize ) )
        return false;

    return true;
}

//----------------------------------------------------------------------
//! \brief Return a reference to the bytes of the message ID
//! \return Pointer to the message ID bytes.  This pointer is valid for
//!     the lifetime of the MessageID object.
//----------------------------------------------------------------------
const uint8 * MessageId::getId() const
{
    return mId;
}
//----------------------------------------------------------------------
//! \brief Return the size of the message ID
//! \return The size of the message ID (0..16).
//----------------------------------------------------------------------
uint8 MessageId::getIdSize() const
{
    return mIdSize;
}

//----------------------------------------------------------------------
//! \brief CString representation of the message ID
//! \details Returns a CString representation of the message ID. If the
//!     string consists exclusively of printable characters, the
//!     resulting string will interpret the message ID as text.
//!     Otherwise, the resulting string will be "0x" followed by the
//!     message ID in hexadecimal.
//! \param aCodePage The code page to convert from.
//----------------------------------------------------------------------
CString MessageId::toCString
    (
    codepage_type aCodePage
    ) const
{
    TCHAR  messageIdWide[35];
    char   messageId[35];
    CString formattedId;

    memset( messageId, 0, 35 );
    if( UTIL_data_is_printable( (const char *)mId, mIdSize ) )
    {
        strncpy( messageId, (const char *)mId, mIdSize );
    }
    else
    {
        strcpy( messageId, "0x" );
        UTIL_uint8_to_hex( mId, &messageId[2], mIdSize );
    }
    MultiByteToWideChar( aCodePage, 0, messageId, -1, messageIdWide, 35 );
    messageIdWide[34] = '\0';
    formattedId.Format( _T(" %s"), messageIdWide );

    return formattedId;
}
