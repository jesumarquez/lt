using System;
using OpenQA.Selenium;

namespace UI.Web.Test.Page
{
    public class PageModel
    {
        public IWebDriver WebDriver { get; private set; }
        public string Title { get { return WebDriver.Title; } }

        protected PageModel(IWebDriver webDriver, By knownElementOnPage, String loadUrl = "")
        {
            WebDriver = webDriver;
            if (!string.IsNullOrEmpty(loadUrl))
            {
                webDriver.Navigate().GoToUrl(loadUrl);
            }
            
            FingKnownElementOnPage(knownElementOnPage);
        }
        
        private void FingKnownElementOnPage(By knownElementOnPage)
        {
            WebDriver.FindElement(knownElementOnPage);
        }
    }
}
