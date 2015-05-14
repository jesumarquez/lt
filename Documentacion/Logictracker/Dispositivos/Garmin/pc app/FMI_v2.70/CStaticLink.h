//////////////////////////////////////////////////////////////////
// CStaticLink 1997 Microsoft Systems Journal.
// If this program works, it was written by Paul DiLascia.
// If not, I don't know who wrote it.
/////////////////////////////////////////////////////////////////
#ifndef CStaticLink_H
#define CStaticLink_H

//----------------------------------------------------------------------
//! \brief Static hyperlink control
//! \details CStaticLink implements a static control that's a hyperlink
//!     to any file on your desktop or web. You can use it in dialog
//!     boxes to create hyperlinks to web sites. When clicked, opens
//!     the file/URL.
//----------------------------------------------------------------------
class CStaticLink : public CStatic
{
    DECLARE_DYNAMIC( CStaticLink )
    DECLARE_MESSAGE_MAP()

public:
    CStaticLink();

    // you can change these any time:
    COLORREF     mUnvisitedColor;         //!< color for unvisited
    COLORREF     mVisitedColor;           //!< color for visited
    BOOL         mIsVisited;               //!< whether visited or not

    //! \brief URL/filename for non-text controls (e.g., icon, bitmap)
    //! \details Link text to use.  If you don't set this, CStaticLink
    //!     will use GetWindowText to get the link.
    CString     mLinkText;

protected:
    CFont         mFont;                  //!< underline font for text control

    // message handlers
    afx_msg HBRUSH CtlColor
        (
        CDC  * aDC,
        UINT   aCtlColor
        );
    afx_msg void OnClicked();
};

#endif
