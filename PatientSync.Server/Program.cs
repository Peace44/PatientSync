using Serilog;
using PatientSync.Server.Repositories;
using PatientSync.Server.Services;
using PatientSync.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace PatientSync.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Information("Configuring web application builder...");
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Information("Configuring Serilog...");
            Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(builder.Configuration)
                                .Enrich.FromLogContext()
                                .CreateLogger();

            builder.Host.UseSerilog(); // Replace the default logger with Serilog

            // Make all URLs lowercase
            Log.Information("Configuring route options...");
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            //// Add CORS services
            //Log.Information("Configuring CORS services...");
            //builder.Services.AddCors(options =>
            //{
            //    //options.AddPolicy("AllowFrontend", policy =>
            //    //{
            //    //    policy.WithOrigins("https://localhost:59052")
            //    //          .AllowAnyHeader()
            //    //          .AllowAnyMethod();
            //    //          .AllowCredentials();
            //    //});

            //    // Don't use this in production!
            //    options.AddPolicy("AllowAnyOrigin", policy =>
            //    {
            //        policy.AllowAnyOrigin()
            //              .AllowAnyHeader()
            //              .AllowAnyMethod();
            //    });
            //});

            // Register MVC controllers and views
            Log.Information("Registering MVC controllers and views...");
            builder.Services.AddControllersWithViews();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            Log.Information("Configuring Swagger...");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register your in-memory repo as a singleton
            Log.Information("Registering in-memory repository...");
            builder.Services.AddSingleton<IRepository, InMemoryRepository>();

            // Register the service layer as scoped services
            Log.Information("Registering service layer...");
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IUserService, UserService>();

            Log.Information("Registering SignalR hub...");
            builder.Services.AddSignalR(); // Register SignalR hub
            Log.Information("Registering background service for updating alarms...");
            builder.Services.AddHostedService<AlarmUpdateService>(); // Register the background service for updating alarms

            // Configure Cookie Authentication
            Log.Information("Configuring cookie authentication...");
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.LoginPath = "/authentication/login";
                                options.LogoutPath = "/authentication/logout";
                                options.AccessDeniedPath = "/authentication/login";
                                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                                options.Cookie.Name = "PatientSyncAuthCookie";
                                //options.Cookie.HttpOnly = true;
                                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                                //options.Cookie.SameSite = SameSiteMode.Strict;
                            });

            Log.Information("Building the web application...");
            var app = builder.Build();

            app.UseDefaultFiles(); // To enable default file mapping (e.g., index.html)
            app.UseStaticFiles(); // To enable static file serving from wwwroot

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                Log.Information("Configuring development environment...");
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                Log.Information("Configuring production environment...");
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //app.UseCors("AllowFrontend");
            //app.UseCors("AllowAnyOrigin");

            // Enable Authentication and Authorization Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // the following line should come after configuring middleware and before app.Run()
            app.MapHub<PatientHub>("/patientHub"); // creates an endpoint at /patientHub that clients can use to connect to the SignalR Hub

            app.MapFallbackToFile("/index.html"); // will serve index.html for any request that doesn't match an existing file or API route

            try
            {
                Log.Information("Starting PatientSync Server...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "PatientSync SERVER TERMINATED UNEXPECTEDLY!");
            }
            finally
            {
                Log.Information("Closing and flushing Serilog...");
                Log.CloseAndFlush();
            }
        }
    }
}
