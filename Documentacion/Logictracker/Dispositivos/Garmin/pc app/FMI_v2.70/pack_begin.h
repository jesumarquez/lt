/*********************************************************************
*
*   HEADER NAME:
*       pack_begin.h - Specific compiler commands required to enter
*                      structure packing mode.
*
* Copyright 2008 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

/* Motorola 68K compiler packing */
#ifdef __MC68K__
#pragma options align=packed
#endif

/* Win32 Visual C++ compiler packing */
#ifdef _WIN32
#define __packed
#pragma warning( disable : 4103 ) /* Disable warning about using pack pragma */
#pragma pack( push, 1 )
#endif
