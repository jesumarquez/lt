using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TurnoDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(Shift); } }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindTurnos(this); }

        #endregion
    }
}