/*********************************************************************
*
*   MODULE NAME:
*       CSelectCommPortDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "stdafx.h"
#include "CFmiApplication.h"
#include "CSelectCommPortDlg.h"
#include "SerialPort.h"

IMPLEMENT_DYNAMIC( CSelectCommPortDlg, CDialog )

BEGIN_MESSAGE_MAP( CSelectCommPortDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
    ON_BN_CLICKED( IDCANCEL, OnBnClickedCancel )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent of this dialog
//----------------------------------------------------------------------
CSelectCommPortDlg::CSelectCommPortDlg
    (
    CWnd * aParent
    )
    : CDialog( IDD_SELECT_PORT, aParent )
    , mSelectedPortIndex( 0 )
{
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CSelectCommPortDlg::~CSelectCommPortDlg()
{
}

//----------------------------------------------------------------------
//! \brief Perform dialog data exchange and validation
//! \param  aDataExchange The DDX context
//----------------------------------------------------------------------
void CSelectCommPortDlg::DoDataExchange
    (
    CDataExchange * aDataExchange
    )
{
    CDialog::DoDataExchange( aDataExchange );

    DDX_CBIndex( aDataExchange, IDC_COMPORT_CBO_PORT, mSelectedPortIndex );
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created. It
//!     sets up the parent, so it can get info from and send a message
//!     to Com.  It also enumerates the serial ports on the system
//!     (trying both the registry and CreateFile methods) and updates
//!     the combo box with the list of ports found.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CSelectCommPortDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    CComboBox * portList = (CComboBox *)GetDlgItem( IDC_COMPORT_CBO_PORT );
    portList->ResetContent();

    std::list<CString> portNames;
    SerialPort::getPortList( portNames );

    std::list<CString>::const_iterator iter;
    for( iter = portNames.begin(); iter != portNames.end(); iter++ )
    {
        portList->AddString( iter->GetString() );
    }

    portList->SetCurSel( 0 );

    return TRUE;
} /* OnInitDialog() */

//----------------------------------------------------------------------
//! \brief Click handler for the OK button
//! \details Starts the init process with the Com Port chosen.
//----------------------------------------------------------------------
void CSelectCommPortDlg::OnBnClickedOk()
{
    UpdateData( TRUE );
    CComboBox * portList = (CComboBox *)GetDlgItem( IDC_COMPORT_CBO_PORT );
    CString portName;

    portList->GetLBText( mSelectedPortIndex, portName );
    if( SerialPort::getInstance()->init( portName ) )
    {
        OnOK();
    }
    else
    {
        ::MessageBox( m_hWnd, SerialPort::getInstance()->getLastError(), _T("Error"), MB_OK );
    }
}    /* OnBnClickedOk() */

//----------------------------------------------------------------------
//! \brief Click handler for the Cancel button
//! \details Close the dialog.
//----------------------------------------------------------------------
void CSelectCommPortDlg::OnBnClickedCancel()
{
    OnCancel();
}
