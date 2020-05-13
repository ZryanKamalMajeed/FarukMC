using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FarukMC.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        
            public string DisplayName { get; set; }

            public bool Active { get; set; }


    }
}
