#region Usings

using System;

#endregion

namespace MessageQueueObject
{
    [Serializable]
    public class InternalOpaque
    {
        public double comaflotante;
        public string cadena;

        public InternalOpaque()
        {
        }

        public InternalOpaque(double a, string b)
        {
            comaflotante = a;
            cadena = b;
        }

        public override string ToString()
        {
            return String.Format("{0} - '{1}'", comaflotante, cadena);
        }
    }

    [Serializable]
    public class ComplexOpaque
    {
        public string cadena;
        public DateTime fecha;
        public InternalOpaque clase;

        public ComplexOpaque()
        {
        }

        public ComplexOpaque(string a, DateTime b, string c, double d)
        {
            cadena = a;
            fecha = b;
            clase = new InternalOpaque(d, c);
        }

        public override string ToString()
        {
            return String.Format(" '{0}' - '{1}' - [ {2} ] ", cadena, fecha, clase);
        }

    }
}
