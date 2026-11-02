using APPR_POE_PART_1_prototype.Application_Services;
using APPR_POE_PART_1_prototype.Data;
using APPR_POE_PART_1_prototype.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APPR_POE_PART_1_prototype.Pages
{
    public class RegisterModel : PageModel
    {
        public string validationResult { get; set; } = string.Empty;

        private readonly GOGAppDBContext _context;
        private readonly VerificationServices _verificationServices;

        public RegisterModel(GOGAppDBContext context, VerificationServices verifyer)
        {
            _context = context;
            _verificationServices = verifyer;
        }

        [BindProperty]
        public string FirstName { get; set; } = "";

        [BindProperty]
        public string LastName { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string ConfirmPassword { get; set; } = "";

        [BindProperty]
        public List<string> RoleSelection { get; set; } = new List<string>();

        public void OnPost()
        {
            string firstName = FirstName;
            string lastName = LastName;
            string username = Username;
            string email = Email;
            string password = Password;
            string confirmPassword = ConfirmPassword;
            List<string> roleSelection = RoleSelection;

            

            string check_A = _verificationServices.passwordStrength(password);
            if (RoleSelection == null || !RoleSelection.Any())
            {
                validationResult = "VER-REG-1005: PLEASE SELECT AT LEAST ONE ROLE";
                return;
            }

            //Password and confirm password must match
            if (password != confirmPassword)
            {
                validationResult = "VER-REG-1004: PASSWORDS DO NOT MATCH";
                return;
            }

            //Check if the password is strong enough and if the credentials are valid
            if (check_A != "VER-PWD-0000")
            {
                validationResult = check_A;
                return;
            }

            string check_B = _verificationServices.checkCredentials(firstName, lastName, username, email);

            //Check if the credentials are valid
            if (check_B != "VER-REG-0000: REGISTRATION SUCCESSFUL")
            {
                validationResult = check_B;
                return;
            }

            //If all checks pass, create a new user and save to the database
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            User newUser = new User(firstName, lastName, email, username, passwordHash);
            
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();

                //Check are you a volunteer, if yes , create a new volunteer and save to the database
                if (roleSelection.Contains("volunteer"))
                {
                    Volunteer newVolunteer = new Volunteer(newUser.UserID);
                    _context.Volunteers.Add(newVolunteer);
                    _context.SaveChanges();

                    //Now check are you also a repeat donor, if yes, thank you for registering as a volunteer and donor, else thank you for registering as a volunteer
                    if (roleSelection.Contains("repeat-donor"))
                    {
                        
                        validationResult = check_B + $"\nThank you, {firstName} {lastName} \nfor registering as a volunteer and donor\nWe appreciate your time and kind contribution";
                        transaction.Commit();
                        return;
                    }

                    //Not a repeat donor as well, thank you for registering as a volunteer                    
                    validationResult = check_B + $"\nThank you, {firstName} {lastName} \nfor registering as a volunteer\nWe appreciate your time";
                    transaction.Commit();
                    return;

                }

                //Not a volunteer, so you must be a repeat donor, thank you for registering as a donor
                validationResult = check_B + $"\nThank you, {firstName} {lastName} \nfor registering as a donor\nWe appreciate your kind contribution";
                transaction.Commit();
                return;
            }
            catch
            {
                //Otherwise, failed to register, rollback the transaction and return an error message
                transaction.Rollback();
                validationResult = "VER-REG-3001: USER REGISTRATION FAILED";
                return;
            }
        }
    }
}