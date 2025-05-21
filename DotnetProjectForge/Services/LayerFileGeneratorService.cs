using DotnetProjectForge.Models;
using DotnetProjectForge.Services.Interfaces;

namespace DotnetProjectForge.Services
{
    public class LayerFileGeneratorService : ILayerFileGeneratorService
    {
        private readonly ITemplateService _templateService;
        private readonly IFileSystemService _fileSystemService;

        public LayerFileGeneratorService(ITemplateService templateService, IFileSystemService fileSystemService)
        {
            _templateService = templateService;
            _fileSystemService = fileSystemService;
        }

        public async Task GenerateCsprojFileAsync(string layerPath, string layer, ProjectGenerationRequest request, Dictionary<string, string> replacements)
        {
            string templateName = layer == "Presentation" ? "API" : layer;
            string renderedContent = _templateService.RenderTemplate(templateName, replacements);
            string csprojFileName = $"{request.ProjectName}.{layer}.csproj";
            await _fileSystemService.GenerateFileAsync(layerPath, csprojFileName, renderedContent);
        }

        public async Task GenerateLayerFilesAsync(string layerPath, string layer, ProjectGenerationRequest request, Dictionary<string, string> replacements)
        {
            if (layer == "API" || layer == "Presentation")
            {
                await GenerateFileAsync(layerPath, "Startup.cs", "Startup", replacements);
                await GenerateFileAsync(layerPath, "Program.cs", "Program", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Controllers"), "SampleController.cs", "SampleController", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Properties"), "launchSettings.json", "launchSettings", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, ".vscode"), "launch.json", "launch", replacements);
            }
            else if (layer == "Application")
            {
                await GenerateFileAsync(Path.Combine(layerPath, "Interfaces"), "ISampleService.cs", "ISampleService", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Services"), "SampleService.cs", "SampleService", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Dtos"), "SampleDto.cs", "SampleDto", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Mappings"), "MappingProfile.cs", "MappingProfile", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Validations"), "SampleDtoValidator.cs", "SampleDtoValidator", replacements);
                await GenerateFileAsync(layerPath, "DependencyInjection.cs", "ApplicationDependencyInjection", replacements);
            }
            else if (layer == "Infrastructure" && request.Database != "None")
            {
                if (request.Database == "EFCore")
                    await GenerateFileAsync(Path.Combine(layerPath, "Data"), "AppDbContext.cs", "AppDbContext", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Repositories"), "GenericRepository.cs", "GenericRepository", replacements);
                await GenerateFileAsync(Path.Combine(layerPath, "Repositories"), "IRepository.cs", "IRepository", replacements);
                await GenerateFileAsync(layerPath, "DependencyInjection.cs", "InfrastructureDependencyInjection", replacements);
            }
            else if (layer == "Domain")
            {
                await GenerateFileAsync(layerPath, "BaseEntity.cs", "BaseEntity", replacements);
                await GenerateFileAsync(layerPath, "SampleEntity.cs", "SampleEntity", replacements);
            }
        }

        public async Task GenerateAppSettingsAsync(string basePath, List<string> layers, Dictionary<string, string> replacements)
        {
            if (layers.Contains("API") || layers.Contains("Presentation"))
            {
                string apiLayer = layers.Contains("API") ? "API" : "Presentation";
                await GenerateFileAsync(Path.Combine(basePath, apiLayer), "appsettings.json", "appsettings", replacements);
            }
        }

        private async Task GenerateFileAsync(string directory, string fileName, string templateName, Dictionary<string, string> replacements)
        {
            string templateContent = _templateService.GetTemplate(templateName);
            string renderedContent = _templateService.RenderTemplate(templateContent, replacements);
            await _fileSystemService.GenerateFileAsync(directory, fileName, renderedContent);
        }
    }
}
