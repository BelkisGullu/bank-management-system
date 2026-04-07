using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index(string? searchName)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Query string'den direkt değer al (duplicate parametre sorununu önlemek için)
            var searchNameValue = Request.Query["searchName"].FirstOrDefault() ?? searchName;

            IQueryable<Employee> employeesQuery = _context.Employees
                .Include(e => e.Branch);

            // İsme göre arama
            if (!string.IsNullOrEmpty(searchNameValue))
            {
                employeesQuery = employeesQuery.Where(e => e.Name.Contains(searchNameValue));
            }

            var employees = await employeesQuery.ToListAsync();

            ViewData["SearchName"] = searchNameValue;

            return View(employees);
        }

        // GET: Employee/Create
        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,TC,Position,BranchId,Phone,Email")] Employee employee)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            // TC kontrolü
            if (await _context.Employees.AnyAsync(e => e.TC == employee.TC))
            {
                ModelState.AddModelError("TC", "Bu TC Kimlik No zaten kullanılıyor!");
            }

            // Şube Müdürü kontrolü
            if (employee.Position == "Şube Müdürü")
            {
                var existingManager = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Position == "Şube Müdürü" && 
                                            e.BranchId == employee.BranchId);
                
                if (existingManager != null)
                {
                    ModelState.AddModelError("Position", "Bu şubede zaten bir Şube Müdürü var! Her şubede sadece 1 müdür olabilir.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", employee.BranchId);
            return View(employee);
        }

        // GET: Employee/Edit/5
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

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", employee.BranchId);
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TC,Position,BranchId,Phone,Email")] Employee employee)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != employee.Id)
            {
                return NotFound();
            }

            // TC kontrolü (kendisi hariç)
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.TC == employee.TC && e.Id != id);
            
            if (existingEmployee != null)
            {
                ModelState.AddModelError("TC", "Bu TC Kimlik No zaten kullanılıyor!");
            }

            // Şube Müdürü kontrolü (kendisi hariç)
            if (employee.Position == "Şube Müdürü")
            {
                var existingManager = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Position == "Şube Müdürü" && 
                                            e.BranchId == employee.BranchId &&
                                            e.Id != id);
                
                if (existingManager != null)
                {
                    ModelState.AddModelError("Position", "Bu şubede zaten bir Şube Müdürü var! Her şubede sadece 1 müdür olabilir.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", employee.BranchId);
            return View(employee);
        }

        // GET: Employee/Delete/5
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

            var employee = await _context.Employees
                .Include(e => e.Branch)
                .Include(e => e.Customers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = await _context.Employees
                .Include(e => e.Customers)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee != null)
            {
                // İlişkili müşteriler varsa silinemez
                if (employee.Customers.Any())
                {
                    TempData["ErrorMessage"] = "Bu çalışana bağlı müşteriler olduğu için silinemez!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Şubeye göre çalışanları getir
        [HttpGet]
        public async Task<IActionResult> GetEmployeesByBranch(int branchId)
        {
            var employees = await _context.Employees
                .Where(e => e.BranchId == branchId && e.Position == "Müşteri Temsilcisi")
                .Select(e => new { id = e.Id, name = e.Name })
                .ToListAsync();

            return Json(employees);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}



