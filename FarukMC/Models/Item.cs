using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;

namespace FarukMC.Models
{
    public class Item : BaseEntity
    {
        [Key]
        public int Id {get ; set;}
        [DisplayName("Description")]
        public string Description { get; set; }
        public short Quantity { get; set; }
        public List<ItemsBooked> ItemsBooked { get; set; }
    }
}
