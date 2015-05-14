/*********************************************************************
*
*   MODULE NAME:
*       GarminTransportLayer.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "GarminTransportLayer.h"
#include "Event.h"
#include "Logger.h"

#define COM_TIMEOUT 3                   //!< timeout in seconds

GarminTransportLayer * GarminTransportLayer::sInstance = NULL;

//----------------------------------------------------------------------
//! \brief Get the one and only GarminTransportLayer
//! \return The GarminTransportLayer.
//! \note An server that connects to multiple client devices should
//!    have a GarminTransportLayer object for each device.
//----------------------------------------------------------------------
GarminTransportLayer * GarminTransportLayer::getInstance()
{
    if( sInstance == NULL )
    {
        sInstance = new GarminTransportLayer;
        atexit( GarminTransportLayer::destroyInstance );
    }
    return sInstance;
}

//----------------------------------------------------------------------
//! \brief Delete the one and only GarminTransportLayer instance
//----------------------------------------------------------------------
void GarminTransportLayer::destroyInstance()
{
    delete sInstance;
    sInstance = NULL;
}

//----------------------------------------------------------------------
//! \brief Construct a GarminTransportLayer
//----------------------------------------------------------------------
GarminTransportLayer::GarminTransportLayer()
    : mLinkLayer( GarminLinkLayer::getInstance() )
{
    mCommError = false;
    mLinkLayer->setTransportLayer( this );
}

//----------------------------------------------------------------------
//! \brief GarminTransportLayer destructor
//! \details Empty the transmit queue
//----------------------------------------------------------------------
GarminTransportLayer::~GarminTransportLayer()
{
    if( mLinkLayer )
    {
        mLinkLayer->setTransportLayer( NULL );
    }

    while( mAppLayers.size() > 0 )
    {
        mAppLayers.front()->disconnectTransportLayer();
        mAppLayers.pop_front();
    }

    while( !mTransmitQueue.empty() )
    {
        const GarminPacket * packet = mTransmitQueue.front();
        mTransmitQueue.pop_front();
        delete packet;
    }
}
//----------------------------------------------------------------------
//! \brief Packet receive callback
//! \details This function is called by the GarminLinkLayer when a
//!     complete packet is received.  It processes ACK and NAK packets,
//!     and passes any others up to the application layer.  When an
//!     ACK is received, the next packet (if any) in the transmit queue
//!     is sent.  When a NAK is received, the packet at the head of the
//!     queue (if any) is resent.
//! \param  aPacket The received packet.
//----------------------------------------------------------------------
void GarminTransportLayer::rx
    (
    Packet * aPacket
    )
{
    GarminPacket * thePacket = dynamic_cast<GarminPacket *>( aPacket );
    ASSERT( thePacket != NULL );
    ASSERT( mAppLayers.size() != 0 );

    // Handle the packet that was received.
    switch( thePacket->mPacketId )
    {
    case ID_ACK_BYTE:
        {
            //packet was received at client, remove from queue
            //and send the next (if there was one)
            if( !mTransmitQueue.empty() )
            {
                const GarminPacket * packet = mTransmitQueue.front();

                if( packet->mSender != NULL )
                    packet->mSender->onAck( packet );

                mTransmitQueue.pop_front();
                delete packet;
            }
            if( !mTransmitQueue.empty() )
                sendPacket();
            break;
        }
    case ID_NAK_BYTE:
        //NAK means error in communication...resend the same
        // packet again... if there was one
        if( !mTransmitQueue.empty() )
            sendPacket();
        break;
    default:
        {
            // All other packets are sent to the app layer for processing.
            AppLayerList::iterator iter;
            for( iter = mAppLayers.begin(); iter != mAppLayers.end(); iter++ )
            {
                if( (*iter)->rx( thePacket ) )
                    break;
            }
        }
    }
}

//----------------------------------------------------------------------
//! \brief Periodic callback
//! \details The Transport Layer is responsible for timeout handling.
//!    This implementation posts an FMI_COMM_TIMEOUT event if more
//!    than 3 seconds have elapsed since the packet at the head of the
//!    transmit queue was last sent.
//! \note A more robust implementation might retry the last packet
//!    several times, resend an enable, etc. Also, the 3 second timeout
//!    is based on the fact that the link is direct serial and thus
//!    low-latency, and it is expected that the client device is always
//!    powered.
//----------------------------------------------------------------------
void GarminTransportLayer::onTimer()
{
    if( !mCommError && !mTransmitQueue.empty() )
    {
        time_t currentTime;
        double elapsed;

        time( &currentTime );
        elapsed = difftime( currentTime, mTransmitQueue.front()->mTxTime );
        if( elapsed > COM_TIMEOUT )
        {
            Event::post( EVENT_COMM_TIMEOUT );
            mCommError = true;
        }
    }
}

//----------------------------------------------------------------------
//! \brief Tries to resend the most recently sent packet.
//----------------------------------------------------------------------
void GarminTransportLayer::retry()
{
    mCommError = false;
    sendPacket();
}

//----------------------------------------------------------------------
//! \brief Transmit a Packet
//! \details This procedure builds a packet, adds it to the transmit
//!     queue, and, if appropriate, calls sendPacket to send it. If the
//!     transmitted packet is an ACK or NAK, this function does not
//!     leave the packet on the queue; otherwise the packet is kept on
//!     the queue for later matching to a received ACK packet.  If
//!     send_now is false and mTransmitQueue is not empty, the packet
//!     is added to the queue instead of being sent.  This is to
//!     comply with the requirement that only one packet be transmitted
//!     at a time.
//! \param aPacket The packet to send
//! \param aSendNow If true, send the packet immediately; if false,
//!     queue the packet if another is waiting for an ACK.  ACK, NAK,
//!     and Enable packets should have this set to TRUE.
//----------------------------------------------------------------------
void GarminTransportLayer::tx
    (
    Packet * aPacket,
    bool     aSendNow
    )
{
    GarminPacket * packet = dynamic_cast<GarminPacket *>( aPacket );

    ASSERT( packet != NULL );

    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    int             i;

    packet->mChecksum = ( packet->mPacketId + packet->mPayloadSize ) & 0xFF;

    //calculate rest of checksum
    for( i = 0; i < packet->mPayloadSize; i++ )
    {
        packet->mChecksum = ( packet->mPayload[i] + packet->mChecksum ) & 0xFF;
    }

    //the checksum is the 2's complement of all the data
    //in the packet
    packet->mChecksum = ( ( packet->mChecksum ^ 0xFF ) + 1 ) & 0xFF;

    if( aSendNow || mTransmitQueue.empty() )
    {
        mTransmitQueue.push_front( packet );
        if( !mCommError )
        {
            bool result;
            result = sendPacket();
            if( result )
            {
                if( packet->mPacketId == ID_ACK_BYTE ||
                    packet->mPacketId == ID_NAK_BYTE )
                {
                    mTransmitQueue.pop_front();
                    delete packet;
                }
            }
        }
    }
    else
    {
        mTransmitQueue.push_back( packet );
    }
    return;
}

//----------------------------------------------------------------------
//! \brief Send a Packet
//! \details This procedure sends the packet at the head of
//!     mTransmitQueue to the GarminLinkLayer.
//! \return TRUE if the packet was sent successfully, FALSE otherwise.
//!     Note that this does not mean the packet was ACKed, only
//!     that there wasn't a comm error on the server side.
//! \warning This function assumes the packet is complete, but does
//!     not check.
//----------------------------------------------------------------------
bool GarminTransportLayer::sendPacket()
{
    ASSERT( !mTransmitQueue.empty() );

    GarminPacket *packet = mTransmitQueue.front();

    time( &packet->mTxTime );

    return mLinkLayer->tx( packet );
}
