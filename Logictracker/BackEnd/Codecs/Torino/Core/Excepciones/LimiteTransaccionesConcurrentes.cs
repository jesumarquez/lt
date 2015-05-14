using System;
using System.Collections.Generic;
using System.Text;

namespace SA
{
    /// <summary>
    /// Esta excepcion indica que no se puede iniciar una transaccion ya 
    /// que todas las transacciones estan en uso.
    /// </summary>
    public class LimiteTransaccionesConcurrentes : Exception
    {

    }
}
