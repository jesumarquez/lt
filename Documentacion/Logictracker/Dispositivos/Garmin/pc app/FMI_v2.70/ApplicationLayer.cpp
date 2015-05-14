/*********************************************************************
*
*   MODULE NAME:
*       ApplicationLayer.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "ApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Construct an ApplicationLayer
//! \param aTransportLayer The transport layer that this application
//!     layer communicates with
//----------------------------------------------------------------------
ApplicationLayer::ApplicationLayer( TransportLayer* aTransportLayer )
    : mTransportLayer( aTransportLayer )
{
    mTransportLayer->addAppLayer( this );
}

//----------------------------------------------------------------------
//! \brief ApplicationLayer destructor
//! \details Disconnect any TransportLayer from this ApplicationLayer
//----------------------------------------------------------------------
ApplicationLayer::~ApplicationLayer()
{
    if( mTransportLayer )
    {
        mTransportLayer->removeAppLayer( this );
    }
}

//----------------------------------------------------------------------
//! \brief Disconnect the TransportLayer from this ApplicationLayer
//----------------------------------------------------------------------
void ApplicationLayer::disconnectTransportLayer()
{
    mTransportLayer = NULL;
}
