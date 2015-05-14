import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.InetSocketAddress;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import etao.comm.GatewayJClient;
import etao.comm.MessageIn;
import etao.comm.Position;
import etao.comm.GatewayJClient.ACKNACKEvent;
import etao.comm.GatewayJClient.Logger;
import etao.comm.GatewayJClient.MessageInEvent;
import etao.comm.GatewayJClient.PositionEvent;
import etao.comm.GatewayJClient.StatusChangeEvent;

public class TestClientETAO {

  //noPosition 156 190.2.37.141 2020 30 (cese de alerta)
  //reconfigure 156 190.2.37.141 2020 30
  //reboot 156
  //communicationEnd 156 50 60 70 (cese de audio)
  //requiredPosition 156 190.2.37.141 2020 30
  //c1 (velocidades)


/*
	private static Pattern RECONFIGURE = Pattern.compile("reconfigure (\\d)+ ([\\d\\.]+) ([\\d]{1,4}) ([\\d]+)");
	private static Pattern RETRIEVE = Pattern.compile("retrieve ([\\d\\w]+)");
	private static Pattern ASKDEVICE = Pattern.compile("ask ([\\d]+) ([\\d]+)");
*/
    private static Pattern RECONFIGURE = Pattern.compile("reconfigure ([\\d]+) ([\\d\\.]+) ([\\d]{1,4}) ([\\d]+)");
    private static Pattern SPEED = Pattern.compile("c1 ([\\d]+)");
    private static Pattern REBOOT = Pattern.compile("reboot ([\\d]+)");
    private static Pattern WORKFLOW = Pattern.compile("wf ([\\d]+) ([\\d]+)");
    private static Pattern RETRIEVE = Pattern.compile("retrieve ([\\d\\w]+)");
    private static Pattern COMMUNICATION_END = Pattern.compile("communicationEnd ([\\d]+) ([\\d\\.]+) ([\\d]{1,4}) ([\\d]+)");
    private static Pattern NO_POSITION = Pattern.compile("noPosition ([\\d]+) ([\\d\\.]+) ([\\d]{1,4}) ([\\d]+)");
    private static Pattern REQUIRED_POSITION = Pattern.compile("requiredPosition ([\\d]+) ([\\d\\.]+) ([\\d]{1,4}) ([\\d]+)");

    public static void AddDevice(GatewayJClient client, int id, String IMEI, String Type, String password)
    {
    	etao.comm.Device etaoDevice = new etao.comm.Device();
    	etaoDevice.id = id;
        etaoDevice.type = Type;
        etaoDevice.IMEI = IMEI;
        etaoDevice.password = password;
        etaoDevice.UserSettings.put("devKeepAliveLapse", "2");
        etaoDevice.UserSettings.put("devKeepAliveMessages", "3");
        client.add(etaoDevice);
    }

    public static void main(String[] args) {
        GatewayJClient client = GatewayJClient.instance();

        // WISH help
        //      System.out.println("Usage: TestClientETAO id imei pwd host port");


        client.init("java911", null, "comandos");

        // add x parametros
        if (args.length == 5) {
            AddDevice(client, Integer.valueOf(args[0]).intValue(), args[1], "Urbetrack.Unetel.v1.Parser, Urbetrack.Unetel.v1", args[2]);
        }

        // add harcoded
        AddDevice(client, 152, "0355826017394577", "Urbetrack.Unetel.v2.Parser, Urbetrack.Unetel.v2", "telequip");
        AddDevice(client, 153, "0356889013679457", "Urbetrack.Unetel.v1.Parser, Urbetrack.Unetel.v1", "telequip");
        AddDevice(client, 154, "0355826010625092", "Urbetrack.Unetel.v1.Parser, Urbetrack.Unetel.v1", "telequip");
        AddDevice(client, 155, "0355826010637204", "Urbetrack.Unetel.v1.Parser, Urbetrack.Unetel.v1", "telequip");
        AddDevice(client, 156, "0356889013476045", "Urbetrack.Unetel.v1.Parser, Urbetrack.Unetel.v1", "telequip");
        AddDevice(client, 157,  "354779035412031", "Urbetrack.Trax.v1.Parser, Urbetrack.Trax.v1",     "gte123");
        client.registerHandler(new DummyHandler());

        try {
            BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
            String cmd = null;
            while (!"quit".equals(cmd)) {
                cmd = in.readLine();

                if (cmd.startsWith("reconfigure")) {
                    Matcher matcher = RECONFIGURE.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Reconfigurar el dispositivo " + matcher.group(1)+ " al ip " + matcher.group(2) + ", puerto " + matcher.group(3) + " fr. " + matcher.group(4));
                        String reconfig = "/Device.SetParameter?devId=" + Integer.valueOf(matcher.group(1)) + "&cfgParameter=config_0&cfgValue="+ matcher.group(3) + "," + matcher.group(2) + "," + matcher.group(4) + ",3,45";
                        client.request(reconfig, 123456, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de reconfiguración");
                    } else {
                        System.out.println("El formato del comando reconfigure es inválido. Uso: reconfigure id ip puerto frecuencia");
                    }

                }else if (cmd.startsWith("reboot")) {
                	Matcher matcher = REBOOT.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Reboot del dispositivo " + matcher.group(1));
                        String reconfig = "/Device.Reboot?devId=" + Integer.valueOf(matcher.group(1));
                        client.request(reconfig, 9876, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de Reboot");
                    } else {
                        System.out.println("El formato del comando Reboot es inválido. Uso: reboot id");
                    }

                }else if (cmd.startsWith("wf")) {
                	Matcher matcher = WORKFLOW.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Ciclo Logistico del dispositivo " + matcher.group(1));
                        String reconfig = "/Device.SetWorkflowState?devId=" + Integer.valueOf(matcher.group(1)) + "&newState=" + Integer.valueOf(matcher.group(2));
                        client.request(reconfig, 54321, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de Ciclo Logistico");
                    } else {
                        System.out.println("El formato del comando Ciclo Logistico es inválido. Uso: wf id newState");
                    }

                }else if (cmd.startsWith("c1")) {
                    Matcher matcher = SPEED.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Velocidades del dispositivo " + matcher.group(1));
                        String reconfig = "/Device.SetParameter?devId=" + Integer.valueOf(matcher.group(1)) + "&cfgParameter=config_1&cfgValue=100,60,5,15";
                        client.request(reconfig, 3232321, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de Velocidades");
                    } else {
                        System.out.println("El formato del comando reconfigure es inválido. Uso: reconfigure id ip puerto frecuencia");
                    }

                }else if(cmd.startsWith("communicationEnd")){
                    Matcher matcher = COMMUNICATION_END.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Terminar la comunicación con el dispositivo " + matcher.group(1));
						String reconfig = "/Device.SetParameter?devId=" + Integer.valueOf(matcher.group(1)) + "&cfgParameter=P&cfgValue=0";
                        client.request(reconfig, 62231, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de fin de la comunicación");
                    } else {
                        System.out.println("El formato del comando communicationEnd es inválido. Uso: communicationEnd id ip puerto frecuencia");
                    }
                }else if (cmd.startsWith("noPosition")){
                    Matcher matcher = NO_POSITION.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Cese de alerta y envió de posiciones del dispositivo " + matcher.group(1));
                      	String reconfig = "/Device.SetParameter?devId=" + Integer.valueOf(matcher.group(1)) + "&cfgParameter=P&cfgValue=2";
                        client.request(reconfig, 4454, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de no enviar más posiciones");
                    } else {
                        System.out.println("El formato del comando noPosition es inválido. Uso: noPosition id ip puerto frecuencia");
                    }

                }else if(cmd.startsWith("requiredPosition")){
                    Matcher matcher = REQUIRED_POSITION.matcher(cmd);
                    if (matcher.matches()) {
                        System.out.println("Solicitud de envió de posición del dispositivo " + matcher.group(1));
                        String reconfig = "/Device.SetParameter?devId=" + Integer.valueOf(matcher.group(1)) + "&cfgParameter=P&cfgValue=1";
                        client.request(reconfig, 32231, 300, Integer.valueOf(matcher.group(1)));
                        System.out.println("Se envió el mensaje de solicitud de posición al dispositivo");
                    } else {
                        System.out.println("El formato del comando requiredPosition es inválido. Uso: requiredPosition id ip puerto frecuencia");
                    }
                }
            }
        } catch (IOException ioe) {
            ioe.printStackTrace();
        } finally {
            //Cerrar el cliente que no tiene stop...
        }
        System.exit(0);
    }

    public static class DummyHandler implements PositionEvent, StatusChangeEvent, MessageInEvent, ACKNACKEvent, Logger {
        public void Position(Position pos) {
            System.out.println(String.format("Se recibió posición de %s, en %s, %s. TS: %s", pos.id, pos.lat,
                    pos.lon, pos.timestamp));
        }
        public void StatusChange(boolean isOnline) {
            System.out.println("¿Conectado? " + isOnline);
        }

        public void MessageIn(MessageIn msg) {
            System.out.println("Se recibió mensaje: Codigo=" + msg.code + "; Texto=" + msg.message);
        }

        public void Pong(String pong) {
            //System.out.println("Pong " + pong);
        }

        public void PendingQueue(String queue, int cantidad) {
            System.out.println("Cola con mensajes pendientes: " + queue + " cantidad: " + cantidad) ;
        }

        public void ACK(int ackMsgId) {
            System.out.println("ACK para el mensaje " + ackMsgId);
        }

        public void NACK(int nackMsgId) {
            System.out.println("NACK para el mensaje " + nackMsgId);
        }
        public void Debug(String msg) {
            //System.out.println("Debug: " + msg);

        }
        public void Error(String msg) {
            System.out.println("Error: " + msg);
        }
        public void Info(String msg) {
            System.out.println("Info: " + msg);
        }
    }
}