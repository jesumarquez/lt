using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using UI.Web.Test.Page;

namespace UI.Web.Test.Specs
{
    [Binding]
    public class BaseStep
    {
        private const string CurrentPageKey = "Current.Page";

        public static IWebDriver Driver { get; set; }

        protected P GetCurrentPage<P>() where P : PageModel
        {
            return (P)ScenarioContext.Current[CurrentPageKey];
        }

        protected void SetCurrentPage(PageModel page)
        {
            ScenarioContext.Current[CurrentPageKey] = page;
        }
        
        [BeforeScenario]
        public static void StartWebDriverForReal()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 5));
        }

        [AfterScenario]
        public static void killBrowser()
        {
            Driver.Close();
        }
        
        [AfterTestRun]
        public static void TerminateWebDriver()
        {
            // Never Triggered due to Defect https://github.com/techtalk/SpecFlow/issues/#issue/26
            Driver.Close();
        }
    }
}
