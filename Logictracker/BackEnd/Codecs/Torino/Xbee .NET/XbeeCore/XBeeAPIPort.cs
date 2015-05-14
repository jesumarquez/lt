using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Urbetrack.DatabaseTracer.Core;

namespace XbeeCore
{
    /// <summary>
    /// Representa un puerto xbee conectado por puerto serie.
    /// </summary>
    public class XBeeAPIPort
    {
        #region Enumeraciones
        public enum Estados
        {
            PUERTO_CERRADO,
            PUERTO_ABIERTO,
            MODULO_ENLINEA
        }
        #endregion

        #region Constructores
        public XBeeAPIPort()
        {
            port_timer = new Timer(RutinaDeControl, null, 100, 20000);
            SerialConnectionTimeout = 10;
        }
        #endregion

        public void Close()
        {
            //port_timer.Change(TimeSpan.MaxValue, TimeSpan.MaxValue);
        }

        #region Propiedades Publicas
        
        public Estados Estado
        {
            get { return estado; }
            set 
            { 
                var new_estado = value;
                if (new_estado != estado)
                {
                    STrace.Debug(GetType().FullName, "PORT(" + SerialPort + "): Transicion de " + estado + " a " + new_estado);
                    estado = new_estado;
                    if (new_estado == Estados.PUERTO_CERRADO)
                    {
                        if (uart != null)
                        {
                            uart.DataReceived -= DataReceived;
                            uart.ErrorReceived -= ErrorReceived;
                            uart.DtrEnable = false;
                            uart.Close();
                        }
                        uart = null;

                        STrace.Debug(GetType().FullName, "Demorando la Reapertura 10 segundos");
                        port_timer.Change(10000, 5000);
                    }
                }
            }
        }

        public ushort MyXbeeAddress { get; private set; }
        
        public int Id { get; set; }

        public int TrQueueCount
        {
            get { return tr_layer.Count; }
        }

        public int Rate
        {
            get { return rate; }
            set { 
                    rate = value;
                    STrace.Debug(GetType().FullName, "Set Rate: " + value);
                    Estado = Estados.PUERTO_CERRADO;
                }
        }

        public Handshake Handshake
        {
            get { return handshake; }
            set
            {
                handshake = value;
                STrace.Debug(GetType().FullName, "Set Handshake: " + value);
                Estado = Estados.PUERTO_CERRADO;
            }
        }

        public StopBits StopBits
        {
            get { return stopbits; }
            set
            {
                stopbits = value;
                STrace.Debug(GetType().FullName, "Set StopBits: " + value);
                Estado = Estados.PUERTO_CERRADO;
            }
        }

        public Parity Parity
        {
            get { return parity; }
            set
            {
                parity = value;
                STrace.Debug(GetType().FullName, "Set Parity: " + value);
                Estado = Estados.PUERTO_CERRADO;
            }
        }

        public int DataBits
        {
            get { return databits; }
            set
            {
                databits = value;
                STrace.Debug(GetType().FullName, "Set DataBits: " + value);
                Estado = Estados.PUERTO_CERRADO;
            }
        }
        
        public string SerialPort
        {
            get { return serialport; }
            set {
                    serialport = value;
                    STrace.Debug(GetType().FullName, "Set SerialPort: " + value);
                    Estado = Estados.PUERTO_CERRADO;
                }
        }

        public bool CTS
        {
            get { return uart != null && uart.IsOpen && uart.CtsHolding; }
        }

        public bool DSR
        {
            get { return uart != null && uart.IsOpen && uart.DsrHolding; }
        }

        public bool CD
        {
            get { return uart != null && uart.IsOpen && uart.CDHolding; }
        }

        public bool DTR
        {
            get { return uart != null && uart.IsOpen && uart.DtrEnable; }
            set { if (uart != null) uart.DtrEnable = value; }
        }

        public bool RTS
        {
            get { return uart != null && ((uart.Handshake != Handshake.RequestToSend && uart.RtsEnable) || uart.Handshake == Handshake.RequestToSend); }
            set { if (uart != null && uart.Handshake != Handshake.RequestToSend) uart.RtsEnable = value; }
        }

        public int SerialConnectionTimeout { get; set; }

        #endregion

        #region Primitivas
        public bool Send(string data, ushort address)
        {
            return Send(Encoding.ASCII.GetBytes(data), address);
        }

        public bool Write(byte[] data)
        {
            try
            {
                var ix = 0;
                byte checksum = 0;
                var buf = new byte[data.Length * 2];
                buf[ix++] = 0x7E; // Start Delimiter...
                buf[ix++] = 0;
                buf[ix++] = Convert.ToByte(Convert.ToInt16(data.Length & 0xFF));
                foreach (var b in data)
                {
                    checksum += b;
                    EscapeByte(ref buf, ref ix, b);
                }
                EscapeByte(ref buf, ref ix, (byte)(0xFF - checksum));
                uart.Write(buf, 0, ix);
                return true;
            }
			catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
                return false;
            }
        }

        public bool Send(byte[] data, ushort address)
        {
            lock (uartLocker)
            {
                try {
                    if (uart == null || Estado == Estados.PUERTO_CERRADO) return false;
                    var pdu = new XBeePDU(address, 0x01, data);
                    // tr_layer.Create(pdu, active_tx_timeout);
                    var buf = pdu.ToByteArray();
                
                    STrace.Debug(GetType().FullName, String.Format("TX Request 16 bits, Enviando {0} bytes. (sin cabeceras)", buf.GetLength(0)));
                    Stack._writed_bytes += buf.GetLength(0);
                    Write(buf);
                    //Write(buf);
                    return true;
                }
                catch (UnauthorizedAccessException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_unautorized_access_exceptions++;
                    return false;
                }
                catch (IOException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_io_exceptions++;
                    return false;
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_unknow_exceptions++;
                    return false;
                }
            }
        }

        private void EscapeByte(ref byte[] buf, ref int ix, byte b)
        {
            if (b == 0x7E)
            {
                buf[ix++] = 0x7D;
                buf[ix++] = 0x5E;
                return;
            }
            if (b == 0x7D)
            {
                buf[ix++] = 0x7D;
                buf[ix++] = 0x5D;
                return;
            }
            if (b == 0x11)
            {
                buf[ix++] = 0x7D;
                buf[ix++] = 0x31;
                return;
            }
            if (b == 0x13)
            {
                buf[ix++] = 0x7D;
                buf[ix++] = 0x33;
                return;
            }
            buf[ix++] = b;
        }

        #endregion

        #region EVENTO PDURecibida
        public delegate void PDURecibidaHandler(XBeeAPIPort sender, XBeePDU pdu);

        public event PDURecibidaHandler PDURecibida;
        #endregion

        #region EVENTO HardwareStatus
        public delegate void HardwareStatusHandler(XBeeAPIPort sender, XBeePDU pdu);

        public event HardwareStatusHandler HardwareStatus;
        #endregion

        #region Mecanismos de Recepcion de datos.
        private bool unescape_next;
        private void DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            //Stack.DebugHigh("Datos Recibidos");
            lock (uartLocker)
            {
                // este metodo deserializa el puerto y convierte los paquetes recibidos
                // en bloques de datos, en PDUs.
                if (uart == null)
                {
                    STrace.Debug(GetType().FullName,"DataReceived: UART Invalido, amortiguador.");
                    return;
                }
                try
                {                    
                    while (uart.BytesToRead > 0)
                    {
                        // leo un caracter.
                        var b = Convert.ToByte(uart.ReadByte());
                        Stack._readed_bytes++;
                        last_received_data = DateTime.Now;

                        var unescape_prev = false;
                        // Secuencias de Escape
                        if (b == 0x7D)
                        {
                            unescape_next = true;
                            continue;
                        }
                        if (unescape_next)
                        {
                            switch (b)
                            {
                                case 0x5E:
                                    b = 0x7E;
                                    break;
                                case 0x5D:
                                    b = 0x7D;
                                    break;
                                case 0x31:
                                    b = 0x11;
                                    break;
                                case 0x33:
                                    b = 0x13;
                                    break;
                                default:
                                    throw new Exception("XBEEAPI: secuencia de escape incorrecta.");
                            }
                            unescape_next = false;
                            unescape_prev = true;
                        }
                        if (b == 0x7E && !unescape_prev)
                        {
                            if (active_pdu != null)
                            {
                                Stack._decoder_unexpected_x7E++;
                                CompleteReceive(active_pdu);
                            }
                            active_pdu = new XBeePDU();
                            // necesita al menos de 4 bytes para dar true...
                            var status = active_pdu.PushReadedByte(b);
                            switch(status) {
                                case XBeePDU.PushStatus.DISPATCH:
                                    CompleteReceive(active_pdu);
                                    active_pdu = null;
                                    break;
                                case XBeePDU.PushStatus.RESET:
                                    active_pdu = null;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            if (active_pdu == null)
                            {
                                Stack._padding_bytes++;
                            }
                            else 
                            {
                                var status = active_pdu.PushReadedByte(b);
                                switch(status) {
                                    case XBeePDU.PushStatus.DISPATCH:
                                        CompleteReceive(active_pdu);
                                        active_pdu = null;
                                        break;
                                    case XBeePDU.PushStatus.RESET:
                                        active_pdu = null;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Stack._read_unautorized_access_exceptions++;
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    return;
                }
                catch (IOException e)
                {
                    Stack._read_io_exceptions++;
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    return;
                }
                catch (Exception e)
                {
                    Stack._read_unknow_exceptions++;
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    return;
                }
            }
        }

        #region Administracion de Nodos Visibles
        public delegate void NodeChangeHandler(XBeeAPIPort sender, XBeeNode node);

        public event NodeChangeHandler NodeComesUp;
        public event NodeChangeHandler NodeComesDown;
        
        readonly Dictionary<string, XBeeNode> visible_nodes = new Dictionary<string,XBeeNode>();

        public XBeeNode FindNode(ushort addr)
        {
            lock (visible_nodes)
            {
                foreach (var node in visible_nodes.Values)
                {
                    if (node.Address == addr) return node;
                }
                return null;
            }
        }

        public void UpdateNode(XBeeNode node)
        {
            lock (visible_nodes)
            {
                if (visible_nodes.ContainsKey(node.Id))
                {
                    visible_nodes[node.Id].LastUpdate = DateTime.Now;
                }
                else
                {
                    node.LastUpdate = DateTime.Now;
                    visible_nodes.Add(node.Id, node);
                    if (NodeComesUp != null) 
                        NodeComesUp(this, node);
                }
            }
        }

        private void DeleteLostNodes()
        {
            lock (visible_nodes)
            {           
                var todelete = new List<string>();
                foreach (var nd in visible_nodes.Values)
                {
                    var age = DateTime.Now - nd.LastUpdate;
                    if (age.TotalSeconds > 300)
                    {
                        todelete.Add(nd.Id);
                        if (NodeComesDown != null) NodeComesDown(this, nd);
                    }
                }
                foreach (var id in todelete)
                {
                    visible_nodes.Remove(id);
                }
            }
        }
        #endregion

        private void CompleteReceive(XBeePDU pdu)
        {
            try
            {
                STrace.Debug(GetType().FullName, "Recibo PDU");
                if (pdu.Decode())
                {
                    STrace.Debug(GetType().FullName, String.Format("Recibo PDU - Decodificada APIID={0:X}", pdu.IdAPI));
                    if (pdu.IdAPI == 0x88) // AT Command Response.
                    {
                        var tr = tr_layer.Get(pdu.Sequence);
                        // Respuestas a comandos AT eliminan la transaccion.-
                        if (tr != null)
                        {
                            tr.End();
                            tr_layer.Remove(pdu.Sequence);
                        }

                        // A que comando AT se referia?
                        if (pdu.Data[0] == 'M' && pdu.Data[1] == 'Y')
                        {
                            test_failure = 0;
                            MyXbeeAddress = (ushort) ((pdu.Data[2] << 8) + pdu.Data[3]);
                            // Aqui determinamos ENLINEA
                            Estado = Estados.MODULO_ENLINEA;
                        }
                        else if (pdu.Data[0] == 'N' && pdu.Data[1] == 'D')
                        {
                            var cursor = 2;
                            while ((pdu.Data.GetLength(0) - cursor) > 0)
                            {
                                var nodo = new XBeeNode {Address = ((ushort) (pdu.Data[cursor++] << 8))};
                                nodo.Address += pdu.Data[cursor];
                                cursor += 9;
                                nodo.Signal = (char) pdu.Data[cursor++];
                                var id = new byte[20];
                                var chr = pdu.Data[cursor++];
                                var idpos = 0;
                                while (chr != 0)
                                {
                                    id[idpos++] = chr;
                                    chr = pdu.Data[cursor++];
                                }
                                nodo.Id = Encoding.ASCII.GetString(id, 0, idpos);
                                nodo.Trace("NODO DETECTADO:");
                                UpdateNode(nodo);
                            }
                        }
                    }

                    if (pdu.IdAPI == 0x89)
                    {
                        var tr = tr_layer.Get(pdu.Sequence);
                        if (tr != null)
                        {
                            if (pdu.Ack)
                                tr.End();
                            else
                                tr.Cancel();

                            tr_layer.Remove(pdu.Sequence);
                        }
                        else
                        {
                            Stack._acks_too_late++;
                        }
                    }
                    else if (pdu.IdAPI == 0x81)
                    {
                        if (PDURecibida != null)
                        {
                            PDURecibida(this, pdu);
                        }
                        return;
                    }
                    else if (pdu.IdAPI == 0x8A)
                    {
                        if (HardwareStatus != null)
                        {
                            HardwareStatus(this, pdu);
                        }
                        return;
                    }
                }
                else
                {
                    STrace.Debug(GetType().FullName,"XBEE: Recibo PDU - Error al decodificar");
                }
            }
			catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
            }
        }
        #endregion

        #region Mecanismos de control de errores.
        private int transient_uart_errors;
        private void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            lock (uartLocker)
            {
                transient_uart_errors++;
                STrace.Debug(GetType().FullName, "ErrorReceived: " + e.EventType);
                if (transient_uart_errors == 10)
                {
                    transient_uart_errors = 0;
                    Estado = Estados.PUERTO_CERRADO;
                }
            }
        }
        #endregion
        
        #region Mecanismos de Estado del puerto
        private void RutinaDeControl(object sender)
        {
            if (inside_timer) return;
            inside_timer = true;
            DeleteLostNodes();
            var elapsed_time = DateTime.Now.Subtract(last_received_data);
            Stack._last_read_seconds = (int) elapsed_time.TotalSeconds;
            /*
            STrace.Trace(GetType().FullName,2, "XBEEPORT[{0}]: Faltan {1} para Timeout", SerialPort,  (SerialConnectionTimeout - elapsed_time.TotalSeconds));
            if (Stack._transient_read_timedout_guard >= 10)
            {
                STrace.Trace(GetType().FullName,"XBEEPORT[{0}]: Reniciando puerto por exceso de Timeout en rutina de control.", SerialPort);
                Estado = Estados.PUERTO_CERRADO;
                Stack._transient_read_timedout_guard = 0 ;
            }
            if (Estado != Estados.PUERTO_CERRADO && elapsed_time.TotalSeconds > SerialConnectionTimeout)
            {
                STrace.Trace(GetType().FullName,"XBEEPORT[{0}]: Timeout en rutina de control.", SerialPort);
                Stack._read_timedout_guard++;
                Stack._transient_read_timedout_guard++;
            }*/
            if (estado == Estados.PUERTO_CERRADO)
            {
                STrace.Debug(GetType().FullName, String.Format("XBEEPORT[{0}]: Reabriendo puerto.", SerialPort));
                ReOpenSerialPort();
            }
            /*else
            {
                STrace.Trace(GetType().FullName,1, "XBEEPORT[{0}]: Iniciando test del modem.", SerialPort);
                //TestConnection();
            }*/
            inside_timer = false;
        }


        private int test_done;
        private int test_failure;
        private void TestConnection()
        {
            if (test_failure > 32)
            {
                STrace.Debug(GetType().FullName, String.Format("XBEEPORT[{0}]: Cerrando puerto por que no reponde.", SerialPort));
                Estado = Estados.PUERTO_CERRADO;
                test_failure = 0;
                return;
            }
            test_done++;
            test_failure++;
            ATCommand("MY"); // solicito la direccion local del movil
            // si lo hago muy seguido me mata la performance del modulo.
            if (test_done % 10 == 0)
            {
                ATCommand("ND"); // solicito descubrir los Xbee visibles por la base.
            }
        }

        public void SetChannel(byte ch)
        {
            var data = new byte[3];
            data[0] = (byte)'C';
            data[1] = (byte)'H';
            data[2] = ch;
            ATCommand(data);
        }

        public void RemoteATCommand(ushort address, string command)
        {
            lock (uartLocker)
            {
                if (uart == null || Estado == Estados.PUERTO_CERRADO) return;
                // to do: el 0x00 al final de los strings.
                var data = Encoding.ASCII.GetBytes(command);
                var pdu = new XBeePDU(address, 0x17, data);
                tr_layer.Create(pdu, active_tx_timeout);
                var buf = pdu.ToByteArray();
                try
                {
                    STrace.Debug(GetType().FullName, "REMOTE AT");
                    Stack._writed_bytes += buf.GetLength(0);
                    Write(buf);
                    return;
                }
                catch (UnauthorizedAccessException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_unautorized_access_exceptions++;
                    return;
                }
                catch (IOException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_io_exceptions++;
                    return;
                }
                catch (InvalidOperationException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._invalid_operation_exception++;
                    return;
                }
                catch (TimeoutException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._timeout_exceptions++;
                    return;
                }

            }            
        }

        private void ATCommand(string command)
        {
            ATCommand(Encoding.ASCII.GetBytes(command));
        }

        private void ATCommand(byte[] data)
        {
            lock (uartLocker)
            {
                if (uart == null || Estado == Estados.PUERTO_CERRADO) return;
                // to do: el 0x00 al final de los strings.
                var pdu = new XBeePDU(0, 0x08, data);
                tr_layer.Create(pdu, active_tx_timeout);
                var buf = pdu.ToByteArray();
                try
                {
                    STrace.Debug(GetType().FullName,"Enviando AT ");
                    Stack._writed_bytes += buf.GetLength(0);
                    Write(buf);
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    STrace.Debug(GetType().FullName, "Exception: UnauthorizedAccessException.");
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_unautorized_access_exceptions++;
                    return;
                }
                catch (IOException)
                {
                    STrace.Debug(GetType().FullName, "Exception: IOException.");
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._write_io_exceptions++;
                    return;
                }
                catch (InvalidOperationException)
                {
                    STrace.Debug(GetType().FullName, "Exception: InvalidOperationException.");
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._invalid_operation_exception++;
                    return;
                }
                catch (TimeoutException)
                {
                    STrace.Debug(GetType().FullName, "Exception: TimeoutException.");
                    Estado = Estados.PUERTO_CERRADO;
                    Stack._timeout_exceptions++;
                    return;
                }

            }
        }             

        private void ReOpenSerialPort()
        {
            lock (uartLocker)
            {
                if (uart != null)
                {
                    try
                    {
                        if (uart.IsOpen)
                        {
                            STrace.Debug(GetType().FullName, "ReOpenSerialPort: Close!");
                            Estado = Estados.PUERTO_CERRADO;
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        STrace.Debug(GetType().FullName, "ReOpenSerialPort: Close Exception: " + e.GetType());
                        Stack._uart_unknow_exceptions++;
                        Estado = Estados.PUERTO_CERRADO;                        
                        return;
                    }
                }
                try
                {
                    Stack._uart_resets++;
                    uart = new SerialPort();
                    last_received_data = DateTime.Now;
                    uart.BaudRate = rate;
                    uart.PortName = serialport;
                    uart.Handshake = handshake;
                    uart.DataBits = databits;
                    uart.Parity = parity;
                    uart.StopBits = stopbits;
                    uart.WriteTimeout = 1800;
                    uart.WriteBufferSize = 8192;
                    uart.ReadTimeout = 1500;
                    uart.ReadBufferSize = 8192;
                    uart.ReceivedBytesThreshold = 3;
                    uart.DataReceived += DataReceived;
                    uart.ErrorReceived += ErrorReceived;
                    uart.Encoding = Encoding.UTF8;
                    uart.Open();
                    uart.DtrEnable = true;
                    if (uart.Handshake != Handshake.RequestToSend)
                    {
                        uart.RtsEnable = true;
                    }
                    last_received_data = DateTime.Now;
                    unescape_next = false;

                } catch (Exception e)
                {
                    
                    Stack._uart_unknow_exceptions++;
                    STrace.Debug(GetType().FullName, "ReOpenSerialPort: Open Exception: " + e.GetType());
                    STrace.Debug(GetType().FullName, "ReOpenSerialPort: StackTrace: " + e.StackTrace);
                    Estado = Estados.PUERTO_CERRADO;
                    return;
                }
                try
                {
                    if (uart.IsOpen)
                    {
                        Estado = Estados.PUERTO_ABIERTO;
                    }
                    else
                    {
                        STrace.Debug(GetType().FullName, "ReOpenSerialPort: test dio que esta cerrado.");
                        Estado = Estados.PUERTO_CERRADO;
                        return;
                    }
                }
                catch (Exception e)
                {
                    Stack._uart_unknow_exceptions++;
                    STrace.Debug(GetType().FullName, "ReOpenSerialPort: Test Exception: " + e.GetType());
                    Estado = Estados.PUERTO_CERRADO;
                    return;
                }

            }
        }

        #endregion

        #region Variables Privadas
        // control de flujo UART.
        private Handshake handshake = Handshake.None;
        private Parity parity = Parity.None;
        private StopBits stopbits = StopBits.One;
        private int databits = 8;

        private Estados estado = Estados.PUERTO_CERRADO;
        private SerialPort uart;        
        private XBeePDU active_pdu;
        private readonly Timer port_timer;
        private bool inside_timer;
        private String serialport;
        private int rate;
        private readonly object uartLocker = new object();
        private DateTime last_received_data;
        private const int active_tx_timeout = 5000;
        internal XBeeTrLayer tr_layer = new XBeeTrLayer();

        #endregion
    }
}
