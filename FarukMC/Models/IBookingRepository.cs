using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace FarukMC.Models
{
    public interface IBookingRepository
    {
        Booking GetBooking(int id);
        IEnumerable<Booking> GetAllBookings();
        Booking Add(Booking booking);
        Booking Delete(int id);
        Booking Update(Booking booking);
    }
}
