using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;

namespace FarukMC.Models
{
    public class SMS : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
