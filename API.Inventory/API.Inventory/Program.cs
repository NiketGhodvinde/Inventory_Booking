using API.Inventory.Data.RepositoryService;
using API.Inventory.Repository;
using Npgsql;
using System.Data;

public class Program
{
    public static void Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);

        // Set up environment-specific configuration (will automatically pick appsettings.<Environment>.json based on the environment)
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        if (builder.Environment.IsDevelopment())
        {
            // Development-specific configuration
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        }
        else if (builder.Environment.IsProduction())
        {
            // Production-specific configuration
            builder.Configuration.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
        }

        // Register Dapper and other services
        builder.Services.AddScoped<IDbConnection>(sp =>
            new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register the IBookingRepository and its implementation
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();

        // Add CORS policy based on environment
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
                else if (builder.Environment.IsProduction())
                {
                    policy.WithOrigins("http://192.168.0.12:81") 
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        // Add controllers to the DI container
        builder.Services.AddControllers();

        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // Build the app
        var app = builder.Build();

        // Use the CORS policy
        app.UseCors("AllowSpecificOrigin");

        // Enforce HTTPS redirection in production environment
        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        // Map controllers
        app.MapControllers();

        // Run the application
        app.Run();

    }
}