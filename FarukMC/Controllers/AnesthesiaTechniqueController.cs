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
    public class AnesthesiaTechniqueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnesthesiaTechniqueController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AnesthesiaTechnique
        public async Task<IActionResult> Index()
        {
            return View(await _context.AnesthesiaTechnique.ToListAsync());
        }

        // GET: AnesthesiaTechnique/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anesthesiaTechnique = await _context.AnesthesiaTechnique
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anesthesiaTechnique == null)
            {
                return NotFound();
            }

            return View(anesthesiaTechnique);
        }

        // GET: AnesthesiaTechnique/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AnesthesiaTechnique/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text")] AnesthesiaTechnique anesthesiaTechnique)
        {
            if (ModelState.IsValid)
            {
                _context.Add(anesthesiaTechnique);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(anesthesiaTechnique);
        }

        // GET: AnesthesiaTechnique/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anesthesiaTechnique = await _context.AnesthesiaTechnique.FindAsync(id);
            if (anesthesiaTechnique == null)
            {
                return NotFound();
            }
            return View(anesthesiaTechnique);
        }

        // POST: AnesthesiaTechnique/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text")] AnesthesiaTechnique anesthesiaTechnique)
        {
            if (id != anesthesiaTechnique.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anesthesiaTechnique);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnesthesiaTechniqueExists(anesthesiaTechnique.Id))
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
            return View(anesthesiaTechnique);
        }

        // GET: AnesthesiaTechnique/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anesthesiaTechnique = await _context.AnesthesiaTechnique
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anesthesiaTechnique == null)
            {
                return NotFound();
            }

            return View(anesthesiaTechnique);
        }

        // POST: AnesthesiaTechnique/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anesthesiaTechnique = await _context.AnesthesiaTechnique.FindAsync(id);
            _context.AnesthesiaTechnique.Remove(anesthesiaTechnique);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnesthesiaTechniqueExists(int id)
        {
            return _context.AnesthesiaTechnique.Any(e => e.Id == id);
        }
    }
}
