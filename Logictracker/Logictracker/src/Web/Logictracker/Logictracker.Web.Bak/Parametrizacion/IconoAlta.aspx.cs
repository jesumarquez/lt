using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Image = System.Drawing.Image;

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_Icono_Alta : ApplicationSecuredPage
    {
        public int CurrentId
        {
            get { return (int) (ViewState["CurrentId"] ?? 0); }
            set { ViewState["CurrentId"] = value; }
        }
        public int LastIndex
        {
            get { return (int)(ViewState["LastIndex"] ?? -1); }
            set { ViewState["LastIndex"] = value; }
        }
        public short Width
        {
            get { return (short)(ViewState["Width"] ?? 0); }
            set { ViewState["Width"] = value; }
        }
        public short Height
        {
            get { return (short)(ViewState["Height"] ?? 0); }
            set { ViewState["Height"] = value; }
        }
        public short OffsetX
        {
            get { return (ViewState["OffsetX"] != null ? (short)ViewState["OffsetX"] : (short)0); }
            set { ViewState["OffsetX"] = value; }
        }
        public short OffsetY
        {
            get { return (ViewState["OffsetY"] != null ? (short)ViewState["OffsetY"] : (short)0); }
            set { ViewState["OffsetY"] = value; }
        }

        public bool IsAdmin
        {
            get { return Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin; }
        }
        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        #region Security
        /// <summary>
        /// Gets the module refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "ICONO"; }

        /// <summary>
        /// Agrega iconos a la ToolBar.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            ToolBar1.ItemCommand += ToolBar1_ItemCommand;

            if (Usuario.Modules[GetRefference()].Add) ToolBar1.AddNewToolbarButton();
            if (Usuario.Modules[GetRefference()].Edit) ToolBar1.AddSaveToolbarButton();
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cbEmpresa.AddAllItem = IsAdmin;
            ToolBar1.AsyncPostBacks = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //SelectIcon1.SelectedIconChange += SelectIcon1_SelectedIconChange;
            if (!IsPostBack) BindIconos();
        }

        private void Save()
        {
            try
            {
                var editMode = CurrentId > 0;
                var icono = editMode
                                ? DAOFactory.IconoDAO.FindById(CurrentId)
                                : new Icono();

                icono.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                icono.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                icono.Descripcion = txtDescripcion.Text;

                if (!editMode && !filIcono.HasFile) ThrowMustEnter("IMAGEN");

                icono.OffsetX = OffsetX;
                icono.OffsetY = OffsetY;
                if(filIcono.HasFile)
                {
                    var absolutePath = Server.MapPath(Config.Directory.IconDir);
                    var newFileName = filIcono.FileName;
                    var extensionIndex = filIcono.FileName.LastIndexOf('.');
                    var fileName = filIcono.FileName.Substring(0, extensionIndex);
                    var fileExtension = filIcono.FileName.Substring(extensionIndex + 1);
                    var iteration = 1;
                    while (File.Exists(absolutePath + newFileName))
                    {
                        newFileName = fileName + "(" + iteration + ")." + fileExtension;
                        iteration++;
                    }

                    filIcono.PostedFile.SaveAs(absolutePath + newFileName);

                    var img = Image.FromFile(absolutePath + newFileName);

                    icono.Width = (short) img.Width;
                    icono.Height = (short) img.Height;
                    icono.OffsetX = (short)(-icono.Width / 2);
                    icono.OffsetY = (short)(-icono.Height / 2);
                

                    if (editMode && File.Exists(absolutePath + icono.PathIcono)) File.Delete(absolutePath + icono.PathIcono);
                    icono.PathIcono = newFileName;
                }

                DAOFactory.IconoDAO.SaveOrUpdate(icono);

                New();
                BindIconos();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void New()
        {
            if (LastIndex >= 0) (gridIconos.Items[LastIndex].FindControl("Button1") as Button).Style.Add("border", "solid 1px Black");
            panelUpload.TitleVariableName = "NUEVO_ICONO";
            CurrentId = 0;
            LastIndex = -1;
            txtDescripcion.Text = string.Empty;
            imgAnchor.Visible = false;
            imgAnchorPointer.Visible = false;
        }

        protected void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindIconos();
        }

        public void BindIconos()
        {
            var iconos = DAOFactory.IconoDAO.GetList(cbEmpresa.Selected, cbLinea.Selected);
            var filtered = from Icono i in iconos where i.HasKeywords(txtFilter.Text) orderby i.Descripcion select i;
            LastIndex = -1;
            gridIconos.DataSource = filtered;
            gridIconos.DataBind();
        }

        protected void gridIconos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int id;
                if (!int.TryParse(e.CommandArgument.ToString(), out id)) return;

                if (LastIndex >= 0) (gridIconos.Items[LastIndex].FindControl("Button1") as Button).Style.Add("border", "solid 1px Black");

                if (CurrentId != id)
                {
                    CurrentId = id;

                    var icono = DAOFactory.IconoDAO.FindById(CurrentId);

                    if(!IsAdmin && icono.Empresa == null)
                    {
                        ShowResourceError("NOT_ALLOWED");
                        return;
                    }

                    var bic = e.Item.FindControl("Button1") as Button;
                    if (bic == null) return;
                    bic.Style.Add("border", "solid 2px #0000FF");

                    LastIndex = e.Item.ItemIndex;

                    panelUpload.TitleVariableName = "CAMBIAR_ICONO";
                    txtDescripcion.Text = icono.Descripcion;
                    imgAnchor.ImageUrl = IconDir + icono.PathIcono;
                    Width = icono.Width;
                    Height = icono.Height;
                    OffsetX = icono.OffsetX;
                    OffsetY = icono.OffsetY;
                    MoveAnchor(icono.OffsetX, icono.OffsetY);
                    imgAnchorPointer.Visible = true;
                    imgAnchor.Visible = true;
                }
                else New();
            }

        }

        protected void imgAnchor_Click(object sender, ImageClickEventArgs e)
        {
            MoveAnchor((short)-e.X, (short)-e.Y);
        }

        private void MoveAnchor(short x, short y)
        {
            OffsetX = x;
            OffsetY = y;
            x = (short)(-Width - x);
            y = (short)(-Height - y);
            x -= 12;
            y += 3;
            imgAnchorPointer.Style.Add(HtmlTextWriterStyle.Left, x + "px");
            imgAnchorPointer.Style.Add(HtmlTextWriterStyle.Top, y + "px");
            imgAnchorPointer.Style.Add("position", "relative");
            //imgAnchorPointer.Style.Add(HtmlTextWriterStyle.Left, -9 + "px");
            //imgAnchorPointer.Style.Add(HtmlTextWriterStyle.Top, 5 + "px");
        }
        protected void gridIconos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var icono = e.Item.DataItem as Icono;
            if (icono == null) return;
            var bic = e.Item.FindControl("Button1") as Button;
            if (bic == null) return;
            bic.CommandArgument = icono.Id.ToString();

            bic.Style.Add("background-image", string.Concat("url('", IconDir, icono.PathIcono, "')"));
            bic.Style.Add("background-position", "center center");
            bic.Style.Add("background-repeat", "no-repeat");

            if (icono.Id == CurrentId) bic.Style.Add("border", "solid 2px #0000FF");
            if(!IsAdmin && icono.Empresa == null)
            {
                bic.Style.Add("border", "solid 1px #e0c0c0");
                bic.Style.Add("background-color", "#f0f0f0");
            }

            var lbl = e.Item.FindControl("lblDescri") as Label;
            if (lbl == null) return;
            lbl.Text = icono.Descripcion.Replace(" ", "&nbsp;");

            var lit = e.Item.FindControl("litAconym") as Literal;
            if (lit == null) return;
            lit.Text = "<acronym title='" + icono.Descripcion + "'>";
        
        }
        #region Tools

        protected void ToolBar1_ItemCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Save": Save(); break;
                case "New": New(); break;
            }
        }


        protected void btFilter_Click(object sender, EventArgs e)
        {
            BindIconos();
        }
        #endregion
    }
}