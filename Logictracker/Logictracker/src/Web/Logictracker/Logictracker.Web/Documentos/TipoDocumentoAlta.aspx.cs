#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Documentos
{
    public partial class Documentos_TipoDocumentoAlta : SecuredAbmPage<TipoDocumento>
    {
        #region Protected Methods

        protected List<TipoDocumentoParametro> NewParams
        {
            get { return (List<TipoDocumentoParametro>) (ViewState["NewParams"] ?? new List<TipoDocumentoParametro>()); }
            set { ViewState["NewParams"] = value; }
        }

        protected List<int> ToDelete
        {
            get { return (List<int>)(ViewState["ToDelete"] ?? new List<int>()); }
            set { ViewState["ToDelete"] = value; }
        }

        protected override string VariableName { get { return "DOC_TIPO_DOCUMENTO"; } }

        protected override string RedirectUrl { get { return "TipoDocumentoLista.aspx"; } }

        protected override string GetRefference() { return "TIPODOCUMENTO"; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            GetNewData();
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.NullValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.NullValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtNombre.Text = EditObject.Nombre;
            chkVehiculo.Checked = EditObject.AplicarAVehiculo;
            chkEmpleado.Checked = EditObject.AplicarAEmpleado;
            chkTransportista.Checked = EditObject.AplicarATransportista;
            chkEquipo.Checked = EditObject.AplicarAEquipo;
            chkVencimiento.Checked = EditObject.RequerirVencimiento;
            chkPresentacion.Checked = EditObject.RequerirPresentacion;
            chkControlaConsumo.Checked = EditObject.ControlaConsumo;
            txtPrimerAviso.Text = EditObject.PrimerAviso.ToString();
            txtSegundoAviso.Text = EditObject.SegundoAviso.ToString();
            BindParametros();
        }

        protected override void OnDelete()
        {
            var list = DAOFactory.DocumentoDAO.FindByTipo(EditObject.Id);
            if (list.Count > 0) ThrowError("CANT_DELETE_TIPODOC");

            DAOFactory.TipoDocumentoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Nombre = txtNombre.Text;
            EditObject.AplicarAVehiculo = chkVehiculo.Checked;
            EditObject.AplicarAEmpleado = chkEmpleado.Checked;
            EditObject.AplicarATransportista = chkTransportista.Checked;
            EditObject.AplicarAEquipo = chkEquipo.Checked;
            EditObject.RequerirVencimiento = chkVencimiento.Checked;
            EditObject.RequerirPresentacion = chkPresentacion.Checked;
            EditObject.ControlaConsumo = chkControlaConsumo.Checked;

            short pri, seg;
            if(!short.TryParse(txtPrimerAviso.Text, out pri)) ThrowError("FORMAT_PRIMER_AVISO");

            if (!short.TryParse(txtSegundoAviso.Text, out seg)) ThrowError("FORMAT_SEGUNDO_AVISO");

            EditObject.PrimerAviso = pri;
            EditObject.SegundoAviso = seg;

            foreach (var todel in ToDelete)
                DAOFactory.TipoDocumentoDAO.DeleteValoresParametro(todel);

            var list = GetParametersFromView();
            EditObject.Parametros.Clear();
            foreach (var parametro in list)
                EditObject.Parametros.Add(parametro);

            DAOFactory.TipoDocumentoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEmpty(txtNombre.Text, "NAME");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");
        }

        protected void cbTipoParam_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetView();
        }

        protected void btNewParam_Click(object sender, EventArgs e)
        {
            var parameter = new TipoDocumentoParametro {Largo = 10};
            var par = NewParams;
            par.Add(parameter);
            NewParams = par;
            var list = GetParametersFromView();
            list.Add(parameter);
            ReBindParametros(list);
        }

        protected void gridParametros_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var param = e.Row.DataItem as TipoDocumentoParametro;
            if(param == null) return;

            var txtNombreParam = e.Row.FindControl("txtNombreParam") as TextBox;
            var cbTipoParam = e.Row.FindControl("cbTipoParam") as DropDownList;
            var txtLargo = e.Row.FindControl("txtLargo") as TextBox;
            var txtPrecision = e.Row.FindControl("txtPrecision") as TextBox;
            var chkObligatorio = e.Row.FindControl("chkObligatorio") as CheckBox;
            var txtDefault = e.Row.FindControl("txtDefault") as TextBox;
            var txtOrden = e.Row.FindControl("txtOrden") as TextBox;
            var txtRepeticion = e.Row.FindControl("txtRepeticion") as TextBox;

            txtNombreParam.Text = param.Nombre;

            BindTipoDato(cbTipoParam);

            var li = cbTipoParam.Items.FindByValue(param.TipoDato??TipoParametroDocumento.String);
            if (li != null) li.Selected = true;          

            txtLargo.Text = param.Largo.ToString();
            txtPrecision.Text = param.Precision.ToString();

            txtOrden.Text = param.Orden.ToString(CultureInfo.InvariantCulture);
            txtRepeticion.Text = param.Repeticion.ToString();

            chkObligatorio.Checked = param.Obligatorio;

            txtDefault.Text = param.Default;
        }

        protected void gridParametros_ItemCommand(object sender, C1GridViewCommandEventArgs e)
        {
            switch(e.CommandName)
            {
                case "Eliminar":
                    DeleteParam(e.Row.RowIndex);
                    break;
            }
        }

        private void DeleteParam(int index)
        {
            var id = (int) gridParametros.DataKeys[index].Value;
            if(id > 0)
            {
                var todel = ToDelete;
                todel.Add(id);
                ToDelete = todel;
            }
            var list = GetParametersFromView();
            if (id == 0)
            {
                list.RemoveAt(index);
            }
            ReBindParametros(list);
        }

        private void BindParametros()
        {
            var pars = (from TipoDocumentoParametro t in EditObject.Parametros select t).ToList();
            pars.AddRange(NewParams);
            ReBindParametros(pars);
        }
        private void ReBindParametros(List<TipoDocumentoParametro> pars)
        {
            gridParametros.DataSource = pars;
            gridParametros.DataBind();
            ResetView();
        }

        private static void BindTipoDato(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_TEXT"), TipoParametroDocumento.String));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_BARCODE"), TipoParametroDocumento.Barcode));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_INTEGER"), TipoParametroDocumento.Integer));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_FLOAT"), TipoParametroDocumento.Float));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_DATETIME"), TipoParametroDocumento.DateTime));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_BOOL"), TipoParametroDocumento.Boolean));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_IMAGE"), TipoParametroDocumento.Image));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_VEHICULO"), TipoParametroDocumento.Coche));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_EMPLEADO"), TipoParametroDocumento.Chofer));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_TRANSPORTISTA"), TipoParametroDocumento.Aseguradora));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_CLIENTE"), TipoParametroDocumento.Cliente));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_EQUIPO"), TipoParametroDocumento.Equipo));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_TANQUE"), TipoParametroDocumento.Tanque));
            combo.Items.Add(new ListItem(CultureManager.GetLabel("DOC_FIELDTYPE_CENTRODECOSTO"), TipoParametroDocumento.CentroCostos));
        }

        private void GetNewData()
        {
            var par = NewParams;
            var p = 0;
            for (var i = 0; i < gridParametros.Rows.Count; i++)
            {
                var id = (int) gridParametros.DataKeys[i].Value;
                if (id > 0) continue;
                var item = gridParametros.Rows[i];
                var txtNombreParam = item.FindControl("txtNombreParam") as TextBox;
                var cbTipoParam = item.FindControl("cbTipoParam") as DropDownList;
                var txtLargo = item.FindControl("txtLargo") as TextBox;
                var txtPrecision = item.FindControl("txtPrecision") as TextBox;
                var chkObligatorio = item.FindControl("chkObligatorio") as CheckBox;
                var txtDefault = item.FindControl("txtDefault") as TextBox;
                var txtOrden = item.FindControl("txtOrden") as TextBox;
                var txtRepeticion = item.FindControl("txtRepeticion") as TextBox;

                par[p].Nombre = txtNombreParam.Text;
                par[p].TipoDato = cbTipoParam.SelectedValue;
                short largo, pres, repeticion;
                if (!short.TryParse(txtLargo.Text, out largo)) txtLargo.Text = "0";
                if (!short.TryParse(txtPrecision.Text, out pres)) txtPrecision.Text = "0";
                if (!short.TryParse(txtRepeticion.Text, out repeticion))
                {
                    txtRepeticion.Text = "1";
                    repeticion = 1;
                }
                par[p].Largo = largo;
                par[p].Precision = pres;
                par[p].Repeticion = repeticion;

                par[p].Obligatorio = chkObligatorio.Checked;
                par[p].Default = txtDefault.Text;
                double orden;
                if (!double.TryParse(txtOrden.Text.Replace(',', '.'), NumberStyles.None, CultureInfo.InvariantCulture, out orden)) 
                {
                    txtOrden.Text = i.ToString(CultureInfo.InvariantCulture);
                    orden = i; 
                }
                par[p].Orden = orden;
                p++;
            }
            NewParams = par;
        }
        private void ResetView()
        {
            foreach (C1GridViewRow item in gridParametros.Rows)
            {
                //var txtNombreParam = item.FindControl("txtNombreParam") as TextBox;
                var cbTipoParam = item.FindControl("cbTipoParam") as DropDownList;
                var txtLargo = item.FindControl("txtLargo") as TextBox;
                var txtPrecision = item.FindControl("txtPrecision") as TextBox;
                //var chkObligatorio = item.FindControl("chkObligatorio") as CheckBox;
                var txtDefault = item.FindControl("txtDefault") as TextBox;
                switch(cbTipoParam.SelectedValue)
                {
                    case TipoParametroDocumento.Integer:
                    case TipoParametroDocumento.Float:
                        txtLargo.Enabled = txtPrecision.Enabled = txtDefault.Enabled = true;
                        break;
                    case TipoParametroDocumento.String:
                    case TipoParametroDocumento.Barcode:
                        txtLargo.Enabled = txtDefault.Enabled = true;
                        txtPrecision.Enabled = false;
                        break;
                    case TipoParametroDocumento.DateTime:
                        txtLargo.Enabled = txtDefault.Enabled = txtPrecision.Enabled = false;
                        break;
                    case TipoParametroDocumento.Coche:
                    case TipoParametroDocumento.Chofer:
                    case TipoParametroDocumento.Aseguradora:
                    case TipoParametroDocumento.Usuario:
                    case TipoParametroDocumento.Equipo:
                    case TipoParametroDocumento.Image:
                        txtLargo.Enabled = txtDefault.Enabled = txtPrecision.Enabled = false;
                        break;
                }
            }
        }

        protected List<TipoDocumentoParametro> GetParametersFromView()
        {
            var todelete = ToDelete;
            var newparams = NewParams;
            var list = new List<TipoDocumentoParametro>();

            var newparcount = 0;
            var i = 0;
            foreach (C1GridViewRow row in gridParametros.Rows)
            {
                var id = (int)gridParametros.DataKeys[row.RowIndex].Value;
                if (todelete.Contains(id)) continue;

                TipoDocumentoParametro parametro = null;

                if (id == 0)
                {
                    parametro = newparams[newparcount];
                    parametro.TipoDocumento = EditObject;
                    newparcount++;
                }
                else
                {
                    parametro = (from TipoDocumentoParametro p in EditObject.Parametros where p.Id == id select p).First();
                }
                var txtNombreParam = row.FindControl("txtNombreParam") as TextBox;
                var cbTipoParam = row.FindControl("cbTipoParam") as DropDownList;
                var txtLargo = row.FindControl("txtLargo") as TextBox;
                var txtPrecision = row.FindControl("txtPrecision") as TextBox;
                var chkObligatorio = row.FindControl("chkObligatorio") as CheckBox;
                var txtDefault = row.FindControl("txtDefault") as TextBox;
                var txtOrden = row.FindControl("txtOrden") as TextBox;
                var txtRepeticion = row.FindControl("txtRepeticion") as TextBox;

                parametro.Nombre = txtNombreParam.Text;
                parametro.TipoDato = cbTipoParam.SelectedValue;
                short largo, pres, repeticion;
                if (!short.TryParse(txtLargo.Text, out largo)) txtLargo.Text = "0";
                if (!short.TryParse(txtPrecision.Text, out pres)) txtPrecision.Text = "0";
                if (!short.TryParse(txtRepeticion.Text, out repeticion))
                {
                    txtRepeticion.Text = "1";
                    repeticion = 1;
                }
                parametro.Largo = largo;
                parametro.Precision = pres;
                parametro.Obligatorio = chkObligatorio.Checked;
                parametro.Default = txtDefault.Text;
                parametro.Repeticion = repeticion;
                double orden;
                if (!double.TryParse(txtOrden.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out orden))
                {
                    txtOrden.Text = i.ToString(CultureInfo.InvariantCulture);
                    orden = i;
                }
                parametro.Orden = orden;

                list.Add(parametro);
                i++;
            }

            return list;
        }
    }
}
