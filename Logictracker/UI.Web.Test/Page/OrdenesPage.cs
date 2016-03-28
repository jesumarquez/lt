using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;

namespace UI.Web.Test.Page
{
    internal class OrdenesPage : PageModel
    {
        private const string UrlOrdenes = "http://localhost:9980/Ordenes/Programar";
        private static readonly By ModalSelector = By.Id("myModal");
        private static readonly By BuscarSelector = By.Name("buscar");
        private static readonly By AsignarSelector = By.Name("asignar");

        public OrdenesPage(IWebDriver webDriver)
            : base(webDriver, ModalSelector, UrlOrdenes)
        {
        }

        public void ExisteFiltro(string filtro)
        {
            Assert.IsTrue(WebDriver.FindElement(By.Name("filter-" + filtro.ToLower())).TagName == "input",
                string.Format("Filtro {0} no encontrado", filtro));
        }

        public void ExisteColumna(string nombreColumna)
        {
            var selector = By.CssSelector(string.Format("th[data-title='{0}']", nombreColumna));


            Assert.IsTrue(FindElement(selector, TimeSpan.FromSeconds(30)).Displayed,
                string.Format("Columna {0} no encontrada", nombreColumna));
        }

        public void Buscar()
        {
            BuscarBtn.Click();
        }

        private IWebElement BuscarBtn
        {
            get { return WebDriver.FindElement(BuscarSelector); }
        }


        internal void SetFiltro(string filtro, string valor)
        {
            var selector = By.Name("filter-" + filtro.ToLower());

            var elemn = WebDriver.FindElement(selector);

            //elemn.Click();
           
            elemn.SendKeys(valor);
        }

        private IWebElement FindElement(By selector, TimeSpan timeout)
        {
            var wait = new WebDriverWait(WebDriver, timeout);

            var element = wait.Until(driver => driver.FindElement(selector));

            return element;
        }

        public static IWebElement GetParent(IWebElement e)
        {
            return e.FindElement(By.XPath(".."));
        }
    }
}