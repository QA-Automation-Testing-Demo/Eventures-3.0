using System;
using System.Diagnostics;

using Eventures.Tests.Common;

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

    //  Add headless and temp profile for GitHub Actions
    chromeOptions.AddArgument("--headless=new");
    chromeOptions.AddArgument("--no-sandbox");
    chromeOptions.AddArgument("--disable-dev-shm-usage");

    // Create a unique temp directory for user profile
    var tempProfileDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    chromeOptions.AddArguments($"--user-data-dir={tempProfileDir}");

    this.driver = new ChromeDriver(chromeOptions);

    // Set an implicit wait for the UI interaction
    this.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
}


        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            // Stop and dispose the Selenium driver
            driver.Quit();

            // Stop and dispose the local Web server
            this.testEventuresApp.Dispose();
        }
    }
}
