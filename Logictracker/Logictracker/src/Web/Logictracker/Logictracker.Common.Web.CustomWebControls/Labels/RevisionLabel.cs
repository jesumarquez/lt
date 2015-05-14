#region Usings

using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI.WebControls;
using Logictracker.Security;

#endregion

namespace Logictracker.Web.CustomWebControls.Labels
{
    public class RevisionLabel: Label
    {
        /// <summary>
        /// Secures the control
        /// </summary>
        [Category("Custom Resources")]
        public string SecureRefference
        {
            get { return ViewState["SecureRefference"] != null ? ViewState["SecureRefference"].ToString() : string.Empty; }
            set { ViewState["SecureRefference"] = value; }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
        }

        public override string Text
        {
            get
            {
                var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Logictracker.Web.CustomWebControls.Labels.version.txt");

                if (stream == null) return "0";

                var buffer = new byte[stream.Length];

                stream.Read(buffer, 0, (int)stream.Length);
                stream.Close();

                var rev = Encoding.UTF8.GetString(buffer);

                return base.Text.Contains("{0}") ? string.Format(base.Text, rev) : rev;
            }
            set { base.Text = value; }
        }
    }
}
