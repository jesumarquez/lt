using System;
using System.Web.UI;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_ColorPicker : UserControl
    {
        protected string JsSelectedColorVariable { get { return ClientID + "selectedColor"; } }
        protected string JsChangeFunction { get { return ClientID + "change"; } }
        public event EventHandler ColorChanged;

        protected void OnColorChanged(EventArgs e)
        {
            EventHandler handler = ColorChanged;
            if (handler != null) handler(this, e);
        }

        public bool AutoPostback
        {
            get { return tbColor.AutoPostBack; }
            set { tbColor.AutoPostBack = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            cpe.OnClientColorSelectionChanged = JsChangeFunction;
        }

        public string Color
        {
            get{ return tbColor.Text;}
            set { tbColor.Text = value; }
        }
        protected void tbColor_TextChanged(object sender, EventArgs e)
        {
            OnColorChanged(e);
        }
    }
}
