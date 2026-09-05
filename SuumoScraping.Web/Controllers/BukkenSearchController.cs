namespace SuumoScraping.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Application.UseCases;
    using SuumoScraping.Domain.Models;
    using SuumoScraping.Extensions;

    public class BukkenSearchController : Controller
    {
        private readonly GetFilteredBukkensUseCase _getFilteredBukkensUseCase;

        private readonly ILogger<BukkenSearchController> _logger;

        public BukkenSearchController(
            GetFilteredBukkensUseCase getFilteredBukkensUseCase,
            ILogger<BukkenSearchController> logger
        )
        {
            _getFilteredBukkensUseCase = getFilteredBukkensUseCase;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var filter = this.TempData.Get<FilterForm>("FilterForm") ?? new FilterForm();

            return this.View(filter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            FilterForm model,
            CancellationToken cancellationToken = default
        )
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index");
            }

            var data = await _getFilteredBukkensUseCase.ExecuteAsync(model, cancellationToken);
            this.TempData.Put("BukkenInfos", data);
            this.TempData.Put("FilterForm", model);

            return this.RedirectToAction("Index", "Bukken");
        }
    }
}
