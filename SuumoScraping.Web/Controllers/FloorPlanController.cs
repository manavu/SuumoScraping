namespace SuumoScraping.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Application.UseCases;

    public class FloorPlanController : Controller
    {
        private readonly GetFloorPlansUseCase _getFloorPlansUseCase;

        private readonly ILogger<FloorPlanController> _logger;

        public FloorPlanController(
            GetFloorPlansUseCase getFloorPlansUseCase,
            ILogger<FloorPlanController> logger
        )
        {
            _getFloorPlansUseCase = getFloorPlansUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index(CancellationToken cancellationToken = default)
        {
            var model = await _getFloorPlansUseCase.ExecuteAsync(cancellationToken);

            return this.View(model);
        }
    }
}
