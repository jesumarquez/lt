/*********************************************************************
*
*   HEADER NAME:
*       ApplicationLayer.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef ApplicationLayer_H
#define ApplicationLayer_H

class ApplicationLayer;

#include "garmin_types.h"
#include "TransportLayer.h"
#include "Packet.h"

//----------------------------------------------------------------------
//! \brief Abstract base class for the application layer in the OSI
//!     model
//! \details The ApplicationLayer sends packets to and receives packets
//!     from the TransportLayer.  A mechanism is also provided that
//!     allows the ApplicationLayer to be notified when the other
//!     device has received and acknowledged a particular packet.
//----------------------------------------------------------------------
class ApplicationLayer
{
public:
    ApplicationLayer
        (
        TransportLayer * aTransportLayer
        );

    virtual ~ApplicationLayer();

    virtual void disconnectTransportLayer();

    //! \brief Callback for received packets
    //! \param aPacket The aPacket that was received
    //! \return true if the aPacket was processed, false otherwise
    virtual bool rx
        (
        const Packet * aPacket
        ) = 0;

    //! \brief Callback when a sent packet is ACKed
    //! \param aPacket The packet that was ACKed
    virtual void onAck
        (
        const Packet * aPacket
        ) = 0;

protected:
    //! The next layer down in the stack
    TransportLayer* mTransportLayer;
};

#endif
