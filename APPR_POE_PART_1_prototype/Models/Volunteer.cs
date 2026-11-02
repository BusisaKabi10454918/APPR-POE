using System.ComponentModel.DataAnnotations;

namespace APPR_POE_PART_1_prototype.Models
{
    public class Volunteer
    {
        public Volunteer() { }

        public Volunteer(Guid userId)
        {
            UserID = userId;
        }

        [Key]
        public Guid VolunteerID { get; set; } = Guid.NewGuid();

        public Guid UserID { get; set; }

        public User User { get; set; } = null!;
    }
}