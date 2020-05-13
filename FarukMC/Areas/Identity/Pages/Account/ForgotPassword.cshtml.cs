using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using NETCore.MailKit.Core;
using FarukMC.Areas.Identity.Data;

namespace FarukMC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                //|| !(await _userManager.IsEmailConfirmedAsync(user))) -- Don't check for Confirm since we are not confirming with email
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }


                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailService.SendAsync(
                    Input.Email,
                    "FMC Booking Reset Password",
                    $@"<h1>Hi {user.DisplayName},</h1>
                        <p>You recently requested to reset your password for your FarukMC account. Use the button below to reset it. <strong>This password reset is only valid for the next 24 hours.</strong></p>                                                
                        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' target='_blank'>Reset your password</a>                        
                        <br/>
                        <p>If you’re having trouble with the button above, copy and paste the URL below into your web browser.</p>
                        <p>{HtmlEncoder.Default.Encode(callbackUrl)}</p>                        
                        <p>If you did not request a password reset, please ignore this email.</p>
                        <br/>
                        <p>Thanks,<br/>
                        <a href='https://www.FarukMC.com' target='_blank'>FarukMC</a></p>
                      
    ", true);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
