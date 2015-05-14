using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using etao.marshall;

namespace avl2cmd
{
    public class cmd_acceptor : tcp_acceptor
    {
        public bool _connected = false;
        static ILogger LOG;
        
        public static void SETLOG(ILogger log) {
            LOG = log;
        }

        public override void on_accept()
        {
            LOG.log("CMD Conectado!");
            _connected = true;
            LOG.take(this);
        }

        public override void on_data()
        {
            LOG.log(String.Format("DATA:[{0}];",Encoding.ASCII.GetString(get_buffer(),0,get_read_bytes())));
            if ((get_buffer()[0] == '?') // WAKEUP
                || (get_buffer()[0] == '!') // ATTACK DELAY
                || (get_buffer()[0] == 'T') // GPS Autostatus
                || (get_buffer()[0] == 'T') // TEXT MESSAGE
                || (get_buffer()[0] == 'M')) // PREDEFINED MESSAGE
            {
                // esta version no hace nada con lo que pida el command data asi que simplemente nos limitamos a contestar con un ACK
                send(CMDPDU.encode_ack());
                LOG.log(String.Format("Mensaje del HOST type={0}, se contesto con ACK y se ignoro.", get_buffer()[0]));
            }
            else if (get_buffer()[0] == 0x06 || get_buffer()[0] == 'A') // RECIBIMOS ACK
            {
                LOG.ack(true);
            }
            else
            {
                // cualquier rta incomprensible decimos NACK..
                LOG.ack(false);
            }
        }

        public override void on_close()
        {
            LOG.log("CMD Desconectado!");
            _connected = false;
        }
    }
}
