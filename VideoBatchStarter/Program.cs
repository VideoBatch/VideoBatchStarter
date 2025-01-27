// Copyright (c) 2025 - ColhounTech Limited
namespace VideoBatchApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.Run(new VideoBatchForm());
        }
    }
}