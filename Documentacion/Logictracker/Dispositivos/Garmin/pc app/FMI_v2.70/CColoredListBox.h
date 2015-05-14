/*********************************************************************
*
*   HEADER NAME:
*       CColoredListBox.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef CColoredListBox_H
#define CColoredListBox_H

#include "stdafx.h"

//----------------------------------------------------------------------
//! \brief A CListBox that highlights list items matching a search
//! string.
//----------------------------------------------------------------------
class CColoredListBox : public CListBox
{
public:
    void DrawItem
        (
        LPDRAWITEMSTRUCT aDrawItem
        );

    CString getSearchString() const;

    void setSearchString
        (
        CString aSearchString
        );

protected:
    //! \brief The search string to match.
    CString mSearchString;

    //! \brief If true, a search string was specified.
    BOOL    mHasSearchString;
};

#endif
