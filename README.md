DotnetProjectForge API

Overview
DotnetProjectForge is a modern, scalable, and secure ASP.NET Core API built with a layered architecture. It provides a robust foundation for building RESTful services with features like JWT authentication, rate limiting, standardized responses, Swagger documentation, and more. The project is designed to be easily extensible and maintainable, following best practices for clean code and dependency injection.
Features

Layered Architecture: Organized into API, Application, Infrastructure, and Domain layers for clear separation of concerns.
JWT Authentication: Secure endpoints with JSON Web Token authentication.
Rate Limiting: Prevents abuse by limiting requests to 60 per minute per IP using AspNetCoreRateLimit.
Standardized Responses: All API responses (except file downloads) are wrapped in a consistent JSON format:
Success: { "success": true, "data": {...}, "message": null }
Error: { "success": false, "data": null, "message": "error message" }


Global Exception Handling: Exceptions are caught and returned in the standardized format.
Swagger Documentation: Interactive API documentation available at /swagger.
Serilog Logging: Structured logging for better debugging and monitoring.
Entity Framework Core: Database access with EF Core (configured with an in-memory database for demo purposes).
AutoMapper: Simplifies object mapping between layers.
FluentValidation: Validates incoming DTOs with clear error messages.
Dependency Injection: Services are registered in Application and Infrastructure layers for loose coupling.

Prerequisites
Before running the project, ensure you have the following installed:

.NET SDK 9.0 (or later)
Visual Studio 2022 (or another IDE like Visual Studio Code)
SQL Server (optional, if you replace the in-memory database with a real database)
Git (to clone the repository)

Getting Started
1. Clone the Repository
git clone https://github.com/ValdrinH/DotnetProjectForge)
cd DotnetProjectForge

2. Restore Dependencies
Restore the required NuGet packages:
dotnet restore

3. Configure the Application
Update the appsettings.json file in the DotnetProjectForge.API project with your settings:

JWT Configuration:
"Jwt": {
  "Key": "YourSecretKey",
  "Issuer": "YourIssuer",
  "Audience": "YourAudience"
}

Replace YourSecretKey, YourIssuer, and YourAudience with your values.

Rate Limiting:
"IpRateLimiting": {
  "EnableEndpointRateLimiting": true,
  "StackBlockedRequests": false,
  "RealIpHeader": "X-Real-IP",
  "ClientIdHeader": "X-ClientId",
  "GeneralRules": [
    {
      "Endpoint": "*",
      "Period": "1m",
      "Limit": 60
    }
  ]
}

Adjust the rate limit (Limit) or period (Period) as needed.

Database (optional):Replace the in-memory database in Program.cs with a real database connection string if needed:
services.AddDbContext<AppDbContext>(options => options.UseSqlServer("YourConnectionString"));



4. Build and Run
Build and run the project:
dotnet build
dotnet run --project DotnetProjectForge.API

The API will be available at https://localhost:5001 (or http://localhost:5000).
5. Explore the API

Swagger UI: Open https://localhost:5001/swagger in your browser to explore the API endpoints.
Sample Endpoint: Test the sample endpoint at GET /api/sample (requires JWT authentication).

Usage
Authentication
To access protected endpoints, obtain a JWT token and include it in the Authorization header:
Authorization: Bearer <your-token>

Rate Limiting
The API enforces a rate limit of 60 requests per minute per IP. Exceeding this limit returns a 429 Too Many Requests status.
Standardized Responses

Successful Response (e.g., GET /api/sample):{
  "success": true,
  "data": {
    "id": 1,
    "name": "Sample"
  },
  "message": null
}


Error Response (e.g., exception):{
  "success": false,
  "data": null,
  "message": "An error occurred"
}



File Downloads
File download endpoints (e.g., /api/projectgenerator) return raw file streams without the standard JSON wrapper.
Project Structure

DotnetProjectForge.API: Presentation layer with controllers, middleware, and configuration.
Controllers/: API endpoints.
Middleware/StandardResponseMiddleware.cs: Standardizes responses and handles exceptions.
Program.cs: Application entry point and middleware pipeline.


DotnetProjectForge.Application: Business logic layer.
Interfaces/: Service interfaces.
Services/: Service implementations.
Dtos/: Data Transfer Objects.
Mappings/: AutoMapper profiles.
Validations/: FluentValidation rules.


DotnetProjectForge.Infrastructure: Data access layer.
Data/: EF Core database context.
Repositories/: Generic repository pattern.


DotnetProjectForge.Domain: Domain models and entities.

Contributing
Contributions are welcome! Follow these steps:

Fork the repository.
Create a new branch (git checkout -b feature/your-feature).
Make your changes and commit (git commit -m "Add your feature").
Push to your branch (git push origin feature/your-feature).
Open a Pull Request.

License
This project is licensed under the MIT License. See the LICENSE file for details.
Contact
For questions or feedback, reach out to your-email@example.com.
