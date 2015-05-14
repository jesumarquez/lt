
#include "stdafx.h"
#include "QuadTreeDll.h"
#include <string>

using namespace std;

#define ASK_AND_EXIT() \
	{ printf("Presione RETURN para grabar y salir...");\
		char data[16]; scanf("%s",data); return 1; }


int _tmain(int argc, _TCHAR* argv[])
{
	printf("QuadTree Tester Inicializando v 1.0\r\n");
	QuadTree qtree;
	string basedir = "C:\\GR2_TEST2";
	if (!::SetCurrentDirectory(basedir.c_str())) {
		printf("El repositorio [%s] no existe, lo creamos.\r\n", basedir.c_str());
		if (qtree.Init(basedir.c_str()) != 0) {
			printf("Imposible inicializar el repositorio GR2\r\n");
			ASK_AND_EXIT()
		}
	}
	if (qtree.Open(basedir.c_str()) != 0) {
		printf("El repositorio GR2 parece corrupto.\r\n");
		ASK_AND_EXIT()
	}

	int clase = 0;
	char zona = 'A';
	for(int latmin=0; latmin < 1000; latmin++) {
		//printf("hecho: %0.2f\r\n", (latmin/10.0));
		for(int lonmin=0; lonmin < 1000; lonmin++) {
			float lat = 34.0 + (latmin/1000.0);
			float lon = 55.0 + (lonmin/1000.0);
			//printf("%f %f\r\n", lat, lon);
			qtree.SetPositionClass(lat, lon, clase++, zona++);	
			if (clase == 16) clase = 0;
			if (zona == 'Z') zona = 'A';
		}
	}


	/*
	qtree.SetPositionClass(34.5665, 55.2235, 12);
	int clase = qtree.GetPositionClass(34.5665, 55.2235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	qtree.SetPositionClass(34.2665, 55.4235, 3);
	qtree.SetPositionClass(34.267, 55.4235, 4);
	qtree.SetPositionClass(34.268, 55.4235, 5);
	qtree.SetPositionClass(34.269, 55.4235, 6);
	qtree.SetPositionClass(34.270, 55.4235, 7);
	qtree.SetPositionClass(34.271, 55.4235, 8);

	clase = qtree.GetPositionClass(34.2665, 55.4235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	clase = qtree.GetPositionClass(34.5665, 55.2235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	clase = qtree.GetPositionClass(34.2665, 55.4235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	qtree.SetPositionClass(37.5665, 55.2235, 12);
	clase = qtree.GetPositionClass(37.5665, 55.2235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	qtree.SetPositionClass(37.2665, 55.4235, 3);
	clase = qtree.GetPositionClass(37.2665, 55.4235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	clase = qtree.GetPositionClass(37.5665, 55.2235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	clase = qtree.GetPositionClass(37.2665, 55.4235);
	printf("sample: clase leida %d\r\n", clase);
	printf("\r\n");

	qtree.SetPositionClass(37.2665, 54.4235, 8);

	qtree.SetPositionClass(34.0, 55.0, 4);
	qtree.SetPositionClass(34.99999, 55.99999, 4);

	qtree.SetPositionClass(46.5, 65.5, 5);
	qtree.SetPositionClass(47.2, 56.4, 15);
	*/

	qtree.Close();
	ASK_AND_EXIT()	
	return 0;
}

