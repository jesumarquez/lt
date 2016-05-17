using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ValueObjects.Mensajes;
using Logictracker.Web.BaseClasses.BasePages;
using Image = System.Web.UI.WebControls.Image;

namespace Logictracker.Parametrizacion
{
    public partial class AccionLista : SecuredListPage<AccionVo>
    {
        #region Private Properties

        /// <summary>
        /// True or false behaivour representative icons.
        /// </summary>
        private static readonly string TrueIcon = String.Concat(ImagesDir, "accept.png");
        private static readonly string FalseIcon = String.Concat(ImagesDir, "cancel.png");

        #endregion

        #region Protected Properties

        protected override string RedirectUrl { get { return "AccionesAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_ACCIONES"; } }
        protected override string GetRefference() { return "ACCION"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override void GenerateCustomColumns()
        {
            base.GenerateCustomColumns();
            Grid.Columns[AccionVo.IndexReportaAssistCargo].Visible = WebSecurity.IsSecuredAllowed(Securables.AssistCargoInterface);
        }

        protected override List<AccionVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            var empresa = ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : linea != null ? linea.Empresa : null;

            return DAOFactory.AccionDAO.FindByDistritoYBase(empresa, linea, user).Cast<Accion>().Select(a => new AccionVo(a)).ToList();
        }

        /// <summary>
        /// Generates the Template columns for the action behaviours.
        /// </summary>
        /// <param name="row"></param>
        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            CreateImageTemplate(row, "imgGrabaEnBase", AccionVo.IndexGrabaEnBase);
            CreateImageTemplate(row, "imgCambiaMensaje", AccionVo.IndexCambiaMensaje);
            CreateImageTemplate(row, "imgEsPopUp", AccionVo.IndexEsPopUp);
            CreateImageTemplate(row, "imgRequiereAtencion", AccionVo.IndexRequiereAtencion);
            CreateImageTemplate(row, "imgEsAlarmaSonora", AccionVo.IndexEsAlarmaSonora);
            CreateImageTemplate(row, "imgEnviaMails", AccionVo.IndexEnviaMails);
            CreateImageTemplate(row, "imgEnviaSms", AccionVo.IndexEnviaSms);
            CreateImageTemplate(row, "imgHabilitaUsuario", AccionVo.IndexHabilitaUsuario);
            CreateImageTemplate(row, "imgInhabilitaUsuario", AccionVo.IndexInHabilitaUsuario);
            CreateImageTemplate(row, "imgModificaIcono", AccionVo.IndexModificaIcono);
            CreateImageTemplate(row, "imgPideFoto", AccionVo.IndexPideFoto);
            CreateImageTemplate(row, "imgEvaluaGeocerca", AccionVo.IndexEvaluaGeocerca);
            CreateImageTemplate(row, "imgReportaAssistCargo", AccionVo.IndexReportaAssistCargo);
            CreateImageTemplate(row, "imgEnviaReporte", AccionVo.IndexEnviaReporte);
            CreateImageTemplate(row, "imgReportaResponsable", AccionVo.IndexReportaResponsable);
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, AccionVo dataItem)
        {
            CreateRowTemplate(e.Row);

            SetActionColors(e, dataItem);

            SetBehaivoursIcons(e, dataItem);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the current action back and fore colors.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="accion"></param>
        private static void SetActionColors(C1GridViewRowEventArgs e, AccionVo accion)
        {
            var color = Color.FromArgb(accion.Alpha, accion.Red, accion.Green, accion.Blue);

            e.Row.BackColor = color;
            e.Row.ForeColor = color.GetBrightness() > 0.45 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Displays the current actions behaivour with representative icons.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="accion"></param>
        private void SetBehaivoursIcons(C1GridViewRowEventArgs e, AccionVo accion)
        {
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexGrabaEnBase).FindControl("imgGrabaEnBase")).ImageUrl = accion.GrabaEnBase ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexCambiaMensaje).FindControl("imgCambiaMensaje")).ImageUrl = accion.CambiaMensaje ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexEsPopUp).FindControl("imgEsPopUp")).ImageUrl = accion.EsPopUp ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexRequiereAtencion).FindControl("imgRequiereAtencion")).ImageUrl = accion.RequiereAtencion ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexEsAlarmaSonora).FindControl("imgEsAlarmaSonora")).ImageUrl = accion.EsAlarmaSonora ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexEnviaMails).FindControl("imgEnviaMails")).ImageUrl = accion.EsAlarmaDeMail ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexEnviaSms).FindControl("imgEnviaSms")).ImageUrl = accion.EsAlarmaSms ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexHabilitaUsuario).FindControl("imgHabilitaUsuario")).ImageUrl = accion.Habilita ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexInHabilitaUsuario).FindControl("imgInhabilitaUsuario")).ImageUrl = accion.Inhabilita ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexModificaIcono).FindControl("imgModificaIcono")).ImageUrl = accion.ModificaIcono ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexPideFoto).FindControl("imgPideFoto")).ImageUrl = accion.PideFoto ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexEvaluaGeocerca).FindControl("imgEvaluaGeocerca")).ImageUrl = accion.EvaluaGeocerca ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexReportaAssistCargo).FindControl("imgReportaAssistCargo")).ImageUrl = accion.ReportaAssistCargo ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexEnviaReporte).FindControl("imgEnviaReporte")).ImageUrl = accion.EnviaReporte ? TrueIcon : FalseIcon;
            ((Image)GridUtils.GetCell(e.Row, AccionVo.IndexReportaResponsable).FindControl("imgReportaResponsable")).ImageUrl = accion.ReportaResponsable ? TrueIcon : FalseIcon;
        }

        /// <summary>
        /// Creates the template for the ImageButton that represents an action property.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="controlId"></param>
        /// <param name="originalColumnIndex"> </param>
        private void CreateImageTemplate(C1GridViewRow row, string controlId, int originalColumnIndex)
        {
            var imgbtn = GridUtils.GetCell(row, originalColumnIndex).FindControl(controlId) as Image;
            if (imgbtn == null) GridUtils.GetCell(row, originalColumnIndex).Controls.Add(new Image { ID = controlId });
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, ddlDistrito);
            data.LoadStaticFilter(FilterData.StaticBase, ddlBase);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            return data;
        }
    }
}