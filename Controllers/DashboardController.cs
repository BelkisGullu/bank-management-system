using BankManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Index
        public IActionResult Index()
        {
            // Session kontrolü - Sadece Admin
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Admin")
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            return View();
        }

        // GET: Dashboard/Statistics
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var customerCount = await _context.Customers.CountAsync();
            var employeeCount = await _context.Employees.CountAsync();
            var branchCount = await _context.Branches.CountAsync();
            var unreadMessageCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            return Json(new
            {
                customers = customerCount,
                employees = employeeCount,
                branches = branchCount,
                unreadMessages = unreadMessageCount
            });
        }

        // GET: Dashboard/BranchStats
        [HttpGet]
        public async Task<IActionResult> BranchStats()
        {
            var branches = await _context.Branches
                .Include(b => b.Customers)
                .Include(b => b.Employees)
                .ToListAsync();

            var stats = branches.Select(b => new
            {
                name = b.Name,
                customers = b.Customers.Count,
                employees = b.Employees.Count
            }).ToList();

            return Json(stats);
        }
    }
}



