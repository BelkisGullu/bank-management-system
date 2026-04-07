using System.ComponentModel.DataAnnotations;

namespace BankManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;
    }
}



