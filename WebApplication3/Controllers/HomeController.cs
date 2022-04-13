using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.ViewModels;

namespace WebApplication3.Controllers
{
    public class HomeController : BaseController
    {
     
        public HomeController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager):base(userManager,signInManager)
        {
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Member");//bu metod bizi direkt member sayfana gonderecek eger kayitli bir hesabimiz varsa

            }
            return View();
        }

        public IActionResult LogIn(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult>LogIn(LoginViewModel userLogin) 
        {

            if (ModelState.IsValid) 
            {
                AppUser user = await UserManager.FindByEmailAsync(userLogin.Email);
               
                if (user!=null)
                {

                    if (await UserManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabiniz bir sureligine kilitlenmistir. Lutfen daha sonra tekrar deneyiniz");
                        return View(userLogin);
                    }

                    if (UserManager.IsEmailConfirmedAsync(user).Result==false)
                    {

                        ModelState.AddModelError("", "Email adresiniz onaylanmamistir. Lutfen epostanizi kontrol ediniz");
                        return View(userLogin);


                    }


                    await SignInManager.SignOutAsync(); //bizim yazdigimiz bir coookie  varsa bu cookie yi siler 

                    Microsoft.AspNetCore.Identity.SignInResult result= await SignInManager.PasswordSignInAsync(user,userLogin.Password,userLogin.RememberMe,false);

                    if (result.Succeeded)
                    {
                        await UserManager.ResetAccessFailedCountAsync(user);//access count sayisini sifirlar

                        if (TempData["ReturnUrl"] !=null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");   
                    }
                    else
                    {
                        await UserManager.AccessFailedAsync(user);
                        int fail = await UserManager.GetAccessFailedCountAsync(user);

                        ModelState.AddModelError("", $"{fail} kez basarisiz giris.");
                        if (fail==3)
                        {
                            await UserManager.SetLockoutEndDateAsync(user, new System.DateTimeOffset(DateTime.Now.AddMinutes(20)));
                            ModelState.AddModelError("", "Hesabiniz 3 basarisiz giristen dolayi 20 dk sureyle kilitlenmistir. Lutfen daha sonra tekrar deneyiniz.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email adresiniz veya sifreniz yanlis.");

                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bu email adresine kayitli kullanici bulunamamistir.");
                     
                } 
            }   

            return View(userLogin);
        }


        public IActionResult SingUp()
        {
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult>SingUp(UserViewModel userViewModel)
        {

            if (ModelState.IsValid)
            {
                AppUser user=new AppUser();
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;
                IdentityResult result = await UserManager.CreateAsync(user,userViewModel.Password);

                if (result.Succeeded)
                {
                    string confirmationToken= await UserManager.GenerateEmailConfirmationTokenAsync(user);

                    string link=Url.Action("ConfirmEmail","Home",new 
                    {
                        userId=user.Id,
                        token=confirmationToken
                    },protocol:HttpContext.Request.Scheme);  //http mi https mi onu kontrol ediyor

                    Helpers.EmailConfirmation.SendEmail(link,user.Email);


                    return RedirectToAction("Login");
                }
                else
                {
                    AddModelError(result);
                }
            }

            return View(userViewModel);
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(PasswordResetViewModel passwordResetViewModel)
        {

            AppUser appUser = UserManager.FindByEmailAsync(passwordResetViewModel.Email).Result;//result await in yapmis oldugu isin aynisini yapiyor

            if(appUser != null)
            {
                string passwordResetToken=UserManager.GeneratePasswordResetTokenAsync(appUser).Result;
                 
                string passwordResetLink = Url.Action("ResetPasswordConfirm","Home",new
                {
                    userId=appUser.Id,
                    token=passwordResetToken
                },HttpContext.Request.Scheme);//http mi https mi onu kontrol ediyor

                Helpers.PasswordReset.PasswordResetSendEmail(passwordResetLink,appUser.Email); 

                ViewBag.status = "Success";
            }
            else 
            {
                ModelState.AddModelError("", "Sistemde kayitli bir Email adresi bulunamamistir");
            }

            return View(passwordResetViewModel);
        }

        public IActionResult ResetPasswordConfirm(string userId,string token)
        {
            TempData["userId"] = userId;//sayfalar arasi veri tasimak icin kullaniliyor tempdata
            TempData["token"] = token;//


            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")]PasswordResetViewModel passwordResetViewModel)//bind in amaci PasswordResetViewModel in icerisinde istedigimizi cagirmamizdir
        {
            string token = TempData["token"].ToString();
            string userId = TempData["userId"].ToString();

            AppUser appUser = await UserManager.FindByIdAsync(userId);


            if(appUser != null) 
            {
                IdentityResult result = await UserManager.ResetPasswordAsync(appUser,token,passwordResetViewModel.PasswordNew);
                if (result.Succeeded) 
                {
                    await UserManager.UpdateSecurityStampAsync(appUser);
                    ViewBag.status = "Success";


                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir hata meydana gelmistir. Lutfen daha sonra tekrar deneyiniz");
            }
            return View(passwordResetViewModel); 
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token) 
        {

            var user=await UserManager.FindByIdAsync(userId);
            
            IdentityResult result= await UserManager.ConfirmEmailAsync(user,token);

            if (result.Succeeded)
            {
                ViewBag.status = "Email adresiniz onaylanmistir. Login ekranindan giris yapabilirsiniz";
                

            }
            else
            {

                ViewBag.status = "Bir hata meydana geldi. Lutfen daha sonra tekrar deneyiniz.";


            }
            return View();

        }


        public IActionResult FacebookLogin(string ReturnUrl)

        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });

            var properties = SignInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);

            return new ChallengeResult("Facebook", properties);
        }

        public IActionResult GoogleLogin(string ReturnUrl)

        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });

            var properties = SignInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);

            return new ChallengeResult("Google", properties);
        }
        public IActionResult MicrosoftLogin(string ReturnUrl)

        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });

            var properties = SignInManager.ConfigureExternalAuthenticationProperties("Microsoft", RedirectUrl);

            return new ChallengeResult("Microsoft", properties);
        }


        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

                if (result.Succeeded)
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    AppUser user = new AppUser();

                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;

                        userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString();

                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    }

                    IdentityResult createResult = await UserManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        IdentityResult loginResult = await UserManager.AddLoginAsync(user, info);

                        if (loginResult.Succeeded)
                        {
                            // await SignInManager.SignInAsync(user, true);
                            //eger ustteki gibi yaparsak admin sayfasinda claimleri gordugumuz yerde nereden geldigini goremeyiz ama altta ki gibi bir giris yaptirirsak gorebiliriz
                            await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            AddModelError(loginResult);
                        }
                    }
                    else
                    {
                        AddModelError(createResult);
                    }
                }
            }



            List<String> errors =ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();



            return View("Error",errors);
        }

        public IActionResult Error()
        {
            return View(); 
        }
    }
}
 