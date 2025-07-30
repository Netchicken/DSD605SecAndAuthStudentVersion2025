using Microsoft.AspNetCore.Identity;

namespace RolesForAssessment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Phone { get; set; }

    }
}
