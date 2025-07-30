using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RolesForAssessment.Models;

namespace DSD605SecAndAuthStudentVersion2025.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<RolesForAssessment.Models.Movie> Movie { get; set; } = default!;
    }
}
