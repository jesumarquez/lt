using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class Documentos_ListDocumento : SecuredListPage<PartePersonalVo>
    {
        protected override string RedirectUrl { get { return "AltaDocumento.aspx?t=1"; } }
        protected override string VariableName { get { return "COST_PARTES"; } }
        protected override string GetRefference() { return "PARTE"; }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtInicio.SelectedDate = DateTime.Now.Date;
                dtFin.SelectedDate = DateTime.Now.Date;
            }
            base.OnLoad(e);
        }

        #region Protected Methods

        protected override List<PartePersonalVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (!dtInicio.SelectedDate.HasValue) ThrowMustEnter("DESDE");
            if (!dtFin.SelectedDate.HasValue) ThrowMustEnter("HASTA");

            var dc = DAOFactory.DocumentoDAO.FindList(cbTransportistas.Selected, ddlPlanta.Selected, ddlMovil.Selected,
                                                      dtInicio.SelectedDate.Value.ToDataBaseDateTime(), dtFin.SelectedDate.Value.ToDataBaseDateTime(),
                                                      Convert.ToInt32(cbEstado.SelectedValue), ddlEquipo.Selected, user);
            return dc.OfType<Documento>().Select(d => new PartePersonalVo(new PartePersonal(d, DAOFactory))).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, PartePersonalVo dataItem)
        {
            e.Row.BackColor = dataItem.Estado == 1 ? Color.Yellow
                                  : dataItem.Estado == 2 ? Color.LightGreen
                                        : Color.LightCoral;
        }

        protected void btnSearch_Click(object o, EventArgs e)
        {
            Bind();
        }
        #endregion

        protected override void FilterChangedHandler(object sender, EventArgs e)
        {
            base.FilterChangedHandler(sender, e);
            ddlMovil.Enabled = ddlPlanta.Selected != -1;
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            var transportista = data[FilterData.StaticTransportista];
            var coche = data[FilterData.StaticVehiculo];
            var equipo = data[FilterData.StaticEquipo];
            var estado = data["ESTADO"];
            var desde = data["DESDE"];
            var hasta = data["HASTA"];
            if (empresa != null) ddlDistrito.SetSelectedValue((int)empresa);
            if (linea != null) ddlPlanta.SetSelectedValue((int)linea);
            if (transportista != null) cbTransportistas.SetSelectedValue((int)transportista);
            if (coche != null) ddlMovil.SetSelectedValue((int)coche);
            if (equipo != null) ddlEquipo.SetSelectedValue((int)equipo);
            if (estado != null) cbEstado.SelectedIndex = (int)estado;
            if (desde != null) dtInicio.SelectedDate = (DateTime)desde;
            if (hasta != null) dtFin.SelectedDate = (DateTime)hasta;
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlPlanta.Selected);
            data.AddStatic(FilterData.StaticTransportista, cbTransportistas.Selected);
            data.AddStatic(FilterData.StaticVehiculo, ddlMovil.Selected);
            data.AddStatic(FilterData.StaticEquipo, ddlEquipo.Selected);
            data.Add("ESTADO", cbEstado.SelectedIndex);
            data.Add("DESDE", dtInicio.SelectedDate);
            data.Add("HASTA", dtFin.SelectedDate);
            return data;
        }

    }
}