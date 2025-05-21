namespace DotnetProjectForge.Services.Interfaces
{
    public interface IFileSystemService
    {
        Task GenerateFileAsync(string directory, string fileName, string content);
        string CreateTempDirectory(string projectName);
        string CreateZip(string sourcePath, string projectName);
        void DeleteTempDirectory(string path);
    }
}
