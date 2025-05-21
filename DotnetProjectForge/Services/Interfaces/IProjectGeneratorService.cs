using DotnetProjectForge.Models;

namespace DotnetProjectForge.Services.Interfaces
{
    public interface IProjectGeneratorService
    {
        Task<string> GenerateProjectAsync(ProjectGenerationRequest request);
    }
}
