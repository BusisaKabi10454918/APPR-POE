using APPR_POE_PART_1_prototype.Application_Services;
using APPR_POE_PART_1_prototype.Data;
using APPR_POE_PART_1_prototype.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace APPR_Tests
{
    public class UnitTests
    {
        [Fact]
        public void RegistrationTest_SuccessfulRegistration()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);

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

            string firstnamePass = "John";
            string lastnamePass = "Doe";
            
            pageModel.OnPost();

            Assert.Equal($"VER-REG-0000: REGISTRATION SUCCESSFUL\nThank you, {firstnamePass} {lastnamePass} \nfor registering as a volunteer\nWe appreciate your time", pageModel.validationResult);
        }

        [Fact]
        public void RegistrationTest_MismatchedPasswords()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);

            //Passing credentials for a failed registration
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Username = "johndoe",
                Password = "Password123!",
                ConfirmPassword = "Random_test",
                RoleSelection = new List<string> { "volunteer" }
            };
            
            pageModel.OnPost();

            Assert.Equal($"VER-REG-1004: PASSWORDS DO NOT MATCH", pageModel.validationResult);
        }

        [Fact]
        public void RegistrationTest_DualRoleRegistration()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);

            //Passing credentials for dual role registration
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Username = "johndoe",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleSelection = new List<string> { "volunteer", "repeat-donor" }
            };
            string firstnameDualRole = "John";
            string lastnameDualRole = "Doe";

            pageModel.OnPost();

            Assert.Equal($"VER-REG-0000: REGISTRATION SUCCESSFUL\nThank you, {firstnameDualRole} {lastnameDualRole} \nfor registering as a volunteer and donor\nWe appreciate your time and kind contribution", pageModel.validationResult);
        }

        [Fact]
        public void RegistrationTest_NoRoleSelected()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);

            //Passing credentials for no role selection
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Username = "johndoe",
                Password = "Password123!",
                ConfirmPassword = "Password123",
                RoleSelection = null
            };
            
            pageModel.OnPost();

            Assert.Equal($"VER-REG-1005: PLEASE SELECT AT LEAST ONE ROLE", pageModel.validationResult);
        }

        [Fact]
        public void LoginTest_Success()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);


            //Register a user for login testing
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "Thomas",
                LastName = "Wayne",
                Email = "TWayne@WayneCoEnt.us",
                Username = "TWayne13",
                Password = "BatmanIsAwesome21!",
                ConfirmPassword = "BatmanIsAwesome21!",
                RoleSelection = new List<string> { "volunteer" }
            };

            pageModel.OnPost();

            LoginModel loginModel = new LoginModel(verificationServices)
            {
                Credential = "TWayne13",
                Password = "BatmanIsAwesome21!"
            };

            var httpContext = new DefaultHttpContext();

            httpContext.Features.Set<ISessionFeature>(
                new TestSessionFeature());

            loginModel.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            loginModel.OnPost();

            Assert.Equal($"VER-LOG-0000:\nLOGIN SUCCESSFUL\nWelcome back, Thomas", loginModel.validationResult);
        }

        [Fact]
        public void LoginTest_IncorrectUsername()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);


            //Register a user for login testing
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "Thomas",
                LastName = "Wayne",
                Email = "TWayne@WayneCoEnt.us",
                Username = "TWayne13",
                Password = "BatmanIsAwesome21!",
                ConfirmPassword = "BatmanIsAwesome21!",
                RoleSelection = new List<string> { "volunteer" }
            };

            pageModel.OnPost();

            LoginModel loginModel = new LoginModel(verificationServices)
            {
                Credential = "Robin4",
                Password = "BatmanIsAwesome21!"
            };

            loginModel.OnPost();

            Assert.Equal($"VER-LOG-0003:\nUsername not found\nLogin failed.\nPlease check your credentials.", loginModel.validationResult);
        }

        [Fact]
        public void LoginTest_IncorrectPassword()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<GOGAppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new GOGAppDBContext(options);

            context.Database.EnsureCreated();

            var verificationServices = new VerificationServices(context);


            //Register a user for login testing
            RegisterModel pageModel = new RegisterModel(context, verificationServices)
            {
                FirstName = "Thomas",
                LastName = "Wayne",
                Email = "TWayne@WayneCoEnt.us",
                Username = "TWayne13",
                Password = "BatmanIsAwesome21!",
                ConfirmPassword = "BatmanIsAwesome21!",
                RoleSelection = new List<string> { "volunteer" }
            };

            pageModel.OnPost();

            LoginModel loginModel = new LoginModel(verificationServices)
            {
                Credential = "TWayne13",
                Password = "PasswordTest"
            };

            loginModel.OnPost();

            Assert.Equal($"VER-LOG-0004:\nUsername/Email or Password is incorrect\nLogin failed.\nPlease check your credentials.", loginModel.validationResult);

        }
    }
}
