#region Usings

using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.Wrappers.DropDownLists.BussinessObjects
{
    /// <summary>
    /// .Net drop down list custom wrapper for adding messages related functionality.
    /// </summary>
    public class MensajeDropDownListWrapper: DropDownListBaseWrapper<Mensaje>, IMensajeAutoBindeable
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new messages DropDownLists wrapper.
        /// </summary>
        /// <param name="dropDownList"></param>
        public MensajeDropDownListWrapper(DropDownList dropDownList) : base(dropDownList) { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines if only the maintenance messages should be displayed.
        /// </summary>
        public bool SoloMantenimiento { get; set; }

        /// <summary>
        /// Determines if only the fuel messages should be displayed.
        /// </summary>
        public bool SoloCombustible { get; set; }

        /// <summary>
        /// Determines if the no message option should be displayed.
        /// </summary>
        public bool AddSinMensaje { get; set; }

        public bool SoloAtencion { get; set; }

        public bool BindIds { get; set; }

        #endregion
    }
}
