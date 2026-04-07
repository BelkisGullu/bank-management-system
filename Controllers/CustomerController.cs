using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<IActionResult> Index(string? branchFilter, string? searchName)
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Admin")
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            // Query string'den direkt değer al (duplicate parametre sorununu önlemek için)
            var searchNameValue = Request.Query["searchName"].FirstOrDefault() ?? searchName;
            var branchFilterValue = Request.Query["branchFilter"].FirstOrDefault() ?? branchFilter;

            IQueryable<Customer> customersQuery = _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative);

            // Şube filtresi
            if (!string.IsNullOrEmpty(branchFilterValue) && branchFilterValue != "all")
            {
                customersQuery = customersQuery.Where(c => c.BranchId == int.Parse(branchFilterValue));
            }

            // İsme göre arama
            if (!string.IsNullOrEmpty(searchNameValue))
            {
                customersQuery = customersQuery.Where(c => c.Name.Contains(searchNameValue));
            }

            var customers = await customersQuery.ToListAsync();

            ViewData["Branches"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
            ViewData["SelectedBranch"] = branchFilterValue ?? "all";
            ViewData["SearchName"] = searchNameValue;

            return View(customers);
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
            ViewData["RepresentativeId"] = new SelectList(new List<Employee>(), "Id", "Name");
            
            // Tüm temsilcileri şubelerine göre grupla (JavaScript için - AJAX yok)
            var allRepresentatives = await _context.Employees
                .Where(e => e.Position == "Müşteri Temsilcisi")
                .Select(e => new { BranchId = e.BranchId, Id = e.Id, Name = e.Name })
                .ToListAsync();
            ViewData["AllRepresentatives"] = allRepresentatives;
            
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,TC,Phone,Email,BranchId,RepresentativeId,AccountNumber,Balance,CreditCard,CreditLimit,UsedCredit")] Customer customer)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            // TC kontrolü
            if (await _context.Customers.AnyAsync(c => c.TC == customer.TC))
            {
                ModelState.AddModelError("TC", "Bu TC Kimlik No zaten kullanılıyor!");
            }

            // Hesap numarası kontrolü
            if (await _context.Customers.AnyAsync(c => c.AccountNumber == customer.AccountNumber))
            {
                ModelState.AddModelError("AccountNumber", "Bu hesap numarası zaten kullanılıyor!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Müşteri başarıyla eklendi! ✅";
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", customer.BranchId);
            
            if (customer.BranchId > 0)
            {
                var representatives = await _context.Employees
                    .Where(e => e.BranchId == customer.BranchId && e.Position == "Müşteri Temsilcisi")
                    .ToListAsync();
                ViewData["RepresentativeId"] = new SelectList(representatives, "Id", "Name", customer.RepresentativeId);
            }
            else
            {
                ViewData["RepresentativeId"] = new SelectList(new List<Employee>(), "Id", "Name");
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
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

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", customer.BranchId);
            
            var representatives = await _context.Employees
                .Where(e => e.BranchId == customer.BranchId && e.Position == "Müşteri Temsilcisi")
                .ToListAsync();
            ViewData["RepresentativeId"] = new SelectList(representatives, "Id", "Name", customer.RepresentativeId);

            // Tüm temsilcileri şubelerine göre grupla (JavaScript için - AJAX yok)
            var allRepresentatives = await _context.Employees
                .Where(e => e.Position == "Müşteri Temsilcisi")
                .Select(e => new { BranchId = e.BranchId, Id = e.Id, Name = e.Name })
                .ToListAsync();
            ViewData["AllRepresentatives"] = allRepresentatives;

            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TC,Phone,Email,BranchId,RepresentativeId,AccountNumber,Balance,CreditCard,CreditLimit,UsedCredit")] Customer customer)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != customer.Id)
            {
                return NotFound();
            }

            // TC kontrolü (kendisi hariç)
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TC == customer.TC && c.Id != id);
            
            if (existingCustomer != null)
            {
                ModelState.AddModelError("TC", "Bu TC Kimlik No zaten kullanılıyor!");
            }

            // Hesap numarası kontrolü (kendisi hariç)
            var existingAccount = await _context.Customers
                .FirstOrDefaultAsync(c => c.AccountNumber == customer.AccountNumber && c.Id != id);
            
            if (existingAccount != null)
            {
                ModelState.AddModelError("AccountNumber", "Bu hesap numarası zaten kullanılıyor!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Müşteri başarıyla güncellendi! ✅";
                return RedirectToAction(nameof(Index));
            }

            ViewData["BranchId"] = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name", customer.BranchId);
            
            var representatives = await _context.Employees
                .Where(e => e.BranchId == customer.BranchId && e.Position == "Müşteri Temsilcisi")
                .ToListAsync();
            ViewData["RepresentativeId"] = new SelectList(representatives, "Id", "Name", customer.RepresentativeId);

            return View(customer);
        }

        // GET: Customer/Delete/5
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

            var customer = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Müşteri başarıyla silindi! ✅";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/GiveCredit/5
        public async Task<IActionResult> GiveCredit(int? id)
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/GiveCredit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveCredit(int id, decimal creditAmount)
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (creditAmount <= 0)
            {
                TempData["ErrorMessage"] = "Kredi tutarı 0'dan büyük olmalıdır!";
                return RedirectToAction(nameof(GiveCredit), new { id });
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Krediyi müşterinin hesabına ekle
            customer.Balance += creditAmount;

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{customer.Name} müşterisine {creditAmount:N2} TL kredi başarıyla verildi ve hesabına eklendi! ✅";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // AJAX: Şubeye göre temsilcileri getir
        [HttpGet]
        public async Task<IActionResult> GetRepresentativesByBranch(int branchId)
        {
            var representatives = await _context.Employees
                .Where(e => e.BranchId == branchId && e.Position == "Müşteri Temsilcisi")
                .Select(e => new { id = e.Id, name = e.Name })
                .ToListAsync();

            return Json(representatives);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}



