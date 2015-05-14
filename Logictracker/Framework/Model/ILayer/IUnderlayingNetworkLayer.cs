namespace Logictracker.Model
{
    /// <summary>
    /// Representa la capa de abstraccion de red subyacente.
    /// </summary>
    public interface IUnderlayingNetworkLayer : ILayer
    {
        /// <summary>
        /// Debe ser true si el UNL entrega los datos en el mismo orden que los recibe.
        /// </summary>
        //[StalkedProperty(Label = "Ordenamiento",
        //    Description = "Si es verdadero indica que nivel de red subyacente, entrega los datos al receptor en el mismo orden en que el emisor los envio.-")]
        bool OrderedTransfer { get; }
        
        /// <summary>
        /// Debe ser true si el UNL comprueba la validez de los datos que entrega.
        /// </summary>
        //[StalkedProperty(Label = "Control de Errores",
        //    Description = "Si es verdadero indica que nivel de red subyacente comprueba la ausencia de errores antes de entregar los datos.-")]
        bool ErrorFree { get; }
        
        /// <summary>
        /// Debe ser true si el UNL es capaz de adaptarse a condiciones de congestión de red.
        /// </summary>
        //[StalkedProperty(Label = "Control de Congestion",
        //    Description = "Si es verdadero indica que nivel de red subyacente puede detectar y adaptarse a condiciones de congestion en la red.-")]
        bool CongestionControl { get; }

        /// <summary>
        /// Debe ser true si el UNL es capaz de adaptar el flujo de datos a la velocidad del consumidor.
        /// </summary>
        //[StalkedProperty(Label = "Control de Flujo",
        //    Description = "Si es verdadero indica que nivel de red subyacente puede detectar y adaptarse a la capacidad de transferencia de su par conectado.-")]
        bool FlowControl { get; }

        /// <summary>
        /// Tamaño maximo de Trama DLL que UNL puede transportar.
        /// </summary>
        //[StalkedProperty(Label = "Unidad Maxima de Transferencia",
         //   Description = "Tamaño maximo de mensaje que puede transportar.-")]
        short Mtu { get; }
        
        /// <summary>
        /// Velocidad de transferencia efectiva admitida por la UNL.
        /// </summary>
        //[StalkedProperty(Label = "Ancho de Banda",
        //    Description = "Ancho de banda declarado que esta disposible para el nivel de red subyacente.-")]
        int Bps { get; }

	    /// <summary>
	    /// Envia un frame al TSP asociado.
	    /// </summary>
	    /// <param name="path">vinculo sobre el cual realizar el envio.</param>
	    /// <param name="frame">arreglo con los datos a enviar</param>
	    /// <returns>
	    /// falso indica que la trama no se enviara y verdadero indica que 
	    /// se ha encolado la trama para su envio, esto no implica garantias
	    /// del envio.
	    /// </returns>
	    /// <remarks>
	    /// Al enviar una trama, si el tamaño del frame es mayor que MTU descarta la trama y 
	    /// silenciosamente retorna false. 
	    /// </remarks>
	    void SendFrame(ILink path, IFrame frame);

	    INode Parser { get; }
    }
}