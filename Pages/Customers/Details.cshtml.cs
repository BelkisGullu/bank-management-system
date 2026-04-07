using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Pages.Customers
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Customer? Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Session kontrolü
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Admin")
            {
                return RedirectToPage("/Account/CustomerLogin");
            }

            if (id == null)
            {
                return NotFound();
            }

            Customer = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Customer == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}



