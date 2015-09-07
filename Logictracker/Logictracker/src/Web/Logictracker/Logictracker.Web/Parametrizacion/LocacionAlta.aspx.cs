using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class LocacionAlta : SecuredAbmPage<Empresa>
    {
        protected override string VariableName { get { return "PAR_REGION"; } }
        protected override string RedirectUrl { get { return "LocacionLista.aspx"; } }
        protected override string GetRefference() { return "LOCACION"; }

        #region Protected Methods

        /// <summary>
        /// Validates current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            ValidateEmpty(txtCodigo.Text, "CODE");
            
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            var frecuencia = ValidateEmpty(txtFrecuenciaReporte.Text, "FRECUENCIA_REPORTE_AC");
            var frecuenciaInt = ValidateInt32(frecuencia, "FRECUENCIA_REPORTE_AC");
            if (frecuenciaInt < 0) ThrowInvalidValue("FRECUENCIA_REPORTE_AC");
        }

        /// <summary>
        /// Saves current object.
        /// </summary>
        protected override void OnSave()
        {
            var identificaChoferesChanged = EditObject.IdentificaChoferes != chkIdentificaChoferes.Checked;

            EditObject.Codigo = txtCodigo.Text;
            EditObject.RazonSocial = txtDescripcion.Text.Trim();
            EditObject.TimeZoneId = ddlTimeZone.SelectedValue;
            EditObject.IdentificaChoferes = chkIdentificaChoferes.Checked;
            EditObject.CochesPorDistrito = chkChoferesPorBase.Checked;
            EditObject.FrecuenciaReporte = Convert.ToInt32(txtFrecuenciaReporte.Text.Trim());

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {

                    var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

                    if (!EditMode &&
                        user.PorEmpresa)
                    {

                        user.AddEmpresa(DAOFactory.EmpresaDAO.FindById(EditObject.Id));

                        DAOFactory.UsuarioDAO.SaveOrUpdate(user);
                    }

                    #region Parametros

                    var parametros = new List<ParametroEmpresa>();
                    foreach (C1GridViewRow row in gridParametros.Rows)
                    {
                        var nombre = (row.FindControl("txtNombre") as TextBox).Text.Trim();
                        var valor = (row.FindControl("txtValor") as TextBox).Text.Trim();
                        if (string.IsNullOrEmpty(nombre) ||
                            string.IsNullOrEmpty(valor)) continue;
                        var param = EditObject.Parametros.Where(p => p.Nombre == nombre).FirstOrDefault() ??
                                    new ParametroEmpresa {Nombre = nombre, Valor = valor, Empresa = EditObject};
                        param.Valor = valor;
                        parametros.Add(param);
                    }
                    EditObject.Parametros.Clear();
                    foreach (var p in parametros) EditObject.Parametros.Add(p);

                    #endregion

                    DAOFactory.EmpresaDAO.SaveOrUpdate(EditObject);

                    transaction.Commit();

                    if (EditMode && identificaChoferesChanged) UpdateCochesIdentificaChofer(EditObject.IdentificaChoferes);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        private void UpdateCochesIdentificaChofer(bool identificaChoferes)
        {
            var coches = DAOFactory.CocheDAO.GetList(new[] { EditObject.Id }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 })
                .Where(c => c.Transportista == null && c.Linea == null && c.Empresa != null && c.IdentificaChoferes != identificaChoferes);

            foreach (var c in coches)
            {
                c.IdentificaChoferes = identificaChoferes;
                DAOFactory.CocheDAO.SaveOrUpdate(c);
            }
        }

        /// <summary>
        /// Deletes current edited object.
        /// </summary>
        protected override void OnDelete() { DAOFactory.EmpresaDAO.Delete(EditObject); }

        /// <summary>
        /// Binds current edited object.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.RazonSocial;
            chkIdentificaChoferes.Checked = EditObject.IdentificaChoferes;
            chkChoferesPorBase.Checked = EditObject.CochesPorDistrito;
            txtFrecuenciaReporte.Text = EditObject.FrecuenciaReporte.ToString();

            if (string.IsNullOrEmpty(EditObject.TimeZoneId)) return;

            var item = ddlTimeZone.Items.FindByValue(EditObject.TimeZoneId);

            if (item != null) ddlTimeZone.SelectedIndex = ddlTimeZone.Items.IndexOf(item);

            gridParametros.DataSource = EditObject.Parametros;
            gridParametros.DataBind();
        }

    #endregion

        protected void gridParametros_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var details = e.Row.DataItem as ParametroEmpresa;

            if (details == null) return;
            (e.Row.FindControl("txtNombre") as TextBox).Text = details.Nombre;
            (e.Row.FindControl("txtValor") as TextBox).Text = details.Valor;
        }
        protected void btAddParameter_Click(object sender, EventArgs e)
        {
            var parametros = new List<ParametroEmpresa>();
            foreach (C1GridViewRow row in gridParametros.Rows)
            {
                var nombre = (row.FindControl("txtNombre") as TextBox).Text.Trim();
                var valor = (row.FindControl("txtValor") as TextBox).Text.Trim();
                var param = new ParametroEmpresa { Nombre = nombre, Valor = valor };
                parametros.Add(param);
            }
            parametros.Add(new ParametroEmpresa());
            gridParametros.DataSource = parametros;
            gridParametros.DataBind();
        }
    }
}