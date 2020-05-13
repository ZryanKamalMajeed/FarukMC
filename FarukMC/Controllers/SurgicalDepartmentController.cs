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
    public class SurgicalDepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurgicalDepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurgicalDepartment
        public async Task<IActionResult> Index()
        {
            return View(await _context.SurgicalDepartment.ToListAsync());
        }

        // GET: SurgicalDepartment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgicalDepartment = await _context.SurgicalDepartment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surgicalDepartment == null)
            {
                return NotFound();
            }

            return View(surgicalDepartment);
        }

        // GET: SurgicalDepartment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SurgicalDepartment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SurgicalDepartmentDescription")] SurgicalDepartment surgicalDepartment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surgicalDepartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surgicalDepartment);
        }

        // GET: SurgicalDepartment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgicalDepartment = await _context.SurgicalDepartment.FindAsync(id);
            if (surgicalDepartment == null)
            {
                return NotFound();
            }
            return View(surgicalDepartment);
        }

        // POST: SurgicalDepartment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SurgicalDepartmentDescription")] SurgicalDepartment surgicalDepartment)
        {
            if (id != surgicalDepartment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surgicalDepartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurgicalDepartmentExists(surgicalDepartment.Id))
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
            return View(surgicalDepartment);
        }

        // GET: SurgicalDepartment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgicalDepartment = await _context.SurgicalDepartment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surgicalDepartment == null)
            {
                return NotFound();
            }

            return View(surgicalDepartment);
        }

        // POST: SurgicalDepartment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surgicalDepartment = await _context.SurgicalDepartment.FindAsync(id);
            _context.SurgicalDepartment.Remove(surgicalDepartment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurgicalDepartmentExists(int id)
        {
            return _context.SurgicalDepartment.Any(e => e.Id == id);
        }
    }
}
