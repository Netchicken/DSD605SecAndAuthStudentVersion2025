using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DSD605SecAndAuthStudentVersion2025.Pages.ClaimsManager
{
    /// <summary>
    /// Page model for displaying existing claims, creating new claims, and modifying existing claims
    /// </summary>
    public class CreateModel(UserManager<IdentityUser> userManager) : PageModel
    {
        // Primary constructor parameter automatically becomes a private readonly field
        private readonly UserManager<IdentityUser> _userManager = userManager;

        /// <summary>
        /// Collection of users for dropdown selection - not bound to prevent tampering
        /// </summary>
        [BindNever]
        public SelectList Users { get; set; } = new(Enumerable.Empty<SelectListItem>());

        /// <summary>
        /// List of all existing claims with user details
        /// </summary>
        [BindNever]
        public List<UserClaimDetail> AllUserClaims { get; set; } = new();

        /// <summary>
        /// List of unique claim type groups for reference
        /// </summary>
        [BindNever]
        public List<ClaimTypeGroup> ClaimTypeGroups { get; set; } = new();

        /// <summary>
        /// Total count of unique claim types
        /// </summary>
        [BindNever]
        public int TotalClaimTypes { get; set; }

        /// <summary>
        /// Total count of all claims across all users
        /// </summary>
        [BindNever]
        public int TotalClaims { get; set; }

        /// <summary>
        /// Total number of users with claims
        /// </summary>
        [BindNever]
        public int UsersWithClaims { get; set; }

        /// <summary>
        /// The selected user's ID who will receive the claim - REQUIRED
        /// </summary>
        [BindProperty]
        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "User")]
        public string SelectedUserId { get; set; } = string.Empty;

        /// <summary>
        /// The type/category of the claim being created - STORED IN DATABASE
        /// </summary>
        [BindProperty]
        [Required(ErrorMessage = "Claim type is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Claim type must be between 1 and 100 characters")]
        [Display(Name = "Claim Type")]
        public string ClaimType { get; set; } = string.Empty;

        /// <summary>
        /// The value of the claim - STORED IN DATABASE
        /// </summary>
        [BindProperty]
        [StringLength(255, ErrorMessage = "Claim value cannot exceed 255 characters")]
        [Display(Name = "Claim Value")]
        public string? ClaimValue { get; set; }

        /// <summary>
        /// Status message for user feedback
        /// </summary>
        [TempData]
        public string? StatusMessage { get; set; }

        /// <summary>
        /// Handles GET requests to display the create claim form and existing claims
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadUsersAsync();
            await LoadExistingClaimsAsync();
        }

        /// <summary>
        /// Handles POST requests to process claim creation
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Reload data in case of validation errors
            await LoadUsersAsync();
            await LoadExistingClaimsAsync();

            // Validate form input
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var claimType = ClaimType.Trim();
                var claimValue = ClaimValue?.Trim() ?? string.Empty;

                // Find the selected user
                var user = await _userManager.FindByIdAsync(SelectedUserId);
                if (user is null)
                {
                    ModelState.AddModelError(nameof(SelectedUserId), "Selected user not found.");
                    return Page();
                }

                // Check for duplicate claims for this user
                var existingUserClaims = await _userManager.GetClaimsAsync(user);
                var isDuplicate = existingUserClaims.Any(c =>
                    string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(c.Value, claimValue, StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    ModelState.AddModelError(string.Empty,
                        $"User '{user.UserName}' already has the claim '{claimType}: {claimValue}'.");
                    return Page();
                }

                // Create and assign the claim
                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.AddClaimAsync(user, claim);

                if (result.Succeeded)
                {
                    StatusMessage = $"Success: Created and assigned claim '{claimType}: {claimValue}' to user '{user.UserName}'.";

                    // Clear the form for next entry
                    ClaimType = string.Empty;
                    ClaimValue = string.Empty;
                    SelectedUserId = string.Empty;

                    return RedirectToPage();
                }

                // Handle creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while creating the claim: {ex.Message}");
            }

            return Page();
        }

        /// <summary>
        /// Handles POST requests to delete a specific claim
        /// </summary>
        public async Task<IActionResult> OnPostDeleteClaimAsync(string userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    StatusMessage = "Error: User not found.";
                    return RedirectToPage();
                }

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.RemoveClaimAsync(user, claim);

                if (result.Succeeded)
                {
                    StatusMessage = $"Success: Removed claim '{claimType}: {claimValue}' from user '{user.UserName}'.";
                }
                else
                {
                    StatusMessage = $"Error: Failed to remove claim '{claimType}: {claimValue}' from user '{user.UserName}'.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }

            return RedirectToPage();
        }

        /// <summary>
        /// Handles POST requests to update a specific claim
        /// </summary>
        public async Task<IActionResult> OnPostUpdateClaimAsync(string userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    StatusMessage = "Error: User not found.";
                    return RedirectToPage();
                }

                // Remove old claim
                var oldClaim = new Claim(oldClaimType, oldClaimValue);
                var removeResult = await _userManager.RemoveClaimAsync(user, oldClaim);

                if (removeResult.Succeeded)
                {
                    // Add new claim
                    var newClaim = new Claim(newClaimType.Trim(), newClaimValue?.Trim() ?? string.Empty);
                    var addResult = await _userManager.AddClaimAsync(user, newClaim);

                    if (addResult.Succeeded)
                    {
                        StatusMessage = $"Success: Updated claim for user '{user.UserName}' from '{oldClaimType}: {oldClaimValue}' to '{newClaimType}: {newClaimValue}'.";
                    }
                    else
                    {
                        // Re-add old claim if new claim failed
                        await _userManager.AddClaimAsync(user, oldClaim);
                        StatusMessage = $"Error: Failed to update claim. Restored original claim.";
                    }
                }
                else
                {
                    StatusMessage = $"Error: Failed to remove old claim '{oldClaimType}: {oldClaimValue}'.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }

            return RedirectToPage();
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
                    .Select(u => new { u.Id, u.UserName, u.Email })
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

        /// <summary>
        /// Loads all existing claims from all users with detailed information
        /// </summary>
        private async Task LoadExistingClaimsAsync()
        {
            try
            {
                // Get all users and their claims
                var users = await _userManager.Users.AsNoTracking().ToListAsync();
                AllUserClaims = new List<UserClaimDetail>();
                var allClaims = new List<Claim>();

                foreach (var user in users)
                {
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    allClaims.AddRange(userClaims);

                    foreach (var claim in userClaims)
                    {
                        AllUserClaims.Add(new UserClaimDetail
                        {
                            UserId = user.Id,
                            UserName = user.UserName ?? "Unknown",
                            Email = user.Email ?? "No Email",
                            ClaimType = claim.Type,
                            ClaimValue = claim.Value
                        });
                    }
                }

                // Group claims by type for reference
                ClaimTypeGroups = allClaims
                    .GroupBy(c => c.Type)
                    .Select(g => new ClaimTypeGroup
                    {
                        ClaimType = g.Key,
                        Values = g.Select(c => c.Value).Distinct().OrderBy(v => v).ToList(),
                        UsageCount = g.Count()
                    })
                    .OrderBy(ctg => ctg.ClaimType)
                    .ToList();

                // Calculate statistics
                TotalClaimTypes = ClaimTypeGroups.Count;
                TotalClaims = allClaims.Count;
                UsersWithClaims = AllUserClaims.Select(uc => uc.UserId).Distinct().Count();
            }
            catch (Exception)
            {
                // Fallback to empty lists if loading fails
                AllUserClaims = new List<UserClaimDetail>();
                ClaimTypeGroups = new List<ClaimTypeGroup>();
                TotalClaimTypes = 0;
                TotalClaims = 0;
                UsersWithClaims = 0;
            }
        }
    }

    /// <summary>
    /// Helper class to group claims by type with their values
    /// </summary>
    public class ClaimTypeGroup
    {
        public string ClaimType { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
        public int UsageCount { get; set; }
    }

    /// <summary>
    /// Detailed information about a user's claim
    /// </summary>
    public class UserClaimDetail
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }
}