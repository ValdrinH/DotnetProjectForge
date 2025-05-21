namespace DotnetProjectForge.Models
{
    public class ProjectGenerationRequest
    {
        public string ProjectName { get; set; }
        public string Architecture { get; set; }
        public string Authentication { get; set; }
        public string Database { get; set; }
        public List<string> Features { get; set; } = new();
        public string DotnetVersion { get; set; }
    }
}