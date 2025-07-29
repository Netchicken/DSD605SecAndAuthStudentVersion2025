using DSD605SecAndAuthStudentVersion2025.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace DSD605SecAndAuthStudentVersion2025.Pages.RolesManager
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // All roles in system
        public List<IdentityRole> Roles { get; set; }

        // Each user with their roles
        public List<UserRolesViewModel> UserRoles { get; set; }

        public async Task OnGetAsync()
        {
            // Load roles (no tracking for read-only)
            Roles = await _roleManager.Roles
                .AsNoTracking()
                .ToListAsync();

            // Load users and their roles
            var users = await _userManager.Users
                .AsNoTracking()
                .ToListAsync();

            UserRoles = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles.Add(new UserRolesViewModel
                {
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }
        }
    }

}
