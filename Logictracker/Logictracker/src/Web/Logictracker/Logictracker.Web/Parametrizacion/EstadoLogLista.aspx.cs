#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_EstadoLogLista : SecuredListPage<EstadoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_ESTADOLOG"; } }
        protected override string RedirectUrl { get { return "EstadoLogAlta.aspx"; } }
        protected override string GetRefference() { return "ESTADOLOG"; }
        protected override bool DuplicateButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<EstadoVo> GetListData()
        {
            return DAOFactory.EstadoDAO.GetByPlanta(cbLinea.Selected).OfType<Estado>().Select(e=>new EstadoVo(e)).ToList();
        }

        protected override void Duplicate()
        {
            var ids = Grid.DataKeys.OfType<DataKey>().Select(k => Convert.ToInt32(k.Value)).ToList();
            Session.Add("MessagesIds", ids);
            Session.Add("CurrentCompany", cbLinea.Selected);
            Response.Redirect("EstadoLogDuplicar.aspx",false);
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            return data;
        }
    }
}
