using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    /// <summary>
    /// Decorates .net DropDownLists with custom employee related behaivour.
    /// </summary>
    public class EmpleadoDropDownListWrapper : DropDownListBaseWrapper<Empleado>, IEmpleadoAutoBindeable
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new employee DropDownLists wrapper.
        /// </summary>
        /// <param name="empleadoDropDownList"></param>
        /// <param name="soloResponsables"></param>
        /// <param name="addSinEmpleado"></param>
        public EmpleadoDropDownListWrapper(DropDownList empleadoDropDownList, bool soloResponsables, bool addSinEmpleado) : base(empleadoDropDownList)
        {
            AddNoneItem = addSinEmpleado;
            SoloResponsables = soloResponsables;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Detemrines if only those employees in charge of othre employees should be binded.
        /// </summary>
        public bool SoloResponsables { get; set; }

        /// <summary>
        /// Determines if it will filter the results based only in district values.
        /// </summary>
        public bool AllowOnlyDistrictBinding { get; set; }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_EMPLOYEE"); } }

        #endregion
    }
}