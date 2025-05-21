using DotnetProjectForge.Models;

namespace DotnetProjectForge.Services.Interfaces
{
    public interface IReplacementBuilderService
    {
        Dictionary<string, string> BuildReplacements(ProjectGenerationRequest request, string layer);
    }
}
