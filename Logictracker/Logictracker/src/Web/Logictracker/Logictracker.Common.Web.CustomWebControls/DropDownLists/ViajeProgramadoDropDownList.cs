using System;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ViajeProgramadoDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(ViajeProgramado); } }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindViajesProgramados(this); }

        #endregion
    }
}