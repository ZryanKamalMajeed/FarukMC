using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FarukMC.ViewModel
{
    public class CreateBookingViewModel : Booking
    {
        public string BookedItemsList { get; set; }
    }
}
