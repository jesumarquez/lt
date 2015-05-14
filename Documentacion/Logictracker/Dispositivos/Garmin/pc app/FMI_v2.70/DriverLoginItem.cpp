/*********************************************************************
*
*   MODULE NAME:
*       DriverLoginItem.cpp
*
*   Copyright 2009-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "DriverLoginItem.h"

//----------------------------------------------------------------------
//! \brief Constructor
//----------------------------------------------------------------------
DriverLoginItem::DriverLoginItem() 
    : mParent( NULL )
{

}

//----------------------------------------------------------------------
//! \brief Destructor
//----------------------------------------------------------------------
DriverLoginItem::~DriverLoginItem()
{
}

//----------------------------------------------------------------------
//! \brief Get the driver ID as a CString
//! \return the driver ID, as a CString
//----------------------------------------------------------------------
CString DriverLoginItem::getDriverId() const
{
    CString driverId;
    TCHAR tcharName[50];
    MultiByteToWideChar( CP_UTF8, 0, mDriverId.c_str(), -1, tcharName, 50 );
    driverId.SetString( tcharName );

    return driverId;
}

//----------------------------------------------------------------------
//! \brief Get item's ID
//! \details Get the unique ID (driver ID) of this item
//! \return The item's ID
//----------------------------------------------------------------------
const DriverLoginItem::key_type & DriverLoginItem::getId() const
{
    return mDriverId;
}

//----------------------------------------------------------------------
//! \brief Set the ID (key) of this item
//! \details Sets the key associated with this item
//----------------------------------------------------------------------
CString DriverLoginItem::getPassword() const
{
    CString driverPassword;
    TCHAR tcharName[50];
    MultiByteToWideChar( CP_UTF8, 0, mDriverPassword.c_str(), -1, tcharName, 50 );
    driverPassword.SetString( tcharName );

    return driverPassword;
}

//----------------------------------------------------------------------
//! \brief Read a DriverLoginItem from an input stream
//! \details Read id and password from the input stream, and
//!     updates the member variables appropriately.
//! \param aStream The stream to read from
//----------------------------------------------------------------------
void DriverLoginItem::readFromStream
    (
    std::istream &aStream
    )
{
    char temp;

    mDriverId.clear();
    while( aStream.peek() != '\0' && !aStream.eof() )
    {
        aStream.read( &temp, 1 );
        mDriverId.append( 1, temp );
    }

    aStream.get();    // consume the '\0' inserted between mDriverId and mPassword

    mDriverPassword.clear();
    while( aStream.peek() != '\n' && !aStream.eof() )
    {
        aStream.read( &temp, 1 );
        mDriverPassword.append( 1, temp );
    }

    aStream.get();    // consume the '\n' inserted after mPassword

    if( !aStream.eof() )
    {
        mIsValid = TRUE;
    }
}

//----------------------------------------------------------------------
//! \brief Save this item
//! \details Invokes parent to save all items
//----------------------------------------------------------------------
void DriverLoginItem::save()
{
    if( mParent )
    {
        mParent->save();
    }
}

//----------------------------------------------------------------------
//! \brief Set the ID (key) of this item
//! \details Sets the key associated with this item
//! \param aId The item's ID
//----------------------------------------------------------------------
void DriverLoginItem::setId
    (
    const key_type & aId
    )
{
    mDriverId = aId;
}

//----------------------------------------------------------------------
//! \brief Set the parent map of this item
//! \details Sets the map that this item is in
//! \param aParent The item's parent
//----------------------------------------------------------------------
void DriverLoginItem::setParent
    (
    FileBackedMap<DriverLoginItem>* aParent
    )
{
    mParent = aParent;
}

//----------------------------------------------------------------------
//! \brief Set the password of this driver
//! \details Sets the password for this driver login
//! \param aPassword The driver's password
//----------------------------------------------------------------------
void DriverLoginItem::setPassword
    (
    const std::string & aPassword
    )
{
    mDriverPassword = aPassword; 
   
    setValid();
}

//----------------------------------------------------------------------
//! \brief Write a DriverLoginItem to an output stream
//! \details Append driver ID and password to the output stream.
//! \param aStream The stream to write to
//----------------------------------------------------------------------
void DriverLoginItem::writeToStream
    (
    std::ofstream &aStream
    ) const
{
    aStream << mDriverId << '\0' << mDriverPassword << '\n';
}
