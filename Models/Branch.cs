using System.ComponentModel.DataAnnotations;

namespace BankManagementSystem.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Şube adı zorunludur")]
        [Display(Name = "Şube Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres zorunludur")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Display(Name = "Telefon")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; } = string.Empty;

        // Navigation Properties - İlişkiler
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}



