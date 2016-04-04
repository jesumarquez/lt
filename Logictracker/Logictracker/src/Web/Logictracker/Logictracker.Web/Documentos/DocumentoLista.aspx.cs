using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;
using System;
using Logictracker.Security;

namespace Logictracker.Documentos
{
    public partial class DocumentoLista : SecuredListPage<DocumentoVo>
    {
        protected override string VariableName { get { return "DOC_DOCUMENTOS"; } }
        protected override string GetRefference() { return "DOCUMENTO"; }
        protected override string RedirectUrl { get { return "DocumentoAlta.aspx?t=" + cbTipoDocumento.SelectedValue; } }
        protected override string ImportUrl { get { return "DocumentoImport.aspx"; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ImportButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
            }
            base.OnLoad(e);
        }

        protected override List<DocumentoVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            return DAOFactory.DocumentoDAO.FindByTipoAndUsuario(user, cbTipoDocumento.Selected, cbEmpresa.Selected, cbLinea.Selected, cbTransportista.Selected)
                .OfType<Documento>()
                .Where(x => x.FechaAlta >= SecurityExtensions.ToDataBaseDateTime(dtpDesde.SelectedDate.Value) &&
                    x.FechaAlta <= SecurityExtensions.ToDataBaseDateTime(dtpHasta.SelectedDate.Value))
                .Select(d=> new DocumentoVo(d))
                .ToList()
                ;
        }

        protected void FilterChanged(object sender, EventArgs e) { if (IsPostBack) Bind(); }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            SetColumnVisibility();
        }

        /// <summary>
        /// Muestra solamente las columnas de entidad a las que aplica el tipo de documento seleccionado.
        /// </summary>
        protected void SetColumnVisibility()
        {
            if (cbTipoDocumento.Selected <= 0) return;
        
            var tipoDocumento = DAOFactory.TipoDocumentoDAO.FindById(cbTipoDocumento.Selected);

            var indexVehiculo = GridUtils.GetColumnIndex(DocumentoVo.IndexVehiculo);
            if (Grid.Columns.Count > indexVehiculo)
                GridUtils.GetColumn(DocumentoVo.IndexVehiculo).Visible = tipoDocumento.AplicarAVehiculo;

            var indexEmpleado = GridUtils.GetColumnIndex(DocumentoVo.IndexEmpleado);
            if (Grid.Columns.Count > indexEmpleado)
                GridUtils.GetColumn(DocumentoVo.IndexEmpleado).Visible = tipoDocumento.AplicarAEmpleado;

            var indexTransportista = GridUtils.GetColumnIndex(DocumentoVo.IndexTransportista);
            if (Grid.Columns.Count > indexTransportista)
                GridUtils.GetColumn(DocumentoVo.IndexTransportista).Visible = tipoDocumento.AplicarATransportista;

            var indexEquipo = GridUtils.GetColumnIndex(DocumentoVo.IndexEquipo);
            if (Grid.Columns.Count > indexEquipo)
                GridUtils.GetColumn(DocumentoVo.IndexEquipo).Visible = tipoDocumento.AplicarAEquipo;
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticTipoDocumento, cbTipoDocumento);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticTipoDocumento, cbTipoDocumento.Selected);
            return data;
        }
    }
}
