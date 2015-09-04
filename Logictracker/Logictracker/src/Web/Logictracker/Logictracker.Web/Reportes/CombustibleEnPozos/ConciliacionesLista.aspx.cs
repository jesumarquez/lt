#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class ReportesCombustibleEnPozosConciliacionesLista : SecuredListPage<ConciliacionVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "CONCILIACIONES_LIST"; } }
        protected override string RedirectUrl { get { return "Conciliaciones.aspx"; } }
        protected override string GetRefference() { return "CONCILIACIONES"; }

        #endregion

        #region Protected Methods

        protected override List<ConciliacionVo> GetListData()
        {
            if (cbTanque.SelectedIndex < 0 || !dpDesde.SelectedDate.HasValue || !dpHasta.SelectedDate.HasValue) 
                return new List<ConciliacionVo>();

            var list = DAOFactory.MovimientoDAO.FindConciliacionesByTanqueAndFecha(
                cbTanque.Selected, dpDesde.SelectedDate.Value, dpHasta.SelectedDate.Value);

            return list.Select(m => new ConciliacionVo(m)).ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }
        #endregion
    }
}
