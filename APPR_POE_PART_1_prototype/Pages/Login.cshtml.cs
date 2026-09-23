using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APPR_POE_PART_1_prototype.Pages
{
    public class LoginModel : PageModel
    {
        public string error_message = string.Empty;
        public void OnGet()
        {
        }

        public string attemptLogin(string cred, string password)
        {
            bool loginSuccess = false;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data", "sudoDB.txt");

            if (System.IO.File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();

                    if (content.Contains(cred) && content.Contains(password))
                    {
                        loginSuccess = true;
                    }
                }
            }
            
            if(loginSuccess)
            {
                return "Pass";
            }
            return "Fail";

        }

        public void OnPost()
        {
            string cred = Request.Form["credential"];
            string password = Request.Form["Password"];

            if (attemptLogin(cred, password).Equals("Pass"))
            {
                error_message = $"Welcome back, {cred}";
            }
            else
            {
                error_message = "Incorrect Username or Password.";
            }
        }
    }
}
