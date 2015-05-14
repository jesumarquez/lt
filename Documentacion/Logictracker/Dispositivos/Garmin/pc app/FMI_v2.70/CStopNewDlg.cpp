/*********************************************************************
*
*   MODULE NAME:
*       CStopNewDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CStopNewDlg.h"
#include "util.h"

IMPLEMENT_DYNAMIC( CStopNewDlg, CDialog )

BEGIN_MESSAGE_MAP( CStopNewDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_EN_CHANGE( IDC_STOPNEW_EDIT_LAT, OnFormChanged )
    ON_EN_CHANGE( IDC_STOPNEW_EDIT_LON, OnFormChanged )
    ON_EN_CHANGE( IDC_STOPNEW_EDIT_NAME, OnFormChanged )
    ON_BN_CLICKED( IDC_STOPNEW_RDO_LEGACY, OnFormChanged )
#if( FMI_SUPPORT_A602 )
    ON_BN_CLICKED( IDC_STOPNEW_RDO_A602, OnFormChanged )
#endif
#if( FMI_SUPPORT_A603 )
    ON_EN_CHANGE( IDC_STOPNEW_EDIT_STOPID, OnFormChanged )
    ON_BN_CLICKED( IDC_STOPNEW_RDO_A603, OnFormChanged )
#endif
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CStopNewDlg::CStopNewDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_STOP_NEW, aParent )
    , mCom( aCom )
    , mLatitudeStr( _T("") )
    , mLongitudeStr( _T("") )
    , mMessageStr( _T("") )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CStopNewDlg::~CStopNewDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CStopNewDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

    DDX_Text( aDataExchange, IDC_STOPNEW_EDIT_LAT, mLatitudeStr );
    DDX_Text( aDataExchange, IDC_STOPNEW_EDIT_LON, mLongitudeStr );
    DDX_Text( aDataExchange, IDC_STOPNEW_EDIT_NAME, mMessageStr );
    DDX_Radio( aDataExchange, IDC_STOPNEW_RDO_A603, mStopProtocol );
#if( FMI_SUPPORT_A603 )
    DDX_Text( aDataExchange, IDC_STOPNEW_EDIT_STOPID, mStopId );
#endif
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to FmiApplicationLayer.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CStopNewDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    UpdateData( TRUE );
#if( FMI_SUPPORT_A603 )
    mStopProtocol = STOP_PROTOCOL_A603;
#elif( FMI_SUPPORT_A602 )
    mStopProtocol = STOP_PROTOCOL_A602;
#else
    mStopProtocol = STOP_PROTOCOL_LEGACY;
#endif

#if( FMI_SUPPORT_A603 )
    mStopId.Format( _T("%u"), mCom.getNextStopId() );
#endif
#if( !FMI_SUPPORT_A603 )
    CButton * rdo603 = (CButton *)GetDlgItem( IDC_STOPNEW_RDO_A603 );
    rdo603->SetCheck( BST_UNCHECKED );
    rdo603->EnableWindow( FALSE );
    GetDlgItem( IDC_STOPNEW_EDIT_STOPID )->EnableWindow( FALSE );
#endif
#if( !FMI_SUPPORT_A602 )
    CButton * rdo602 = (CButton *)GetDlgItem( IDC_STOPNEW_RDO_A602 );
    rdo602->SetCheck( BST_UNCHECKED );
    rdo602->EnableWindow( FALSE );
#endif
#if( !FMI_SUPPORT_LEGACY )
    CButton * rdoLegacy = (CButton *)GetDlgItem( IDC_STOPNEW_RDO_LEGACY );
    rdoLegacy->SetCheck( BST_UNCHECKED );
    rdoLegacy->EnableWindow( FALSE );
#endif
    UpdateData( FALSE );
    return TRUE;
}   /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Change handler for all radio buttons and edit boxes on this
//!     dialog
//! \details Validate all required fields, and enable the OK button
//!     if all fields are present.
//----------------------------------------------------------------------
void CStopNewDlg::OnFormChanged()
{
    bool formIsValid = true;

#if !SKIP_VALIDATION
    char stopIdString[11];
    memset( stopIdString, 0, sizeof( stopIdString ) );

    UpdateData( TRUE );
    if( mLatitudeStr == "" || _tstof( mLatitudeStr ) < -90 || _tstof( mLatitudeStr ) >= 90 )
        formIsValid = false;
    if( mLongitudeStr == "" || _tstof( mLongitudeStr ) < -180 || _tstof( mLongitudeStr ) >= 180 )
        formIsValid = false;
    if( mMessageStr.GetLength() == 0 )
        formIsValid = false;
    if( mStopProtocol == STOP_PROTOCOL_A602 && mMessageStr.GetLength() >= 51 )
        formIsValid = false;
    if( mMessageStr.GetLength() >= 200 )
        formIsValid = false;

    #if( FMI_SUPPORT_A603 )
    if( mStopProtocol == STOP_PROTOCOL_A603 )
    {
        GetDlgItem( IDC_STOPNEW_EDIT_STOPID )->EnableWindow( TRUE );
        GetDlgItem( IDC_STOPNEW_LBL_STOPID )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDC_STOPNEW_EDIT_STOPID )->EnableWindow( FALSE );
        GetDlgItem( IDC_STOPNEW_LBL_STOPID )->EnableWindow( FALSE );
    }
    if( 0 != WideCharToMultiByte( CP_ACP, 0, mStopId.GetBuffer(), mStopId.GetLength(), stopIdString, sizeof( stopIdString ), NULL, NULL ) )
    {
        if( UTIL_data_is_uint32( stopIdString ) )
        {
            uint32 stopId = _tstoi( mStopId.GetBuffer() );
            if( mCom.mA603Stops.contains( stopId ) )
                formIsValid = false;
        }
        else
        {
            formIsValid = false;
        }

    }
    else
    {
        formIsValid = false;
    }
    #endif
#endif

    if( formIsValid )
    {
        GetDlgItem( IDOK )->EnableWindow( TRUE );
    }
    else
    {
        GetDlgItem( IDOK )->EnableWindow( FALSE );
    }

}   /* OnFormChanged */

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Gets the detail from the form and sends a new stop to the
//!     client using the mStopProtocol specified by the user.
//----------------------------------------------------------------------
void CStopNewDlg::OnBnClickedOk()
{
    double lat;
    double lon;
    char message[200];
#if( FMI_SUPPORT_A603 )
    uint32 stopId;
#endif

    UpdateData( TRUE );
    WideCharToMultiByte( mCom.mClientCodepage, 0, mMessageStr.GetBuffer(), -1, message, 200, NULL, NULL );
    message[199] = '\0';
    lat = _tstof( mLatitudeStr.GetBuffer() );
    lon = _tstof( mLongitudeStr.GetBuffer() );
    switch( mStopProtocol )
        {
#if( FMI_SUPPORT_A603 )
        case STOP_PROTOCOL_A603: // A603
            if( mStopId == "" )
                stopId = mCom.getNextStopId();
            else
                stopId = _tstoi( mStopId.GetBuffer() );
            mCom.sendA603Stop( lat, lon, message, stopId );
            break;
#endif
#if( FMI_SUPPORT_A602 )
        case STOP_PROTOCOL_A602: // A602
            mCom.sendA602Stop( lat, lon, message );
            break;
#endif
#if( FMI_SUPPORT_LEGACY )
        case STOP_PROTOCOL_LEGACY: // Legacy
            mCom.sendLegacyStop( lat, lon, message );
            break;
#endif
        default:
            break;
        }

    OnOK();
}   /* OnBnClickedOk() */
