using Microsoft.AspNetCore.Identity;

namespace WebApplication3.CustomValidation
{
    public class CustomIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError() { Code = "InvalidUserName", Description = $"Bu {userName} gecersizdir." };
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError() { Code = "DuplicateEmail", Description = $"Bu {email} kullanilmaktadir" };
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError() { Code = "PasswordTooShort", Description = $"Sifreniz en az {length} karakterli olmalidir. " };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError() { Code="DuplicateUserName",Description=$"Bu {userName} zaten kullanilmaktadir." };
        }
    }
}
