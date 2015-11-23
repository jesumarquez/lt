#region Usings

using System;
using Logictracker.Web.BaseClasses.BaseControls;

#endregion

namespace Logictracker.App_Controls.Pickers
{
    public partial class App_Controls_NumberPicker : BaseUserControl
    {
        #region Public Properties

        /// <summary>
        /// The mask used to validate the number value.
        /// </summary>
        public string Mask
        {
            set
            {
                meeInterval.Mask = value;
                mevInterval.TooltipMessage = string.Format("Formato ({0})", value);
            }
        }

        /// <summary>
        /// Maximun value used to validate the number value.
        /// </summary>
        public string MaximumValue
        {
            set
            {
                mevInterval.MaximumValue = value;
                mevInterval.MaximumValueMessage = string.Format("Valor máximo es {0}", value);  
            }
            get { return mevInterval.MaximumValue; }
        }

        /// <summary>
        /// Value of TextBox validated.
        /// </summary>
        public int Number
        {
            get { return string.IsNullOrEmpty(txtInterval.Text) ? int.MinValue : Convert.ToInt32(txtInterval.Text); } 
            set { txtInterval.Text = value.ToString(); }
        }

        /// <summary>
        /// Enables or disables the associated textbox.
        /// </summary>
        public bool Enabled
        {
            get { return txtInterval.Enabled; }
            set { txtInterval.Enabled = value; }
        }

        /// <summary>
        /// Textbox width
        /// </summary>
        public int Width { set { txtInterval.Width = value; } }

        /// <summary>
        /// Determines wither if the controls accepts or not null values.
        /// </summary>
        public bool IsValidEmpty { set { mevInterval.IsValidEmpty = value; } }

        #endregion

        #region Private Event Handlers

        /// <summary>
        /// The event for a change in the text field
        /// </summary>
        private event EventHandler textHasChanged;

        #endregion

        #region

        /// <summary>
        /// Sets the text changed event handler.
        /// </summary>
        public event EventHandler TextHasChanged
        {
            add
            {
                textHasChanged = (EventHandler)Delegate.Combine(textHasChanged, value);
                txtInterval.AutoPostBack = true;
            }
            remove
            {
                if (textHasChanged != null) textHasChanged = (EventHandler)Delegate.Remove(textHasChanged, value);
                txtInterval.AutoPostBack = textHasChanged.GetInvocationList().Length > 0;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Activates the event OnTextChanged in the NumberPicker control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextChanged(object sender, EventArgs e) { if (textHasChanged != null) textHasChanged(this, EventArgs.Empty); }

        #endregion
    }
}
