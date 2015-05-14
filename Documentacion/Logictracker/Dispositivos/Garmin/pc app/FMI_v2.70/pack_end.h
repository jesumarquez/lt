/*********************************************************************
*
*   HEADER NAME:
*       pack_end.h - Specific compiler commands required to exit
*                    structure packing mode.
*
* Copyright 2008 by Garmin Ltd. or its subsidiaries.
*---------------------------------------------------------------------
* $NoKeywords$
*********************************************************************/

/* End of Win32 Visual C++ compiler packing */
#ifdef _WIN32
#pragma pack(pop)
#endif

/* End of Motorola 68K compiler packing */
#ifdef __MC68K__
#pragma options align=reset
#endif
