using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Eventures.WebApp.SeleniumTests
{
    public class SeleniumTestsEvents : SeleniumTestsBase
    {
        [OneTimeSetUp]
        public void SetupUser()
        {
            RegisterUserForTesting();
        }

        [Test]
        public void Test_HomePage_AllEventsLink()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            var allEventsPageLink = driver.FindElement(By.XPath("//a[@href='/Events/All'][contains(.,'all events')]"));
            allEventsPageLink.Click();

            WaitUntilUrlContains("/Events/All");
            WaitUntilTitleContains("All Events");

            Assert.That(driver.PageSource.Contains("<h1>All Events</h1>"));
            Assert.That(driver.PageSource.Contains(@"<a href=""/Events/Create"">Create New</a>"));
        }

        [Test]
        public void Test_AllEventsPage_ThroughNavigation()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.Id("dropdownMenuLink")).Click();
            var allEventsLinkItem = driver.FindElements(By.XPath("//a[@class='dropdown-item']"))[0];
            allEventsLinkItem.Click();

            WaitUntilUrlContains("/Events/All");
            WaitUntilTitleContains("All Events");

            Assert.That(driver.PageSource.Contains("<h1>All Events</h1>"));
            Assert.That(driver.PageSource.Contains(@"<a href=""/Events/Create"">Create New</a>"));
        }

        [Test]
        public void Test_HomePage_CreateEventPageLink()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            var createEventPageLink = driver.FindElement(By.XPath("//a[@href='/Events/Create'][contains(.,'new event')]"));
            createEventPageLink.Click();

            WaitUntilUrlContains("/Events/Create");
            WaitUntilTitleContains("Create Event");

            Assert.That(driver.PageSource.Contains("<h1>Create New Event</h1>"));
            Assert.That(driver.PageSource.Contains(@"<a class=""btn btn-secondary"" href=""/Events/All"">Back to List</a>"));
        }

        [Test]
        public void Test_CreateEventPage_ThroughNavigation()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.Id("dropdownMenuLink")).Click();
            var createEventLinkItem = driver.FindElements(By.XPath("//a[@class='dropdown-item']"))[1];
            createEventLinkItem.Click();

            WaitUntilUrlContains("/Events/Create");
            WaitUntilTitleContains("Create Event");

            Assert.That(driver.PageSource.Contains("<h1>Create New Event</h1>"));
            Assert.That(driver.PageSource.Contains(@"<a class=""btn btn-secondary"" href=""/Events/All"">Back to List</a>"));
        }

        [Test]
        public void Test_CreateEventPage_BackToListLink()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Events/Create");
            WaitUntilTitleContains("Create Event");

            driver.FindElement(By.XPath("//a[@href='/Events/All'][contains(.,'Back to List')]")).Click();

            WaitUntilUrlContains("/Events/All");
            WaitUntilTitleContains("All Events");

            Assert.That(driver.PageSource.Contains("<h1>All Events</h1>"));
        }

        [Test]
        public void Test_CreateEventPage_CreateEvent_ValidData()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Events/Create");
            WaitUntilTitleContains("Create Event");

           var eventName = "Party" + DateTime.Now.Ticks;
    driver.FindElement(By.Id("Name")).SendKeys(eventName);
    driver.FindElement(By.Id("Place")).SendKeys("Beach");
    driver.FindElement(By.Id("Start")).SendKeys("2026-01-01");
    driver.FindElement(By.Id("End")).SendKeys("2026-01-02");
    driver.FindElement(By.Id("TotalTickets")).SendKeys("100");
    driver.FindElement(By.Id("PricePerTicket")).SendKeys("10.00");


            driver.FindElement(By.XPath("//input[contains(@value,'Create')]")).Click();

            WaitUntilUrlContains("/Events/All");

            Assert.That(driver.PageSource.Contains(eventName));
            Assert.That(driver.PageSource.Contains("Beach"));
            Assert.That(driver.PageSource.Contains(username));

            var eventRow = driver.FindElements(By.TagName("tr")).FirstOrDefault(e => e.Text.Contains(eventName));
            Assert.That(eventRow, Is.Not.Null);
            Assert.That(eventRow.Text.Contains("Delete"));
            Assert.That(eventRow.Text.Contains("Edit"));
        }

        [Test]
        public void Test_CreateEventPage_CreateEvent_InvalidData()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Events/Create");
            WaitUntilTitleContains("Create Event");

            var nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            nameField.SendKeys(string.Empty);

            driver.FindElement(By.XPath("//input[contains(@value,'Create')]")).Click();

            WaitUntilUrlContains("/Events/Create");

            var errorSpan = driver.FindElement(By.Id("Name-error"));
            Assert.That(errorSpan.Text.Equals("The Name field is required."));
        }

        [Test]
        public void Test_DeleteEvent()
        {
            string eventName = CreateEvent();

            WaitUntilUrlContains("/Events/All");
            Assert.That(driver.PageSource.Contains(eventName));

            var deleteBtn = driver.FindElement(
                By.XPath($"//tr//td[contains(text(), '{eventName}')]/..//a[contains(.,'Delete')]"));
            deleteBtn.Click();

            WaitUntilUrlContains("/Events/Delete/");
            WaitUntilTitleContains("Delete Event");

            Assert.That(driver.PageSource.Contains(eventName));

            driver.FindElement(By.XPath("//input[contains(@value,'Delete')]")).Click();

            WaitUntilUrlContains("/Events/All");

            Assert.That(!driver.PageSource.Contains(eventName));
        }

        [Test]
        public void Test_EditEvent_ValidData()
        {
            string eventName = CreateEvent();

            WaitUntilUrlContains("/Events/All");
            Assert.That(driver.PageSource.Contains(eventName));

            var editButton = driver.FindElement(
                By.XPath($"//tr//td[contains(text(), '{eventName}')]/..//a[contains(.,'Edit')]"));
            editButton.Click();

            WaitUntilUrlContains("/Events/Edit/");
            WaitUntilTitleContains("Edit Event");

            var changedName = "Best Best Show" + DateTime.Now.Ticks;
            var editNameField = driver.FindElement(By.Id("Name"));
            editNameField.Clear();
            editNameField.SendKeys(changedName);

            driver.FindElement(By.XPath("//input[contains(@value,'Edit')]")).Click();

            WaitUntilUrlContains("/Events/All");

            Assert.That(driver.PageSource.Contains(changedName));
            Assert.That(!driver.PageSource.Contains(eventName));
        }

        [Test]
        public void Test_EditEvent_InvalidData()
        {
            string eventName = CreateEvent();

            WaitUntilUrlContains("/Events/All");
            Assert.That(driver.PageSource.Contains(eventName));

            var editButton = driver.FindElement(
                By.XPath($"//tr//td[contains(text(), '{eventName}')]/..//a[contains(.,'Edit')]"));
            editButton.Click();

            WaitUntilUrlContains("/Events/Edit/");
            WaitUntilTitleContains("Edit Event");

            var editNameField = driver.FindElement(By.Id("Name"));
            editNameField.Clear();
            editNameField.SendKeys(string.Empty);

            driver.FindElement(By.XPath("//input[contains(@value,'Edit')]")).Click();

            WaitUntilUrlContains("/Events/Edit/");

            var errorSpan = driver.FindElement(By.Id("Name-error"));
            Assert.That(errorSpan.Text.Equals("The Name field is required."));
        }

        private void RegisterUserForTesting()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Identity/Account/Register");

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

        private string CreateEvent()
        {
            driver.Navigate().GoToUrl(this.baseUrl + "/Events/Create");
            WaitUntilTitleContains("Create Event");

            var eventName = "Best Show" + DateTime.Now.Ticks;
            driver.FindElement(By.Id("Name")).SendKeys(eventName);

            driver.FindElement(By.XPath("//input[contains(@value,'Create')]")).Click();

            WaitUntilUrlContains("/Events/All");

            return eventName;
        }
    }
}
