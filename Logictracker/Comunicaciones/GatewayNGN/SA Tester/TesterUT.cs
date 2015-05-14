using System;
using System.IO;
using System.Text;
using SA.Mensajeria;
using SA.UsuariosTransaccion;

namespace SA
{
    class TesterUT : ServerUT
    {
        static private FileStream fs;
        public TesterUT()
        {
            try
            {
                string filename = String.Format("{0}\\systemreport.csv", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
                filename = filename.Substring(6);
                Marshall.User(filename);
                fs = File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read);
                SystemReport += TesterUT_SystemReport;
            }
            catch (Exception e)
            {
                Marshall.Error(String.Format("SYSTEM REPORT DESHABILITADO!!! ERROR={0}", e));
            }
            AutoReport += TesterUT_AutoReport;
            RFIDDetected +=TesterUT_RFIDDetected;
            ExcesoVelocidad +=TesterUT_ExcesoVelocidad;
        }

        private static void TesterUT_SystemReport(object sender, SystemReport pdu)
        {
            Marshall.User("        GPS_FixedSeconds: {0}",pdu.GPS_FixedSeconds);
            Marshall.User("        GPS_BlindSeconds: {0}", pdu.GPS_BlindSeconds);
            Marshall.User("        GPS_Resets: {0}", pdu.GPS_Resets);
            Marshall.User("        MODEM_Resets: {0}", pdu.MODEM_Resets);
            Marshall.User("        NETWORK_UDP_ReceivedBytes: {0}", pdu.NETWORK_UDP_ReceivedBytes);
            Marshall.User("        NETWORK_UDP_SentBytes: {0}", pdu.NETWORK_UDP_SentBytes);
            Marshall.User("        NETWORK_UDP_ReceivedDgrams: {0}", pdu.NETWORK_UDP_ReceivedDgrams);
            Marshall.User("        NETWORK_UDP_SentDgrams: {0}", pdu.NETWORK_UDP_SentDgrams);
            Marshall.User("        NETWORK_Resets: {0}", pdu.NETWORK_Resets);
            Marshall.User("        WatchDogResets: {0}", pdu.WatchDogResets);
            Marshall.User("        SystemResets: {0}", pdu.SystemResets);
            Marshall.User("System Report: ");
            if (fs == null) return;
            var report = String.Format("{0:yyyyMMddhhmmss},{1},{2},{3},{4},{5}\r\n",
                                          DateTime.Now, pdu.IdDispositivo, pdu.NETWORK_UDP_ReceivedBytes,
                                          pdu.NETWORK_UDP_ReceivedDgrams, pdu.NETWORK_UDP_SentBytes,
                                          pdu.NETWORK_UDP_SentDgrams);
            var buff = Encoding.ASCII.GetBytes(report);
            fs.BeginWrite(buff, 0, buff.GetLength(0), TesterUT_SystemReport_Commit, null);
        }

        internal static void TesterUT_SystemReport_Commit(IAsyncResult state)
        {
            Marshall.User("System Report Commited!");
            fs.EndWrite(state);
            fs.Flush();
        }

        private void TesterUT_ExcesoVelocidad(object sender, ExcesoVelocidad pdu)
        {
            if (pdu.CL == 0x01)
            {
                Marshall.User("El dispositivo {0} exedio la velocida maxima velocidad={1}", pdu.IdDispositivo,
                              pdu.VelocidadMaximaAlcanzada);
            }
            else
            {
                Marshall.User("El dispositivo {0} vuelve a velocidad normal. la velocida maxima velocidad alcanzada={1}", pdu.IdDispositivo, pdu.VelocidadMaximaAlcanzada);
            }
        }

        internal void TesterUT_AutoReport(object sender, Posicion pdu)
        {
            foreach (string s in pdu.TraceFull()) {
                Marshall.User(s);
            }
            Marshall.User("**** Autoreporte recibido:");
        }

        internal void TesterUT_RFIDDetected(object sender, RFIDDetectado pdu)
        {
            foreach (string s in pdu.TraceFull())
                Marshall.User(s);
            Marshall.User("**** RFID recibido:");
        }
    }
}
