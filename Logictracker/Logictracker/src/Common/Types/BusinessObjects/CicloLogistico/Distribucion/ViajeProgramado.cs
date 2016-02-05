using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ViajeProgramado : IAuditable, IHasEmpresa, IHasTransportista
    {
        Type IAuditable.TypeOf() { return GetType(); }
        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual string Codigo { get; set; }
        public virtual double Horas { get; set; }
        public virtual double Km { get; set; }

        private IList<EntregaProgramada> _detalles;
        public virtual IList<EntregaProgramada> Detalles
        {
            get { return _detalles ?? (_detalles = new List<EntregaProgramada>()); }
            set { _detalles = value; }
        }
    }        
}
