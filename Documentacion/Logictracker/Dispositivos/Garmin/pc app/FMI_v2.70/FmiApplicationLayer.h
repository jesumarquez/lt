/*********************************************************************
*
*   HEADER NAME:
*       FmiApplicationLayer.h
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef FmiApplicationLayer_H
#define FmiApplicationLayer_H

class FmiApplicationLayer;

#include <map>
#include <vector>
#include <list>

#include "fmi.h"

#include "ApplicationLayer.h"

#if( FMI_SUPPORT_A602 )
#include "MessageId.h"
#endif

#if( FMI_SUPPORT_A603 )
#include "ClientListItem.h"
#include "StopListItem.h"
#include "FileBackedMap.h"
#endif

#if( FMI_SUPPORT_A604 )
#include "InboxListItem.h"
#endif

#if( FMI_SUPPORT_A607 )
#include "WaypointListItem.h"
#include "DriverLoginItem.h"
#endif

//----------------------------------------------------------------------
//! \brief Serial communication controller for Garmin and FMI packets
//! \details The FmiApplicationLayer class performs the application-
//!     level communication with the client device.
//! \note This implementation makes the assumption that the client
//!     correctly follows the protocols.  If the client follows the
//!     link-layer protocol but does not send the application-level
//!     response, the protocol will not continue.  For example, during
//!     a GPI file transfer, a file data packet is not sent until the
//!     receipt is received for the previous packet; if the previous
//!     packet is ACKed but a receipt is not received, the file
//!     transfer will stall.  A robust implementation should include
//!     appropriate timeouts at the application layer to account for
//!     this possibility.
//----------------------------------------------------------------------
class FmiApplicationLayer : public ApplicationLayer
{
    //
    // TYPES
    //
public:

#if( FMI_SUPPORT_A604 )
    //! Enum to track GPI transfer status.
    //! \since Protocol A604
    enum transfer_state
    {
        TRANSFER_NOT_STARTED,
        TRANSFER_STARTED,
        TRANSFER_IN_PROGRESS,
        TRANSFER_COMPLETED,
        TRANSFER_FAILED
    };
#endif

    //
    // METHODS
    //
    public:
        FmiApplicationLayer();

        virtual ~FmiApplicationLayer();

        virtual bool rx
            (
            const Packet * aPacket
            );

        virtual void onAck
            (
            const Packet * aPacket
            );

        void sendEnablePvtCommand
            (
            bool aEnable
            );

        void sendUnitIdRequest();

        void clearError();

#if( FMI_SUPPORT_LEGACY )
        void sendLegacyProductRequest();

        void sendLegacyStop
            (
            double aLatitude,
            double aLongitude,
            char * aStopName
            );

        void sendLegacyTextMessage
            (
            char * aMessageText
            );
#endif

#if( FMI_SUPPORT_A602 )
        void sendA602Stop
            (
            double aLatitude,
            double aLongitude,
            char * aStopName
            );

        void sendA602TextMessage
            (
            fmi_id_type       aFmiPacketId,
            char            * aMessageText,
            const MessageId & aMessageId
            );

        void sendEnable
            (
            uint16 *aFeatureCodes = NULL,
            uint8 aFeatureCount = 0
            );

        void sendFmiPacket
            (
            uint16   aFmiPacketId,
            uint8  * aFmiPayload,
            uint8    aFmiPayloadSize
            );

        void sendProductRequest();
#endif

#if( FMI_SUPPORT_A603 )
        uint32 getNextStopId();

        void sendA603Stop
            (
            double   aLatitude,
            double   aLongitude,
            char   * aStopName,
            uint32   aStopId
            );

        void sendStopStatusRequest
            (
            uint32                  aStopId,
            stop_status_status_type aStopStatus,
            uint16                  aMoveToIndex = INVALID16
            );

        void sendStopMoveRequest
            (
            uint16 aMoveToIndex,
            uint16 aMoveFromIndex
            );

        void removeStopByIndex
            (
            uint16 aDeletedStopIndex
            );

        void sendAutoArrival
            (
            uint32 aArrivalTime,
            uint32 aArrivalDistance
            );

        void sendEtaRequest();

        void sendDataDeletionRequest
            (
            del_data aDataType
            );
#endif

#if( FMI_SUPPORT_A604 )
        void sendA604TextMessage
            (
            const char      * aMessageText,
            const MessageId & aMessageId,
            uint8             aMessageType   = A604_MESSAGE_TYPE_NORMAL
            );
        void sendCannedResponseTextMessage
            (
            const char   * aMessageText,
            const MessageId & aMessageId,
            uint8          aResponseCount,
            const uint32 * aResponseList,
            uint8          aMessageType
            );

        void sendGpiFile
            (
            char  * aFilename,
            uint8   aVersionLength,
            uint8 * aVersion
            );

        void stopGpiFileTransfer();

        void sendCannedResponse
            (
            uint32    aResponseId,
            CString   aResponseText
            );

        void sendDeleteCannedResponseRequest
            (
            uint32    aResponseId
            );

        void sendCannedMessage
            (
            uint32  aMessageId,
            CString aMessageText
            );

        void sendDeleteCannedMessageRequest
            (
            uint32 aMessageId
            );

        void sendPing();

        void sendGpiFileInfoRequest();

        void sendDriverIdUpdate
            (
            char * aDriverId
            );

        void sendDriverIdRequest();

        void sendDriverStatusListItem
            (
            uint32    aDriverStatusId,
            CString   aDriverStatusText
            );

        void sendDeleteDriverStatusListItem
            (
            uint32    aDriverStatusId
            );

        void sendDriverStatusUpdate
            (
            uint32    aDriverStatusId
            );

        void sendDriverStatusRequest();

        void sendTextMessageStatusRequest
            (
            const MessageId & aMessageId
            );

        void sendStopSortRequest();

        void sendUserInterfaceText
            (
            uint32 aElementId,
            char * aText
            );

        void sendMessageThrottlingUpdate
            (
            uint16 aPacketId,
            uint16 aNewState
            );
#endif

#if( FMI_SUPPORT_A605 )
        void sendMessageThrottlingQuery();
#endif

#if (FMI_SUPPORT_A606)
        void sendFmiSafeModeSpeed
            (
            float speed
            );
#endif

#if( FMI_SUPPORT_A607 )
        void sendA607DriverIdRequest
            (
            uint8 aDriverIndex
            );

        void sendA607DriverStatusRequest
            (
            uint8 aDriverIndex
            );

        void sendA607DriverIdUpdate
            (
            char *    aDriverId,
            uint8     aIndex
            );

        void sendA607DriverStatusUpdate
            (
            uint32 aDriverStatusId,
            uint8  aIndex
            );

        void sendCreateWaypointCat
            (
            uint8 aCatId,
            CString &aCatName
            );
        void sendDeleteWaypoint
            (
            uint16 aUniqueId
            );

        void sendDeleteWaypointCat
            (
            uint16 aCatIdx
            );

        void sendWaypoint
            (
            uint16 aUniqueId,
            double aLat,
            double aLon,
            uint16 aSymbol,
            CString &aName,
            uint16 aCat,
            CString &aComment
            );

#endif

#if( FMI_SUPPORT_A607 )
        void sendMessageDeleteRequest
            (
            const MessageId & aMessageId
            );
#endif

#if( FMI_SUPPORT_A608 )
        void sendSetSpeedLimitAlerts
            (
            uint8   aMode,
            uint8   aTimeOver,
            uint8   aTimeUnder,
            boolean aAlertUser,
            float   aThreshold
            );
#endif

        //! Code page used for encoding of text fields when communicating
        //!     with the client
        codepage_type               mClientCodepage;

        //! If TRUE, the stop list has been initialized.
        bool                        mStopListInitialized;

        //! Null-terminated ASCII string containing the Unit ID (ESN) of the
        //!     client.
        char                        mClientUnitId[11];

        //! GPS fix type from the last PVT packet, interpreted as an
        //!     null-terminated ASCII string
        char                        mPVTFixType[9];

        //! Date from the last PVT packet, as an ASCII null-terminated
        //!     string of the form MM/DD/YYYY.
        char                        mPvtDate[11];

        //! Time from the last PVT packet
        //! \details This is an ASCII null-terminated string of the form
        //!     HH:MM:SS AA (AA is AM or PM) in the server's time zone
        char                        mPvtTime[13];

        //! Latitude in degrees from the last PVT packet, as an ASCII
        //!     null-terminated string.
        //! \details Latitude is formatted with six decimal places and
        //!     is suffixed by the degree symbol and N or S.
        char                        mPvtLatitude[14];

        //! Longitude in degrees from the last PVT packet, as an ASCII
        //!     null-terminated string.
        //! \details Longitude is formatted with six decimal places and
        //!     is suffixed by the degree symbol and E or W.
        char                        mPvtLongitude[14];

        //! Altitude in meters from the last PVT packet, as an ASCII
        //!     null-terminated string
        //! \details Altitude is formatted with three decimal places and
        //!     is relative to the WGS84 ellipsoid.
        char                        mPvtAltitude[13];

        //! East-west velocity in meters per second from the last PVT
        //!     packet, as an ASCII null-terminated string
        //! \details String is formatted with three decimal places and
        //!     includes the unit "m/s" and direction (W or E)
        char                        mPvtEastWestVelocity[15];

        //! North-south velocity in meters per second from the last PVT
        //!     packet, as an ASCII null-terminated string
        //! \details String is formatted with three decimal places and
        //!     includes the unit "m/s" and direction (N or S)
        char                        mPvtNorthSouthVelocity[15];

        //! Up-down velocity in meters per second from the last PVT
        //!     packet, as an ASCII null-terminated string
        //! \details String is formatted with three decimal places and
        //!     includes the unit "m/s" and direction (U or D)
        char                        mPvtUpDownVelocity[15];

        //! 2-D velocity in the horizontal plane in meters per second from
        //!     the last PVT packet, as an ASCII null-terminated string
        //! \details String is formatted with three decimal places and
        //!     includes the unit "m/s" and heading as the nearest cardinal
        //!     (N, E, S, W) or inter-cardinal direction (NE, SE, NW, SW)
        char                        mHorizontalVelocity[15];

        //! Numeric product ID reported by the client.
        //! \see CFmiPcAppDlg::getProductName() for one way to translate
        //!     this into a human-readable description of the client.
        uint16                      mClientProductId;

        //! Raw software version reported by the client.
        //! \details This should be divided by 100 to obtain a formatted
        //!     representation.  For example, 312 means software version
        //!     3.12.
        sint16                      mClientSoftwareVersion;

        //! Null-terminated ASCII string containing the protocols supported
        //!     by the client.
        //! \details The list is space-delimited, and each protocol is in
        //!     the format Xnnn where X is the protocol type (A/D/L/T) and
        //!     nnn is the number (602, etc.)
        char                        mProtocols[PROTOCOL_SIZE];

#if( FMI_SUPPORT_A602 )
        //! If true, the FMI Enable protocol is in progress.
        bool                        mEnablePending;
#endif

#if( FMI_SUPPORT_A603 )
        //! String containing the ETA time from the client, or "invalid"
        //! \details This is a null-terminated ASCII string of the
        //!     form "HH:MM:SS AA" (hours, minutes, seconds, AM/PM,
        //!     respectively), or the ASCII string "invalid" if the ETA
        //!     time or distance received from the client is invalid.
        //! \since Protocol A603
        char                        mEtaTime[13];
        //! Null-terminated ASCII string containing the distance to the
        //!     current destination, including units
        //! \details This will be in meters if the distance is less than 1
        //!     km, otherwise the units are km.  If either the ETA time or
        //!     ETA distance received from the client is invalid, this
        //!     will be an empty string.
        //! \since Protocol A603
        char                        mEtaDistance[13];
        //! String representing the latitude in decimal degrees (and N or S)
        //! \details Null-terminated ASCII string containing the latitude
        //!     of the current destination in positive decimal degrees,
        //!     followed by a direction suffix (N or S).  This information
        //!     is current as of the last ETA packet received.  If the last
        //!     ETA indicated that the ETA time or distance were invalid,
        //!     this will be an empty string.
        //! \since Protocol A603
        char                        mEtaLatitude[14];
        //! String representing the longitude in decimal degrees (and E or W)
        //! \details Null-terminated ASCII string containing the longitude
        //!     of the current destination in positive decimal degrees,
        //!     followed by a direction suffix (E or W).  This information
        //!     is current as of the last ETA packet received.  If the last
        //!     ETA indicated that the ETA time or distance were invalid,
        //!     this will be an empty string.
        //! \since Protocol A603
        char                        mEtaLongitude[14];

        //! Unique ID of the active stop
        //! \since Protocol A603
        uint32                      mActiveStopId;

        //! Vector relating the stop list to unique_ids
        //! \details mStopIndexInList[n] contains the unique_id of the nth
        //!     stop in the list. n=0 is the first stop in the list.
        //! \since Protocol A603
        std::vector<uint32>         mStopIndexInList;

        //! If TRUE, the client has a stop active.
        //! \details This is derived from the ETA data so that it is correct
        //!     for all stop types.
        //! \since Protocol A603
        bool                        mActiveRoute;

        //! Map of unique ID to stop details for all A603 stops on the client
        //! \since Protocol A603
        FileBackedMap<StopListItem>  mA603Stops;
#endif

#if( FMI_SUPPORT_A604 )
        //! \brief Last driver ID received from the client
        //! \since Protocol A604
        char                         mDriverId[FMI_DRIVER_COUNT][50];
        //! \brief Last driver status received from the client
        //! \since Protocol A604
        char                         mDriverStatus[FMI_DRIVER_COUNT][50];

        //! \brief GPI file version (FMI_GPI_FILE_INFORMATION)
        //! \since Protocol A604
        uint8                        mGpiFileVersion[16];
        //! \brief Number of significant bytes in mGpiFileVersion
        //! \since Protocol A604
        uint8                        mGpiFileVersionLength;

        //! \brief GPI file size (FMI_GPI_FILE_INFORMATION)
        //! \since Protocol A604
        uint32                       mGpiFileSize;
        //! \brief Number of bytes of the GPI file that have been transferred so far
        //! \since Protocol A604
        uint32                       mGpiTransferBytesDone;
        //! \brief Total number of bytes in the GPI file being transferred
        //! \since Protocol A604
        uint32                       mGpiFileTransferSize;
        //! State of the current GPI file transfer (in progress, completed, etc.)
        //! \since Protocol A604
        transfer_state               mGpiTransferState;

        //! \brief Number of client-to-server pings received; can be reset by the user
        //! \since Protocol A604
        int                          mClientPingCount;
        //! \brief Number of server-to-client pings sent; can be reset by the user
        //! \since Protocol A604
        int                          mServerPingCount;

        //! Last time when a client to server ping was received
        //! \since Protocol A604
        CTime                        mLastClientPingTime;

        //! Last time when a server to client ping was received
        //! \since Protocol A604
        CTime                        mLastServerPingTime;

        //! Map of canned response IDs to names
        //! \since Protocol A604
        FileBackedMap<ClientListItem>   mCannedResponses;
        //! Map of canned message IDs to names
        //! \since Protocol A604
        FileBackedMap<ClientListItem>   mCannedMessages;
        //! Map of driver status IDs to names
        //! \since Protocol A604
        FileBackedMap<ClientListItem>   mDriverStatuses;
        //! List of message IDs for canned response text messages
        //! \details Used in the handler for the text message ack to
        //!     determine whether the ID corresponds to OK/Yes/No or
        //!     a canned response id.
        //! \since Protocol A604
        FileBackedMap<InboxListItem>    mSentCannedResponseMessages;

        //! Flag to indicate that the user wants to stop the GPI
        //!     file transfer
        //! \details Processed when the next GPI file data receipt
        //!     is received.
        //! \since Protocol A604
        bool                               mStopGpiTransfer;
#endif

#if( FMI_SUPPORT_A605 )
        //! Response from the last Message Throttling Query Protocol
        //! \since Protocol A605
        message_throttling_data_type mThrottledProtocols[60];
#endif

#if( FMI_SUPPORT_A607 )
        //! Map of waypoint category IDs to names
        FileBackedMap<ClientListItem> mCategories;

        //! Map of waypoint IDs to names
        FileBackedMap<WaypointListItem> mWaypoints;

        //! Map of allowed driver IDs and passwords
        FileBackedMap<DriverLoginItem> mDriverLogins;

        //! If true, multiple driver support is enabled
        bool mUseMultipleDrivers;

        //! If true, driver password support is enabled
        bool mUsePasswords;
#endif

    private:
        void txCommand
            (
            command_type const aCommandId
            );

        void calculate2DVelocity
            (
            float32 aNorthVelocity,
            float32 aEastVelocity
            );

#if( FMI_SUPPORT_A602 )
        void txFmi
            (
            fmi_id_type  const         aFmiPacketId,
            uint8        const * const aFmiPayload,
            uint8        const         aFmiPayloadSize,
            bool         const         aSendNow = false
            );
#endif
#if( FMI_SUPPORT_A603 )
        void removeStopById
            (
            uint32 aStopId
            );

        void resetSavedStops();
#endif

#if( FMI_SUPPORT_A604 )
        boolean procDriverStatusUpdate
            (
            uint32 aDriverStatusId,
            uint8  aDriverIndex = 0
            );
        void resetCannedResponseMessages();
        void resetDriverStatusList();
        void resetCannedMessages();
        void resetCannedResponses();
#endif

#if( FMI_SUPPORT_A607 )
        void resetWaypoints();
#endif

#if( FMI_SUPPORT_A604 )
        //! The last driver status that was sent to the client
        //! \since Protocol A604
        uint32                     mSentDriverStatus;
        //! The last driver ID that was sent to the client
        //! \details This is a null-terminated string in the client's
        //!     codepage.
        //! \since Protocol A604
        char                       mSentDriverId[50];

        //! Path to the GPI file that is being transferred to the client
        //! \since Protocol A604
        char                       mGpiFilePath[200];
        //! The last GPI packet that was sent
        //! \since Protocol A604
        gpi_file_packet_data_type  mLastGpiPacketSent;

        //! The message text for the server to client canned text message.
        //! \details Stored so that the server can send it in response to
        //!     the canned response list ack message.
        //! \since Protocol A604
        char                       mCannedResponseMessageBody[200];

        //! The message_type for the server to client canned text message.
        //! \details Stored so that the server can send it in response to
        //!     the canned response list ack message.
        //! \since Protocol A604
        uint8                      mCannedResponseMessageType;

        //! If TRUE, FmiApplicationLayer is handling a request to refresh
        //!     the driver status list
        //! \since Protocol A604
        bool                       mRefreshingDriverStatusList;
        //! If TRUE, FmiApplicationLayer is handling a request to refresh
        //!     the canned response list
        //! \since Protocol A604
        bool                       mRefreshingCannedResponses;
        //! If TRUE, FmiApplicationLayer is handling a request to refresh
        //!     the canned message list
        //! \since Protocol A604
        bool                       mRefreshingCannedMessages;
#endif

};

#endif
