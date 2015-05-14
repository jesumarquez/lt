/*********************************************************************
*
*   MODULE NAME:
*       CWndEventListener.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"

#include "CWndEventListener.h"

//----------------------------------------------------------------------
//! \brief Handles event callbacks by translating an EventId into a
//!    windows message.
//! \details Casts this to a CWnd then posts a Windows message to it.
//!    The use of dynamic_cast is used as an alternative to the
//!    "dreaded diamond" caused by C++ multiple inheritance.
//! \param  aEventId The event ID
//! \param  aEventData The event data
//! \param  aEventDataPtr Pointer to more event data
//! \note Interested parties receive these events as Windows messages,
//!    i.e., ON_MESSAGE( aEventId, CallbackFunction ).  aEventData is
//!    the WPARAM, and aEventDataPtr is the LPARAM.
//----------------------------------------------------------------------
void CWndEventListener::onEvent
    (
    EventId aEventId,
    uint32  aEventData,
    void*   aEventDataPtr
    )
{
    CWnd* thisWindow = dynamic_cast<CWnd*>( this );
    UINT  windowsEventId = WM_EVENT( aEventId );

    ASSERT( thisWindow != NULL );
    ASSERT( aEventId < EVENT_ID_CNT );

    if( IsWindow( thisWindow->m_hWnd ) )
        thisWindow->PostMessage( windowsEventId, (WPARAM)aEventData, (LPARAM)aEventDataPtr );
}
