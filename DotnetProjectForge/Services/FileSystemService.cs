using System.IO.Compression;
using DotnetProjectForge.Services.Interfaces;

namespace DotnetProjectForge.Services
{
    public class FileSystemService : IFileSystemService
    {
        public Task GenerateFileAsync(string directory, string fileName, string content)
        {
            Directory.CreateDirectory(directory);
            string destinationFile = Path.Combine(directory, fileName);
            return File.WriteAllTextAsync(destinationFile, content);
        }

        public string CreateTempDirectory(string projectName)
        {
            string tempBasePath = Path.Combine(Path.GetTempPath(), "GeneratedProjects", $"{projectName}_{DateTime.Now:yyyyMMddHHmmss}");
            Directory.CreateDirectory(tempBasePath);
            return tempBasePath;
        }

        public string CreateZip(string sourcePath, string projectName)
        {
            string zipPath = Path.Combine(Path.GetTempPath(), $"{projectName}_{DateTime.Now:yyyyMMddHHmmss}.zip");
            ZipFile.CreateFromDirectory(sourcePath, zipPath);
            return zipPath;
        }

        public void DeleteTempDirectory(string path)
        {
            Directory.Delete(path, true);
        }
    }
}
