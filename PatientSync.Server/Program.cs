using Serilog;
using PatientSync.Server.Repositories;
using PatientSync.Server.Services;
using PatientSync.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System;

namespace PatientSync.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(builder.Configuration)
                                .Enrich.FromLogContext()
                                .CreateLogger();

            builder.Host.UseSerilog(); // Replace the default logger with Serilog

            // Register MVC controllers and views
            builder.Services.AddControllersWithViews();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register your in-memory repo as a singleton
            builder.Services.AddSingleton<IRepository, InMemoryRepository>();
            
            // Register the service layer as scoped services
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddSignalR(); // Register SignalR hub
            builder.Services.AddHostedService<AlarmUpdateService>(); // Register the background service for updating alarms

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<PatientHub>("/patientHub"); // if SignalR hub is used

            app.MapFallbackToFile("/index.html");

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
                Log.CloseAndFlush();
            }
        }
    }
}
