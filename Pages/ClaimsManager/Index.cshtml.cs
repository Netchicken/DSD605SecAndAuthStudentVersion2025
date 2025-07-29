using DSD605SecAndAuthStudentVersion2025.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DSD605SecAndAuthStudentVersion2025.Pages.ClaimsManager
{
    /// <summary>
    /// Page model for the Claims Manager Index page that displays all users and their assigned claims
    /// </summary>
    [BindProperties]
    public class IndexModel : PageModel
    {
        // Dependency injection for managing user operations
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Constructor that receives UserManager through dependency injection
        /// </summary>
        /// <param name="userManager">ASP.NET Core Identity UserManager for user operations</param>
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Collection of all users in the system for display purposes
        /// </summary>
        public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        /// <summary>
        /// Collection of view models containing each user and their associated claims
        /// </summary>
        public List<UserClaimsViewModel> UserClaims { get; set; } = new List<UserClaimsViewModel>();

        /// <summary>
        /// Handles GET requests to load and display all users with their claims
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        public async Task OnGetAsync()
        {
            // Load all users from the database with no change tracking for better performance
            Users = await _userManager.Users
                .AsNoTracking()
                .ToListAsync();

            // Initialize the collection to store user claims data
            UserClaims = new List<UserClaimsViewModel>();

            // Loop through each user to get their claims
            foreach (var user in Users)
            {
                // Retrieve all claims for the current user
                var claims = await _userManager.GetClaimsAsync(user);

                // Create a view model combining user info with their claims
                UserClaims.Add(new UserClaimsViewModel
                {
                    UserName = user.UserName ?? string.Empty, // Handle null username
                    Claims = claims.ToList() // Convert IList to List for the view model
                });
            }
        }
    }
}