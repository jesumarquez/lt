// This is the main DLL file.

#include "stdafx.h"
#include "parser_dll.h"

using namespace boost::spirit;
using namespace parser_dll::command_data;

////////////////////////////////////////////////////////////////////////////
//
//  Trazador, valido tambien como accion semantica.
//
////////////////////////////////////////////////////////////////////////////

struct trace
{
	trace(const char * _msg) : msg(_msg) {}
    void operator()(iterator_t start, iterator_t end) const
    {
		if (ticket_parser::_scan_tracer) {
			std::cout << "scan trace:" << msg << std::endl;
			std::cout << "\tmatch:'" << std::string(start, end) << "'" << std::endl;
		}
    }
	
	std::string msg;
};

////////////////////////////////////////////////////////////////////////////
//
//  Semantic actions
//
////////////////////////////////////////////////////////////////////////////

struct record_mg
{
	/** \note para agreagar un registro, se ejectuta la secuencia set_id 
	    seguido de create_number 
     */
	enum actions_e {
		create,	
		cancel,
		close,
		create_field,
		set_field_data
	};

	record_mg(table_t & table, actions_e action) :_table(table), _action(action) {}
    
	void operator()(iterator_t start, iterator_t end) const;
    
	actions_e _action;
	table_t  & _table;
};

// usada durante pruebas, imprime en pantalla lo que se matcheo.
struct out_string
{
    out_string() {}
    void operator()(iterator_t start, iterator_t end) const
    {
		while(start != end)
			if (ticket_parser::_global_tracer) std::cout << (*start++);
		if (ticket_parser::_global_tracer) std::cout << std::endl;
    }
};

/////////////////////////////////////////////////////////////////////////////// 
// 
//  Error reporting parser 
// 
/////////////////////////////////////////////////////////////////////////////// 
struct error_report_parser { 
    
    error_report_parser(const char *msg) : _msg(msg), _fatal(true) {}

    typedef nil_t result_t; 
    
    template <typename ScannerT> 
    int operator()(ScannerT const& scan, result_t& /*result*/) const 
    { 
		ticket_parser::errcount++; 
		if (ticket_parser::_global_tracer) {
			if (_fatal) std::cout << "fatal ";
			std::cout << "error:" << _msg << std::endl;
		}
		if (_fatal) return -1; 
		return 0;
    } 

private: 
	bool _fatal;
	std::string _msg; 
}; 

typedef functor_parser<error_report_parser> error_report_p; 

////////////////////////////////////////////////////////////////////////////
//
//  Gramatica
//
////////////////////////////////////////////////////////////////////////////

struct ticket_grammar : public grammar<ticket_grammar>
{
	static error_report_p error_missing_quote;     
	static error_report_p error_missing_eol;
	static error_report_p error_missing_preamble;
	static error_report_p error_missing_close;

	ticket_grammar(table_t & mytable) : _table(mytable) {}

	table_t & _table;

    template <typename ScannerT>
    struct definition
    {
        definition(ticket_grammar const& self)
		{
			/*integer =
                lexeme_d[ (+digit_p)[push_int(self.eval)] ]*/
			
			//record = +(anychar_p - ch_p('"')); // [out_string()];
			

			ticket_fields = +( (digit_p >> digit_p>> digit_p)[record_mg(self._table, record_mg::create_field)] >>
				               (*(anychar_p - eol_p))[record_mg(self._table, record_mg::set_field_data)] >> eol_p);
			
			w_fields = ( str_p("W001") >> (+digit_p >> ch_p('-') >> +alpha_p >> ch_p('-') >> +digit_p >> ch_p(' ') >> +digit_p >> ch_p(':') >> +digit_p)[trace("WAKEUP")] >> eol_p );

			t_fields = ( str_p("T002") >> (+digit_p)[record_mg(self._table, record_mg::create)] >> eol_p
						| str_p("T007")[trace("datos de ticket")] >> (*ticket_fields)
					    | str_p("T003") >> (+digit_p)[record_mg(self._table, record_mg::close)] >> eol_p
						| str_p("T006") >> (+digit_p)[record_mg(self._table, record_mg::cancel)] >> eol_p
			            );

			record_preamble = (ch_p(0x16) >> ch_p(0x16) >> (ch_p(0x02)|ch_p(0x05)|ch_p(0x1B))) // [trace("preambulo matcheado")]
							   | error_missing_preamble;

			record_close =      (ch_p(0x03) >> ch_p(0x04)) 
					           | ch_p(0x04);

			record_valid = +(anychar_p - ch_p('"') - record_close);

			record = record_preamble >>
					  (+(str_p("011")[trace("ignorar 011")]|+w_fields|+t_fields)-record_close)
					   >> record_close | error_missing_close;

			required_quote = (str_p("\"")|error_missing_quote); 
			
			quoted_record =  required_quote[trace("abre quote")]>> record >> required_quote[trace("cierra quote")];

			expression = end_p 
						|*(quoted_record >> (eol_p|error_missing_eol));
					     
        }

        rule<ScannerT> expression, quoted_record, required_quote, record,
					record_preamble	, record_close, t_fields, record_valid,
					ticket_fields, w_fields
					;
        rule<ScannerT> const&
        start() const { return expression; }
    };

	std::list<int> fields;
};



void record_mg::operator()(iterator_t start, iterator_t end) const {
	std::string record(start, end);		
	switch (_action) {
		case create:
			_table.create(atoi(record.c_str()));
			break;
		case cancel:
			_table.cancel(atoi(record.c_str()));
			break;
		case create_field:
			_table.create_field(atoi(record.c_str()));
			break;
		case set_field_data:
			_table.set_field_data(record);
			break;
		case close:
			break;
		default:
			throw std::exception("accion desconcida.");
	}
}

error_report_p ticket_grammar::error_missing_quote("se esperaba una comilla doble."); 
error_report_p ticket_grammar::error_missing_eol("se esperaba un fin de linea.");
error_report_p ticket_grammar::error_missing_preamble("se esperaba el preambulo del registro.");
error_report_p ticket_grammar::error_missing_close("se esperaba el cierre del registro.");

////////////////////////////////////////////////////////////////////////////
//
//  Main program
//
////////////////////////////////////////////////////////////////////////////
bool ticket_parser::parse_str(String ^ _net_record, ticket ^ out_ticket)
{	

	std::string record = std::string((char*)(void*)Marshal::StringToHGlobalAnsi(_net_record));
	table_t our_table;

    // Create a file iterator for this file
    iterator_t first = record.begin();
    iterator_t last = record.end();

    ticket_grammar tickets(our_table); //  Our parser

	parse_info<iterator_t> info = parse<iterator_t>(first, last, tickets, nothing_p);
	if (info.full || info.hit) {
		//TINFO("parsing succeeded");
		if (our_table.size()) {
			record_t record = our_table.begin()->second;
			out_ticket->_canceled = record._canceled;
			out_ticket->id = our_table.begin()->first;
			for (std::map<int, std::string>::iterator it = record.fields.begin();
				it != record.fields.end(); ++it) {
					out_ticket->fields->Add(it->first, gcnew System::String(it->second.c_str()));
			}
		}
		return true;	
	} else {
		TRACE("parsing failed");
	}

   return false;
}

// eof
