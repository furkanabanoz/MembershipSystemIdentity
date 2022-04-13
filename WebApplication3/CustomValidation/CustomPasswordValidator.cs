using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.CustomValidation
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                if (!user.Email.Contains(user.UserName))
                {

                    errors.Add(new IdentityError() { Code = "PasswordContainsUserName", Description = "Sifreniz kullanici adinizi iceremez" });

                }

            }

            if (password.ToLower().Contains("1234")) 
            {
                errors.Add(new IdentityError() { Code = "PasswordContains1234", Description = "Sifreniz ardisik sayi olamaz" });
            }
            if (password.ToLower().Contains(user.Email.ToLower()))
            {
                errors.Add(new IdentityError() { Code = "PasswordContainsEmail", Description = "Sifreniz email adresinizi iceremez" });
            }
            if(errors.Count == 0) 
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
