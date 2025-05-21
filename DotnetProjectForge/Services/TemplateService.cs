using System.Reflection;
using DotnetProjectForge.Services.Interfaces;

namespace DotnetProjectForge.Services
{
    public class TemplateService : ITemplateService
    {
        public string GetTemplate(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith($"Templates.{name}.txt") ||
                                     x.EndsWith($"Templates.{name}.csproj") ||
                                     x.EndsWith($"Templates.{name}.json"));

            if (resourceName == null)
                throw new FileNotFoundException($"Template '{name}' not found as embedded resource.");

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }

        public string RenderTemplate(string template, Dictionary<string, string> values)
        {
            foreach (var pair in values)
            {
                template = template.Replace($"{{{{{pair.Key}}}}}", pair.Value);
            }
            return template;
        }
    }
}