#region Usings

using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    public class MovilDropDownListWrapper : DropDownListBaseWrapper<Coche>, IMovilAutoBindeable
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new employee DropDownLists wrapper.
        /// </summary>
        /// <param name="movilDropDownList"></param>
        /// <param name="showDriverName"></param>
        public MovilDropDownListWrapper(DropDownList movilDropDownList, bool showDriverName) : base(movilDropDownList) { ShowDriverName = showDriverName; }

        #endregion

        #region Public Properties

        public bool ShowDriverName { get; set; }
        public bool ShowOnlyAccessControl { get; set; }
        public bool HideWithNoDevice { get; set; }
        public bool HideInactive { get; set; }

        public MovilOptionGroupValue OptionGroupProperty { get; set; }

        public int Coche { get; set; }

        #endregion
    }
}