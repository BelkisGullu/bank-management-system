using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Controllers
{
    public class BranchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Branch
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var branches = await _context.Branches
                .Include(b => b.Customers)
                .Include(b => b.Employees)
                .ToListAsync();

            return View(branches);
        }

        // GET: Branch/Create
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,Phone")] Branch branch)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(branch);
        }

        // GET: Branch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Phone")] Branch branch)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != branch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(branch);
        }

        // GET: Branch/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Customers)
                .Include(b => b.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                // İlişkili kayıtlar varsa silinemez kontrolü
                if (branch.Customers.Any() || branch.Employees.Any())
                {
                    TempData["ErrorMessage"] = "Bu şubeye bağlı müşteriler veya çalışanlar olduğu için silinemez!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}



