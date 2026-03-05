using Markdig;

namespace Marky.Services
{
    public class MarkdownService
    {
        public string ConvertToHtml(string md)
        {
            return Markdown.ToHtml(md);
        }
    }
}
