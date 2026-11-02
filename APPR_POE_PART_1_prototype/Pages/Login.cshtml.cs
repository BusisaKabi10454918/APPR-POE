using APPR_POE_PART_1_prototype.Application_Services;
using APPR_POE_PART_1_prototype.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace APPR_POE_PART_1_prototype.Pages
{
    public class LoginModel : PageModel
    {
        public string validationResult { get; set; } = string.Empty;

        private readonly GOGAppDBContext _context;
        private readonly VerificationServices _verificationServices;

        public LoginModel(VerificationServices verifyer)
        {
            _verificationServices = verifyer;
        }

        [BindProperty]
        public string Credential { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnPost()
        {
            string credential = Credential;
            string password = Password;

            if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(password))
            {
                validationResult = "VER-LOG-0001:\nPlease enter both email/username and password";
                return;
            }

            //Check if the credential is an email, otherwise assume a username
            validationResult = _verificationServices.attemptLogin(credential, password);

            if (validationResult.Contains("VER-LOG-0000"))
            {
                //Successful login. setup session
                HttpContext.Session.SetString("User", credential);
                return;
            }
            else
            {
                //Failed login
                validationResult += "\nLogin failed.\nPlease check your credentials.";
                return;
            }
        }
    }
}
