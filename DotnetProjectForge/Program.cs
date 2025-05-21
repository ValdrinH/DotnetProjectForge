using AspNetCoreRateLimit;
using DotnetProjectForge.API.Middleware;
using DotnetProjectForge.Services;
using DotnetProjectForge.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IFileSystemService, FileSystemService>();
builder.Services.AddScoped<IReplacementBuilderService, ReplacementBuilderService>();
builder.Services.AddScoped<ILayerFileGeneratorService, LayerFileGeneratorService>();
builder.Services.AddScoped<IProjectGeneratorService, ProjectGeneratorService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Add rate limiting services
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseStandardResponse();
app.UseAuthorization();

app.MapControllers();

app.Run();

