using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.Soporte;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Security;

namespace Logictracker.Soporte
{
    public partial class SoporteTicketSoporteLista : SecuredListPage<SupportTicketVo>
    {
        protected override string RedirectUrl { get { return "TicketSoporteAlta.aspx"; } }
        protected override string VariableName { get { return "SUP_TICKETSOPORTE"; } }
        protected override string GetRefference() { return "SUPPORT"; }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            if (!IsPostBack) BindEstados();
        }

        protected override List<SupportTicketVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var desde = dtDesde.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.GetValueOrDefault()): new DateTime?();
            var hasta = dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.GetValueOrDefault()): new DateTime?();
            var tickets = DAOFactory.SupportTicketDAO.GetList(ddlLocacion.Selected, user, cbEstado.SelectedIndex - 1, desde, hasta, SearchString);

            return tickets.Select(ticket => new SupportTicketVo(ticket)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, SupportTicketVo ticket)
        {
            base.OnRowDataBound(grid, e, ticket);

            e.Row.BackColor = DAOFactory.SupportTicketDAO.GetColoresEstados()[ticket.CurrentState];

            GridUtils.GetCell(e.Row, SupportTicketVo.IndexFecha).Text = ticket.Fecha.ToString("dd/MM/yyyy HH:mm");
            GridUtils.GetCell(e.Row, SupportTicketVo.IndexCurrentState).Text = DAOFactory.SupportTicketDAO.GetEstados()[ticket.CurrentState];

            var problema = DAOFactory.SupportTicketDAO.GetTiposProblema()[ticket.TipoProblema];

            problema += ": " + DAOFactory.SupportTicketDAO.GetCategoriasProblemaByTipo(ticket.TipoProblema)[ticket.Categoria < 0 ? 0 : ticket.Categoria];

            var desc = ticket.Descripcion.Length > 20 ? ticket.Descripcion.Substring(0, 20) + "..." : ticket.Descripcion;

            GridUtils.GetCell(e.Row, SupportTicketVo.IndexDescripcion).Text = desc;
            GridUtils.GetCell(e.Row, SupportTicketVo.IndexTipoProblema).Text = problema;
        }

        private void BindEstados()
        {
            var estados = DAOFactory.SupportTicketDAO.GetEstados();

            cbEstado.Items.Clear();
            cbEstado.Items.Add("Todos");

            foreach (var estado in estados) cbEstado.Items.Add(estado);
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var estado = data["ESTADO"];
            var desde = data["DESDE"];
            var hasta = data["HASTA"];
            if (empresa != null) ddlLocacion.SetSelectedValue((int) empresa);
            if (estado != null) cbEstado.SelectedIndex = (int) estado;
            if (desde != null) dtDesde.SelectedDate = (DateTime?) desde;
            if (hasta != null) dtHasta.SelectedDate = (DateTime?) hasta;
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlLocacion.Selected);
            data.Add("ESTADO", cbEstado.SelectedIndex);
            data.Add("DESDE", dtDesde.SelectedDate);
            data.Add("HASTA", dtHasta.SelectedDate);
            return data;
        }
    }
}