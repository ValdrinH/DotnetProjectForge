namespace DotnetProjectForge.Helpers
{
    public static class TemplateRenderer
    {
        public static string RenderTemplate(string path, Dictionary<string, string> values)
        {
            var content = File.ReadAllText(path);

            foreach (var kvp in values)
            {
                content = content.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }

            return content;
        }
    }
}
