using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.ValueObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Parametrizacion
{
    public partial class GeoRefLista : SecuredListPage<ReferenciaGeograficaVo>
    {
        #region Protected Properties

        protected override string RedirectUrl { get { return "GeoRefAlta.aspx"; } }
        protected override string ImportUrl { get { return "GeoRefImport.aspx"; } }
        protected override string VariableName { get { return "PAR_POI"; } }
        protected override string GetRefference() { return "DOMICILIO"; }
        protected override bool DeleteButton { get { return true; } }
        protected override bool ImportButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        
        #endregion

        #region Private Properties

        private Usuario _user;
        private Usuario user { get { return _user ?? (_user = DAOFactory.UsuarioDAO.FindById(Usuario.Id)); } }

        #endregion

        #region Protected Methods

        protected override List<ReferenciaGeograficaVo> GetListData()
        {
            return
                DAOFactory.ReferenciaGeograficaDAO.GetList(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected},
                                                           new[] {cbTipoReferenciaGeografica.Selected})
                    .Select(r => new ReferenciaGeograficaVo(r))
                    .ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReferenciaGeograficaVo dataItem)
        {
            var clickScript = ClientScript.GetPostBackEventReference(Grid, string.Format("Select:{0}", e.Row.RowIndex));

            e.Row.Attributes.Remove("onclick");

            var checkIndex = GridUtils.GetColumnIndex(ReferenciaGeograficaVo.IndexCheck);
            for (var i = 0; i < e.Row.Cells.Count; i++)
                if (i != checkIndex) GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", clickScript);

            if (!string.IsNullOrEmpty(dataItem.IconUrl))
                GridUtils.GetCell(e.Row, ReferenciaGeograficaVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", IconDir + dataItem.IconUrl);
            else if (!string.IsNullOrEmpty(dataItem.Color))
                GridUtils.GetCell(e.Row, ReferenciaGeograficaVo.IndexIconUrl).Text = string.Format("<div style='margin: 6px; width: 20px; height: 20px; background-color:#{0};'></div>", dataItem.Color);


            if (dataItem.IsPoint) GridUtils.GetCell(e.Row, ReferenciaGeograficaVo.IndexIsPoint).Controls.Add(new Image { ID = "imgPoint", ImageUrl = "~/images/point.png" });
            if (dataItem.IsPolygon) GridUtils.GetCell(e.Row, ReferenciaGeograficaVo.IndexIsPolygon).Controls.Add(new Image { ID = "imgPolygon", ImageUrl = "~/images/polygon.png" });


            var lineas = user.Lineas.OfType<Linea>().Select(l => l.Id);
            var empresas = user.Empresas.OfType<Empresa>().Select(em => em.Id);
            var enabled  = ((!lineas.Any() || (dataItem.Linea != -1 && lineas.Contains(dataItem.Linea)))
                && (!empresas.Any() || (dataItem.Empresa != -1 && empresas.Contains(dataItem.Empresa))));

            var linXemp = dataItem.Empresa > 0 ? DAOFactory.LineaDAO.FindList(new[]{dataItem.Empresa}).ToList() : null;
            enabled |= (dataItem.Linea == -1 && dataItem.Empresa != -1 && user.Lineas.ContainsAll(linXemp));

            (e.Row.FindControl("chkSelected") as CheckBox).Enabled = enabled;
        }

        protected override void CreateHeaderTemplate(C1GridViewRow row)
        {
            if(row == null) return;
           
            var chk = row.FindControl("chkSelectAll") as CheckBox;
            if (chk == null) GridUtils.GetCell(row, ReferenciaGeograficaVo.IndexCheck).Controls.Add(chk = new CheckBox { ID = "chkSelectAll", AutoPostBack = true });
            chk.CheckedChanged += ChkSelectAllCheckedChanged;
        }
        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            var check = (GridUtils.GetCell(row, ReferenciaGeograficaVo.IndexCheck).FindControl("chkSelected") as CheckBox);
            if (check == null) GridUtils.GetCell(row, ReferenciaGeograficaVo.IndexCheck).Controls.Add(check = new CheckBox { ID = "chkSelected" });
        }

        protected override void Delete()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var toDelete =
                        Grid.Rows.OfType<C1GridViewRow>()
                            .Where(r => (GridUtils.GetCell(r, ReferenciaGeograficaVo.IndexCheck).FindControl("chkSelected") as CheckBox).Checked)
                            .Select(r => Convert.ToInt32(Grid.DataKeys[r.RowIndex].Value));

                    if (!toDelete.Any()) throw new ApplicationException("No hay ningun elemento seleccionado");

                    foreach (var id in toDelete)
                    {
                        if (!DAOFactory.ReferenciaGeograficaDAO.ValidateDelete(id)) throw new ApplicationException(CultureManager.GetError("DELETE_GEO_REF"));

                        var georef = DAOFactory.ReferenciaGeograficaDAO.FindById(id);

                        georef.Baja = true;

                        if (georef.Vigencia == null) georef.Vigencia = new Vigencia();

                        georef.Vigencia.Fin = DateTime.UtcNow;

                        DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(georef);
                        STrace.Trace("QtreeReset", "GeoRefLista");
                    }

                    transaction.Commit();

                    Bind();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowError(ex);                    
                }
            }
        }

        protected void ChkSelectAllCheckedChanged(object sender, EventArgs e)
        {
            var enabled = Grid.Rows.OfType<C1GridViewRow>().Select(r => GridUtils.GetCell(r, ReferenciaGeograficaVo.IndexCheck).FindControl("chkSelected")).OfType<CheckBox>().Where(chk => chk.Enabled);
            var checkAll = enabled.Count(c => c.Checked) == 0;

            foreach (var box in enabled) box.Checked = checkAll;

            (sender as CheckBox).Checked = checkAll;
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var tipo = data[FilterData.StaticTipoReferenciaGeografica];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
            if (tipo != null) cbTipoReferenciaGeografica.SetSelectedValue((int)tipo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoReferenciaGeografica, cbTipoReferenciaGeografica.Selected);
            return data;
        }
    }
}