using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace DSD605SecAndAuthStudentVersion2025.Pages.RolesManager
{
    /// <summary>
    /// Page model for assigning roles to users
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AssignModel : PageModel
    {
        // Dependency injection for user and role management
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Constructor that receives UserManager and RoleManager through dependency injection
        /// </summary>
        public AssignModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// The selected username from the form
        /// </summary>
        [BindProperty]
        [Required]
        [Display(Name = "User")]
        public string SelectedUserName { get; set; } = string.Empty;

        /// <summary>
        /// The selected role name from the form
        /// </summary>
        [BindProperty]
        [Required]
        [Display(Name = "Role")]
        public string SelectedRoleName { get; set; } = string.Empty;

        /// <summary>
        /// List of all users for the dropdown - not bound to prevent tampering
        /// </summary>
        public SelectList Users { get; private set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        /// <summary>
        /// List of all roles for the dropdown - not bound to prevent tampering
        /// </summary>
        public SelectList Roles { get; private set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        /// <summary>
        /// Handles GET requests to display the assign role form
        /// </summary>
        public async Task OnGetAsync()
        {
            // Load users and roles for the dropdown lists
            await PopulateListsAsync();
        }

        /// <summary>
        /// Handles POST requests to assign a role to a user
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Reload dropdown lists in case of validation errors
            await PopulateListsAsync();

            // Check if form data is valid
            if (!ModelState.IsValid)
                return Page();

            // Find the selected user
            var user = await _userManager.FindByNameAsync(SelectedUserName);
            if (user == null)
            {
                ModelState.AddModelError(
                    nameof(SelectedUserName),
                    "Selected user does not exist.");
                return Page();
            }

            // Check if the selected role exists
            if (!await _roleManager.RoleExistsAsync(SelectedRoleName))
            {
                ModelState.AddModelError(
                    nameof(SelectedRoleName),
                    "Selected role does not exist.");
                return Page();
            }

            // Check if user already has this role to prevent duplicates
            if (await _userManager.IsInRoleAsync(user, SelectedRoleName))
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"User '{SelectedUserName}' is already assigned to role '{SelectedRoleName}'.");
                return Page();
            }

            // Attempt to assign the role to the user
            var result = await _userManager.AddToRoleAsync(user, SelectedRoleName);

            // Handle assignment errors
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            // Success - set message and redirect to index
            TempData["SuccessMessage"] = $"Successfully assigned user '{SelectedUserName}' to role '{SelectedRoleName}'.";
            return RedirectToPage(nameof(Index));
        }

        /// <summary>
        /// Helper method to populate the dropdown lists with users and roles
        /// </summary>
        private async Task PopulateListsAsync()
        {
            // Get all users sorted alphabetically
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .ToListAsync();

            // Get all roles sorted alphabetically
            var roles = await _roleManager.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            // Create SelectList for users dropdown
            Users = new SelectList(
                users,
                nameof(IdentityUser.UserName),
                nameof(IdentityUser.UserName),
                SelectedUserName);

            // Create SelectList for roles dropdown
            Roles = new SelectList(
                roles,
                nameof(IdentityRole.Name),
                nameof(IdentityRole.Name),
                SelectedRoleName);
        }
    }
}


