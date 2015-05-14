
#pragma warning (disable:4995)

#include "app.h"
#include "parser.hpp"

// los estaticos.
std::string command_data::app_trait::ticket_dir;
std::string command_data::app_trait::ticket_prefix;
std::string command_data::app_trait::ticket_ext;
std::string command_data::app_trait::ticket_old;
std::map<std::string, long> command_data::wd_trait::files;
std::string command_data::app_trait::table_name;
std::string command_data::app_trait::cancel_field;
int command_data::app_trait::key_field;
std::map<int, std::string> command_data::app_trait::table_fields;

server::watch_dir_loop< command_data::wd_trait > command_data::app_trait::_app_wd;

bool command_data::app_trait::initialize() {
	_app_wd.initialize(ticket_dir);
	command_data::wd_trait::refresh_directory(ticket_dir);
	return true;
}

bool command_data::app_trait::each_loop() {
	_app_wd.each_loop();
	return true;
}

void command_data::wd_trait::refresh_directory(std::string dir) {
	std::ostringstream fp; 
	fp << dir << command_data::app_trait::ticket_prefix << "*." << command_data::app_trait::ticket_ext;

	std::string pattern = fp.str();
	TDEBUG("procesando archivos:" << pattern);

	WIN32_FIND_DATA FindFileData;
	HANDLE hFind;	
	hFind = FindFirstFile(pattern.c_str(), &FindFileData);
	if (hFind == INVALID_HANDLE_VALUE) 
	{
		TERROR("Invalid File Handle. GetLastError reports " << GetLastError());
		ExitProcess(1);
	} else {        
		long file_size = (FindFileData.nFileSizeHigh * (MAXDWORD+1)) + FindFileData.nFileSizeLow;
		process_new_file(dir, std::string(FindFileData.cFileName), file_size);
		while (FindNextFile(hFind, &FindFileData) != 0) {
			file_size = (FindFileData.nFileSizeHigh * (MAXDWORD+1)) + FindFileData.nFileSizeLow;
			process_new_file(dir, std::string(FindFileData.cFileName), file_size);
		}
        FindClose(hFind);
	}
};

void command_data::wd_trait::process_new_file(std::string dir, std::string file, long filesize) {
	TDEBUG("Procesando Archivo:" << file << " size:" << filesize);
	std::map<std::string, long>::iterator it = files.find(file);
	if (it != files.end()) {
		if (it->second == -1) {
			TINFO("Archivo:" << file << " se ignora por errores en el formato.");
			return;
		}
		if (it->second < filesize) {
			TDEBUG("Archivo:" << file << " ha crecido de:" << it->second << " a " << filesize);
			long newpoint = process_file(dir + file, it->second);
			files[file] = newpoint;
		} else {
			TDEBUG("Archivo:" << file << " sin modificaciones.");
		}
	} else {
		long newpoint = process_file(dir + file, 0);
		TDEBUG("Archivo:" << file << " es nuevo, fue procesado hasta:" << file);
		files.insert(std::make_pair(file, newpoint));
	}
}

long command_data::wd_trait::process_file(std::string filename, long begin_at) {
	TINFO("PROCESAMIENTO DE ARCHIVO:" << filename << " SEEK:" << begin_at);
	int end_at = begin_at;
	std::ifstream file(filename.c_str());
	file.seekg(begin_at, std::ios_base::beg);
	char current;
	// otra fsm
	int state = 0; // 0 busca registro, 1 dentro registro
	int last_record_begin = end_at;
	std::ostringstream record;
	while(file.get(current)) {
		if (file.fail()) {
			TDEBUG("EOF --> " << filename);
			break;
		}
		end_at++;
		if (state == 0) {
			if (current == '"') {
				state = 1;
				record.str(std::string(""));
				record << '"';
			}
		} else if (state == 1) {
			if (current == '"') {
				last_record_begin = end_at + 1;
				state = 0;
				record << current << "\r\n";
				if (!process_record(record.str())) {
					return -1;
				}
				continue;
			}
			record << current;
		}
	}
	file.close();
	return last_record_begin;
};

bool command_data::wd_trait::process_record(std::string record) {
	TDEBUGDEBUG("REGISTRO:" << record);
	command_data::table_t the_table;
	bool ret = tickets_parser::parse_record(record, the_table);
	if (ret && the_table.size() == 1) {
		// creamos el sql.
		bool dbexists = false; // TODO: consulta SQL a la base a ver si existe.
		if (dbexists) {
			TDEBUG("El registro ya existe en la base de datos.");
		} else {
			TDEBUG("Se va a insertar un registro en la base de datos.");
		}
		return true;
	}
}

//// eof
