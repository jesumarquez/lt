/*********************************************************************
*
*   HEADER NAME:
*       CUITextChangeDlg.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CUITextChangeDlg_H
#define CUITextChangeDlg_H

#include "FmiApplicationLayer.h"

//----------------------------------------------------------------------
//! \brief Dialog allowing the user to change certain UI text elements
//!     on the client
//! \since Protocol A604
//----------------------------------------------------------------------
class CUITextChangeDlg : public CDialog
{
    DECLARE_DYNAMIC( CUITextChangeDlg )
    DECLARE_MESSAGE_MAP()

public:
    CUITextChangeDlg
        (
        CWnd                * aParent,
        FmiApplicationLayer & aCom
        );

    virtual ~CUITextChangeDlg();

protected:
    virtual void DoDataExchange
        (
        CDataExchange * aDataExchange
        );

    BOOL OnInitDialog();
    afx_msg void OnChange();
    afx_msg void OnBnClickedOk();

    //! Index of the selected user interface element
    int mListIndex;

    //! The new text entered by the user for the selected element
    CString mNewText;

    //! Reference to the FMI communication controller that this dialog uses
    FmiApplicationLayer & mCom;
};

#endif
