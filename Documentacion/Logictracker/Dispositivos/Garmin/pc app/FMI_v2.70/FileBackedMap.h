/*********************************************************************
*
*   HEADER NAME:
*       FileBackedMap.h
*
*   Copyright 2008-2011 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#ifndef _FILEBACKEDMAP_H
#define _FILEBACKEDMAP_H

#include <map>
#include <vector>

//----------------------------------------------------------------------
//! \brief Map whose contents are also saved to a file.
//! \details A FileBackedMap is a map of uint32 to a value type.  Each
//!    value in the map for which isValid() returns TRUE is saved to
//!    a text file, with one line per item.
//!
//!    Methods are provided to add/replace, remove, check for, and get
//!    items from the underlying map, iterate through the items in the
//!    map, and clear the map's contents.  All keep the underlying file
//!    in sync.
//!
//! \tparam T The value type for items in the map.
//! \since Protocol A603
//----------------------------------------------------------------------
template <class T>
class FileBackedMap
{
public:
    //! key type
    typedef typename T::key_type key_type;

    //--------------------------------------------------------------------
    //! \brief Create a new FileBackedMap
    //! \details Construct a FileBackedMap and load all items from file
    //! \param aFileName The file associated with this map.
    //! \note In this implementation, aFileName must be static const; the
    //!     name is referenced, not copied.
    //--------------------------------------------------------------------
    FileBackedMap
        (
        const char * aFileName
        ) : mFileName( aFileName )
    {
        load();
    }

    //! Iterator for read-only traversal through the map
    typedef typename std::map<key_type, T>::const_iterator const_iterator;

    //--------------------------------------------------------------------
    //! \brief Iterator positioned at the first element in the map
    //--------------------------------------------------------------------
    const_iterator begin()
    {
        return mMap.begin();
    }

    //--------------------------------------------------------------------
    //! \brief Iterator positioned after the last element in the map
    //--------------------------------------------------------------------
    const_iterator end()
    {
        return mMap.end();
    }

    //--------------------------------------------------------------------
    //! \brief Check whether the specified key is in the map
    //! \param aKey The key to look for
    //! \return true if the item is in the map, false otherwise
    //--------------------------------------------------------------------
    bool contains
        (
        const key_type & aKey
        )
    {
        return mMap.end() != mMap.find( aKey );
    }

    //--------------------------------------------------------------------
    //! \brief Remove all elements from the map
    //--------------------------------------------------------------------
    void clear()
    {
        mMap.clear();
        save();
    }

    //--------------------------------------------------------------------
    //! \brief Get the item by key
    //! \details Get the item with the specified key, creating it if it
    //!     is not already in the map.
    //! \param aKey The key of the item to retrieve
    //! \return The item with the specified key.
    //--------------------------------------------------------------------
    T & get
        (
        const key_type & aKey
        )
    {
        T& item = mMap[aKey];
        item.setId( aKey );
        item.setParent( this );
        return item;
    }

    //--------------------------------------------------------------------
    //! \brief Get the key for the item at a given list index
    //! \param aIndex The key of the item to retrieve.  Must be less than
    //!     validCount().
    //! \return The key of the item at the specified index.
    //--------------------------------------------------------------------
    const key_type& getKeyAt
        (
        uint32 aIndex
        ) const
    {
        ASSERT( aIndex < mKeyList.size() );

        return mKeyList.at( aIndex );
    }

    //--------------------------------------------------------------------
    //! \brief Add (or replace) an item in the map
    //! \param aValue The item to add
    //--------------------------------------------------------------------
    void put
        (
        T & aValue
        )
    {
        aValue.setParent( this );
        mMap[ aValue.getId() ] = aValue;
        save();
    }

    //--------------------------------------------------------------------
    //! \brief Remove an item from the map
    //! \param aKey The key of the item to remove
    //--------------------------------------------------------------------
    void remove
        (
        const key_type & aKey
        )
    {
        std::map<key_type, T>::iterator iter = mMap.find( aKey );
        if( iter != mMap.end() )
        {
            mMap.erase( iter );
            save();
        }
    }

    //--------------------------------------------------------------------
    //! \brief Check whether the map is empty
    //! \return true if there are no elements in the map, false otherwise
    //--------------------------------------------------------------------
    bool empty()
    {
        return mMap.empty();
    }

    //--------------------------------------------------------------------
    //! \brief Save the map to disk
    //--------------------------------------------------------------------
    void save()
    {
        std::ofstream outputStream;
        std::map<key_type, T>::iterator iter;

        outputStream.open( mFileName, std::ios::out );

        if( outputStream.good() )
        {
            mKeyList.clear();
            for( iter = mMap.begin(); iter != mMap.end(); ++iter )
            {
                if( iter->second.isValid() )
                {
                    iter->second.writeToStream( outputStream );
                    mKeyList.push_back( iter->first );
                }
            }
        }
        outputStream.close();
    }

    //--------------------------------------------------------------------
    //! \brief The number of items in the map
    //! \return The total number of items in the map
    //--------------------------------------------------------------------
    uint32 count() const
    {
        return mMap.count();
    }

    //--------------------------------------------------------------------
    //! \brief The number of valid items in the map
    //! \return The number of valid items in the map
    //--------------------------------------------------------------------
    uint32 validCount() const
    {
        return mKeyList.size();
    }

private:
    //--------------------------------------------------------------------
    //! \brief Read the contents of the backing file into the map
    //! \details Load all items from mFileName into mMap.
    //--------------------------------------------------------------------
    void load()
    {
        std::ifstream inputStream( mFileName, std::ios_base::in );
        mKeyList.clear();
        if( inputStream.good() )
        {
            inputStream.peek(); //need to do a read before eof registers
            while( !inputStream.eof() )
            {
                typename T item;
                item.setParent( this );
                item.readFromStream( inputStream );
                mMap[ item.getId() ] = item;
                if( item.isValid() )
                {
                    mKeyList.push_back( item.getId() );
                }
                inputStream.peek();
            }
        }
        inputStream.close();
    }

    //! The name of the file that this map is saved to
    const char* mFileName;

    //! The underlying map in memory
    std::map<key_type, T> mMap;

    //! Map of indexes to keys
    std::vector<key_type> mKeyList;
};

#endif
