/*********************************************************************
*
*   HEADER NAME:
*       PhysicalLayer.h
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef PhysicalLayer_H
#define PhysicalLayer_H

class PhysicalLayer;

#include "garmin_types.h"
#include "LinkLayer.h"

//----------------------------------------------------------------------
//! \brief Abstract base class for the physical layer
//! \details The physical layer is the lowest layer in the OSI
//!     layered communication model.  This layer is responsible for
//!     sending and receiving bytes; bytes received are sent to the
//!     link layer for assembly into frames.
//----------------------------------------------------------------------
class PhysicalLayer
{
public:
    //----------------------------------------------------------------------
    //! \brief Transmit data
    //! \param aData The bytes to transmit
    //! \param aSize The number of bytes to transmit
    //! \return true if the data was transmitted, false otherwise
    //----------------------------------------------------------------------
    virtual bool tx
        (
        uint8  * aData,
        uint16   aSize
        ) = 0;

    virtual void setLinkLayer
        (
        LinkLayer* aLinkLayer
        );

protected:
    //! The link layer that is one level up from this serial port
    LinkLayer *  mLinkLayer;
};

#endif /* _PHYSICALLAYER_H */
