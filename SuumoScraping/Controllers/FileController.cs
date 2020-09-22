namespace SuumoScraping.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Models;
    using SuumoScraping.ViewModels;

    public class FileController : Controller
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public FileController(IScrapingContextFactory scrapingContextFactory)
        {
            _scrapingContextFactory = scrapingContextFactory;
        }

        // GET: File
        [ResponseCache(Duration = 10000)]
        [HttpGet]
        public ActionResult Data(int id)
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var file = db.Files.SingleOrDefault(m => m.Id == id);

                return this.File(file.FileData, file.ContentType);
            }
        }

        [HttpGet]
        public ActionResult List()
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var model = db.NewBukkens
                    .SelectMany(m => m.Files)
                    .Where(m => m.Type == "間取り図")
                    .Select(m => new FloorPlanInfo()
                    {
                        FileId = m.File.Id,
                        BukkenId = m.NewBukken.Id,
                        FloorArea = m.NewBukken.FloorArea,
                    })
                    .Take(1000)
                    .ToList();

                return View(model);
            }
        }
    }
}