using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    /// <summary>
    /// Decorates .net drop down lists with equipment specific behaivour.
    /// </summary>
    public class EquipoDropDownListWrapper : DropDownListBaseWrapper<Equipo>
    {
        #region Constructors

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="addNoneItem"></param>
        public EquipoDropDownListWrapper(DropDownList ddl, bool addNoneItem) : base(ddl) { AddNoneItem = addNoneItem; }

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        public EquipoDropDownListWrapper(DropDownList ddl) : base(ddl) { AddNoneItem = false; }

        #endregion

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_EQUIPMENT"); } }
    }
}
