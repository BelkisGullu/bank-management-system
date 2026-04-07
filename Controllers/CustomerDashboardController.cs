using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Controllers
{
    public class CustomerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerDashboard/Index
        public async Task<IActionResult> Index()
        {
            // Müşteri kontrolü
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer")
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .FirstOrDefaultAsync(c => c.Id == int.Parse(customerId));

            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("CustomerLogin", "Account");
            }

            return View(customer);
        }

        // GET: CustomerDashboard/Profile
        public async Task<IActionResult> Profile()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer")
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers
                .Include(c => c.Branch)
                .Include(c => c.Representative)
                .FirstOrDefaultAsync(c => c.Id == int.Parse(customerId));

            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("CustomerLogin", "Account");
            }

            return View(customer);
        }

        // POST: CustomerDashboard/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int id, string name, string phone, string email, string? password, string? passwordConfirm)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer" || id.ToString() != customerId)
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Email kontrolü (başka müşteri kullanıyorsa)
            if (await _context.Customers.AnyAsync(c => c.Email == email && c.Id != id))
            {
                TempData["ErrorMessage"] = "Bu email adresi zaten kullanılıyor!";
                return RedirectToAction("Profile");
            }

            customer.Name = name;
            customer.Phone = phone;
            customer.Email = email;

            // Şifre güncelleme (opsiyonel)
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(passwordConfirm))
            {
                if (password == passwordConfirm && password.Length >= 8)
                {
                    customer.Password = password;
                }
                else
                {
                    TempData["ErrorMessage"] = "Şifreler eşleşmiyor veya en az 8 karakter olmalıdır!";
                    return RedirectToAction("Profile");
                }
            }

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi! ✅";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Profile");
        }

        // GET: CustomerDashboard/Deposit
        public async Task<IActionResult> Deposit()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer")
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers.FindAsync(int.Parse(customerId));
            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("CustomerLogin", "Account");
            }

            return View(customer);
        }

        // POST: CustomerDashboard/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer" || id.ToString() != customerId)
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Yatırılacak tutar 0'dan büyük olmalıdır!";
                return View(customer);
            }

            customer.Balance += amount;
            
            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{amount:N2} ₺ hesabınıza yatırıldı! Yeni bakiyeniz: {customer.Balance:N2} ₺";
                TempData["LastTransaction"] = "true";
                TempData["LastTransactionType"] = "Deposit";
                TempData["LastTransactionAmount"] = amount.ToString("N2");
                TempData["LastTransactionDescription"] = "Para Yatırma";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: CustomerDashboard/Withdraw
        public async Task<IActionResult> Withdraw()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer")
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers.FindAsync(int.Parse(customerId));
            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("CustomerLogin", "Account");
            }

            return View(customer);
        }

        // POST: CustomerDashboard/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id, decimal amount)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(customerId) || userType != "Customer" || id.ToString() != customerId)
            {
                return RedirectToAction("CustomerLogin", "Account");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Çekilecek tutar 0'dan büyük olmalıdır!";
                return View(customer);
            }

            if (amount > customer.Balance)
            {
                TempData["ErrorMessage"] = $"Yetersiz bakiye! Mevcut bakiyeniz: {customer.Balance:N2} ₺";
                return View(customer);
            }

            customer.Balance -= amount;
            
            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{amount:N2} ₺ hesabınızdan çekildi! Kalan bakiyeniz: {customer.Balance:N2} ₺";
                TempData["LastTransaction"] = "true";
                TempData["LastTransactionType"] = "Withdraw";
                TempData["LastTransactionAmount"] = amount.ToString("N2");
                TempData["LastTransactionDescription"] = "Para Çekme";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

