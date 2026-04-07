using BankManagementSystem.Data;
using BankManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BankManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            GenerateCaptcha();
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, int captchaAnswer)
        {
            // Captcha kontrolü
            int? storedCaptcha = HttpContext.Session.GetInt32("CaptchaResult");
            if (!storedCaptcha.HasValue || storedCaptcha.Value != captchaAnswer)
            {
                ViewBag.ErrorMessage = "Güvenlik sorusunu yanlış cevapladınız!";
                GenerateCaptcha();
                return View();
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı ve şifre gereklidir!";
                GenerateCaptcha();
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
                GenerateCaptcha();
                return View();
            }

            // Session'a kullanıcı bilgisi kaydet
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserType", "Admin");

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Account/CustomerLogin
        public IActionResult CustomerLogin()
        {
            GenerateCaptcha();
            return View();
        }

        // POST: Account/CustomerLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerLogin(string login, string password, int captchaAnswer)
        {
            // Captcha kontrolü
            int? storedCaptcha = HttpContext.Session.GetInt32("CaptchaResult");
            if (!storedCaptcha.HasValue || storedCaptcha.Value != captchaAnswer)
            {
                ViewBag.ErrorMessage = "Güvenlik sorusunu yanlış cevapladınız!";
                GenerateCaptcha();
                return View();
            }

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "TC/Email ve şifre gereklidir!";
                GenerateCaptcha();
                return View();
            }

            // TC veya Email ile müşteri ara
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => (c.TC == login || c.Email == login) && c.Password == password);

            if (customer == null || string.IsNullOrEmpty(customer.Password))
            {
                ViewBag.ErrorMessage = "TC/Email veya şifre hatalı!";
                GenerateCaptcha();
                return View();
            }

            // Session'a müşteri bilgisi kaydet
            HttpContext.Session.SetString("CustomerId", customer.Id.ToString());
            HttpContext.Session.SetString("CustomerName", customer.Name);
            HttpContext.Session.SetString("UserType", "Customer");

            return RedirectToAction("Index", "CustomerDashboard");
        }

        // GET: Account/CustomerRegister
        public IActionResult CustomerRegister()
        {
            GenerateRegCaptcha();
            return View();
        }

        // POST: Account/CustomerRegister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerRegister(string name, string tc, string phone, string email, string password, string passwordConfirm, int regCaptchaAnswer)
        {
            // Captcha kontrolü
            int? storedCaptcha = HttpContext.Session.GetInt32("RegCaptchaResult");
            if (!storedCaptcha.HasValue || storedCaptcha.Value != regCaptchaAnswer)
            {
                ViewBag.ErrorMessage = "Güvenlik sorusunu yanlış cevapladınız!";
                GenerateRegCaptcha();
                return View();
            }

            // Şifre kontrolü
            if (password != passwordConfirm)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor!";
                GenerateRegCaptcha();
                return View();
            }

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                ViewBag.ErrorMessage = "Şifre en az 8 karakter olmalıdır!";
                GenerateRegCaptcha();
                return View();
            }

            // TC kontrolü
            if (await _context.Customers.AnyAsync(c => c.TC == tc))
            {
                ViewBag.ErrorMessage = "Bu TC Kimlik No ile zaten kayıt var!";
                GenerateRegCaptcha();
                return View();
            }

            // Email kontrolü
            if (await _context.Customers.AnyAsync(c => c.Email == email))
            {
                ViewBag.ErrorMessage = "Bu Email ile zaten kayıt var!";
                GenerateRegCaptcha();
                return View();
            }

            // Hesap numarası oluştur (12 haneli)
            string accountNumber;
            Random rnd = new Random();
            do
            {
                accountNumber = "";
                for (int i = 0; i < 12; i++)
                {
                    accountNumber += rnd.Next(0, 10).ToString();
                }
            } while (await _context.Customers.AnyAsync(c => c.AccountNumber == accountNumber));

            // İlk şubeyi al (varsayılan)
            var firstBranch = await _context.Branches.FirstOrDefaultAsync();
            if (firstBranch == null)
            {
                ViewBag.ErrorMessage = "Sistemde şube bulunamadı! Lütfen yönetici ile iletişime geçin.";
                GenerateRegCaptcha();
                return View();
            }

            // İlk müşteri temsilcisini al
            var firstRep = await _context.Employees
                .Where(e => e.BranchId == firstBranch.Id && e.Position == "Müşteri Temsilcisi")
                .FirstOrDefaultAsync();

            if (firstRep == null)
            {
                ViewBag.ErrorMessage = "Müşteri temsilcisi bulunamadı! Lütfen yönetici ile iletişime geçin.";
                GenerateRegCaptcha();
                return View();
            }

            // Yeni müşteri oluştur
            var customer = new Customer
            {
                Name = name,
                TC = tc,
                Phone = phone,
                Email = email,
                Password = password,
                AccountNumber = accountNumber,
                Balance = 0,
                BranchId = firstBranch.Id,
                RepresentativeId = firstRep.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...";
            GenerateRegCaptcha();

            // 2 saniye sonra login sayfasına yönlendir
            Response.Headers["Refresh"] = "2;url=/Account/CustomerLogin";
            return View();
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            GenerateRegCaptcha();
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string regUsername, string regPassword, string regPasswordConfirm, int regCaptchaAnswer)
        {
            // Captcha kontrolü
            int? storedCaptcha = HttpContext.Session.GetInt32("RegCaptchaResult");
            if (!storedCaptcha.HasValue || storedCaptcha.Value != regCaptchaAnswer)
            {
                ViewBag.ErrorMessage = "Güvenlik sorusunu yanlış cevapladınız!";
                GenerateRegCaptcha();
                return View();
            }

            // Şifre kontrolü
            if (regPassword != regPasswordConfirm)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor!";
                GenerateRegCaptcha();
                return View();
            }

            // Şifre kuralları kontrolü
            var passwordErrors = ValidatePassword(regPassword);
            if (passwordErrors.Any())
            {
                ViewBag.ErrorMessage = "Şifre kurallarına uymalıdır: " + string.Join(", ", passwordErrors);
                GenerateRegCaptcha();
                return View();
            }

            // Kullanıcı adı kontrolü
            if (await _context.Users.AnyAsync(u => u.Username == regUsername))
            {
                ViewBag.ErrorMessage = "Bu kullanıcı adı zaten kullanılıyor!";
                GenerateRegCaptcha();
                return View();
            }

            // Yeni kullanıcı oluştur
            var user = new User
            {
                Username = regUsername,
                Password = regPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...";
            GenerateRegCaptcha();

            // 2 saniye sonra login sayfasına yönlendir
            Response.Headers["Refresh"] = "2;url=/Account/Login";
            return View();
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            var userType = HttpContext.Session.GetString("UserType");
            HttpContext.Session.Clear();
            
            if (userType == "Customer")
            {
                return RedirectToAction("CustomerLogin");
            }
            return RedirectToAction("Login");
        }

        // Helper: Şifre doğrulama
        private List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (password.Length < 8)
                errors.Add("En az 8 karakter");
            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("En az 1 büyük harf");
            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("En az 1 küçük harf");
            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("En az 1 sayı");
            if (!Regex.IsMatch(password, @"[.!@#$%^&*()_+\-=\[\]{}|;:,<>?]"))
                errors.Add("En az 1 özel karakter");

            return errors;
        }

        // Helper: Captcha oluştur
        private void GenerateCaptcha()
        {
            Random rnd = new Random();
            int num1 = rnd.Next(1, 11);
            int num2 = rnd.Next(1, 11);
            HttpContext.Session.SetInt32("CaptchaNum1", num1);
            HttpContext.Session.SetInt32("CaptchaNum2", num2);
            HttpContext.Session.SetInt32("CaptchaResult", num1 + num2);
            ViewBag.CaptchaNum1 = num1;
            ViewBag.CaptchaNum2 = num2;
        }

        // Helper: Register Captcha oluştur
        private void GenerateRegCaptcha()
        {
            Random rnd = new Random();
            int num1 = rnd.Next(1, 11);
            int num2 = rnd.Next(1, 11);
            HttpContext.Session.SetInt32("RegCaptchaNum1", num1);
            HttpContext.Session.SetInt32("RegCaptchaNum2", num2);
            HttpContext.Session.SetInt32("RegCaptchaResult", num1 + num2);
            ViewBag.RegCaptchaNum1 = num1;
            ViewBag.RegCaptchaNum2 = num2;
        }

    }
}

