namespace SuumoScraping.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Extensions;
    using SuumoScraping.Models;
    using SuumoScraping.UseCases;
    using SuumoScraping.ViewModels;

    public class BukkenController : Controller
    {
        private readonly GetFilteredBukkensUseCase _getFilteredBukkensUseCase;

        private readonly GetBukkenDetailsUseCase _getBukkenDetailsUseCase;

        private readonly ILogger<SuumoDataProvider> _logger;

        private readonly BukkenService _bukkenService;

        public BukkenController(
            GetFilteredBukkensUseCase getFilteredBukkensUseCase,
            GetBukkenDetailsUseCase getBukkenDetailsUseCase,
            ILogger<SuumoDataProvider> logger,
            BukkenService bukkenService
        )
        {
            _getFilteredBukkensUseCase = getFilteredBukkensUseCase;
            _getBukkenDetailsUseCase = getBukkenDetailsUseCase;
            _logger = logger;
            _bukkenService = bukkenService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Filter()
        {
            var filter = this.TempData.Get<FilterForm>("FilterForm") ?? new FilterForm();

            return this.View(filter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Filter(
            FilterForm model,
            CancellationToken cancellationToken = default
        )
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Filter");
            }

            var data = await _getFilteredBukkensUseCase.ExecuteAsync(model, cancellationToken);
            this.TempData.Put("BukkenInfos", data);
            this.TempData.Put("FilterForm", model);

            return this.RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            var filter = this.TempData.Get<FilterForm>("FilterForm") ?? new FilterForm();

            var data =
                this.TempData.Get<IList<BukkenInfo>>("BukkenInfos") ?? new List<BukkenInfo>();

            this.TempData.Put("BukkenInfos", data);
            this.TempData.Put("FilterForm", filter);

            return View(data);
        }

        [HttpGet]
        public async Task<ActionResult> Details(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var model = await _getBukkenDetailsUseCase.ExecuteAsync(id, cancellationToken);
            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }
    }
}
