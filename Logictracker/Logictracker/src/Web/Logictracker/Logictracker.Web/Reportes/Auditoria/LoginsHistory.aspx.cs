using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Auditoria;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reportes.Auditoria
{
    public partial class Reportes_Auditoria_LoginsHistory : SecuredGridReportPage<LoginAuditVo>
    {
        protected override string VariableName { get { return "AUD_LOGIN_HISTORY"; } }
        protected override string GetRefference() { return "AUD_LOGIN_HIST"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<LoginAuditVo> GetResults()
        {
            var currentUser = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var users = (from Usuario u in DAOFactory.UsuarioDAO.FindByUsuario(currentUser) select u.Id).ToList();

            return DAOFactory.LoginAuditDAO.GetAuditHistory(users, 
                                                            SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()),
                                                            SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()))
                .Select(a => new LoginAuditVo(a)).ToList();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override void OnRowDataBound(C1.Web.UI.Controls.C1GridView.C1GridView grid, C1.Web.UI.Controls.C1GridView.C1GridViewRowEventArgs e, LoginAuditVo dataItem)
        {
            if (dataItem == null) return;

            if (!dataItem.FechaFin.HasValue) GridUtils.GetCell(e.Row, LoginAuditVo.IndexFechaFin).Text = string.Empty;

        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }
    }
}
