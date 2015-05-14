using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [Serializable]
    public class TipoDistribucionDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(ViajeDistribucion.Tipos); } }

        protected override void Bind() { BindingManager.BindTipoDistribucion(this); }
  
    }
}
