
#ifndef _PARSER_HPP

#define BOOST_SPIRIT_SINGLE_GRAMMAR_INSTANCE

#include "trace/trace.h"
#include <boost/spirit/core.hpp>
#include <boost/spirit/utility/functor_parser.hpp>
#include <boost/spirit/iterator/file_iterator.hpp>
#include <iostream>
#include <sstream>
#include <map>
#include <list>
#include "app.h"

namespace command_data {

	////////////////////////////////////////////////////////////////////////////
	//
	//  Datos, base de datos string.
	//
	////////////////////////////////////////////////////////////////////////////

	struct record_t {
		record_t(bool canceled = false) : _canceled(canceled) {}
		typedef std::map<int, std::string> fields_t;
		fields_t fields;
		bool _canceled;
	};

	struct table_t : std::map<int, record_t> {
		int _active_id;
		int _active_field_id;
		std::map<int, record_t>::iterator _active_record;

		std::map<int, record_t>::iterator & get_active() {
			return _active_record;
		}

		void create_field(int id) {
			_active_field_id = id;
		}

		void set_field_data(std::string data) {
			if (_active_record == end()) throw std::exception("no hay un registro activo.");
			TDEBUG("\tCAMPO ID: " << _active_field_id << " DATA: " << data);
			get_active()->second.fields.insert(make_pair(_active_field_id, data));
		}

		void create(int id) {
			_active_id = id;
			TDEBUG("NUEVO TICKET NUMERO: " << id);
			record_t empty_record;
			insert(std::make_pair(id, empty_record));
			_active_record = find(id);
		}

		void cancel(int id) {
			_active_record = find(id);
			if (_active_record != end()) {
				TDEBUG("CANCELA TICKET NUMERO: " << id);
				// primero alta luego cancel? en fin..
				get_active()->second._canceled = true;
			} else {
				TDEBUG("CANCELA NUEVO TICKET NUMERO: " << id);
				record_t canceled_record(true);
				insert(std::make_pair(id, canceled_record));
			}
		}
	};

	typedef std::string::iterator iterator_t;

	struct tickets_parser {
		static int errcount;
		static int _global_tracer;
		static int _scan_tracer;		
		static bool parse_record(std::string & record, table_t & our_table);
	};

}

#endif
