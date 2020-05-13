using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace FarukMC.Data
{
    public class BaseEntity
    {
        [DisplayName("Date Created")]
        public DateTime? CreatedDate { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? ModifiedDate { get; set; }
 
        [Column("Created By")]
        [Display(Name = "Creator")]
        public string CreatedBy { get; set; }
 
        [Column("Modified By")]
        [Display(Name = "Modifier")]
        public string ModifiedBy { get; set; }   
    }
    public static class MyIdentityDataInitializer
    {
        public static void SeedData
        (UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers
            (UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync
                    ("Admin@fmcbooking.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Admin@fmcbooking.com";
                user.Email = "Admin@fmcbooking.com";                
                user.DisplayName = "Admin";
                user.Active = true;

                IdentityResult result = userManager.CreateAsync
                    (user, "Ankara2020!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,
                        "Administrator").Wait();
                }
            }

        }

        public static void SeedRoles
            (RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync
                ("Administrator").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Administrator";
                IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync
                ("Surgeon").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Surgeon";
                IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync
                ("StatusUpdate").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "StatusUpdate";
                IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
            }
        }
    }
}
