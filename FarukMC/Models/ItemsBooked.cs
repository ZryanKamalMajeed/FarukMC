using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;

namespace FarukMC.Models
{
    public class ItemsBooked : BaseEntity
    {
        [Key]
        public int Id {get ; set;}
        public int Quantity { get; set; }
        public int BookingId {get;set;}
        public Booking Booking { get; set; }
        public int ItemId {get;set;}
        public Item Item { get; set; }

    }
}
