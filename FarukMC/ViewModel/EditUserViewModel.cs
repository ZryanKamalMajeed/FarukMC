using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FarukMC.ViewModel
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string DisplayName { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }
        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
}
