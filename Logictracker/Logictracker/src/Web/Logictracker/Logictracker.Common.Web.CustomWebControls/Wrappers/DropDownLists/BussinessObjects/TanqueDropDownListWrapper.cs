#region Usings

using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    public class TanqueDropDownListWrapper: DropDownListBaseWrapper<Tanque>, ITanqueAutoBindeable
    {
        #region Constructors

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="addNoneItem"></param>
        public TanqueDropDownListWrapper(DropDownList ddl, bool addNoneItem) : base(ddl) { AddNoneItem = addNoneItem; }

        /// <summary>
        /// Decorates the givenn .net drop down list with the specified behaivour.
        /// </summary>
        /// <param name="ddl"></param>
        public TanqueDropDownListWrapper(DropDownList ddl) : base(ddl) { AddNoneItem = false; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines wither to add the none option.
        /// </summary>
        public new bool AddNoneItem { get; set; }

        #endregion

        public bool AllowEquipmentBinding
        {
            get { return GetParent<Equipo>() != null; }
        }

        public bool AllowBaseBinding
        {
            get { return GetParent<Linea>() != null; }
        }
    }
}
