/*********************************************************************
*
*   MODULE NAME:
*       CColoredListBox.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#include "CColoredListBox.h"

//----------------------------------------------------------------------
//! \brief Draw the list item specified by aDrawItem.
//! \details Draw the list item.  If the item text contains the search
//!     string, draw the item as black text on a yellow background; if
//!     the search text is empty or not found, draw the item as black
//!     text on a white background.  However, if the item is
//!     highlighted, use the system colors for a highlighted item.
//! \note This is an override of CListBox::DrawItem().
//! \param  aDrawItem The structure representing the item to draw.
//----------------------------------------------------------------------
void CColoredListBox::DrawItem
    (
    LPDRAWITEMSTRUCT aDrawItem
    )
{
    CDC dc;
    CRect rcItem( aDrawItem->rcItem );
    UINT nIndex = aDrawItem->itemID;
    if( (int)nIndex >= 0 )
    {
        COLORREF rgbBkGnd;
        COLORREF rgbText;
        CString  itemText;
        CString  compareText;

        GetText( nIndex, itemText );
        compareText = itemText;
        compareText = compareText.MakeLower();

        if( aDrawItem->itemState & ODS_SELECTED )
        {
            rgbBkGnd = ::GetSysColor( COLOR_HIGHLIGHT );
            rgbText = ::GetSysColor( COLOR_HIGHLIGHTTEXT );
        }
        else if( !mSearchString.IsEmpty() && compareText.Find( mSearchString, 0 ) != -1 )
        {
            rgbBkGnd = RGB( 255, 255, 0 );  // yellow
            rgbText = RGB( 0, 0, 0 );       // black
        }
        else
        {
            rgbBkGnd = RGB( 255, 255, 255 ); // white
            rgbText = RGB( 0, 0, 0 );        // black
        }
        dc.Attach( aDrawItem->hDC );

        if( aDrawItem->itemState & ODS_FOCUS )
            dc.DrawFocusRect( rcItem );

        CBrush br( rgbBkGnd );
        dc.FillRect( rcItem, &br );
        dc.SetBkColor( rgbBkGnd );
        dc.SetTextColor( rgbText );
        dc.TextOut( rcItem.left + 2, rcItem.top + 2, itemText );

        dc.Detach();
    }
}

//----------------------------------------------------------------------
//! \brief Get the search string being used.
//! \return A lowercase representation of the search string.
//----------------------------------------------------------------------
CString CColoredListBox::getSearchString() const
{
    return mSearchString;
}

//----------------------------------------------------------------------
//! \brief Set the search string to use.
//! \param  aSearchString The new search string
//----------------------------------------------------------------------
void CColoredListBox::setSearchString( CString aSearchString )
{
    mSearchString = aSearchString;
    mSearchString.MakeLower();
}
