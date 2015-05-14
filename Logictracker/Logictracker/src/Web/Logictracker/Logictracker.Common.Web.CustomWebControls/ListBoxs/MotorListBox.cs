#region Usings

using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class CaudalimetroListBox : ListBoxBase, ICaudalimetroAutoBindeable
    {
        #region Public Methods

        public override Type Type { get { return typeof (Caudalimetro); } }

        public bool ShowDeIngreso
        {
            get { return ViewState["ShowDeIngreso"] != null ? Convert.ToBoolean(ViewState["ShowDeIngreso"]) : false; }
            set { ViewState["ShowDeIngreso"] = value; }
        }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindCaudalimetro(this); }

        #endregion


    }
}
