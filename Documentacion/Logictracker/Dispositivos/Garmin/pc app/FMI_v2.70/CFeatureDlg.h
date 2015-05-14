/*********************************************************************
*
*   HEADER NAME:
*      CFeatureDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CFeatureDlg_H
#define CFeatureDlg_H

#include "fmi.h"
#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief FMI Feature (enable) modal dialog
//! \details This dialog allows the user to send an FMI Enable packet
//!     with the specified features enabled or disabled.
//! \since Protocol A607
//----------------------------------------------------------------------
class CFeatureDlg : public CDialog
{
    DECLARE_MESSAGE_MAP()

public:
    CFeatureDlg
        (
        CWnd * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CFeatureDlg();

    afx_msg void OnBnClickedOk();
    BOOL OnInitDialog();

protected:
    void checkFeature
        (
        CButton * aCheckBox,
        fmi_feature_type aFeature
        );

    //! Reference to the FMI application layer
    FmiApplicationLayer &mCom;

    //! Number of feature codes to send in the FMI Enable packet
    uint8 mFeatureCount;

    //! Array of feature codes to send in the FMI Enable packet
    uint16 mFeatureCodes[ 126 ];

    //! If TRUE, the Unicode Support check box is selected
    CButton *mUnicode;

    //! If TRUE, the A607 Support check box is selected
    CButton *mA607;

    //! If TRUE, the Multiple Driver Support check box is selected
    CButton *mMultidriver;

    //! If TRUE, the Driver Password Support check box is selected
    CButton *mPasswords;
};

#endif
