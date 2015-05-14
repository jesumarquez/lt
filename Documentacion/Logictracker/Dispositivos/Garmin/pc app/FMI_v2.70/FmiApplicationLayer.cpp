/*********************************************************************
*
*   MODULE NAME:
*       FmiApplicationLayer.cpp
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include <stdlib.h>
#include <fstream>

#include "fmi.h"
#include "FmiApplicationLayer.h"
#include "GarminTransportLayer.h"
#include "GarminPacket.h"
#include "Event.h"
#include "util.h"
#include "utl_crc.h"

/*--------------------------------------------------------------------
LITERAL CONSTANTS
--------------------------------------------------------------------*/
#if( FMI_SUPPORT_A603 )
//! File name of the saved stops data
//! \since Protocol A603
#define SAVE_STOPS          "a603_stops.dat"
#endif

#if( FMI_SUPPORT_A604 )
//! File name of the saved canned response data
//! \since Protocol A604
#define SAVE_CAN_RESP       "canned_responses.dat"

//! File name of the saved canned message data
//! \since Protocol A604
#define SAVE_CAN_MSG        "canned_messages.dat"

//! File name of the saved driver status list data
//! \since Protocol A604
#define SAVE_STATUS         "driver_status.dat"

//! File name of the saved sent canned response message data
//! \since Protocol A604
#define SAVE_SENT_CAN       "sent_canned_response_messages.dat"
#endif

#if( FMI_SUPPORT_A607 )
//! File name of the saved category data
//! \since Protocol A607
#define SAVE_CATEGORIES     "categories.dat"

//! File name of the saved waypoint data
//! \since Protocol A607
#define SAVE_WAYPOINTS      "waypoints.dat"

//! File name of the saved driver IDs/passwords
//! \since Protocol A607
#define SAVE_DRIVER_LOGINS  "drivers.dat"
#endif

using namespace std;

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
FmiApplicationLayer::FmiApplicationLayer()
    : ApplicationLayer( GarminTransportLayer::getInstance() )
#if( FMI_SUPPORT_A603 )
    , mA603Stops( SAVE_STOPS )
#endif
#if( FMI_SUPPORT_A604 )
    , mCannedMessages( SAVE_CAN_MSG )
    , mCannedResponses( SAVE_CAN_RESP )
    , mDriverStatuses( SAVE_STATUS )
    , mSentCannedResponseMessages( SAVE_SENT_CAN )
#endif
#if( FMI_SUPPORT_A607 )
    , mWaypoints( SAVE_WAYPOINTS )
    , mCategories( SAVE_CATEGORIES )
    , mDriverLogins( SAVE_DRIVER_LOGINS )
#endif
{
#if( FMI_SUPPORT_A604 )
    memset( mGpiFilePath, 0, sizeof( mGpiFilePath ) );
    memset( mCannedResponseMessageBody, 0, sizeof( mCannedResponseMessageBody ) );
    memset( mSentDriverId, 0, sizeof( mSentDriverId ) );
    memset( &mLastGpiPacketSent, 0, sizeof( mLastGpiPacketSent ) );
    mSentDriverStatus               = INVALID32;
    mGpiFileTransferSize            = INVALID32;
    mCannedResponseMessageType      = FALSE;
    mRefreshingDriverStatusList     = false;
    mRefreshingCannedResponses      = false;
    mRefreshingCannedMessages       = false;
#endif

    //public
    mClientCodepage         = CODEPAGE_ASCII;
    mStopListInitialized    = FALSE;
    memset( mClientUnitId,          0, sizeof( mClientUnitId ) );
    memset( mPVTFixType,            0, sizeof( mPVTFixType ) );
    memset( mPvtDate,               0, sizeof( mPvtDate ) );
    memset( mPvtTime,               0, sizeof( mPvtTime ) );
    memset( mPvtLatitude,           0, sizeof( mPvtLatitude ) );
    memset( mPvtLongitude,          0, sizeof( mPvtLongitude ) );
    memset( mPvtAltitude,           0, sizeof( mPvtAltitude ) );
    memset( mPvtEastWestVelocity,   0, sizeof( mPvtEastWestVelocity ) );
    memset( mPvtNorthSouthVelocity, 0, sizeof( mPvtNorthSouthVelocity ) );
    memset( mPvtUpDownVelocity,     0, sizeof( mPvtUpDownVelocity ) );
    memset( mHorizontalVelocity,    0, sizeof( mHorizontalVelocity ) );
    memset( mProtocols,             0, sizeof( mProtocols ) );
    mClientProductId = 0;
    mClientSoftwareVersion = 0;
#if( FMI_SUPPORT_A602 )
    mEnablePending = false;
#endif
#if( FMI_SUPPORT_A603 )
    memset( mEtaTime,      0, sizeof( mEtaTime ) );
    memset( mEtaLatitude,  0, sizeof( mEtaLatitude ) );
    memset( mEtaLongitude, 0, sizeof( mEtaLongitude ) );
    memset( mEtaDistance,  0, sizeof( mEtaDistance ) );
    mActiveStopId         = INVALID32;
    mActiveRoute          = FALSE;
#endif
#if( FMI_SUPPORT_A604 )
    memset( mDriverId,       0, sizeof( mDriverId ) );
    memset( mDriverStatus,   0, sizeof( mDriverStatus ) );
    memset( mGpiFileVersion, 0, sizeof( mGpiFileVersion ) );
    mGpiTransferBytesDone  = 0;
    mGpiFileSize           = 0;
    mGpiFileVersionLength  = 0;
    mClientPingCount       = 0;
    mServerPingCount       = 0;
    mGpiTransferState      = TRANSFER_NOT_STARTED;
    mStopGpiTransfer       = FALSE;
#endif

#if( FMI_SUPPORT_A607 )
    mUseMultipleDrivers = false;
    mUsePasswords = false;
#endif
}    /* FmiApplicationLayer() */

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
FmiApplicationLayer::~FmiApplicationLayer()
{
}

//----------------------------------------------------------------------
//! \brief Handle a packet received from the client
//! \details This is where all the magic happens.  This function sets
//!     up the state variables containing the data received from the
//!     client to be displayed by the fmi_pc_app user interface, and
//!     dispatches Windows messages as appropriate for the user
//!     interface to process.  It also transmits the next packet in
//!     the protocol when there is one.
//! \param aPacket The received packet to process
//! \return true if the packet was handled, false if an error occurred
//!     (for example, a packet was to be sent in response, but the
//!     send failed)
//----------------------------------------------------------------------
bool FmiApplicationLayer::rx
    (
    const Packet * aPacket
    )
{
    const GarminPacket * garminPacket = dynamic_cast<const GarminPacket *>( aPacket );

    ASSERT( garminPacket != NULL );

    switch( garminPacket->mPacketId )
    {
    case ID_PVT_DATA:
        {
            pvt_data_type pvt;
            time_type pvtTimeUtc;
            time_type pvtTimeLocal;
            gps_time_type gpsTime;
            date_time_data_type pvtDateTime;
            double latitudeDegrees;
            double longitudeDegrees;

            memset( &pvt, 0, sizeof( pvt ) );
            memcpy( &pvt, garminPacket->mPayload, minval( sizeof( pvt ), garminPacket->mPayloadSize ) );

            switch( pvt.type_of_gps_fix )
            {
            case GPS_FIX_UNUSABLE:
                sprintf( mPVTFixType, "%s", "unusable" );
                break;
            case GPS_FIX_INVALID:
                sprintf( mPVTFixType, "%s", "invalid" );
                break;
            case GPS_FIX_2D:
                sprintf( mPVTFixType, "%s", "2D-fix" );
                break;
            case GPS_FIX_3D:
                sprintf( mPVTFixType, "%s", "3D-fix" );
                break;
            case GPS_FIX_2D_DIFF:
                sprintf( mPVTFixType, "%s", "2D-diff" );
                break;
            case GPS_FIX_3D_DIFF:
                sprintf( mPVTFixType, "%s", "3D-diff" );
                break;
            }

            gpsTime.time_of_week = maxval( 0, (sint32) pvt.time_of_week - (sint32) pvt.leap_seconds );
            gpsTime.week_number_days = pvt.week_number_days;
            UTIL_convert_gps_time_to_seconds( &gpsTime, &pvtTimeUtc );
            UTIL_convert_UTC_to_local( &pvtTimeUtc, &pvtTimeLocal );

            UTIL_convert_seconds_to_date_type( &pvtTimeLocal, &pvtDateTime );
            UTIL_format_date_string( &pvtDateTime, mPvtDate );
            UTIL_convert_seconds_to_time_type( &pvtTimeLocal, &pvtDateTime );
            UTIL_format_time_string( &pvtDateTime, mPvtTime );

            latitudeDegrees = UTIL_convert_radians_to_degrees( pvt.position.lat );
            if( latitudeDegrees < 0 )
                sprintf( mPvtLatitude, "%08.6f °S", latitudeDegrees * -1 );
            else
                sprintf( mPvtLatitude, "%08.6f °N", latitudeDegrees );

            longitudeDegrees = UTIL_convert_radians_to_degrees( pvt.position.lon );
            if( longitudeDegrees < 0 )
                sprintf( mPvtLongitude, "%08.6f °W", longitudeDegrees * -1 );
            else
                sprintf( mPvtLongitude, "%08.6f °E", longitudeDegrees );

            sprintf( mPvtAltitude, "%05.3f m", pvt.altitude );

            if( pvt.east_velocity < 0 )
                sprintf( mPvtEastWestVelocity, "%05.3f m/s W", pvt.east_velocity * -1 );
            else
                sprintf( mPvtEastWestVelocity, "%05.3f m/s E", pvt.east_velocity );

            if( pvt.north_velocity < 0 )
                sprintf( mPvtNorthSouthVelocity, "%05.3f m/s S", pvt.north_velocity * -1 );
            else
                sprintf( mPvtNorthSouthVelocity, "%05.3f m/s N", pvt.north_velocity );

            if( pvt.up_velocity < 0 )
                sprintf( mPvtUpDownVelocity, "%05.3f m/s D", pvt.up_velocity * -1 );
            else
                sprintf( mPvtUpDownVelocity, "%05.3f m/s U", pvt.up_velocity );

            calculate2DVelocity( pvt.north_velocity, pvt.east_velocity );

            //tell app to update PVT
            Event::post( EVENT_PVT_RECEIVED );
            break;
        } // end of case ID_PVT_DATA
    case ID_UNIT_ID:
        {
            unit_id_data_type unitIdData;
            memset( &unitIdData, 0, sizeof( unitIdData ) );
            memcpy( &unitIdData, garminPacket->mPayload, minval( sizeof( unitIdData ), garminPacket->mPayloadSize ) );

            sprintf( mClientUnitId, "%u", unitIdData.unit_id );
            Event::post( EVENT_ESN_RECEIVED );
            break;
        } // end of case ID_UNIT_ID
    case ID_PRODUCT_DATA:
        {
            /* Legacy product data protocol.  See 001-00063-00 Rev C */
            product_id_data_type productIdPacket;
            memset( &productIdPacket, 0, sizeof( productIdPacket ) );
            memcpy( &productIdPacket, garminPacket->mPayload, minval( sizeof( productIdPacket ), garminPacket->mPayloadSize ) );

            mClientProductId = productIdPacket.product_id;
            mClientSoftwareVersion = productIdPacket.software_version;
            Event::post( EVENT_PRODUCT_ID_RECEIVED );
            break;
        }
#if( FMI_SUPPORT_A602 )
    case ID_FMI_PACKET:
        {
            fmi_id_type fmiPacketId = *(fmi_id_type *) garminPacket->mPayload;
            uint8 const * fmiPayload = &garminPacket->mPayload[ sizeof( fmiPacketId ) ];
            sint16 fmiPayloadSize = garminPacket->mPayloadSize - sizeof( fmiPacketId );

            //This is an FMI Packet, so we need to switch the FMI ID to see what we really have
            switch( fmiPacketId )
            {
            case FMI_ID_PRODUCT_ID_DATA: // 0x0002
                {
                    product_id_data_type productIdPacket;
                    memset( &productIdPacket, 0, sizeof( productIdPacket ) );
                    memcpy( &productIdPacket, fmiPayload, minval( sizeof( productIdPacket ), fmiPayloadSize ) );
                    mClientProductId = productIdPacket.product_id;
                    mClientSoftwareVersion = productIdPacket.software_version;
                    //flag to tell app new mClientProductId was received and to update the info
                    Event::post( EVENT_PRODUCT_ID_RECEIVED );
                    break;
                } // end of FMI_ID_PRODUCT_ID_DATA
            case FMI_ID_PROTOCOL_DATA: // 0x0003
                {
                    protocol_support_data_type protocol;
                    for( unsigned int i = 0; i < fmiPayloadSize - sizeof( protocol ); i += sizeof( protocol ) )
                    {
                        //copy one protocol from fmiPayload
                        memset( &protocol, 0, sizeof( protocol ) );
                        memcpy( &protocol, &fmiPayload[i], sizeof( protocol ) );
                        //print one char tag and 3 digit data (i.e.,. A604) and a space to protocol string
                        sprintf( mProtocols + ( 5 * ( i / sizeof( protocol ) ) ), "%c%03d ", (char)protocol.tag, protocol.data );
                    }
                    //flag to tell app we have received a protocol listing
                    Event::post( EVENT_PROTOCOLS_RECEIVED );
                    break;
                } // end of FMI_ID_PROTOCOL_DATA
#if( FMI_SUPPORT_A604 && UNICODE_ENABLED )
            case FMI_ID_UNICODE_REQUEST: // 0x0004
                {
                    mClientCodepage = CODEPAGE_UNICODE;
                    txFmi( FMI_ID_UNICODE_RESPONSE, NULL, 0 );
                    break;
                } // FMI_ID_UNICODE_REQUEST
#endif
            case FMI_ID_TEXT_MSG_ACK: // 0x0020
                {
                    text_msg_ack_data_type data;
                    text_msg_ack_event_type *ackEvent = new text_msg_ack_event_type;
#if( FMI_SUPPORT_A604 )
                    BOOL cannedResponse = FALSE;
#endif

                    memset( &data, 0, sizeof( data ) );
                    memcpy( &data, fmiPayload, minval( sizeof( data ), fmiPayloadSize ) );

                    memset( ackEvent->ack_text, 0, sizeof( ackEvent->ack_text ) );
                    ackEvent->message_id = MessageId( data.id_size, data.id );

#if( FMI_SUPPORT_A604 )
                    //prepare and send receipt
                    text_msg_id_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    receipt.id_size = minval( data.id_size, sizeof( data.id ) );
                    memcpy( receipt.id, data.id, receipt.id_size );
                    txFmi( FMI_ID_TEXT_MSG_ACK_RCPT, (uint8*)&receipt, sizeof( receipt ) );

                    //this packet ID is sent back for all A602 ACKs and canned messages
                    //must check to see if the id corresponds to a canned response
                    if( mSentCannedResponseMessages.contains( MessageId( data.id_size, data.id ) ) )
                    {
                        cannedResponse = TRUE;
                        mSentCannedResponseMessages.remove( MessageId( data.id_size, data.id ) );
                    }

                    if( cannedResponse )
                    {
                        //Get the response text
                        ClientListItem& canResp = mCannedResponses.get( data.msg_ack_type );
                        if( canResp.isValid() )
                        {
                            char str[50];
                            WideCharToMultiByte( mClientCodepage, 0, canResp.getCurrentName(), -1, str, 50, NULL, NULL );
                            str[49] = '\0';
                            sprintf( ackEvent->ack_text, "%s", str );
                        }
                        else
                            sprintf( ackEvent->ack_text, "%s", "invalid" );
                    }
                    //if not a canned message...it is an old A602 ACK
                    else
#endif //FMI_SUPPORT_A604
                    {
                        switch( data.msg_ack_type )
                        {
                        case OK_ACK:
                            strcpy( ackEvent->ack_text, "Okay" );
                            break;
                        case YES_ACK:
                            strcpy( ackEvent->ack_text, "Yes" );
                            break;
                        case NO_ACK:
                            strcpy( ackEvent->ack_text, "No" );
                            break;
                        default:
                            strcpy( ackEvent->ack_text, "invalid" );
                            break;
                        }
                    }
                    //Tell app to display the text message ack dialog
                    Event::post( EVENT_FMI_TXT_MSG_ACK, 0, ackEvent );
                    break;
                } // end of FMI_ID_TXT_MSG_ACK
#if( FMI_SUPPORT_A603 )
            case FMI_ID_CLIENT_OPEN_TXT_MSG: // 0x0024
                {
                    client_to_server_open_text_msg_data_type textMessage;
                    client_to_server_text_msg_receipt_data_type receipt;
                    text_msg_from_client_event_type *textMessageEvent = new text_msg_from_client_event_type;

                    memset( &textMessage, 0, sizeof( textMessage ) );
                    memcpy( &textMessage, fmiPayload, minval( sizeof( textMessage ), fmiPayloadSize ) );

                    receipt.unique_id = textMessage.unique_id;
                    txFmi( FMI_ID_CLIENT_TXT_MSG_RCPT, (uint8*)&receipt, sizeof( receipt ) );

                    memset( textMessageEvent, 0, sizeof( textMessageEvent ) );
                    strncpy( textMessageEvent->message_text, textMessage.text_message, sizeof( textMessageEvent->message_text ) );
                    textMessageEvent->origination_time = textMessage.origination_time;

                    textMessageEvent->message_id = textMessage.unique_id;
                    Event::post( EVENT_FMI_TXT_MSG_FROM_CLIENT, 0, textMessageEvent );
                    break;
                } // end of FMI_ID_CLIENT_OPEN_TXT_MSG
#endif
#if( FMI_SUPPORT_A607 )
            case FMI_ID_A607_CLIENT_OPEN_TXT_MSG: // 0x0026
                {
                    client_to_server_D607_open_text_msg_data_type textMessage;
                    client_to_server_text_msg_receipt_data_type receipt;
                    text_msg_from_client_event_type *textMessageEvent = new text_msg_from_client_event_type;

                    memset( &textMessage, 0, sizeof( textMessage ) );
                    memcpy( &textMessage, fmiPayload, minval( sizeof( textMessage ), fmiPayloadSize ) );

                    receipt.unique_id = textMessage.unique_id;
                    txFmi( FMI_ID_CLIENT_TXT_MSG_RCPT, (uint8*)&receipt, sizeof( receipt ) );

                    memset( textMessageEvent->message_text, 0, sizeof( textMessageEvent->message_text ) );
                    strncpy( textMessageEvent->message_text, textMessage.text_message, sizeof( textMessageEvent->message_text ) );
                    textMessageEvent->origination_time = textMessage.origination_time;

                    textMessageEvent->message_id = textMessage.unique_id;
                    textMessageEvent->link_id = MessageId( textMessage.id_size, textMessage.id );

                    Event::post( EVENT_FMI_TXT_MSG_FROM_CLIENT, 0, textMessageEvent );
                    break;
                }
#endif
#if( FMI_SUPPORT_A604 )
            case FMI_ID_CANNED_RESP_LIST_RCPT: // 0x0029
                {
                    canned_response_list_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    if( receipt.result_code == CANNED_RESP_LIST_SUCCESS ) //success
                    {
                        //we have to keep track of canned response text messages
                        //because the packet that is sent back for the response
                        //is the same as Okay/Yes/No messages so they need
                        //to be distinguished
                        MessageId messageId( receipt.id_size, receipt.id );
                        InboxListItem & item = mSentCannedResponseMessages.get( messageId );
                        item.setValid();
                        mSentCannedResponseMessages.put( item );

                        sendA604TextMessage( mCannedResponseMessageBody, MessageId(receipt.id_size, receipt.id), mCannedResponseMessageType );
                    }
                    else
                        Event::post( EVENT_FMI_CANNED_RESP_LIST_RCPT_ERROR, receipt.result_code, NULL );
                    break;
                }

            case FMI_ID_A604_OPEN_TEXT_MSG_RCPT: // 0x002B
                {
                    server_to_client_text_msg_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( server_to_client_text_msg_receipt_data_type ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( server_to_client_text_msg_receipt_data_type ), fmiPayloadSize ) );
                    if( receipt.result_code == FALSE )
                    {
                        Event::post( EVENT_FMI_A604_TXT_MSG_ERROR, 0, new MessageId( receipt.id_size, receipt.id ) );
                    }
                    break;
                }
#endif

#if( FMI_SUPPORT_A607 )
            case FMI_ID_TEXT_MSG_DELETE_RESPONSE:
                {
                    delete_message_response_data_type response;
                    memset( &response, 0, sizeof( response ) );
                    memcpy( &response, fmiPayload, minval( fmiPayloadSize, sizeof( response ) ) );

                    Event::post( EVENT_FMI_DELETE_TEXT_MESSAGE_STATUS, response.result_code );
                    break;
                }
#endif

#if( FMI_SUPPORT_A604 )
            case FMI_ID_SET_CANNED_RESPONSE_RCPT: // 0x0032
                {
                    canned_response_receipt_data_type receipt;
                    map<uint32, ClientListItem>::iterator iter;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( mRefreshingCannedResponses )
                        mRefreshingCannedResponses = FALSE;
                    else if( receipt.result_code )
                    {
                        if( mCannedResponses.contains( receipt.response_id ) ) //is in map...otherwise there was an error
                        {
                            ClientListItem& item = mCannedResponses.get( receipt.response_id );
                            item.commitName();
                            Event::post( EVENT_FMI_CANNED_RESP_LIST_CHANGED );
                        }
                    }
                    break;
                }
            case FMI_ID_DELETE_CANNED_RESPONSE_RCPT: // 0x0033
                {
                    canned_response_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( receipt.result_code )
                    {
                        mCannedResponses.remove( receipt.response_id );
                        Event::post( EVENT_FMI_CANNED_RESP_LIST_CHANGED );
                    }
                    break;
                }
            case FMI_ID_REFRESH_CANNED_RESP_LIST: // 0x0034
                {
                    request_canned_response_list_refresh_data_type request;
                    FileBackedMap<ClientListItem>::const_iterator iter;
                    memset( &request, 0, sizeof( request ) );
                    memcpy( &request, fmiPayload, minval( fmiPayloadSize, sizeof( request ) ) );

                    //send all valid ids
                    if( request.response_count == 0 )
                    {
                        for( iter = mCannedResponses.begin(); iter != mCannedResponses.end(); iter++ )
                        {
                            mRefreshingCannedResponses = TRUE;
                            //send Set Canned Response packet
                            if( iter->second.isValid() )
                            {
                                sendCannedResponse( iter->first, iter->second.getCurrentName() );
                            }
                        }
                    }
                    //only send requested ids
                    else if( request.response_count > 0 && request.response_count <= 50 )
                    {
                        //loop through requested ids and send if valid
                        for( unsigned int i = 0; i < request.response_count; i++ )
                        {
                            if( mCannedResponses.contains( request.response_id[i] ) )
                            {
                                ClientListItem& item = mCannedResponses.get( request.response_id[i] );
                                if( item.isValid() )
                                {
                                    //send Set Canned Response packet
                                    sendCannedResponse( item.getId(), item.getCurrentName() );
                                }
                            }
                        }
                    }
                    break;
                }
            case FMI_ID_TEXT_MSG_STATUS: // 0x0041
                {
                    message_status_data_type status;
                    text_msg_status_event_type* statusEvent = new text_msg_status_event_type;
                    memset( &status, 0, sizeof( status ) );
                    memcpy( &status, fmiPayload, minval( sizeof( status ), fmiPayloadSize ) );

                    statusEvent->msg_id = MessageId( status.id_size, status.id );
                    statusEvent->message_status = (fmi_A604_message_status)status.status_code;

                    Event::post( EVENT_FMI_TXT_MSG_STATUS_RECEIVED, 0, statusEvent );
                    break;
                }

            case FMI_ID_SET_CANNED_MSG_RCPT: // 0x0051
                {
                    canned_message_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( mRefreshingCannedMessages )
                        mRefreshingCannedMessages = FALSE;
                    else if( receipt.result_code )
                    {
                        if( mCannedMessages.contains( receipt.message_id ) ) //is in map...otherwise there was an error
                        {
                            ClientListItem& item = mCannedMessages.get( receipt.message_id );
                            item.commitName();
                            Event::post( EVENT_FMI_CANNED_MSG_LIST_CHANGED );
                        }
                    }
                    break;
                }

            case FMI_ID_DELETE_CANNED_MSG_RCPT: // 0x0053
                {
                    canned_message_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( receipt.result_code )
                    {
                        mCannedMessages.remove( receipt.message_id );
                        Event::post( EVENT_FMI_CANNED_MSG_LIST_CHANGED );
                    }
                    break;
                }
            case FMI_ID_REFRESH_CANNED_MSG_LIST: // 0x0054
                {
                    FileBackedMap<ClientListItem>::const_iterator iter;
                    for( iter = mCannedMessages.begin(); iter != mCannedMessages.end(); iter++ )
                    {
                        if( iter->second.isValid() )
                        {
                            mRefreshingCannedMessages = TRUE;
                            sendCannedMessage( iter->first, iter->second.getCurrentName() );
                        }
                    }
                    break;
                }

            case FMI_ID_SORT_STOP_LIST_ACK: // 0x0111
                {
                    FileBackedMap<StopListItem>::const_iterator iter;
                    for( iter = mA603Stops.begin(); iter != mA603Stops.end(); iter++ )
                    {
                        sendStopStatusRequest( iter->first, REQUEST_STOP_STATUS );
                    }
                    break;
                }
#endif

#if FMI_SUPPORT_A607
            case FMI_ID_WAYPOINT_RCPT:
                {
                    waypoint_rcpt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    if( receipt.result_code )
                    {
                        if( mWaypoints.contains( receipt.unique_id ) ) //is in map...otherwise there was an error
                        {
                            WaypointListItem& item = mWaypoints.get( receipt.unique_id );
                            item.commitName();
                            Event::post( EVENT_FMI_WAYPOINT_LIST_CHANGED );
                        }
                    }
                    break;
                }

            case FMI_ID_WAYPOINT_DELETED:
                {
                    waypoint_deleted_data_type receipt;
                    uint16 uniqueId;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    uniqueId = receipt.unique_id;

                    if( mWaypoints.contains( receipt.unique_id ) ) //is in map...otherwise there was an error
                        {
                        mWaypoints.remove( receipt.unique_id );
                        Event::post( EVENT_FMI_WAYPOINT_LIST_CHANGED );
                        }

                    txFmi( FMI_ID_WAYPOINT_DELETED_RCPT, (uint8*)&uniqueId, sizeof( uniqueId ) );
                    break;
                }

            case FMI_ID_DELETE_WAYPOINT_CAT_RCPT:
                {
                    delete_by_category_rcpt_data_type receipt;
                    boolean listChanged = FALSE;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    for( int i = 0; i < 16; i++ )
                    {
                        if( receipt.cat_id & setbit( i ) )
                        {
                            mCategories.remove( i );
                            listChanged = TRUE;
                        }
                    }

                    if( listChanged )
                    {
                        Event::post( EVENT_FMI_CATEGORY_LIST_CHANGED );
                    }
                    break;
                }
            case FMI_ID_CREATE_WAYPOINT_CAT_RCPT:
                {
                    category_rcpt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    if( receipt.result_code )
                    {
                        if( mCategories.contains( receipt.id ) ) //is in map...otherwise there was an error
                        {
                            ClientListItem& item = mCategories.get( receipt.id );
                            item.commitName();
                            Event::post( EVENT_FMI_CATEGORY_LIST_CHANGED );
                        }
                    }
                    break;
                }
#endif

#if( FMI_SUPPORT_A603 )
            case FMI_ID_ETA_DATA: //0x0201
                {
                    eta_data_type etaDataPacket;
                    eta_data_receipt_type receipt;
                    memset( &etaDataPacket, 0, sizeof( etaDataPacket ) );
                    memcpy( &etaDataPacket, fmiPayload, minval( sizeof( etaDataPacket ), fmiPayloadSize ) );
                    receipt.unique_id = etaDataPacket.unique_id;
                    txFmi( FMI_ID_ETA_DATA_RCPT, (uint8*)&receipt, sizeof( receipt ) );

                    if( etaDataPacket.distance_to_destination != INVALID32 &&
                        etaDataPacket.eta_time                != INVALID32 )
                    {
                        double latitudeDegrees;
                        double longitudeDegrees;
                        time_type etaTimeLocal;
                        date_time_data_type dateTimeLocal;

                        latitudeDegrees = UTIL_convert_semicircles_to_degrees( etaDataPacket.position_of_destination.lat );
                        if( latitudeDegrees < 0 )
                        {
                            sprintf( mEtaLatitude, "%08.6f °S", latitudeDegrees * -1 );
                        }
                        else
                        {
                            sprintf( mEtaLatitude, "%08.6f °N", latitudeDegrees );
                        }

                        longitudeDegrees = UTIL_convert_semicircles_to_degrees( etaDataPacket.position_of_destination.lon );
                        if( longitudeDegrees < 0 )
                        {
                            sprintf( mEtaLongitude, "%08.6f °W", longitudeDegrees * -1 );
                        }
                        else
                        {
                            sprintf( mEtaLongitude, "%08.6f °E", longitudeDegrees );
                        }

                        UTIL_convert_UTC_to_local( &etaDataPacket.eta_time, &etaTimeLocal );
                        UTIL_convert_seconds_to_time_type( &etaTimeLocal, &dateTimeLocal );
                        UTIL_format_time_string( &dateTimeLocal, mEtaTime );

                        if( etaDataPacket.distance_to_destination > 1000 )
                        {
                            sprintf( mEtaDistance, "%.2f km", (float)etaDataPacket.distance_to_destination/1000 );
                        }
                        else
                        {
                            sprintf( mEtaDistance, "%d m", etaDataPacket.distance_to_destination );
                        }

                        mActiveRoute = TRUE;
                    }
                    else
                    {
                        strcpy( mEtaTime, "invalid" );
                        strcpy( mEtaDistance, "" );
                        strcpy( mEtaLatitude, "" );
                        strcpy( mEtaLongitude, "" );
                        mActiveRoute = FALSE;
                    }
                    // notify user interface
                    Event::post( EVENT_FMI_ETA_RECEIVED );
                    break;
                }

            case FMI_ID_STOP_STATUS: // 0x0211
                {
                    stop_status_data_type stopStatus;
                    stop_status_receipt_data_type receipt;
                    map<uint32, StopListItem>::iterator iter;

                    memcpy( &stopStatus, fmiPayload, minval( sizeof( stopStatus ), fmiPayloadSize ) );

                    //send receipt
                    receipt.unique_id = stopStatus.unique_id;
                    txFmi( FMI_ID_STOP_STATUS_RCPT, (uint8*)&receipt, sizeof( receipt ) );

                    if( !mStopListInitialized )
                    {
                        // When the server starts up, it must ask for updates for all stops since
                        // the client can change their statuses and delete them when not connected
                        // to the server.
                        // Deleting a stop requires all the data structures to be set up correctly,
                        // but this status could be sent without any of the stop indexes in the
                        // list having been set up at all.
                        // So, stop status requests are sent out for all stops the server has in
                        // its save file when initializing the server. If the mStopListInitialized
                        // flag hasn't been set yet, we only care about deleted stops. We must
                        // remove them from the save file and stop id list. When all deleted stops
                        // are gone, we can safely use this update packet to assign the index in the
                        // stop list, as nothing will be sent for deleted stops anymore.
                        if( stopStatus.stop_status == STOP_STATUS_DELETED )
                        {
                            mA603Stops.remove( stopStatus.unique_id );
                        }
                        else
                        {
                            //flags for ETA display
                            if( stopStatus.stop_status == STOP_STATUS_ACTIVE )
                            {
                                mActiveStopId = stopStatus.unique_id;
                            }
                            else if( stopStatus.unique_id   == mActiveStopId      &&
                                     stopStatus.stop_status != STOP_STATUS_ACTIVE )
                            {
                                mActiveRoute = FALSE;
                                mActiveStopId = INVALID32;
                            }
                        }
                    }
                    //We are initialized, so either we have mIsValid data structures or nothing will be
                    //deleted. So we can proceed like normal.
                    else if( stopStatus.unique_id          != INVALID32 &&
                             stopStatus.stop_index_in_list != INVALID16 )
                    {
                        //the stop id and index in list are valid, so update
                        mA603Stops.get( stopStatus.unique_id ).setStopStatus( stopStatus.stop_status );
                        if( mStopIndexInList.size() <= stopStatus.stop_index_in_list )
                            mStopIndexInList.resize( stopStatus.stop_index_in_list + 10, INVALID32 );
                        mStopIndexInList[stopStatus.stop_index_in_list] = stopStatus.unique_id;

                        //flags for ETA display
                        if( stopStatus.stop_status == STOP_STATUS_ACTIVE )
                            mActiveStopId = stopStatus.unique_id;
                        else if( stopStatus.unique_id   == mActiveStopId      &&
                                 stopStatus.stop_status != STOP_STATUS_ACTIVE )
                        {
                            mActiveRoute = FALSE;
                        }
                    }
                    else if( stopStatus.stop_index_in_list == INVALID16 &&
                             stopStatus.stop_status        == STOP_STATUS_DELETED )
                    {
                        removeStopById( stopStatus.unique_id );
                    }
                    uint32 eventData = stopStatus.stop_status;
                    if( mActiveStopId == stopStatus.unique_id )
                        eventData |= setbit( 16 );
                    Event::post( EVENT_FMI_STOP_STATUS_CHANGED, eventData );
                    break;
                }

#endif

#if( FMI_SUPPORT_A604 )
            case FMI_ID_USER_INTERFACE_TEXT_RCPT: //0x0241
                {
                    user_interface_text_receipt_data_type receipt;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    if( !receipt.result_code )
                        Event::post( EVENT_FMI_UI_UPDATE_ERROR );
                    break;
                }

            case FMI_ID_MSG_THROTTLING_RESPONSE: // 0x0251
                {
                    message_throttling_data_type response;

                    memset( &response, 0, sizeof( response ) );
                    memcpy( &response, fmiPayload, minval( sizeof( response ), fmiPayloadSize ) );

                    if( response.new_state != MESSAGE_THROTTLE_STATE_DISABLE &&
                        response.new_state != MESSAGE_THROTTLE_STATE_ENABLE )
                    {
                        Event::post( EVENT_FMI_MSG_THROTTLE_ERROR );
                    }
                    break;
                }
#endif

#if( FMI_SUPPORT_A605 )
            case FMI_ID_MSG_THROTTLING_QUERY_RESPONSE: // 0x0253
                {
                    message_throttling_list_data_type list;

                    memset( &list, 0, sizeof( list ) );
                    memcpy( &list, fmiPayload, minval( sizeof( list ), fmiPayloadSize ) );

                    uint16 throttleCount = minval( list.response_count, MAX_THROTTLED_PROTOCOLS );
                    for( uint16 i = 0; i < throttleCount; ++i )
                    {
                        mThrottledProtocols[i].new_state = list.response_list[i].new_state;
                        mThrottledProtocols[i].packet_id = list.response_list[i].packet_id;
                    }
                    Event::post( EVENT_FMI_MSG_THROTTLE_QUERY_RESP_RECEIVED, throttleCount, mThrottledProtocols );
                    break;
                }
#endif

#if( FMI_SUPPORT_A604 )
            case FMI_ID_PING: // 0x0260
                {
                    mClientPingCount++;
                    mLastClientPingTime = CTime::GetCurrentTime();

                    txFmi( FMI_ID_PING_RESPONSE, NULL, 0 );

                    Event::post( EVENT_FMI_PING_RECEIVED );
                    break;
                }
            case FMI_ID_PING_RESPONSE: // 0x0261
                {
                    mServerPingCount++;
                    mLastServerPingTime = CTime::GetCurrentTime();

                    Event::post( EVENT_FMI_PING_RESPONSE_RECEIVED );
                    break;
                }
            case FMI_ID_GPI_FILE_START_RCPT: // 0x0403
                {
                    gpi_file_receipt_data_type receipt;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    //check success
                    if( receipt.result_code == 0 )
                    {
                        fstream gpiFile( mGpiFilePath, ios_base::binary | ios_base::in );
                        if( gpiFile.good() )
                        {
                            gpi_file_packet_data_type data;
                            mGpiTransferState = TRANSFER_IN_PROGRESS;

                            memset( &data, 0, sizeof( data ) );
                            gpiFile.read( (char*)data.file_data, 245 );

                            data.data_length = (uint8)gpiFile.gcount();

                            memcpy( &mLastGpiPacketSent, &data, sizeof( data ) );
                            txFmi( FMI_ID_GPI_FILE_DATA_PACKET, (uint8*)&data, sizeof( data ) - sizeof( data.file_data ) + data.data_length );

                            gpiFile.close();
                        }
                        else
                            mGpiTransferState = TRANSFER_FAILED;
                    }
                    else
                        mGpiTransferState = TRANSFER_FAILED;
                    Event::post( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED );
                    break;
                }
            case FMI_ID_GPI_PACKET_RCPT: // 0x0404
                {
                    gpi_packet_receipt_data_type receipt;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    mGpiTransferBytesDone = receipt.next_offset;
                    Event::post( EVENT_FMI_GPI_TRANSFER_PROGRESS );

                    if( receipt.next_offset == mGpiFileTransferSize || mStopGpiTransfer == TRUE )
                    {
                        //transferring file data is complete or server is stopping manually
                        //calculate and send crc
                        gpi_file_end_data_type data;
                        uint8* fileData;
                        mStopGpiTransfer = FALSE;
                        fstream gpiFile( mGpiFilePath, ios_base::binary | ios_base::in );
                        if( gpiFile.good() )
                        {
                            fileData = new uint8[mGpiFileTransferSize];
                            gpiFile.read( (char*)fileData, mGpiFileTransferSize );
                            data.crc = UTL_calc_crc32( fileData, mGpiFileTransferSize, 0 );
                            txFmi( FMI_ID_GPI_FILE_TRANSFER_END, (uint8*)&data, sizeof( data ) );
                            gpiFile.close();
                            delete[] fileData;
                        }
                        else
                        {
                            mGpiTransferState = TRANSFER_FAILED;
                            Event::post( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED );
                        }
                    }
                    else if( receipt.next_offset == receipt.offset )
                    {
                        //resend last packet
                        txFmi
                            (
                            FMI_ID_GPI_FILE_DATA_PACKET,
                            (uint8*)&mLastGpiPacketSent,
                            sizeof( mLastGpiPacketSent ) - sizeof( mLastGpiPacketSent.file_data ) + mLastGpiPacketSent.data_length
                            );
                    }
                    else if( receipt.next_offset == INVALID32 )
                    {
                        mGpiTransferState = TRANSFER_FAILED;
                        Event::post( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED );
                    }
                    else
                    {
                        // mIsValid offset...send next set of data
                        fstream gpiFile( mGpiFilePath, ios_base::binary | ios_base::in );
                        if( gpiFile.good() )
                        {
                            gpi_file_packet_data_type data;

                            gpiFile.seekg( receipt.next_offset, ios::beg );
                            memset( &data, 0, sizeof( data ) );
                            data.offset = receipt.next_offset;
                            gpiFile.read( (char*)data.file_data, 245 );
                            data.data_length = (uint8)gpiFile.gcount();
                            memcpy( &mLastGpiPacketSent, &data, sizeof( data ) );
                            txFmi
                                (
                                FMI_ID_GPI_FILE_DATA_PACKET,
                                (uint8*)&data,
                                sizeof( data ) - sizeof( data.file_data ) + data.data_length
                                );
                        }
                        else
                        {
                            mGpiTransferState = TRANSFER_FAILED;
                            Event::post( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED );
                        }
                        gpiFile.close();
                    }
                    break;
                }
            case FMI_ID_GPI_FILE_END_RCPT: // 0x0405
                {
                    gpi_file_receipt_data_type receipt;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    //check success
                    if( receipt.result_code == 0 )
                    {
                        mGpiTransferState = TRANSFER_COMPLETED;
                    }
                    else
                        mGpiTransferState = TRANSFER_FAILED;
                    Event::post( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED );
                    break;
                }
            case FMI_ID_GPI_FILE_INFORMATION: // 0x0407
                {
                    gpi_file_info_data_type gpiFileInfo;

                    memset( &gpiFileInfo, 0, sizeof( gpiFileInfo ) );
                    memcpy( &gpiFileInfo, fmiPayload, minval( sizeof( gpiFileInfo ), fmiPayloadSize ) );

                    mGpiFileSize = gpiFileInfo.file_size;
                    mGpiFileVersionLength = gpiFileInfo.file_version_length;

                    memset( mGpiFileVersion, 0, 16 );
                    memcpy( mGpiFileVersion, gpiFileInfo.file_version, gpiFileInfo.file_version_length );
                    Event::post( EVENT_FMI_GPI_FILE_INFO_RECEIVED );
                    break;
                }
            case FMI_ID_SET_DRIVER_STATUS_LIST_ITEM_RCPT: // 0x0802
                {
                    driver_status_list_item_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( mRefreshingDriverStatusList )
                    {
                        mRefreshingDriverStatusList = FALSE;
                        break;
                    }
                    else if( receipt.result_code )
                    {
                        if( mDriverStatuses.contains( receipt.status_id ) ) //found in map
                        {
                            ClientListItem& item = mDriverStatuses.get( receipt.status_id );
                            item.commitName();
                            Event::post( EVENT_FMI_DRIVER_STATUS_LIST_CHANGED );
                        }
                    }
                    break;
                }
            case FMI_ID_DEL_DRIVER_STATUS_LIST_ITEM_RCPT: // 0x0803
                {
                    driver_status_list_item_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( receipt.result_code )
                    {
                        mDriverStatuses.remove( receipt.status_id );
                        Event::post( EVENT_FMI_DRIVER_STATUS_LIST_CHANGED );
                    }
                    else
                    {
                        Event::post( EVENT_FMI_DRIVER_STATUS_LIST_DELETE_FAILED );
                    }
                    break;
                }
            case FMI_ID_DRIVER_STATUS_LIST_REFRESH: //0x0804
                {
                    FileBackedMap<ClientListItem>::const_iterator iter;
                    for( iter = mDriverStatuses.begin(); iter != mDriverStatuses.end(); iter++ )
                    {
                        if( iter->second.isValid() )
                        {
                            mRefreshingDriverStatusList = TRUE;
                            sendDriverStatusListItem( iter->first, iter->second.getCurrentName() );
                        }
                    }
                    break;
                }
            case FMI_ID_DRIVER_ID_UPDATE: // 0x0811
                {
                    driver_id_data_type data;
                    memset( &data, 0, sizeof( data ) );
                    memcpy( &data, fmiPayload, minval( sizeof( data ), fmiPayloadSize ) );

                    driver_id_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    receipt.result_code = TRUE;
                    receipt.status_change_id = data.status_change_id;

                    strcpy( mDriverId[0], data.driver_id );
                    Event::post( EVENT_FMI_DRIVER_ID_CHANGED );
                    txFmi( FMI_ID_DRIVER_ID_RCPT, (uint8*)&receipt, sizeof( receipt ) );
                    break;
                }
            case FMI_ID_DRIVER_ID_RCPT: // 0x0812
                {
                    driver_id_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( receipt.result_code && receipt.driver_idx < FMI_DRIVER_COUNT )
                    {
                        int index = receipt.driver_idx;
                        strncpy( mDriverId[index], mSentDriverId, 50 );
                        Event::post( EVENT_FMI_DRIVER_ID_CHANGED, index );
                    }
                    break;
                }
#if( FMI_SUPPORT_A607 )
            case FMI_ID_DRIVER_ID_UPDATE_D607:
                {
                    driver_id_D607_data_type data;
                    memset( &data, 0, sizeof( data ) );
                    memcpy( &data, fmiPayload, minval( sizeof( data ), fmiPayloadSize ) );

                    driver_id_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );
                    receipt.status_change_id = data.status_change_id;
                    receipt.driver_idx = data.driver_idx;
                    receipt.result_code = TRUE;

                    if( data.driver_idx >= FMI_DRIVER_COUNT )
                    {
                        receipt.result_code = FALSE;
                    }
                    else if( 0 == strlen( data.driver_id ) )
                    {
                        // not logged in
                        receipt.result_code = TRUE;
                    }
                    else if( mUsePasswords )
                    {
                        if( mDriverLogins.contains( data.driver_id ) )
                        {
                            DriverLoginItem & login = mDriverLogins.get( data.driver_id );
                            char actualPassword[ sizeof( data.password ) ];
                            WideCharToMultiByte( CP_UTF8, 0, login.getPassword().GetBuffer(), -1, actualPassword, sizeof( actualPassword ), NULL, NULL );
                            if( 0 != strcmp( data.password, actualPassword ) )
                            {
                                // incorrect password
                                receipt.result_code = FALSE;
                            }
                        }
                        else
                        {
                            // driver ID not found
                            receipt.result_code = FALSE;
                        }
                    }

                    if( receipt.result_code )
                    {
                        strcpy( mDriverId[data.driver_idx], data.driver_id );
                        Event::post( EVENT_FMI_DRIVER_ID_CHANGED, data.driver_idx );
                    }

                    txFmi( FMI_ID_DRIVER_ID_RCPT, (uint8*)&receipt, sizeof( receipt ) );
                    break;
                }
#endif
            case FMI_ID_DRIVER_STATUS_UPDATE: // 0x0821
                {
                    driver_status_data_type status;
                    driver_status_receipt_data_type receipt;

                    memset( &status, 0, sizeof( status ) );
                    memcpy( &status, fmiPayload, minval( sizeof( status ), fmiPayloadSize ) );
                    memset( &receipt, 0, sizeof( receipt ) );

                    receipt.status_change_id = status.status_change_id;
                    receipt.result_code = procDriverStatusUpdate( status.driver_status );

                    txFmi( FMI_ID_DRIVER_STATUS_RCPT, (uint8*)&receipt, sizeof( receipt ) );
                    break;
                }
            case FMI_ID_DRIVER_STATUS_RCPT: // 0x0822
                {
                    driver_status_receipt_data_type receipt;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    if( receipt.result_code )
                    {
                        procDriverStatusUpdate( mSentDriverStatus, receipt.driver_idx );
                    }
                    break;
                }
#endif // FMI_SUPPORT_A604
#if( FMI_SUPPORT_A606 )
            case FMI_SAFE_MODE_RESP: // 0x0901
                {
                    safe_mode_speed_receipt_data_type receipt;
                    memset( &receipt, 0, sizeof( receipt ) );

                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );
                    if( receipt.result_code == FALSE )
                    {
                        Event::post( EVENT_FMI_SAFE_MODE_ERROR );
                    }
                break;
                }
#endif // FMI_SUPPORT_A606
#if( FMI_SUPPORT_A607 )
            case FMI_ID_DRIVER_STATUS_UPDATE_D607: // 0x0823
                {
                    driver_status_D607_data_type status;
                    driver_status_receipt_data_type receipt;

                    memset( &status, 0, sizeof( status ) );
                    memcpy( &status, fmiPayload, minval( sizeof( status ), fmiPayloadSize ) );
                    memset( &receipt, 0, sizeof( receipt ) );

                    receipt.status_change_id = status.status_change_id;
                    receipt.result_code = procDriverStatusUpdate( status.driver_status, status.driver_idx );
                    receipt.driver_idx = status.driver_idx;

                    txFmi( FMI_ID_DRIVER_STATUS_RCPT, (uint8*)&receipt, sizeof( receipt ) );
                    break;
                }
#endif // FMI_SUPPORT_A607
#if( FMI_SUPPORT_A608 )
            case FMI_SPEED_LIMIT_RCPT: //0X1001
                {
                    speed_limit_receipt_data_type receipt;

                    memset( &receipt, 0, sizeof( receipt ) );
                    memcpy( &receipt, fmiPayload, minval( sizeof( receipt ), fmiPayloadSize ) );

                    Event::post( EVENT_FMI_SPEED_LIMIT_SET_RESULT, receipt.result_code );

                    break;
                }
            case FMI_SPEED_LIMIT_ALERT: //0X1002
                {
                    speed_limit_alert_data_type alert;
                    speed_limit_alert_receipt_data_type receipt;

                    memset( &alert, 0, sizeof( alert ) );
                    memcpy( &alert, fmiPayload, minval( sizeof( alert ), fmiPayloadSize ) );

                    receipt.timestamp = alert.timestamp;

                    txFmi( FMI_SPEED_LIMIT_ALERT_RCPT, (uint8*)&receipt, sizeof( receipt ) );
                    break;
                }
#endif //FMI_SUPPORT_A608
            default:
                break;
            } //end of switch( fmiPacketId )
            break;
        } // end of case ID_FMI_PACKET
#endif // FMI_SUPPORT_A602
    default:
        break;
    } // end of switch( rx_packet->id )

    return true;
} // end of FmiApplicationLayer::rx()


//----------------------------------------------------------------------
//! \brief Callback when a packet is ACKed by the client.
//! \details If the packet that was sent was an FMI Enable packet,
//!     clear the flag that indicates that protocol is in progress.
//! \param aPacket The packet that the client ACKed
//----------------------------------------------------------------------
void FmiApplicationLayer::onAck
    (
    const Packet * aPacket
    )
{
    const GarminPacket* packet = dynamic_cast<const GarminPacket*>( aPacket );

    ASSERT( packet != NULL );

#if( FMI_SUPPORT_A602 )
    switch( packet->mPacketId )
    {
    case ID_FMI_PACKET:
        {
            fmi_id_type fmiPacketId = *(fmi_id_type *) packet->mPayload;
#if( FMI_SUPPORT_A604 )
            const uint8* fmiPayload = packet->mPayload + sizeof( fmi_id_type );
#endif
            switch( fmiPacketId )
            {
            case FMI_ID_ENABLE:
                mEnablePending = false;
                break;
#if( FMI_SUPPORT_A604 )
            case FMI_ID_DATA_DELETION:
                {
                    const data_deletion_data_type *delData = (const data_deletion_data_type *)fmiPayload;
                    if( delData->data_type == DISABLE_FMI )
                    {
                        Event::post( EVENT_FMI_DISABLE_COMPLETE );
                    }
                }
                break;
#endif
            default:
                break;
            }
            break;
        }
    default:
        break;
    }
#endif
}

//----------------------------------------------------------------------
//! \brief Send an Enable PVT or Disable PVT command.
//! \param aEnable If TRUE, send an enable PVT command, else send a
//!     disable PVT command
//----------------------------------------------------------------------
void FmiApplicationLayer::sendEnablePvtCommand
    (
    bool aEnable
    )
{
    if( aEnable )
    {
        txCommand( COMMAND_TURN_ON_PVT_DATA );
    }
    else
    {
        txCommand( COMMAND_TURN_OFF_PVT_DATA );
    }
}

#if( FMI_SUPPORT_LEGACY )
//----------------------------------------------------------------------
//! \brief Send a legacy product request packet
//! \details Sends a request to the client for product ID data, using
//!     the A000 product request protocol
//! \see Garmin Device Interface SDK, at
//!     http://developer.garmin.com/web-device/device-sdk/
//! \note Use sendProductRequest() if A602 support is enabled
//----------------------------------------------------------------------
void FmiApplicationLayer::sendLegacyProductRequest()
{
    ASSERT( mTransportLayer != NULL );

    mTransportLayer->tx( new GarminPacket( ID_PRODUCT_RQST, NULL, 0, this ), TRUE );
}

//----------------------------------------------------------------------
//! \brief Initiates the legacy stop protocol.
//! \param aDegreesLatitude The destination latitude, in degrees
//!     (positive is north)
//! \param aDegreesLongitude The destination longitude, in degrees
//!     (positive is east)
//! \param aStopName A null-terminated ASCII string containing the
//!     name of the stop.
//! \note This does not include a stop id, so no further updates will
//!     be sent about this stop.
//----------------------------------------------------------------------
void FmiApplicationLayer::sendLegacyStop
    (
    double aDegreesLatitude,
    double aDegreesLongitude,
    char * aStopName
    )
{
    legacy_stop_data_type stopPacket;
    uint8                 payloadSize;
    sc_position_type      stopPosition;

    memset( &stopPacket, 0, sizeof( stopPacket ) );

    _snprintf( stopPacket.text,
        sizeof( stopPacket.text ), aStopName );

    stopPosition.lat = UTIL_convert_degrees_to_semicircles( aDegreesLatitude );
    stopPosition.lon = UTIL_convert_degrees_to_semicircles( aDegreesLongitude );

    stopPacket.stop_position = stopPosition;

    payloadSize = offset_of( legacy_stop_data_type, text ) + (uint8)strlen( stopPacket.text ) + 1;

    ASSERT( mTransportLayer != NULL );
    mTransportLayer->tx( new GarminPacket( ID_LEGACY_STOP_MSG, (uint8*) &stopPacket, payloadSize, this ), FALSE );
}

//----------------------------------------------------------------------
//! \brief Sends a text message with the legacy protocol.
//! \param aMessageText The message text, as a null-terminated ASCII
//!     string
//----------------------------------------------------------------------
void FmiApplicationLayer::sendLegacyTextMessage
    (
    char * aMessageText
    )
{
    ASSERT( mTransportLayer != NULL );

    uint8 payloadSize = minval( 200, strlen( aMessageText ) + 1);

    mTransportLayer->tx( new GarminPacket( ID_LEGACY_TEXT_MSG, (uint8*) aMessageText, payloadSize, this ), FALSE );
}
#endif

//----------------------------------------------------------------------
//! \brief Request the Unit ID (ESN) from the client
//----------------------------------------------------------------------
void FmiApplicationLayer::sendUnitIdRequest()
{
    txCommand( COMMAND_REQ_UNIT_ID );
}

//----------------------------------------------------------------------
//! \brief Clear any communication error.
//! \details Instructs the TransportLayer to retry sending the last
//!    packet.
//----------------------------------------------------------------------
void FmiApplicationLayer::clearError()
{
    ASSERT( mTransportLayer != NULL );
    mTransportLayer->retry();
}

#if( FMI_SUPPORT_A602 )
//----------------------------------------------------------------------
//! \brief Initiates the A602 stop protocol.
//! \param aDegreesLatitude The destination latitude, in degrees
//!     (positive is north)
//! \param aDegreesLongitude The destination longitude, in degrees
//!     (positive is east)
//! \param aStopName A null-terminated string in the client codepage
//!     containing the name of the stop.
//! \note This does not include a stop id, so no further updates will
//!     be sent about this stop.
//! \since Protocol A602
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA602Stop
    (
    double aDegreesLatitude,
    double aDegreesLongitude,
    char * aStopName
    )
{
    A602_stop_data_type stopData;
    uint8               stopPayloadSize;
    sc_position_type    position;

    memset( &stopData, 0, sizeof( stopData ) );

    _snprintf( stopData.text,
        sizeof( stopData.text ), aStopName );

    position.lat = UTIL_convert_degrees_to_semicircles( aDegreesLatitude );
    position.lon = UTIL_convert_degrees_to_semicircles( aDegreesLongitude );

    stopData.stop_position = position;

    time_type originationTime = UTIL_get_current_garmin_time();
    stopData.origination_time = originationTime;
    stopPayloadSize = offset_of( A602_stop_data_type, text ) + (uint8)strlen( stopData.text ) + 1;

    txFmi( FMI_ID_A602_STOP, (uint8*)&stopData, stopPayloadSize );
}

//----------------------------------------------------------------------
//! \brief Send a server to client text message, for all A602 FMI
//!     protocols.
//! \details This function is called for all A602 FMI text messages.
//!     Since these text message protocols don't need an id or id_size,
//!     these arguments are given default values of NULL and 0
//!     respectively
//! \param aFmiPacketId FMI packet ID for the protocol to use
//! \param aMessageText Message text, as a null-terminated string in
//!     the client codepage
//! \param aMessageId The message ID.
//! \since Protocol A602
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA602TextMessage
    (
    fmi_id_type       aFmiPacketId,
    char            * aMessageText,
    const MessageId & aMessageId
    )
{
    uint8    payloadSize;

    //all messages need a originationTime, so calculate it now
    time_type originationTime = UTIL_get_current_garmin_time();

    if( aFmiPacketId == FMI_ID_SERVER_OPEN_TXT_MSG )
    {
        A602_server_to_client_open_text_msg_data_type    textMessage;
        memset( &textMessage, 0, sizeof( textMessage ) );
        textMessage.origination_time = originationTime;
        _snprintf
            (
            textMessage.text_message,
            sizeof( textMessage.text_message ),
            aMessageText
            );
        payloadSize = offset_of( A602_server_to_client_open_text_msg_data_type, text_message )
                    + (uint8)strlen( textMessage.text_message )
                    + 1;
        txFmi( aFmiPacketId, (uint8*)&textMessage, payloadSize );
    }
    else
    {
        server_to_client_ack_text_msg_data_type    textMessage;
        memset( &textMessage, 0, sizeof( textMessage ) );
        textMessage.origination_time = originationTime;
        textMessage.id_size = aMessageId.getIdSize();
        memmove( textMessage.id, aMessageId.getId(), textMessage.id_size );
        _snprintf
            (
            textMessage.text_message,
            sizeof( textMessage.text_message ),
            aMessageText
            );
        payloadSize = offset_of( server_to_client_ack_text_msg_data_type, text_message )
                    + (uint8)strlen( textMessage.text_message )
                    + 1;
        txFmi( aFmiPacketId, (uint8*)&textMessage, payloadSize );
    }
}

//----------------------------------------------------------------------
//! \brief Transmit an FMI Enable packet, and clear any error state.
//! \details Make sure there is an FMI Enable packet at the head of the
//!     tx queue, then send it.  This packet may already be present
//!     if a previous enable timed out.
//! \param aFeatureCodes Array of feature codes to send
//! \param aFeatureCount Number of feature codes to send.  To send an
//!     A602 enable, this must be zero.
//! \since Protocol A602
//----------------------------------------------------------------------
void FmiApplicationLayer::sendEnable
    (
    uint16 * aFeatureCodes,
    uint8    aFeatureCount
    )
{
    fmi_features_data_type features;
    uint8 dataSize;

    if( aFeatureCount > 126 )
    {
        aFeatureCount = 126;
    }

    features.feature_count = aFeatureCount;
    features.reserved = 0;
    memset( features.features, 0, 126*sizeof( uint16 ) );
    memcpy( features.features, aFeatureCodes, aFeatureCount * sizeof( uint16 ) );

    dataSize = offset_of( fmi_features_data_type, features )
             + sizeof( uint16 ) * features.feature_count;

    if( mEnablePending )
    {
        ASSERT( mTransportLayer != NULL );
        mTransportLayer->retry();
    }
    else
    {
        txFmi( FMI_ID_ENABLE, (uint8*)&features, dataSize, TRUE );
    }

    mEnablePending = true;
}

//----------------------------------------------------------------------
//! \brief Send an arbitrary FMI packet
//! \details Sends a user-specified FMI packet to the client.  This
//!     packet is not checked for errors, and no data structures are
//!     updated when the packet is sent.  However, packets received
//!     in response to this packet are handled normally.
//! \param  aFmiPacketId The FMI packet ID
//! \param  aFmiPayload The FMI payload
//! \param  aFmiPayloadSize The size of the FMI payload
//! \since Protocol A602
//----------------------------------------------------------------------
void FmiApplicationLayer::sendFmiPacket
    (
    uint16  aFmiPacketId,
    uint8*  aFmiPayload,
    uint8   aFmiPayloadSize
    )
{
    txFmi( aFmiPacketId, aFmiPayload, aFmiPayloadSize );
}
//----------------------------------------------------------------------
//! \brief Send an FMI Product ID request.
//! \since Protocol A602
//----------------------------------------------------------------------
void FmiApplicationLayer::sendProductRequest()
{
    txFmi( FMI_ID_PRODUCT_ID_SUPPORT_RQST, NULL, 0 );
}
#endif

#if( FMI_SUPPORT_A603 )
//----------------------------------------------------------------------
//! \brief Find an available stop ID.
//! \return A stop ID that is not in the map.
//! \since Protocol A603
//----------------------------------------------------------------------
uint32 FmiApplicationLayer::getNextStopId()
{
    uint32 uniqueId = 1;

    if( !mA603Stops.empty() )
    {
        //loop through the map and find an id hole
        FileBackedMap<StopListItem>::const_iterator iter = mA603Stops.begin();
        for(; iter != mA603Stops.end(); iter++ )
        {
            if( iter->first != uniqueId ) //found a hole!
            {
                uniqueId = iter->first - 1;             //leave the biggest hole so next
                break;                                  //search can possibly be as fast (unless only one spot)
            }
            else
                uniqueId = iter->first + 1;             //no hole yet, so use the next ID (maybe)
        }
    }

    ASSERT( !mA603Stops.contains( uniqueId ) );

    return uniqueId;
}

//----------------------------------------------------------------------
//! \brief Find the index of the deleted stop corresponding to a stop
//!     ID.
//! \details Since the index in the stop list is not sent back on a
//!     stop status update for a deleted stop, we must find the index
//!     that needs to be deleted before calling update_delete_stop.
//! \param  aUniqueId The unique_id of the stop to delete
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::removeStopById
    (
    uint32 aUniqueId
    )
{
    for( uint16 i = 0; i < mStopIndexInList.size(); i++ )
    {
        if( mStopIndexInList[i] == aUniqueId )
        {
            removeStopByIndex( i );
        }
    }
}

//----------------------------------------------------------------------
//! \brief Remove a deleted stop.
//! \details This function is called when the client deletes a stop or
//!     responds to a delete request by the server.  It removes the
//!     unique id from the mA603Stops, updates the mStopIndexInList
//!     vector, and deletes the stop from the save file.
//! \param  aStopIndex The index of the stop in the list.
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::removeStopByIndex
    (
    uint16 aStopIndex
    )
{
    int i = 0;
    int size = (int)mStopIndexInList.size();
    mA603Stops.remove( mStopIndexInList[aStopIndex] );
    if( mA603Stops.empty() )
    {
        mStopIndexInList.clear();
    }
    else
    {
        //stops below the deleted stop need to be shifted up
        //check to make sure this isn't the last stop in the list
        if( aStopIndex + 1 < size )
        {
            //get id after deleted stop
            uint32 id = mStopIndexInList[aStopIndex + 1];
            while( id != INVALID32 )
            {
                //next index replaces previous index
                mStopIndexInList[aStopIndex + i] = id;
                i++;
                //get next id to replace
                if( aStopIndex + i + 1 < size )
                    id = mStopIndexInList[aStopIndex + i + 1];
                else
                    break;    //if we are over running the vector we are done
            }
            //mark whatever was the last index as invalid (one less now)
            mStopIndexInList[aStopIndex + i] = INVALID32;
        }
        else
        {
            mStopIndexInList[aStopIndex] = INVALID32;    //deleting last stop so nothing
        }                                    //else needs to be done
    }
}
//----------------------------------------------------------------------
//! \brief Initiates the A603 stop protocol.
//! \param aLatitude The destination latitude, in degrees (positive is
//!     north)
//! \param aLongitude The destination longitude, in degrees (positive
//!     is east)
//! \param aStopName A null-terminated string containing the name of
//!     the stop.
//! \param aStopId The stop ID to use.
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA603Stop
    (
    double aLatitude,
    double aLongitude,
    char * aStopName,
    uint32 aStopId
    )
{
    A603_stop_data_type stopData;
    uint8               stopPayloadSize;
    sc_position_type    position;
    wofstream           stopFile;
    TCHAR               stopNameWide[200];
    char                stopName[200];

    memset( &stopData, 0, sizeof( stopData ) );
    memset( stopName, 0, sizeof( stopName ) );

    _snprintf
        (
        stopData.text,
        minval( sizeof( stopData.text ) - 1, strlen( aStopName ) ),
        aStopName
        );
    //has to be null terminated

    position.lat = UTIL_convert_degrees_to_semicircles( aLatitude );
    position.lon = UTIL_convert_degrees_to_semicircles( aLongitude );

    stopData.stop_position = position;
    stopData.unique_id = aStopId;

    //update map
    StopListItem& stopListItem = mA603Stops.get( aStopId );
    MultiByteToWideChar( mClientCodepage, 0, stopData.text, -1, stopNameWide, 200 );
    stopListItem.setCurrentName( stopNameWide );
    stopListItem.setStopStatus( STOP_STATUS_UNREAD );
    stopListItem.setValid();
    mA603Stops.put( stopListItem );

    stopData.origination_time = UTIL_get_current_garmin_time();
    stopPayloadSize = offset_of( A603_stop_data_type, text )
                    + (uint8)strlen( stopData.text )
                    + 1;
    txFmi( FMI_ID_A603_STOP, (uint8*)&stopData, stopPayloadSize );
}

//----------------------------------------------------------------------
//! \brief Send a request to update the auto-arrival preferences.
//! \param aAutoArrivalTime The minimum stop time, in seconds
//! \param aAutoArrivalDistance The minimum stop distance, in meters
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::sendAutoArrival
    (
    uint32 aAutoArrivalTime,
    uint32 aAutoArrivalDistance
    )
{
    auto_arrival_data_type        autoArrival;
    autoArrival.stop_distance = aAutoArrivalDistance;
    autoArrival.stop_time = aAutoArrivalTime;
    txFmi( FMI_ID_AUTO_ARRIVAL, (uint8*)&autoArrival, sizeof( autoArrival ) );
}

#if( FMI_SUPPORT_A606 )
//---------------------------------------------------------------------
//! \brief Set FMI safe mode speed
//! \details This function sets safe mode speed on the client
//! \param aSpeed Safe mode speed
//! \return TRUE if the packet was ACK'd, FALSE if a timeout occurred
//! \since Protocol A606
//---------------------------------------------------------------------
void FmiApplicationLayer::sendFmiSafeModeSpeed
    (
    float aSpeed
    )
{
    uint8                       data_size;
    safe_mode_speed_data_type   data;
    memset( &data, 0, sizeof( data ) );

    data.speed = aSpeed;

    data_size = sizeof( aSpeed );
    txFmi( FMI_SAFE_MODE,(uint8*)&data, data_size );
}
#endif

//----------------------------------------------------------------------
//! \brief Send a data deletion request and remove the appropriate data
//!    from the server.
//! \param aDelDataType The type of data that the client should delete
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDataDeletionRequest
    (
    del_data aDelDataType
    )
{
    // Perform the data deletion protocol
    data_deletion_data_type        data;
    data.data_type = aDelDataType;
    txFmi( FMI_ID_DATA_DELETION, (uint8*)&aDelDataType, sizeof( data ) );

    // Based on the requested data type, reset the server-side data
    switch( aDelDataType )
    {
#if FMI_SUPPORT_A603
    case DELETE_ALL_STOPS:
        resetSavedStops();
        break;
#endif

#if FMI_SUPPORT_A604
    case DELETE_ALL_MESSAGES:
        resetCannedResponseMessages();
        break;

    case DELETE_CANNED_MESSAGES:
        resetCannedMessages();
        break;

    case DELETE_CANNED_RESPONSES:
        resetCannedResponses();
        break;

    case DELETE_DRIVER_ID_AND_STATUS:
        resetDriverStatusList();
        break;

    case DISABLE_FMI:
        resetCannedMessages();
        resetCannedResponseMessages();
        resetDriverStatusList();
        resetSavedStops();
#if( FMI_SUPPORT_A607 )
        resetWaypoints();
#endif
        break;
#endif

#if( FMI_SUPPORT_A607 )
    case DELETE_WAYPOINTS:
        resetWaypoints();
        break;
#endif

    }
}

//----------------------------------------------------------------------
//! \brief Send a request for ETA data.
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::sendEtaRequest()
{
    txFmi( FMI_ID_ETA_DATA_REQUEST, NULL, 0 );
}

//----------------------------------------------------------------------
//! \brief Send a stop move request
//! \param  aMoveToIndex The index that the stop should be moved to
//! \param  aMoveFromIndex The index that the stop should be moved from
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::sendStopMoveRequest
    (
    uint16 aMoveToIndex,
    uint16 aMoveFromIndex
    )
{
    uint32 moveToId;
    uint32 stopId;

    //starting out with stopId the stop that is being moved
    //and moveToId the stop at the index it will be moved to
    moveToId = mStopIndexInList[aMoveToIndex];
    stopId = mStopIndexInList[aMoveFromIndex];
    sendStopStatusRequest( stopId, REQUEST_MOVE_STOP, aMoveToIndex );

    // The client will not send stop aStopStatus updates (with the new
    // list index) under at least two known conditions:
    // 1. The stop aStopStatus protocol is throttled.
    // 2. The client software is older. (Prior to A604, clients only
    // send a stop aStopStatus for the stop indicated in the request, but
    // not for the other stops that were moved to make room.)
    // So, we need to update the position of all affected stops now.
    if( aMoveFromIndex < aMoveToIndex )
    {
        //moving down, so move and shift others up
        //starting at aMoveToIndex - i (where to move the selected stop)
        //and then moving up a position for each stop affected
        for( int i = 0; i <= aMoveToIndex - aMoveFromIndex; i++ )
        {
            mStopIndexInList[aMoveToIndex-i] = stopId;
            stopId = moveToId;
            if( i != aMoveToIndex - aMoveFromIndex )
                moveToId = mStopIndexInList[aMoveToIndex - i - 1];
        }
    }
    else if( aMoveToIndex < aMoveFromIndex )
        //move up, so move and shift others down
        //starting at aMoveToIndex + i (where to move the selected stop)
        //and then moving down a position for each stop affected
    {
        for( int i = 0; i <= aMoveFromIndex - aMoveToIndex; i++ )
        {
            mStopIndexInList[aMoveToIndex + i] = stopId;
            stopId = moveToId;
            if( i != aMoveFromIndex - aMoveToIndex )
                moveToId = mStopIndexInList[aMoveToIndex + i + 1];
        }
    }
}

//----------------------------------------------------------------------
//! \brief Initiate the Stop Status protocol to update an A603 stop.
//! \param aStopId The unique_id of the stop to update
//! \param aStopStatus The new stop aStopStatus
//! \param aMoveToIndex The stop_position_in_list if this is a move
//!     stop request.  Defaults to INVALID16 (0xFFFF)
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::sendStopStatusRequest
    (
    uint32                  aStopId,
    stop_status_status_type aStopStatus,
    uint16                  aMoveToIndex
    )
{
    stop_status_data_type    stopData;
    stopData.unique_id = aStopId;
    stopData.stop_status = aStopStatus;
    switch( aStopStatus )
    {
        //these 4 all do require stop index to be INVALID16
    case REQUEST_STOP_STATUS:
    case REQUEST_MARK_STOP_DONE:
    case REQUEST_ACTIVATE_STOP:
    case REQUEST_DELETE_STOP:
        stopData.stop_index_in_list = INVALID16;
        break;
    case REQUEST_MOVE_STOP:
        stopData.stop_index_in_list = aMoveToIndex;
        break;
    default:
        ASSERT( false ); // probably a bug
        break;
    }
    txFmi( FMI_ID_STOP_STATUS_REQUEST, (uint8*) &stopData, sizeof( stopData ) );
}

#endif

#if( FMI_SUPPORT_A604 )
//----------------------------------------------------------------------
//! \brief Send an A604 server to client open text message.
//! \details This function is called to send an A604 server to client
//!     text message.
//! \param aMessageText Message text, as a null-terminated string in
//!     the client codepage
//! \param aMessageId The message ID.
//! \param aMessageType The message type, a value from the
//!     a604_message_type enum
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA604TextMessage
    (
    const char      * aMessageText,
    const MessageId & aMessageId,
    uint8             aMessageType
    )
{
    uint8    payloadSize;

    // all messages need a origination time, so calculate it now
    time_type originationTime = UTIL_get_current_garmin_time();

    A604_server_to_client_open_text_msg_data_type    textMessage;
    memset( &textMessage, 0, sizeof( textMessage ) );
    textMessage.origination_time = originationTime;
    textMessage.id_size = aMessageId.getIdSize();
    memmove( textMessage.id, aMessageId.getId(), aMessageId.getIdSize() );
    _snprintf( textMessage.text_message, sizeof( textMessage.text_message ), aMessageText );
    payloadSize = offset_of( A604_server_to_client_open_text_msg_data_type, text_message )
        + (uint8)strlen( textMessage.text_message )
        + 1;
    textMessage.message_type = aMessageType;
    txFmi( FMI_ID_A604_OPEN_TEXT_MSG, (uint8*)&textMessage, payloadSize );
}
//----------------------------------------------------------------------
//! \brief Initiate the canned response text message protocol.
//! \details Send a Canned Response List packet, and save the values
//!     (message text, message type, etc.) that are sent in later
//!     packets.  The actual text message will be sent by handle_rx
//!     when the canned response list receipt is processed.
//! \param aMessageText The message text, as a null-terminated string
//!     in the client codepage
//! \param aMessageId The message ID
//! \param aResponseCount The number of allowed responses
//! \param aResponseList Pointer to an array of allowed responses.
//! \param aMessageType The aMessageType for the message; see the
//!     a604_message_type enum.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendCannedResponseTextMessage
    (
    const char      * aMessageText,
    const MessageId & aMessageId,
    uint8             aResponseCount,
    const uint32    * aResponseList,
    uint8             aMessageType
    )
{
    uint8                            payloadSize;
    canned_response_list_data_type   data;

    mCannedResponseMessageType = aMessageType;
    strcpy( mCannedResponseMessageBody, aMessageText );

    memset( &data, 0, sizeof( data ) );
    data.id_size = aMessageId.getIdSize();
    memcpy( data.id, aMessageId.getId(), data.id_size );
    data.response_count = (uint8) minval( 50, aResponseCount );
    memcpy( data.response_id, aResponseList, sizeof(uint32) * data.response_count );

    payloadSize = offset_of( canned_response_list_data_type, response_id ) + sizeof( uint32 ) * aResponseCount;

    txFmi( FMI_ID_SET_CANNED_RESP_LIST, (uint8*)&data, payloadSize );
}

//----------------------------------------------------------------------
//! \brief Transmit a new or updated canned response to the client.
//! \param aResponseId The canned response ID
//! \param aResponseText The response text associated with the ID
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendCannedResponse
    (
    uint32  aResponseId,
    CString aResponseText
    )
{
    uint8    payloadSize;
    canned_response_data_type  data;
    char temp[50];

    //Update the canned response list with the updated text.
    ClientListItem& responseItem = mCannedResponses.get( aResponseId );
    responseItem.setUpdateName( aResponseText );
    mCannedResponses.put( responseItem );

    //Build and send the packet
    memset( &data, 0, sizeof( data ) );

    WideCharToMultiByte( mClientCodepage, 0, aResponseText.GetBuffer(), -1, temp, 50, NULL, NULL );
    temp[49] = '\0';
    strcpy( data.response_text, temp );

    data.response_id = aResponseId;
    payloadSize = (uint8)( offset_of( canned_response_data_type, response_text ) + strlen( data.response_text ) + 1 );
    txFmi( FMI_ID_SET_CANNED_RESPONSE, (uint8*)&data, payloadSize );
}

//----------------------------------------------------------------------
//! \brief Transmit a new or updated canned message to the client.
//! \param aMessageId The canned message ID
//! \param aMessageText The message text associated with the ID
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendCannedMessage
    (
    uint32  aMessageId,
    CString aMessageText
    )
{
    uint8    payloadSize;
    canned_message_data_type  data;
    char temp[50];

    //Update the canned message list on the server
    ClientListItem& listItem = mCannedMessages.get( aMessageId );
    listItem.setUpdateName( aMessageText );
    listItem.setId( aMessageId );
    mCannedMessages.put( listItem );

    //Build and send the packet
    memset( &data, 0, sizeof( data ) );
    WideCharToMultiByte( mClientCodepage, 0, aMessageText.GetBuffer(), -1, temp, 50, NULL, NULL );
    temp[49] = '\0';

    data.message_id = aMessageId;
    strcpy( data.message, temp );
    payloadSize = offset_of( canned_message_data_type, message ) + (uint8) strlen( data.message ) + 1;
    txFmi( FMI_ID_SET_CANNED_MSG, (uint8*)&data, payloadSize );
}

//----------------------------------------------------------------------
//! \brief Send a request to the client to delete a canned response.
//! \param aResponseId The canned response ID to delete
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDeleteCannedResponseRequest
    (
    uint32 aResponseId
    )
{
    canned_response_delete_data_type  data;
    data.response_id = aResponseId;
    txFmi( FMI_ID_DELETE_CANNED_RESPONSE, (uint8*)&data, sizeof( data ) );
}

//----------------------------------------------------------------------
//! \brief Send a request to the client to delete a canned message.
//! \param aMessageId The canned message ID to delete
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDeleteCannedMessageRequest
    (
    uint32 aMessageId
    )
{
    canned_message_delete_data_type data;
    data.message_id = aMessageId;
    txFmi( FMI_ID_DELETE_CANNED_MSG, (uint8*)&data, sizeof( data ) );
}

//----------------------------------------------------------------------
//! \brief Initiate the Delete Driver Status List Item protocol
//! \param aStatusId The ID of the item to be deleted on the client
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDeleteDriverStatusListItem
    (
    uint32    aStatusId
    )
{
    driver_status_list_item_delete_data_type deleteItem;
    deleteItem.status_id = aStatusId;
    txFmi( FMI_ID_DELETE_DRIVER_STATUS_LIST_ITEM, (uint8*)&deleteItem, sizeof( deleteItem ) );
}

//----------------------------------------------------------------------
//! \brief Ask the client for the current Driver ID
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDriverIdRequest()
{
    txFmi( FMI_ID_DRIVER_ID_REQUEST, NULL, 0 );
}

#if( FMI_SUPPORT_A607 )
//----------------------------------------------------------------------
//! \brief Ask the client for the current Driver ID
//! \param aDriverIndex The index of the driver
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA607DriverIdRequest
    (
    uint8 aDriverIndex
    )
{
    driver_id_request_data_type request;
    memset( &request, 0, sizeof( request ) );
    request.driver_idx = aDriverIndex;

    txFmi( FMI_ID_DRIVER_ID_REQUEST, (uint8*)&request, sizeof( request ) );
}

//----------------------------------------------------------------------
//! \brief Ask the client for the current Driver Status
//! \param aDriverIndex The index of the driver
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA607DriverStatusRequest
    (
    uint8 aDriverIndex
    )
{
    driver_id_request_data_type request;
    memset( &request, 0, sizeof( request ) );
    request.driver_idx = aDriverIndex;

    txFmi( FMI_ID_DRIVER_STATUS_REQUEST, (uint8*)&request, sizeof( request ) );
}
#endif

//----------------------------------------------------------------------
//! \brief Send a Driver ID Update to the client
//! \param  aDriverId The new driver ID, in the client's code page
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDriverIdUpdate
    (
    char * aDriverId
    )
{
    uint8     payloadSize;
    time_type originationTime = UTIL_get_current_garmin_time();

    strcpy( mSentDriverId, aDriverId );

    driver_id_data_type data;
    memset( &data, 0, sizeof( data ) );

    //need to allow app to actually set this field
    data.status_change_id = originationTime;
    data.status_change_time = originationTime;
    strcpy( data.driver_id, aDriverId );
    data.driver_id[49] = '\0';
    payloadSize = offset_of( driver_id_data_type, driver_id ) + (uint8)strlen( data.driver_id ) + 1;

    txFmi( FMI_ID_DRIVER_ID_UPDATE, (uint8*)&data, payloadSize );
}

//----------------------------------------------------------------------
//! \brief Initiate the Set Driver Status List Item protocol
//! \param  aStatusId The ID of the aStatusText item to be added (or
//!     updated) on the client
//! \param  aStatusText The textual description corresponding to the id
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDriverStatusListItem
    (
    uint32        aStatusId,
    CString       aStatusText
    )
{
    uint8                               payloadSize;
    char                                statusText[50];
    driver_status_list_item_data_type   data;

    // Update the driver aStatusText list on the server side
    ClientListItem& listItem = mDriverStatuses.get( aStatusId );
    listItem.setUpdateName( aStatusText );
    mDriverStatuses.put( listItem );

    // Send the driver aStatusText item to the client
    memset( &data, 0, sizeof( data ) );
    WideCharToMultiByte( mClientCodepage, 0, aStatusText.GetBuffer(), -1, statusText, 50, NULL, NULL );
    statusText[49] = '\0';

    data.status_id = aStatusId;
    strcpy( data.status, statusText );
    payloadSize = offset_of( driver_status_list_item_data_type, status ) + (uint8)strlen( data.status ) + 1;

    txFmi( FMI_ID_SET_DRIVER_STATUS_LIST_ITEM, (uint8*)&data, payloadSize );
}

//----------------------------------------------------------------------
//! \brief  Asks the client for the current driver status
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDriverStatusRequest()
{
    txFmi( FMI_ID_DRIVER_STATUS_REQUEST, NULL, 0 );
}

//----------------------------------------------------------------------
//! \brief Initiate the Driver Status Update protocol
//! \param  aDriverStatusId The new driver status.
//! \note The driver_status specified must already have been sent to
//!     the client using the Set Driver Status List Item protocol.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDriverStatusUpdate
    (
    uint32 aDriverStatusId
    )
{
    time_type originationTime = UTIL_get_current_garmin_time();
    mSentDriverStatus = aDriverStatusId;

    driver_status_data_type  status;
    memset( &status, 0, sizeof( status ) );

    //need to make this an input
    status.status_change_id = originationTime;

    status.status_change_time = originationTime;
    status.driver_status = aDriverStatusId;

    txFmi( FMI_ID_DRIVER_STATUS_UPDATE, (uint8*)&status, sizeof( status ) );
}

//----------------------------------------------------------------------
//! \brief Sends an update to the client to tell it whether to enable
//!     or disable a protocol
//! \param  aPacketId The packet ID of the first packet in the protocol
//! \param  aThrottleState The new state (0=disable, 1=enable)
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendMessageThrottlingUpdate
    (
    uint16 aPacketId,
    uint16 aThrottleState
    )
{
    message_throttling_data_type throttle;
    throttle.packet_id = aPacketId;
    throttle.new_state = aThrottleState;

    txFmi( FMI_ID_MSG_THROTTLING_COMMAND, (uint8*)&throttle, sizeof( throttle ) );
}
//----------------------------------------------------------------------
//! \brief Start a GPI file transfer protocol.
//! \details This sends the start request, the rest of the transfer is
//!   performed in rx when the receipt packets are received.
//! \param aFilename The local file name to transfer
//! \param aVersionLength The number of significant bytes of aVersion
//!     (must be between 0 and 16, inclusive)
//! \param aVersion  An array of bytes of aVersion data to associate
//!     with this file.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendGpiFile
    (
    char  * aFilename,
    uint8   aVersionLength,
    uint8 * aVersion
    )
{
    fstream gpiFile( aFilename, ios_base::binary | ios_base::in );
    int length = 0;
    if( gpiFile.good() )
    {
        gpi_file_info_data_type        fileInfo;
        memset( &fileInfo, 0, sizeof( fileInfo ) );

        gpiFile.seekg( 0, ios::end );
        length = gpiFile.tellg();

        gpiFile.seekg( 0, ios::beg );
        fileInfo.file_size = length;
        mGpiFileTransferSize = length;

        fileInfo.file_version_length = aVersionLength;
        memcpy( &fileInfo.file_version, aVersion, minval( aVersionLength, 16 ) );

        mGpiTransferState = TRANSFER_STARTED;
        strcpy( mGpiFilePath, aFilename );
        mStopGpiTransfer = FALSE;

        Event::post( EVENT_FMI_GPI_TRANSFER_STATE_CHANGED );
        txFmi
            (
            FMI_ID_GPI_FILE_TRANSFER_START,
            (uint8*)&fileInfo,
            sizeof( fileInfo ) - sizeof( fileInfo.file_version ) + fileInfo.file_version_length
            );
    }
    gpiFile.close();
}

//----------------------------------------------------------------------
//! \brief Send a GPI File Information Request to the client.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendGpiFileInfoRequest()
{
    txFmi( FMI_ID_GPI_FILE_INFORMATION_REQUEST, NULL, 0 );
}

//----------------------------------------------------------------------
//! \brief Stop a GPI file transfer protocol.
//! \details This sets a flag to stop a GPI file transfer.  The flag
//!     will be processed when the next GPI file or packet receipt is
//!     processed in handle_rx().
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::stopGpiFileTransfer()
{
    mStopGpiTransfer = TRUE;
} /* stopGpiFileTransfer() */

//----------------------------------------------------------------------
//! \brief Send a ping packet to the client.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendPing()
{
    txFmi( FMI_ID_PING, NULL, 0 );
}

//----------------------------------------------------------------------
//! \brief Sends a sort stop list request to the client.
//! \note Once the request is completed, the server will send stop
//!     aStopStatus requests for all A603 stops to determine their new
//!     position in the list.  These requests are sent in rx().
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendStopSortRequest()
{
    txFmi( FMI_ID_SORT_STOP_LIST, NULL, 0 );
}

//----------------------------------------------------------------------
//! \brief Ask the server for the aStopStatus of a text message
//! \param  aMessageId The message ID
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendTextMessageStatusRequest
    (
    const MessageId & aMessageId
    )
{
    message_status_request_data_type    request;

    memset( &request, 0, sizeof( request ) );
    request.id_size = aMessageId.getIdSize();
    memcpy( request.id, aMessageId.getId(), request.id_size );

    txFmi( FMI_ID_TEXT_MSG_STATUS_REQUEST, (uint8*)&request, sizeof( request ) );
}

//----------------------------------------------------------------------
//! \brief Request that the text of a particular user interface text
//!     element be changed
//! \param  aElementId The ID of the element
//! \param  aNewText A null-terminated string (50 characters max)
//!     containing the new text for the element.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::sendUserInterfaceText
    (
    uint32   aElementId,
    char   * aNewText
    )
{
    user_interface_text_data_type uiText;
    uint8 payloadSize;
    uint8 textLength;

    memset( &uiText, 0, sizeof( uiText ) );

    uiText.text_element_id = aElementId;

    //! \todo Bug here?
    textLength = minval( (uint8) strlen( aNewText ) + 1, 49 );
    memcpy( uiText.new_text, aNewText, textLength );

    //! \todo Bug here?
    payloadSize = offset_of( user_interface_text_data_type, new_text ) + (uint8) strlen( uiText.new_text ) + 1;

    txFmi( FMI_ID_USER_INTERFACE_TEXT, (uint8*)&uiText, payloadSize );
}

#endif

#if( FMI_SUPPORT_A608 )
//---------------------------------------------------------------------
//! \brief Set FMI speed limit alerts
//! \details This function sets speed limit alerts on the client
//! \param aMode Mode
//! \param aTimeOver Time over in seconds
//! \param aTimeUnder Time under in seconds
//! \param aAlertUser Alert audibly when speeding event begins
//! \param aThreshold Threshold in m/s over speed limit
//! \since Protocol A608
//---------------------------------------------------------------------
void FmiApplicationLayer::sendSetSpeedLimitAlerts
    (
    uint8   aMode,
    uint8   aTimeOver,
    uint8   aTimeUnder,
    boolean aAlertUser,
    float   aThreshold
    )
{
    speed_limit_data_type   data;
    data.mode = aMode;
    data.time_over = aTimeOver;
    data.time_under = aTimeUnder;
    data.alert_user = aAlertUser;
    data.threshold = aThreshold;

    txFmi( FMI_SPEED_LIMIT_SET,(uint8*)&data, sizeof( data ) );
}
#endif

#if( FMI_SUPPORT_A605 )
//----------------------------------------------------------------------
//! \brief Initiate the Message Throttling Query protocol
//! \since Protocol A605
//----------------------------------------------------------------------
void FmiApplicationLayer::sendMessageThrottlingQuery()
{
    txFmi( FMI_ID_MSG_THROTTLING_QUERY, NULL, 0 );
}
#endif

#if FMI_SUPPORT_A607
//----------------------------------------------------------------------
//! \brief Send a waypoint
//! \param aUniqueId Server-assigned unique ID for the waypoint
//! \param aLat The waypoint latitude, in degrees (east is positive)
//! \param aLon The waypoint longitude, in degrees (north is positive)
//! \param aSymbol The waypoint symbol
//! \param aName The waypoint name
//! \param aCat Categories that this waypoint is part of (bit mapped)
//! \param aComment The waypoint comment
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendWaypoint
    (
    uint16 aUniqueId,
    double aLat,
    double aLon,
    uint16 aSymbol,
    CString &aName,
    uint16 aCat,
    CString &aComment
    )
{
    waypoint_data_type waypoint;

    waypoint.unique_id = aUniqueId;
    waypoint.posn.lat = UTIL_convert_degrees_to_semicircles( aLat );
    waypoint.posn.lon = UTIL_convert_degrees_to_semicircles( aLon );
    waypoint.symbol = aSymbol;

    //Update the waypoint list on the server
    WaypointListItem& listItem = mWaypoints.get( aUniqueId );
    listItem.setUpdateName( aName );
    listItem.setCategories( aCat );
    mWaypoints.put( listItem );

    waypoint.cat = aCat;

    WideCharToMultiByte( mClientCodepage, 0, aName.GetBuffer(), -1, waypoint.name, 30, NULL, NULL );
    waypoint.name[30] = '\0';

    WideCharToMultiByte( mClientCodepage, 0, aComment.GetBuffer(), -1, waypoint.comment, 50, NULL, NULL );
    waypoint.comment[50] = '\0';

    txFmi( FMI_ID_WAYPOINT, (uint8*)&waypoint, sizeof( waypoint ) );
}

//----------------------------------------------------------------------
//! \brief Delete a waypoint
//! \param aUniqueId The unique ID of the waypoint to delete
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDeleteWaypoint
    (
    uint16 aUniqueId
    )
{
    txFmi( FMI_ID_WAYPOINT_DELETE, (uint8*)&aUniqueId, sizeof( aUniqueId ) );
}

//----------------------------------------------------------------------
//! \brief Delete waypoint category
//! \param aCatIdx The category to delete
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendDeleteWaypointCat
    (
    uint16 aCatIdx
    )
{
    FileBackedMap<WaypointListItem>::const_iterator iter;
    bool update = false;

    for( iter = mWaypoints.begin(); iter != mWaypoints.end(); ++iter )
    {
        if( iter->second.getCategories() & aCatIdx )
        {
            update = true;

            // Set the waypoint as invalid, which will remove it from
            // the save file.  The item cannot be removed from the
            // FileBackedMap because it is being iterated over.
            WaypointListItem& wpt = mWaypoints.get( iter->first );
            wpt.setValid( FALSE );
        }
    }

    if( update )
    {
        Event::post( EVENT_FMI_WAYPOINT_LIST_CHANGED );
    }

    txFmi( FMI_ID_DELETE_WAYPOINT_CAT, (uint8*)&aCatIdx, sizeof( aCatIdx ) );
}

//----------------------------------------------------------------------
//! \brief Create waypoint category
//! \param aCatId The category ID (0-15)
//! \param aCatName The category name
//----------------------------------------------------------------------
void FmiApplicationLayer::sendCreateWaypointCat
    (
    uint8 aCatId,
    CString &aCatName
    )
{
    category_data_type category;
    category.id = aCatId;

    WideCharToMultiByte( mClientCodepage, 0, aCatName.GetBuffer(), -1, category.name, 16, NULL, NULL );
    category.name[16] = '\0';

    //Update the waypoint list on the server
    ClientListItem& listItem = mCategories.get( aCatId );
    listItem.setUpdateName( aCatName );
    mCategories.put( listItem );

    txFmi( FMI_ID_CREATE_WAYPOINT_CAT, (uint8*)&category, sizeof( category ) );
}

//----------------------------------------------------------------------
//! \brief Ask the server to delete a text message
//! \param aMessageId The message ID
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendMessageDeleteRequest
    (
    const MessageId & aMessageId
    )
{
    delete_message_request_data_type    request;

    memset( &request, 0, sizeof( request ) );
    request.id_size = aMessageId.getIdSize();
    memcpy( request.id, aMessageId.getId(), request.id_size );

    txFmi( FMI_ID_TEXT_MSG_DELETE_REQUEST, (uint8*)&request, sizeof( request ) );
}
#endif

//----------------------------------------------------------------------
//! \brief Calculate and format a 2D velocity given the north and east
//!     components
//! \details Formats into a human-readable speed and direction in m/s,
//!     and updates the member variable mHorizontalVelocity.
//! \param aNorthVelocity The north velocity in m/s; negative values
//!     represent south
//! \param aEasstVelocity The east velocity in m/s; negative values
//!     represent west
//----------------------------------------------------------------------
void FmiApplicationLayer::calculate2DVelocity
    (
    float32 aNorthVelocity,
    float32 aEasstVelocity
    )
{
    double speed;
    char   direction[3];

    speed = UTIL_calc_2d_speed( aNorthVelocity, aEasstVelocity );
    UTIL_calc_2d_direction( aNorthVelocity, aEasstVelocity, direction );

    sprintf( mHorizontalVelocity, "%05.3f m/s %s", speed, direction );
}

//----------------------------------------------------------------------
//! \brief Transmit Garmin command packet (packet ID = ID_COMMAND_BYTE)
//! \param aCommandId The command ID to send to the client.
//----------------------------------------------------------------------
void FmiApplicationLayer::txCommand
    (
    command_type    const aCommandId
    )
{
    ASSERT( mTransportLayer != NULL );
    mTransportLayer->tx( new GarminPacket( ID_COMMAND_BYTE, (uint8 *)&aCommandId, sizeof( aCommandId ), this ), FALSE );
}   /* txCommand() */

#if( FMI_SUPPORT_A602 )
//----------------------------------------------------------------------
//! \brief Transmit FMI packet (packet ID = ID_FMI_BYTE)
//! \details This procedure frames and transmits a FMI specific packet.
//! \param aFmiPacketId The FMI packet ID
//! \param aData The FMI payload.  This may be NULL if aSize is 0.
//! \param aSize The number of bytes in the FMI payload.
//! \param aSendNow If TRUE, place this packet at the head of the
//!     tx_queue and send it immediately; otherwise add it to the end
//!     of the queue and send it after any other packets that are
//!     outstanding.
//! \since Protocol A602
//----------------------------------------------------------------------
void FmiApplicationLayer::txFmi
    (
    fmi_id_type                  const aFmiPacketId,
    uint8                const * const aData,
    uint8                        const aSize,
    bool                         const aSendNow
    )
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    fmi_id_type     fmiPacketId;
    uint8           buffer[ MAX_PAYLOAD_SIZE ];


    fmiPacketId = aFmiPacketId;
    memcpy( &buffer[ 0 ], &fmiPacketId, sizeof( fmiPacketId ) );
    memcpy( &buffer[ sizeof( fmiPacketId ) ], aData, aSize );

    ASSERT( mTransportLayer != NULL );
    mTransportLayer->tx( new GarminPacket( ID_FMI_PACKET, buffer, aSize + sizeof( fmiPacketId ), this ), aSendNow );
}   /* txFmi() */
#endif // FMI_SUPPORT_A602

#if( FMI_SUPPORT_A603 )
//----------------------------------------------------------------------
//! \brief Clear the stop list on the server.
//! \since Protocol A603
//----------------------------------------------------------------------
void FmiApplicationLayer::resetSavedStops()
{
    mA603Stops.clear();
    mStopIndexInList.clear();
}
#endif

#if( FMI_SUPPORT_A607 )
//----------------------------------------------------------------------
//! \brief Clear the waypoint and waypoint category lists on the
//!     server.
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::resetWaypoints()
{
    mWaypoints.clear();
    mCategories.clear();

    Event::post( EVENT_FMI_WAYPOINT_LIST_CHANGED );
    Event::post( EVENT_FMI_CATEGORY_LIST_CHANGED );
}
#endif

#if( FMI_SUPPORT_A604 )
//----------------------------------------------------------------------
//! \brief Clear the canned messages on the server.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::resetCannedMessages()
{
    mCannedMessages.clear();

    Event::post( EVENT_FMI_CANNED_MSG_LIST_CHANGED );
}

//----------------------------------------------------------------------
//! \brief Clear the canned response text messages on the server.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::resetCannedResponseMessages()
{
    mSentCannedResponseMessages.clear();
}

//----------------------------------------------------------------------
//! \brief Clear the canned responses on the server.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::resetCannedResponses()
{
    mCannedResponses.clear();

    Event::post( EVENT_FMI_CANNED_RESP_LIST_CHANGED );
}

//----------------------------------------------------------------------
//! \brief Clear the driver statuses on the server.
//! \since Protocol A604
//----------------------------------------------------------------------
void FmiApplicationLayer::resetDriverStatusList()
{
    mDriverStatuses.clear();

    Event::post( EVENT_FMI_DRIVER_STATUS_LIST_CHANGED );
}
#endif

#if FMI_SUPPORT_A607
//----------------------------------------------------------------------
//! \brief Send an A607 Driver ID Update to the client
//! \param aDriverId The new driver ID, in the client's code page
//! \param aIndex The driver index
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA607DriverIdUpdate
    (
    char  * aDriverId,
    uint8   aIndex
    )
{
    uint8     payloadSize;
    time_type originationTime = UTIL_get_current_garmin_time();

    strcpy( mSentDriverId, aDriverId );

    driver_id_D607_data_type data;
    memset( &data, 0, sizeof( data ) );

    //need to allow app to actually set this field
    data.status_change_id = originationTime;
    data.status_change_time = originationTime;
    data.driver_idx = aIndex;
    strcpy( data.driver_id, aDriverId );
    data.driver_id[49] = '\0';
    payloadSize = offset_of( driver_id_D607_data_type, driver_id ) + (uint8)strlen( data.driver_id ) + 1;

    txFmi( FMI_ID_DRIVER_ID_UPDATE_D607, (uint8*)&data, payloadSize );
}

//----------------------------------------------------------------------
//! \brief Initiate the A607 Driver Status Update protocol
//! \param aDriverStatusId The new driver status.
//! \param aIndex The driver index.
//! \note The driver_status specified must already have been sent to
//!     the client using the Set Driver Status List Item protocol.
//! \since Protocol A607
//----------------------------------------------------------------------
void FmiApplicationLayer::sendA607DriverStatusUpdate
    (
    uint32 aDriverStatusId,
    uint8  aIndex
    )
{
    time_type originationTime = UTIL_get_current_garmin_time();
    mSentDriverStatus = aDriverStatusId;

    driver_status_D607_data_type status;
    memset( &status, 0, sizeof( status ) );

    //need to make this an input
    status.status_change_id = originationTime;

    status.status_change_time = originationTime;
    status.driver_status = aDriverStatusId;
    status.driver_idx = aIndex;

    txFmi( FMI_ID_DRIVER_STATUS_UPDATE_D607, (uint8*)&status, sizeof( status ) );
}
#endif

#if( FMI_SUPPORT_A604 )
//----------------------------------------------------------------------
//! \brief Process a Driver Status Update from the client
//! \param aDriverStatusId The new driver status.
//! \param aDriverIndex The driver index; must be zero if A607 support
//!     is not enabled.
//! \since Protocol A604
//----------------------------------------------------------------------
boolean FmiApplicationLayer::procDriverStatusUpdate
    (
    uint32 aDriverStatusId,
    uint8  aDriverIndex
    )
{
    boolean result = FALSE;

    if( aDriverIndex >= FMI_DRIVER_COUNT )
    {
        return FALSE;
    }

    if( aDriverStatusId == INVALID32 )
    {
        strcpy( mDriverStatus[aDriverIndex], "status not set" );
        result = TRUE;
    }
    else if( mDriverStatuses.contains( aDriverStatusId ) )
    {
        ClientListItem& item = mDriverStatuses.get( aDriverStatusId );
        if( item.isValid() )
        {
            WideCharToMultiByte( mClientCodepage, 0, item.getCurrentName().GetBuffer(), -1, mDriverStatus[aDriverIndex], 50, NULL, NULL );
            mDriverStatus[aDriverIndex][49] = '\0';
            result = TRUE;
        }
    }

    if( result )
    {
        Event::post( EVENT_FMI_DRIVER_STATUS_CHANGED, aDriverIndex );
    }
    return result;
}
#endif
