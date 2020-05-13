using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using FarukMC.Models;
using Newtonsoft.Json;

namespace FarukMC.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; }
        public string JsonResult { get; set; }
        

    }
}
