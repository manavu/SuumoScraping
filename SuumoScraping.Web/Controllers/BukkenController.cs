namespace SuumoScraping.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Application.UseCases;
    using SuumoScraping.Domain.Models;
    using SuumoScraping.Extensions;

    public class BukkenController : Controller
    {
        private readonly GetBukkenDetailsUseCase _getBukkenDetailsUseCase;

        private readonly ILogger<BukkenController> _logger;

        public BukkenController(
            GetBukkenDetailsUseCase getBukkenDetailsUseCase,
            ILogger<BukkenController> logger
        )
        {
            _getBukkenDetailsUseCase = getBukkenDetailsUseCase;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var filter = this.TempData.Get<FilterForm>("FilterForm") ?? new FilterForm();

            var data =
                this.TempData.Get<IList<BukkenInfo>>("BukkenInfos") ?? new List<BukkenInfo>();

            this.TempData.Put("BukkenInfos", data);
            this.TempData.Put("FilterForm", filter);

            return this.View(data);
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
