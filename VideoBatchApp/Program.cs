// Copyright (c) 2025 - ColhounTech Limited
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using VideoBatch.Services;
using VideoBatch.UI.Controls;
using VideoBatch.UI.Forms;
using VideoBatch.UI.Forms.Docking;
using VideoBatchApp;
using VideoBatch.Logging;

/* TODO : Short Roadmap of non-critical nice to have tsks
 * Setup Host Builder to load appsettings/Environment variables/User Secrets
 * Create Resource Projects for svg/png/jpg files
 */

namespace VideoBatchApp
{
    internal static class Program
    {
        public static IConfiguration Configuration { get; private set; }

        [STAThread]
        static void Main()
        {
            Console.WriteLine("Starting VideoBatch Application...");

            // --- Configuration Setup ---
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            // ---------------------------

            Console.WriteLine($"Current Time: {DateTime.Now}");

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            var services = new ServiceCollection();
            ConfigureService(services);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                // Discover tasks early
                var taskDiscovery = serviceProvider.GetService<ITaskDiscoveryService>();
                taskDiscovery?.DiscoverTasks();

                var htmlService = serviceProvider.GetService<IHtmlTemplateService>();
                if (htmlService != null)
                {
                   _ = htmlService.LoadCssTemplateAsync();
                }
                else
                {
                   Console.WriteLine("Warning: IHtmlTemplateService not registered or resolved.");
                }

                var form = serviceProvider.GetRequiredService<VideoBatchForm>();
                Application.Run(form);
            }
        }

        private static void ConfigureService(ServiceCollection services)
        {
            services.AddSingleton(Configuration);

            // --- Register OutputDock Logger Components ---
            services.AddSingleton<OutputDock>();
            services.AddSingleton<OutputLogQueueService>();
            // OutputDockLoggerProvider is now registered below within AddLogging
            // services.AddSingleton<OutputDockLoggerProvider>(); // REMOVED from here
            // -------------------------------------------

            services.AddLogging(loggingBuilder => {
                 loggingBuilder.ClearProviders();
                 loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                 loggingBuilder.AddConsole();
                 
                 // --- CORRECT WAY TO ADD PROVIDER VIA DI ---
                 // Register the provider type with the service collection.
                 // The logging infrastructure will resolve it later using the main ServiceProvider,
                 // ensuring it gets the correct singleton OutputLogQueueService.
                 loggingBuilder.Services.AddSingleton<ILoggerProvider, OutputDockLoggerProvider>(); 
                 // -------------------------------------------

                 // REMOVED Incorrect resolution:
                 // using var tempProvider = services.BuildServiceProvider(); 
                 // loggingBuilder.AddProvider(tempProvider.GetRequiredService<OutputDockLoggerProvider>());
             });

            services.Configure<HtmlTemplateOptions>(Configuration.GetSection(HtmlTemplateOptions.Position));

            services
                .AddSingleton<IDataService, JsonDataService>()
                .AddSingleton<IDocumentationService, DocumentationService>()
                .AddSingleton<IRecentFilesService, RecentFilesService>()
                .AddSingleton<IHtmlTemplateService, HtmlTemplateService>()
                .AddSingleton<ITaskDiscoveryService, TaskDiscoveryService>()
                .AddScoped<IWorkAreaFactory, WorkAreaFactory>()
                .AddScoped<VideoBatchForm>()
                .AddScoped<ProjectTree>()
                .AddScoped<IProjectServices, ProjectServices>()
                .AddScoped<AssetsDock>()
                .AddScoped<TaskExplorerDock>()
                .AddTransient(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger<AssetsDock>())
                .AddTransient(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger<TaskExplorerDock>())
                .AddTransient(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger<SettingsForm>())
                ;
        }
    }
}