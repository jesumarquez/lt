using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.Organizacion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.DAL.DAO.BaseClasses;

namespace Logictracker.Organizacion
{
    public partial class ListaPerfil : SecuredListPage<PerfilVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "SOC_PERFILES"; } }
        protected override string RedirectUrl { get { return "PerfilAlta.aspx"; } }
        protected override string GetRefference() { return "PERFIL"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<PerfilVo> GetListData()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var list = user.Perfiles.IsEmpty() ? DAOFactory.PerfilDAO.FindAll() : user.Perfiles.Where(p=> p.FechaBaja == null);
            return list.Select(p => new PerfilVo(p)).ToList();
        }

        #endregion

    }
}