namespace DotnetProjectForge.Services.Interfaces
{
    public interface ITemplateService
    {
        string GetTemplate(string name);
        string RenderTemplate(string template, Dictionary<string, string> values);
    }
}
