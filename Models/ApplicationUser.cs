using Microsoft.AspNetCore.Identity;

namespace DSD605SecAndAuthStudentVersion2025.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Phone { get; set; }

    }
}
