using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication3.Models;
using WebApplication3.ViewModels;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Controllers
{

    [Authorize(Roles ="Admin")]//sadece admin girebilir
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager, null, roleManager)
        {
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Claims() 
        {
            return View(User.Claims.ToList()); 
        }
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            AppRole appRole = new AppRole();
            appRole.Name = roleViewModel.Name;
            IdentityResult result = RoleManager.CreateAsync(appRole).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            else
            {
                AddModelError(result);
            }

            return View(roleViewModel);
        }
        public IActionResult Roles()
        {
            return View(RoleManager.Roles.ToList());
        }
        public IActionResult Users()
        {
            return View(UserManager.Users.ToList());
        }
        public IActionResult RoleDelete(string id)
        {
            AppRole role = RoleManager.FindByIdAsync(id).Result;
            if (role != null)
            {
                IdentityResult result = RoleManager.DeleteAsync(role).Result;

            }
            return RedirectToAction("Roles");
        }
        public IActionResult RoleUpdate(string id) 
        {

            AppRole role=RoleManager.FindByIdAsync(id).Result;
            if (role!=null)
            {
                return View(role.Adapt<RoleViewModel>());
                
            }
            return RedirectToAction("Roles");

        }

        [HttpPost]
        public IActionResult RoleUpdate(RoleViewModel roleViewModel) 
        {
            AppRole appRole = RoleManager.FindByIdAsync(roleViewModel.Id).Result;
            if (appRole!=null)
            {
                appRole.Name = roleViewModel.Name;
                IdentityResult result=RoleManager.UpdateAsync(appRole).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");

                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Guncelleme islemi basarisiz oldu.");
            }
            return View(roleViewModel);
        }
        public IActionResult RoleAssign(string id)
        {
            TempData["userId"] = id;

            AppUser user = UserManager.FindByIdAsync(id).Result;

            ViewBag.userName=user.UserName;

            IQueryable<AppRole> roles = RoleManager.Roles;

            List<string> userroles = UserManager.GetRolesAsync(user).Result as List<string>;//as List<string> demek cast etmek demek



            List<RoleAssignViewModel> roleAssignViewModel = new List<RoleAssignViewModel>();
            foreach (var role in roles) 
            {

                RoleAssignViewModel roleAssign = new RoleAssignViewModel();
                roleAssign.RoleId = role.Id;
                roleAssign.RoleName = role.Name; 
                if (userroles.Contains(role.Name)) 
                {

                    roleAssign.IsExist = true;
                }
                else 
                {

                    roleAssign.IsExist = false;
                }


                roleAssignViewModel.Add(roleAssign);
            }
            
            
            
            return View(roleAssignViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> roleAssignViewModels) 
        {
            AppUser appUser = UserManager.FindByIdAsync(TempData["userId"].ToString()).Result;

            foreach (var item in roleAssignViewModels)
            {
                if (item.IsExist)
                {
                    await UserManager.AddToRoleAsync(appUser, item.RoleName);

                }
                else
                {
                    await UserManager.RemoveFromRoleAsync(appUser, item.RoleName);
                }

            }
            return RedirectToAction("Users");


        }
    }

}

