using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using FarukMC.Areas.Identity.Data;
using FarukMC.Controllers;

namespace FarukMC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {

        private readonly IWebHostEnvironment _env;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailService emailService,
            IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }


        [TempData] public string StatusMessage { get; set; }

        public class InputModel
        {

            [Required]
            [DataType(DataType.Text, ErrorMessage = "Please enter a User Name")]
            [Display(Name = "UserName")]
            public string DisplayName { get; set; }

            [Required]
            [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid Email Address")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid Arcade Level")]
            public int ArcadeLevel { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid Simulation Level")]
            public int SimulationLevel { get; set; }

            [Required]
            [DataType(DataType.Text, ErrorMessage = "Please select a country")]
            [Display(Name = "Country")]
            public string Country { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);



            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                StatusMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                StatusMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                var userName = info.Principal.FindFirstValue(ClaimTypes.Email);
                var dirPath = "img/UserProfiles";
                if (!System.IO.File.Exists(Path.Combine(dirPath, userName)))
                {
                    if (info.Principal.HasClaim(c => c.Type == "Picture"))
                    {
                        string url = info.Principal.FindFirstValue("Picture");



                        string fileName = userName + ".jpg";


                        using WebClient webClient = new WebClient();
                        webClient.DownloadFile(url, Path.Combine(_env.WebRootPath, dirPath, fileName));
                    }
                }

                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name,
                    info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            if (info.Principal.FindFirstValue(ClaimTypes.Email) != null && user != null)
            {
                return RedirectToPage("./RegisterConfirmation", new { email = user.Email });
            }

            ReturnUrl = returnUrl;
            LoginProvider = info.LoginProvider;

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    DisplayName = info.Principal.FindFirstValue(ClaimTypes.Email)?.Substring(0, info.Principal.FindFirstValue(ClaimTypes.Email).IndexOf('@')),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)

                };
            }
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                StatusMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    DisplayName = Input.DisplayName,
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {

                    result = await _userManager.AddLoginAsync(user, info);

                    if (!user.EmailConfirmed && _userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        if (string.IsNullOrEmpty(info.Principal.FindFirstValue(ClaimTypes.Email)))
                        {
                            await _emailService.SendAsync(user.Email, "Confirm your email",
                                $"Welcome to VR Tournament Club.<br/>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                            var props = new AuthenticationProperties();
                            props.StoreTokens(info.AuthenticationTokens);
                            props.IsPersistent = true;

                            return RedirectToPage("./RegisterConfirmation", new { email = user.Email });
                        }
                        else
                        {
                            await _userManager.ConfirmEmailAsync(user, code);
                        }

                    }



                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);




                        var props = new AuthenticationProperties();
                        props.StoreTokens(info.AuthenticationTokens);
                        props.IsPersistent = true;


                        await _signInManager.SignInAsync(user, props);

                        RedirectToAction("Index", "Home");

                    }
                }
                else
                {
                    StatusMessage = result.Errors.FirstOrDefault()?.Description;

                }

                if (!string.IsNullOrEmpty(StatusMessage))
                {
                    ModelState.AddModelError(string.Empty, StatusMessage);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}

