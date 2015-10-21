using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Parametrizacion
{
    public partial class LineaTelefonicaAlta : SecuredAbmPage<LineaTelefonica>
    {
        protected override string VariableName { get { return "PAR_LINEA_TELEFONICA"; } }

        protected override string RedirectUrl { get { return "LineaTelefonicaLista.aspx"; } }

        protected override string GetRefference() { return "PAR_LINEA_TELEFONICA"; }

        protected override bool AddButton { get { return false; } }

        protected override bool DuplicateButton { get { return false; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                dtDesde.SelectedDate = DateTime.Today.ToDataBaseDateTime();
        }

        protected override void Bind()
        {
            txtNumeroLinea.Text = EditObject.NumeroLinea;
            txtImei.Text = EditObject.Imei;

            gridPlanes.Visible = EditObject.Vigencias.Count > 0;

            if (gridPlanes.Visible)
            {
                gridPlanes.DataSource = GetPlanesList();
                gridPlanes.DataBind();
            }
        }

        protected override void OnDelete()
        {
            var dispositivos = DAOFactory.DispositivoDAO.GetByLineaTelefonica(EditObject.Id);

            if (dispositivos.Count == 0)
                DAOFactory.LineaTelefonicaDAO.Delete(EditObject);
            else
                ThrowError("LINEA_TELEFONICA_ASIGNADA", new[] { dispositivos[0].Codigo });
        }

        protected override void OnSave()
        {
            EditObject.NumeroLinea = txtNumeroLinea.Text.Trim();
            EditObject.Imei = txtImei.Text.Trim();

            DAOFactory.LineaTelefonicaDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var numero = ValidateEmpty((string) txtNumeroLinea.Text, (string) "NUMERO_LINEA");
            var imei = ValidateEmpty((string) txtImei.Text, (string) "SIM");

            if (!DAOFactory.LineaTelefonicaDAO.IsNumberUnique(EditObject.Id, numero))
                ThrowDuplicated("NUMERO_LINEA");

            if (!DAOFactory.LineaTelefonicaDAO.IsImeiUnique(EditObject.Id, imei))
                ThrowDuplicated("SIM");
        }

        protected void GridPlanes_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var vigencia = e.Row.DataItem as VigenciaPlanLineaVo;
                if (vigencia != null)
                {
                    ((Label)e.Row.Cells[0].FindControl("lblDesde")).Text = vigencia.Inicio;
                    ((Label)e.Row.Cells[1].FindControl("lblHasta")).Text = vigencia.Fin;
                    ((Label)e.Row.Cells[2].FindControl("lblPlan")).Text = vigencia.Plan;
                    ((Label)e.Row.Cells[3].FindControl("lblEmpresa")).Text = vigencia.Empresa;
                }
            }
        }

        protected void BtnGuardar_OnClick(object sender, EventArgs e)
        {
            ValidarDatosVigencia();

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                if (EditObject.Id == 0)
                    GuardarNuevaLinea();

                var nuevaVigencia = new VigenciaPlanLinea
                                    {
                                        Inicio = SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value),
                                        LineaTelefonica = EditObject,
                                        Plan = cbPlan.Selected > 0 ? DAOFactory.PlanDAO.FindById(cbPlan.Selected) : null
                                    };

                ValidarCondicionesVigencia(nuevaVigencia);

                EditObject.Vigencias.Add(nuevaVigencia);

                DAOFactory.LineaTelefonicaDAO.SaveOrUpdate(EditObject);

                transaction.Commit();
            }
            Response.Redirect(RedirectUrl);
        }

        private void ValidarDatosVigencia()
        {
            if (!dtDesde.SelectedDate.HasValue)
            {
                DisplayError(CultureManager.GetLabel("VIGENCIA_DESDE"));
                return;
            }

            if (cbPlan.Selected <= 0)
            {
                DisplayError(CultureManager.GetEntity("PARENTI73"));
                return;
            }
        }

        private void GuardarNuevaLinea()
        {
            if (string.IsNullOrEmpty(txtNumeroLinea.Text.Trim()))
            {
                DisplayError(string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetLabel("NUMERO_LINEA")));
                return;
            }
            if (string.IsNullOrEmpty(txtImei.Text.Trim()))
            {
                DisplayError(string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetLabel("SIM")));
                return;
            }

            if (!DAOFactory.LineaTelefonicaDAO.IsNumberUnique(EditObject.Id, txtNumeroLinea.Text.Trim()))
            {
                DisplayError(string.Format(CultureManager.GetError("DUPLICATED"), CultureManager.GetLabel("NUMERO_LINEA")));
                return;
            }

            if (!DAOFactory.LineaTelefonicaDAO.IsImeiUnique(EditObject.Id, txtImei.Text.Trim()))
            {
                DisplayError(string.Format(CultureManager.GetError("DUPLICATED"), CultureManager.GetLabel("SIM")));
                return;
            }

            EditObject.NumeroLinea = txtNumeroLinea.Text.Trim();
            EditObject.Imei = txtImei.Text.Trim();

            DAOFactory.LineaTelefonicaDAO.SaveOrUpdate(EditObject);
        }

        private void ValidarCondicionesVigencia(VigenciaPlanLinea nuevaVigencia)
        {
            if (dtHasta.SelectedDate.HasValue) // SI SE DEFINE UNA FECHA FIN
            {
                nuevaVigencia.Fin = SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value);

                // LA FECHA DE FIN NO TIENE QUE ESTAR CONTENIDA EN NINGUNA VIGENCIA EXISTENTE
                var vigenteFin = EditObject.Vigencias.Cast<VigenciaPlanLinea>()
                                                     .Where(v => nuevaVigencia.Fin > v.Inicio
                                                              && nuevaVigencia.Fin < v.Fin);
                if (vigenteFin.Count() > 0)
                {
                    DisplayError(string.Format(CultureManager.GetError("INVALID_VALUE"), CultureManager.GetLabel("VIGENCIA_HASTA")));
                    return;
                }
            }
            else // SI NO SE DEFINE UNA FECHA FIN
            {
                // NO PUEDE EXISTIR UNA VIGENCIA POSTERIOR A LA FECHA INICIO
                var vigenciasPosteriores = EditObject.Vigencias.Cast<VigenciaPlanLinea>().Where(v => v.Inicio > nuevaVigencia.Inicio);
                if (vigenciasPosteriores.Count() > 0)
                {
                    DisplayError(string.Format(CultureManager.GetError("INVALID_VALUE"), CultureManager.GetLabel("VIGENCIA_DESDE")));
                    return;
                }
            }

            // LA FECHA DE INICIO NO TIENE QUE ESTAR CONTENIDA EN NINGUNA VIGENCIA EXISTENTE
            var vigenteInicio = EditObject.Vigencias.Cast<VigenciaPlanLinea>()
                                                    .Where(v => nuevaVigencia.Inicio > v.Inicio
                                                             && nuevaVigencia.Inicio < v.Fin);
            if (vigenteInicio.Count() > 0)
            {
                DisplayError(string.Format(CultureManager.GetError("INVALID_VALUE"), CultureManager.GetLabel("VIGENCIA_DESDE")));
                return;
            }

            // SI HAY ALGUNA VIGENCIA SIN FECHA DE FIN
            // Y CON FECHA DE INICIO MENOR A LA FECHA DE INICIO DE LA NUEVA VIGENCIA
            // => SE ASIGNA SU FECHA DE FIN COMO LA FECHA DE INICIO DE LA NUEVA VIGENCIA
            var vigenteActual = EditObject.Vigencias.Cast<VigenciaPlanLinea>()
                                                    .Where(v => v.Inicio < nuevaVigencia.Inicio
                                                             && v.Fin == null);

            if (vigenteActual.Count() > 0 && vigenteActual.First() != null && vigenteActual.First().Fin == null && vigenteActual.First().Inicio < nuevaVigencia.Inicio)
                vigenteActual.First().Fin = nuevaVigencia.Inicio;
        }
                  
        private List<VigenciaPlanLineaVo> GetPlanesList()
        {
            return EditObject.Vigencias.OfType<VigenciaPlanLinea>()
                             .Select(o => new VigenciaPlanLineaVo(o))
                             .OrderByDescending(o => o.Inicio)
                             .ToList();
        }
    }
}
