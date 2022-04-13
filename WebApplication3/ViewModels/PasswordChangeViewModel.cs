using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage ="Eski sifreniz gereklidir" )]
        [Display(Name ="Eski sifreniz")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Sifreniz en az 4 karakterlidir")]
        public string PasswordOld { get; set; }
        [Required(ErrorMessage = "Yeni sifreniz gereklidir")]
        [Display(Name = "Yeni sifreniz")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Sifreniz en az 4 karakterli olmalidir")]
        public string PasswordNew { get; set; }
        [Required(ErrorMessage = "Yeni sifrenizin dogrulamasi icin gereklidir")]
        [Display(Name = "Yeni sifrenizin tekrari")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Sifreniz en az 4 karakterli olmalidir")]
        [Compare("PasswordNew",ErrorMessage ="Yeni sifreniz ile onay sifreniz birbirinden farklidir.")]
        public string PasswordConfirm { get; set; }
    }
}
