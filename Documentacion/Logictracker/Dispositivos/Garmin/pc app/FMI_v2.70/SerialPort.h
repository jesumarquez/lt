/*********************************************************************
*
*   HEADER NAME:
*       SerialPort.h
*
*   Copyright 2008-2009 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/
#ifndef SerialPort_H
#define SerialPort_H

class SerialPort;

#include <list>

#include "stdafx.h"
#include "garmin_types.h"
#include "TimerListener.h"
#include "PhysicalLayer.h"

class LinkLayer;

//----------------------------------------------------------------------
//! \brief Physical layer implementation for a serial port
//----------------------------------------------------------------------
class SerialPort : public PhysicalLayer, public TimerListener
{
public:
    static SerialPort * getInstance();
    static void destroyInstance();
    static void getPortList
        (
        std::list<CString> &aList
        );

    bool init
        (
        const CString& aPortName
        );

    bool setBaudRate
        (
        uint32 aBaudRate
        );

    const CString& getLastError() const;
    const CString& getPortName() const;

    bool isOpen() const;

    void onTimer();

    virtual bool tx
        (
        uint8 * aData,
        uint16  aSize
        );

protected:
    static bool getPortListFromRegistry
        (
        std::list<CString> &aList
        );

    static void getPortListEnum
        (
        std::list<CString> &aList
        );

    SerialPort();
    virtual ~SerialPort();

    void recordErrorText
        (
        const CString& aOperation
        );

private:
    void pumpRx();
    void close();

    //! The one and only instance of this object
    static SerialPort *     sInstance;

    //! File handle for the com port that is open, or INVALID_HANDLE_VALUE
    //!     if the port is not open
    HANDLE                  mComPortHandle;

    //! If TRUE, mComPortHandle is initialized.
    BOOL                    mHandleInitialized;

    //! Display name of the serial port being used for communication
    CString                 mPortName;

    //! String containing the last communication error.
    //! \note This is currently only used for errors reported by
    //!     SerialPort::init()
    CString                 mLastErrorText;
};

#endif
