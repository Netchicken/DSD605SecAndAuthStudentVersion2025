using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

namespace DSD605SecAndAuthStudentVersion2025.Pages.RolesManager
{
    /// <summary>
    /// Page model for creating new roles
    /// </summary>
    [BindProperties]
    public class CreateModel : PageModel
    {
        // Dependency injection for role management
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Constructor that receives RoleManager through dependency injection
        /// </summary>
        public CreateModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// The name of the role to be created
        /// </summary>
        [Required]
        [StringLength(256, MinimumLength = 3)]
        [Display(Name = "Role Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Handles GET requests to display the create role form
        /// </summary>
        public void OnGet()
        {
            // Nothing to load for GET - form starts empty
        }

        /// <summary>
        /// Handles POST requests to create a new role
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if form data is valid
            if (!ModelState.IsValid)
                return Page();

            // Clean up the role name
            var roleName = Name.Trim();

            // Check if role already exists to prevent duplicates
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError(
                    nameof(Name),
                    $"The role \"{roleName}\" already exists.");
                return Page();
            }

            // Create the new role
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                // Success - redirect to index page
                return RedirectToPage("Index");
            }

            // Handle creation errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
