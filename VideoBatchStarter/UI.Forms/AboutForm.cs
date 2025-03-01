using AcrylicUI.Forms;
using AcrylicUI.Resources;
using System.Diagnostics;
using System.Reflection;

namespace VideoBatch.UI.Forms
{
    public class AboutForm
    {
        public static void Show(IWin32Window owner)
        {
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "1.0.0";

            var message = $"VideoBatch ver: {version}\n" +
                         "Copyright © 2025 VideoBatch Limited.\n" +
                         "www.videobatch.co.uk";

            AcrylicMessageBox.ShowInformation(message, "About VideoBatch");
        }
    }
} 