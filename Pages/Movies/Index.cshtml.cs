using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSD605SecAndAuthStudentVersion2025.Data;
using RolesForAssessment.Models;

namespace DSD605SecAndAuthStudentVersion2025.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext _context;

        public IndexModel(DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Movie = await _context.Movie.ToListAsync();
        }
    }
}
