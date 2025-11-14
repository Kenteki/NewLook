using Markdig;
using System.Text.RegularExpressions;

namespace NewLook.Services
{
    public class MarkdownService
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdownService()
        {
            // Configure the Markdown pipeline with extensions
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
        }

        public string ToHtml(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return string.Empty;

            var html = Markdown.ToHtml(markdown, _pipeline);

            // Sanitize HTML to remove potentially dangerous attributes
            html = SanitizeHtml(html);

            return html;
        }

        private string SanitizeHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Remove ALL attributes that start with @ (Blazor directives) - more aggressive pattern
            // This handles: @onclick, @bind, @ref, etc.
            html = Regex.Replace(html, @"@\w+(?:-\w+)*(?::\w+)?(?:=[""'][^""']*[""']|=[^\s>]*)?", string.Empty, RegexOptions.IgnoreCase);

            // Also remove any standalone @ symbols followed by word characters that might be directives
            html = Regex.Replace(html, @"(?<=[<\s])@(?=\w)", string.Empty, RegexOptions.IgnoreCase);

            // Remove potentially dangerous event handlers (onclick, onload, etc.)
            html = Regex.Replace(html, @"\son\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);

            // Remove javascript: protocol
            html = Regex.Replace(html, @"javascript:", "blocked:", RegexOptions.IgnoreCase);

            // Remove any remaining @ symbols in attribute positions
            html = html.Replace("@onclick", "").Replace("@bind", "").Replace("@ref", "");

            return html;
        }
    }
}
