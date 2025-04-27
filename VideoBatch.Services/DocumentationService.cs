using Markdig;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace VideoBatch.Services
{
    /// <summary>
    /// Implementation of IDocumentationService that renders Markdown documentation as HTML
    /// </summary>
    public class DocumentationService : IDocumentationService
    {
        private readonly ILogger<DocumentationService> _logger;
        private const string README_FILENAME = "README.md";
        private const string HTML_TEMPLATE = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>VideoBatch Documentation</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
            max-width: 900px;
            margin: 0 auto;
            padding: 2rem;
            background-color: #1e1e1e;
            color: #d4d4d4;
        }}
        h1, h2, h3, h4 {{ 
            color: #ffffff;
            margin-top: 2rem;
        }}
        a {{
            color: #569cd6;
            text-decoration: none;
        }}
        a:hover {{
            text-decoration: underline;
        }}
        code {{
            background-color: #2d2d2d;
            padding: 0.2em 0.4em;
            border-radius: 3px;
            font-family: Consolas, Monaco, 'Courier New', monospace;
        }}
        pre code {{
            display: block;
            padding: 1em;
            overflow-x: auto;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 1rem 0;
        }}
        th, td {{
            border: 1px solid #404040;
            padding: 0.5rem;
        }}
        th {{
            background-color: #2d2d2d;
        }}
        hr {{
            border: none;
            border-top: 1px solid #404040;
            margin: 2rem 0;
        }}
    </style>
</head>
<body>
    {0}
</body>
</html>";

        public DocumentationService(ILogger<DocumentationService> logger)
        {
            _logger = logger;
        }

        public async Task ShowDocumentationAsync()
        {
            try
            {
                string readmePath = GetReadmePath();
                _logger.LogInformation("Loading documentation from: {Path}", readmePath);
                
                if (!File.Exists(readmePath))
                {
                    throw new FileNotFoundException($"Documentation file not found at {readmePath}. Please ensure README.md is included with the application.", readmePath);
                }

                string markdownContent = await File.ReadAllTextAsync(readmePath);
                
                // Configure Markdig with common extensions
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build();

                // Convert Markdown to HTML
                string htmlContent = Markdown.ToHtml(markdownContent, pipeline);

                // Create full HTML document with styling
                string fullHtml = string.Format(HTML_TEMPLATE, htmlContent);

                // Create a temporary HTML file
                string tempHtmlPath = Path.Combine(Path.GetTempPath(), "VideoBatch_Documentation.html");
                await File.WriteAllTextAsync(tempHtmlPath, fullHtml);

                // Open the HTML file in the default browser
                var psi = new ProcessStartInfo
                {
                    FileName = tempHtmlPath,
                    UseShellExecute = true
                };
                Process.Start(psi);

                _logger.LogInformation("Documentation opened in default browser");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing documentation");
                throw;
            }
        }

        private string GetReadmePath()
        {
            // First try the executable directory
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string readmePath = Path.Combine(exePath, README_FILENAME);

            if (File.Exists(readmePath))
            {
                return readmePath;
            }

            // If not found and we're in development (bin/Debug/net9.0-windows), try going up to project root
            if (exePath.Contains("bin") && (exePath.Contains("Debug") || exePath.Contains("Release")))
            {
                readmePath = Path.GetFullPath(Path.Combine(exePath, "..", "..", "..", README_FILENAME));
                if (File.Exists(readmePath))
                {
                    return readmePath;
                }
            }

            // Default to executable directory path even if not found (will throw appropriate error)
            return Path.Combine(exePath, README_FILENAME);
        }
    }
} 