using System;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class SensorDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Sensor); } }

        protected override void Bind()
        {
            BindingManager.BindSensores(this);
        }
    }
}