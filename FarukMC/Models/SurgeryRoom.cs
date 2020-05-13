using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;

namespace FarukMC.Models
{
    public class SurgeryRoom : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Surgery Room")]
        public string SurgeryRoomDescription { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
