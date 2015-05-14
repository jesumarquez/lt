// QuadTree.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "QuadTreeDll.h"


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

QUADTREE_API int Q43_Create(HANDLE* handle)
{
	QuadTree* rv = new QuadTree();
	*handle = static_cast<HANDLE>(rv);
	return 0;
}

QUADTREE_API int Q43_Destroy(HANDLE handle)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	delete rv;
	return 0;
}


QUADTREE_API int Q43_Init(HANDLE handle , const char * basedir)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->Init(basedir);
}

QUADTREE_API int Q43_Open(HANDLE handle ,const char * basedir)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->Open(basedir);
}


QUADTREE_API void Q43_Close(HANDLE handle)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->Close();
}

QUADTREE_API void Q43_ProtectedMode(HANDLE handle, bool enabled)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->ProtectedMode(enabled);
}

QUADTREE_API int Q43_GetPositionClass(HANDLE handle ,float lat, float lon)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->GetPositionClass(lat,lon);
}

QUADTREE_API char Q43_GetPositionZone(HANDLE handle ,float lat, float lon)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->GetPositionZone(lat,lon);
}

QUADTREE_API bool Q43_SetPositionClass(HANDLE handle ,float lat, float lon, int _class, char zone)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	return rv->SetPositionClass(lat,lon,_class, zone);
}

QUADTREE_API void Q43_TrLog(HANDLE handle ,const char * action, const char * data)
{
	QuadTree* rv = static_cast<QuadTree*>(handle);
	rv->TrLog(action,data);
}