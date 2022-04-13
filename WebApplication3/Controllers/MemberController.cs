using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using Mapster;
using WebApplication3.ViewModels;
using Mapster;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using WebApplication3.Enums;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager):base(userManager,signInManager)
        {
        }
        public IActionResult Index()
        { 

            AppUser appUser = CurrentUser;
            UserViewModel userViewModel = appUser.Adapt<UserViewModel>();


            return View(userViewModel);
        }

        public IActionResult UserEdit() 
        {
            AppUser user = CurrentUser;

            UserViewModel userViewModel=user.Adapt<UserViewModel>();

            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender))); //


            return View(userViewModel); 
        }
        [HttpPost]
        public  async Task<IActionResult> UserEdit(UserViewModel userViewModel, IFormFile userPicture)
        {
            ModelState.Remove("Password");//userviewmodel ile gelen passwordu cikartiyoruz cunku edituser da password alani yok ve bu yuzden isvalid in icerisine girmiyor.
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));//yukarida belirtmistik ama burada da belirtmemiz gerekiyor bunu enumdan aldigimizdan dolayi burasi dolsun
            if (ModelState.IsValid)
            {

                AppUser user = CurrentUser;

                if (userPicture != null && userPicture.Length>0)
                {
                    var fileName=Guid.NewGuid().ToString()+Path.GetExtension(userPicture.FileName);//fileName olusturduk,GetExtension bizim methotta verdigimiz userPicture uzantisini alacak
                    var path=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/userPicture", fileName);//path wwwroot icerisine gidip userPicture var mi yokmu kontrol edecek yoksa alltaki koda stream edecek
                    using(var stream =new FileStream(path, FileMode.Create)) 
                    {
                        await userPicture.CopyToAsync(stream);//gelen userPictureyi stream a kopyaliyoruz
                        user.Picture = "/UserPicture/" + fileName;//kaydedildikten sonra userpicture+filename adinda veritabanina kaydini gerceklestirecek
                    }
                //statik dosyalarin hepsi wwwroot icerisinde olmasi gerekiyor!!
                }
                user.City =userViewModel.City;
                user.BirthDay =userViewModel.BirthDay;
                user.Gender = (int)userViewModel.Gender;//int e cast etmemiz gerekiyor
                


                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber=userViewModel.PhoneNumber;
                

                IdentityResult result= await UserManager.UpdateAsync(user);

                if (result.Succeeded) 
                {
                    await UserManager.UpdateSecurityStampAsync(user);
                    await SignInManager.SignOutAsync();
                    await SignInManager.SignInAsync(user,true );
                    ViewBag.success = "true";


                }
                else
                {
                    AddModelError(result);
                }
            }
            return View(userViewModel);
        }

        public IActionResult PasswordChange() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = CurrentUser;
                if (user != null)
                {
                    bool exits = UserManager.CheckPasswordAsync(user,passwordChangeViewModel.PasswordOld).Result;
                    if (exits)
                    {

                        IdentityResult result = UserManager.ChangePasswordAsync(user, passwordChangeViewModel.PasswordOld,passwordChangeViewModel.PasswordNew).Result;

                        if (result.Succeeded)
                        {
                            UserManager.UpdateSecurityStampAsync(user);
                            SignInManager.SignOutAsync();
                            SignInManager.PasswordSignInAsync(user,passwordChangeViewModel.PasswordNew,true,false);//kullaniciyahissettirmeden cikis ve giris islemi yaptiracak
                            ViewBag.success = "true";
                             

                        }
                        else
                        {
                            AddModelError(result);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Eski sifrenizi yanlis girdiniz");
                    }

                }



            }

            return View(passwordChangeViewModel);
        }

        public void Logout()
        {
            SignInManager.SignOutAsync();//burada cikacagimizi belirtiyoruz ancak startup.cs icerisinde logout yerinde nerenden cikaricagimizi gosteriyoruzu ve memberlayout.cshtml icerisinde asp-route-returnUrl te nereye gidecegini gosteriyoruz
        }

        public IActionResult AccessDenied(string returnUrl) 
        {
            if (returnUrl.Contains("ViolancePage"))
            {
                ViewBag.message = "Erismeye calistiginiz sayfa siddet videolari icerdiginden dolayi 15 yasindan buyuk olmaniz gerekmektedir.";
            }
            else if (returnUrl.Contains("GiresunPage"))
            {
                ViewBag.message = "Erismeye calistiginiz sayfa sadece Giresunlular icindir.";

            }
            else if (returnUrl.Contains("ExchangePage"))
            {
                ViewBag.message = "30 gunluk ucretsiz erisim sureniz dolmustur.";
            }
            else
            {
                ViewBag.message = "BU SAYFAYA ERISIM IZNINIZ YOKTUR. ERISIM IZNI ALMAK ICIN SITE YONETICISI ILE GORUSUNUZ.";
            }





            return View(); 
        }
        [Authorize(Roles ="Editor,Admin")]//sadece editorun girebilecegini belirtiyoruz ve kisitliyoruz
        public IActionResult Editor() 
        {
            return View(); 
        }

        [Authorize(Roles = "Manager,Admin")]//sadece editorun girebilecegini belirtiyoruz ve kisitliyoruz
        public IActionResult Manager()
        {
            return View();
        }
        [Authorize(Policy ="GiresunPolicy")]
        public IActionResult GiresunPage() 
        {
            return View();   
        }

        [Authorize(Policy = "ViolancePolicy")]
        public IActionResult ViolancePage()
        {
            return View(); 
        }


        public async Task<IActionResult> ExchangePageRedirect()
        {

            bool result = User.HasClaim(x => x.Type=="ExpireDateExchange");
            if (!result)
            {
                Claim expireDateExchange=new Claim("ExpireDateExchange",DateTime.Now.AddDays(30).Date.ToShortDateString(),ClaimValueTypes.String,"Internal");
                await UserManager.AddClaimAsync(CurrentUser,expireDateExchange);
                await SignInManager.SignOutAsync();
                await SignInManager.SignInAsync(CurrentUser,true);

            }


            return RedirectToAction("ExchangePage");
        }
        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult ExchangePage() 
        {
            return View();
        }
    }
}
