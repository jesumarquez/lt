/*********************************************************************
*
*   HEADER NAME:
*       util_macros.h
*
* Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef UTIL_MACROS_H
#define UTIL_MACROS_H

//----------------------------------------------------------------------
//! \file util_macros.h
//! \brief Basic utility macros for low-level functions.
//----------------------------------------------------------------------

//----------------------------------------------------------------------
//! \brief A bit mask with several bits set and the rest cleared
//! \param _b The lowest bit number that is set, counting from 0 as
//!     the LSB.
//! \param _len The number of bits that are set.
//----------------------------------------------------------------------
#define setbits( _b, _len )             ( ( ( 1U << ( (_len) - 1 ) ) - 1U + ( 1U << ( (_len) - 1 ) ) ) << (_b) )

//----------------------------------------------------------------------
//! \brief A bit mask with one bit set and the rest cleared
//! \param _b The bit number that is set, counting from 0 as the LSB.
//----------------------------------------------------------------------
#define setbit( _b )                    ( (unsigned)1 << (_b) )

//----------------------------------------------------------------------
//! \brief A bit mask with several bits cleared and the rest set
//! \param _b The lowest bit number that is cleared, counting from 0
//!     as the LSB.
//! \param _len The number of bits that are cleared.
//----------------------------------------------------------------------
#define clrbits( _b, _len)              ( ~setbits( (_b), (_len) ) )

//----------------------------------------------------------------------
//! \brief A bit mask with one bit cleared and the rest set
//! \param _b The bit number that is cleared, counting from 0 as the
//!     LSB.
//----------------------------------------------------------------------
#define clrbit( _b )                    ( ~setbit( _b ) )

//----------------------------------------------------------------------
//! \brief True if all mask bits are set in the specified value
//! \param _val The value to test
//! \param _mask The mask bits to check
//----------------------------------------------------------------------
#define allbitset( _val, _mask )        ( ( (_val) & (_mask) ) == (_mask) )

//----------------------------------------------------------------------
//! \brief True if any mask bits are set in the specified value
//! \param _val The value to test
//! \param _mask The mask bits to check
//----------------------------------------------------------------------
#define anybitset( _val, _mask )        ( ( (_val) & (_mask) )  != 0 )

//----------------------------------------------------------------------
//! \brief The maximum signed integer that can be stored in a type
//! \param _t The data type to test.
//----------------------------------------------------------------------
#define max_sint_val( _t )              ( setbits( 0, ( sizeof( _t ) * 8 ) - 1 ) )

//----------------------------------------------------------------------
//! \brief The minimum signed integer that can be stored in a type
//! \param _t The data type to test.
//----------------------------------------------------------------------
#define min_sint_val( _t )              ( ~max_sint_val( _t ) )

//----------------------------------------------------------------------
//! \brief The maximum unsigned integer that can be stored in a type
//! \param _t The data type to test.
//----------------------------------------------------------------------
#define max_uint_val( _t )              ( setbits( 0, ( sizeof( _t ) * 8 ) ) )

//----------------------------------------------------------------------
//! \brief The offset of _m from the beginning of _s
//! \param _s the name of the struct
//! \param _m the name of the member
//----------------------------------------------------------------------
#define offset_of( _s, _m )             ( (unsigned char *)&( ( (_s *)0 )->_m ) - (unsigned char *)0 )

//----------------------------------------------------------------------
//! \brief The number of elements in _a
//! \param _a the name of the array
//----------------------------------------------------------------------
#define cnt_of_array( _a )             ( sizeof( (_a) ) / sizeof( (_a)[0] ) )

//----------------------------------------------------------------------
//! \brief The smaller of _x and _y
//----------------------------------------------------------------------
#define minval( _x, _y )                ( (_x) < (_y) ? (_x) : (_y) )

//----------------------------------------------------------------------
//! \brief The larger of _x and _y
//----------------------------------------------------------------------
#define maxval( _x, _y )                ( (_x) > (_y) ? (_x) : (_y) )

//----------------------------------------------------------------------
//! \brief Return if a condition is true
//! \param _check The boolean expression to check
//----------------------------------------------------------------------
#define returnif( _check )              \
    {                                   \
    if( _check )                        \
        {                               \
        return;                         \
        }                               \
    }   /* returnif */

//----------------------------------------------------------------------
//! \brief Return a value if a condition is true
//! \param _check The boolean expression to check
//! \param _value The value to return
//----------------------------------------------------------------------
#define returnif_v( _check, _value )    \
    {                                   \
    if( _check )                        \
        {                               \
        return( _value );               \
        }                               \
    }   /* returnif */

//----------------------------------------------------------------------
//! \brief Concatenate two tokens.  Do not use directly.
//! \details Use tokcat() and not _tokcat().  _tokcat() only exists to
//!     allow tokcat() to get around preprocessor substitution rules.
//! \param _x The first token
//! \param _y The second token
//----------------------------------------------------------------------
#define _tokcat( _x, _y )               _x ## _y

//----------------------------------------------------------------------
//! \brief Concatenate two tokens.
//! \details tokcat() returns a single C token that is the result of
//!     concatenating two tokens.  For example, if
//!     "tokcat( _, __LINE__)" appears at line 123, it will be
//!     replaced with "_123" by the preprocessor.  tokcat() is
//!     analogous to strcat().
//! \param _x The first token
//! \param _y The second token
//----------------------------------------------------------------------
#define tokcat( _x, _y )                _tokcat( _x, _y )

//----------------------------------------------------------------------
//! \brief Compile time assert
//! \details Generate compiler error if _e is 0/false
//! \param _e The value to assert.  This must evaluate to a
//!     compile-time constant.
//! \param _m A valid C identifier that represents the module name.
//----------------------------------------------------------------------
#define _compiler_assert( _e, _m ) \
    struct tokcat( __, tokcat( _m, __LINE__ ) ) { int _ : ( (_e) ? 1 : 0 ); }

//----------------------------------------------------------------------
//! \brief Compile time assert
//! \details Generate compiler error if _e is 0/false
//! \param _e The value to assert.  This must evaluate to a
//!     compile-time constant.
//----------------------------------------------------------------------
#define compiler_assert( _e )           _compiler_assert( _e, _UTIL_H_ )

#endif
