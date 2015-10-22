using System;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion.Camaras
{
    public partial class Operacion_Camaras_Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var camId = Request.QueryString["i"];
                if (camId == null) return;
                var camera = Config.Camera.Config.Camera.Where(c => c.Id == camId).FirstOrDefault();
                if (camera == null) return;
                Title = camera.Title;
                litBaseUrl.Text = camera.BaseUrl;
                litUrl.Text = camera.Url;
                litWidth.Text = camera.Width.ToString();
                litHeight.Text = camera.Height.ToString(); 
            }
        }

        #region Overrides of BasePage

        protected override InfoLabel LblInfo
        {
            get { return null; }
        }

        #endregion
    }
}
