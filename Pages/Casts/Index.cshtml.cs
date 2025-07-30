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
    public class IndexModel : PageModel
    {
        private readonly DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext _context;

        public IndexModel(DSD605SecAndAuthStudentVersion2025.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cast> Cast { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Cast = await _context.Cast
                .Include(c => c.Movie).ToListAsync();
        }
    }
}
