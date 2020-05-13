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
    public class SurgeryRoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurgeryRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurgeryRoom
        public async Task<IActionResult> Index()
        {
            return View(await _context.SurgeryRoom.OrderBy(x=>x.SurgeryRoomDescription).ToListAsync());
        }

        // GET: SurgeryRoom/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgeryRoom = await _context.SurgeryRoom
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surgeryRoom == null)
            {
                return NotFound();
            }

            return View(surgeryRoom);
        }

        // GET: SurgeryRoom/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SurgeryRoom/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SurgeryRoomDescription")] SurgeryRoom surgeryRoom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surgeryRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surgeryRoom);
        }

        // GET: SurgeryRoom/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgeryRoom = await _context.SurgeryRoom.FindAsync(id);
            if (surgeryRoom == null)
            {
                return NotFound();
            }
            return View(surgeryRoom);
        }

        // POST: SurgeryRoom/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SurgeryRoomDescription")] SurgeryRoom surgeryRoom)
        {
            if (id != surgeryRoom.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surgeryRoom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurgeryRoomExists(surgeryRoom.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(surgeryRoom);
        }

        // GET: SurgeryRoom/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgeryRoom = await _context.SurgeryRoom
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surgeryRoom == null)
            {
                return NotFound();
            }

            return View(surgeryRoom);
        }

        // POST: SurgeryRoom/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surgeryRoom = await _context.SurgeryRoom.FindAsync(id);
            _context.SurgeryRoom.Remove(surgeryRoom);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurgeryRoomExists(int id)
        {
            return _context.SurgeryRoom.Any(e => e.Id == id);
        }
    }
}
