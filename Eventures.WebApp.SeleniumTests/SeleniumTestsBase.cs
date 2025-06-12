using System;
using System.Diagnostics;
using Eventures.Tests.Common;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Eventures.WebApp.SeleniumTests
{
    public abstract class SeleniumTestsBase
    {
        protected TestDb testDb;
        protected IWebDriver driver;
        protected TestEventuresApp<Startup> testEventuresApp;
        protected string baseUrl;
        protected string username = "user" + DateTime.Now.Ticks.ToString().Substring(10);
        protected string password = "pass" + DateTime.Now.Ticks.ToString().Substring(10);

        [OneTimeSetUp]
        public void OneTimeSetupBase()
        {
            // Run the Web app in a local Web server
            this.testDb = new TestDb();
            this.testEventuresApp = new TestEventuresApp<Startup>(
                testDb, "../../../../Eventures.WebApp");
            this.baseUrl = this.testEventuresApp.ServerUri;

            // Setup the ChromeDriver
            var chromeOptions = new ChromeOptions();

            // Enable headless in CI (and optionally in local)
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArguments("--no-sandbox");
            chromeOptions.AddArguments("--disable-gpu");
            chromeOptions.AddArguments("--window-size=1920,1080");

            this.driver = new ChromeDriver(chromeOptions);

            // Implicit wait for finding elements
            this.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Waits for the current URL to contain the given fragment (case-insensitive).
        /// </summary>
        protected void WaitUntilUrlContains(string fragment, int seconds = 10)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
                .Until(d => d.Url.ToLower().Contains(fragment.ToLower()));
        }

        /// <summary>
        /// Waits for the current page title to contain the given text.
        /// </summary>
        protected void WaitUntilTitleContains(string titlePart, int seconds = 5)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
                .Until(d => d.Title.Contains(titlePart));
        }

        /// <summary>
        /// Takes a screenshot with a given name (saves in the working directory).
        /// </summary>
        protected void TakeScreenshot(string name)
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile($"screenshot-{name}.png", ScreenshotImageFormat.Png);
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            driver.Quit();
            this.testEventuresApp.Dispose();
        }
    }
}
