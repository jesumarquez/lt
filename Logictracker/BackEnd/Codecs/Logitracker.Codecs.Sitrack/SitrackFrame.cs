using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitracker.Codecs.Sitrack
{
    public class SitrackFrame
    {
        /// <summary>
        /// Identificador de reporte de posición
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Fecha y hora de emisión del reporte de posición generado por el equipo GPS
        /// </summary>
        public DateTime ReportDate { get; set; }
        /// <summary>
        /// Fecha y hora de recepción del reporte de posición en nuestro Sistema Central
        /// </summary>
        public DateTime InputDate { get; set; } 
        /// <summary>
        /// Identificador de equipo GPS instalado
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// Identificador de vehículo ó unidad móvil
        /// </summary>
        public int HolderId { get; set; }
        /// <summary>
        /// Patente del vehículo ó unidad móvil
        /// </summary>
        public string HolderDomain { get; set; } 
        /// <summary>
        /// Nombre o código del cliente asignado al vehículo ó unidad móvil
        /// </summary>
        public string HolderName { get; set; } 
        /// <summary>
        /// Identificador del tipo de evento que generó el reporte
        /// </summary>
        public int EventId { get; set; } 
        /// <summary>
        /// Descripción del tipo de evento que generó el reporte
        /// </summary>
        public string EventDesc { get; set; } 
        /// <summary>
        /// Indica si el módulo GPS del equipo contaba con suficiente señal satelital al momento de generar 
        /// el reporte. Si el valor es menor a 90, los datos de posición son confiables. Si este valor es igual o 
        /// mayor a 90 los datos de posición no son confiables por lo tanto no se incluyen los campos latitude, 
        /// longitude, location, course ni speed
        /// </summary>
        public int Validity { get; set; } 
        /// <summary>
        /// Latitud de la posición expresada en grados decimales
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitud de la posición expresada en grados decimales
        /// </summary>
        public double Longitude { get; set; } 
        /// <summary>
        /// Dirección postal aproximada de la posición (Calle/Ruta, Localidad/Distrito, Ciudad/Municipio, 
        /// Provincia/Estado, País)
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Sentido de circulación en grados sexagecimales (0° es el Norte)
        /// </summary>
        public double Course { get; set; }
        /// <summary>
        /// Velocidad en Km/h
        /// </summary>
        public double Speed { get; set; }
    }
}
