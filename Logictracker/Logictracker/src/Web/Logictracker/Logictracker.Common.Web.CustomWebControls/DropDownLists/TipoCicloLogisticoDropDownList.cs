using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoCicloLogisticoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TipoCicloLogistico); } }

        protected override void Bind() { BindingManager.BindTipoCicloLogistico(this); }
    }
}