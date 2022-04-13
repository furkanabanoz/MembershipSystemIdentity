using System;
using System.ComponentModel.DataAnnotations;
using WebApplication3.Enums;

namespace WebApplication3.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage ="Kullanici ismi gereklidir.")]
        [Display(Name ="Kullanici adi")]
        public string UserName { get; set; }
        
        [Display(Name ="Telefon Numarasi")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="Email adresi gereklidir.")]
        [Display (Name ="Email Adresi")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Email adresiniz dogru formatta degil")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Sifreniz gereklidir.")]
        [Display(Name = "Sifreniz")]
        [DataType (DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Sehir adi")]
        public string City { get; set; }

        [Display(Name = "Fotografiniz")]
        public string Picture { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Dogum gununuz")]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "Cinsiyetiniz")]
        public Gender Gender { get; set; }


    }
}
