using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FarukMC.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FarukMC.Data;
using FarukMC.Models;
using FarukMC.ViewModel;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FarukMC.Controllers
{
    [Authorize(Roles = "Administrator,Surgeon")]
     public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingRepository _bookingRepository;
        private readonly IConfiguration _configuration;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IBookingRepository _bookingRepository, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            this._bookingRepository = _bookingRepository;
            _configuration = configuration;
        }


        // GET: Booking
        public async Task<IActionResult> Index()
        {
            return View(_bookingRepository.GetAllBookings());
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = _bookingRepository.GetBooking((int)id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AnesthesiaTechniqueId"] = new SelectList(_context.AnesthesiaTechnique, "Id", "Text");
            ViewData["Rooms"] = JsonConvert.SerializeObject(_context.SurgeryRoom.OrderBy(x => x.SurgeryRoomDescription).Select(x => new
            {
                id = x.Id,
                description = x.SurgeryRoomDescription.Trim()
            }));
            ViewData["Appointments"] = JsonConvert.SerializeObject(_bookingRepository.GetAllBookings().Select(x => new
            {
                id = x.ID,
                startTime = x.SurgeryStartingTime,
                endTime = x.SurgeryEndingTime,
                roomId = x.SurgeryRoomId,
                itemsBooked = x.ItemsBooked?.Where(p => p.Quantity > 0)
                .SelectMany(p => Enumerable.Range(0, p.Quantity)
                    .Select(i => new
                    {
                        itemId = p.ItemId,
                        quantity = p.Quantity
                    }))
            }));


            ViewData["SurgicalDepartmentId"] = new SelectList(_context.SurgicalDepartment, "Id", "SurgicalDepartmentDescription");

            var items = _context.Item.ToArray();
            var listItems = items.Select(i => new { id = i.Id, description = i.Description }).Distinct().OrderBy(x => x.description);
            ViewData["Item"] = JsonConvert.SerializeObject(listItems);

            var userModel = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Surgeon"))
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserID = user.Id,
                        UserName = user.DisplayName,
                    };
                    userModel.Add(userRoleViewModel);
                }
            }

            ViewData["CurrentUserName"] = (await _userManager.FindByIdAsync(_userManager.GetUserId(User)).ConfigureAwait(false)).DisplayName;
            ViewData["SurgeonId"] = new SelectList(userModel, "UserID", "UserName", _userManager.GetUserId(User));

            return View();
        }




        // POST: Booking/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,SurgeryStartingTime,SurgeryEndingTime,SurgeryRoomId,PatientFullName,PatientDateOfBirth,PatientGender,PatientMRNnumber,SurgeryName,SurgerySite,SurgicalDepartmentId,SurgeonID,BloodRequested,RequestedPostOperativeCare,SurgeryPosition,FrozenSection,SpecialThingsLikeSutures,SpecialThingsLikeSuturesText,Consumables,AnesthesiaTechniqueId,SpecialDevices,Turniquet,CArm,Harmonic,Ligasure,Microscope,Others,BookedItemsList,AppointmentStatus,BloodRequestedText,ConsumablesText,SpecialDevicesText,SurgeryPositionText")] CreateBookingViewModel booking)
        {
            if (ModelState.IsValid)
            {
                var itemsBooked = booking.BookedItemsList?.Split(',');
                booking.ItemsBooked = new List<ItemsBooked>();

                if (itemsBooked != null)
                    foreach (var i in itemsBooked)
                    {
                        var booked = new ItemsBooked()
                        {
                            Booking = booking,
                            Item = _context.Item.FirstOrDefault(x => x.Id == int.Parse(i)),
                            Quantity = 1
                        };
                        booking.ItemsBooked.Add(booked);
                    }

                var overlapBookings = _bookingRepository.GetAllBookings().Where(x =>
                    x.SurgeryStartingTime < booking.SurgeryEndingTime &&
                    x.SurgeryEndingTime > booking.SurgeryStartingTime).ToList();

                var overLapRoom = overlapBookings.FirstOrDefault(x => x.SurgeryRoomId == booking.SurgeryRoomId);
                    if (overLapRoom != null)
                    {
                        ModelState.AddModelError("SurgeryRoomId", $"Room {overLapRoom.SurgeryRoom.SurgeryRoomDescription} booked in this hour since someone booked it just now. Please select other time or room.");
                    }

                var totalItems = _context.Item.AsNoTracking().ToList();

                foreach (var i in totalItems.Where(i => overlapBookings.Any(x => x.ItemsBooked.Any(c => c.Item.Id == i.Id))))
                {
                    i.Quantity--;
                }

                foreach (var i in totalItems.Where(x => booking.ItemsBooked.Any(c => c.Item.Id == x.Id && x.Quantity < 1)))
                {
                    ModelState.AddModelError("", $"'{i.Description}' is not available in this time since someone booked it just now. Please select other time.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    _bookingRepository.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["AnesthesiaTechniqueId"] = new SelectList(_context.AnesthesiaTechnique, "Id", "Text", booking.AnesthesiaTechniqueId);
            ViewData["Rooms"] = JsonConvert.SerializeObject(_context.SurgeryRoom.OrderBy(x => x.SurgeryRoomDescription).Select(x => new
            {
                id = x.Id,
                description = x.SurgeryRoomDescription.Trim()
            }));
            ViewData["Appointments"] = JsonConvert.SerializeObject(_bookingRepository.GetAllBookings().Select(x => new
            {
                startTime = x.SurgeryStartingTime,
                endTime = x.SurgeryEndingTime,
                roomId = x.SurgeryRoomId,
                itemsBooked = x.ItemsBooked?.Where(p => p.Quantity > 0)
                .SelectMany(p => Enumerable.Range(0, p.Quantity)
                    .Select(i => new
                    {
                        itemId = p.ItemId,
                        quantity = p.Quantity
                    }))
            }));


            ViewData["SurgicalDepartmentId"] = new SelectList(_context.SurgicalDepartment, "Id", "SurgicalDepartmentDescription", booking.SurgicalDepartmentId);

            var items = _context.Item.AsNoTracking().ToArray();
            var listItems = items.Select(i => new { id = i.Id, description = i.Description }).Distinct().OrderBy(x => x.description);
            ViewData["Item"] = JsonConvert.SerializeObject(listItems);

            var userModel = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Surgeon"))
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserID = user.Id,
                        UserName = user.DisplayName,
                    };
                    userModel.Add(userRoleViewModel);
                }
            }

            ViewData["CurrentUserName"] = (await _userManager.FindByIdAsync(_userManager.GetUserId(User)).ConfigureAwait(false)).DisplayName;
            ViewData["SurgeonId"] = new SelectList(userModel, "UserID", "UserName", booking.SurgeonID);


            return View(booking);
        }



        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking =  _bookingRepository.GetBooking((int)id);
            if (booking == null)
            {
                return NotFound();
            }

            //check if user is allowed to edit
            if (!(User.IsInRole("Administrator") || User.IsInRole("StatusUpdate") || (booking.CreatedBy == User.FindFirst(ClaimTypes.NameIdentifier).Value && booking.AppointmentStatus == AppointmentStatus.Pending)))
            {
                return View("Index");
            }

            booking.ItemsBooked = _context.ItemsBooked.Where(x => x.BookingId == booking.ID).Include(x=>x.Item).AsNoTracking().ToList();

            ViewData["AnesthesiaTechniqueId"] = new SelectList(_context.AnesthesiaTechnique, "Id", "Text", booking.AnesthesiaTechniqueId);
            ViewData["Rooms"] = JsonConvert.SerializeObject(_context.SurgeryRoom.OrderBy(x => x.SurgeryRoomDescription).Select(x => new
            {
                id = x.Id,
                description = x.SurgeryRoomDescription.Trim()
            }));
            var allBookings = _bookingRepository.GetAllBookings();

            ViewData["Appointments"] = JsonConvert.SerializeObject(allBookings.Where(x => x.ID != id).Select(x => new
            {
                startTime = x.SurgeryStartingTime,
                endTime = x.SurgeryEndingTime,
                roomId = x.SurgeryRoomId,
                itemsBooked = x.ItemsBooked?.Where(p => p.Quantity > 0)
                  .SelectMany(p => Enumerable.Range(0, p.Quantity)
                      .Select(i => new
                      {
                          itemId = p.ItemId,
                          quantity = p.Quantity
                      }))
            }));


            ViewData["SurgicalDepartmentId"] = new SelectList(_context.SurgicalDepartment, "Id", "SurgicalDepartmentDescription", booking.SurgicalDepartmentId);

            var items = _context.Item.AsNoTracking().ToArray();
            var listItems = items.Select(i => new { id = i.Id, description = i.Description }).Distinct().OrderBy(x => x.description);
            ViewData["Item"] = JsonConvert.SerializeObject(listItems);

            var userModel = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Surgeon"))
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserID = user.Id,
                        UserName = user.DisplayName,
                    };
                    userModel.Add(userRoleViewModel);
                }
            }



            ViewData["bookedItems"] = JsonConvert.SerializeObject(booking.ItemsBooked?.Select(x => x.Item.Id).ToArray());


            ViewData["CurrentUserName"] = (await _userManager.FindByIdAsync(_userManager.GetUserId(User)).ConfigureAwait(false)).DisplayName;
            ViewData["SurgeonId"] = new SelectList(userModel, "UserID", "UserName", booking.SurgeonID);
            ViewData["Anesthetics"] = new SelectList(_context.Anesthetics, "Id", "Name", booking.AnestheticsId);

            if(booking.SMS != null)
                ViewData["SMS"] = JsonConvert.SerializeObject(booking.SMS.Select(x=> new
                {
                    phoneNumber = x.PhoneNumber,
                    message = x.Message,
                    sentBy = _userManager.FindByIdAsync(x.CreatedBy).Result.DisplayName,
                    sentOn = ((DateTime) x.CreatedDate).ToString("dd/MM/yyyy")

                }));

            var bookingModel = new CreateBookingViewModel()
            {
                ID = booking.ID,
                SurgeryStartingTime = booking.SurgeryStartingTime,
                SurgeryEndingTime = booking.SurgeryEndingTime,
                SurgeryRoomId = booking.SurgeryRoomId,
                PatientFullName = booking.PatientFullName,
                PatientDateOfBirth = booking.PatientDateOfBirth,
                PatientGender = booking.PatientGender,
                PatientMRNnumber = booking.PatientMRNnumber,
                SurgeryName = booking.SurgeryName,
                SurgerySite = booking.SurgerySite,
                SurgicalDepartmentId = booking.SurgicalDepartmentId,
                SurgeonID = booking.SurgeonID,
                BloodRequested = booking.BloodRequested,
                BloodRequestedText = booking.BloodRequestedText,
                RequestedPostOperativeCare = booking.RequestedPostOperativeCare,
                SurgeryPosition = booking.SurgeryPosition,
                SurgeryPositionText = booking.SurgeryPositionText,
                FrozenSection = booking.FrozenSection,
                SpecialThingsLikeSutures = booking.SpecialThingsLikeSutures,
                SpecialThingsLikeSuturesText = booking.SpecialThingsLikeSuturesText,
                Consumables = booking.Consumables,
                ConsumablesText = booking.ConsumablesText,
                AnesthesiaTechniqueId = booking.AnesthesiaTechniqueId,
                SpecialDevices = booking.SpecialDevices,
                SpecialDevicesText = booking.SpecialDevicesText,
                Turniquet = booking.Turniquet,
                CArm = booking.CArm,
                Harmonic = booking.Harmonic,
                Ligasure = booking.Ligasure,
                Microscope = booking.Microscope,
                Others = booking.Others,
                ItemsBooked = booking.ItemsBooked,
                AppointmentStatus = booking.AppointmentStatus,
                PatientStatus = booking.PatientStatus,
                CreatedDate = booking.CreatedDate,
                ModifiedDate = booking.ModifiedDate,
                CreatedBy = booking.CreatedBy,
                ModifiedBy = booking.ModifiedBy,
                
            };

            return View(bookingModel);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,SurgeryStartingTime,SurgeryEndingTime,SurgeryRoomId,PatientFullName,PatientDateOfBirth,PatientGender,PatientMRNnumber,SurgeryName,SurgerySite,SurgicalDepartmentId,SurgeonID,BloodRequested,RequestedPostOperativeCare,SurgeryPosition,FrozenSection,SpecialThingsLikeSutures,SpecialThingsLikeSuturesText,Consumables,AnesthesiaTechniqueId,SpecialDevices,Turniquet,CArm,Harmonic,Ligasure,Microscope,Others,AppointmentStatus,PatientStatus,BookedItemsList,BloodRequestedText,ConsumablesText,SpecialDevicesText,SurgeryPositionText,AnestheticsId")] CreateBookingViewModel booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var itemsBooked = booking.BookedItemsList?.Split(',').Where(x => int.TryParse(x, out _))
                        .Select(int.Parse)
                        .ToList();


                    booking.ItemsBooked = _context.ItemsBooked.Where(x => x.BookingId == booking.ID).Include(x=>x.Item).ToList();

                    //delete extra items in itemsbooked
                    List<ItemsBooked> collection = new List<ItemsBooked>();
                    if (booking.ItemsBooked.Any())
                    {                        
                        if (itemsBooked != null)
                        {
                            collection = booking.ItemsBooked
                                .Where(x => itemsBooked.All(c => c != x.ItemId)).ToList();
                        }
                        else
                        {
                            collection = booking.ItemsBooked;
                        }
                        if (collection.Any())
                            _context.ItemsBooked.RemoveRange(collection);

                    }

                    //add new items
                    if (itemsBooked != null)
                        foreach (var i in itemsBooked)
                        {

                            var booked = new ItemsBooked()
                            {
                                Booking = booking,
                                Item = _context.Item.FirstOrDefault(x => x.Id == i),
                                Quantity = 1
                            };
                            if (booking.ItemsBooked.All(x=>x.ItemId != booked.Item.Id))
                                booking.ItemsBooked.Add(booked);
                        }

                    var bookingId = booking.ID;
                    var overlapBookings = _bookingRepository.GetAllBookings().Where(x =>
                        x.SurgeryStartingTime < booking.SurgeryEndingTime &&
                        x.SurgeryEndingTime > booking.SurgeryStartingTime && x.ID != bookingId).ToList();

                    if (User.IsInRole("StatusUpdate") && booking.AnestheticsId == null)
                    {
                        ModelState.AddModelError("AnestheticsId", $"Anesthetics is required.");
                    }

                    var overLapRoom = overlapBookings.FirstOrDefault(x => x.SurgeryRoomId == booking.SurgeryRoomId);
                    if (overLapRoom != null)
                    {
                        ModelState.AddModelError("SurgeryRoomId", $"Room {overLapRoom.SurgeryRoom.SurgeryRoomDescription} booked in this hour since someone booked it just now. Please select other time or room.");
                    }

                    var totalItems = _context.Item.AsNoTracking().ToList();

                    foreach (var i in totalItems.Where(i => overlapBookings.Any(x => x.ItemsBooked.Any(c => c.Item.Id == i.Id))))
                    {
                        i.Quantity--;
                    }

                    foreach (var i in totalItems.Where(x => booking.ItemsBooked.Any(c => c.Item.Id == x.Id && x.Quantity < 1)))
                    {
                        ModelState.AddModelError("", $"'{i.Description}' is not available in this time since someone booked it just now. Please select other time.");
                    }

                    if (ModelState.ErrorCount == 0)
                    {
                        _bookingRepository.Update(booking);
                       
                    }

                   


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (ModelState.ErrorCount == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["Rooms"] = JsonConvert.SerializeObject(_context.SurgeryRoom.OrderBy(x => x.SurgeryRoomDescription).Select(x => new
            {
                id = x.Id,
                description = x.SurgeryRoomDescription.Trim()
            }));
            var items = _context.Item.ToArray();
            var listItems = items.Select(i => new { id = i.Id, description = i.Description }).Distinct().OrderBy(x => x.description);
            ViewData["Item"] = JsonConvert.SerializeObject(listItems);

            ViewData["AnesthesiaTechniqueId"] = new SelectList(_context.AnesthesiaTechnique, "Id", "Text", booking.AnesthesiaTechniqueId);
            ViewData["SurgeryRoomId"] = new SelectList(_context.SurgeryRoom, "Id", "SurgeryRoomDescription", booking.SurgeryRoomId);
            ViewData["SurgicalDepartmentId"] = new SelectList(_context.Set<SurgicalDepartment>(), "Id", "SurgicalDepartmentDescription", booking.SurgicalDepartmentId);
            ViewData["Anesthetics"] = new SelectList(_context.Anesthetics, "Id", "Name", booking.AnestheticsId);
             var allBookings = _bookingRepository.GetAllBookings();

            ViewData["Appointments"] = JsonConvert.SerializeObject(allBookings.Where(x => x.ID != id).Select(x => new
            {
                startTime = x.SurgeryStartingTime,
                endTime = x.SurgeryEndingTime,
                roomId = x.SurgeryRoomId,
                itemsBooked = x.ItemsBooked?.Where(p => p.Quantity > 0)
                  .SelectMany(p => Enumerable.Range(0, p.Quantity)
                      .Select(i => new
                      {
                          itemId = p.ItemId,
                          quantity = p.Quantity
                      }))
            }));

            
            ViewData["bookedItems"] = JsonConvert.SerializeObject(booking.ItemsBooked.Select(x => x.Item.Id).ToArray());
            var userModel = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Surgeon"))
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserID = user.Id,
                        UserName = user.UserName,
                    };
                    userModel.Add(userRoleViewModel);
                }

            }

            ViewData["SurgeonId"] = new SelectList(userModel, "UserID", "UserName", booking.SurgeonID);
            
            var originalBooking = _bookingRepository.GetBooking((int)id);

            booking.CreatedBy = originalBooking.CreatedBy;
            booking.ModifiedBy = originalBooking.ModifiedBy;


            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = _bookingRepository.GetBooking((int)id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.ID == id);
        }

        [HttpPost]
        public ActionResult SendSMS([FromBody]object json)
        {
            
            var jObj = JObject.Parse(json.ToString());
            var phoneNumber = jObj["phoneNumber"].ToString();
            var textMessage = jObj["textMessage"].ToString();
            int id = (int)jObj["id"];
            
            
            var accountSid = _configuration["SMS:AccountSid"]; // in appsettings.json
            var authToken = _configuration["SMS:AuthToken"];
            var fromPhoneNumber = _configuration["SMS:PhoneNumber"];
            
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(textMessage) || !(id > 0))
            {
               
                    return Json("Phone Number or TextMessage empty");
              
            }
            else
            {
                try
                {
                    var booking = _bookingRepository.GetBooking((int)id);

                    if (booking == null)
                    {
                        return NotFound();
                    }

                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        body: textMessage,
                        from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
                        to: new Twilio.Types.PhoneNumber(phoneNumber)
                    );

                    var sms = new SMS {Message = textMessage, PhoneNumber = phoneNumber, BookingId = id};
                    _context.SMS.Add(sms);
                    _context.SaveChanges();

                    booking = _bookingRepository.GetBooking((int)id);
                    
                    return Json(new
                    {
                        result = "success",
                        sms = JsonConvert.SerializeObject(booking.SMS.Select(x=> new
                        {
                            phoneNumber = x.PhoneNumber,
                            message = x.Message,
                            sentBy = _userManager.FindByIdAsync(x.CreatedBy).Result.DisplayName,
                            sentOn = ((DateTime) x.CreatedDate).ToString("dd/MM/yyyy")

                        }))
                        

                    });
                }
                catch (Exception e)
                {
                    return Json(e.Message);
                }
               
            }


        }

    }
}
