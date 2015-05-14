using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [Serializable]
    public class DepositoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Deposito); } }
        
        protected override void Bind() { BindingManager.BindDeposito(this); }
    }
}