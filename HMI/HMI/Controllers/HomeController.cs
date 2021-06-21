using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HMI.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace HMI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
