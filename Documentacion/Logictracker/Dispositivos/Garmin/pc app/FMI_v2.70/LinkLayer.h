/*********************************************************************
*
*   HEADER NAME:
*       LinkLayer.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef LinkLayer_H
#define LinkLayer_H

class LinkLayer;

#include "PhysicalLayer.h"
#include "TransportLayer.h"
#include "Packet.h"

//----------------------------------------------------------------------
//! \brief Abstract base class for link Layers in the OSI model
//! \details In the OSI model, the Data Link layer provides
//!    encapsulation of data packets into frames, frame
//!    synchronization, and possibly error control for errors at the
//!    physical layer.
//----------------------------------------------------------------------
class LinkLayer
{

public:
    //----------------------------------------------------------------------
    //! \brief Transmit data to the physical layer
    //! \details The TransportLayer calls this function when a packet
    //!     should be sent.  The LinkLayer should send the raw bytes
    //!     to the physical layer if it is up.
    //! \param aPacket The Packet to transmit.
    //! \return true if the data was sent, false otherwise.
    //----------------------------------------------------------------------
    virtual bool tx
        (
        Packet * aPacket
        ) = 0;

    //----------------------------------------------------------------------
    //! \brief Receive data from the physical layer
    //! \details The physical layer calls this function when data
    //!     is received.  The LinkLayer should append the data to
    //!     what has been received so far, then find any frames and
    //!     pass them up to the TransportLayer for processing.
    //! \param aData The data received
    //! \param aSize The number of bytes received.
    //----------------------------------------------------------------------
    virtual void rx
        (
        uint8 const * const aData,
        uint32        const aSize
        ) = 0;

    void setPhysicalLayer
        (
        PhysicalLayer * aPort
        );

    void setTransportLayer
        (
        TransportLayer * aTransport
        );

protected:
    LinkLayer();

    virtual ~LinkLayer();

    //! The serial port that this link layer communicates with.
    PhysicalLayer *            mPhysicalLayer;

    //! The transport layer that this link layer communicates with.
    TransportLayer *           mTransportLayer;
};

#endif
