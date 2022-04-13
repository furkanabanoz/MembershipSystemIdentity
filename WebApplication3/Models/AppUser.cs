using Microsoft.AspNetCore.Identity;
using System;

namespace WebApplication3.Models

{
    public class AppUser:IdentityUser//buraya kendi istedigimiz db de ki sutunlari yazacagiz
    {

        public string City { get; set; }
        public string Picture { get; set; }
        public DateTime? BirthDay { get; set; }//? isaretinin amaci null olabilecek bir datetime olabilecegini gostermektir.
        public int Gender { get; set; }

    }
}
