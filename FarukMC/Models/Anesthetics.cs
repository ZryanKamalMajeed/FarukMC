using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;
using Org.BouncyCastle.Crypto;

namespace FarukMC.Models
{
    public class Anesthetics : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Anesthetic Name")]
        public string Name { get; set; }

        public List<Booking> Bookings { get; set; }
    }
}
