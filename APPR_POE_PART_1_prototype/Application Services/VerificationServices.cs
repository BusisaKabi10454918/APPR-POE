using APPR_POE_PART_1_prototype.Data;
using System.Text.RegularExpressions;

namespace APPR_POE_PART_1_prototype.Application_Services
{

    public class VerificationServices
    {

        public VerificationServices(GOGAppDBContext context)
        {
            _context = context;
        }
        private readonly GOGAppDBContext _context;
        public string checkCredentials(string firstname, string lastname, string username, string email)
        {
            //Checks will happen to verify the parameters aren't empty regardless we must check them
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(email) ||
                String.IsNullOrWhiteSpace(firstname) || String.IsNullOrWhiteSpace(lastname))
            {
                //Invalid Entry
                return "VER-REG-2001: INVALID ENTRY";
            }

            //Check if the first and last names are valid, only letters allowed
            if (!Regex.IsMatch(firstname, @"^[a-zA-Z]+$"))
            {
                return "VER-REG-2004: INVALID FIRST NAME";
            }

            if(!Regex.IsMatch(lastname, @"^[a-zA-Z]+$"))
            {
                return "VER-REG-2005: INVALID LAST NAME";
            }

            //Check if the email is valid, must contain @ and a domain
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "VER-REG-2006: INVALID EMAIL FORMAT";
            }

            //No invalid entries start checks for credentials
            if (credSearch(username))
            {
                //Username already in use
                return "VER-REG-2002: USERNAME ALREADY IN USE";
            }

            if (credSearch(email))
            {
                //Email in use
                return "VER-REG-2003: EMAIL ALREADY REGISTERED";
            }

            //No issues, clean exit code
            return "VER-REG-0000: REGISTRATION SUCCESSFUL";
        }

        public string passwordStrength(string password)
        {
            //Measure length
            if (!Regex.IsMatch(password, @"^.{8,}$"))
            {
                //Below minimum
                return "VER-PWD-1001: PASSWORD TOO SHORT";
            }

            //At least 1 number
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                //No Number
                return "VER-PWD-1002: PASSWORD MUST CONTAIN AT LEAST ONE NUMBER";
            }

            //At least 1 special character
            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
            {
                //No special character
                return "VER-PWD-1003: PASSWORD MUST CONTAIN AT LEAST ONE SPECIAL CHARACTER";
            }

            //All check good, proceed
            return "VER-PWD-0000";
        }

        public string attemptLogin(string credential, string password)
        {
            //Check if the credential is an email, otherwise assume a username
            if (credential.Contains("@"))
            {
                //Credential is an email
                if (!_context.Users.Any(u => u.Email == credential))
                {
                    return "VER-LOG-0002:\nEmail not found";
                }
            }
            else
            {
                //Credential is a username
                if (!_context.Users.Any(u => u.Username == credential))
                {
                    return "VER-LOG-0003:\nUsername not found";
                }
            }
            //Check if the password matches the credential
            var user = _context.Users.FirstOrDefault(u => u.Username == credential || u.Email == credential);
            if (user != null && !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return "VER-LOG-0004:\nUsername/Email or Password is incorrect";
            }
            //All checks passed, login successful
            return $"VER-LOG-0000:\nLOGIN SUCCESSFUL\nWelcome back, {user.FirstName}";
        }

        private bool credSearch(string credential)
        {
            //Check if the credential is an email, if so check if taken.
            if (credential.Contains("@"))
            {                
                return _context.Users.Any(u => u.Email == credential);
                
            }
            return _context.Users.Any(u => u.Username == credential);
        }
    }
}
