using DotnetProjectForge.Models;
using DotnetProjectForge.Services.Interfaces;

namespace DotnetProjectForge.Services
{
    public class ReplacementBuilderService : IReplacementBuilderService
    {
        public Dictionary<string, string> BuildReplacements(ProjectGenerationRequest request, string layer)
        {
            var replacements = new Dictionary<string, string>
            {
                ["ProjectName"] = request.ProjectName,
                ["Namespace"] = request.ProjectName,
                ["DotnetVersion"] = request.DotnetVersion ?? "net6.0"
            };

            string packages = "";
            if (layer == "API" || layer == "Presentation")
            {
                if (request.Authentication == "JWT")
                    packages += "<PackageReference Include=\"Microsoft.AspNetCore.Authentication.JwtBearer\" Version=\"6.0.0\" />\n";
                if (request.Features.Contains("Swagger"))
                    packages += "<PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"6.2.3\" />\n";
                if (request.Features.Contains("Serilog"))
                    packages += "<PackageReference Include=\"Serilog.AspNetCore\" Version=\"4.1.0\" />\n";
            }
            else if (layer == "Application")
            {
                packages += "<PackageReference Include=\"AutoMapper\" Version=\"10.1.1\" />\n";
                packages += "<PackageReference Include=\"FluentValidation\" Version=\"10.3.6\" />\n";
                packages += "<PackageReference Include=\"FluentValidation.DependencyInjectionExtensions\" Version=\"10.3.6\" />\n";
            }
            else if (layer == "Infrastructure")
            {
                if (request.Database == "EFCore")
                {
                    packages += "<PackageReference Include=\"Microsoft.EntityFrameworkCore\" Version=\"6.0.0\" />\n";
                    packages += "<PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"6.0.0\" />\n";
                }
                else if (request.Database == "Dapper")
                    packages += "<PackageReference Include=\"Dapper\" Version=\"2.0.123\" />\n";
            }
            replacements["Packages"] = packages;

            if (layer == "API" || layer == "Presentation")
            {
                replacements["AuthenticationServices"] = BuildAuthenticationServices(request);
                replacements["DatabaseServices"] = BuildDatabaseServices(request);
                replacements["Middleware"] = BuildMiddleware(request);
                replacements["ServiceRegistration"] = BuildServiceRegistration(request);
            }

            string appSettings = "{ \"Logging\": { \"LogLevel\": { \"Default\": \"Information\" } }";
            if (request.Authentication == "JWT")
                appSettings += ", \"Jwt\": { \"Key\": \"YourSecretKey\", \"Issuer\": \"YourIssuer\", \"Audience\": \"YourAudience\" }";
            if (request.Features.Contains("Serilog"))
                appSettings += ", \"Serilog\": { \"MinimumLevel\": \"Information\" }";
            appSettings += " }";
            replacements["AppSettingsContent"] = appSettings;

            return replacements;
        }

        private string BuildAuthenticationServices(ProjectGenerationRequest request)
        {
            if (request.Authentication == "JWT")
            {
                return @"
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration[""Jwt:Issuer""],
        ValidAudience = Configuration[""Jwt:Audience""],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[""Jwt:Key""]))
    };
});";
            }
            return "";
        }

        private string BuildDatabaseServices(ProjectGenerationRequest request)
        {
            if (request.Database == "EFCore")
                return "services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(\"InMemoryDb\"));";
            return "";
        }

        private string BuildMiddleware(ProjectGenerationRequest request)
        {
            string middleware = "";
            if (request.Authentication != "None")
            {
                middleware += "app.UseAuthentication();\n";
                middleware += "app.UseAuthorization();\n";
            }
            if (request.Features.Contains("Swagger"))
            {
                middleware += "app.UseSwagger();\n";
                middleware += "app.UseSwaggerUI(c => c.SwaggerEndpoint(\"/swagger/v1/swagger.json\", \"{{ProjectName}} API V1\"));\n";
            }
            return middleware;
        }

        private string BuildServiceRegistration(ProjectGenerationRequest request)
        {
            return @"
services.AddApplication();
services.AddInfrastructure();";
        }
    }
}
