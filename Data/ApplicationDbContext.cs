using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DSD605SecAndAuthStudentVersion2025.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Movie> Movie { get; set; } = default!;
        public DbSet<Models.Cast> Cast { get; set; } = default!;
    }
}
