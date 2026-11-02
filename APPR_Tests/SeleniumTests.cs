using Microsoft.Data.SqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace APPR_Tests
{
    public class SeleniumTests
    {
        [Fact]
        public void DonateAnonymously() 
        {
            var options = new ChromeOptions();

            using IWebDriver driver = new ChromeDriver(options);

            driver.Navigate().GoToUrl("https://localhost:7083");

            driver.FindElement(
                By.Id("donatelink")).Click();

            IWebElement CurrencySelect = driver.FindElement(By.CssSelector("input[name='Currency'][value='USD']"));
            IWebElement DonationAmount = driver.FindElement(By.Id("DonationAmount"));

            CurrencySelect.Click();
            DonationAmount.SendKeys("200,95");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            IWebElement successPopup = driver.FindElement(By.Id("success-validation"));

            Assert.Contains("VER-DON-0000", successPopup.Text);
        }

        [Fact]
        public void CreateNewTestUser() 
        {
            var options = new ChromeOptions();

            using IWebDriver driver = new ChromeDriver(options);

            driver.Navigate().GoToUrl("https://localhost:7083");

            driver.FindElement(
                By.Id("volunteerlink")).Click();

            driver.FindElement(
                By.Id("registerlink")).Click();

            IWebElement firstName = driver.FindElement(By.Id("firstname"));
            IWebElement lastName = driver.FindElement(By.Id("lastname"));
            IWebElement email = driver.FindElement(By.Id("email"));
            IWebElement username = driver.FindElement(By.Id("username"));
            IWebElement password = driver.FindElement(By.Id("password"));
            IWebElement confirmPassword = driver.FindElement(By.Id("confirm-password"));
            IWebElement volunteer = driver.FindElement(By.Id("volunteer"));

            firstName.SendKeys("Harley");
            lastName.SendKeys("Quinn");
            email.SendKeys("DrQuinzel@ArkhamAsylum.us");
            username.SendKeys("DrHQuinn");
            password.SendKeys("password123!");
            confirmPassword.SendKeys("password123!");

            volunteer.Click();

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            IWebElement successPopup = driver.FindElement(By.Id("success-validation"));

            Assert.Contains("VER-REG-0000", successPopup.Text);

            ClearTestUser();
        }

        private void ClearTestUser()
        {
            string connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=GiftOfTheGiversDB;Trusted_Connection=True;TrustServerCertificate=True;";

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = @"
            DELETE FROM Volunteers
            WHERE UserID IN (
                SELECT UserID FROM Users WHERE Username = @Username
            );

            DELETE FROM Users
            WHERE Username = @Username;
            ";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", "DrHQuinn");
            command.ExecuteNonQuery();
        }

        //This second method will perform the exact same test on a ideal chrome build, preferably on another computer with chrome installed
    }
}
