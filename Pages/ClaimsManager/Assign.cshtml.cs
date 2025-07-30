using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DSD605SecAndAuthStudentVersion2025.Pages.ClaimsManager
{
    /// <summary>
    /// Page model for assigning claims to users using modern C# patterns
    /// </summary>
    [BindProperties]
    public class AssignModel(UserManager<IdentityUser> userManager) : PageModel
    {
        // Primary constructor parameter automatically becomes a private readonly field
        private readonly UserManager<IdentityUser> _userManager = userManager;

        /// <summary>
        /// Collection of users for dropdown selection - not bound to prevent tampering
        /// </summary>
        public SelectList Users { get; private set; } = new(Enumerable.Empty<SelectListItem>());

        /// <summary>
        /// The selected user's ID who will receive the claim
        /// </summary>
        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "User")]
        public required string SelectedUserId { get; set; } = string.Empty;

        /// <summary>
        /// The type/category of the claim being assigned
        /// </summary>
        [Required(ErrorMessage = "Claim type is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Claim type must be between 1 and 100 characters")]
        [Display(Name = "Claim Type")]
        public required string ClaimType { get; set; } = string.Empty;

        /// <summary>
        /// The value of the claim - optional for boolean-type claims
        /// </summary>
        [StringLength(255, ErrorMessage = "Claim value cannot exceed 255 characters")]
        [Display(Name = "Claim Value")]
        public string? ClaimValue { get; set; }

        /// <summary>
        /// Status message for user feedback
        /// </summary>
        [TempData]
        public string? StatusMessage { get; set; }

        /// <summary>
        /// Handles GET requests to display the assign claim form
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadUsersAsync();
        }

        /// <summary>
        /// Handles POST requests to process claim assignment
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Reload users in case of validation errors
            await LoadUsersAsync();

            // Validate form input
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Find the selected user
                var user = await _userManager.FindByIdAsync(SelectedUserId);
                if (user is null)
                {
                    ModelState.AddModelError(nameof(SelectedUserId), "Selected user not found.");
                    return Page();
                }

                // Check for duplicate claims
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var isDuplicate = existingClaims.Any(c =>
                    string.Equals(c.Type, ClaimType, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(c.Value, ClaimValue ?? string.Empty, StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    ModelState.AddModelError(string.Empty,
                        $"User '{user.UserName}' already has the claim '{ClaimType}: {ClaimValue ?? string.Empty}'.");
                    return Page();
                }

                // Create and assign the claim
                var claim = new Claim(ClaimType, ClaimValue ?? string.Empty);
                var result = await _userManager.AddClaimAsync(user, claim);

                if (result.Succeeded)
                {
                    StatusMessage = $"Successfully assigned claim '{ClaimType}: {ClaimValue ?? string.Empty}' to user '{user.UserName}'.";
                    return RedirectToPage("/ClaimsManager/Index");
                }

                // Handle assignment errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return Page();
        }

        /// <summary>
        /// Loads all users for the dropdown selection with proper error handling
        /// </summary>
        private async Task LoadUsersAsync()
        {
            try
            {
                // Get all users sorted alphabetically for better UX
                var users = await _userManager.Users
                    .AsNoTracking()
                    .OrderBy(u => u.UserName)
                    .Select(u => new { u.Id, u.UserName, u.Email }) // Project only needed fields
                    .ToListAsync();

                // Create SelectList with fallback display names
                var userItems = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName ?? u.Email ?? "Unknown User"
                });

                Users = new SelectList(userItems, "Value", "Text", SelectedUserId);
            }
            catch (Exception)
            {
                // Fallback to empty list if loading fails
                Users = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}