/*********************************************************************
*
*   MODULE NAME:
*       TransportLayer.cpp
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "TransportLayer.h"

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
TransportLayer::TransportLayer()
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
TransportLayer::~TransportLayer()
{
}

//----------------------------------------------------------------------
//! \brief Add an app layer object to the callback list
//! \param  app The ApplicationLayer to get packet notifications
//----------------------------------------------------------------------
void TransportLayer::addAppLayer
    (
    ApplicationLayer * app
    )
{
    mAppLayers.push_back( app );
}

//----------------------------------------------------------------------
//! \brief Remove an app layer object from the callback list
//! \param  app The ApplicationLayer that should no longer get packet
//!     notifications
//----------------------------------------------------------------------
void TransportLayer::removeAppLayer
    (
    ApplicationLayer * app
    )
{
    mAppLayers.remove( app );
}
