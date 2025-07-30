namespace RolesForAssessment.Models
{
    using Microsoft.AspNetCore.Identity;

    using System;

    public class DSD605SecAndAuthStudentVersion2025 : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
