/*********************************************************************
*
*   MODULE NAME:
*       PhysicalLayer.cpp
*
* Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "PhysicalLayer.h"

//----------------------------------------------------------------------
//! \brief Change the link layer that receives bytes from this
//!    SerialPort.
//! \param aLinkLayer The LinkLayer object that receives bytes, or NULL
//!    if no LinkLayer should receive data from this port.
//----------------------------------------------------------------------
void PhysicalLayer::setLinkLayer( LinkLayer* aLinkLayer )
{
    mLinkLayer = aLinkLayer;
}
