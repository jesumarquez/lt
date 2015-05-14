/*********************************************************************
*
*   MODULE NAME:
*       LinkLayer.cpp
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "LinkLayer.h"

//----------------------------------------------------------------------
//! \brief Construct a LinkLayer
//! \details Construct a link layer.  Initially the LinkLayer is not
//!     connected to a PhysicalLayer or a TransportLayer.
//----------------------------------------------------------------------
LinkLayer::LinkLayer()
    : mPhysicalLayer( NULL )
    , mTransportLayer( NULL )
{

}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
LinkLayer::~LinkLayer()
{
    if( mPhysicalLayer )
        mPhysicalLayer->setLinkLayer( NULL );
}

//----------------------------------------------------------------------
//! \brief Set the physical layer
//! \details Set the physical layer that this LinkLayer sends bytes to
//!     and receives bytes from.
//! \param aPort The physical layer, or NULL if none.
//----------------------------------------------------------------------
void LinkLayer::setPhysicalLayer
    (
    PhysicalLayer * aPort
    )
{
    mPhysicalLayer = aPort;
}

//----------------------------------------------------------------------
//! \brief Set the transport layer
//! \details Set the transport layer that this LinkLayer sends packets
//!     to and receives packets from.
//! \param aTransport The physical layer, or NULL if none.
//----------------------------------------------------------------------
void LinkLayer::setTransportLayer
    (
    TransportLayer * aTransport
    )
{
    mTransportLayer = aTransport;
}
