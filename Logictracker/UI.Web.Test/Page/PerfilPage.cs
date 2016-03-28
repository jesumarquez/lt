using OpenQA.Selenium;

namespace UI.Web.Test.Page
{
    class PerfilPage : PageModel
    {
        private static readonly By PerfilesSelector = By.Id("cbPerfiles");
        private static readonly By SeleccionarSelector = By.Id("btSelPerfil");

        public PerfilPage(IWebDriver webDriver)
            : base(webDriver, PerfilesSelector)
        {
        }

        public MainPage UsePerfil(string perfil = "Todos")
        {
            Perfil.SendKeys(perfil);
            Seleccionar.Click();
            return new MainPage(WebDriver);
        }

        public IWebElement Perfil { get { return WebDriver.FindElement(PerfilesSelector); } }

        public IWebElement Seleccionar { get { return WebDriver.FindElement(SeleccionarSelector); } }
    }
}
