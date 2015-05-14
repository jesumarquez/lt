#region Usings

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Hacking;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class DataPage : PDU
    {
        public DataPage()
        {
            CH = (byte)Codes.HighCommand.DATA_PAGE;
            CL = 0x00; // 00 RESERVADO
            Saliente = true;
            Intento = 0;
        }

        public DataPage(byte[] buffer, int pos)
        {
            PDU this_pdu = this;
            Entrante = true;
            UrbetrackCodec.DecodeHeaders(buffer, ref this_pdu, ref pos);
            if (CL != 0x00) throw new Exception("No conincide CL con el Tipo FOTA DATA.");
            Pagina = UrbetrackCodec.DecodeShort(buffer, ref pos);
            Buffer = UrbetrackCodec.DecodeBytes(buffer, ref pos, 512);
        }

        public byte[] Buffer { get; set; }

        public short Pagina { get; set; }

        public short Lat { get; set; }
        public short Lon { get; set; }

        public short TotalDePaginas { get; set; }

        public short Intento { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            UrbetrackCodec.EncodeShort(ref buffer, ref pos, Pagina);
            if (CL >= 0x02) // DataPage Extendida.
            {
                UrbetrackCodec.EncodeShort(ref buffer, ref pos, TotalDePaginas);
                UrbetrackCodec.EncodeShort(ref buffer, ref pos, (short)Buffer.GetLength(0));
                var md5Hasher = MD5.Create();
                var firma = md5Hasher.ComputeHash(Buffer);
                if (firma.GetLength(0) != 16)
                {
                    throw new Exception("El tamaño del hash MD5 no es valido.");
                }

                if (Hacker.ServerUT.DataPageExSignatureError && (Pagina == 2 || Pagina == 4 || Pagina == 12) && Intento < 2)
                {
                    STrace.Debug(GetType().FullName, String.Format("DATAPAGE: Hacker de pagina {0}, Intento {1}, seq {2}", Pagina, Intento, Seq));
                    var lie = new byte[16];
                    UrbetrackCodec.EncodeBytes(ref buffer, ref pos, lie);
                } else {
                    UrbetrackCodec.EncodeBytes(ref buffer, ref pos, firma);
                }
            }
            UrbetrackCodec.EncodeBytes(ref buffer, ref pos, Buffer);
        }

        public override string Trace(string data)
        {
            return base.Trace("DATAPAGE");
        }

        public List<string> TraceFull()
        {
            var lista = new List<string>();
            lista.Insert(0, base.Trace("DATAPAGE"));
            lista.Insert(0, String.Format("\tPagina: {0} Tamaño: {1}", Pagina, Buffer.GetLength(0)));
            return lista;
        }
    }
}