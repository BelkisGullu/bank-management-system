using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Session kontrolü - Sadece Admin
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Admin")
            {
                Response.Redirect("/Account/CustomerLogin");
                return;
            }

            Customers = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .ToListAsync();
        }
    }
}



