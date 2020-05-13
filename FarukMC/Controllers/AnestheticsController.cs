using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FarukMC.Data;
using FarukMC.Models;
using Microsoft.AspNetCore.Authorization;

namespace FarukMC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AnestheticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnestheticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Anesthetics.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Anesthetics anesthetic)
        {
            if (ModelState.IsValid)
            {
                _context.Anesthetics.Add(anesthetic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(anesthetic);
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var anesthetic = await _context.Anesthetics.FindAsync(id);
            if (anesthetic == null)
            {
                return NotFound();
            }
            return View(anesthetic);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Anesthetics anesthetic)
        {
            if (ModelState.IsValid)
            {
                _context.Update(anesthetic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(anesthetic);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var anesthetic = await _context.Anesthetics.FindAsync(id);
            if (anesthetic == null)
            {
                return NotFound();
            }
            return View(anesthetic);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var anesthetic = await _context.Anesthetics.FindAsync(id);
            if (anesthetic == null)
            {
                return NotFound();
            }
            _context.Anesthetics.Remove(anesthetic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anesthetic = await _context.Anesthetics.FindAsync(id);
            if (anesthetic == null)
            {
                return NotFound();
            }

            return View(anesthetic);
        }


    }

}
