using APPR_POE_PART_1_prototype.Application_Services;
using APPR_POE_PART_1_prototype.Data;
using APPR_POE_PART_1_prototype.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace APPR_Tests
{
    public class IntegrationTest
    {
        [Fact]
        public void StandardIntegrationTest()
        {
            //Create an in-memory SQLite database connection to simulate the actual DBcontext for testing purposes. This allows us to perform integration tests without affecting the real database.
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();


            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            //Use the actual GOGAppDBContext with the in-memory SQLite database connection to perform integration tests.
            using var context = new GOGAppDBContext(options);

            //Ensure the database is created and ready for testing.
            context.Database.EnsureCreated();

            //Make use of the VerificationServices class to perform integration tests on the database context.
            var verificationServices = new VerificationServices(context);

            //Register a new user in the in-memory database for testing purposes.
            //Passing credentials for a successful registration
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Username = "johndoe",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleSelection = new List<string> { "volunteer" }
            };

            //Call the OnPost method to simulate the registration process and validate the result.
            string firstnamePass = "John";
            string lastnamePass = "Doe";

            pageModel.OnPost();

            //Ensure the registration was successful and the validation result is as expected.
            Assert.Equal($"VER-REG-0000: REGISTRATION SUCCESSFUL\nThank you, {firstnamePass} {lastnamePass} \nfor registering as a volunteer\nWe appreciate your time", pageModel.validationResult);

            //Now we login our new user
            LoginModel loginModel = new LoginModel(verificationServices)
            {
                Credential = "johndoe",
                Password = "Password123!"
            };

            var httpContext = new DefaultHttpContext();

            httpContext.Features.Set<ISessionFeature>(
                new TestSessionFeature());

            loginModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            loginModel.OnPost();

            Assert.Equal($"VER-LOG-0000:\nLOGIN SUCCESSFUL\nWelcome back, John", loginModel.validationResult);
            Assert.Equal("johndoe", httpContext.Session.GetString("User"));
        }
    }
}
