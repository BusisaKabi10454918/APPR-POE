using APPR_POE_PART_1_prototype.Data;
using GiftOfTheGivers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace APPR_POE_PART_1_prototype.Pages
{
    public class DonateModel : PageModel
    {

        public DonateModel(GOGAppDBContext context)
        {
            _context = context;
        }
        private readonly GOGAppDBContext _context;
        public string validationResult = string.Empty;

        [BindProperty]
        public string Currency { get; set; } = string.Empty;

        [BindProperty]
        public string DonationAmount { get; set; } = string.Empty;

        public void OnPost()
        {
            string currencySelection = Currency;
            decimal donationAmount = 0;

            //Attempt to parse the donation amoount. Failure means an invalid entry.
            try
            {
                string amountInput = DonationAmount.Replace(',', '.');

                if (!decimal.TryParse(
                    amountInput,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal amount))
                {
                    validationResult = "VER-DON-0001:\nInvalid donation amount format.\nPlease enter a valid number.";
                    return;
                }

                donationAmount = amount;
            }
            catch (FormatException)
            {
                validationResult = "VER-DON-0001:\nInvalid donation amount format.\nPlease enter a valid number.";
                return;
            }

            //Amount is valid. Forms minimum is 1.00, in case of emergency, check if somehow a number 0 or less than 1 was entered
            if (donationAmount < 1)
            {
                validationResult = "VER-DON-0002:\nDonation amount must be at least 1.00.\nPlease enter a valid donation amount.";
                return;
            }

            //Should be a valid amount, check the currency, convert to ZAR, and then process the donation.
            decimal convertedAmount = currencyConversion(currencySelection, donationAmount);

            //Save the record to the database, and return a success message to the user.
            if(HttpContext.Session.GetString("User") != null)
            {
                
                //A user has logged in, call their id and save the donation.
                var userCredential = HttpContext.Session.GetString("User");

                //Find the users name to thank them
                var user = _context.Users.FirstOrDefault(u => u.Username == userCredential || u.Email == userCredential);
                
                //Check if the session maybe expired
                if (user == null)
                {
                    validationResult = "VER-DON-0003:\nUser session is invalid. Please log in again.";
                    return;
                }

                //Session hasnt expired, so there should be a name
                string name = user.FirstName;

                DonationRecord newDonation = new DonationRecord
                {
                    DonorId = user.UserID.ToString(),
                    Amount = convertedAmount,
                    Date = DateTime.Now
                };

                _context.Add(newDonation);
                _context.SaveChanges();
                validationResult = $"VER-DON-0000:\nDonation successful.\nThank you {name},\nfor your generous donation of {donationAmount} {Currency}.";
            }
            else
            {
                DonationRecord newDonation = new DonationRecord
                {
                    DonorId = $"ANONYMOUS-{Guid.NewGuid()}",
                    Amount = convertedAmount,
                    Date = DateTime.Now
                };

                _context.Add(newDonation);
                _context.SaveChanges();
                validationResult = $"VER-DON-0000:\nDonation successful.\nThank you ANONYMOUS,\nfor your generous donation of {donationAmount} {Currency}.";
            }
            
        }

        public decimal currencyConversion(string currency, decimal donationTotal)
        {
            decimal convertedTotal = 0;
            switch (currency)
            {
                case "USD":
                    convertedTotal = donationTotal * 19;
                    break;
                case "EUR":
                    convertedTotal = donationTotal * 20;
                    break;
                default:
                    convertedTotal = donationTotal;
                    break;
            }
            return convertedTotal;
        }

    }


}
