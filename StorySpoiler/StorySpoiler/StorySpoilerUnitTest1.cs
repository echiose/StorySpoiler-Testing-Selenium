using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StorySpoiler
{
    [TestFixture]
    public class Tests
    {
        protected IWebDriver driver;
        private static readonly string baseUrl = "http://144.91.123.158:100";
        private static string lastCreatedSpoilerTitle;
        private static string lastCreatedSpoilerDescription;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var chromeOptions = new ChromeOptions();
            //turn off chrome allert 
            chromeOptions.AddUserProfilePreference(
                "credentials_enable_service",
                false
            );

            chromeOptions.AddUserProfilePreference(
                "profile.password_manager_enabled",
                false
            );

            chromeOptions.AddUserProfilePreference(
                "profile.password_manager_leak_detection",
                false
            );

            driver = new ChromeDriver(chromeOptions);

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl($"{baseUrl}/User/Login");

            //login to the application
            driver.FindElement(By.Id("username")).SendKeys("eli");
            driver.FindElement(By.Id("password")).SendKeys("eli123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Console.WriteLine("After login: " + driver.Url);
        }

        [Test, Order(1)]
        //2.2.1.	Create Story Spoiler With Invalid Data Test
        public void CreateStorySpoilerWithInvalidDataTest()
        {
            //arrange
            string invalidTitle = "";
            string invalidDescription = "";

            //go to create story spoiler page
            driver.Navigate().GoToUrl($"{baseUrl}/Story/Add");

            
            //Console.WriteLine("Before alert: " + driver.Url);
            //check for allert + wait for 2 sec
            AcceptAlertIfPresent();

            //Console.WriteLine("After alert: " + driver.Url);
            //Console.WriteLine(
            //    "PageSource contains id=\"title\": " +
            //    driver.PageSource.Contains("id=\"title\"")
            //);

            //fill out the form with invalid data
            driver.FindElement(By.Id("title")).SendKeys(invalidTitle);
            driver.FindElement(By.Id("description")).SendKeys(invalidDescription);

            //Submit the form by clicking the submit button
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            //check that the page remains on the same URL
            string currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{baseUrl}/Story/Add"), "The page should remain the same");

            //check that the main error message is displayed
            var mainErrorMessage = driver.FindElement(By.CssSelector(".validation-summary-errors li"));
            Assert.That(mainErrorMessage.Text, Is.EqualTo("Unable to add this spoiler!"), "The main error message should be displayed");

            //check that the validation error messages for the title and description fields are displayed
            var validationFieldsErrors = driver.FindElements(By.CssSelector(".field-validation-error"));
            Assert.That(validationFieldsErrors[0].Text, Is.EqualTo("The Title field is required."), "The title field error message should be displayed");
            Assert.That(validationFieldsErrors[1].Text, Is.EqualTo("The Description field is required."), "The description field error message should be displayed");
            Assert.That(validationFieldsErrors.Count, Is.EqualTo(2), "There should be two validation error messages displayed");
        }



        [Test, Order(2)]
        //2.2.2.	Create Random Story Spoiler Test
        public void CreateRandomStorySpoilerTest()
        {
            //arrange
            lastCreatedSpoilerTitle = "Spoiler" + +new Random().Next(100, 999);
            lastCreatedSpoilerDescription = "Description" + new Random().Next(1000, 9999);

            //go to create story spoiler page
            driver.Navigate().GoToUrl($"{baseUrl}/Story/Add");
            //Console.WriteLine("Current URL: " + driver.Url);

            //check for allert + wait for 2 sec
            AcceptAlertIfPresent();

            //fill out the form with invalid data
            //driver.FindElement(By.Id("title")).SendKeys(lastCreatedSpoilerTitle);
            driver.FindElement(By.XPath("//*[@id=\"title\"]")).SendKeys(lastCreatedSpoilerTitle);
            driver.FindElement(By.Id("description")).SendKeys(lastCreatedSpoilerDescription);

            //Submit the form by clicking the submit button
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            //check for allert + wait for 2 sec
            AcceptAlertIfPresent();

            //check that the browser navigated to the home page after submission.
            string currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{baseUrl}/"));

            //check that the last created spoiler is displayed on the home page with the correct title
            var spoilerRows = driver.FindElements(By.CssSelector(".row.gx-5.align-items-center"));
            var lastSpoilerCard = spoilerRows.Last();

            var spoilerTitleElement = lastSpoilerCard.FindElement(By.CssSelector(".display-4"));

            Assert.That(spoilerTitleElement.Text, Is.EqualTo(lastCreatedSpoilerTitle), "The title of the last created spoiler should match the expected title");
        }


        [Test, Order(3)]
        //2.2.3.	Edit Last Created Story Spoiler Title Test
        public void EditLastCreatedStorySpoilerTitleTest()
        {
            //go to the home page
            driver.Navigate().GoToUrl($"{baseUrl}/");

            //get list of spoiler cards
            var spoilerRows = driver.FindElements(By.CssSelector(".row.gx-5.align-items-center"));

            //assert that there are any spoiler cards present
            Assert.That(spoilerRows.Count, Is.GreaterThan(0), "There should be at least one spoiler card");

            //Console.WriteLine(lastCreatedSpoilerTitle);
            //Console.WriteLine(lastCreatedSpoilerDescription);

            //get the last spoiler card
            var lastSpoilerCard = spoilerRows.Last();

            //get the edit button from the last spoiler card
            var editButton = lastSpoilerCard.FindElement(By.CssSelector("a[href*='/Story/Edit']"));
            new Actions(driver).MoveToElement(editButton).Click().Perform();

            //Update the last created spoiler title
            lastCreatedSpoilerTitle = "Changed Title: " + lastCreatedSpoilerTitle;

            //Modify the spoiler title and save
            var titleInput = driver.FindElement(By.XPath("//*[@id=\"title\"]"));
            titleInput.Clear();
            titleInput.SendKeys(lastCreatedSpoilerTitle);
            //Click the save button
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            //check for allert + wait for 2 sec
            AcceptAlertIfPresent();

            //check that the browser navigated to the home page after submission.
            string currentUrl = driver.Url;
            //Console.WriteLine(currentUrl);
            Assert.That(currentUrl, Is.EqualTo($"{baseUrl}/"));

            //check that the last created spoiler is displayed on the home page with the correct title
            spoilerRows = driver.FindElements(By.CssSelector(".row.gx-5.align-items-center"));
            lastSpoilerCard = spoilerRows.Last();

            //get the title element from the last spoiler card and assert that it matches the updated title
            var spoilerTitleElement = lastSpoilerCard.FindElement(By.CssSelector(".display-4"));
            Assert.That(spoilerTitleElement.Text, Is.EqualTo(lastCreatedSpoilerTitle), "The title of the last created spoiler should match the expected title");

        }

        [Test, Order(4)]
        //2.2.4.	Delete Last Created Story Spoiler Test
        public void DeleteLastCreatedStorySpoilerTest()
        {

            //go to the home page
            driver.Navigate().GoToUrl($"{baseUrl}/");

            //get list of spoiler rows
            var spoilerRows = driver.FindElements(By.CssSelector(".row.gx-5.align-items-center"));
            //get the list of all spoiler cards
            int countBeforeDelete = spoilerRows.Count;

            //assert that there are any spoiler cards present
            Assert.That(spoilerRows.Count, Is.GreaterThan(0), "There should be at least one spoiler card");

            //get the last spoiler row
            var lastSpoilerCard = spoilerRows.Last();

            //get the edit button from the last spoiler row
            var deleteButton = lastSpoilerCard.FindElement(By.CssSelector("a[href*='/Story/Delete']"));
            new Actions(driver).MoveToElement(deleteButton).Click().Perform();

            //check for allert + wait for 2 sec
            AcceptAlertIfPresent();


            //Navigate back to home page and view the last created story spoiler
            Assert.That(driver.Url, Is.EqualTo($"{baseUrl}/"), "The page url should be home page url");

            //get updated list of spoiler rows
            spoilerRows = driver.FindElements(By.CssSelector(".row.gx-5.align-items-center"));

            //get the list of all spoiler titles
            var titles = spoilerRows
                 .Select(row => row.FindElement(By.CssSelector(".display-4")).Text.Trim())
                 .ToList();

            //assert that the deleted spoiler title is no longer present
            Assert.That(titles, Does.Not.Contain(lastCreatedSpoilerTitle),"The deleted spoiler title should no longer be present on home page");

            //get the last spoiler row
            lastSpoilerCard = spoilerRows.Last();

            //get the title element from the last spoiler row 
            var spoilerTitleElement = lastSpoilerCard.FindElement(By.CssSelector(".display-4"));

            //check that the title of the last created spoiler does not match the deleted title
            Assert.That(spoilerTitleElement.Text, Is.Not.EqualTo(lastCreatedSpoilerTitle), "The title of the last created spoiler should not match the deleted title");

            //check that the count of spoiler cards has decreased by one
            Assert.That(titles.Count(), Is.EqualTo(countBeforeDelete - 1), "The count of spoiler cards has decreased by one  ");
        }

        [Test, Order(5)]
        //2.2.5.	Try to Edit Non-Existent Story Spoiler Test
        public void EditNonExistentStorySpoilerTitleTest()
        {

            //Navigate to the Home Page
            driver.Navigate().GoToUrl($"{baseUrl}/");

            //Create a random non-existent story spoiler ID
            string invalidStoryId = Guid.NewGuid().ToString();

            //Attempt to edit a non-existent story spoiler
            driver.Navigate().GoToUrl($"{baseUrl}/Story/Edit?storyId={invalidStoryId}");

            //Find the error message
            var errorMessage = driver.FindElement(
                    By.XPath("//*[contains(text(), 'No such spoiler!')]")
                );

            //Verify that the error message is displayed
            Assert.That(errorMessage.Displayed, Is.True, "The error message should be displayed");

            //Verify the error message text
            Assert.That(errorMessage.Text.Trim(), Does.Contain("No such spoiler!"), "The error message should contain 'No such spoiler!'");

        }

        [Test, Order(5)]
        //2.2.5.	Try to Delete Non-Existent Story Spoiler Test
        public void DeleteNonExistentStorySpoilerTest()
        {

            //Navigate to the Home Page
            driver.Navigate().GoToUrl($"{baseUrl}/");

            //Create a random non-existent story spoiler ID
            string invalidStoryId = Guid.NewGuid().ToString();


            //Attempt to delete a non-existent story spoiler
            driver.Navigate().GoToUrl($"{baseUrl}/Story/Delete?storyId={invalidStoryId}");

            //Find the error message
            var errorMessage = driver.FindElement(
                    By.XPath("//*[contains(text(), 'No such spoiler!')]")
                );

            //Verify that the error message is displayed
            Assert.That(errorMessage.Displayed, Is.True, "The error message should be displayed");

            //Verify the error message text
            Assert.That(errorMessage.Text.Trim(), Does.Contain("No such spoiler!"), "The error message should contain 'No such spoiler!'");

        }

        private void AcceptAlertIfPresent()
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                wait.Until(drv =>
                {
                    try
                    {
                        drv.SwitchTo().Alert();
                        return true;
                    }
                    catch (NoAlertPresentException)
                    {
                        return false;
                    }
                });

                driver.SwitchTo().Alert().Accept();
            }
            catch (WebDriverTimeoutException)
            {
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Dispose();
        }
    }
}
