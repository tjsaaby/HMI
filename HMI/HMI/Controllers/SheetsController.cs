using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMI.Data;
using HMI.Models;

namespace HMI.Controllers
{
    public class SheetsController : Controller
    {
        private readonly HMIContext _context;

        public SheetsController(HMIContext context)
        {
            _context = context;
        }

        // GET: Sheets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sheet.ToListAsync());
        }

        // GET: Sheets/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sheet = await _context.Sheet
                .FirstOrDefaultAsync(m => m.SheetID == id);
            if (sheet == null)
            {
                return NotFound();
            }

            return View(sheet);
        }

        // GET: Sheets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sheet = await _context.Sheet
                .FirstOrDefaultAsync(m => m.SheetID == id);
            if (sheet == null)
            {
                return NotFound();
            }

            return View(sheet);
        }

        // POST: Sheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sheet = await _context.Sheet.FindAsync(id);
            _context.Sheet.Remove(sheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SheetExists(string id)
        {
            return _context.Sheet.Any(e => e.SheetID == id);
        }
    }
}
