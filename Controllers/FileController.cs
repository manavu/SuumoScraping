namespace SuumoScraping.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Models;
    using SuumoScraping.UseCases;
    using SuumoScraping.ViewModels;

    public class FileController : Controller
    {
        private readonly GetFileDataUseCase _getFileDataUseCase;

        private readonly GetFloorPlansUseCase _getFloorPlansUseCase;

        public FileController(
            GetFileDataUseCase getFileDataUseCase,
            GetFloorPlansUseCase getFloorPlansUseCase
        )
        {
            _getFileDataUseCase = getFileDataUseCase;
            _getFloorPlansUseCase = getFloorPlansUseCase;
        }

        // GET: File
        [ResponseCache(Duration = 10000)]
        [HttpGet]
        public async Task<ActionResult> Data(int id, CancellationToken cancellationToken = default)
        {
            var result = await _getFileDataUseCase.ExecuteAsync(id, cancellationToken);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.File(result.Value.FileData, result.Value.ContentType);
        }

        [HttpGet]
        public async Task<ActionResult> List(CancellationToken cancellationToken = default)
        {
            var model = await _getFloorPlansUseCase.ExecuteAsync(cancellationToken);

            return this.View(model);
        }
    }
}
