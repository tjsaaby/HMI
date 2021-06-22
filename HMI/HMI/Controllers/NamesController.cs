using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMI.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace HMI.Data
{
    public class NamesController : Controller
    {
        
        private readonly HMIContext _context;
        private readonly IMapper _mapper;
        public NamesController(HMIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Importing data from file into database.
        /// The sheet data is loaded into memory.
        /// Then the relevant data is extracted and uploaded.
        /// </summary>
        /// <remarks>
        /// Only edge case errors has checked.
        /// TODO: Check for bad data.
        /// Features.
        /// TODO: Enable support to upload multiple files at the time.
        /// TODO: Enable POST-API support using DTOs.
        /// TODO: Enable logging capability for monitoring.
        /// </remarks>
        public async Task<IActionResult> Import(IFormFile file)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            await using var stream = new MemoryStream();
            string fileTypeExt = Path.GetExtension(file.ContentType);

            if (fileTypeExt != ".sheet")
            {
                return ValidationProblem();
            }

            try
            {
                if (file == null)
                {
                    return ValidationProblem();
                }
                var sheet = new Sheet
                {
                    Name = file.FileName
                };
                _context.Sheet.Add(sheet);
                await _context.SaveChangesAsync();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                //For successful debugging capability
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                if (worksheet.Dimension == null)
                {
                    return ValidationProblem();
                }
                var rowcount = worksheet.Dimension.Rows;
                //Begin from 2, since first is header
                for (int row = 2; row <= rowcount; row++)
                {
                    var names = new Names
                    {
                        //NameID = worksheet.Cells[row, 1].Value.ToString().Trim(),
                        Name = worksheet.Cells[row, 2].Value.ToString().Trim(),
                        Address = worksheet.Cells[row, 3].Value.ToString().Trim(),
                   
                    };
                    names.SheetID = sheet.SheetID;
                    _context.Add(names);
                    await _context.SaveChangesAsync(); // how to check if you await-ed all async methods.
                }
                await transaction.CommitAsync();
                return Ok("Import succesful - Refresh to return");
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("An error ocurred while executing the data import: {0}", e.Message), e);
            } //https://stackoverflow.com/questions/19697445/how-efficiently-manage-a-stream-with-try-catch-finally-c-sharp review, is to be included or not
            
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

        /// <summary>
        /// API to view all Names in database.
        /// </summary>
        /// <remarks>
        /// This API using DTO to hide ids and prevent direct access when accessing content.
        /// </remarks>
        // GET: Names/api
        [HttpGet("Names/api")]
        public async Task<ActionResult<IEnumerable<GetNamesDto>>> GetNamesApi()
        {
            var names = await _context.Names.ToListAsync();
            var namesDto = _mapper.Map<List<Names>, List<GetNamesDto>>(names);
            return Ok(namesDto);

        }

        /// <summary>
        /// API to view all Names in database.
        /// </summary>
        /// <remarks>
        /// This API using DTO to hide ids and prevent direct access when accessing content.
        /// </remarks>
        // GET: Names/Details/api
        [HttpGet("Names/Details/api")]
        public async Task<ActionResult<Names>> GetDetailsApi(string id)
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
            var namesDto = _mapper.Map<Names, GetNamesDto>(names);

            return Ok(namesDto);
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
