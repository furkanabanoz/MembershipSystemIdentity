using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class BaseController : Controller//kod kalabaliginin onune gecmek icin yapilmis bir base class
    {
        
        protected UserManager<AppUser> UserManager { get; }
        protected SignInManager<AppUser> SignInManager { get; }
        protected RoleManager<AppRole> RoleManager { get; }//role manager
        protected AppUser CurrentUser => UserManager.FindByNameAsync(User.Identity.Name).Result;
        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<AppRole> roleManager=null)//roleManager a null degeri vermemizin sebebi member controllerda ki kodlarimizi patlatmamak amacinda

        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.RoleManager = roleManager;
        }
        public void AddModelError(IdentityResult result) 
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);

            }
        }






}
}
