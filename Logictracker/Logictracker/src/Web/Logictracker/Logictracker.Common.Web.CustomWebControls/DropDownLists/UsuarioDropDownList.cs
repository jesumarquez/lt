#region Usings

using System;
using System.Web.UI;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:UsuariosDropDownList runat=\"server\"></{0}:UsuariosDropDownList>")]
    public class UsuarioDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(Usuario); } }

        #endregion

        #region Protected Methods

        protected override void Bind()
        {
            var dao = new DAOFactory();

            BindingManager.BindUsuarios(this, dao.UsuarioDAO.FindById(Usuario.Id));
        }

        #endregion
    }
}
