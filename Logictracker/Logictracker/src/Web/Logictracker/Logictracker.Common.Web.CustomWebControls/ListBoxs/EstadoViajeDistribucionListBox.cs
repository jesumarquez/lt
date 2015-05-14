using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class EstadoViajeDistribucionListBox : ListBoxBase
    {
        public override Type Type { get { return typeof (ViajeDistribucion.Estados); } }

        protected override void Bind() { BindingManager.BindEstadoViajeDistribucion(this); }
    }
}
