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
    public class DetailsModel : PageModel
    {
        private readonly DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext _context;

        public DetailsModel(DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Movie Movie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);

            if (movie is not null)
            {
                Movie = movie;

                return Page();
            }

            return NotFound();
        }
    }
}
