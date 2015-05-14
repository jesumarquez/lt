#region Usings

using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

#endregion

namespace Logictracker.WindowsServices
{
    /// <summary>
    /// Class that represents a win service failure recovery action.
    /// </summary>
    [FrameworkElement(XName = "FailureAction", IsContainer = false)]
    public class FailureAction : FrameworkElement
    {
		#region Attributes

		/// <summary>
        /// The configured recovery action type.
        /// </summary>
        [ElementAttribute(XName = "RecoverAction", IsSmartProperty = true, IsRequired = true)]
        public RecoverAction RecoverAction
        {
            get { return (RecoverAction)GetValue("RecoverAction"); }
            set { SetValue("RecoverAction", value); }
        }

        /// <summary>
        /// Delay before applying the specified action.
        /// </summary>
        [ElementAttribute(XName = "Delay", IsSmartProperty = true, IsRequired = false, DefaultValue = 60)]
        public int Delay
        {
            get { return (int)GetValue("Delay"); }
            set { SetValue("Delay", value); }
        }

        /// <summary>
        /// Delay before applying the specified action.
        /// </summary>
        [ElementAttribute(XName = "Command", IsSmartProperty = true, IsRequired = false)]
        public int Command
        {
            get { return (int)GetValue("Command"); }
            set { SetValue("Command", value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new failure action.
        /// </summary>
        public FailureAction() { }

        /// <summary>
        /// Instanciates anew failure action with the specified action type and delay.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="actionDelay"></param>
        public FailureAction(RecoverAction actionType, int actionDelay)
        {
            RecoverAction = actionType;
            Delay = actionDelay;
        }

        #endregion
    }
}