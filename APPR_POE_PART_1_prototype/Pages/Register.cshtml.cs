using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace APPR_POE_PART_1_prototype.Pages
{
    public class RegisterModel : PageModel
    {
        public string error_message = string.Empty;
        public void OnGet()
        {
        }

        public string SaveDetails(List<string> creds)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data", "sudoDB.txt");

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                foreach (string cred in creds)
                {
                    writer.WriteLine(cred);
                }
                writer.WriteLine(Environment.NewLine);
                Console.WriteLine("Credential saved");
            }
            
            return "Saved";
        }

        public void OnPost()
        {
            List<string> credentials = new List<string>();

            string firstname = Request.Form["firstname"].ToString();
            string lastname = Request.Form["lastname"].ToString();
            string email = Request.Form["email"].ToString();
            string username = Request.Form["username"].ToString();
            string password = Request.Form["password"].ToString();
            string confirmPassword = Request.Form["confirm-password"].ToString();
            List<string> roleSelection = Request.Form["role"].ToList();

            if (password.Equals(confirmPassword))
            {
                if (roleSelection.Contains("volunteer") && roleSelection.Contains("repeat-donor"))
                {
                    credentials.Add($"{firstname} {lastname}");
                    credentials.Add(username);
                    credentials.Add(email);
                    credentials.Add(password);
                    credentials.Add("volunteer + repeat-donor");

                    error_message = $"""
                        Thank you, {credentials[0]} 
                        for registering as a volunteer and donor
                        We appreciate your time and kind contribution
                        """;

                    SaveDetails(credentials);
                }
                else if (roleSelection.Contains("volunteer"))
                {
                    credentials.Add($"{firstname} {lastname}");
                    credentials.Add(username);
                    credentials.Add(email);
                    credentials.Add(password);
                    credentials.Add("volunteer");

                    error_message = $"""
                        Thank you, {credentials[0]} 
                        for registering as a volunteer
                        We appreciate your time
                        """;

                    SaveDetails(credentials);

                }
                else if (roleSelection.Contains("repeat-donor"))
                {
                    credentials.Add($"{firstname} {lastname}");
                    credentials.Add(username);
                    credentials.Add(email);
                    credentials.Add(password);
                    credentials.Add("repeat-donor");

                    error_message = $"""
                        Thank you, {credentials[0]} 
                        for registering as a Repeat Donor
                        You are very kind
                        """;

                    SaveDetails(credentials);
                }
            }
            else
            {
                error_message = "Please enter matching passwords";
            }
        }
    }
}
