﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.CustomValidation
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            foreach (var item in digits)
            {
                if (user.UserName[0].ToString() == item)
                {
                    errors.Add(new IdentityError() { Code = "UserNameContainsFirstLetterDigitsContains", Description = "Kullanici adinin ilk karakteri sayisal karakter iceremez" });
                }

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

