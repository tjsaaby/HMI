using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMI.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace HMI.Data
{
    public class NamesController : Controller
    {
        //Import excel function
        //Possible vulnerable to data corrupt attack?
        public async Task<List<Names>> Import(IFormFile file)
        {
            var list = new List<Names>();
            await using var stream = new MemoryStream();
            try
            {
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                //For successful debugging capability
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                var rowcount = worksheet.Dimension.Rows;
                //Begin from 2, since first is header
                for (int row = 2; row <= rowcount; row++)
                {
                    list.Add(new Names
                    {
                        NameID = worksheet.Cells[row, 1].Value.ToString().Trim(),
                        Name = worksheet.Cells[row, 2].Value.ToString().Trim(),
                        Address = worksheet.Cells[row, 3].Value.ToString().Trim(),
                    });
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("An error ocurred while executing the data import: {0}", e.Message), e);
            } //https://stackoverflow.com/questions/19697445/how-efficiently-manage-a-stream-with-try-catch-finally-c-sharp review, is to be included or not

            return list;
        }

        private readonly HMIContext _context;

        public NamesController(HMIContext context)
        {
            _context = context;
        }

        // GET: Names
        public async Task<IActionResult> Index()
        {
            return View(await _context.Names.ToListAsync());
        }

        // GET: Names/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var names = await _context.Names
                .FirstOrDefaultAsync(m => m.NameID == id);
            if (names == null)
            {
                return NotFound();
            }

            return View(names);
        }

        // GET: Names/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Names/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameID,Name,Address")] Names names)
        {
            if (ModelState.IsValid)
            {
                _context.Add(names);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(names);
        }

        // GET: Names/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var names = await _context.Names.FindAsync(id);
            if (names == null)
            {
                return NotFound();
            }
            return View(names);
        }

        // POST: Names/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NameID,Name,Address")] Names names)
        {
            if (id != names.NameID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(names);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NamesExists(names.NameID))
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
            return View(names);
        }

        // GET: Names/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var names = await _context.Names
                .FirstOrDefaultAsync(m => m.NameID == id);
            if (names == null)
            {
                return NotFound();
            }

            return View(names);
        }

        // POST: Names/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var names = await _context.Names.FindAsync(id);
            _context.Names.Remove(names);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NamesExists(string id)
        {
            return _context.Names.Any(e => e.NameID == id);
        }
    }
}
