using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Sync
{
    [Serializable]
    public class OutQueue: IAuditable, ISecurable
    {
        public static class Queries
        {
            public const string Molinete = "molinete";
        }
        public static class Operations
        {
            public const string Empleado = "empleado";
            public const string Tarjeta = "tarjeta";
        }
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Query { get; set; }
        public virtual string Operacion { get; set; }
        public virtual string Parametros { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual int Prioridad { get; set; }
        public virtual int Transaction { get; set; }
    }
}
