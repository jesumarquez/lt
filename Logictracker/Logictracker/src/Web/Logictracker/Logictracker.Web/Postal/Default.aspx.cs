using System;
using Logictracker.Types.BusinessObjects.Postal;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Postal
{
    public partial class Postal_Default : BasePage
    {
        public Ruta Route; 
        protected void Page_Load(object sender, EventArgs e)
        {
            var pieza = Request.QueryString["i"];
            if (pieza == null) return;

            Route = DAOFactory.RutaDAO.GetRouteByPieza(pieza);
            if (Route == null) return;
        }

        #region Overrides of BasePage

        protected override InfoLabel LblInfo { get { return null; } }

        #endregion
    }
}
