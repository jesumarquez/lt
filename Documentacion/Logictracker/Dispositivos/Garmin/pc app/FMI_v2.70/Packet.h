/*********************************************************************
*
*   HEADER NAME:
*       Packet.h
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef Packet_H
#define Packet_H

#include "garmin_types.h"

//----------------------------------------------------------------------
//! \brief Abstract base class for a packet of data
//----------------------------------------------------------------------
class Packet
{
public:
    Packet();
    virtual ~Packet();

    //! \brief Get the size of the raw frame
    //! \return The size of the raw frame in bytes
    virtual uint32 getRawSize() = 0;

    //! \brief Get pointer to the raw frame
    //! \return The pointer to the raw frame.
    virtual const uint8 * getRawBytes() = 0;
};

#endif
