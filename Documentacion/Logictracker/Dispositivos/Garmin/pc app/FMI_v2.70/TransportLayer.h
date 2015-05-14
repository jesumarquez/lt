/*********************************************************************
*
*   HEADER NAME:
*       TransportLayer.h
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef TransportLayer_H
#define TransportLayer_H

#include <list>

class TransportLayer;

#include "Packet.h"
#include "ApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Abstract base class for transport layer in the OSI model
//! \details In the OSI model, the transport layer is responsible for
//!     the reliable delivery of packets to the partner device.  It
//!     handles flow control (sending packets to the link layer at the
//!     appropriate rate) and error control (handling errors and
//!     retransmitting or timing out as appropriate).
//----------------------------------------------------------------------
class TransportLayer
{
public:
    TransportLayer();
    virtual ~TransportLayer();

    //----------------------------------------------------------------------
    //! \brief Transmit a packet
    //! \param aPacket The packet to transmit
    //! \param aSendNow If true, the packet should be sent immediately (not FIFO)
    //----------------------------------------------------------------------
    virtual void tx
        (
        Packet * aPacket,
        bool     aSendNow
        ) = 0;

    //----------------------------------------------------------------------
    //! \brief Retry the last transmit
    //----------------------------------------------------------------------
    virtual void retry() = 0;

    //----------------------------------------------------------------------
    //! \brief Callback when a complete packet is assembled by the LinkLayer.
    //! \param aPacket The packet that was received
    //----------------------------------------------------------------------
    virtual void rx
        (
        Packet * aPacket
        ) = 0;

    virtual void addAppLayer
        (
        ApplicationLayer * aAppLayer
        );

    virtual void removeAppLayer
        (
        ApplicationLayer * aAppLayer
        );

protected:
    //! A list of app layers that are connected to this link layer
    typedef std::list<ApplicationLayer *> AppLayerList;

    //! The application layer objects that send/receive using this
    //!     GarminTransportLayer
    AppLayerList                 mAppLayers;
};

#endif
