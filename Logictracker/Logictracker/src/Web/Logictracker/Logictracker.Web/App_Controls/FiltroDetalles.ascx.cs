using System.Globalization;
using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DAL.Filtros;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.DropDownLists;

namespace Logictracker.App_Controls
{
    public partial class FiltroDetalles : BaseUserControl
    {
        public List<Filtro> Filtros
        {
            get { return (List<Filtro>)(ViewState["FILTROS_DETALLE"] ?? new List<Filtro>()); } 
            set { ViewState["FILTROS_DETALLE"] = value; }
        }

        public int Empresa
        {
            get { return  ViewState["Empresa"] != null ? Convert.ToInt32(ViewState["Empresa"]) : -1; } 
            set { ViewState["Empresa"] = value.ToString(); }
        }
        public int Linea
        {
            get { return ViewState["Linea"] != null ? Convert.ToInt32(ViewState["Linea"]) : -1; }
            set { ViewState["Linea"] = value.ToString(); }
        }
        public List<int> TipoEntidad
        {
            get { return ViewState["TipoEntidad"] != null ? ViewState["TipoEntidad"] as List<int> : new List<int>{-1}; }
            set { ViewState["TipoEntidad"] = value; }
        }
        public C1GridView Grilla { get { return grdFiltros; } }
        
        protected override void OnLoad(EventArgs e)
        {
            GetData();

            grdFiltros.Visible = grdFiltros.Rows.Count > 0;
        }

        protected void GrdFiltrosItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var param = e.Row.DataItem as Filtro;
            if (param == null) return;

            var cbUnion = e.Row.FindControl("cbUnion") as UnionDropDownList;
            var cbDetalle = e.Row.FindControl("cbDetalle") as DetalleDropDownList;
            var cbOperador = e.Row.FindControl("cbOperador") as OperadorDropDownList;
            var txtValor = e.Row.FindControl("txtValor") as TextBox;
            var cbValor = e.Row.FindControl("cbValor") as DropDownList;
            var lstValor = e.Row.FindControl("lstValor") as ListBox;
            var btnEliminar = e.Row.FindControl("btnEliminar") as ImageButton;

            if (cbDetalle != null)
            {
                cbDetalle.DataSource = DAOFactory.DetalleDAO.GetList(new[] { Empresa },
                                                                     new[] { Linea },
                                                                     TipoEntidad ,
                                                                     new[] { -1 })
                                                            .Where(d => d.EsFiltro);
                cbDetalle.DataBind();
                cbDetalle.Attributes.Add("index", e.Row.RowIndex.ToString());
            }

            if (cbUnion != null) cbUnion.SetSelectedValue(param.IdUnion);
            if (cbOperador != null) cbOperador.SetSelectedValue(param.IdOperador);
            if (btnEliminar != null) btnEliminar.Attributes.Add("index", e.Row.RowIndex.ToString());
            if (cbDetalle != null)
            {
                if (param.IdDetalle > 0) 
                    cbDetalle.SetSelectedValue(param.IdDetalle);
                else
                {
                    if (cbDetalle.Items.Count > 0)
                    {
                        cbDetalle.SetSelectedValue(Convert.ToInt32(cbDetalle.Items[0].Value));
                        param.IdDetalle = cbDetalle.Selected;
                    }
                }
            }

            var detalle = param.IdDetalle > 0 ? DAOFactory.DetalleDAO.FindById(param.IdDetalle) : null;
            if (detalle != null && detalle.Id > 0)
            {
                switch (detalle.Representacion)
                {
                    case 1: // TEXTBOX
                        if (txtValor != null)
                        {
                            txtValor.Text = param.ValorStr;
                            txtValor.Visible = true;
                        }
                        break;
                    case 2: // DROPDOWNLIST
                        if (cbValor != null)
                        {
                            cbValor.Visible = true;

                            var opciones = detalle.Opciones.Split('|');
                            if (opciones.Length > 0)
                            {
                                cbValor.Items.Clear();
                                foreach (var opcion in opciones)
                                {
                                    var valores = opcion.Split('.');
                                    if (valores.Length > 1)
                                    {
                                        cbValor.Items.Add(new ListItem(valores[valores.Length - 1],
                                                                       valores[valores.Length - 2]));
                                    }
                                }
                            }

                            cbValor.SelectedValue = Convert.ToInt32(param.ValorNum).ToString();
                        }
                        break;
                    case 3: // LISTBOX
                        if (lstValor != null)
                        {
                            lstValor.Visible = true;

                            var options = detalle.Options;
                            
                            var selected = param.ValorStr != null ? param.ValorStr.Split('-').ToList() : new List<string>();

                            foreach (var option in options)
                            {
                                var li = new ListItem
                                             {
                                                 Text = option.Text,
                                                 Value = option.Index,
                                                 Selected = selected.Contains(option.Index)
                                             };
                                lstValor.Items.Add(li);
                            }
                        }
                        break;
                    default:
                        if (txtValor != null)
                        {
                            txtValor.Text = param.ValorStr;
                            txtValor.Visible = true;
                        }
                        break;
                }
            }
        }

        protected void BtnAgregarOnClick(object sender, EventArgs e)
        {
            var parameter = new Filtro();
            var par = Filtros;
            par.Add(parameter);
            Filtros = par;
            grdFiltros.DataSource = par;
            grdFiltros.DataBind();
            grdFiltros.Visible = true;
        }

        protected void CbDetalleOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var cbDetalle = sender as DropDownList;
            if (cbDetalle == null) return;
            
            var detalle = DAOFactory.DetalleDAO.FindById(Convert.ToInt32(cbDetalle.SelectedValue));
            var index = Convert.ToInt32(cbDetalle.Attributes["index"]);

            if (grdFiltros.Rows.Count <= index) return;

            var row = grdFiltros.Rows[index];
            var txtValor = row.FindControl("txtValor") as TextBox;
            var cbValor = row.FindControl("cbValor") as DropDownList;
            var lstValor = row.FindControl("lstValor") as ListBox;
            var cbOperador = row.FindControl("cbOperador") as OperadorDropDownList;

            if (detalle != null && detalle.Id > 0)
            {
                switch (detalle.Representacion)
                {
                    case 1: // TEXTBOX
                        if (txtValor != null)
                        {
                            //txtValor.Text = string.Empty;
                            txtValor.Visible = true;
                            if (cbValor != null) cbValor.Visible = false;
                            if (lstValor != null) lstValor.Visible = false;

                            if (cbOperador != null && cbOperador.Items.Count != 4)
                            {
                                cbOperador.ClearItems(); 
                                cbOperador.AddItem(CultureManager.GetLabel("IGUAL_A"), 1);
                                cbOperador.AddItem(CultureManager.GetLabel("MAYOR_A"), 2);
                                cbOperador.AddItem(CultureManager.GetLabel("MENOR_A"), 3);
                                cbOperador.AddItem(CultureManager.GetLabel("CONTIENE_A"), 4);
                            }
                        }
                        break;
                    case 2: // DROPDOWNLIST
                        if (cbValor != null)
                        {
                            cbValor.Visible = true;
                            if (txtValor != null) txtValor.Visible = false;
                            if (lstValor != null) lstValor.Visible = false;

                            var opciones = detalle.Opciones.Split('|');
                            if (opciones.Length > 0)
                            {
                                if (DistintasOpcionesParaCombo(cbValor.Items, opciones))
                                {
                                    cbValor.Items.Clear();
                                    foreach (var opcion in opciones)
                                    {
                                        var valores = opcion.Split('.');
                                        if (valores.Length > 1)
                                        {
                                            cbValor.Items.Add(new ListItem(valores[valores.Length - 1],
                                                                           valores[valores.Length - 2]));
                                        }
                                    }
                                }
                            }
                            if (cbOperador != null && 
                               (cbOperador.Items.Count != 1
                                || (cbOperador.Items.Count == 1 && cbOperador.Items[0].Value != @"1")))
                            {
                                cbOperador.ClearItems();
                                cbOperador.AddItem(CultureManager.GetLabel("IGUAL_A"), 1);
                            }
                        }
                        break;
                    case 3: // LISTBOX
                        if (lstValor != null)
                        {
                            lstValor.Visible = true;
                            if (txtValor != null) txtValor.Visible = false;
                            if (cbValor != null) cbValor.Visible = false;

                            var options = detalle.Options;

                            var selected = new List<string>();

                            if (DistintasOpcionesParaListBox(lstValor.Items, options))
                            {

                                lstValor.Items.Clear();
                                foreach (var option in options)
                                {
                                    var li = new ListItem
                                                 {
                                                     Text = option.Text,
                                                     Value = option.Index,
                                                     Selected = selected.Contains(option.Index)
                                                 };
                                    lstValor.Items.Add(li);
                                }
                            }
                            if (cbOperador != null &&
                              (cbOperador.Items.Count != 1
                               || (cbOperador.Items.Count == 1 && cbOperador.Items[0].Value != @"4")))
                            {
                                cbOperador.ClearItems();
                                cbOperador.AddItem(CultureManager.GetLabel("CONTIENE_A"), 4);
                            }
                        }
                        break;
                    default:
                        if (txtValor != null)
                        {
                            txtValor.Text = string.Empty;
                            txtValor.Visible = true;
                        }
                        break;
                }
            }
        }

        protected void BtnEliminarOnClick(object sender, EventArgs e)
        {
            if (sender == null) return;

            var btnEliminar = sender as ImageButton;

            if (btnEliminar == null) return;
            
            var index = Convert.ToInt32(btnEliminar.Attributes["index"]);
            
            Filtros.RemoveAt(index);

            grdFiltros.DataSource = Filtros;
            grdFiltros.DataBind();
            grdFiltros.Visible = grdFiltros.Rows.Count > 0;
        }

        private void GetData()
        {
            var par = Filtros;
            var p = 0;
            for (var i = 0; i < grdFiltros.Rows.Count; i++)
            {
                var item = grdFiltros.Rows[i];
                var cbUnion = item.FindControl("cbUnion") as UnionDropDownList;
                var cbDetalle = item.FindControl("cbDetalle") as DetalleDropDownList;
                var cbOperador = item.FindControl("cbOperador") as OperadorDropDownList;
                var txtValor = item.FindControl("txtValor") as TextBox;
                var cbValor = item.FindControl("cbValor") as DropDownList;
                var lstValor = item.FindControl("lstValor") as ListBox;

                par[p].IdUnion = cbUnion != null ? int.Parse(cbUnion.SelectedValue) : 0;
                par[p].IdDetalle = cbDetalle != null && cbDetalle.SelectedValue != string.Empty ? int.Parse(cbDetalle.SelectedValue) : 0;
                par[p].IdOperador = cbOperador != null ? int.Parse(cbOperador.SelectedValue) : 0;
                
                if (cbDetalle != null)
                {
                    cbDetalle.DataSource = DAOFactory.DetalleDAO.GetList(new[] { Empresa },
                                                                         new[] { Linea },
                                                                         TipoEntidad,
                                                                         new[] { -1 })
                                                                .Where(d => d.EsFiltro);
                    cbDetalle.DataBind();
                }

                var detalle = DAOFactory.DetalleDAO.FindById(par[p].IdDetalle);

                if (txtValor != null && txtValor.Visible)
                {
                    if (detalle != null && detalle.Id > 0)
                    {
                        switch (detalle.Tipo)
                        {
                            case 1:
                                par[p].ValorStr = txtValor.Text;
                                break;
                            case 2:
                                double num;
                                par[p].ValorNum = double.TryParse(txtValor.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out num)
                                                      ? num
                                                      : 0;
                                break;
                            case 3:
                                DateTime dt;
                                par[p].ValorDt = DateTime.TryParse(txtValor.Text, out dt) ? dt : DateTime.Today;
                                break;
                            default:
                                par[p].ValorStr = txtValor.Text;
                                break;
                        }
                    }
                }

                if (cbValor != null && cbValor.Visible)
                    par[p].ValorNum = Convert.ToInt32(cbValor.SelectedValue);

                if (lstValor != null && lstValor.Visible)
                {
                    var str = "";

                    foreach (var index in lstValor.GetSelectedIndices())
                    {
                        if (str != "") str += "-";
                        str += lstValor.Items[index].Value;
                    }

                    par[p].ValorStr = str;
                }
                
                if (cbDetalle != null)
                    cbDetalle.SetSelectedValue(par[p].IdDetalle);

                p++;
            }
            Filtros = par;
        }

        private static bool DistintasOpcionesParaCombo(ListItemCollection items, IList<string> options)
        {
            if (items.Count != options.Count) return true;

            for (var i = 0; i < items.Count; i++)
            {
                var valores = options[i].Split('.');
                if (valores.Length > 1)
                {
                    var item = new ListItem(valores[valores.Length - 1], valores[valores.Length - 2]);
                    if (item.Value != items[i].Value || item.Text != items[i].Text)
                        return true;
                }
            }

            return false;
        }

        private static bool DistintasOpcionesParaListBox(ListItemCollection items, IList<Detalle.Option> options)
        {
            if (items.Count != options.Count) return true;

            for (var i = 0; i < items.Count; i++)
            {
                var item = new ListItem(options[i].Text, options[i].Index);
                if (item.Value != items[i].Value || item.Text != items[i].Text)
                    return true;
            }

            return false;
        }
    }
}
