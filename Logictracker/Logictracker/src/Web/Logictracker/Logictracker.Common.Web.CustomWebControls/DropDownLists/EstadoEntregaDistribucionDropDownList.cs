using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [Serializable]
    public class EstadoEntregaDistribucionDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(EntregaDistribucion.Estados); } }
        
        protected override void Bind() { BindingManager.BindEstadoEntregaDistribucion(this); }
    }
}