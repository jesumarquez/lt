using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    /// <summary>
    /// Wrapper for adding transport companies logic to a base .net drop down list.
    /// </summary>
    public class TransportistaDropDownListWrapper : DropDownListBaseWrapper<Transportista>
    {
        #region Constructors

        /// <summary>
        /// Generates a new transportista drop down list wrapper.
        /// </summary>
        /// <param name="dropDownList"></param>
        /// <param name="addNoneItem"></param>
        public TransportistaDropDownListWrapper(DropDownList dropDownList, bool addNoneItem) : base(dropDownList) { AddNoneItem = addNoneItem; }

        #endregion

        public new virtual string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_TRANSPORTISTA"); }
        }
    }
}
