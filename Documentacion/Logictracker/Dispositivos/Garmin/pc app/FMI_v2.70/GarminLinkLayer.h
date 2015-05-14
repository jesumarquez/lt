/*********************************************************************
*
*   HEADER NAME:
*       GarminLinkLayer.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef GarminLinkLayer_H
#define GarminLinkLayer_H

class GarminLinkLayer;

#include "LinkLayer.h"
#include "GarminPacket.h"

//! \brief The size of the server's internal FIFO buffer for parsing
//!     raw data into packets for processing.
#define FIFO_SIZE           ( 1024 )

//----------------------------------------------------------------------
//! \brief Link Layer in the OSI model
//! \details In the OSI model, the Data Link layer provides
//!     encapsulation of data packets into frames, frame
//!     synchronization, and error control.  For Fleet Management,
//!     encapsulation means adding the frame header, frame footer, and
//!     sequencing and DLE-stuffing the elements of the packet before
//!     transmitting a packet as a frame, and removing the DLE stuffing
//!     and verifying the checksum on incoming frames.  Frame
//!     synchronization is performed by receiving bytes from the serial
//!     port into a FIFO buffer, then searching the data received to
//!     find valid frames (starting with DLE and ending with DLE-ETX),
//!     and sending any valid frames received up to the TransportLayer.
//!     Error control means responding to each received frame with
//!     either ACK or NAK, depending on whether the frame was received
//!     correctly.
//----------------------------------------------------------------------
class GarminLinkLayer : public LinkLayer
{
public:
    static GarminLinkLayer * getInstance();
    static void destroyInstance();

    virtual bool tx
        (
        Packet * aPacket
        );

    virtual void rx
        (
        uint8  const * const aData,
        uint32 const         aSize
        );

private:
    //! A circular FIFO buffer for processing received bytes into packets
    struct fifo_type
    {
        uint32              size;                       //!< Number of bytes of data in the buffer
        uint32              head;                       //!< Index of first byte of data
        uint32              tail;                       //!< Index of last byte of data
        uint8               buffer[ FIFO_SIZE ];        //!< Data buffer
    };

    //! \brief Construct a new GarminLinkLayer
    GarminLinkLayer();

    int  numBytesInFifo();
    bool getPacketFromFifo();
    void getDataFromFifo
        (
        uint8  * aData,
        uint32   aSize
        );
    void incrementHead
        (
        uint32 aIncrement
        );
protected:

private:
    //! The one and only instance of this class
    static GarminLinkLayer *   sInstance;

    //! FIFO (First-In, First-Out) buffer used to store bytes received
    //! from the client until they are parsed into packets
    fifo_type                  mFifo;
};

#endif
