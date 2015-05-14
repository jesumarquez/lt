using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    public class CentroDeCostoDropDownListWrapper: DropDownListBaseWrapper<CentroDeCostos>
    {
        #region Constructors

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="addNoneItem"></param>
        public CentroDeCostoDropDownListWrapper(DropDownList ddl, bool addNoneItem) : base(ddl) { AddNoneItem = addNoneItem; }

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        public CentroDeCostoDropDownListWrapper(DropDownList ddl) : base(ddl) { AddNoneItem = false; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines wither to add the none option.
        /// </summary>
        public new bool AddNoneItem { get; set; }

        #endregion
    }
}
