using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage ="Role ismi gereklidir.")]
        [Display(Name = "Role ismi")]
        public string Name { get; set; }

        public string Id { get; set; }//backendde kullanacagimizdan dolayi burayi isaretlemiyoruz birseyle


    }
}
