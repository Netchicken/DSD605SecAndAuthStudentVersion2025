using System.Security.Claims;

namespace DSD605SecAndAuthStudentVersion2025.ViewModels
{
    /// <summary>
    /// View model for displaying user claims information in the Claims Manager
    /// </summary>
    public class UserClaimsViewModel
    {
        /// <summary>
        /// The username of the user
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of all claims assigned to this user
        /// </summary>
        public List<Claim> Claims { get; set; } = new List<Claim>();

        /// <summary>
        /// Returns true if the user has any claims assigned
        /// </summary>
        public bool HasClaims => Claims.Any();

        /// <summary>
        /// Returns the total number of claims assigned to this user
        /// </summary>
        public int ClaimCount => Claims.Count;
    }
}

