using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagementSystem.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefon")]
        [StringLength(10, ErrorMessage = "Telefon numarası 10 haneli olmalıdır")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Konu zorunludur")]
        [Display(Name = "Konu")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [Display(Name = "Mesaj")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Gönderim Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Okundu mu?")]
        public bool IsRead { get; set; } = false;

        // Foreign Key - Customer ile ilişki (opsiyonel - müşteri değilse null olabilir)
        [Display(Name = "Müşteri")]
        public int? CustomerId { get; set; }

        // Navigation Property - Customer ile ilişki
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}



