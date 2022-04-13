using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.CustomTagHelpers
{


    [HtmlTargetElement("td",Attributes ="user-roles")]//hem td si olan hem de user-roles olan tagi yakalamak icin 
    public class UserRolesName:TagHelper
    {
        public UserManager<AppUser> UserManager { get;set;}

        public UserRolesName(UserManager<AppUser> userManager)
        {
            this.UserManager = userManager;
        }
        [HtmlAttributeName("user-roles")]//Users.cshtml icerisin di ser-roles icerisinde ki id yi alttaki prop la denklestirecek
        public string UserId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context,  TagHelperOutput output)//genelde userManager ile asyc kullaniyoruz . Bu methodun yaptigi islem outputta olani gidiyor users taki user-roles olan yerin icine aktariyor
        {
            AppUser appUser= await UserManager.FindByIdAsync(UserId);

            IList<string> roles= await UserManager.GetRolesAsync(appUser);

            string html=string.Empty;

            roles.ToList().ForEach(x => 
            {
                html+=$"<span class='btn btn-info'> {x} </span>";
            
            });
            output.Content.SetHtmlContent(html);




        }





    }
}
    