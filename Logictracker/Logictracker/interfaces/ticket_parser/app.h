#ifndef _APPL_H
#define _APPL_H

#define SVR_LOOP_WINCLASSNAME "cmdtp_class"
#define SVR_LOOP_WINNAME "cmdtpd"

#include <watchdir.h>
#include "server/application.h"
#include <sstream>
#include <string>

namespace command_data
{
	/* rasgos del monitor */
	struct wd_trait
	{
		static void refresh_directory(std::string dir);

		static void appended_file(std::string file);

		static void process_new_file(std::string dir, std::string file, long filesize);

		static long process_file(std::string file, long begin);

		static bool process_record(std::string record);
		
		static std::map<std::string, long> files;
	};

	/* rasgos de la aplicacion */
	struct app_trait
	{
		static bool initialize();

		static bool each_loop();

		static server::watch_dir_loop< wd_trait > _app_wd;

		static std::string ticket_dir;
		static std::string ticket_prefix;
		static std::string ticket_ext;
		static std::string ticket_old;
		static std::string table_name;
		static std::string cancel_field;
		static int key_field;
		static std::map<int, std::string> table_fields;
	};

	/* tipo de server */
	typedef server::application< app_trait > appt;
	
}

#endif 

// eof
