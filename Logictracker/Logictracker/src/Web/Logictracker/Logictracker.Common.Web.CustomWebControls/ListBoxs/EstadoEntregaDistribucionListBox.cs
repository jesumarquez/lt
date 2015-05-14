using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class EstadoEntregaDistribucionListBox : ListBoxBase
    {
        public override Type Type { get { return typeof (EntregaDistribucion.Estados); } }

        protected override void Bind() { BindingManager.BindEstadoEntregaDistribucion(this); }
    }
}
