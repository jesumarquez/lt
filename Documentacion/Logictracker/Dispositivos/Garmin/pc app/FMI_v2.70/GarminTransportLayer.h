/*********************************************************************
*
*   HEADER NAME:
*       GarminTransportLayer.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef GarminTransportLayer_H
#define GarminTransportLayer_H

class GarminTransportLayer;

#include <time.h>

#include "TimerListener.h"
#include "TransportLayer.h"
#include "ApplicationLayer.h"
#include "GarminLinkLayer.h"
#include "GarminPacket.h"

//----------------------------------------------------------------------
//! \brief Transport layer in the OSI communication model
//! \details In the OSI model, the transport layer is responsible for
//!    the reliable delivery of packets to the partner device.  For the
//!    Fleet Management Interface, this means forming data packets from
//!    the Garmin packet ID, payload size, and payload and passing them
//!    down to the GarminLinkLayer.
//!    The GarminTransportLayer also processes incoming ACK and NAK
//!    packets, and determines when the link has timed out.  In this
//!    implementation, link timeouts are handled by sending an event to
//!    the UI, but a robust implementation might retry the packet or
//!    reset the link.
//!    Packets are maintained in a queue; each packet must be ACK'd by
//!    the client before the next packet is sent. The application layer
//!    may request that a packet be inserted at the head of the queue;
//!    this is necessary for FMI Enable packets and for ACK/NAK
//!    responses to incoming packets from the client, as these need to
//!    be sent before any other packets that may be in the queue.
//----------------------------------------------------------------------
class GarminTransportLayer : public TransportLayer, public TimerListener
{

private:
    //! A list of packets
    typedef std::list<GarminPacket *> PacketList;

public:
    static GarminTransportLayer * getInstance();
    static void destroyInstance();

    virtual void tx
        (
        Packet * aPacket,
        bool     aSendImmediate
        );
    virtual void onTimer();

    void rx
        (
        Packet * aPacket
        );
    virtual void retry();

private:
    GarminTransportLayer();
    virtual ~GarminTransportLayer();

    bool sendPacket();

    //! The one and only instance of this class
    static GarminTransportLayer * sInstance;

    //! The link layer that frames are sent/received from
    GarminLinkLayer             * mLinkLayer;

    //! Queue of packets being transmitted; the head of the queue
    //! may have been sent but is not ACKed.
    PacketList                    mTransmitQueue;

    //! If true, a communication error has occurred (transmit is suspended)
    bool                          mCommError;
};

#endif
