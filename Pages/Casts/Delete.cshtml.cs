using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSD605SecAndAuthStudentVersion2025.Data;
using DSD605SecAndAuthStudentVersion2025.Models;

namespace DSD605SecAndAuthStudentVersion2025.Pages.Casts
{
    public class DeleteModel : PageModel
    {
        private readonly DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext _context;

        public DeleteModel(DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cast Cast { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cast = await _context.Cast.FirstOrDefaultAsync(m => m.Id == id);

            if (cast is not null)
            {
                Cast = cast;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cast = await _context.Cast.FindAsync(id);
            if (cast != null)
            {
                Cast = cast;
                _context.Cast.Remove(Cast);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
