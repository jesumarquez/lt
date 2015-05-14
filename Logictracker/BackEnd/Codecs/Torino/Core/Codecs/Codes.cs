namespace Urbetrack.Comm.Core.Codecs
{
    public class Codes
    {
        public enum HighCommand {
            // Solicitudes
            Discovery = 0x00,
            LoginRequest = 0x01,
            Reboot = 0x02,
            SystemReport = 0x03,
            KeepAlive = 0x04,
            SetParameter = 0x05,
            ShortMessage = 0x06,
            SetRider = 0x07,
            RemoteShell = 0x08,
            SetBootloaderAction = 0x09,

            // Mensajes AVL
            MsgPosicion = 0x10,
            //MsgTeclado = 0x11,
            //MsgTecladoTr = 0x12,
            MsgEvento = 0x13,

            // Entre Servidores (llevan options 1)
            HeartBeat = 0x50,   //!< anuncio de vida del server.
            Command   = 0x51,   //!< reenvio de paquete de un dispositivo.

            // Transferencia de Datos
            DATA_PAGE = 0x91,
            Query = 0x66,

            // Respuestas
            OKACK = 0x80,
            LoginAceptado = 0x81,

            // Errores Generales
            BadOptions = 0xB0,
            LoginRechazado = 0xB1,
            LoginRequerido = 0xB2,

            // QTree
            ErrorInterno = 0xDF,
        }

        public enum DecodeErrors
        {
            NoError,
            ChecksumError,
            BadLength,
            BadOptions,
            Ignore,
            UnknownMessage
        }
    }
}