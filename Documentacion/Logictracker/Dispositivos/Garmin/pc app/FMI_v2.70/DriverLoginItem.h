/*********************************************************************
*
*   HEADER NAME:
*       DriverLoginItem.h
*
*   Copyright 2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef DRIVERLOGINITEM_H
#define DRIVERLOGINITEM_H

#include <string>

#include "ClientListItem.h"

//! \brief Data structure to holds an allowed driver login (ID and 
//!     password).  Used by the FmiApplicationLayer to validate logins
//!     when driver password support is enabled.
//! \since Protocol A607
class DriverLoginItem :
    public ClientListItem
{
public:
    typedef std::string key_type;

    DriverLoginItem();
    virtual ~DriverLoginItem();

    CString getDriverId() const;

    const key_type & getId() const;
    CString getPassword() const;

    void readFromStream ( std::istream &aStream );
    void save();
    void setId(const key_type & aId);
    void setParent( FileBackedMap<DriverLoginItem>* aParent );

    void setPassword( const std::string & aPassword );
    void writeToStream ( std::ofstream &aStream ) const;

private:
    //! The driver ID, UTF8 encoded.
    std::string mDriverId;

    //! The driver password, UTF8 encoded.
    std::string mDriverPassword;

    //! The map that this DriverLoginItem is a part of.
    FileBackedMap<DriverLoginItem>* mParent;

};

#endif
