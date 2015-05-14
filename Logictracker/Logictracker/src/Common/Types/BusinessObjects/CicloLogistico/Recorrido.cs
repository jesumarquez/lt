using System;
using System.Collections.Generic;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    [Serializable]
    public class Recorrido: IAuditable, ISecurable
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Nombre { get; set; }
        public virtual int Desvio { get; set; }
        public virtual bool Baja { get; set; }

        private IList<DetalleRecorrido> _detalles;
        public virtual IList<DetalleRecorrido> Detalles
        {
            get { return _detalles ?? (_detalles = new List<DetalleRecorrido>()); }
            set { _detalles = value; }
        }
    }
}
