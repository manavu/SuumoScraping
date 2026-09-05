namespace SuumoScraping.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Application.UseCases;

    public class FileController : Controller
    {
        private readonly GetFileDataUseCase _getFileDataUseCase;

        private readonly ILogger<FileController> _logger;

        public FileController(GetFileDataUseCase getFileDataUseCase, ILogger<FileController> logger)
        {
            _getFileDataUseCase = getFileDataUseCase;
            _logger = logger;
        }

        // GET: File/Show/{id}
        [ResponseCache(Duration = 10000)]
        [HttpGet]
        public async Task<ActionResult> Show(int id, CancellationToken cancellationToken = default)
        {
            var result = await _getFileDataUseCase.ExecuteAsync(id, cancellationToken);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.File(result.Value.FileData, result.Value.ContentType);
        }

        // 後方互換性用: File/Data/{id}
        [ResponseCache(Duration = 10000)]
        [HttpGet]
        [ActionName("Data")]
        public async Task<ActionResult> Data(int id, CancellationToken cancellationToken = default)
        {
            return await this.Show(id, cancellationToken);
        }
    }
}
