/*********************************************************************
*
*   HEADER NAME:
*       MessageId.h
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#ifndef MESSAGEID_H
#define MESSAGEID_H

#include "stdafx.h"
#include <fstream>

#include "fmi.h"

//----------------------------------------------------------------------
//! Encapsulation of a message ID.
//! \details Encapsulates a message ID (0-16 bytes of binary data), and
//!     provides methods to compare and copy message IDs, and convert
//!     the ID to and from a string representation.
//! \since Protocol A602
//----------------------------------------------------------------------
class MessageId
{
public:
    MessageId();
    MessageId
        (
        const MessageId & aRightSide
        );
    MessageId
        (
        const uint8   aIdSize,
        const uint8 * aId
        );
    MessageId
        (
        const CString & aCString,
        codepage_type   aCodePage
        );

    bool operator<
        (
        const MessageId & aRightSide
        ) const;

    const MessageId& operator=
        (
        const MessageId & aRightSide
        );

    bool operator==
        (
        const MessageId & aRightSide
        ) const;

    const uint8 * getId() const;

    uint8 getIdSize() const;

    CString toCString
        (
        codepage_type aCodePage
        ) const;

private:
    uint8                mId[16];          //!< The message ID
    uint8                mIdSize;          //!< Number of significant bytes of mId
};

#endif
