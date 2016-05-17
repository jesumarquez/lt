#region Usings

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.App_Controls
{
    public partial class AppControlsAltaMarca : BaseUserControl
    {
        public Unit Width
        {
            get { return panelMarcas.Width; }
            set { panelMarcas.Width = value; }
        }

        public bool EditMode
        {
            get { return (bool) (ViewState["EditMode"] ?? false); }
            set { ViewState["EditMode"] = value;}
        }

        public int MarcaId
        {
            get
            {
                if (ViewState["MarcaId"] == null) MarcaId = cbMarca.Selected;
                return (int)ViewState["MarcaId"];
            }
            set { ViewState["MarcaId"] = value;
                cbMarca.SelectedValue = value.ToString(); }
        }

        protected void btAceptar_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMarca.Text)) 
                    ThrowMustEnter("MARCA");

                if (!EditMode && DAOFactory.MarcaDAO.ExistsDescripcion(-1,-1, txtMarca.Text)) 
                    ThrowDuplicated("MARCA");

                var marca = !EditMode ? new Marca() : DAOFactory.MarcaDAO.FindById(cbMarca.Selected);

                if (EditMode)
                {
                    var item = cbMarca.Items.FindByValue(marca.Id.ToString());

                    cbMarca.Items.Remove(item);
                }

                marca.Descripcion = txtMarca.Text;

                DAOFactory.MarcaDAO.SaveOrUpdate(marca);

                cbMarca.Items.Insert(0, new ListItem(txtMarca.Text, marca.Id.ToString()));

                MarcaId = marca.Id;

                multiMarca.SetActiveView(viewSelect);
            }
            catch(Exception ex)
            {
                var basePage = Page as BasePage;

                if (basePage == null) throw;

                basePage.ShowError(ex);
            }
        }
        protected void btCerrar_Click(object sender, ImageClickEventArgs e)
        {
            txtMarca.Text = string.Empty;
            multiMarca.SetActiveView(viewSelect);
        }

        protected void btAdd_Click(object sender, ImageClickEventArgs e)
        {
            EditMode = false;
            txtMarca.Text = string.Empty;
            multiMarca.SetActiveView(viewEdit);
        }

        protected void btEdit_Click(object sender, ImageClickEventArgs e)
        {
            EditMode = true;
            var m = DAOFactory.MarcaDAO.FindById(cbMarca.Selected);
            txtMarca.Text = m.Descripcion;
            multiMarca.SetActiveView(viewEdit);
        }

        protected void cbMarca_SelectedIndexChanged(object sender, EventArgs e) { MarcaId = cbMarca.Selected; }

        protected void cbMarca_PreBind(object sender, EventArgs e) { cbMarca.EditValue = MarcaId; }
    }
}