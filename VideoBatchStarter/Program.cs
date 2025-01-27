// Copyright (c) 2025 - ColhounTech Limited
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace VideoBatchApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
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
                //.AddScoped<ProjectTree>()
                //.AddScoped<CanvasDock>()
                //.AddScoped<MediaDock>()
                //.AddScoped<LibraryDock>()
                ;
        }
    }
}