using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.CustomValidation;
using WebApplication3.Models;

namespace WebApplication3
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration) 
        {
            this.Configuration = configuration;
        }
  

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();//her IAuthorizationHandler bu interface ile karsilastigi zaman ExpireDateExchangeHandler a bir tane nesne ornegi olusturuyor 

            services.AddRazorPages();

            services.AddControllers();


            services.AddDbContext<AppIdentityDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:DefaultConnectionString"]);
            });

            services.AddAuthorization(opts =>//claim bazli yapabilmemiz icin bunlari belirtmemiz gerekiyor
            {//istedigimiz kadar kisitlama kodlari yazabiliriz policy icersinde
                opts.AddPolicy("GiresunPolicy", policy =>
                 {
                     policy.RequireClaim("City", "Giresun");
 

                 });
                opts.AddPolicy("ViolancePolicy", policy =>
                 {
                     policy.RequireClaim("Violance");
                 });
                opts.AddPolicy("ExchangePolicy", policy =>
                 {
                     policy.AddRequirements(new ExpireDateExchangeRequirement());

                 });



            });


            services.AddAuthentication().AddFacebook(opts =>
            {
                opts.AppId = Configuration["Authentication:Facebook:AppId"];
                opts.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(opts => 
            {
                opts.ClientId= Configuration["Authentication:Google:ClientID"];
                opts.ClientSecret= Configuration["Authentication:Google:ClientSecret"];

            }).AddMicrosoftAccount(opts =>
            {
                opts.ClientId = Configuration["Authentication:Microsoft:ClientID"];
                opts.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];

            });






            services.AddIdentity<AppUser,AppRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnoçpqrsþtuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";



                opts.Password.RequiredLength = 4;
                opts.Password.RequireNonAlphanumeric = false;//*,! gibi karakterlerin girilmesini istemiyoruz
                opts.Password.RequireDigit = false;//0-9 olan sayilarin girilmesini istemiyoruz
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddPasswordValidator<CustomPasswordValidator>
            ().AddUserValidator<CustomUserValidator>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddDefaultTokenProviders();

            CookieBuilder cookieBuilder = new CookieBuilder();

            cookieBuilder.Name = "MyBlog";
            cookieBuilder.HttpOnly = false;
            cookieBuilder.SameSite = SameSiteMode.Lax;
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = new PathString("/Home/Login");
                opts.LogoutPath = new PathString("/Member/Logout");
                opts.Cookie = cookieBuilder;
                opts.SlidingExpiration = true;
                opts.ExpireTimeSpan = TimeSpan.FromDays(60);
                opts.AccessDeniedPath = new PathString("/Member/AccessDenied");//eger kullanici uye olduktan sonra kendi rolunden ustte olan bir rolun erisebilecegi bir linke tiklarsa bu sayfaya erisemeyecegi ile ilgili bilgi verilmesi lazim iste bu bilgiyi pathte gosteriyoruz.
                //yukarida ki icin eger rolunun ustunde bir yere girecekse uye buraya aktarilir
            
            });

            services.AddScoped<IClaimsTransformation, ClaimProvider.ClaimProvider>();



            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
