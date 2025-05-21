using DotnetProjectForge.Models;
using DotnetProjectForge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotnetProjectForge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectGeneratorController : ControllerBase
    {
        private readonly IProjectGeneratorService _generator;
        public ProjectGeneratorController(IProjectGeneratorService generator)
        {
            _generator = generator;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateProject([FromBody] ProjectGenerationRequest request)
        {
            var zipPath = await _generator.GenerateProjectAsync(request);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
            var fileName = $"{request.ProjectName}.zip";

            var result = File(fileBytes, "application/zip", fileName);

            HttpContext.Items["__ActionResult"] = result;

            return result;
        }
    }
}
