using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class Pedido: IAuditable, ISecurable, IHasCliente, IHasPuntoEntrega, IHasBocaDeCarga, IHasProducto
    {
        public static class Estados
        {
            public const int Pendiente = 0;
            public const int EnCurso = 10;
            public const int Entregado = 50;
            public const int Cancelado = 90;
        }

        Type IAuditable.TypeOf() { return GetType(); }

        public virtual int Id { get; set;}
        public virtual string Codigo{ get; set;}
        public virtual DateTime FechaEnObra { get; set;}
        public virtual Cliente Cliente { get; set;}
        public virtual PuntoEntrega PuntoEntrega { get; set;}
        public virtual string NumeroBomba { get; set; }
        public virtual DateTime HoraCarga { get; set; }
        public virtual bool EsMinimixer { get; set;}
        public virtual string Observacion { get; set; }
        public virtual string Contacto { get; set; }
        public virtual double Cantidad { get; set; }
        public virtual double CantidadAjuste { get; set; }
        public virtual double CargaViaje { get; set; }
        public virtual int TiempoCiclo { get; set;}
        public virtual int Frecuencia { get; set;}
        public virtual BocaDeCarga BocaDeCarga { get; set;}
        public virtual Producto Producto { get; set; }
        public virtual Empresa Empresa { get; set;}
        public virtual Linea Linea { get; set;}
        public virtual int Estado { get; set; }
        public virtual bool Baja { get; set; }
    }
}
