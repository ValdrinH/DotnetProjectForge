using DotnetProjectForge.Models;
using DotnetProjectForge.Services.Interfaces;

namespace DotnetProjectForge.Services
{
    public class ProjectGeneratorService : IProjectGeneratorService
    {
        private readonly ITemplateService _templateService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IReplacementBuilderService _replacementBuilderService;
        private readonly ILayerFileGeneratorService _layerFileGeneratorService;
        private readonly Dictionary<string, List<string>> _architectureLayers = new()
        {
            ["Layered"] = new() { "API", "Application", "Infrastructure", "Domain" },
            ["Clean"] = new() { "Presentation", "Application", "Infrastructure", "Domain" },
            ["Minimal"] = new() { "API" }
        };

        public ProjectGeneratorService(
            ITemplateService templateService,
            IFileSystemService fileSystemService,
            IReplacementBuilderService replacementBuilderService,
            ILayerFileGeneratorService layerFileGeneratorService)
        {
            _templateService = templateService;
            _fileSystemService = fileSystemService;
            _replacementBuilderService = replacementBuilderService;
            _layerFileGeneratorService = layerFileGeneratorService;
        }

        public async Task<string> GenerateProjectAsync(ProjectGenerationRequest request)
        {
            // Create temp directory
            string tempBasePath = _fileSystemService.CreateTempDirectory(request.ProjectName);

            if (!_architectureLayers.TryGetValue(request.Architecture, out var layers))
            {
                throw new ArgumentException("Unsupported architecture type");
            }

            // Build initial replacements
            var replacements = _replacementBuilderService.BuildReplacements(request, layers[0]);

            // Generate files for each layer
            foreach (var layer in layers)
            {
                string layerPath = Path.Combine(tempBasePath, layer);
                replacements = _replacementBuilderService.BuildReplacements(request, layer);

                await _layerFileGeneratorService.GenerateCsprojFileAsync(layerPath, layer, request, replacements);
                await _layerFileGeneratorService.GenerateLayerFilesAsync(layerPath, layer, request, replacements);
            }

            await _layerFileGeneratorService.GenerateAppSettingsAsync(tempBasePath, layers, replacements);

            // Create ZIP and clean up
            string zipPath = _fileSystemService.CreateZip(tempBasePath, request.ProjectName);
            _fileSystemService.DeleteTempDirectory(tempBasePath);

            return zipPath;
        }
    }
}