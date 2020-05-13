using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;

namespace FarukMC.Models
{
    public class AnesthesiaTechnique : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
