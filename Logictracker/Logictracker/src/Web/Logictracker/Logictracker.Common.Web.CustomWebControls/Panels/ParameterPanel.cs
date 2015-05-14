#region Usings

using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.Helpers.ExtensionMethods;

#endregion

namespace Logictracker.Web.CustomWebControls.Panels
{
    [Themeable(true)]
    public class ParameterPanel : Panel
    {
        public delegate void DispatchCommandHandler(string cmd);

        public event DispatchCommandHandler DispatchCommand;

        /**protected override void Render(HtmlTextWriter writer)
        {
            if (!Width.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
            //if (!HasOverflow && !Height.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            //writer.RenderBeginTag(HtmlTextWriterTag.Table);

            //writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, TitleCssClass);
            //writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(!string.IsNullOrEmpty(TitleResourceName) ? CultureManager.GetString(TitleResourceName, TitleVariableName): Title);
            //writer.RenderEndTag();
            //writer.RenderEndTag();

            //writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            //writer.AddAttribute(HtmlTextWriterAttribute.Class, BodyCssClass);
            //writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
            //writer.RenderBeginTag(HtmlTextWriterTag.Td);

            
            //writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, overflowx);
            //writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, overflowy);

            //if (HasOverflow && !Height.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            //writer.RenderBeginTag(HtmlTextWriterTag.Div);
            foreach (Control control in Controls)
                control.RenderControl(writer);

            //writer.RenderEndTag();

            //writer.RenderEndTag();
            //writer.RenderEndTag();

            writer.RenderEndTag();
        }*/

        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public DetalleDispositivo DetalleDispositivo
        {
            get { return (DetalleDispositivo) ViewState["DetalleDispositivo"]; }
            set
            {
               ViewState["DetalleDispositivo"] = value;
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if (DetalleDispositivo == null) return;
            var md = WebExtentions.ParseMetadata(DetalleDispositivo.TipoParametro.Metadata);
            var ctrl = WebExtentions.CreateWebControl(DetalleDispositivo.TipoParametro.Nombre,
                DetalleDispositivo.TipoParametro.Metadata, DetalleDispositivo.Valor);
            var buttonText = md["action_text"] ?? DetalleDispositivo.TipoParametro.Descripcion;
            Controls.Add(ctrl);
            var doit = new Button
                           {
                ID = DetalleDispositivo.TipoParametro.Nombre + "doIt",
                Text = buttonText
            };
            doit.Click += EventCallback;
            Controls.Add(doit);
        }

        protected void EventCallback(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var md = WebExtentions.ParseMetadata(DetalleDispositivo.TipoParametro.Metadata);
                var cmd = md["command"];
                cmd = cmd.Replace("$ID", DetalleDispositivo.Dispositivo.Id.ToString());
                cmd = cmd.Replace("$VALUE", DetalleDispositivo.Valor);
                cmd = cmd.Replace("$TAG", md["tag"] ?? string.Empty);
                if (DispatchCommand != null)
                    DispatchCommand(cmd);
            }
            //Content.Text = "Content Changed.";
        }
    }
}
