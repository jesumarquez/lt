/*********************************************************************
*
*   HEADER NAME:
*       GarminPacket.h
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef GarminPacket_H
#define GarminPacket_H

class GarminPacket;

#include <time.h>
#include "ApplicationLayer.h"
#include "Packet.h"

//----------------------------------------------------------------------
//! \brief Garmin serial packet
//! \details Encapsulates a Garmin serial packet (packet ID, payload
//!     data, payload size) along with related data needed to track the
//!     packet's state (sender application for onAck, and transmit time
//!     to detect timeouts).
//----------------------------------------------------------------------
class GarminPacket : public Packet
{
public:
    GarminPacket();

    GarminPacket
        (
        id_type            aId,
        uint8            * aPayloadData = NULL,
        uint8              aPayloadSize = 0,
        ApplicationLayer * aSender      = NULL
        );
    virtual ~GarminPacket();

    virtual uint32 getRawSize();
    virtual const uint8 * getRawBytes();

    //! A complete serial packet.
    id_type            mPacketId;                     //!< Garmin packet ID
    time_t             mTxTime;                       //!< Time when the packet was last sent
    uint8              mPayloadSize;                  //!< Size of the payload, before DLE stuffing
    uint16             mFrameSize;                    //!< Number of bytes in the packet, with header, footer, and DLE stuffing
    uint8              mChecksum;                     //!< Twos-complement checksum of this packet
    uint8              mPayload[MAX_PAYLOAD_SIZE];    //!< The payload, before DLE stuffing
    uint8              mFrame[MAX_PACKET_SIZE];       //!< The packet as transmitted on the serial link (with DLE stuffing)
    ApplicationLayer * mSender;                       //!< The ApplicationLayer that originated this packet (for ACK callback)
};

#endif
