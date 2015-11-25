using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class LogicLinkFile : IAuditable, ISecurable
    {
        public static class Estados
        {
            public const int Pendiente = 0;
            public const int Procesando = 1;
            public const int Procesado = 2;
            public const int Cancelado = 3;


            public static string GetString(short estado)
            {
                switch (estado)
                {
                    case Pendiente: return "Pendiente";
                    case Procesando: return "Procesando";
                    case Procesado: return "Procesado";
                    case Cancelado:return "Cancelado";
                    default: return string.Empty;
                }
            }

        }

        public static class Estrategias
        {
            public const string DistribucionFemsa = "Distribucion.FEMSA";
            public const string DistribucionQuilmes = "Distribucion.QUILMES";
            public const string DistribucionMusimundo = "Distribucion.MUSIMUNDO";
            public const string DistribucionBrinks = "Distribucion.BRINKS";
            public const string DistribucionSos = "Distribucion.SOS";
            public const string DistribucionReginaldLee = "Distribucion.RL";
            public const string DistribucionCCU = "Distribucion.CCU";
            public const string PedidosPetrobras = "Pedidos.PETROBRAS";
            public const string AsignacionClienteEmpleado = "Cliente.Empleado";
        }

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        
        public virtual string ServerName { get; set; }        
        public virtual string FilePath { get; set; }
        public virtual string FileSource { get; set; }
        public virtual string Strategy { get; set; }
        public virtual DateTime DateAdded { get; set; }
        public virtual DateTime? DateProcessed { get; set; }
        public virtual int Status { get; set; }
        public virtual string Result { get; set; }
    }
}