using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting.Internal;
using FarukMC.Areas.Identity.Data;

namespace FarukMC.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Text, ErrorMessage = "Please enter a User Name")]
            [Display(Name = "Name")]
            public string DisplayName { get; set; }

       
            public IFormFile Photo { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            //var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //var arcadeLevel = user.ArcadeLevel;
            //var simulationLevel = user.SimulationLevel;
            var displayName = user.DisplayName;
            //var country = user.Country;

            //Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                //SimulationLevel = simulationLevel,
                //ArcadeLevel =  arcadeLevel,
                DisplayName = displayName,
                //Country = country
                
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (Input.Photo != null)
            {
                string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img/UserProFiles");
                string filepath = Path.Combine(uploadFolder, user.UserName + ".jpg");
                await using var stream = new FileStream(filepath,FileMode.Create);
                await Input.Photo.CopyToAsync(stream);
            }

            //user.ArcadeLevel = Input.ArcadeLevel;
            //user.SimulationLevel = Input.SimulationLevel;
            
            //user.Country = Input.Country;
            
            user.DisplayName = Input.DisplayName;
            await _userManager.UpdateAsync(user);

            

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
