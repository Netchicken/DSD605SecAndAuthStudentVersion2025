namespace RolesForAssessment.Models
{
    using Microsoft.AspNetCore.Identity;

    using System;

    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
