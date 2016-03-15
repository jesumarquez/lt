using OpenQA.Selenium;

namespace UI.Web.Test.Page
{
    class LoginPage : PageModel
    {
        private const string pageUrl = "http://localhost:9980/Default.aspx";
        private static readonly By UsuarioSelector = By.Id("txtUsuario");
        private static readonly By PasswordSelector = By.Id("txtPassword");
        private static readonly By LoginSelector = By.Id("btLogin");

        public  LoginPage(IWebDriver webDriver)
            : base(webDriver, UsuarioSelector, pageUrl)
        {

        }


        public PerfilPage LoginAs(string user, string password)
        {
            Usuario.SendKeys(user);
            Password.SendKeys(password);
            Login.Click();
            return new PerfilPage(WebDriver);
        }

        public IWebElement Login { get { return WebDriver.FindElement(LoginSelector); } }

        public IWebElement Password { get { return WebDriver.FindElement(PasswordSelector); } }

        public IWebElement Usuario  { get { return WebDriver.FindElement(UsuarioSelector); }}
    }
}
