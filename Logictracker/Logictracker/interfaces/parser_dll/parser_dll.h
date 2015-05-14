// parser_dll.h

#pragma once
#include <boost/spirit/core.hpp>
#include <boost/spirit/utility/functor_parser.hpp>
#include <boost/spirit/iterator/file_iterator.hpp>
#include <iostream>
#include <sstream>
#include <map>
#include <list>

#define TRACE(x) { if (ticket_parser::_global_tracer) std::cout << "ACTION:" << x << std::endl; }

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices; // for class Marshal

namespace parser_dll {

namespace command_data {

	public ref class ticket {
	public:
		ticket(): fields(gcnew Dictionary<int, String^>()), _canceled(false), id(0) {}
		Dictionary<int, String^> ^ fields;
		bool _canceled;
		int id;
	};

	public ref class ticket_parser {
	public:
		static int errcount;
		static int _global_tracer;
		static int _scan_tracer;		
		static bool parse_str(String ^ record, ticket ^ out_ticket);
	};

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
			TRACE("\tCAMPO ID: " << _active_field_id << " DATA: " << data);
			get_active()->second.fields.insert(make_pair(_active_field_id, data));
		}

		void create(int id) {
			_active_id = id;
			TRACE("NUEVO TICKET NUMERO: " << id);
			record_t empty_record;
			insert(std::make_pair(id, empty_record));
			_active_record = find(id);
		}

		void cancel(int id) {
			_active_record = find(id);
			if (_active_record != end()) {
				TRACE("CANCELA TICKET NUMERO: " << id);
				// primero alta luego cancel? en fin..
				get_active()->second._canceled = true;
			} else {
				TRACE("CANCELA NUEVO TICKET NUMERO: " << id);
				record_t canceled_record(true);
				insert(std::make_pair(id, canceled_record));
			}
		}
	};

	typedef std::string::iterator iterator_t;
}

} // parserdll
