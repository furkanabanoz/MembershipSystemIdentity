using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.ClaimProvider
{
    public class ClaimProvider : IClaimsTransformation
    {
        public UserManager<AppUser> UserManager { get; set; }

        public ClaimProvider(UserManager<AppUser> userManager) 
        {

            this.UserManager = userManager;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)//bu method identy api cookieden gelen degerleri claim olustururken key value seklinde bir tane de biz eklicez dinamik olarak
        {
            if (principal!=null && principal.Identity.IsAuthenticated)//kullanici uyemi degil mi diye tespit ediyoruz,identity kimlik karti var mi onu tespit ediyoruz
            {
                ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
                AppUser user = await UserManager.FindByNameAsync(identity.Name);

                if (user != null) 
                {
                    if (user.BirthDay!=null)
                    {
                        var today = DateTime.Today;
                        var age = today.Year - user.BirthDay?.Year;

                        if (age>15)
                        {
                            Claim violanceClaim = new Claim("Violance", true.ToString(), ClaimValueTypes.String, "Internal");
                            identity.AddClaim(violanceClaim);
                        }




                    }

                    if (user.City!=null)
                    {
                        if (!principal.HasClaim(c=>c.Type=="City"))
                        {
                            Claim cityClaim = new Claim("City", user.City, ClaimValueTypes.String, "Internal");
                            identity.AddClaim(cityClaim);
                        }

                    }
                }




            }
            return principal;






        }
    }
}
