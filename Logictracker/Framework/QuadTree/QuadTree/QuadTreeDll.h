#pragma once
#include "QuadTree.h"

#ifdef QUADTREE_EXPORTS
#define QUADTREE_API __declspec(dllexport)
#else
#define QUADTREE_API __declspec(dllimport)
#endif

#pragma pack(push, 1)
struct QUADTREE_API GridGeometry
{ 
	char Signature[16]; /* Descripcion/Identificacion del respositorio */
	char CellBits; /* Cantidad de bits de resolicion por celda (actualmente fijo 4)*/
	unsigned long ActiveKey; /* Código de activación = 0x55AA55AA */ 
	unsigned long DataStart; /* Inicio de datos de las zonas en sectores, nunca menor a 1 */ 
	unsigned long FileSectorCount; /* Cantidad de Sectores por Fila, debe ser mayor o igual a (Lon_GridCount / 32) teniendo en cuenta el remanente */ 
	long Lat_OffSet; /* OffSet de Latitud en 1/10,000,000 de Grados */ 
	long Lon_OffSet; /* OffSet de Longitud en 1/10,000,000 de Grados */ 
	unsigned long Lat_Grid; /* Tamaño de la Grilla de Latitud en 1/10,000,000 de Grados */ 
	unsigned long Lon_Grid; /* Tamaño de la Grilla de Longitud en 1/10,000,000 de Grados */ 
	unsigned long Lat_GridCount; /* Cantidad de Grillas en Latitud */ 
	unsigned long Lon_GridCount; /* Cantidad de Grillas en Longitud */ 
}; 
#pragma pack(pop)

class QUADTREE_API QuadTree
{
	
private:
	HANDLE hTRLOG;
	bool protected_mode;
	short cache_lat;
	short cache_lon;
	QuatTreeGR2 cached_gr2;
	QuatTreeExtended cached_extended;
public:
	QuadTree(void);
	virtual ~QuadTree(void);

	// manipulacion del repositorio con medidas SEXAGESIMALES
	int Init(const char * basedir);
	int Open(const char * basedir);
	// manipulacion del repositorio con medidas DECIMALES
	int InitEx(const char * basedir, GridGeometry & geometry) { return 0; }
	int OpenEx(const char * basedir, GridGeometry & geometry) { return 0; }
	// manupulacion del respositorio generica
	void Close();

	// manipulacion del lockeo.
	void ProtectedMode(bool enabled);
	
	// manipulacion de posiciones.
	int GetPositionClass(float lat, float lon);
	bool SetPositionClass(float lat, float lon, int _class, char zone = 'R');
	char GetPositionZone(float lat, float lon);
	void TrLog(const char * action, const char * data);
private:
	void RefreshCache(float lat, float lon);
	int logs;
	int active_revision;
	int transitions;
	string basedir;
};

extern "C" {
QUADTREE_API int Q43_Create(HANDLE* handle);
QUADTREE_API int Q43_Destroy(HANDLE handle);
QUADTREE_API int Q43_Init(HANDLE handle , const char * basedir);
QUADTREE_API int Q43_Open(HANDLE handle ,const char * basedir);
QUADTREE_API void Q43_Close(HANDLE handle);
QUADTREE_API int Q43_GetPositionClass(HANDLE handle ,float lat, float lon);
QUADTREE_API bool Q43_SetPositionClass(HANDLE handle ,float lat, float lon, int _class, char zone);
QUADTREE_API char Q43_GetPositionZone(HANDLE handle ,float lat, float lon);
QUADTREE_API void Q43_TrLog(HANDLE handle ,const char * action, const char * data);
QUADTREE_API void Q43_TrLog(HANDLE handle ,const char * action, const char * data);
QUADTREE_API void Q43_ProtectedMode(HANDLE handle, bool enabled);
}