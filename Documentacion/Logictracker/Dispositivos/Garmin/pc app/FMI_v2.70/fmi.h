/*********************************************************************
*
*   HEADER NAME:
*       fmi.h - Data structures, types, and constants specific to
*               the Fleet Management Interface Control Specification
*
* Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef FMI_H
#define FMI_H

/*--------------------------------------------------------------------
                           GENERAL INCLUDES
--------------------------------------------------------------------*/
#include "garmin_types.h"

/*--------------------------------------------------------------------
                           LITERAL CONSTANTS
--------------------------------------------------------------------*/
#ifndef FMI_PROTOCOL_LEVEL
#define FMI_PROTOCOL_LEVEL 608
#endif

//! If true, app was build with support for A602 protocols
#define FMI_SUPPORT_A602 ( FMI_PROTOCOL_LEVEL >= 602 )

//! If true, app was build with support for A603 protocols
#define FMI_SUPPORT_A603 ( FMI_PROTOCOL_LEVEL >= 603 )

//! If true, app was build with support for A604 protocols
#define FMI_SUPPORT_A604 ( FMI_PROTOCOL_LEVEL >= 604 )

//! If true, app was build with support for A605 protocols
#define FMI_SUPPORT_A605 ( FMI_PROTOCOL_LEVEL >= 605 )

//! If true, app was build with support for A606 protocols
#define FMI_SUPPORT_A606 ( FMI_PROTOCOL_LEVEL >= 606 )

//! If true, app was build with support for A607 protocols
#define FMI_SUPPORT_A607 ( FMI_PROTOCOL_LEVEL >= 607 )

//! If true, app was build with support for legacy protocols
#define FMI_SUPPORT_LEGACY ( !FMI_SUPPORT_A607 )

//! If true, app was build with support for A608 protocols
#define FMI_SUPPORT_A608 ( FMI_PROTOCOL_LEVEL >= 608 )

//! If TRUE, the server supports Unicode
//! \note This should normally be TRUE
#define UNICODE_ENABLED  ( TRUE )

//! If TRUE, no validation of inputs is performed in the UI.
//! \details Set this to FALSE for a server app that conforms to
//!    the FMI protocols.  Set to TRUE to be allowed to perform
//!    certain operations that violate the Fleet Management
//!    Interface specification.
#define SKIP_VALIDATION  ( FALSE )

//! If TRUE, the Enable is minimal
//! \details If TRUE, the initial FMI enable process only consists
//!     of the Enable FMI protocol.  PVT and auto-ETA are not
//!     enabled, the stop list is not refreshed, and the Unit ID/ESN
//!     Protocol and Product id and Support protocols are not
//!     performed.
//! \note This should normally be FALSE.  If setting this to TRUE
//!     to test a server that does not send any packets after the
//!     enable, also set UNICODE_ENABLED to FALSE.
#define MINIMAL_ENABLE   ( FALSE )

//! If TRUE, interpret the raw packet as ASCII
//! \details If TRUE, the log viewer shows the raw packet as ASCII
//!     as well as in hex; this may make it easier to understand a
//!     packet's content when it contains mostly text.  However,
//!     individual fields are already parsed, and packets are
//!     usually binary data, so this has limited use.  Setting this
//!     to TRUE may require the log viewer window to be resized
//!     to show all the data.
#define LOG_SHOW_RAW_ASCII ( TRUE )

#if FMI_SUPPORT_A605
//! \brief Maximum number of protocols that can be throttled.
//! \details This limit is derived from the size of the
//!     message_throttling_list_data_type
#define MAX_THROTTLED_PROTOCOLS 60
#endif

//! \brief Placeholder for an invalid 32-bit value
#define INVALID32            0xFFFFFFFF

//! \brief Placeholder for an invalid 16-bit value
#define INVALID16            0xFFFF

#if( FMI_SUPPORT_A607 )
//! \brief Number of supported drivers
//! \since Protocol A607
#define FMI_DRIVER_COUNT     ( 3 )
#elif( FMI_SUPPORT_A604 )
//! \brief Number of supported drivers
//! \since Protocol A604
#define FMI_DRIVER_COUNT     ( 1 )
#endif

//! \brief Size of the formatted "supported protocols" string
//! \details 5 characters are needed for each protocol_support_data_type,
//! "Xnnn ", plus 1 character is needed for the null terminator.
#define PROTOCOL_SIZE        ( MAX_PAYLOAD_SIZE / sizeof( protocol_support_data_type ) * 5 + 1 )

/*--------------------------------------------------------------------
                          FUNDAMENTAL TYPES
--------------------------------------------------------------------*/

/*--------------------------------------------------------------------
                           ENUMERATED TYPES
--------------------------------------------------------------------*/

//! The code page used for encoding of text fields sent to or received
//!     from the client.
//! \since Protocol A604 added UTF8 support.
enum codepage_type
    {
    CODEPAGE_ASCII   = 1252,
#if( FMI_SUPPORT_A604 )
    CODEPAGE_UNICODE = CP_UTF8
#endif
    };

//! Garmin packet ID
enum id_enum
    {
    ID_COMMAND_BYTE         =  10,
    ID_UNIT_ID              =  38,

    ID_DATE_TIME_DATA       =  14,
    ID_PVT_DATA             =  51,

#if( FMI_SUPPORT_LEGACY )
    ID_LEGACY_STOP_MSG      = 135,
    ID_LEGACY_TEXT_MSG      = 136,
#endif

#if( FMI_SUPPORT_A602 )
    ID_FMI_PACKET           = 161,
#endif

    ID_PROTOCOL_ARRAY       = 253,
    ID_PRODUCT_RQST         = 254,
    ID_PRODUCT_DATA         = 255
    }; /* id_type */

//! \brief Garmin command ID (payload when packet ID == ID_COMMAND_BYTE)
//! \see command_enum for supported values
typedef uint16 command_type;
//! Garmin command ID
enum command_enum
    {
    COMMAND_REQ_DATE_TIME       =  5,
    COMMAND_REQ_UNIT_ID         = 14,
    COMMAND_TURN_ON_PVT_DATA    = 49,
    COMMAND_TURN_OFF_PVT_DATA   = 50,
    }; /* command_type */

//! Fleet Management packet ID  (first two bytes of payload when Garmin packet ID == ID_FMI_PACKET)
//! \see fmi_packet_id_enum for supported values
typedef uint16 fmi_id_type;

//! Fleet Management packet ID
enum fmi_packet_id_enum
    {
#if( FMI_SUPPORT_A602 )
    FMI_ID_ENABLE                           = 0x0000,
    FMI_ID_PRODUCT_ID_SUPPORT_RQST          = 0x0001,
    FMI_ID_PRODUCT_ID_DATA                  = 0x0002,
    FMI_ID_PROTOCOL_DATA                    = 0x0003,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_UNICODE_REQUEST                  = 0x0004,
    FMI_ID_UNICODE_RESPONSE                 = 0x0005,
#endif

#if( FMI_SUPPORT_A602 )
    FMI_ID_TEXT_MSG_ACK                     = 0x0020,
    FMI_ID_SERVER_OPEN_TXT_MSG              = 0x0021,
    FMI_ID_SERVER_OK_ACK_TXT_MSG            = 0x0022,
    FMI_ID_SERVER_YES_NO_CONFIRM_MSG        = 0x0023,
#endif

#if( FMI_SUPPORT_A603 )
    FMI_ID_CLIENT_OPEN_TXT_MSG              = 0x0024,
    FMI_ID_CLIENT_TXT_MSG_RCPT              = 0x0025,
#endif

#if( FMI_SUPPORT_A607 )
    FMI_ID_A607_CLIENT_OPEN_TXT_MSG         = 0x0026,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_SET_CANNED_RESP_LIST             = 0x0028,
    FMI_ID_CANNED_RESP_LIST_RCPT            = 0x0029,
    FMI_ID_A604_OPEN_TEXT_MSG               = 0x002A,
    FMI_ID_A604_OPEN_TEXT_MSG_RCPT          = 0x002B,
    FMI_ID_TEXT_MSG_ACK_RCPT                = 0x002C,
#endif

#if( FMI_SUPPORT_A607 )
    FMI_ID_TEXT_MSG_DELETE_REQUEST          = 0x002D,
    FMI_ID_TEXT_MSG_DELETE_RESPONSE         = 0x002E,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_SET_CANNED_RESPONSE              = 0x0030,
    FMI_ID_DELETE_CANNED_RESPONSE           = 0x0031,
    FMI_ID_SET_CANNED_RESPONSE_RCPT         = 0x0032,
    FMI_ID_DELETE_CANNED_RESPONSE_RCPT      = 0x0033,
    FMI_ID_REFRESH_CANNED_RESP_LIST         = 0x0034,

    FMI_ID_TEXT_MSG_STATUS_REQUEST          = 0x0040,
    FMI_ID_TEXT_MSG_STATUS                  = 0x0041,

    FMI_ID_SET_CANNED_MSG                   = 0x0050,
    FMI_ID_SET_CANNED_MSG_RCPT              = 0x0051,
    FMI_ID_DELETE_CANNED_MSG                = 0x0052,
    FMI_ID_DELETE_CANNED_MSG_RCPT           = 0x0053,
    FMI_ID_REFRESH_CANNED_MSG_LIST          = 0x0054,
#endif

#if( FMI_SUPPORT_A602 )
    FMI_ID_A602_STOP                        = 0x0100,
#endif

#if( FMI_SUPPORT_A603 )
    FMI_ID_A603_STOP                        = 0x0101,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_SORT_STOP_LIST                   = 0x0110,
    FMI_ID_SORT_STOP_LIST_ACK               = 0x0111,
#endif

#if( FMI_SUPPORT_A607 )
    FMI_ID_WAYPOINT                         = 0x0130,
    FMI_ID_WAYPOINT_RCPT                    = 0x0131,
    FMI_ID_WAYPOINT_DELETE                  = 0x0132,
    FMI_ID_WAYPOINT_DELETED                 = 0x0133,
    FMI_ID_WAYPOINT_DELETED_RCPT            = 0x0134,
    FMI_ID_DELETE_WAYPOINT_CAT              = 0x0135,
    FMI_ID_DELETE_WAYPOINT_CAT_RCPT         = 0x0136,
    FMI_ID_CREATE_WAYPOINT_CAT              = 0x0137,
    FMI_ID_CREATE_WAYPOINT_CAT_RCPT         = 0x0138,
#endif

#if( FMI_SUPPORT_A603 )
    FMI_ID_ETA_DATA_REQUEST                 = 0x0200,
    FMI_ID_ETA_DATA                         = 0x0201,
    FMI_ID_ETA_DATA_RCPT                    = 0x0202,

    FMI_ID_STOP_STATUS_REQUEST              = 0x0210,
    FMI_ID_STOP_STATUS                      = 0x0211,
    FMI_ID_STOP_STATUS_RCPT                 = 0x0212,

    FMI_ID_AUTO_ARRIVAL                     = 0x0220,
    FMI_ID_DATA_DELETION                    = 0x0230,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_USER_INTERFACE_TEXT              = 0x0240,
    FMI_ID_USER_INTERFACE_TEXT_RCPT         = 0x0241,

    FMI_ID_MSG_THROTTLING_COMMAND           = 0x0250,
    FMI_ID_MSG_THROTTLING_RESPONSE          = 0x0251,
#endif

#if( FMI_SUPPORT_A605 )
    FMI_ID_MSG_THROTTLING_QUERY             = 0x0252,
    FMI_ID_MSG_THROTTLING_QUERY_RESPONSE    = 0x0253,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_PING                             = 0x0260,
    FMI_ID_PING_RESPONSE                    = 0x0261,

    FMI_ID_GPI_FILE_TRANSFER_START          = 0x0400,
    FMI_ID_GPI_FILE_DATA_PACKET             = 0x0401,
    FMI_ID_GPI_FILE_TRANSFER_END            = 0x0402,
    FMI_ID_GPI_FILE_START_RCPT              = 0x0403,
    FMI_ID_GPI_PACKET_RCPT                  = 0x0404,
    FMI_ID_GPI_FILE_END_RCPT                = 0x0405,
    FMI_ID_GPI_FILE_INFORMATION_REQUEST     = 0x0406,
    FMI_ID_GPI_FILE_INFORMATION             = 0x0407,

    FMI_ID_SET_DRIVER_STATUS_LIST_ITEM      = 0x0800,
    FMI_ID_DELETE_DRIVER_STATUS_LIST_ITEM   = 0x0801,
    FMI_ID_SET_DRIVER_STATUS_LIST_ITEM_RCPT = 0x0802,
    FMI_ID_DEL_DRIVER_STATUS_LIST_ITEM_RCPT = 0x0803,
    FMI_ID_DRIVER_STATUS_LIST_REFRESH       = 0x0804,
    FMI_ID_DRIVER_ID_REQUEST                = 0x0810,
    FMI_ID_DRIVER_ID_UPDATE                 = 0x0811,
    FMI_ID_DRIVER_ID_RCPT                   = 0x0812,
#endif

#if( FMI_SUPPORT_A607 )
    FMI_ID_DRIVER_ID_UPDATE_D607            = 0x0813,
#endif

#if( FMI_SUPPORT_A604 )
    FMI_ID_DRIVER_STATUS_REQUEST            = 0x0820,
    FMI_ID_DRIVER_STATUS_UPDATE             = 0x0821,
    FMI_ID_DRIVER_STATUS_RCPT               = 0x0822,
#endif

#if( FMI_SUPPORT_A607 )
    FMI_ID_DRIVER_STATUS_UPDATE_D607        = 0x0823,
#endif

#if ( FMI_SUPPORT_A606 )
    FMI_SAFE_MODE                           = 0x0900,
    FMI_SAFE_MODE_RESP                      = 0x0901,
#endif

#if ( FMI_SUPPORT_A608 )
    FMI_SPEED_LIMIT_SET                     = 0X1000,
    FMI_SPEED_LIMIT_RCPT                    = 0X1001,
    FMI_SPEED_LIMIT_ALERT                   = 0X1002,
    FMI_SPEED_LIMIT_ALERT_RCPT              = 0X1003,
#endif

    FMI_ID_END = 0xFFFF
    }; /* fmi_id_type */

#if( FMI_SUPPORT_A608 )
//! Enum for speed limit alert category
//! \since Protocol A608
enum speed_limit_alert_category_type
    {
    SPEED_LIMIT_ALERT_BEGIN     = 0,
    SPEED_LIMIT_ALERT_CHANGE    = 1,
    SPEED_LIMIT_ALERT_END       = 2,
    SPEED_LIMIT_ALERT_ERROR     = 3,
    SPEED_LIMIT_ALERT_INVALID   = 4
    };

//! Enum for speed limit alert result
//! \since Protocol A608
enum speed_limit_alert_result_data_type
    {
    SPEED_LIMIT_RESULT_SUCCESS          = 0,
    SPEED_LIMIT_RESULT_ERROR            = 1,
    SPEED_LIMIT_RESULT_MODE_UNSUPPORTED = 2
    };

//! Enum for speed limit alert mode
//! \since Protocol A608
enum speed_limit_alert_mode_type
    {
    SPEED_LIMIT_MODE_CAR                = 0,
    SPEED_LIMIT_MODE_OFF                = 1,
    SPEED_LIMIT_MODE_TRUCK              = 2,

    SPEED_LIMIT_MODE_CNT
    };
#endif

#if( FMI_SUPPORT_A607 )
//! Valid values for the features field of the fmi_features_data_type
//! \since Protocol A607
enum fmi_feature_type
{
    FEATURE_ID_UNICODE               = 1,
    FEATURE_ID_A607_SUPPORT          = 2,
    FEATURE_ID_DRIVER_PASSWORDS      = 10,
    FEATURE_ID_MULTIPLE_DRIVERS      = 11,

    FEATURE_STATE_ENABLED            = 1 << 15,
    FEATURE_STATE_DISABLED           = 0 << 15,

    FEATURE_ID_MASK                  = setbits(  0, 15 ),
    FEATURE_STATE_MASK               = setbits( 15,  1 )
};
#endif

#if( FMI_SUPPORT_A604 )
//! Valid values for the message_type field of the
//! A604_server_to_client_open_text_msg_data_type
//! \since Protocol A604
enum a604_message_type
{
    A604_MESSAGE_TYPE_NORMAL          = 0,
    A604_MESSAGE_TYPE_DISP_IMMEDIATE  = 1
};

//! Enum for A604 message status protocol.
//! \since Protocol A604
enum fmi_A604_message_status
{
    MESSAGE_STATUS_UNREAD      = 0,
    MESSAGE_STATUS_READ        = 1,
    MESSAGE_STATUS_NOT_FOUND   = 2
};

//! Enumeration for result_code from the canned_response_list packet.
//! \since Protocol A604
enum canned_response_list_result
{
    CANNED_RESP_LIST_SUCCESS            = 0,
    CANNED_RESP_LIST_INVALID_COUNT      = 1,
    CANNED_RESP_LIST_INVALID_MSG_ID     = 2,
    CANNED_RESP_LIST_DUPLICATE_MSG_ID   = 3,
    CANNED_RESP_LIST_FULL               = 4
};

//! Type for new_state from the message_throttling_data_type
//! \since Protocol A604
typedef uint16 message_throttling_state_type;

//! Enumeration for new_state from the message_throttling_data_type
//! \since Protocol A604
enum message_throttling_state_enum
{
    MESSAGE_THROTTLE_STATE_DISABLE = 0,
    MESSAGE_THROTTLE_STATE_ENABLE  = 1,
    MESSAGE_THROTTLE_STATE_ERROR   = 4095
};

#endif

#if( FMI_SUPPORT_A602 )
//! Enumeration for A602 ack text message responses.
//! \since Protocol A602
enum txt_ack_type
{
    OK_ACK       = 0,
    YES_ACK      = 1,
    NO_ACK       = 2
};
#endif

#if( FMI_SUPPORT_A603 )
//! Enumeration for Stop Status protocol.
//! \since Protocol A603
typedef uint16 stop_status_status_type;
enum stop_status_status_enum
{
    INVALID_STOP_STATUS      = INVALID16,

    REQUEST_STOP_STATUS      = 0,
    REQUEST_MARK_STOP_DONE   = 1,
    REQUEST_ACTIVATE_STOP    = 2,
    REQUEST_DELETE_STOP      = 3,
    REQUEST_MOVE_STOP        = 4,

    STOP_STATUS_ACTIVE       = 100,
    STOP_STATUS_DONE         = 101,
    STOP_STATUS_UNREAD       = 102,
    STOP_STATUS_READ         = 103,
    STOP_STATUS_DELETED      = 104
};

//! Enumeration for Data Deletion protocol
//! \since Protocol A603
enum del_data
{
    DELETE_ALL_STOPS             = 0,
    DELETE_ALL_MESSAGES          = 1,
#if( FMI_SUPPORT_A604 )
    DELETE_ACTIVE_ROUTE          = 2,
    DELETE_CANNED_MESSAGES       = 3,
    DELETE_CANNED_RESPONSES      = 4,
    DELETE_GPI_FILE              = 5,
    DELETE_DRIVER_ID_AND_STATUS  = 6,
    DISABLE_FMI                  = 7,
#endif
#if( FMI_SUPPORT_A607 )
    DELETE_WAYPOINTS             = 8
#endif
};
#endif     // end of #if( FMI_SUPPORT_A603 )

#include "pack_begin.h"
//! Payload for Garmin ID_PVT_DATA packet
__packed struct pvt_data_type
    {
    float32                 altitude;               //!< Altitude above the WGS84 ellipsoid, in meters.
    float32                 epe;                    //!< Estimated position error, 2 sigma, in meters.
    float32                 eph;                    //!< Estimated horizontal position error, 2 sigma, in meters.
    float32                 epv;                    //!< Estimated vertical position error, 2 sigma, in meters.
    uint16                  type_of_gps_fix;        //!< Enum for type of GPS fix, see gps_fix_type
    float64                 time_of_week;           //!< Seconds since Sunday 12:00 AM (excludes leap seconds)
    double_position_type    position;               //!< Current position of the client
    float32                 east_velocity;          //!< East velocity in m/s, negative is west
    float32                 north_velocity;         //!< North velocity in m/s, negative is south
    float32                 up_velocity;            //!< Up velocity in m/s, negative is down
    float32                 mean_sea_level_height;  //!< Height of WGS84 ellipsoid above MSL at current position, in meters
    sint16                  leap_seconds;           //!< Number of leap seconds as of the current time
    uint32                  week_number_days;       //!< Days from UTC December 31st, 1989 to beginning of current week
    };

//! Possible values for pvt_data_type.type_of_gps_fix
enum gps_fix_type
{
    GPS_FIX_UNUSABLE = 0,
    GPS_FIX_INVALID  = 1,
    GPS_FIX_2D       = 2,
    GPS_FIX_3D       = 3,
    GPS_FIX_2D_DIFF  = 4,
    GPS_FIX_3D_DIFF  = 5
};

//! Payload for Garmin ID_UNIT_ID packet
__packed struct unit_id_data_type /* Garmin */
    {
    uint32                    unit_id;     //!< Unit ID (ESN) of the client
    };

//! Payload for Garmin ID_PRODUCT_DATA (A000)
//! and FMI FMI_ID_PRODUCT_ID_DATA (A602) packet
__packed struct product_id_data_type
    {
    uint16                  product_id;          //!< Product ID of the client
    sint16                  software_version;    //!< Software version * 100 (312 means version 3.12)
    };

//! \brief Element of the array returned in Garmin ID_PROTOCOL_ARRAY (A001)
//! or FMI_ID_PROTOCOL_DATA (A602) packets.
__packed struct protocol_support_data_type
    {
    char                    tag;                  //!< Type of protocol (e.g., 'A', D')
    sint16                  data;                 //!< Protocol number
    };

//! \brief Payload for FMI_ID_ENABLE
__packed struct fmi_features_data_type
    {
    uint8                   feature_count;       //!< Number of feature IDs in features[]
    uint8                   reserved;            //!< Set to 0
    uint16                  features[ 126 ];     //!< Array of feature IDs
    };

#if( FMI_SUPPORT_A602 )
//! \brief Payload of FMI_ID_SERVER_OPEN_TXT_MSG packet
//! \since Protocol A602
__packed struct A602_server_to_client_open_text_msg_data_type
    {
    time_type           origination_time;    //!< Time when the client sent the message
    char                text_message[ 200 ]; //!< The message text (variable length, null terminated, 200 bytes max)
    };

//! \brief Payload of server to client messages requiring a response (A602)
//! \details Payload of FMI_ID_SERVER_OK_ACK_TXT_MSG and FMI_ID_SERVER_YES_NO_CONFIRM_MSG packets.
//! \since Protocol A602
__packed struct server_to_client_ack_text_msg_data_type
    {
    time_type            origination_time;        //!< Origination time of the message
    uint8                id_size;                 //!< Number of significant bytes of the message ID
    uint8                reserved[3];             //!< set to 0
    uint8                id[ 16 ];                //!< message ID
    char                 text_message[ 200 ];     //!< Text message (variable length, null-terminated string, 200 bytes max)
    };

//! \brief Payload of FMI_ID_TEXT_MSG_ACK packet
//! \since Protocol A602
__packed struct text_msg_ack_data_type /* D602 */
    {
    time_type            origination_time;        //!< Origination time of the response
    uint8                id_size;                 //!< Number of significant bytes of the message ID
    uint8                reserved[3];             //!< set to 0
    uint8                id[ 16 ];                //!< message ID
    uint32               msg_ack_type;            //!< The response selected by the user
    };
#endif //FMI_SUPPORT_A602)

#if( FMI_SUPPORT_A603 )
//! \brief Payload of FMI_ID_CLIENT_OPEN_TXT_MSG packet
//! \since Protocol A603
__packed struct client_to_server_open_text_msg_data_type /* D603 */
    {
    time_type           origination_time;         //!< Time when the message was sent by the client
    uint32              unique_id;                //!< Unique ID generated by client
    char                text_message[ 200 ];      //!< Message text (variable length, null-terminated string)
    };

//! \brief Payload of FMI_ID_CLIENT_TXT_MSG_RCPT packet
//! \since Protocol A603
__packed struct client_to_server_text_msg_receipt_data_type /* D603 */
    {
    uint32 unique_id;                              //!< unique_id from client_to_server_open_text_msg_data_type
    };
#endif

#if( FMI_SUPPORT_A607 )
//! \brief Payload of FMI_ID_A607_CLIENT_OPEN_TXT_MSG packet
//! \since Protocol A607
__packed struct client_to_server_D607_open_text_msg_data_type /* D607 */
{
    time_type           origination_time;         //!< Time when the message was created by the client
    sc_position_type    scposn;                   //!< Position when the text message was created by the client
    uint32              unique_id;                //!< Unique ID generated by client
    uint8               id_size;                  //!< ID size of message being responded to
    uint8               reserved[ 3 ];            //!< Set to 0
    uint8               id[ 16 ];                 //!< ID of message being responded to
    char                text_message[ 200 ];      //!< Message text (variable length, null-terminated string)
};
#endif

#if( FMI_SUPPORT_A603 )
//! \brief Payload of FMI_ID_A603_STOP packet
//! \since Protocol A603
__packed struct A603_stop_data_type
    {
    time_type             origination_time;    //!< Time when the stop was originated by the server
    sc_position_type      stop_position;       //!< Location of the stop
    uint32                unique_id;           //!< Unique ID of the stop for use with the Stop Status protocol
    char                  text[ 200 ];         //!< Text (description) of stop. Variable length, null-terminated string.
    };

//! \brief Payload of FMI_ID_STOP_STATUS and FMI_ID_STOP_STATUS_REQUEST packets
//! \since Protocol A603
__packed struct stop_status_data_type
    {
    uint32                    unique_id;           //!< Unique ID of the A603 stop
    uint16                    stop_status;         //!< The stop status
    uint16                    stop_index_in_list;  //!< The stop index in list
    };

//! \brief Payload of FMI_ID_STOP_STATUS_RCPT packet
//! \since Protocol A603
__packed struct stop_status_receipt_data_type
    {
    uint32                    unique_id;           //!< unique_id from the stop_status_data_type.
    };

#endif // FMI_SUPPORT_A603

#if( FMI_SUPPORT_A602 )
//! \brief Payload of FMI_ID_A602_STOP packet
//! \since Protocol A602
__packed struct A602_stop_data_type
    {
    time_type           origination_time;     //!< Origination time when the server sent the stop to the client
    sc_position_type    stop_position;        //!< Location of the stop
    char                text[ 51 ];           //!< Text/description of the stop.  Variable length, null-terminated string.
    };
#endif

#if( FMI_SUPPORT_LEGACY )
//! \brief Payload of Garmin ID_LEGACY_STOP_MSG packet
__packed struct legacy_stop_data_type
    {
    sc_position_type    stop_position;         //!< Location of the stop
    char                text[ 200 ];           //!< Text/description of the stop.  Variable length, null-terminated string.
    };
#endif

#if( FMI_SUPPORT_A603 )
//! \brief Payload of FMI_ID_AUTO_ARRIVAL packet
//! \since Protocol A603
__packed struct auto_arrival_data_type /* D603 */
    {
    uint32 stop_time;                          //!< Minimum stop time before auto-arrival is activated, in seconds
    uint32 stop_distance;                      //!< Minimum distance to destination before auto-arrival is activated, in meters
    };

//! Data type for the ETA Data Packet ID
//! \since Protocol A603
__packed struct eta_data_type /* D603 */
    {
    uint32                  unique_id;               //!< Uniquely identifies the ETA message
    time_type               eta_time;                //!< Estimated time of arrival, or 0xFFFFFFFF if no active destination
    uint32                  distance_to_destination; //!< Distance to destination, in meters, or 0xFFFFFFFF if no active destination
    sc_position_type        position_of_destination; //!< Location of destination
    };

//! Data type for the ETA Data Receipt Packet ID
//! \since Protocol A603
__packed struct eta_data_receipt_type /* D603 */
    {
    uint32 unique_id;                                //!< Unique ID from eta_data_type
    };

//! Data type for the Data Deletion Packet ID
//! \since Protocol A603
__packed struct data_deletion_data_type /* D603 */
    {
    uint32 data_type;                                //!< Type of data to delete, see del_data for valid values
    };
#endif // FMI_SUPPORT_A603

#if( FMI_SUPPORT_A604 )
//! Data type for the GPI File Transfer Start Packet ID
//! \since Protocol A604
__packed struct  gpi_file_info_data_type /*  D604  */
    {
    uint32 file_size;                                //!< Size of the FMI GPI file, in bytes
    uint8  file_version_length;                      //!< Number of significant bytes in file_version
    uint8  reserved[3];                              //!< Set to 0
    uint8  file_version[16];                         //!< Server-defined version string
    };

//! Data type for GPI File Start Receipt Packet ID
//! and GPI File End Receipt Packet ID
//! \since Protocol A604
__packed struct  gpi_file_receipt_data_type /*  D604  */
    {
    uint8  result_code;                              //!< Result of operation
    uint8  reserved[3];                              //!< Set to 0
    };

//! Data type for GPI File Data Packet ID
//! \since Protocol A604
__packed struct  gpi_file_packet_data_type /*  D604  */
    {
    uint32 offset;            //!< offset of this data from the beginning of the file
    uint8  data_length;       //!< length of file_data (0..245)
    uint8  reserved[3];       //!< Set to 0
    uint8  file_data[245];    //!< file data, variable length
    };

//! Packet receipt for GPI Packet Receipt Packet ID
//! \since Protocol A604
__packed struct gpi_packet_receipt_data_type /* D604 */
    {
    uint32 offset;            //!< offset of data received
    uint32 next_offset;       //!< offset of next data the server should send, or 0xFFFFFFFF for an error
    };

//! Data type for GPI File Transfer End
//! \since Protocol A604
__packed struct gpi_file_end_data_type /* D604 */
    {
    uint32 crc;                       //!< CRC of entire file as computed by UTL_calc_crc32
    };

//! Data type for the A604 Server to Client Open Text Message Packet ID
//! \since Protocol A604
__packed struct A604_server_to_client_open_text_msg_data_type /* D604 */
    {
    time_type  origination_time;      //!< Time the message was sent from the server
    uint8      id_size;               //!< Number of significant bytes in the message ID
    uint8      message_type;          //!< Message type, a valid a604_message_type
    uint16     reserved;              //!< Set to 0
    uint8      id[16];                //!< Message ID
    char       text_message[ 200 ];   //!< Message text, variable length, null-terminated string, 200 bytes max
    };

//! Data type for the Server to Client Open Text Message Receipt Packet ID
//! \since Protocol A604
__packed struct    server_to_client_text_msg_receipt_data_type /* D604 */
    {
    time_type    origination_time;         //!< Origination time of the message being acknowledged
    uint8        id_size;                  //!< Size of the message ID
    boolean      result_code;              //!< Result code.  TRUE if success, FALSE otherwise
    uint16       reserved;                 //!< Set to 0
    uint8        id[16];                   //!< The message ID from the server to client open text message
    };

//! Data type for the Set Canned Response Packet ID
//! \since Protocol A604
__packed struct canned_response_data_type /*  D604  */
    {
    uint32   response_id;        //!< Unique ID of this canned response
    char     response_text[50];  //!< Response text to display on client (variable length, null terminated string)
    };

//! Data type for the Delete Canned Response Packet ID
//! \since Protocol A604
__packed struct canned_response_delete_data_type /* D604 */
    {
    uint32    response_id;       //!< The canned response ID to delete
    };

//! Data type for the Set Canned Response Receipt Packet ID
//! and Delete Canned Response Receipt Packet ID
//! \since Protocol A604
__packed struct canned_response_receipt_data_type /*  D604  */
    {
    uint32   response_id;        //!< The canned response ID from the set or delete
    boolean  result_code;        //!< True if the operation was successful
    uint8    reserved[3];        //!< Set to 0
    };

//! Data type for the Canned Response List Packet ID
//! \since Protocol A604
__packed struct  canned_response_list_data_type /*  D604  */
    {
    uint8      id_size;           //!< Size of the message ID
    uint8      response_count;    //!< Number of elements in response_id array
    uint16     reserved;          //!< Set to 0
    uint8      id[16];            //!< Message ID that this list is for
    uint32     response_id[50];   //!< List of responses that are allowed
    };

//! List of canned responses that the client requests updated text for
//! \since Protocol A604
__packed struct request_canned_response_list_refresh_data_type /*  D604  */
    {
    uint32 response_count;        //!< Number of responses in the array; if 0, all responses need refresh
    uint32 response_id[50];       //!< Canned response IDs
    };

//! Data type for Canned Response List Packet ID
//! \since Protocol A604
__packed struct canned_response_list_receipt_data_type /*  D604  */
    {
    uint8     id_size;            //!< id_size from the canned_response_list_data_type
    uint8     result_code;        //!< Enum indicating result code, see canned_response_list_result for valid values
    uint16    reserved;           //!< Set to 0
    uint8     id[16];             //!< Message ID from the canned_response_list_data_type
    };

//! Data type for Driver ID Update Packet ID
//! \since Protocol A604
__packed struct driver_id_data_type /* D604 */
    {
    uint32     status_change_id;   //!< Unique ID for this driver ID change
    time_type  status_change_time; //!< Time when the driver ID changed
    char       driver_id[50];      //!< New driver ID (null terminated string, 50 bytes max)
    };

#if( FMI_SUPPORT_A607 )
//! Data type for Driver ID Update Packet ID
//! \since Protocol A607
__packed struct driver_id_D607_data_type /* D607 */
    {
    uint32     status_change_id;   //!< Unique ID for this driver ID change
    time_type  status_change_time; //!< Time when the driver ID changed
    uint8      driver_idx;         //!< Driver index to change
    uint8      reserved[3];        //!< Set to 0
    char       driver_id[50];      //!< New driver ID (null terminated string)
    char       password[20];       //!< Driver password (null terminated string).  Optional if driver password support is not enabled.
    };
#endif

#if( FMI_SUPPORT_A607 )
//! Data type for Driver ID Request Packet ID
//! \note Prior to A607, the Driver ID Request Packet had no payload
//! \since Protocol A607
__packed struct driver_id_request_data_type /* D607 */
{
    uint8      driver_idx;         //!< Driver index to change
    uint8      reserved[ 3 ];      //!< Set to 0
};
#endif

//! Data type for Driver ID Receipt packet
//! \since Protocol A604
__packed struct driver_id_receipt_data_type /* D604 */
    {
    uint32  status_change_id;      //!< status_change_id from the driver_id_data_type
    boolean result_code;           //!< True if the update was successful
    uint8   driver_idx;            //!< Index of driver changed
    uint8   reserved[2];           //!< Set to 0
    };

//! Data type for the Set Driver Status List Item packet
//! \since Protocol A604
__packed struct  driver_status_list_item_data_type /* D604 */
    {
    uint32   status_id;            //!< Unique identifier and sort key for the status item
    char     status[50];           //!< Text displayed for the item (variable length, null terminated, 50 bytes max)
    };

//! Data type for the Set Driver Status List Item
//! and Delete Driver Status List Item Receipt packets
//! \since Protocol A604
__packed struct  driver_status_list_item_receipt_data_type /* D604 */
    {
    uint32  status_id;              //!< status_id from the driver_status_list_item_data_type or driver_status_list_item_delete_data_type
    boolean result_code;            //!< True if the update was successful
    uint8   driver_idx;             //!< Index of driver changed
    uint8   reserved[2];            //!< Set to 0
    };

//! Data type for Delete Driver Status List Item Receipt
//! \since Protocol A604
__packed struct driver_status_list_item_delete_data_type /* D604 */
    {
    uint32  status_id;              //!< ID for the driver status list item to delete
    };

//! Data type for the Driver Status Update packet
//! \since Protocol A604
__packed struct  driver_status_data_type /* D604 */
    {
    uint32           status_change_id;    //!< unique identifier
    time_type        status_change_time;  //!< timestamp of status change
    uint32           driver_status;       //!< ID corresponding to the new driver status
    };

//! Data type for the A607 Driver Status Update packet
//! \since Protocol A607
__packed struct  driver_status_D607_data_type /* D607 */
    {
    uint32           status_change_id;    //!< unique identifier
    time_type        status_change_time;  //!< timestamp of status change
    uint32           driver_status;       //!< ID corresponding to the new driver status
    uint8            driver_idx;          //!< Index of driver to change
    uint8            reserved[ 3 ];       //!< Set to zero
    };

//! Data type for the Driver Status Update Receipt packet
//! \since Protocol A604
__packed struct driver_status_receipt_data_type /* D604 */
    {
    uint32       status_change_id;        //!< status_change_id from the driver_status_data_type
    boolean      result_code;             //!< True if the update was successful
    uint8        driver_idx;              //!< Index of the driver to update
    uint8        reserved[ 2 ];           //!< Set to 0
    };

#if( FMI_SUPPORT_A607 )
//! Data type for Driver Status Request Packet ID
//! \since Protocol A607
__packed struct driver_status_request_data_type /* D607 */
{
    uint8      driver_idx;         //!< Driver index requested
    uint8      reserved[ 3 ];      //!< Set to 0
};
#endif

//! Data type for the Set Canned Message Packet ID
//! \since Protocol A604
__packed struct canned_message_data_type /*  D604  */
    {
    uint32     message_id;                //!< Unique identifier and sort key for this canned message
    char       message[50];               //!< Message text, variable length, null terminated (50 bytes max)
    };

//! Data type for the Delete Canned Message Packet ID
//! \since Protocol A604
__packed struct canned_message_delete_data_type /*  D604  */
    {
    uint32 message_id;                    //!< ID of the canned message to delete
    };

//! Data type for the Set Canned Message Receipt Packet ID
//! and Delete Canned Message Receipt Packet ID
//! \since Protocol A604
__packed struct canned_message_receipt_data_type /*  D604  */
    {
    uint32  message_id;                   //!< ID of the canned message
    boolean result_code;                  //!< Result (true if successful, false otherwise)
    uint8   reserved[3];                  //!< Set to 0
    };

//! Data type for the Message Status Request Packet ID
//! \since Protocol A604
__packed struct message_status_request_data_type /*  D604  */
    {
    uint8 id_size;                        //!< Number of significant bytes in the message ID
    uint8 reserved[3];                    //!< Set to 0
    uint8 id[ 16 ];                       //!< The message ID
    };

//! Data type for the Message Status Packet ID
//! \since Protocol A604
__packed struct message_status_data_type /*  D604  */
    {
    uint8 id_size;                        //!< Number of significant bytes in the message ID
    uint8 status_code;                    //!< Message status, see fmi_A604_message_status for valid values
    uint16 reserved;                      //!< Set to 0
    uint8 id[ 16 ];                       //!< The message ID
    };

//! Data type for the User Interface Text Packet ID
//! \since Protocol A604
__packed struct user_interface_text_data_type /*  D604  */
    {
    uint32 text_element_id;               //!< ID of the user interface element being changed
    char   new_text[50];                  //!< Text to display
    };

//! Data type for the User Interface Text Receipt Packet ID
//! \since Protocol A604
__packed struct user_interface_text_receipt_data_type /*  D604  */
    {
    uint32  text_element_id;              //!< text_element_id from the user_interface_text_data_type
    boolean result_code;                  //!< True if the update was successful
    uint8   reserved[3];                  //!< Set to 0
    };

//! Data type for the Message Throttling Command Packet ID
//! and Message Throttling Response Packet ID
//! \since Protocol A604
__packed struct message_throttling_data_type /*  D604  */
    {
    uint16  packet_id;                    //!< First packet ID in the protocol to throttle
    uint16  new_state;                    //!< New state, see message_throttling_state_type for valid values
    };

//! Data type for the Text Message Ack Receipt Packet ID
//! \since Protocol A604
__packed struct text_msg_id_data_type /* D604 */
    {
    uint8    id_size;                    //!< Size of the message ID
    uint8    reserved[3];                //!< Set to 0
    uint8    id[16];                     //!< Message ID
    };
#endif   // FMI_SUPPORT_A604

#if( FMI_SUPPORT_A605 )
//! Data type for the Message Throttling Query Response Packet ID
//! \since Protocol A605
__packed struct message_throttling_list_data_type /*  D605  */
    {
    uint16                       response_count;     //!< Number of protocols in the response_list
    message_throttling_data_type response_list[60];  //!< One element for each protocol with ID and state
    };
#endif

#if( FMI_SUPPORT_A606 )
//! Data type for the FMI Safe Mode setup Packet ID
//! \since Protocol A606
__packed struct safe_mode_speed_data_type /*  D606  */
    {
    float32  speed;                       //!< FMI safe mode speed
    };

//! Data type for the User Interface Text Receipt Packet ID
//! \since Protocol A606
__packed struct safe_mode_speed_receipt_data_type /*  D606  */
    {
    boolean result_code;                  //!< True if the update was successful
    uint8   reserved[3];                  //!< Set to 0
    };
#endif

#if( FMI_SUPPORT_A608 )
//! Data type for the Speed Limit Alert Packet ID
//! \since Protocol A608
__packed struct speed_limit_alert_data_type
    {
    uint8            category;            //!< Alert category, a valid speed_limit_alert_category_type
    uint8            reserved[3];         //!< Set to 0
    sc_position_type posn;                //!< Position at the time of alert
    time_type        timestamp;           //!< Time the alert was generated
    float            speed;               //!< Speed at the time of alert
    float            speed_limit;         //!< Speed limit at the time of alert
    float            max_speed;           //!< Maximum speed since last alert
    };

//! Data type for the Speed Limit Alerts setup Packet ID
//! \since Protocol A608
__packed struct speed_limit_data_type
    {
    uint8            mode;                //!< Mode, a valid speed_limit_alert_mode_type
    uint8            time_over;           //!< Seconds until speeding event begins
    uint8            time_under;          //!< Seconds until speeding event ends
    boolean          alert_user;          //!< Audibly alert the driver
    float            threshold;           //!< Speed over speed limit when speeding event begins
    };

//! Data type for the Speed Limit Alerts setup Receipt Packet ID
//! \since Protocol A608
__packed struct speed_limit_receipt_data_type
    {
    uint8            result_code;         //!< Result code, a valid speed_limit_alert_result_data_type
    uint8            reserved[3];         //!< Set to 0
    };

//! Data type for the Speed Limit Alert Receipt Packet ID
//! \since Protocol A608
__packed struct speed_limit_alert_receipt_data_type
    {
    time_type       timestamp;            //!< Timestamp of the alert that is being acknowledged
    };
#endif

#if( FMI_SUPPORT_A607 )
//! Data type for FMI_ID_WAYPOINT packet
//! \since Protocol A607
__packed struct waypoint_data_type
{
    uint16                       unique_id;         //!< Server-assigned unique ID for the waypoint
    uint16                       symbol;            //!< Waypoint symbol
    sc_position_type             posn;              //!< Waypoint position
    uint16                       cat;               //!< Waypoint categories, bit-mapped
    char                         name[ 30 + 1 ];    //!< Waypoint name, null-terminated
    char                         comment[ 50 + 1 ]; //!< Waypoint comment, null-terminated
};

//! Data type for the FMI_ID_WAYPOINT_RCPT packet
//! \since Protocol A607
__packed struct waypoint_rcpt_data_type
{
    uint16                       unique_id;         //!< Server-assigned unique ID from the FMI_ID_WAYPOINT packet
    boolean                      result_code;       //!< TRUE if the operation was successful, FALSE otherwise
    uint8                        reserved;          //!< Set to 0
};

//! Data type for the FMI_ID_WAYPOINT_DELETED packet
//! \since Protocol A607
typedef waypoint_rcpt_data_type waypoint_deleted_data_type;

//! Data type for the FMI_ID_CREATE_WAYPOINT_CAT packet
//! \since Protocol A607
__packed struct category_data_type
{
    uint8                        id;                //!< Waypoint category
    char                         name[16 + 1];      //!< Category name, null terminated
};

//! Data type for the FMI_ID_CREATE_WAYPOINT_CAT_RCPT packet
//! \since Protocol A607
__packed struct category_rcpt_data_type
{
    uint8                        id;                //!< Waypoint category (0-15)
    boolean                      result_code;       //!< TRUE if the operation was successful, FALSE otherwise
};

//! Data type for the FMI_ID_DELETE_WAYPOINT_CAT_RCPT packet
__packed struct delete_by_category_rcpt_data_type
{
    uint16                       cat_id;            //!< Category that was deleted (0-15)
    uint16                       count;             //!< Number of items deleted
};

//! Data type for the Message Status Request Packet ID
//! \since Protocol A607
__packed struct delete_message_request_data_type /*  D607  */
{
    uint8                        id_size;           //!< Number of significant bytes in the message ID
    uint8                        reserved[3];       //!< Set to 0
    uint8                        id[ 16 ];          //!< The message ID
};

//! Data type for the Delete Message Status Packet ID
//! \since Protocol A607
__packed struct delete_message_response_data_type /*  D607  */
{
    uint8                        id_size;           //!< Number of significant bytes in the message ID
    boolean                      result_code;       //!< TRUE if message was deleted, FALSE if message was not found
    uint16                       reserved;          //!< Set to 0
    uint8                        id[ 16 ];          //!< The message ID
};

#endif

#include "pack_end.h"

#endif  /* _FMI_H_ */
