using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static GiftOfTheGivers.Helpers;

namespace APPR_POE_PART_1_prototype.Pages
{
    public class DonateModel : PageModel
    {
        public string error_message = string.Empty;
        public void OnGet()
        {
        }

        public void OnPost()
        {
            string currencySelect = Request.Form["currency"].ToString();
            string donationAmount = Request.Form["donation-amount"].ToString();

            if (string.IsNullOrEmpty(currencySelect) || string.IsNullOrEmpty(donationAmount))
            {
                error_message = "Please fill in all fields.";
            }
            else if (donationAmount.Contains("e"))
            {
                error_message = "Please enter a valid number.";
            }
            else
            {
                error_message = $"Thank you for your generous donation of {donationAmount} {currencySelect}.\nWe appreciate your kindness.";
            }

            collectDonationTotals();
        }

        public void collectDonationTotals()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data", "donationsCollected.txt");
            string donationsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data", "SudoDonationDB.csv");

            var helperfunction = new donationCalculator();

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine("New Donation");
                writer.WriteLine($"Current Monthly Total: {helperfunction.collectMonthlyTotal(donationsPath)}");
                writer.WriteLine($"Current Yearly Total: {helperfunction.collectYearlyTotal(donationsPath)}");
            }
        }

    }


}
