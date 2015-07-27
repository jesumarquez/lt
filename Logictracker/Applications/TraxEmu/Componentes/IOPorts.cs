using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraxEmu.Componentes
{

    class Disparadores
    {
        public int[] GSMax = new int[7];
        public int[] GSMin = new int[7];
    }
    class Factors
    {
        // Diferencia entre la entrada real y la que se pretende 
        public int K { get; set; }
    }

    class IOPorts
    {
        public double IN0 { get; set; }
        public double IN1 { get; set; }
        public double IN2 { get; set; }
        public double IN3 { get; set; }
        public int IN4 { get; set; }
        public int IN5 { get; set; }
        public double Contacto { get; set; }
        public double OUT0 { get; set; }
        public double OUT1 { get; set; }
        public double OUT2 { get; set; }
        public double OUT3 { get; set; }
        // Volts Bateria principal 00.00
        public int MainBat { get; set; }
        // Volts Volts bateria auxiliar 00.00
        public int AuxBat { get; set; }
    }
}
