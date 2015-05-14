/*********************************************************************
*
*   MODULE NAME:
*       CTxtMsgFromClient.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CTxtMsgFromClient.h"
#include "util.h"

IMPLEMENT_DYNAMIC( CTxtMsgFromClient, CDialog )

BEGIN_MESSAGE_MAP( CTxtMsgFromClient, CDialog )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//! \param aEvent The text message received from the client
//----------------------------------------------------------------------
CTxtMsgFromClient::CTxtMsgFromClient
    (
    CWnd                                  * aParent,
    FmiApplicationLayer                   & aCom,
    const text_msg_from_client_event_type * aEvent
    )
    : CDialog( IDD_TXT_MSG_FROM_CLIENT, aParent )
    , mTextMessageEvent( *aEvent )
    , mCom( aCom )
    , mOriginationTime( _T("") )
    , mMessageText( _T("") )
    , mMessageId( _T("") )
#if( FMI_SUPPORT_A607 )
    , mLinkId( _T("") )
#endif
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CTxtMsgFromClient::~CTxtMsgFromClient()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CTxtMsgFromClient::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );
    DDX_Text( aDataExchange, IDC_CLIENTMSG_TXT_TIME, mOriginationTime );
    DDX_Text( aDataExchange, IDC_CLIENTMSG_TXT_MESSAGE, mMessageText );
    DDX_Text( aDataExchange, IDC_CLIENTMSG_TXT_ID, mMessageId );
#if( FMI_SUPPORT_A607 )
    DDX_Text( aDataExchange, IDC_CLIENTMSG_TXT_LINK_ID, mLinkId );
#endif
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer. It also initializes the text boxes from
//!     the FmiApplicationLayer members which contain the text message
//!     details.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CTxtMsgFromClient::OnInitDialog()
{
    TCHAR               stringBuffer[200];
    time_type           localTime;
    date_time_data_type localDateTime;
    char                localTimeString[13];

    CDialog::OnInitDialog();

    UTIL_convert_UTC_to_local( &mTextMessageEvent.origination_time, &localTime );
    UTIL_convert_seconds_to_time_type( &localTime, &localDateTime );
    UTIL_format_time_string( &localDateTime, localTimeString );

    MultiByteToWideChar( mCom.mClientCodepage, 0, localTimeString, -1, stringBuffer, 13 );
    stringBuffer[13] = '\0';
    mOriginationTime.Format( _T(" %s"), stringBuffer );

    MultiByteToWideChar( mCom.mClientCodepage, 0, mTextMessageEvent.message_text, -1, stringBuffer, 200 );
    stringBuffer[199] = '\0';
    mMessageText.Format( _T(" %s"), stringBuffer );
    mMessageId.Format( _T(" %d"), mTextMessageEvent.message_id );

#if( FMI_SUPPORT_A607 )
    mLinkId = mTextMessageEvent.link_id.toCString( mCom.mClientCodepage );
#endif

    UpdateData( FALSE );

    return TRUE;
}    /* OnInitDialog() */
