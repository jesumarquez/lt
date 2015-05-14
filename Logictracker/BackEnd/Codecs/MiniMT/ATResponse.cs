namespace Logictracker.MiniMT.v1
{
    public class AtResponse
    {
        /*public bool FromByteArray(byte[] buffer, int size)
        {
            APINumber = Convert.ToInt16(buffer[0]) * 256 + Convert.ToInt16(buffer[1]);
            Tipo = Convert.ToInt16(buffer[2]);
            respuesta = Encoding.ASCII.GetString(buffer, 4, size - 4);
            return true;
        }*/

        /*public Posicion asPosicion(int id)
        {
            var partes = respuesta.Split(',');
            if (partes[0] != "$GPRMC") return null;
            if (partes[2] != "A") return null;
            var posi = "QP";
            posi += id.ToString().PadLeft(5, '0');
            posi += "1NNNNN";
            var hora = DateTime.Now;
            hora = hora.ToUniversalTime();
            var v = Convert.ToInt32(Convert.ToDouble(partes[7])).ToString().PadLeft(3,'0');
            posi +=
                String.Format("{0:HHmmss},{1},{2},{3},{4},{5}.", hora, 
                            partes[3].Substring(0, 9), partes[4], 
                            partes[5].Substring(0, 10), partes[6],v);
            STrace.Trace(GetType().FullName,1,"Mensaje de Posicion rearmado:{0}",posi);
            var p = new Posicion(Encoding.ASCII.GetBytes(posi));
            return p;
        }*/

        /*public static bool IsATResponse(int api, int type)
        {
            return type == 5;
        }//*/
    }
}