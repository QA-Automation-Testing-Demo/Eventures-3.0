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
            var allEventsPageLink = driver.FindElement(
                By.XPath("//a[@href='/Events/All'][contains(.,'all events')]"));

            allEventsPageLink.Click();

            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.Title, Does.Contain("All Events"));
            Assert.That(driver.PageSource, Does.Contain("<h1>All Events</h1>"));
            Assert.That(driver.PageSource, Does.Contain("Create New"));
        }

        [Test]
        public void Test_AllEventsPage_ThroughNavigation()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.Id("dropdownMenuLink")).Click();
            var dropdownItems = driver.FindElements(By.XPath("//a[@class='dropdown-item']"));
            dropdownItems[0].Click();

            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.Title, Does.Contain("All Events"));
            Assert.That(driver.PageSource, Does.Contain("<h1>All Events</h1>"));
            Assert.That(driver.PageSource, Does.Contain("Create New"));
        }

        [Test]
        public void Test_HomePage_CreateEventPageLink()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            var createEventPageLink = driver.FindElement(
                By.XPath("//a[@href='/Events/Create'][contains(.,'new event')]"));

            createEventPageLink.Click();

            StringAssert.Contains("/Events/Create", driver.Url);
            Assert.That(driver.Title, Does.Contain("Create Event"));
            Assert.That(driver.PageSource, Does.Contain("<h1>Create New Event</h1>"));
            Assert.That(driver.PageSource, Does.Contain("Back to List"));
        }

        [Test]
        public void Test_CreateEventPage_ThroughNavigation()
        {
            driver.Navigate().GoToUrl(this.baseUrl);
            driver.FindElement(By.Id("dropdownMenuLink")).Click();
            var dropdownItems = driver.FindElements(By.XPath("//a[@class='dropdown-item']"));
            dropdownItems[1].Click();

            StringAssert.Contains("/Events/Create", driver.Url);
            Assert.That(driver.Title, Does.Contain("Create Event"));
            Assert.That(driver.PageSource, Does.Contain("<h1>Create New Event</h1>"));
            Assert.That(driver.PageSource, Does.Contain("Back to List"));
        }

        [Test]
        public void Test_CreateEventPage_BackToListLink()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Events/Create");
            Assert.That(driver.Title, Does.Contain("Create Event"));

            driver.FindElement(By.XPath("//a[@href='/Events/All'][contains(.,'Back to List')]")).Click();

            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.Title, Does.Contain("All Events"));
            Assert.That(driver.PageSource, Does.Contain("<h1>All Events</h1>"));
        }

        [Test]
        public void Test_CreateEventPage_CreateEvent_ValidData()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Events/Create");
            Assert.That(driver.Title, Does.Contain("Create Event"));

            var eventName = "Party" + DateTime.Now.Ticks;
            driver.FindElement(By.Id("Name")).SendKeys(eventName);
            driver.FindElement(By.Id("Place")).SendKeys("Beach");
            driver.FindElement(By.Id("TotalTickets")).SendKeys("100");
            driver.FindElement(By.Id("PricePerTicket")).SendKeys("10.00");

            driver.FindElement(By.XPath("//input[contains(@value,'Create')]")).Click();

            Assert.That(driver.Title, Does.Contain("All Events"));
            Assert.That(driver.PageSource, Does.Contain("<h1>All Events</h1>"));
            Assert.That(driver.PageSource, Does.Contain(eventName));
            Assert.That(driver.PageSource, Does.Contain("Beach"));
            Assert.That(driver.PageSource, Does.Contain(username));

            var eventRow = driver.FindElements(By.TagName("tr")).FirstOrDefault(e => e.Text.Contains(eventName));
            Assert.That(eventRow, Is.Not.Null);
            Assert.That(eventRow.Text, Does.Contain("Delete"));
            Assert.That(eventRow.Text, Does.Contain("Edit"));
        }

        [Test]
        public void Test_CreateEventPage_CreateEvent_InvalidData()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Events/Create");
            Assert.That(driver.Title, Does.Contain("Create Event"));

            driver.FindElement(By.Id("Name")).SendKeys("");
            driver.FindElement(By.XPath("//input[contains(@value,'Create')]")).Click();

            StringAssert.Contains("/Events/Create", driver.Url);
            var errorSpan = driver.FindElement(By.Id("Name-error"));
            Assert.That(errorSpan.Text, Is.EqualTo("The Name field is required."));
        }

        [Test]
        public void Test_DeleteEvent()
        {
            var eventName = CreateEvent();

            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.PageSource, Does.Contain(eventName));

            var deleteBtn = driver.FindElement(By.XPath($"//tr//td[contains(text(), '{eventName}')]/..//a[contains(.,'Delete')]"));
            deleteBtn.Click();

            StringAssert.Contains("/Events/Delete/", driver.Url);
            Assert.That(driver.Title, Does.Contain("Delete Event"));
            Assert.That(driver.PageSource, Does.Contain(eventName));

            driver.FindElement(By.XPath("//input[contains(@value,'Delete')]")).Click();

            Assert.That(driver.PageSource.Contains(eventName), Is.False);
        }

        [Test]
        public void Test_EditEvent_ValidData()
        {
            var eventName = CreateEvent();
            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.PageSource, Does.Contain(eventName));

            var editButton = driver.FindElement(By.XPath($"//tr//td[contains(text(), '{eventName}')]/..//a[contains(.,'Edit')]"));
            editButton.Click();

            StringAssert.Contains("/Events/Edit/", driver.Url);
            Assert.That(driver.Title, Does.Contain("Edit Event"));

            var changedName = "Best Best Show" + DateTime.Now.Ticks;
            var nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            nameField.SendKeys(changedName);

            driver.FindElement(By.XPath("//input[contains(@value,'Edit')]")).Click();

            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.PageSource, Does.Contain(changedName));
            Assert.That(driver.PageSource.Contains(eventName), Is.False);
        }

        [Test]
        public void Test_EditEvent_InvalidData()
        {
            var eventName = CreateEvent();
            StringAssert.Contains("/Events/All", driver.Url);
            Assert.That(driver.PageSource, Does.Contain(eventName));

            var editButton = driver.FindElement(By.XPath($"//tr//td[contains(text(), '{eventName}')]/..//a[contains(.,'Edit')]"));
            editButton.Click();

            StringAssert.Contains("/Events/Edit/", driver.Url);
            Assert.That(driver.Title, Does.Contain("Edit Event"));

            var nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            nameField.SendKeys("");

            driver.FindElement(By.XPath("//input[contains(@value,'Edit')]")).Click();

            StringAssert.Contains("/Events/Edit/", driver.Url);
            var errorSpan = driver.FindElement(By.Id("Name-error"));
            Assert.That(errorSpan.Text, Is.EqualTo("The Name field is required."));
        }

        private void RegisterUserForTesting()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Identity/Account/Register");

            driver.FindElement(By.Id("Input_Username")).SendKeys(username);
            driver.FindElement(By.Id("Input_Email")).SendKeys($"{username}@mail.com");
            driver.FindElement(By.Id("Input_Password")).SendKeys(password);
            driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(password);
            driver.FindElement(By.Id("Input_FirstName")).SendKeys("Pesho");
            driver.FindElement(By.Id("Input_LastName")).SendKeys("Petrov");

            driver.FindElement(By.XPath("//button[@type='submit'][contains(.,'Register')]")).Click();

            StringAssert.EndsWith("/", driver.Url);
            Assert.That(driver.PageSource, Does.Contain($"Welcome, {username}"));
        }

        private string CreateEvent()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Events/Create");
            Assert.That(driver.Title, Does.Contain("Create Event"));

            var eventName = "Best Show" + DateTime.Now.Ticks;
            driver.FindElement(By.Id("Name")).SendKeys(eventName);
            driver.FindElement(By.Id("Place")).SendKeys("Sofia");
            driver.FindElement(By.Id("TotalTickets")).SendKeys("50");
            driver.FindElement(By.Id("PricePerTicket")).SendKeys("5");

            driver.FindElement(By.XPath("//input[contains(@value,'Create')]")).Click();

            return eventName;
        }
    }
}
