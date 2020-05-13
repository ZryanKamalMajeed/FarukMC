using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using FarukMC.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using FarukMC.Areas.Identity.Data;
using FarukMC.Models;


namespace FarukMC
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Tokens.ProviderMap.Add("CustomEmailConfirmation",
                    new TokenProviderDescriptor(
                        typeof(CustomEmailConfirmationTokenProvider<ApplicationUser>)));

                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<CustomEmailConfirmationTokenProvider<ApplicationUser>>();

            var mvcBuilder = services.AddControllersWithViews(config =>
            {
                // using Microsoft.AspNetCore.Mvc.Authorization;
                // using Microsoft.AspNetCore.Authorization;
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            if (_env.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();

            //Add MailKit
            services.AddMailKit(optionBuilder =>
            {
                optionBuilder.UseMailKit(new MailKitOptions()
                {
                    //get options from sercets.json
                    Server = Configuration["MailServer:Server"],
                    Port = Convert.ToInt32(Configuration["MailServer:Port"]),
                    SenderName = Configuration["MailServer:SenderName"],
                    SenderEmail = Configuration["MailServer:SenderEmail"],

                    // can be optional with no authentication 
                    Account = Configuration["MailServer:Account"],
                    Password = Configuration["MailServer:Password"],
                    // enable ssl or tls
                    Security = true
                });
            });

            services.Configure<DataProtectionTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromHours(3));

            //services.AddAuthentication().AddFacebook(facebookoptions =>
            //{
            //    facebookoptions.AppId = Configuration["Authentication:Facebook:AppID"];
            //    facebookoptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //    facebookoptions.Scope.Add("public_profile");
            //    facebookoptions.Events = new OAuthEvents
            //    {
            //        OnCreatingTicket = context =>
            //        {
            //            var picture = $"https://graph.facebook.com/{context.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}/picture?type=large";
            //            context.Identity.AddClaim(new Claim("Picture", picture));
            //            return Task.CompletedTask;
            //        }
            //    };
            //    facebookoptions.SaveTokens = true;
            //});

            services.AddScoped<IBookingRepository, SQLBookingRepository>();
            services.AddRazorPages();

        }

        public class CustomEmailConfirmationTokenProvider<TUser>
            : DataProtectorTokenProvider<TUser> where TUser : class
        {
            public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                IOptions<EmailConfirmationTokenProviderOptions> options,
                ILogger<DataProtectorTokenProvider<TUser>> logger)
                : base(dataProtectionProvider, options, logger)
            {

            }
        }
        public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
        {
            public EmailConfirmationTokenProviderOptions()
            {
                Name = "EmailDataProtectorTokenProvider";
                TokenLifespan = TimeSpan.FromHours(4);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                MyIdentityDataInitializer.SeedData(userManager, roleManager);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                MyIdentityDataInitializer.SeedData(userManager, roleManager);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
