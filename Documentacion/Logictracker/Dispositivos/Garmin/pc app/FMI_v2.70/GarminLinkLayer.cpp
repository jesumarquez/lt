/*********************************************************************
*
*   MODULE NAME:
*       GarminLinkLayer.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "SerialPort.h"
#include "GarminLinkLayer.h"
#include "GarminPacket.h"
#include "Logger.h"

GarminLinkLayer * GarminLinkLayer::sInstance = NULL;

//----------------------------------------------------------------------
//! \brief Get the one and only link layer object
//! \return The link layer
//! \note GarminLinkLayer is a singleton because this application is
//!     designed to communicate with a single client device.  In a
//!     server-based application, there must be one link layer per
//!     connected client.
//----------------------------------------------------------------------
GarminLinkLayer * GarminLinkLayer::getInstance()
{
    if( sInstance == NULL )
    {
        sInstance = new GarminLinkLayer();
        atexit( GarminLinkLayer::destroyInstance );
    }
    return sInstance;
}

//----------------------------------------------------------------------
//! \brief Delete the one and only link layer object
//----------------------------------------------------------------------
void GarminLinkLayer::destroyInstance()
{
    delete sInstance;
    sInstance = NULL;
}

//----------------------------------------------------------------------
//! \brief Construct and initialize the link layer.
//----------------------------------------------------------------------
GarminLinkLayer::GarminLinkLayer()
{
    mFifo.size      = FIFO_SIZE;
    mFifo.head      = 0;
    mFifo.tail      = 0;

    setPhysicalLayer( SerialPort::getInstance() );
    mPhysicalLayer->setLinkLayer( this );
}

//----------------------------------------------------------------------
//! \brief Copy Data to FIFO
//! \details This procedure appends data (from the PC's com port) to a
//!  local rx FIFO, then calls getPacketFromFifo.
//! \param aData Buffer containing the bytes to add to the FIFO
//! \param aSize Number of bytes to add
//----------------------------------------------------------------------
void GarminLinkLayer::rx
    (
    uint8  const * const aData,
    uint32         const aSize
    )
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    uint8          *ptr;
    uint32          i;


    ptr = (uint8 *)aData;

    for( i = 0; i < aSize; i++ )
    {
        mFifo.buffer[ mFifo.tail ] = ptr[ i ];
        mFifo.tail = ( mFifo.tail >= ( mFifo.size - 1 ) ) ? 0 : mFifo.tail + 1;
    }

    // now frame the packets
    while( getPacketFromFifo() );
}   /* rx() */

//----------------------------------------------------------------------
//! \brief Calculate the number of bytes in the FIFO buffer
//! \details This procedure calculates the number of bytes between the
//!     head and tail of the receive FIFO.
//! \return The number of bytes held in the FIFO buffer.
//----------------------------------------------------------------------
int GarminLinkLayer::numBytesInFifo()
{
    if( mFifo.tail >= mFifo.head )
    {
        return( mFifo.tail - mFifo.head );
    }
    else
    {
        return( ( mFifo.size - mFifo.head ) + mFifo.tail );
    }
}   /* numBytesInFifo() */

//----------------------------------------------------------------------
//! \brief Frame Packet
//! \details This procedure processes raw data in the FIFO into Garmin
//!     packets, and adds the packets to the rx_queue for processing.
//!     While doing so, it also sends ACK/NAK link-level responses as
//!     appropriate, and logs the packets that are received.
//!
//!     Each invocation of getPacketFromFifo will add at most one
//!     packet to the rx_queue.
//! \return TRUE if it found a valid Garmin packet in the FIFO, FALSE
//!     otherwise.
//! \warning This implementation is not thread-safe - this routine
//!     manipulates the fifo head pointer during the search for a
//!     complete packet, then resets it to retrieve the packet;
//!     therefore appending data to the fifo or even obtaining the size
//!     of the buffer during a get operation is unsafe. A
//!     multi-threaded implementation would need to either copy to a
//!     temporary buffer, or protect all fifo use with a mutex.
//----------------------------------------------------------------------
bool GarminLinkLayer::getPacketFromFifo()
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    uint32                              fifoSize;
    header_type                         header;
    footer_type                         footer;
    uint32                              prevHead;
    uint32                              payloadStart;
    uint32                              payloadEnd;
    uint8                               checksum;
    uint8                              *ptr;
    GarminPacket                       *packet;

    //loop until Garmin back found or FIFO empty
    while( mFifo.head != mFifo.tail )
    {
        prevHead    = mFifo.head;
        fifoSize    = numBytesInFifo();
        if( fifoSize < MIN_PACKET_SIZE ) return false;

        getDataFromFifo( (uint8 *)&header, SIZE_OF_HEADER );

        //find header
        if( ( header.dle != ID_DLE_BYTE ) ||
            ( header.id  == ID_ETX_BYTE ) )
        {
            incrementHead( 1 );
            continue;
        }
        //find footer
        incrementHead( SIZE_OF_HEADER );
        payloadStart = mFifo.head;
        payloadEnd = payloadStart;
        while( mFifo.head != mFifo.tail )
        {
            fifoSize = numBytesInFifo();
            if( fifoSize < SIZE_OF_FOOTER )
            {
                break;
            }

            getDataFromFifo( (uint8*)&footer, SIZE_OF_FOOTER );
            if( ( footer.dle == ID_DLE_BYTE ) &&
                ( footer.etx == ID_DLE_BYTE ) )
            {
                incrementHead( 2 );
            }
            else if( ( footer.dle == ID_DLE_BYTE ) &&
                     ( footer.etx == ID_ETX_BYTE ) )
            {
                payloadEnd = mFifo.head;
                break;
            }
            else
                incrementHead( 1 );
        }
        //if footer not found, entire packet not received yet
        if( payloadStart == payloadEnd )
        {
            mFifo.head = prevHead;
            return false;
        }

        //complete packet exists in receive queue
        packet = new GarminPacket;
        mFifo.head = payloadStart;
        packet->mPacketId = header.id;
        packet->mPayloadSize = header.size;
        if( packet->mPayloadSize == ID_DLE_BYTE )
        {
            incrementHead( 1 );
        }
        packet->mFrameSize = 0;
        packet->mChecksum = ( header.id + header.size ) % 256;

        ptr = (uint8*)packet->mPayload;

        while( ( mFifo.head != payloadEnd )
            && ( packet->mFrameSize < packet->mPayloadSize ) )
        {
            getDataFromFifo( (uint8*)&ptr[packet->mFrameSize], 2 );
            packet->mChecksum = ( ptr[ packet->mFrameSize] + packet->mChecksum ) % 256;
            incrementHead( 1 );
            if( ( ptr[ packet->mFrameSize     ] == ID_DLE_BYTE ) &&
                ( ptr[ packet->mFrameSize + 1 ] == ID_DLE_BYTE ) )
                incrementHead( 1 );
            packet->mFrameSize++;
        }

        getDataFromFifo( (uint8*)&checksum, 1 );
        packet->mChecksum = ( checksum + packet->mChecksum ) % 256;
        if( checksum == ID_DLE_BYTE )
        {
            incrementHead( 2 );
        }
        else
        {
            incrementHead( 1 );
        }

        packet->mFrameSize = 0;
        while( prevHead != mFifo.tail )
        {
            packet->mFrame[packet->mFrameSize++] = mFifo.buffer[prevHead];
            //increment prevHead and wrap around to 0 if goes past FIFO boundaries
            prevHead = ( prevHead >= ( mFifo.size - 1 ) ) ? 0 : prevHead + 1;
        }

        Logger::logRawData( packet, FALSE );
        // checksum is 2's complement of sum of all the data...we calculated checksum
        // as just the sum, so if we add them together they should be zero
        if( packet->mChecksum != 0 )
        {
            // Invalid checksum; send NAK and keep looking for a packet.
            if( mTransportLayer )
            {
                mTransportLayer->tx
                    (
                    new GarminPacket( ID_NAK_BYTE, &packet->mPacketId, sizeof( packet->mPacketId ) ),
                    TRUE
                    );
            }
            delete packet;
            mFifo.head = payloadStart;
            continue;
        }
        else
        {
            if( mTransportLayer )
            {
                // Send ACK (if required) and push the packet up the stack for processing.
                // ACK and NAK packets must not be ACKed.
                if( packet->mPacketId != ID_ACK_BYTE &&
                    packet->mPacketId != ID_NAK_BYTE )
                    mTransportLayer->tx( new GarminPacket( ID_ACK_BYTE, &packet->mPacketId, sizeof( packet->mPacketId ) ), TRUE );
                mTransportLayer->rx( packet );
            }
            delete packet;
        }
        incrementHead( SIZE_OF_FOOTER );
        return true;
    }
    return false;
}  /* getPacketFromFifo */

//----------------------------------------------------------------------
//! \brief Get Data From FIFO
//! \details This procedure copies data from the FIFO and places it
//!     into a persistent buffer. It does not advance the pointers
//!     (i.e., the data remains in the buffer).
//! \param aData The data buffer to write to
//! \param aSize The number of bytes to retrieve.
//! \warning Behavior is undefined if packet_size is greater than the
//!     number of bytes in the FIFO.
//----------------------------------------------------------------------
void GarminLinkLayer::getDataFromFifo
    (
    uint8  * aData,
    uint32   aSize
    )
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    int wrapSize;

    if( ( mFifo.head + aSize ) < mFifo.size )
    {
        memcpy( &aData[0], &mFifo.buffer[mFifo.head], aSize );
    }
    else
    {
        wrapSize = mFifo.size - mFifo.head;
        memmove( &aData[0], &mFifo.buffer[mFifo.head], wrapSize );
        memmove( &aData[ wrapSize ], &mFifo.buffer[0], aSize - wrapSize );
    }
} /* getDataFromFifo() */

//----------------------------------------------------------------------
//! \brief Increment FIFO head
//! \details This procedure increments the receive FIFO's head by the
//!    amount requested, effectively removing the data from the FIFO.
//!    It will not increment the head beyond the tail, and it also
//!    accounts for wrapping.
//! \param aIncrement The number of bytes to remove from the FIFO.
//----------------------------------------------------------------------
void GarminLinkLayer::incrementHead
    (
    uint32 const aIncrement
    )
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    uint32          i;

    i = mFifo.head + aIncrement;

    if( mFifo.tail > mFifo.head )
    {
        if( i <= mFifo.tail )
        {
            mFifo.head = i;
        }
        else
        {
            mFifo.head = mFifo.tail;
        }
    }
    else if( mFifo.tail < mFifo.head )
    {
        if( i < mFifo.size )
        {
            mFifo.head = i;
        }
        else
        {
            i -= mFifo.size;
            if( i<mFifo.tail )
            {
                mFifo.head = i;
            }
            else
            {
                mFifo.head = mFifo.tail;
            }
        }
    }
}    /* incrementHead() */

//----------------------------------------------------------------------
//! \brief Transmit a packet
//! \details Build a packet into a raw frame and call the physical
//!    layer to transmit it.
//! \param  aPacket The packet to build into a frame
//! \return True if the packet was sent successfully, false otherwise
//----------------------------------------------------------------------
bool GarminLinkLayer::tx
    (
    Packet * aPacket
    )
{
    GarminPacket * thePacket = dynamic_cast<GarminPacket *>( aPacket );

    ASSERT( thePacket != NULL );

    uint8  i;
    thePacket->mFrameSize           = 0;

    // Frame header (DLE)
    thePacket->mFrame[ thePacket->mFrameSize++ ] = ID_DLE_BYTE;

    // Packet ID
    thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mPacketId;

    // Payload size, DLE stuffing if necessary
    thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mPayloadSize;
    if( thePacket->mPayloadSize == ID_DLE_BYTE )
    {
        thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mPayloadSize;
    }

    // Payload, DLE stuffing as necessary
    for( i = 0; i < thePacket->mPayloadSize; i++)
    {
        thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mPayload[i];
        if( thePacket->mPayload[i] == ID_DLE_BYTE )
        {
            thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mPayload[i];
        }
    }

    // Checksum, DLE stuffing if necessary
    thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mChecksum;
    if( thePacket->mChecksum == ID_DLE_BYTE )
    {
        thePacket->mFrame[ thePacket->mFrameSize++ ] = thePacket->mChecksum;
    }

    // Frame footer (DLE and ETX bytes)
    thePacket->mFrame[ thePacket->mFrameSize++ ] = ID_DLE_BYTE;
    thePacket->mFrame[ thePacket->mFrameSize++ ] = ID_ETX_BYTE;

    Logger::logRawData( thePacket );

    return mPhysicalLayer->tx( thePacket->mFrame, thePacket->mFrameSize );
}
