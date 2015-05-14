#region Usings

using System;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TipoDocumentoListBox : ListBoxBase, ITipoDocumentoAutoBindeable
    {
        public override Type Type
        {
            get { return typeof (TipoDocumento); }
        }
        public bool OnlyForVehicles { get; set; }
        public bool OnlyForEmployees { get; set; }
        public bool OnlyForEquipment { get; set; }
        public bool OnlyForTransporter { get; set; }

        protected override void Bind()
        {
            BindingManager.BindTipoDocumento(this);
        }
    }
}
