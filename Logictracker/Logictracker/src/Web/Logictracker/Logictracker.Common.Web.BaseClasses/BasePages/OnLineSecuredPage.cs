namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Base page that implements all the necesary security for online mode pages.
    /// </summary>
    public abstract class OnLineSecuredPage : ApplicationSecuredPage
    {
        #region Protected Methods

        /// <summary>
        /// Asks the user to re-login due to a session loss.
        /// </summary>
        protected override void OnSessionLoss()
        {
            if(!IsPostBack) base.OnSessionLoss();
            else OpenWin(string.Concat(NotLoguedUrl, "&cal=1"), "Login", 400, 600);
        }

        #endregion
    }
}