using DotnetProjectForge.Models;

namespace DotnetProjectForge.Services.Interfaces
{
    public interface ILayerFileGeneratorService
    {
        Task GenerateLayerFilesAsync(string layerPath, string layer, ProjectGenerationRequest request, Dictionary<string, string> replacements);
        Task GenerateCsprojFileAsync(string layerPath, string layer, ProjectGenerationRequest request, Dictionary<string, string> replacements);
        Task GenerateAppSettingsAsync(string basePath, List<string> layers, Dictionary<string, string> replacements);
    }
}
