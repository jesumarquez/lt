/*! \mainpage

\section about About the Fleet Management Controller

The Fleet Management Controller is a sample implementation of a server as
defined by the Fleet Management Interface Control Specification (001-00096-00)
Rev. G, and is provided for evaluation and testing of Fleet Management
features, as well as a starting point and reference for those developing Fleet
Management solutions that conform to the Interface Control Specification.

In addition to the software, a Garmin client device supporting FMI and the
appropriate FMI cable are required.  For instructions on assembling a serial
cable to connect to a PC serial port, see
<http://www.garmin.com/specs/pcwiring.pdf>.  The wire color assignments for
the 010-10865-00 and 010-10813-00 FMI cables are as follows:
- Yellow: Data Out (DB9 pin 2),
- White: Data In (DB9 pin 3),
- Black: Ground (DB9 pin 5).

The wire color assignments for the 010-11232-00 FMI cable is as follows:
- Yellow: Data Out (DB9 pin 2),
- White: Data In (DB9 pin 3),
- Brown: Ground (DB9 pin 5).

In the event of any discrepancies between the Fleet Management Interface
Control Specification and this sample application (in documentation, behavior,
or otherwise) the Fleet Management Interface Control Specification shall be
considered the authoritative reference.

\section theory Theory of Operation

The application is implemented in C++ as a single-threaded MFC application,
and communicates with a single client over a direct serial link.

\subsection startup Startup

The program starts up by initializing all data saved from the last execution
of the program.  This data is saved in various save files controlled by the
FileBackedMap template class (namely: canned_messages.dat,
canned_responses.dat, sent_canned_response_messages.dat, driver_status.dat,
a603_stops.dat, categories.dat, waypoints.dat drivers.dat).  If any of these
files are not present or empty, the associated data structures will also be
empty (which is fine).

Upon connection to the client, the client may send refresh requests to the
server (this application) to update the canned message list, canned response
list, and driver status list.  Stops and throttled protocols are not refreshed
on startup; they are refreshed when the corresponding monitor dialog is opened.

The stop data structure is the only one that cannot be read in and assumed to
be up to date, since stops can be updated or deleted when the client is not
connected to the server.  To handle this, stop updates are requested for all
stops in the stop save file.  When the stop data structures are set up
correctly, receiving a deleted stop update will delete that ID from the stop
list and also move any stops below it up.  However, on startup there may be
stops that have not been initialized yet (in other words, the server cannot be
sure of the index in the list).  To solve this problem, a flag is set to tell
the Communication (Com) that the stop list is not initialized.  With this flag
set, the update process is modified.  It ignores all updates except deletes,
and then only removes the stop from the save file and from the ID list. To
fully initialize the stop list, the server sends out two waves of updates.
The first is sent for all stops in the save file with the initialized flag set
to false (update process modified).  This will get rid of all deleted stop
updates and solve the problem.  Whenever the user requests the stop list
dialog for the first time, the initialized flag is set to true (update process
set back to normal) and another wave of updates are sent for the IDs remaining
in the stop ID list.  This time the server will update the status and index
in the stop list for all valid stops.

\subsection comm Communication Protocol

Communication is implemented in a layered manner; the details of each layeer
are in a separate class:
* SerialPort is the physical layer.  It reads and writes bytes from the RS232
port.
* LinkLayer is the next layer up.  It takes the bytes received from the serial
port and builds them into frames, sending NAK and ACK packets in response.  It
also takes packets sent by the transport layer and builds them into a stream of
bytes by assembling the packet and adding the header (DLE), footer (DLE-ETX)
and DLE-stuffing the data.
* TransportLayer is the next layer up.  It processes received ACK and NAK
packets and handles timeouts; other received packets are passed up to the
application layer.  For packets received from the application layer, the
TransportLayer maintains a queue of pending packets to ensure that no more
than one is outstanding at a time.
* The ApplicationLayer sends packets to the TransportLayer and processes the
packets received from the TransportLayer, updates data structures appropriately,
and notifies the UI when events occur.

The Server and client communicate using a handshaking protocol.  This involves
not sending any new packets out after one has been transmitted until a response
is received.  There are two possibilities for reponses--ACKs (successful
transmission) or NAKs (unsuccessful transmission).  To implement this, the last
transmitted packet is left at the top of the transmit queue.  When an ACK is
received, the packet is removed from the queue and the next packet (if any) is
sent.  If a NAK is received, the TransportLayer resends the packet at the top of
the queue.  This allows the server to maintain the handshaking protocol, while
maintaining a responsive user inteface and responding to received packets as they
come in.

\subsection protocol Adherence to protocols

This program has been designed to not allow the sending of packets without all
of the required information (i.e. sending a A604 open text message without a
message ID).  If desired, it is possible to disable these checks in the
application by changing the SKIP_VALIDATION define in fmi.h to TRUE, then
rebuilding the app.  However, this is not recommended and could result in
undefined behavior on the client device.  SKIP_VALIDATION should normally be
left set to FALSE.

\subsection fmi_packet Sending Arbitrary FMI Packets

The program is designed to allow easy transmission of packets without the user
entering in all the header and footer associated with each packet (for example,
calculating the checksum is error-prone when done by hand); however, if there
is something the UI currently doesn't support, there is a means to send
whatever data the user wants.  In the top right hand corner of the home screen,
there is a box entitled 'FMI Packet' with 2 fields.  The ID field is the ID of
the FMI packet to be sent (not 0xa1).  This field requires the entry of all 4
hex digits for all 16 bits.  This means the user must enter 0x0003 (with all
zeros, '0x' is not required) for packet ID 3.  The data field should be entered
exactly as the data will be transmitted WITHOUT DLE byte stuffing.  The current
protocol uses little endian byte ordering so if a particular field of a packet
required the 32 bit hex value of 0x12345678, the proper entry for this is
78563412 ('0x' is not required but can be used).

\subsection logging Logging and the Log Viewer

All packets transmitted to or received from client device are logged to the
file fmi_pc_app.log for analysis or debugging purposes; the program includes a
log viewer which can be used to display, parse, or search the log.  Logs can
also be saved, and the log viewer can be opened without connecting to a client
device, to view a packet log from a previous execution of the program.

The log file is a text file with ANSI encoding.  The format of the log file is
as follows:
- Line 1: Time when log was started, as four comma-delimited numbers
  (hr,mn,ss,millis).
- Lines 2-n: Log details
  -- Char 1: 'T' if the line represents transmitted data, 'R' if received data.
  -- Char 2..k-1: Offset in milliseconds when this line was written.
  -- Char k: A hyphen '-'; this is strictly a delimiter
  -- Char k+1..m: The raw data, in hexadecimal format.  For transmitted data,
     this is exactly one packet; for received data, this shows all data from
     the beginning of the packet to the end of the FIFO buffer.

Note: The last line of the file may not be terminated with a newline character.

\subsection vcproj Protocol support

The program can be built in several configurations to support all protocols up
to a certain identifier - A602, A603, A604, A605, or A607; this is done by
wrapping code to support particular features in preprocessor if / endif blocks.
Several solution configurations are provided which set up the preprocessor
definitions and exclude source files as needed for each protocol level.  For
example, the Message Throttling Query protocol was introduced in protocol A605;
the PC app support for this protocol is preceded by an if (FMI_SUPPORT_A605)
and followed by an endif block.  This supports testing for backward
compatibility, in addition to identifying the source code relevant to the
protocols introduced in each release of the Fleet Management Interface
specification.  Note that this is a compile-time switch; the protocol support
data sent by the client device does not influence the options which are
available in the program.

\subsection unicode Unicode support

The application uses Unicode character strings within the user interface
regardless of the client code page; text strings are converted to either code
page 1252 (ASCII) or UTF-8 before being passed to Com, and text strings
received from the client are converted from 1252 or UTF-8 to Unicode when
they are displayed.  The member variable Com.client_codepage tracks whether
the client supports UTF-8, and the Windows API functions WideCharToMultiByte
and MultiByteToWideChar are used to convert between client and server code
pages.

It is possible to disable the Unicode Support protocol on the server by
changing the define for UNICODE_SUPPORT to FALSE.  This define can be found
in the fmi.h header file.

\subsection other Other Details

The other details of the implementation should be self evident, explained in
the specification document, or commented in the code.

*/
