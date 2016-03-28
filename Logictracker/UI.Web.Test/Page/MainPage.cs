using OpenQA.Selenium;

namespace UI.Web.Test.Page
{
    internal class MainPage : PageModel
    {
        private static readonly By LogoSelector = By.ClassName("Logo");

        public MainPage(IWebDriver webDriver)
            : base(webDriver, LogoSelector)
        {
            if (Title.Contains("Login"))
            {

            }
        }

        public MainPage(IWebDriver webDriver, string userName, string password, string perfil)
            : base(webDriver, LogoSelector)
        {
            if (!Title.Contains("Login")) return;

            var loginPage = new LoginPage(WebDriver);
            loginPage.LoginAs(userName, password).UsePerfil(perfil);
        }


        public OrdenesPage Ordenes()
        {
            return new OrdenesPage(WebDriver);
        }
    }
}