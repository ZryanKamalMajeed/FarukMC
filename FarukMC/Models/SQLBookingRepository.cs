using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Data;
using Microsoft.EntityFrameworkCore;

namespace FarukMC.Models
{
    public class SQLBookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public SQLBookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Booking GetBooking(int id)
        {
            

                //var booking = _context.Bookings.Find(id);
                //_context.Entry(booking).Reference(booking => booking.AnesthesiaTechnique).Load();
                //_context.Entry(booking).Reference(booking => booking.SurgeryRoom).Load();
                //_context.Entry(booking).Reference(booking => booking.SurgicalDepartment).Load();
                //_context.Entry(booking).Reference(booking => booking.Surgeon).Load();
                //_context.Entry(booking).Collection(booking => booking.ItemsBooked).Load();



                return GetAllBookings().ToList().FirstOrDefault(x=>x.ID == id);


        }

        public IEnumerable<Booking> GetAllBookings()
        {
            return _context.Bookings.AsNoTracking()
                .Include(b => b.AnesthesiaTechnique)
                .Include(b => b.SurgeryRoom)
                .Include(b => b.SurgicalDepartment)
                .Include(b => b.Surgeon)
                .Include(b => b.Anesthetics)
                .Include(b=>b.SMS)

                .Include(b => b.ItemsBooked ).ThenInclude(i=>i.Item);
        }

        public Booking Add(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return booking;
        }

        public Booking Delete(int id)
        {
            Booking booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }

            return booking;
        }

        public Booking Update(Booking bookingChanges)
        {
            var booking = _context.Bookings.Attach(bookingChanges);
            booking.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return bookingChanges;
        }
    }
}
