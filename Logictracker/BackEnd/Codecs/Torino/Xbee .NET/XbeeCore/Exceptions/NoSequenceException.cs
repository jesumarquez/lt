#region Usings

using System;

#endregion

namespace XbeeCore.Exceptions
{
    public class NoSequenceException : Exception
    {
        public NoSequenceException() :
            base("No quedan secuencias disponibles en el transction layer especificado.")
        {
            
        }
    }
}
