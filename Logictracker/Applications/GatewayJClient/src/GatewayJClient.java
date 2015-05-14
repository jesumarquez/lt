package etao.comm;

import ionic.Msmq.Queue;
import ionic.Msmq.Message;
import ionic.Msmq.MessageQueueException;

import java.nio.*;
import java.nio.channels.*;
import java.net.*;
import java.io.*;
import java.nio.channels.spi.*;
import java.nio.charset.*;
import java.lang.*;
import java.util.*;
import java.net.*;
import java.io.*;
import org.xml.sax.*;
import org.xml.sax.helpers.*;

/** Cliente Java de acceso al gateway Urbetrack.
 */
public class GatewayJClient
{
	/// tipos enumerados
	public enum Estados {
		WAIT_FOR_INITIALIZE,
    	TRYING,
    	CONNECTED,
    	TIME_WAIT
    };

	/// tipos de eventos
	public interface Event {
    };

	public interface StatusChangeEvent extends Event {
		   public void StatusChange(boolean connected);   //!< true=conectado/false=desconectado
		   public void PendingQueue(String name, int messages); //!< name
		   public void Pong(String signature);   //!< recibe la rta de un ping.
	}

	public interface Logger extends Event {
		   public void Debug(String text);
		   public void Info(String text);
		   public void Error(String text);
	}

	public interface PositionEvent extends Event {
		   public void Position(Position p); //!< Objeto que almacena la posicion.
	}

	public interface MessageInEvent extends Event {
		   public void MessageIn(MessageIn mi);		  //!< Objeto que alamcena el mensaje entrante
	}

	public interface ACKNACKEvent extends Event {
		   public void ACK(int msgId);		  //!< id del mensaje confirmado positivamente
		   public void NACK(int msgId);		  //!< id del mensaje confirmado negativamente
	}

	public interface KeepAliveEvent extends Event {
		   public void KeepAlive(KeepAlive keepalive);   //!< Objecto que almacena el KeepAlive
	}

	public interface LoginEvent extends Event {
	       public void Login(int deviceId);   //!< Id del equipo que se logueo
	}

	///log
	private void Debug(String txt) {
		if (logger != null)	{
			logger.Debug(txt);
			return;
		}
		System.out.println("DEBUG:" + txt);
	}

	private void Error(String txt) {
		if (logger != null)	{
			logger.Error(txt);
			return;
		}
		System.out.println("ERROR:" + txt);
	}

	/// datos
	private StatusChangeEvent statusChangeEvent = null;
	private Logger logger = null;
	private PositionEvent positionEvent = null;
	private MessageInEvent messageInEvent = null;
	private ACKNACKEvent ackNackEvent = null;
	private KeepAliveEvent keepAliveEvent = null;
	private LoginEvent loginEvent = null;
	private Map<Integer, Device> devices = new HashMap<Integer, Device>();
	private Map<String, Queue> devicesQueues = new HashMap<String, Queue>();
	static private Map<String, String> devicesTypesQueueNames = new HashMap<String, String>();
	private RecvThread rt = null;
	private StateThread st = null;
	private Estados estado;
	static private GatewayJClient _instance_ = null;
	private Queue EntranteQ= null;
	private String SalienteQbasename;
	private String Host;
	private long timeOfLastMsg = new Date().getTime();

	private int RECEIVE = 0x01;
	private int SEND = 0x02;
	private int SEND_RECEIVE = 0x03;

	/// pattern - Singleton
	static public GatewayJClient instance() {
		if (_instance_ == null) {
			_instance_ = new GatewayJClient();
			//la dll en c++ que nos da la interface con msmq solo permite 10 colas, incluyendo la de eventos, no pasarse de ese numero!!!
			devicesTypesQueueNames.put("Urbetrack.Unetel.v1.Parser, Urbetrack.Unetel.v1", "UnetelV1Parser");
			devicesTypesQueueNames.put("Urbetrack.Unetel.v2.Parser, Urbetrack.Unetel.v2", "UnetelV2Parser");
			devicesTypesQueueNames.put("Urbetrack.MiniMT.v1.Parser, Urbetrack.MiniMT.v1", "MiniMTParser");
			devicesTypesQueueNames.put("Urbetrack.FulMar.v1.Parser, Urbetrack.FulMar.v1", "FulMarParser");
			devicesTypesQueueNames.put("Urbetrack.GsTraq.v1.Parser, Urbetrack.GsTraq.v1", "GsTraqParser");
			//devicesTypesQueueNames.put("Urbetrack.MaxTrack.v1.Parser, Urbetrack.MaxTrack.v1", "MaxTrackParser");
			//devicesTypesQueueNames.put("Urbetrack.RedSos.v1.Parser, Urbetrack.RedSos.v1", "RedSosParser");
			devicesTypesQueueNames.put("Urbetrack.SimCom.v1.Parser, Urbetrack.SimCom.v1", "SimComParser");
			devicesTypesQueueNames.put("Urbetrack.Trax.v1.Parser, Urbetrack.Trax.v1", "TraxParser");
			devicesTypesQueueNames.put("Urbetrack.SkyPatrol.v1.Parser, Urbetrack.SkyPatrol.v1", "SkyPatrolParser");
		}
		return _instance_;
	}

	private GatewayJClient(){
			estado = Estados.WAIT_FOR_INITIALIZE;
	}

	/// Primitivas del Registro de dispositivos.
	public void remove(Device device) {
		devices.remove(device.id);
		if (!isConnected()) return;
		write(String.format("/Device.Delete?devId=%d", device.id), device.id);
	}

	public void add(Device device) {
		devices.put(device.id, device);
		if (!isConnected()) return;
		write(EncodeAdd(device), device.id);
	}

	private void SendRegisterDevices() {
		Iterator it = devices.entrySet().iterator();
		while (it.hasNext()) {
			Map.Entry pairs = (Map.Entry)it.next();
			Info("Registrando dispositivo: " + pairs.getKey());
			Device d =(Device)pairs.getValue();
			write(EncodeAdd(d), d.id);
		}
	}

    private String EncodeAdd(Device d) {
    	String buf = String.format("/Device.Add?devId=%d&devClassType=%s&devIMEI=%s", d.id, d.type, d.IMEI);

    	for (String key : d.UserSettings.keySet())
    	{
    		try
    		{
	    		String value = URLEncoder.encode(d.UserSettings.get(key), "UTF-8");
	        	buf += "&" + key + "=" + value;
    		}
    		catch (UnsupportedEncodingException e)
    		{
    		}
    	}
    	return buf;
    }
	///

	public void init(String entrante, String host, String saliente) {

		if (estado != Estados.WAIT_FOR_INITIALIZE)
		{
			Error("Error de uso, no llame a \"init(String, String, String)\" mas de una vez para evitar este mensaje. ");
			Error("  no se modifico la configuracion.");
			return;
		}

		if (host == null || host.length() == 0) host = ".";

		Host = host;
		EntranteQ = Open(entrante, host, RECEIVE);
		SalienteQbasename = saliente;

		st = new StateThread("State Thread");
		estado = Estados.TIME_WAIT;
		st.start();
		startReader();
	}

	public boolean ping(String signature) {
		if (estado != Estados.CONNECTED) return false;
		return isConnected();
	}

	public boolean request(String msg, int trackingId, int timeToReceive, Integer DeviceId)
	{
		if (write(msg + String.format("&trackingId=%d&timeToReceive=%d",trackingId,timeToReceive),DeviceId) != 0) return true;
		return false;
	}

	public void registerHandler(Event ev)
	{
		if (ev instanceof PositionEvent) {
			positionEvent = (PositionEvent) ev;
		}
		if (ev instanceof StatusChangeEvent) {
			statusChangeEvent = (StatusChangeEvent) ev;
		}
		if (ev instanceof MessageInEvent) {
			messageInEvent = (MessageInEvent) ev;
		}
		if (ev instanceof Logger) {
			logger = (Logger) ev;
		}
		if (ev instanceof ACKNACKEvent) {
			ackNackEvent = (ACKNACKEvent) ev;
		}
		if (ev instanceof LoginEvent) {
			loginEvent = (LoginEvent) ev;
		}
		if (ev instanceof KeepAliveEvent) {
			keepAliveEvent = (KeepAliveEvent) ev;
		}
	}

	public boolean isConnected() {
		return estado == Estados.CONNECTED;
	}

	public void close()
	{
		try
		{
			if (EntranteQ != null) EntranteQ.close();
			EntranteQ= null;

	    	for (String key : devicesQueues.keySet())
	    	{
	    		devicesQueues.get(key).close();
	    	}
			devicesQueues= null;
		}
		catch (Exception e)
		{
			Error(e.toString());
		}
	}

	private void Info(String txt) {
		if (logger != null)	{
			logger.Info(txt);
			return;
		}
		System.out.println("INFO:" + txt);
	}

	private void connect()
	{
		estado = Estados.TRYING;
		Debug("trying, entro.");
	   	Message msg = null;
		try {
			msg= EntranteQ.peek(2000); // timeout= 2000 ms
		} catch (Exception e) {
			msg= null;
		}

		if (msg == null)
			estado = Estados.TIME_WAIT;
		else
		{
			timeOfLastMsg = new Date().getTime();
			Debug("conectado, entro.");
			estado = Estados.CONNECTED;
		}
	}

	private int write(String message, Integer DeviceId) {
		try {
			Device dev = devices.get(DeviceId);
			if (dev == null) {
				Error("Dev=" + DeviceId + "; No tiene dispositivo asociado");
				return 0;
			}
			String qbasename = devicesTypesQueueNames.get(dev.type);
			if ((qbasename == null) || qbasename.equals("null")) {
				Error("Dev=" + DeviceId + "; Tipo=" + dev.type + "; No tiene cola asociada");
				return 0;
			}

			String qname = SalienteQbasename + "_" + qbasename;
			Queue queue;
			if (!devicesQueues.containsKey(qname))
			{
				queue= Open(qname, Host, SEND_RECEIVE);
				devicesQueues.put(qname, queue);
			}
			else
			{
				queue = devicesQueues.get(qname);
			}

			String mss = message.length() > 20?message.substring(0, 21) : message;
			System.out.println("MSJ: " + mss + " cola:" + queue.getName());


			String correlationID= "L:none";
			String label= message;
			if (label.length() > 100) //la biblioteca de acceso a la msmq limita el label a 100 caracteres, lastima que no son 200...
				label = label.substring(0, 101);//los primeros 100 caracteres, el endIndex es exclusive en java...
			int transactionFlag= 0; // 0 = NO TRANSACTION, 1= MTS, 2= XA, 3= SINGLE_MESSAGE
			Message msg= new Message("<?xml version=\"1.0\" encoding=\"UTF-8\"?><string>"+URLEncoder.encode(message)+"</string>", label, correlationID, transactionFlag);
			queue.send(msg);
			return message.length();
		} catch (Exception e) {
            Error("Dev=" + DeviceId + "; " + e.toString());
            return  0;
        }
    }

	private void onReceivePosition(Position pos)
	{
		if (positionEvent != null) positionEvent.Position(pos);
	}

	private void onStateChange(boolean is_online)
	{
		if (statusChangeEvent != null) statusChangeEvent.StatusChange(is_online);
	}

	private void onPong(String signature)
	{
		if (signature.indexOf("hb") == 0) {
			Debug("hearbeat recibido");
			return;
		}
		if (statusChangeEvent != null) statusChangeEvent.Pong(signature);
	}

	private void onPendingQueue(String queue, int messages)
	{
		if (statusChangeEvent != null) statusChangeEvent.PendingQueue(queue, messages);
	}

	private void onReceiveMessageIn(MessageIn msi)
	{
		if (messageInEvent != null) messageInEvent.MessageIn(msi);
	}

	private void onReceiveACK(boolean is_ack, int msgid) {
		if (ackNackEvent == null) return;
		if (is_ack) {
			ackNackEvent.ACK(msgid);
		} else {
			ackNackEvent.NACK(msgid);
		}
	}

	private void onReceiveKeepAlive(KeepAlive keepalive)
	{
		if (keepAliveEvent != null) keepAliveEvent.KeepAlive(keepalive);
	}

	private void onReceiveLogin(int device_id)
	{
		if (loginEvent != null) loginEvent.Login(device_id);
	}

	private void startReader()
	{
		rt = new RecvThread("Receive Thread");
		rt.start();
	}

	private void stopReader()
	{
		rt.val = false;
	}

	private void checkConnection()
	{
		if ((estado == Estados.CONNECTED) && ((timeOfLastMsg - new Date().getTime()) > 30000))
		{
			Info("El estado del socket ha cambiado a FALLA");
			onStateChange(false);
			estado = Estados.TIME_WAIT;
		} else if (estado == Estados.TIME_WAIT) {
			connect();
		}
	}

	private class StateThread extends Thread {
		private boolean val = true;

		private StateThread(String str) {
			super(str);
		}

		public void run()
		{
			try
			{
				while(val)
				{
					checkConnection();
					Thread.sleep(5000);
				}
			}
			catch (Exception e)
			{
				Error(e.toString());
			}
		}
	}

	private class RecvThread extends Thread
	{
		private boolean val = true;

		private RecvThread(String str)
		{
			super(str);
		}

		public void run()
		{
			String [] vals = null;
			try
			{
				Info("El JClient se ha iniciado");
				SendRegisterDevices();
				Message msg = null;
				while (val)
				{
					try {
						msg= EntranteQ.receive(2000); // timeout= 2000 ms
					} catch (Exception e) {
						msg= null;
						continue;
					}

					String result = msg.getMessage();
					result = result.replaceAll("<.xml version..1.0..>","");
					result = result.replaceAll("<string>","");
					result = result.replaceAll("<string />","");
					result = result.replaceAll("</string>","");
					result = result.trim();//*/

					System.out.println("MENSAJE RECIBIDO: [" + result + "]");

					if (result.indexOf("ST,OFFLINE") > -1) {
						Info("El estado del socket ha cambiado a OFFLINE");
						onStateChange(false);
					}

					if (result.indexOf("ST,ONLINE") > -1) {
						Info("El estado del socket ha cambiado a ONLINE");
						//ByteBuffer buf2 = ByteBuffer.wrap("INIT\n".getBytes());
						estado = Estados.CONNECTED;
						onStateChange(true);
						SendRegisterDevices();
					}

					if (result.indexOf("ACK,") == 0) {
						vals = result.split(",");
						onReceiveACK(true, Integer.valueOf(vals[1]).intValue());
					}

					if (result.indexOf("KA,") == 0) {
						vals = result.split(",");
						KeepAlive ka = new KeepAlive();
						try {
							ka.id = Integer.valueOf(vals[1]).intValue();
							ka.GPSFixed = Integer.valueOf(vals[2].trim()).intValue() == 1;
							ka.MainBattery = Integer.valueOf(vals[3].trim()).intValue() == 1;
							ka.AudioOk = Integer.valueOf(vals[4].trim()).intValue() == 1;
							ka.PanicState = Integer.valueOf(vals[5].trim()).intValue() == 1;
							ka.PanicDateTime = vals[6].trim();
							onReceiveKeepAlive(ka);
						} catch (NumberFormatException e)
						{
							Error(e.toString());
						}

					}

					if (result.indexOf("LOG,") == 0) {
						vals = result.split(",");
						onReceiveLogin(Integer.valueOf(vals[1]).intValue());
					}

					if (result.indexOf("PONG,") == 0) {
						vals = result.split(",");
						onPong(vals[1]);
					}

					if (result.indexOf("QUEUE,") == 0) {
						vals = result.split(",");
						onPendingQueue(vals[1],Integer.valueOf(vals[2]).intValue());
					}

					if (result.indexOf("NACK,") == 0) {
						vals = result.split(",");
						onReceiveACK(false, Integer.valueOf(vals[1]).intValue());
					}

					if (result.indexOf("MI,") == 0) {
						vals = result.split(",");
						MessageIn msi = new MessageIn();
						try {
							msi.id = Integer.valueOf(vals[1]).intValue();
							msi.code =  Integer.valueOf(vals[2].trim()).intValue();
							onReceiveMessageIn(msi);
						} catch (NumberFormatException e)
						{
							Error(e.toString());
						}
					}

					if (result.indexOf("Q,") > -1) {
						vals = result.split(",");
						Position pos = new Position();
						MessageIn msi = new MessageIn();
						try {
							msi.id = Integer.valueOf(vals[1]).intValue();
							msi.code = 0xFF;
							pos.id = Integer.valueOf(vals[1]).intValue();
							pos.timestamp = vals[2].toString();
							pos.lat = Double.valueOf(vals[3]).doubleValue();
							pos.lon = Double.valueOf(vals[4]).doubleValue();
							pos.vel = Double.valueOf(vals[5]).doubleValue();
							pos.rum = Integer.valueOf(vals[6].trim()).intValue();
							msi.pos = pos;
							onReceiveMessageIn(msi);
						} catch (NumberFormatException e)
						{
							Error(e.toString());
						}
					}

					if (result.indexOf("P,") > -1) {
						vals = result.split(",");
						Position pos = new Position();

						try {
							pos.id = Integer.valueOf(vals[1]).intValue();
							pos.timestamp = vals[2].toString();
							pos.lat = Double.valueOf(vals[3]).doubleValue();
							pos.lon = Double.valueOf(vals[4]).doubleValue();
							pos.vel = Double.valueOf(vals[5]).doubleValue();
							pos.rum = Integer.valueOf(vals[6].trim()).intValue();

							onReceivePosition(pos);
						} catch (NumberFormatException e)
						{
							Error(e.toString());
						}
					}
					//Debug("== Reader Ciclo Completo ==");
				}
			}
			catch (Exception e)
			{
				Error(e.toString());
				estado = Estados.TIME_WAIT;
				onStateChange(false);
			}
		}
	}

	private Queue Open(String qname, String hostname, int mode) {
		try {
			String fullname= getQueueFullName(hostname, qname);
			return new Queue(fullname, mode);
		} catch (Exception e) {
			Error(e.toString() + " Queue Name=" + qname);
		}
		return null;
	}

	private String getQueueFullName(String queueShortName) {
		return getQueueFullName(".", queueShortName);
	}

	private String getQueueFullName(String hostname, String queueShortName) {
		String h1= hostname;
		String a1= "OS";
		if ((h1==null) || h1.equals("")) h1=".";
		char[] c= h1.toCharArray();
		if ((c[0]>='1')	&& (c[0]<='9'))
			a1= "TCP";

		return "DIRECT=" + a1 + ":" + h1 + "\\private$\\" + queueShortName;
	}
}
