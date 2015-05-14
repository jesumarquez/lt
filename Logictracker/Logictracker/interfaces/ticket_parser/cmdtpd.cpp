#include "trace/trace.h"
#include "singleton.h"
#include "utils_string.h"
#include "xml/ini.h"
#include "app.h"
//#include "parser.hpp"

#pragma warning (disable:4995)

#include <windows.h>
#include <tchar.h>
#include <strsafe.h>

#include <shlwapi.h>

#define SVCNAME TEXT("CMDTP")

/* clase instanciable del server */
struct application : public command_data::appt, singleton <application> {
};

void configure(char* szPath) {
	std::ostringstream o;
	o << szPath << "\\cmdtp_config.ini";

	TDEBUG("TICKET PARSER loading configuration file:" << o.str());

	std::ifstream fs(o.str().c_str());
	if (!fs.is_open()) {
		TDEBUG("no esta el archivo de configuracion!");
		exit(-2);
	}
	utils::parsing::ini ini_parser;
    ini_parser.load_file(fs);
    fs.close();

	command_data::app_trait::ticket_dir = ini_parser.get_str_value("main","tickets_dir","");
	command_data::app_trait::ticket_prefix = ini_parser.get_str_value("main","tickets_prefix","");
	command_data::app_trait::ticket_ext = ini_parser.get_str_value("main","tickets_ext","");
	command_data::app_trait::ticket_old = ini_parser.get_str_value("main","tickets_old","");
	command_data::app_trait::table_name = ini_parser.get_str_value("import","table","");
	command_data::app_trait::cancel_field = ini_parser.get_str_value("import","cancel_field","");
	command_data::app_trait::key_field = ini_parser.get_int_value("import","key_field",0);
	std::vector<std::string> fields;
	utils::string::split(ini_parser.get_str_value("import","fields",""),",",fields);
	//;
	std::vector<std::string>::iterator it = fields.begin();
	#define magic "lasdaslkdjoilaksdmfl"
	for(; it != fields.end(); ++it) { 
		int field_id = atoi((*it).c_str());
		std::string field_name = ini_parser.get_str_value("import",(*it),magic);
		if (field_name != magic) {
			TDEBUG("Campo:" << field_id << " representado como:" << field_name);
			command_data::app_trait::table_fields.insert(std::make_pair(field_id, field_name));
		}
	}
	TDEBUG("Directorio a Monitorear:" << command_data::app_trait::ticket_dir);
	// ini_parser.get_int_value("main","ctrl_local_address",15555);
}

void __cdecl _tmain(int argc, TCHAR *argv[]) 
{	
	std::cerr << "running!" << std::endl;
	char * module_path = new char[MAX_PATH];
    if (!GetModuleFileName(NULL, module_path, MAX_PATH)) {
		std::cerr << "Imposible obtener el directorio local GetLastError = " << GetLastError() << std::endl;
        exit(-3);
    }	
	PathRemoveFileSpec(module_path);

	if( argc > 1 && lstrcmpi( argv[2], TEXT("trace")) == 0 ) {
		tracer::__default_tracer = &(std::cout);
		SET_TRACELEVEL(1);
	} else {
		std::ostringstream o;
		o << module_path << "\\cmdtp_trace.txt";
		std::ofstream * output_file = new std::ofstream(o.str().c_str(),std::ios::trunc);
		tracer::__default_tracer = output_file;
		SET_TRACELEVEL(1);
	}
	configure(module_path);
	delete [] module_path;
	application::instance()->main(SVCNAME, argc, argv);
}

