//////////////////////////////////////////////////////////////////
// CStaticLink 1997 Microsoft Systems Journal.
// If this program works, it was written by Paul DiLascia.
// If not, I don't know who wrote it.
// CStaticLink implements a static control that's a hyperlink
// to any file on your desktop or web. You can use it in dialog boxes
// to create hyperlinks to web sites. When clicked, opens the file/URL
//
#include "stdafx.h"
#include "CStaticLink.h"

IMPLEMENT_DYNAMIC( CStaticLink, CStatic )

BEGIN_MESSAGE_MAP( CStaticLink, CStatic )
    ON_WM_CTLCOLOR_REFLECT()
    ON_CONTROL_REFLECT( STN_CLICKED, OnClicked )
END_MESSAGE_MAP()

//----------------------------------------------------------------------
//! \brief Constructor
//! \details Sets the default colors to use (unvisited: blue,
//!    visited: purple) and initializes the link as unvisited.
//----------------------------------------------------------------------
CStaticLink::CStaticLink()
{
    mUnvisitedColor = RGB(   0, 0, 255 );     // blue
    mVisitedColor   = RGB( 128, 0, 128 );     // purple
    mIsVisited      = FALSE;                  // not visited yet
}

//----------------------------------------------------------------------
//! \brief Handle reflected WM_CTLCOLOR to set custom control color.
//! \details For a text control, use visited/unvisited colors and
//!     underline font.  For non-text controls, do nothing. Also
//!     ensures SS_NOTIFY is on.
//! \param aDC Contains a pointer to the display context for the
//!     child window.
//! \param aCtlColor Specifies the type of control.
//! \return A handle to the brush that is to be used for painting the
//!     control background.
//! \note For more information, see CWnd::OnCtlColor and the
//!     ON_WM_CTLCOLOR_REFLECT macro, both in the MSDN Library.
//----------------------------------------------------------------------
afx_msg HBRUSH CStaticLink::CtlColor
    (
    CDC  * aDC,
    UINT   aCtlColor
    )
{
    ASSERT( aCtlColor == CTLCOLOR_STATIC );

    DWORD dwStyle = GetStyle();
    if( !( dwStyle & SS_NOTIFY ) )
    {
        // Turn on notify flag to get mouse messages and STN_CLICKED.
        // Otherwise, I'll never get any mouse clicks!
        ::SetWindowLong( m_hWnd, GWL_STYLE, dwStyle | SS_NOTIFY ) ;
    }

    HBRUSH hbr = NULL;
    if( ( dwStyle & 0xFF ) <= SS_RIGHT )
    {
        // this is a text control: set up font and colors
        if( !(HFONT)mFont )
        {
            // first time init: create font
            LOGFONT lf;
            GetFont()->GetObject( sizeof( lf ), &lf );
            lf.lfUnderline = TRUE;
            mFont.CreateFontIndirect( &lf );
        }

        // use underline font and visited/unvisited colors
        aDC->SelectObject( &mFont );
        aDC->SetTextColor( mIsVisited ? mVisitedColor : mUnvisitedColor );
        aDC->SetBkMode( TRANSPARENT );

        // return hollow brush to preserve parent background color
        hbr = (HBRUSH)::GetStockObject( HOLLOW_BRUSH );
    }
    return hbr;
}

//----------------------------------------------------------------------
//! \brief Handle mouse click; open the link.
//----------------------------------------------------------------------
void CStaticLink::OnClicked()
{
    if( mLinkText.IsEmpty() )          // if URL/filename not set..
    {
        GetWindowText( mLinkText );    // ..get it from window text
    }

    // Call ShellExecute to run the file.
    // For an URL, this means opening it in the browser.
    ShellExecute( NULL, _T("open"), mLinkText, NULL, NULL, SW_SHOWNORMAL );

    mIsVisited = TRUE;       // (not really--might not have found link)

    Invalidate();            // repaint to show visited color
}
