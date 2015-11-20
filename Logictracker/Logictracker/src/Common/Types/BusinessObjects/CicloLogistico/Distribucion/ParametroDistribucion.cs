using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using System;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ParametroDistribucion
    {
        public virtual int Id { get; set; }
        public virtual ViajeDistribucion ViajeDistribucion { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Valor { get; set; }
    }
}
