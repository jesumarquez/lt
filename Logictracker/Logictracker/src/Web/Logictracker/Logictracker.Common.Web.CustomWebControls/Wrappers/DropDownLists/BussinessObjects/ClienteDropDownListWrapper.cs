#region Usings

using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    public class ClienteDropDownListWrapper: DropDownListBaseWrapper<Cliente>
    {
        #region Constructors

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="addNoneItem"></param>
        public ClienteDropDownListWrapper(DropDownList ddl, bool addNoneItem) : base(ddl) { AddNoneItem = addNoneItem; }

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        public ClienteDropDownListWrapper(DropDownList ddl) : base(ddl) { AddNoneItem = false; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines wither to add the none option.
        /// </summary>
        public new bool AddNoneItem { get; set; }

        #endregion
    }
}
