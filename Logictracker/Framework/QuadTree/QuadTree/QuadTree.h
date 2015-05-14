/* $Id: QuadTree.h 3974 2009-06-14 22:04:21Z joseg $ */
#pragma once

#include "StdAfx.h"
#include <string>

using namespace std;

#define MAX_REPO_TRANSITIONS 16

#pragma pack(1)
struct qtree_file_headers {
	char signature[8];
	char status;
	int revision;
	short base_lat;
	short base_lon;
};
#pragma pack()

#pragma pack(1)
struct qtree_file_m2 {
	char status;
	int revision;
	char data[450];
};
#pragma pack()

class QuadTree;

class QuatTreeGR2 {
	HANDLE hFile;
	short base_lat;
	short base_lon;
	int cached_sector_num;
	bool cached_sector_modified;
	union {
		char data[512];
		qtree_file_m2 m2;
	} cached_sector;
public:
	QuadTree * qTree;
	QuatTreeGR2();

	int Load(string filename, short lat, short lon);
	void Close();
	
	// manipulacion de posiciones.
	int GetPositionClass(float lat, float lon);
	bool SetPositionClass(float lat, float lon, int _class);

	int Create(string filename, short lat, short lon);
	bool GetFileSector(float lat, float lon, int & sector, int & byte, bool & low_bits);
	bool RefreshCache(int sector_num);
};

#pragma pack(1)
struct qtree_file_extended_2s2 {
	char flags;
	char zone;
};
#pragma pack()

#pragma pack(1)
struct qtree_file_extended_m2 {
	char status;
	int revision;
	qtree_file_extended_2s2 sq_seconds[900];
};
#pragma pack()

#define Q43_EXTENDED_CLUSTER_SIZE 2048

class QuatTreeExtended {
	HANDLE hFile;
	short base_lat;
	short base_lon;
	int cached_cluster_num;
	bool cached_cluster_modified;
	union {
		char data[Q43_EXTENDED_CLUSTER_SIZE];
		qtree_file_extended_m2 m2;
	} cached_cluster;
public:
	QuadTree * qTree;
	QuatTreeExtended();

	int Load(string filename, short lat, short lon);
	void Close();
	
	// manipulacion de posiciones.
	bool GetPositionExtended(float lat, float lon, qtree_file_extended_2s2 & result);
	bool SetPositionExtended(float lat, float lon, qtree_file_extended_2s2 newvalue);

	int Create(string filename, short lat, short lon);
	bool GetFileCluster(float lat, float lon, int & cluster, int & index);
	bool RefreshCache(int cluster);
};

