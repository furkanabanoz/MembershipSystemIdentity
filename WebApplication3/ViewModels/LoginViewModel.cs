using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage ="Email alani gereklidir")]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email Adresiniz")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Sifre alani gereklidir")]
        [DataType(DataType.Password)]
        [Display (Name ="Sifreniz")]
        [MinLength(4,ErrorMessage ="Sifreniz en az 4 karakterli olamalidir")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
