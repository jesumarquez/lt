/*********************************************************************
*
*   MODULE NAME:
*       SerialPort.cpp
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

#include "stdafx.h"
#include "SerialPort.h"
#include "LinkLayer.h"
#include "Logger.h"

//! \brief The size of Windows' RX queue for the com port, in bytes
#define RX_QUEUE_SIZE       ( 0x4000 )

//! \brief The size of Windows' TX queue for the com port, in bytes
#define TX_QUEUE_SIZE       ( 0x1000 )

//! \brief The default baud rate for the serial port.
#define DEFAULT_BAUD_RATE   ( 9600 )

SerialPort * SerialPort::sInstance = NULL;

//----------------------------------------------------------------------
//! \brief Get the one and only serial port object
//! \return The serial port
//! \note SerialPort is a singleton because this application is
//!     designed to communicate with a single client device.  In a
//!     server-based application, there must be one serial port per
//!     connected client.
//----------------------------------------------------------------------
SerialPort * SerialPort::getInstance()
{
    if( sInstance == NULL )
    {
        sInstance = new SerialPort();
        atexit( SerialPort::destroyInstance );
    }

    return sInstance;
}

//----------------------------------------------------------------------
//! \brief Destroy the one and only serial port object
//----------------------------------------------------------------------
void SerialPort::destroyInstance()
{
    delete sInstance;
    sInstance = NULL;
}

//----------------------------------------------------------------------
//! \brief Get the list of serial ports
//! \param aList The list to store the port names.
//----------------------------------------------------------------------
void SerialPort::getPortList( std::list<CString> &aList )
{
    aList.clear();

    // Registry method is faster and gets all ports, when it works...
    if( !SerialPort::getPortListFromRegistry( aList ) )
    {
        // but if it doesn't, fall back to testing which ports can be opened
        aList.clear();
        SerialPort::getPortListEnum( aList );
    }
}

//----------------------------------------------------------------------
//! \brief Construct the SerialPort
//! \note The SerialPort is constructed as a closed port; init() must
//!    be called to open the port and begin communicating.
//----------------------------------------------------------------------
SerialPort::SerialPort()
{
    mComPortHandle = INVALID_HANDLE_VALUE;
    mLinkLayer     = NULL;
}

//----------------------------------------------------------------------
//! \brief Initializes the port passed in
//! \param port Null-terminated string containing the friendly name of
//!     the port (e.g., "COM1")
//! \return TRUE if the port was successfully opened and initialized,
//!     FALSE otherwise
//----------------------------------------------------------------------
bool SerialPort::init
    (
    const CString& port
    )
{
    COMMTIMEOUTS    timeouts;

    // Only clear the log if it isn't already open.
    // Reinitializing the com port shouldn't clear the log.
    if( !Logger::isLogOpen() )
        Logger::clearLog();

    close();

    CString portDeviceFile;
    portDeviceFile.Format( _T("\\\\.\\%s"), port );
    mComPortHandle = CreateFile
        (
        portDeviceFile.GetString(),
        GENERIC_READ | GENERIC_WRITE,
        0,         /* exclusive access                     */
        NULL,      /* no security attributes               */
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
        );

    if( mComPortHandle == INVALID_HANDLE_VALUE )
    {
        recordErrorText( _T("opening port") );
        return false;
    }

    if( !setBaudRate( DEFAULT_BAUD_RATE ) )
    {
        return false;
    }

    if( !SetupComm( mComPortHandle, RX_QUEUE_SIZE, TX_QUEUE_SIZE ) )
    {
        recordErrorText( _T("setting up buffers") );
        close();
        return false;
    }

    timeouts.ReadIntervalTimeout         = 0;
    timeouts.ReadTotalTimeoutMultiplier  = 0;
    timeouts.ReadTotalTimeoutConstant    = 0;
    timeouts.WriteTotalTimeoutMultiplier = 0;
    timeouts.WriteTotalTimeoutConstant   = 0;

    if( !SetCommTimeouts( mComPortHandle, &timeouts ) )
    {
        recordErrorText( _T("setting timeouts") );
        close();
        return false;
    }

    if( !PurgeComm( mComPortHandle, PURGE_RXABORT | PURGE_RXCLEAR | PURGE_TXABORT | PURGE_TXCLEAR ) )
    {
        recordErrorText( _T("clearing buffers") );
        close();
        return false;
    }

    mPortName = port;

    return true;
}

//----------------------------------------------------------------------
//! \brief Close the COM port if one is in use.
//! \details Close the handle for the com port.  After this is called,
//!     init() must be called to resume communication.
//----------------------------------------------------------------------
void SerialPort::close()
{
    if( isOpen() )
    {
        CloseHandle( mComPortHandle );
        mComPortHandle = INVALID_HANDLE_VALUE;
        mPortName = "";
    }
}

//----------------------------------------------------------------------
//! \brief Receive and process any data
//----------------------------------------------------------------------
void SerialPort::pumpRx()
{
    /*----------------------------------------------------------
    Local Variables
    ----------------------------------------------------------*/
    uint8                   readBuffer[ 256 ];
    int                     readSize;
    DWORD                   error;
    COMSTAT                 status;

    //check for error
    ClearCommError( mComPortHandle, &error, &status );
    if( error != 0 )
    {
        PurgeComm( mComPortHandle, PURGE_RXABORT | PURGE_RXCLEAR );
        return;
    }

    //check read size...if zero return because nothing to do
    readSize = minval( status.cbInQue, sizeof( readBuffer ) );
    if( readSize == 0 )
        return;

    if( ReadFile( mComPortHandle, readBuffer, readSize, (LPDWORD)&readSize, NULL ) == 0 )
    {
        ClearCommError( mComPortHandle, &error, &status );
        if( ( error & CE_OVERRUN ) ||
            ( error & CE_RXOVER  ) )
        {
            PurgeComm( mComPortHandle, PURGE_RXABORT | PURGE_RXCLEAR );
        }
        return;
    }
    if( readSize == 0 )
        return;

    // Push the received bytes up to the link layer
    if( mLinkLayer )
    {
        mLinkLayer->rx( readBuffer, readSize );
    }
}

//----------------------------------------------------------------------
//! \brief Transmit bytes on the serial port.
//! \param aData The bytes to transmit
//! \param aSize The number of bytes to transmit
//! \return true if the bytes were transmitted, false otherwise
//----------------------------------------------------------------------
bool SerialPort::tx
    (
    uint8  * aData,
    uint16   aSize
    )
{
    DWORD error;
    DWORD writeCount;
    COMSTAT status;

    ASSERT( mComPortHandle != INVALID_HANDLE_VALUE );
    ASSERT( aData != NULL );
    ASSERT( aSize != 0 );

    if( WriteFile( mComPortHandle, aData, aSize, (LPDWORD)&writeCount, NULL ) == 0 )
    {
        ClearCommError( mComPortHandle, &error, &status );
        if( error & CE_TXFULL )
        {
            PurgeComm( mComPortHandle, PURGE_TXABORT | PURGE_TXCLEAR );
        }
        return false;
    }
    return true;
}

//----------------------------------------------------------------------
//! \brief Get the list of com ports on the system by enumerating the
//!    device map in the Windows registry.
//! \param  aList The list of port names to append to
//! \return TRUE if successful, FALSE otherwise.  Note that a return
//!    value of FALSE does not mean aList is untouched; aList will
//!    have all of the port names that were found.
//! \note May require administrative privileges to open the registry
//----------------------------------------------------------------------
bool SerialPort::getPortListFromRegistry
    (
    std::list<CString> &aList
    )
{
    HKEY     hKey;             // handle for the key being queried
    DWORD    numValues;        // number of values for key
    DWORD    maxValueLen;      // longest value name
    DWORD    maxDataLen;       // longest value data
    DWORD    dataType;         // out: code indicating type of data
    BYTE     *dataBuf;         // out: data buffer
    DWORD    dataLen;          // out: number of bytes
    DWORD i, retCode;

    TCHAR  *valueBuf;
    DWORD  valueLen;

    bool retval = false;

    if( RegOpenKeyEx
        (
        HKEY_LOCAL_MACHINE,
        TEXT("HARDWARE\\DEVICEMAP\\SERIALCOMM"),
        0,
        KEY_READ,
        &hKey
        ) == ERROR_SUCCESS )
    {
        // Get the class name and the value count.
        retCode = RegQueryInfoKey
            (
            hKey,           // key handle
            NULL, NULL,     // buffer and size of class name, don't care
            NULL,           // reserved
            NULL, NULL,     // number and length of subkeys, don't care
            NULL,           // longest class string, don't care
            &numValues,     // number of values for this key
            &maxValueLen,   // longest value name
            &maxDataLen,    // longest value data
            NULL, NULL      // security descriptor, last write time, don't care
            );

        // Enumerate the values and data under this key.
        if( numValues )
        {
            valueBuf = new TCHAR[maxValueLen + 1];
            dataBuf  = new BYTE[maxDataLen + 1];
            retval = true;

            for( i = 0, retCode = ERROR_SUCCESS; i < numValues; i++ )
            {
                valueLen = maxValueLen + 1;
                dataLen = maxDataLen + 1;
                valueBuf[0] = '\0';
                retCode = RegEnumValue
                    (
                    hKey,
                    i,
                    valueBuf,
                    &valueLen,
                    NULL,
                    &dataType,
                    dataBuf,
                    &dataLen
                    );
                if( retCode == ERROR_SUCCESS )
                {
                    if( dataType == REG_SZ )
                        aList.push_back( CString( (TCHAR*)dataBuf ) );
                }
                else
                    retval = false;

            } // end of for loop

            delete[] valueBuf;
            delete[] dataBuf;
        } // end of if( numValues )

    } // end of if( RegOpenKey )

    RegCloseKey( hKey );
    return retval;
}

//----------------------------------------------------------------------
//! \brief Find the COM ports on the system by opening each in turn
//! \param aList The list to append port names to.
//----------------------------------------------------------------------
void SerialPort::getPortListEnum
    (
    std::list<CString> &aList
    )
{
    getInstance()->close();

    for( int portNum = 1; portNum <= 256; portNum++ )
    {
        CString portDeviceFile;  // Device filename
        CString portName;        // Display/"friendly" name
        HANDLE hPort;
        portName.Format( _T("COM%u"), portNum );
        portDeviceFile.Format( _T("\\\\.\\%s"), portName );

        hPort = CreateFile
                    (
                    portDeviceFile,
                    GENERIC_READ | GENERIC_WRITE,
                    0,
                    NULL,
                    OPEN_EXISTING,
                    0,
                    NULL
                    );
        if( hPort != INVALID_HANDLE_VALUE )
        {
            aList.push_back( portName );
            ::CloseHandle( hPort );
        }
#if defined( _DEBUG )
        else
        {
            TRACE( "Failed to open port %s, error was %d\n", portName.GetBuffer(), GetLastError() );
        }
#endif
    }
}

//----------------------------------------------------------------------
//! \brief Return a description of the last error that occurred.
//----------------------------------------------------------------------
const CString& SerialPort::getLastError() const
{
    return mLastErrorText;
}

//----------------------------------------------------------------------
//! \brief Timer callback.
//! \details If the serial port is open, receive (and process) anything
//!    that is ready to receive.
//----------------------------------------------------------------------
void SerialPort::onTimer()
{
    if( mComPortHandle != INVALID_HANDLE_VALUE )
        pumpRx();
}

//----------------------------------------------------------------------
//! \brief Destructor.  Close the serial port.
//----------------------------------------------------------------------
SerialPort::~SerialPort()
{
    if( mLinkLayer )
    {
        mLinkLayer->setPhysicalLayer( NULL );
    }
    close();
}

//----------------------------------------------------------------------
//! \brief Store a textual description of the last error that occurred.
//! \param aOperation The name of the operation that caused the error
//----------------------------------------------------------------------
void SerialPort::recordErrorText
    (
    const CString& aOperation
    )
{
    LPTSTR pszMessage;
    DWORD  errorCode = GetLastError();

    ASSERT( errorCode != ERROR_SUCCESS );

    FormatMessage
        (
        FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        errorCode,
        MAKELANGID( LANG_NEUTRAL, SUBLANG_DEFAULT ),
        (LPTSTR)&pszMessage,
        0,
        NULL
        );
    mLastErrorText.Format( _T("%s: %s"), aOperation, CString( pszMessage ) );
    LocalFree( pszMessage );
}

//----------------------------------------------------------------------
//! \brief Get the name of the serial port that is open.
//! \return The name of the serial port that is open, or an empty
//!     string if the port is closed.
//----------------------------------------------------------------------
const CString& SerialPort::getPortName() const
{
    return mPortName;
}

//----------------------------------------------------------------------
//! \brief Indicate whether the port is open.
//! \return true if the serial port is open, false otherwise.
//----------------------------------------------------------------------
bool SerialPort::isOpen() const
{
    return mComPortHandle != INVALID_HANDLE_VALUE;
}

//----------------------------------------------------------------------
//! \brief Set the baud rate.
//! \param aBaudRate The new baud rate for the port, in bps.
//! \
//----------------------------------------------------------------------
bool SerialPort::setBaudRate
    (
    uint32 aBaudRate
    )
{
    DCB             dcb;

    ASSERT( mComPortHandle != INVALID_HANDLE_VALUE );

    dcb.DCBlength = sizeof( DCB );
    if( !GetCommState( mComPortHandle, &dcb ) )
    {
        recordErrorText( _T("getting port state") );
        close();
        return false;
    }

    dcb.fBinary             = TRUE;
    dcb.BaudRate            = aBaudRate;
    dcb.ByteSize            = 8;
    dcb.StopBits            = ONESTOPBIT;
    dcb.Parity              = NOPARITY;
    dcb.fParity             = FALSE;
    dcb.fAbortOnError       = FALSE;
    dcb.fOutxCtsFlow        = FALSE;
    dcb.fOutxDsrFlow        = FALSE;
    dcb.fDtrControl         = DTR_CONTROL_DISABLE;
    dcb.fRtsControl         = RTS_CONTROL_DISABLE;
    dcb.fDsrSensitivity     = FALSE;
    dcb.fTXContinueOnXoff   = FALSE;
    dcb.fOutX               = FALSE;
    dcb.fInX                = FALSE;
    dcb.fNull               = FALSE;

    if( !SetCommState( mComPortHandle, &dcb ) )
    {
        recordErrorText( _T("setting serial port parameters") );
        close();
        return false;
    }

    return true;
}
