namespace DSD605SecAndAuthStudentVersion2025.ViewModels
{
    public class UserRolesViewModel
    {
        public string? UserName { get; set; }
        public List<string> Roles { get; set; } = new();
        public string? RoleName { get; set; }

        /// <summary>
        /// Returns true if the user has any roles assigned
        /// </summary>
        public bool HasRoles => Roles?.Any() == true;

        /// <summary>
        /// Returns the number of roles assigned to this user
        /// </summary>
        public int RoleCount => Roles?.Count ?? 0;

        /// <summary>
        /// Returns a comma-separated string of all roles
        /// </summary>
        public string RolesDisplay => Roles?.Any() == true ? string.Join(", ", Roles) : "No roles assigned";



    }
}
