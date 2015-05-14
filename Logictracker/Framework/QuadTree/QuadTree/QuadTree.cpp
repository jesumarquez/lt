/* $Id: QuadTree.cpp 3976 2009-06-14 23:58:43Z joseg $ */
#include "StdAfx.h"
#include "QuadTree.h"
#include <string.h>
#include <string>
#include <sstream>
#include "QuadTreeDll.h"

using namespace std;

#ifdef _DEBUG
void trace_printf(char * format, ...)
{
	char buffer[256];
	char buffer2[256];
	va_list args;
	va_start (args, format);
	vsprintf (buffer, format, args);
	va_end (args);
	sprintf(buffer2,"%s\r\n",buffer);
	printf(buffer2);
}
#else
void trace_printf(char * format, ...)
{
}
#endif


#define QUADTREETRACE trace_printf


QuadTree::QuadTree(void)
{
	cache_lat = 0;
	cache_lon = 0;
	logs = 0;
	cached_gr2.qTree = this;
	protected_mode = false;
}

QuadTree::~QuadTree(void)
{
}

#pragma pack(1)
struct gr2_header {
	char signature[8];
	float lat_base;
	float lat_limit;
	float lon_base;
	float lon_limit;
};
#pragma pack()

void QuadTree::ProtectedMode(bool enabled) {
	protected_mode = enabled;
}

int QuadTree::Init(const char * basedir) {
	ostringstream o;
	o << basedir << "\\transaction.log";
	if (::CreateDirectory(basedir, NULL)) {
		// ahora creo el log transaccional.
		//HANDLE hTR = ::CreateFile(o.str().c_str(), 
		HANDLE hTRLOG = ::CreateFile(o.str().c_str(), 
			GENERIC_WRITE,
			0,
			0,
			CREATE_NEW,
			FILE_ATTRIBUTE_NORMAL, 0);

		if ( hTRLOG == INVALID_HANDLE_VALUE ) 
		{
			return ::GetLastError();
		}

		ostringstream tl;
		tl << "GR2-1.1:REV=000000000\r\n";

		string body = tl.str();
		DWORD len = body.length();
		DWORD wr = 0;

		if (!::WriteFile(hTRLOG, body.c_str(), len, &wr, NULL)) {
			return ::GetLastError();	
		}
		if (::CloseHandle(hTRLOG)) {
			return 0;
		}
	}
	return ::GetLastError();
}

int QuadTree::Open(const char * _basedir) {
	basedir = _basedir;
	active_revision = -1;
	ostringstream o;
	o << basedir << "\\transaction.log";
	hTRLOG = ::CreateFile(o.str().c_str(),GENERIC_READ | GENERIC_WRITE,FILE_SHARE_READ
		,NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL , NULL);
	
	if (hTRLOG == INVALID_HANDLE_VALUE) {
		QUADTREETRACE("Open: no se puede abrir TRLOG.");
		return ::GetLastError();
	}

	int	res = ::SetFilePointer(hTRLOG, -15, 0, FILE_END);
	if (res == INVALID_SET_FILE_POINTER) {
		QUADTREETRACE("Open: no se puede mover el cursor al final del TRLOG.");
		return ::GetLastError();
	}
	char buffer[32];
	DWORD readed;
	if (!::ReadFile(hTRLOG, buffer, 32, &readed, NULL)) {
		QUADTREETRACE("Open: error de lectura del TRLOG.");
		return ::GetLastError();
	}
	
	if (buffer[0] != 'R' || buffer[1] != 'E' || buffer[2] != 'V') {
		QUADTREETRACE("Open: No parece un repositorio valido..");
		return ::GetLastError();
	}
	
	buffer[13] = 0;
	active_revision = atoi(buffer+4);
	active_revision++;
	transitions = 0;
	QUADTREETRACE("Revision Activa: [%d]", active_revision);

	return 0;
}

void QuadTree::Close() {
	cached_gr2.Close();
	::CloseHandle(hTRLOG);

	cache_lat = 0;
	cache_lon = 0;
	logs = 0;
}

int QuadTree::GetPositionClass(float lat, float lon) 
{
	RefreshCache(lat, lon);
	return cached_gr2.GetPositionClass(lat,lon);
}

char QuadTree::GetPositionZone(float lat, float lon) 
{
	RefreshCache(lat, lon);
	qtree_file_extended_2s2 extended;
	if (!cached_extended.GetPositionExtended(lat, lon, extended)) {
		throw new exception("imposible obtener datos extendidos");
	}
	return extended.zone;
}

bool QuadTree::SetPositionClass(float lat, float lon, int _class, char zone)
{
	RefreshCache(lat, lon);
	qtree_file_extended_2s2 extended;
	if (!cached_extended.GetPositionExtended(lat, lon, extended)) {
		throw new exception("imposible obtener datos extendidos");
	}
	if (protected_mode) {
		if ((extended.flags & 0x01) == 0x01) {
			// el registro esta bloqueado, y es modo protegido, se ignora.
			return true;
		}
	} else {
		extended.flags |= 0x01;
		extended.zone = zone;
	}
	if (!cached_extended.SetPositionExtended(lat, lon, extended)) {
		throw new exception("imposible grabar datos extendidos");
	}
	return cached_gr2.SetPositionClass(lat, lon, _class);
}

void QuadTree::RefreshCache(float lat, float lon) 
{
	short gr2_lat = (short) lat;
	short gr2_lon = (short) lon;

	if (gr2_lat != cache_lat || gr2_lon != cache_lon) {
		ostringstream filename;
		filename << basedir << "\\M2-" << gr2_lat << "-" << gr2_lon << ".GR2";

		cached_gr2.Close();
		if (cached_gr2.Load(filename.str(), gr2_lat, gr2_lon) != 0) {
			cached_gr2.Create(filename.str(), gr2_lat, gr2_lon);
			ostringstream logfilename;
			logfilename << "M2-" << gr2_lat << "-" << gr2_lon << ".GR2";
			TrLog("ADD-GR2",logfilename.str().c_str());
			cached_gr2.Close();
			if (cached_gr2.Load(filename.str(), gr2_lat, gr2_lon) != 0) {
				throw new exception("imposible crear archivo GR2");
			}
		}

		ostringstream filename_ex;
		filename_ex << basedir << "\\M2-" << gr2_lat << "-" << gr2_lon << ".GX2";
		cached_extended.Close();
		if (cached_extended.Load(filename_ex.str(), gr2_lat, gr2_lon) != 0) {
			cached_extended.Create(filename_ex.str(), gr2_lat, gr2_lon);
			cached_extended.Close();
			if (cached_extended.Load(filename_ex.str(), gr2_lat, gr2_lon) != 0) {
				throw new exception("imposible crear archivo GX2");
			}
		}

		cache_lat = gr2_lat;
		cache_lon = gr2_lon;
		return;
	}
}

void QuadTree::TrLog(const char * action, const char * data) {
	char buffer[512];
	if (transitions++ >= MAX_REPO_TRANSITIONS) {
		transitions = 1;
		active_revision++;
	}
	sprintf(buffer,"T:%s:%s:REV=%09d\r\n", action, data,active_revision);
	DWORD len = strlen(buffer);
	DWORD wr;
	if (!::WriteFile(hTRLOG, buffer, len, &wr, NULL)) {
			throw new exception("imposible grabar revision de TRLOG");
	}
	logs++;	
}


///////////////////////////////////////////////////////////////////////////////////
//// GR2 FILE

QuatTreeGR2::QuatTreeGR2() {
	hFile = 0;
	qTree = 0;
	cached_sector_num = -1;
	cached_sector_modified = false;
	Close();		
}

int QuatTreeGR2::Load(string filename, short lat, short lon)
{
	if (hFile != 0) {
		throw new exception("DEV_QTREE: antes de cargar otro GR2, se debe descargar el anterior.");
	}

	hFile = ::CreateFile(filename.c_str(),GENERIC_READ | GENERIC_WRITE,FILE_SHARE_READ
		,NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	
	if (hFile == INVALID_HANDLE_VALUE) {
		QUADTREETRACE("Load: no se puede abrir GR2 %s.", filename.c_str());
		return -1;
	}

	int	res = ::SetFilePointer(hFile, 0, 0, FILE_BEGIN);
	if (res == INVALID_SET_FILE_POINTER) {
		QUADTREETRACE("Load: no se puede mover el cursor al inicio de %s.", filename.c_str());
		return ::GetLastError();
	}

	union sector_t {
		char block[512];
		qtree_file_headers hdrs;
	} sector;

	DWORD readed;
	if (!::ReadFile(hFile, &sector, 512, &readed, NULL)) {
		QUADTREETRACE("Load: error de lectura de GR2[%s].",filename.c_str());
		return ::GetLastError();
	}

	QUADTREETRACE("GR2 LOADED: [%s] LAT=%d LON=%d REV=%d", filename.c_str(), sector.hdrs.base_lat, sector.hdrs.base_lon,
		sector.hdrs.revision);

	base_lat = sector.hdrs.base_lat;
	base_lon = sector.hdrs.base_lon;

	return 0;
}

void QuatTreeGR2::Close()
{
	if (cached_sector_modified) {
		if (cached_sector_num != 0)
			RefreshCache(0);
		else
			RefreshCache(1);
	}
	if (hFile != 0) {
		CloseHandle(hFile);
		hFile = 0;
	}
	cached_sector_num = -1;
	cached_sector_modified = false;
	memset(cached_sector.data, 0, 512);
	base_lat = 0;
	base_lon = 0;
}
	
int QuatTreeGR2::GetPositionClass(float lat, float lon)
{
	int sector_num;
	int byte;
	bool low_bits;

	if (!GetFileSector(lat, lon, sector_num, byte, low_bits))
	{
		QUADTREETRACE("GR2: imposible obtener el sector.");
		return false;
	}

	if (!RefreshCache(sector_num)) {
		QUADTREETRACE("GR2: error intercambiando cache.");
		return false;
	}
	
	char active_byte = cached_sector.m2.data[byte];

	if (low_bits) {
		return active_byte & 0x0F;
	}

	return (active_byte >> 4) & 0x0F;
}

bool QuatTreeGR2::SetPositionClass(float lat, float lon, int _class)
{
	int sector_num;
	int byte;
	bool low_bits;

	if (!GetFileSector(lat, lon, sector_num, byte, low_bits))
	{
		QUADTREETRACE("GR2: imposible obtener el sector.");
		return false;
	}

	if (!RefreshCache(sector_num)) {
		QUADTREETRACE("GR2: error intercambiando cache.");
		return false;
	}
	
	cached_sector_modified = true;

	if (low_bits) {
		char src_class = (char) _class & 0xF;
		src_class |= (cached_sector.m2.data[byte] & 0xF0);
		cached_sector.m2.data[byte] = src_class;
	} else {
		char src_class = (char)((_class & 0xF) << 4);
		src_class |= (cached_sector.m2.data[byte] & 0x0F);
		cached_sector.m2.data[byte] = src_class;
	}
	return true;
}

int QuatTreeGR2::Create(string filename, short lat, short lon) 
{
	HANDLE hCR = ::CreateFile(filename.c_str(), 
			GENERIC_WRITE,
			0,
			0,
			CREATE_NEW,
			FILE_ATTRIBUTE_NORMAL, 0);

	if ( hCR == INVALID_HANDLE_VALUE ) 
	{
		return ::GetLastError();
	}
	
	union sector_t {
		char block[512];
		qtree_file_headers hdrs;
	} sector;

	memset(sector.block, 0, 512);
	
	sector.hdrs.signature[0] = 'Q';
	sector.hdrs.signature[1] = 'F';
	sector.hdrs.signature[2] = '-';
	sector.hdrs.signature[3] = 'v';
	sector.hdrs.signature[4] = '0';
	sector.hdrs.signature[5] = '.';
	sector.hdrs.signature[6] = '5';
	sector.hdrs.signature[7] = 0;

	sector.hdrs.status = 1; // marco como formateado.
	sector.hdrs.revision = 0; // version original
	sector.hdrs.base_lat = lat; // asigno posicionamiento.
	sector.hdrs.base_lon = lon;

	DWORD wr = 0;

	if (!::WriteFile(hCR, &sector, 512, &wr, NULL)) {
		return ::GetLastError();
	}

	memset(sector.block, 0, 512);

	for(int y=0; y < 60; y++) {
		for(int x=0; x < 60; x++) {
			int sect = (y * 60) + x + 1;
			if (!::WriteFile(hCR, &sector, 512, &wr, NULL)) {
				QUADTREETRACE("Imposible iniciar sector %d", sect);
				return ::GetLastError();
			}
			//TRACE("iniciando sector %d", sect);
		}
	}

	QUADTREETRACE("GR2 CREATED: [%s] LAT=%d LON=%d REV=%d", filename.c_str(), sector.hdrs.base_lat, sector.hdrs.base_lon,
		sector.hdrs.revision);

	if (::CloseHandle(hCR)) {
		return 0;
	}

	return ::GetLastError();
}

bool QuatTreeGR2::RefreshCache(int sector_num) 
{
	if (cached_sector_num != sector_num) {
		if (cached_sector_num != -1 && cached_sector_modified) {
			QUADTREETRACE("StoreM2: salvando sector %d.", cached_sector_num);
			LONG store_offset = cached_sector_num * 512;
			int	res = ::SetFilePointer(hFile, (store_offset+512), 0, FILE_BEGIN);
			if (res == INVALID_SET_FILE_POINTER) {
				QUADTREETRACE("StoreM2: imposible mover puntero al sector %d.", cached_sector_num);
				return false;
			}
			DWORD wr = 0;
			if (!::WriteFile(hFile, &cached_sector, 512, &wr, NULL)) {
				QUADTREETRACE("StoreM2: Imposible salvar sector %d", cached_sector_num);
				return false;
			}
			char buffer[128];			
			sprintf(buffer, "FILE=M2-%d-%d.GR2:SECTOR=%d", base_lat, base_lon, cached_sector_num);
			qTree->TrLog("SET-M2",buffer);
		}

		LONG offset = sector_num * 512;
		int	res = ::SetFilePointer(hFile, (offset+512), 0, FILE_BEGIN);
		if (res == INVALID_SET_FILE_POINTER) {
			QUADTREETRACE("LoadM2: no se puede mover el cursor al sector %d.", sector_num);
			return false;
		}
		DWORD readed;
		if (!::ReadFile(hFile, &cached_sector, 512, &readed, NULL)) {
			QUADTREETRACE("LoadM2: error de lectura de sector %d.",sector_num);
			return false;
		}
		QUADTREETRACE("LoadM2: sector %d cargado.", sector_num);
		cached_sector_num = sector_num;
		cached_sector_modified = false;
	}
	return true;
}

#define ABS(d) if (d < 0) d = d * -1;

bool QuatTreeGR2::GetFileSector(float lat, float lon, int & sector, int & byte, bool & low_bits) 
{
	ABS(lat);
	ABS(lon);

	QUADTREETRACE("calculando sector de posicion %f %f", lat, lon);

	int gr_lat = (int)lat;
	int gr_lon = (int)lon;

	if (base_lat != gr_lat || base_lon != gr_lon) {
		QUADTREETRACE("GR2 %d %d ", base_lat, base_lon);
		QUADTREETRACE("esta posicion no pertenece a este archivo %f %f", lat, lon);
		return false;
	}

	int min_lat = (int)((lat - gr_lat) * 60);
	int min_lon = (int)((lon - gr_lon) * 60);

	int sec_lat = (int)((lat - gr_lat - (min_lat/60.0)) * 3600);
	int sec_lon = (int)((lon - gr_lon - (min_lon/60.0)) * 3600);
	
	int norm_slat = sec_lat >> 1; // div 2
	int norm_slon = sec_lon >> 1; // div 2

	sector = min_lat * 60 + min_lon;
	byte = norm_slat * 15 + (norm_slon/2);
	low_bits = norm_slon % 2;
	
	QUADTREETRACE("Latitude %dº %d' %d\" base=%d" , gr_lat, min_lat, sec_lat, min_lat * 60);
	QUADTREETRACE("Longitude %dº %d' %d\" base=%d" , gr_lon, min_lon, sec_lon, min_lon);
	QUADTREETRACE("sector = %d / byte = %d / en los 4 bits %s", sector, byte, (low_bits == 1 ? "bajos" : "altos"));
			
	return true;
}

/////////////////////////////////////////////////////////////////////////////////////
//// QTREE EXTENDED

QuatTreeExtended::QuatTreeExtended()
{
	hFile = 0;
	qTree = 0;
	cached_cluster_num = -1;
	cached_cluster_modified = false;
	Close();
}

int QuatTreeExtended::Load(string filename, short lat, short lon)
{
	if (hFile != 0) {
		throw new exception("DEV_QTREE: antes de cargar otro GR2-Ex, se debe descargar el anterior.");
	}

	hFile = ::CreateFile(filename.c_str(),GENERIC_READ | GENERIC_WRITE,FILE_SHARE_READ
		,NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	
	if (hFile == INVALID_HANDLE_VALUE) {
		QUADTREETRACE("Load: no se puede abrir GR2-Ex %s.", filename.c_str());
		return -1;
	}

	int	res = ::SetFilePointer(hFile, 0, 0, FILE_BEGIN);
	if (res == INVALID_SET_FILE_POINTER) {
		QUADTREETRACE("Load: no se puede mover el cursor al inicio de %s.", filename.c_str());
		return ::GetLastError();
	}

	union cluster_t {
		char block[Q43_EXTENDED_CLUSTER_SIZE];
		qtree_file_headers hdrs;
	} cluster;

	DWORD readed;
	if (!::ReadFile(hFile, &cluster, Q43_EXTENDED_CLUSTER_SIZE, &readed, NULL)) {
		QUADTREETRACE("Load: error de lectura de GR2ex[%s].",filename.c_str());
		return ::GetLastError();
	}

	QUADTREETRACE("GR2ex LOADED: [%s] LAT=%d LON=%d REV=%d", filename.c_str(), cluster.hdrs.base_lat, cluster.hdrs.base_lon,
		cluster.hdrs.revision);

	base_lat = cluster.hdrs.base_lat;
	base_lon = cluster.hdrs.base_lon;

	return 0;
}

void QuatTreeExtended::Close()
{
	if (cached_cluster_modified) {
		if (cached_cluster_num != 0)
			RefreshCache(0);
		else
			RefreshCache(1);
	}
	if (hFile != 0) {
		CloseHandle(hFile);
		hFile = 0;
	}
	cached_cluster_num = -1;
	cached_cluster_modified = false;
	memset(cached_cluster.data, 0, Q43_EXTENDED_CLUSTER_SIZE);
	base_lat = 0;
	base_lon = 0;
}
	
bool QuatTreeExtended::GetPositionExtended(float lat, float lon, qtree_file_extended_2s2 & result)
{
	int cluster_num;
	int index;

	if (!GetFileCluster(lat, lon, cluster_num, index))
	{
		QUADTREETRACE("GR2EX: imposible obtener el cluster.");
		return false;
	}

	if (!RefreshCache(cluster_num)) {
		QUADTREETRACE("GR2ex: error intercambiando cache.");
		return false;
	}
	
	result = cached_cluster.m2.sq_seconds[index];
	return true;
}

bool QuatTreeExtended::SetPositionExtended(float lat, float lon, qtree_file_extended_2s2 newvalue)
{
	int cluster_num;
	int index;

	if (!GetFileCluster(lat, lon, cluster_num, index))
	{
		QUADTREETRACE("GR2EX: imposible obtener el cluster.");
		return false;
	}

	if (!RefreshCache(cluster_num)) {
		QUADTREETRACE("GR2ex: error intercambiando cache.");
		return false;
	}
	
	cached_cluster_modified = true;
	cached_cluster.m2.sq_seconds[index] = newvalue;
	
	return true;
}

int QuatTreeExtended::Create(string filename, short lat, short lon)
{
	HANDLE hCR = ::CreateFile(filename.c_str(), 
			GENERIC_WRITE,
			0,
			0,
			CREATE_NEW,
			FILE_ATTRIBUTE_NORMAL, 0);

	if ( hCR == INVALID_HANDLE_VALUE ) 
	{
		return ::GetLastError();
	}
	
	union cluster_t {
		char block[Q43_EXTENDED_CLUSTER_SIZE];
		qtree_file_headers hdrs;
	} cluster;

	memset(cluster.block, 0, Q43_EXTENDED_CLUSTER_SIZE);
	
	cluster.hdrs.signature[0] = 'Q';
	cluster.hdrs.signature[1] = 'E';
	cluster.hdrs.signature[2] = 'X';
	cluster.hdrs.signature[3] = 'v';
	cluster.hdrs.signature[4] = '0';
	cluster.hdrs.signature[5] = '.';
	cluster.hdrs.signature[6] = '5';
	cluster.hdrs.signature[7] = 0;

	cluster.hdrs.status = 1; // marco como formateado.
	cluster.hdrs.revision = 0; // version original
	cluster.hdrs.base_lat = lat; // asigno posicionamiento.
	cluster.hdrs.base_lon = lon;

	DWORD wr = 0;

	if (!::WriteFile(hCR, &cluster, Q43_EXTENDED_CLUSTER_SIZE, &wr, NULL)) {
		return ::GetLastError();
	}

	memset(cluster.block, 0, Q43_EXTENDED_CLUSTER_SIZE);

	for(int y=0; y < 60; y++) {
		for(int x=0; x < 60; x++) {
			int clus = (y * 60) + x + 1;
			if (!::WriteFile(hCR, &cluster, Q43_EXTENDED_CLUSTER_SIZE, &wr, NULL)) {
				QUADTREETRACE("Imposible iniciar cluster %d", clus);
				return ::GetLastError();
			}
			//TRACE("iniciando cluster %d", clus);
		}
	}

	QUADTREETRACE("GR2EX CREATED: [%s] LAT=%d LON=%d REV=%d", filename.c_str(), cluster.hdrs.base_lat, cluster.hdrs.base_lon,
		cluster.hdrs.revision);

	if (::CloseHandle(hCR)) {
		return 0;
	}

	return ::GetLastError();
}

bool QuatTreeExtended::GetFileCluster(float lat, float lon, int & cluster, int & index)
{
	ABS(lat);
	ABS(lon);

	QUADTREETRACE("calculando cluster de posicion %f %f", lat, lon);

	int gr_lat = (int)lat;
	int gr_lon = (int)lon;

	if (base_lat != gr_lat || base_lon != gr_lon) {
		QUADTREETRACE("GR2EX %d %d ", base_lat, base_lon);
		QUADTREETRACE("esta posicion no pertenece a este archivo %f %f", lat, lon);
		return false;
	}

	int min_lat = (int)((lat - gr_lat) * 60);
	int min_lon = (int)((lon - gr_lon) * 60);

	int sec_lat = (int)((lat - gr_lat - (min_lat/60.0)) * 3600);
	int sec_lon = (int)((lon - gr_lon - (min_lon/60.0)) * 3600);
	
	int norm_slat = sec_lat >> 1; // div 2
	int norm_slon = sec_lon >> 1; // div 2

	cluster = min_lat * 60 + min_lon;
	index = norm_slat * 30 + norm_slon;

	QUADTREETRACE("Latitude %dº %d' %d\" base=%d" , gr_lat, min_lat, sec_lat, min_lat * 60);
	QUADTREETRACE("Longitude %dº %d' %d\" base=%d" , gr_lon, min_lon, sec_lon, min_lon);
	QUADTREETRACE("cluster = %d / index = %d", cluster, index);
			
	return true;
}

bool QuatTreeExtended::RefreshCache(int cluster_num)
{
	if (cached_cluster_num != cluster_num) {
		if (cached_cluster_num != -1 && cached_cluster_modified) {
			QUADTREETRACE("StoreM2: salvando cluster %d.", cached_cluster_num);
			LONG store_offset = cached_cluster_num * Q43_EXTENDED_CLUSTER_SIZE;
			int	res = ::SetFilePointer(hFile, (store_offset+Q43_EXTENDED_CLUSTER_SIZE), 0, FILE_BEGIN);
			if (res == INVALID_SET_FILE_POINTER) {
				QUADTREETRACE("StoreM2: imposible mover puntero al sector %d.", cached_cluster_num);
				return false;
			}
			DWORD wr = 0;
			if (!::WriteFile(hFile, &cached_cluster, Q43_EXTENDED_CLUSTER_SIZE, &wr, NULL)) {
				QUADTREETRACE("StoreM2: Imposible salvar cluster %d", cached_cluster_num);
				return false;
			}
			char buffer[128];			
			sprintf(buffer, "FILE=M2-%d-%d.GEX:CLUSTER=%d", base_lat, base_lon, cached_cluster_num);			
		}

		LONG offset = cluster_num * Q43_EXTENDED_CLUSTER_SIZE;
		int	res = ::SetFilePointer(hFile, (offset+Q43_EXTENDED_CLUSTER_SIZE), 0, FILE_BEGIN);
		if (res == INVALID_SET_FILE_POINTER) {
			QUADTREETRACE("LoadM2: no se puede mover el cursor al cluster %d.", cluster_num);
			return false;
		}
		DWORD readed;
		if (!::ReadFile(hFile, &cached_cluster, Q43_EXTENDED_CLUSTER_SIZE, &readed, NULL)) {
			QUADTREETRACE("LoadM2: error de lectura de cluster %d.",cluster_num);
			return false;
		}
		QUADTREETRACE("LoadM2: cluster %d cargado.", cluster_num);
		cached_cluster_num = cluster_num;
		cached_cluster_modified = false;
	}
	return true;
}
