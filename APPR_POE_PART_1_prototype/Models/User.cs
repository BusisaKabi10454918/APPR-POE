using System.ComponentModel.DataAnnotations;

namespace APPR_POE_PART_1_prototype.Models
{
    public class User
    {
        //Empty constructor for EF Core
        public User() { }

        //Constructor for creating a new user with all required properties
        public User(string firstName, string lastName, string email, string username, string passwordHash)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Username = username;
            PasswordHash = passwordHash;
        }

        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}
