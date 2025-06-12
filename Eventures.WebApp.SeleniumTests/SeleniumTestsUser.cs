using NUnit.Framework;
using OpenQA.Selenium;

namespace Eventures.WebApp.SeleniumTests
{
    public class SeleniumTestsUser : SeleniumTestsBase
    {
        [Test, Order(1)]
        public void Test_User_Register()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Identity/Account/Register");
            WaitUntilTitleContains("Register");

            driver.FindElement(By.Id("Input_Username")).SendKeys(username);
            driver.FindElement(By.Id("Input_Email")).SendKeys($"{username}@mail.com");
            driver.FindElement(By.Id("Input_Password")).SendKeys(password);
            driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);
            driver.FindElement(By.Id("Input_FirstName")).SendKeys("Pesho");
            driver.FindElement(By.Id("Input_LastName")).SendKeys("Petrov");

            driver.FindElement(By.XPath("//button[@type='submit'][contains(.,'Register')]")).Click();

            WaitUntilUrlContains("/");
            Assert.That(driver.Url.Equals(this.baseUrl + "/"));
            Assert.That(driver.PageSource.Contains($"Welcome, {username}"));
        }

        [Test, Order(2)]
        public void Test_User_Login()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Identity/Account/Login");
            WaitUntilTitleContains("Log in");

            driver.FindElement(By.Id("Input_Username")).SendKeys(username);
            driver.FindElement(By.Id("Input_Password")).SendKeys(password);

            driver.FindElement(By.XPath("//button[@type='submit'][contains(.,'Log in')]")).Click();

            WaitUntilUrlContains("/");
            Assert.That(driver.Url.Equals(this.baseUrl + "/"));
            Assert.That(driver.PageSource.Contains($"Welcome, {username}"));
        }

        [Test, Order(3)]
        public void Test_User_Logout()
        {
            driver.Navigate().GoToUrl(this.baseUrl);

            driver.FindElement(By.XPath("//button[@type='submit'][contains(.,'Logout')]")).Click();

            WaitUntilUrlContains("/");
            Assert.That(driver.Url.Equals(this.baseUrl + "/"));
            Assert.That(driver.PageSource.Contains("Eventures: Events and Tickets"));
        }

        [Test]
        public void Test_HomePage_LoginPageLink_InNavigation()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.XPath("(//a[@href='/Identity/Account/Login'])[1]")).Click();

            WaitUntilUrlContains("/Identity/Account/Login");
            WaitUntilTitleContains("Log in");

            Assert.That(driver.PageSource.Contains("Log in"));
            Assert.That(driver.PageSource.Contains("Use a local account to log in"));
        }

        [Test]
        public void Test_HomePage_LoginPageLink_OnPage()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.XPath("(//a[@href='/Identity/Account/Login'])[2]")).Click();

            WaitUntilUrlContains("/Identity/Account/Login");
            WaitUntilTitleContains("Log in");

            Assert.That(driver.PageSource.Contains("Log in"));
            Assert.That(driver.PageSource.Contains("Use a local account to log in"));
        }

        [Test]
        public void Test_HomePage_RegisterPageLink_InNavigation()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.XPath("(//a[@href='/Identity/Account/Register'])[1]")).Click();

            WaitUntilUrlContains("/Identity/Account/Register");
            WaitUntilTitleContains("Register");

            Assert.That(driver.PageSource.Contains("Register"));
            Assert.That(driver.PageSource.Contains("Create a new account"));
        }

        [Test]
        public void Test_HomePage_RegisterPageLink_OnPage()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.XPath("(//a[@href='/Identity/Account/Register'])[2]")).Click();

            WaitUntilUrlContains("/Identity/Account/Register");
            WaitUntilTitleContains("Register");

            Assert.That(driver.PageSource.Contains("Register"));
            Assert.That(driver.PageSource.Contains("Create a new account"));
        }

        [Test]
        public void Test_AllEventsPage_Anonymous()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Events/All");

            WaitUntilUrlContains("/Identity/Account/Login");
            WaitUntilTitleContains("Log in");

            Assert.That(driver.PageSource.Contains("Log in"));
            Assert.That(driver.PageSource.Contains("Use a local account to log in"));
        }
    }
}
