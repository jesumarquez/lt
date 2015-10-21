using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.ControlAcceso;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.ControlAcceso
{
    public partial class CategoriaAccesoLista : SecuredListPage<CategoriaAccesoVo>
    {
        protected override string RedirectUrl { get { return "CategoriaAccesoAlta.aspx"; } }
        protected override string VariableName { get { return "AC_CATEGORIA"; } }
        protected override string GetRefference() { return "AC_CATEGORIA"; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override List<CategoriaAccesoVo> GetListData()
        {
            return DAOFactory.CategoriaAccesoDAO.FindList(cbEmpresa.SelectedValues, cbLinea.SelectedValues)
                .Select(x => new CategoriaAccesoVo(x))
                .ToList();
        }
    }
}