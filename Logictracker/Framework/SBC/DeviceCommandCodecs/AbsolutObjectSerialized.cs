using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;
using System.Runtime.InteropServices;
using NHibernate.Engine;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public class AbsolutObjectSerialized : GenericParserSerialized
    {
            public string PHeader { get; set; }   //Encabezado del paquete
            public string IMEI { get; set; }      //IMEI del equipo
            public string PNumber { get; set; }   //Número de paquete       
            public string PEvent { get; set; }    //Evento que generó el paquete       
            public string Sgsm { get; set; }      //Nivel de señal GSM. (Entre 0 y 31)   
            public string U1vol { get; set; }     //U1 en voltios      
            public string I1amp { get; set; }     //I1 en Ampere por 10. (10 equivale a 1.0 Amp)        
            public string U2vol { get; set; }     //U2 en voltios            public string paramOne;
            public string I2amp { get; set; }     //I2 en Ampere por 10. (27 equivale a 2.7 Amp)     
            public string D1in { get; set; }      //D1 Entrada DIN1 inactiva          
            public string D2in { get; set; }      //D2 Entrada D2 activa           
            public string T1temp { get; set; }    //T1 Temperatura 25ºC           
            public string T2temp { get; set; }    //T2 Temperatura 23ºC          
            public string T3sen { get; set; }     //T3 Sensor NTC3 abierto       
            public string T4sen { get; set; }     //T4 Sensor NTC4 en cortocircuito      
            public string Lat { get; set; }       //Latitud           
            public string Lon { get; set; } 	  //Longitud           
            public string VerFir { get; set; } 	  //Modelo y versión de firmware del equipo        
            public string DateTime { get; set; }  //Fecha y hora de la generación del evento GMT0          
            public string PEnd { get; set; } 	  //Fin de paquete           
        
        public AbsolutObjectSerialized()
        {
            PHeader = new string(' ', 50);
            IMEI = new string(' ', 50);
            PNumber = new string(' ', 50);
            PEvent = new string(' ', 50);
            Sgsm = new string(' ', 50);
            U1vol = new string(' ', 50);
            I1amp = new string(' ', 50);
            U2vol = new string(' ', 50);
            I2amp = new string(' ', 50);
            D1in = new string(' ', 50);
            D2in = new string(' ', 50);
            T1temp = new string(' ', 50);
            T2temp = new string(' ', 50);
            T3sen = new string(' ', 50);
            T4sen = new string(' ', 50);
            Lat = new string(' ', 50);
            Lon = new string(' ', 50);
            VerFir = new string(' ', 50);
            DateTime = new string(' ', 50);
            PEnd = new string(' ', 50);
        }

        internal void SetStructData(byte[] b1)
        {
            readArrayPacketWithSplit(b1, this, new []{","} );
        }
    }
}
