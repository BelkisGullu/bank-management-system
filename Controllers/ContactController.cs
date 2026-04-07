using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystem.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contact - Bize Ulaşın Formu (Halka açık)
        public IActionResult Index()
        {
            return View();
        }

        // POST: Contact/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Name,Email,Phone,Subject,Message,CustomerId")] ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                contactMessage.CreatedAt = DateTime.Now;
                contactMessage.IsRead = false;

                _context.Add(contactMessage);
                await _context.SaveChangesAsync();

                ViewBag.SuccessMessage = "Mesajınız başarıyla gönderildi! En kısa sürede size dönüş yapacağız.";
                return View(new ContactMessage()); // Formu temizle
            }

            // Eğer müşteri ID'si varsa dropdown'a ekle
            if (contactMessage.CustomerId.HasValue)
            {
                ViewData["CustomerId"] = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name", contactMessage.CustomerId);
            }

            return View(contactMessage);
        }

        // GET: Contact/Messages - Mesaj listesi (Yönetici için)
        public async Task<IActionResult> Messages()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var messages = await _context.ContactMessages
                .Include(m => m.Customer)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View(messages);
        }

        // GET: Contact/Details/5
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

            var message = await _context.ContactMessages
                .Include(m => m.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            // Mesajı okundu olarak işaretle
            if (!message.IsRead)
            {
                message.IsRead = true;
                _context.Update(message);
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        // POST: Contact/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                message.IsRead = true;
                _context.Update(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Messages));
        }

        // POST: Contact/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Messages));
        }
    }
}



