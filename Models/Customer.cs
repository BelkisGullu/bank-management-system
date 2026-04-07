using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagementSystem.Models
{
    public class Customer
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

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Display(Name = "Telefon")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Telefon numarası 10 haneli olmalıdır")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası sadece rakamlardan oluşmalıdır")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hesap Numarası zorunludur")]
        [Display(Name = "Hesap Numarası")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Hesap numarası 12 haneli olmalıdır")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hesap Bakiyesi zorunludur")]
        [Display(Name = "Hesap Bakiyesi")]
        [Range(0, double.MaxValue, ErrorMessage = "Bakiye 0 veya daha büyük olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Display(Name = "Kredi Kartı No")]
        [StringLength(16, ErrorMessage = "Kredi kartı numarası en fazla 16 haneli olabilir")]
        public string? CreditCard { get; set; }

        [Display(Name = "Kredi Limiti")]
        [Range(0, double.MaxValue, ErrorMessage = "Kredi limiti 0 veya daha büyük olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditLimit { get; set; }

        [Display(Name = "Kullanılan Kredi")]
        [Range(0, double.MaxValue, ErrorMessage = "Kullanılan kredi 0 veya daha büyük olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UsedCredit { get; set; }

        // Foreign Key - Branch ile ilişki
        [Required(ErrorMessage = "Şube seçimi zorunludur")]
        [Display(Name = "Şube")]
        public int BranchId { get; set; }

        // Navigation Property - Branch ile ilişki
        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        // Foreign Key - Employee (Müşteri Temsilcisi) ile ilişki
        [Required(ErrorMessage = "Müşteri Temsilcisi seçimi zorunludur")]
        [Display(Name = "Müşteri Temsilcisi")]
        public int RepresentativeId { get; set; }

        // Navigation Property - Employee ile ilişki
        [ForeignKey("RepresentativeId")]
        public virtual Employee? Representative { get; set; }

        // Navigation Property - Bu müşterinin mesajları
        public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();

        // Müşteri Giriş Bilgileri
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre en az 8 karakter olmalıdır")]
        public string? Password { get; set; }
    }
}



