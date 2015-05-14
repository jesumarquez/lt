/*********************************************************************
*
*   MODULE NAME:
*       GarminPacket.cpp
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "GarminPacket.h"

//----------------------------------------------------------------------
//! \brief Construct a Garmin RS232 packet
//! \param aId The packet ID
//! \param aPayloadData The packet payload
//! \param aPayloadSize The size, in bytes, of the packet payload
//! \param aSender Application layer that constructed this packet
//----------------------------------------------------------------------
GarminPacket::GarminPacket
    (
    id_type            aId,
    uint8            * aPayloadData,
    uint8              aPayloadSize,
    ApplicationLayer * aSender
    )
    : mPacketId( aId )
    , mPayloadSize( aPayloadSize )
    , mSender( aSender )
{
    memcpy( mPayload, aPayloadData, mPayloadSize );
}

//----------------------------------------------------------------------
//! \brief Construct an empty Garmin RS232 packet
//----------------------------------------------------------------------
GarminPacket::GarminPacket()
    : mPacketId( 0 )
    , mPayloadSize( 0 )
    , mSender( NULL )
{

}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
GarminPacket::~GarminPacket()
{

}

//----------------------------------------------------------------------
//! \brief Get the raw size of the frame, in bytes
//! \return The number of bytes in the raw frame
//----------------------------------------------------------------------
uint32 GarminPacket::getRawSize()
{
    return mFrameSize;
}

//----------------------------------------------------------------------
//! \brief Get the bytes in the raw frame
//! \return Pointer to the raw frame
//----------------------------------------------------------------------
const uint8 * GarminPacket::getRawBytes()
{
    return mFrame;
}
