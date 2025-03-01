// Copyright (c) 2025 - ColhounTech Limited
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VideoBatch.UI.Controls;
using VideoBatch.UI.Forms;


/* TODO : Short Roadmap of non-critical nice to have tsks
 * Setup Host Builder to load appsettings/Environment variables/User Secrets
 * Create Resource Projects for svg/png/jpg files
 */

namespace VideoBatchApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Starting VideoBatch Application...");
            Console.WriteLine($"Current Time: {DateTime.Now}");

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            var services = new ServiceCollection();
            ConfigureService(services);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var form = serviceProvider
                    .GetRequiredService<VideoBatchForm>()
                    ;
                Application.Run(form);
            }
        }

        private static void ConfigureService(ServiceCollection services)
        {
            services
                .AddLogging(x => x.AddConsole())
                .AddScoped<VideoBatchForm>()
                .AddScoped<ProjectTree>()
                //.AddScoped<CanvasDock>()
                //.AddScoped<MediaDock>()
                //.AddScoped<LibraryDock>()
                ;
        }
    }
}