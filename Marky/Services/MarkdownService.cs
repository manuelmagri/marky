using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Markdig;

namespace Marky.Services
{
    public class MarkdownService
    {
        public string ConvertToHtml(string md) {
            return Markdown.ToHtml(md);
        }
    }
}
