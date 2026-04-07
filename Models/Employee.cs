using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [Display(Name = "TC Kimlik No")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır")]
        public string TC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pozisyon zorunludur")]
        [Display(Name = "Pozisyon")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Display(Name = "Telefon")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Telefon numarası 10 haneli olmalıdır")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası sadece rakamlardan oluşmalıdır")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        // Foreign Key - Branch ile ilişki
        [Required(ErrorMessage = "Şube seçimi zorunludur")]
        [Display(Name = "Şube")]
        public int BranchId { get; set; }

        // Navigation Property - Branch ile ilişki
        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        // Navigation Property - Bu çalışanın müşterileri
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}



