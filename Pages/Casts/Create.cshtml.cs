using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DSD605SecAndAuthStudentVersion2025.Data;
using DSD605SecAndAuthStudentVersion2025.Models;

namespace DSD605SecAndAuthStudentVersion2025.Pages.Casts
{
    public class CreateModel : PageModel
    {
        private readonly DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext _context;

        public CreateModel(DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Cast Cast { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Cast.Add(Cast);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
