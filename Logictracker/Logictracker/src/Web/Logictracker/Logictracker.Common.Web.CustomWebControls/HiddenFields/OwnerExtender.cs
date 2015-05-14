#region Usings

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;
using Logictracker.Web.CustomWebControls.DropDownLists;

#endregion

namespace Logictracker.Web.CustomWebControls.HiddenFields
{
    public class OwnerExtender: HiddenField, IParentControl, INamingContainer
    {
        private bool isFirtsBinding = true;

        public event EventHandler InitialBinding;
        public event EventHandler SelectedIndexChanged;

        public string EmpresaControlId
        {
            get { return (string) ViewState["EmpresaControlId"]; }
            set { ViewState["EmpresaControlId"] = value; }
        }
        public string LineaControlId
        {
            get { return (string)ViewState["LineaControlId"]; }
            set { ViewState["LineaControlId"] = value; }
        }

        public void SetValue(Owner owner)
        {
            SetValue(owner != null ? owner.Empresa : null, owner != null ? owner.Linea : null);
        }
        public void SetValue(Empresa empresa, Linea linea)
        {
            SetValue(empresa != null ? empresa.Id : -1, linea != null ? linea.Id : -1);
        }
        public void SetValue(int empresa, int linea)
        {
            IdEmpresa = empresa;
            IdLinea = linea;
        }

        public int IdEmpresa
        {
            get { return cbEmpresa != null ? cbEmpresa.Selected : -1; } 
            set { if(cbEmpresa != null) cbEmpresa.EditValue = value; }
        }
        public int IdLinea
        {
            get { return cbLinea != null ? cbLinea.Selected : -1; } 
            set { if (cbLinea != null) cbLinea.EditValue = value; }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if(cbEmpresa != null)
            {
                cbEmpresa.InitialBinding += cbEmpresa_InitialBinding;
            }
            if (cbLinea != null)
            {
                cbLinea.InitialBinding += cbLinea_InitialBinding;
                cbLinea.SelectedIndexChanged += cbLinea_SelectedIndexChanged;
            }
        }

        protected void OnInitialBinding()
        {
            if (!isFirtsBinding) return;
            isFirtsBinding = false;
            if (InitialBinding != null) InitialBinding(this, EventArgs.Empty);

        }
        protected void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(this, EventArgs.Empty);
        }

        protected void cbEmpresa_InitialBinding(object sender, EventArgs e) { OnInitialBinding(); }
        protected void cbLinea_InitialBinding(object sender, EventArgs e) { OnInitialBinding(); }
        protected void cbLinea_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(); }

        public IEnumerable<IAutoBindeable> ParentControls { get { return new List<IAutoBindeable> { cbEmpresa, cbLinea }; } }

        private LocacionDropDownList cbEmpresa { get { return FindControl(EmpresaControlId, Page.Controls) as LocacionDropDownList; } }
        private PlantaDropDownList cbLinea { get { return FindControl(LineaControlId, Page.Controls) as PlantaDropDownList; } }

        private static IAutoBindeable FindControl(string parent, ControlCollection controls)
        {
            if(string.IsNullOrEmpty(parent)) return null;
            if (controls == null) return null;

            foreach (Control control in controls)
            {
                if (!string.IsNullOrEmpty(control.ID) && control.ID.Equals(parent)) return control as IAutoBindeable;
                var cnt = FindControl(parent, control.Controls);
                if (cnt != null) return cnt;
            }

            return null;
        }
    }
}
