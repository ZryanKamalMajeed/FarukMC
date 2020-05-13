using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using FarukMC.Areas.Identity.Data;
using FarukMC.Models;
using FarukMC.ViewModel;
using Newtonsoft.Json;

namespace FarukMC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingRepository _bookingRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, IEmailService emailService, IBookingRepository bookingRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
            _bookingRepository = bookingRepository;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            var model = new IndexViewModel();
            model.Bookings = _bookingRepository.GetAllBookings();

            bool userIsAdmin = User.IsInRole("StatusUpdate") || User.IsInRole("Administrator");

           var items = model.Bookings.Select(i => new
            {
                start =   i.SurgeryStartingTime.ToString("MM-dd-yyyy HH:mm") + "', 'MM-DD-YYYY HH:mm').format('X')",
                end = i.SurgeryEndingTime.ToString("MM-dd-yyyy HH:mm") + "', 'MM-DD-YYYY HH:mm').format('X')",
                title = !userIsAdmin ? "" : i.Surgeon.DisplayName,
                content = !userIsAdmin ? "" : $"Name: {i.PatientFullName}" +
                                              $"<br/>Anesthesia Technique: {i.AnesthesiaTechnique.Text}" +
                                              $"<br/>Anesthetic: {i.Anesthetics?.Name}" +
                                              $"<br/>Appointment Status: {i.AppointmentStatus}" +
                                              $"<br/>Patient Status: {i.PatientStatus}",
                category = i.SurgeryRoom.SurgeryRoomDescription
            }).ToArray();

            var json = JsonConvert.SerializeObject(items);
            model.JsonResult = json;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost]
        public IActionResult SaveBooking(IndexViewModel model)
        {
            if (ModelState.IsValid){

                 _bookingRepository.Add(model.Bookings.First());
                return RedirectToAction("Index");
            }
            return View("Index");
        }
        [HttpPost]
        public IActionResult DeleteBooking(int id)
        {
            _bookingRepository.Delete(id);
            return RedirectToAction("Index");

        }
    }
}
