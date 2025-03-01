using Markdig;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace VideoBatch.Services
{
    public interface IDocumentationService
    {
        Task ShowDocumentationAsync();
    }

    public class DocumentationService : IDocumentationService
    {
        private readonly ILogger<DocumentationService> _logger;
        private const string README_PATH = "README.md";
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
                string markdownContent = await File.ReadAllTextAsync(README_PATH);
                
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
    }
} 