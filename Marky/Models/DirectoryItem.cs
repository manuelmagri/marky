using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marky.Models
{
    public class DirectoryItem
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }

        // TODO: Constructor
        // public DirectoryItem(string path) {}

        public List<DirectoryItem> Children { get; set; } = new();
        
    }
}
