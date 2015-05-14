/*********************************************************************
*
*   MODULE NAME:
*       CFeatureDlg.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "CFeatureDlg.h"

BEGIN_MESSAGE_MAP( CFeatureDlg, CDialog )
    ON_BN_CLICKED( IDOK, OnBnClickedOk )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \param aParent The parent window
//! \param aCom Reference to the FMI communication controller
//----------------------------------------------------------------------
CFeatureDlg::CFeatureDlg
    (
    CWnd                * aParent,
    FmiApplicationLayer & aCom
    )
    : CDialog( IDD_FEATURE_SELECT, aParent )
    , mCom( aCom )
{
    mFeatureCount = 0;
}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
CFeatureDlg::~CFeatureDlg()
{
}

//----------------------------------------------------------------------
//! \brief Initialize the dialog
//! \details This function is called when the window is created.  It
//!    gets references to each of the check boxes, and sets the default
//!    state of the Unicode Support and A607 Support boxes to checked.
//! \return TRUE, since this function does not set focus to a control
//----------------------------------------------------------------------
BOOL CFeatureDlg::OnInitDialog()
{
    mUnicode     = (CButton*)GetDlgItem( IDC_FEATURE_CHK_UNICODE );
    mA607        = (CButton*)GetDlgItem( IDC_FEATURE_CHK_A607 );
    mMultidriver = (CButton*)GetDlgItem( IDC_FEATURE_CHK_MULTIDRIVER );
    mPasswords   = (CButton*)GetDlgItem( IDC_FEATURE_CHK_PASSWORDS );

    mUnicode->SetCheck( BST_CHECKED );
    mA607->SetCheck( BST_CHECKED );

    return CDialog::OnInitDialog();
}

//----------------------------------------------------------------------
//! \brief Process a feature check box
//! \details Based on the state of aCheckBox, add aFeature to
//!    mFeatureCodes as enabled or disabled if appropriate.
//! \param aCheckBox The check box to inspect
//! \param aFeature The feature code to enable or disable
//----------------------------------------------------------------------
void CFeatureDlg::checkFeature
    (
    CButton          * aCheckBox,
    fmi_feature_type   aFeature
    )
{
    switch( aCheckBox->GetCheck() )
    {
    case BST_CHECKED:
        mFeatureCodes[mFeatureCount] = (uint16)( aFeature | FEATURE_STATE_ENABLED );
        ++mFeatureCount;
        break;

    case BST_UNCHECKED:
        mFeatureCodes[mFeatureCount] = (uint16)( aFeature | FEATURE_STATE_DISABLED );
        ++mFeatureCount;
        break;

    default:
        break;

    }
}

//----------------------------------------------------------------------
//! \brief Click handler for OK button
//! \details Build and send an extended FMI enable packet based on the
//!    state of the check boxes.
//----------------------------------------------------------------------
void CFeatureDlg::OnBnClickedOk()
{
    // Default values
    mCom.mClientCodepage = CODEPAGE_ASCII;
    mCom.mUsePasswords = FALSE;
    mCom.mUseMultipleDrivers = FALSE;

    if( BST_CHECKED == mUnicode->GetCheck() )
    {
        mCom.mClientCodepage = CODEPAGE_UNICODE;
    }

    if( BST_CHECKED == mPasswords->GetCheck() )
    {
        mCom.mUsePasswords = TRUE;
    }

    if( BST_CHECKED == mMultidriver->GetCheck() )
    {
        mCom.mUseMultipleDrivers = TRUE;
    }

    // Build and send the enable request
    checkFeature( mUnicode, FEATURE_ID_UNICODE );
    checkFeature( mA607, FEATURE_ID_A607_SUPPORT );
    checkFeature( mPasswords, FEATURE_ID_DRIVER_PASSWORDS );
    checkFeature( mMultidriver, FEATURE_ID_MULTIPLE_DRIVERS );

    mCom.sendEnable( mFeatureCodes, mFeatureCount );

    OnOK();
}
