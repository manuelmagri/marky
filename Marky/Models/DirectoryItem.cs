using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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


        // Constructor for creating a DirectoryItem
        // from explicit parameters
        public DirectoryItem(string name, string fullPath, bool isDirectory)
        {
            Name = name;
            FullPath = fullPath;
            IsDirectory = isDirectory;
        }

        // For directories, this will hold the child items
        // (both files and subdirectories)
        // OLD: public List<DirectoryItem> Children { get; set; } = new();
        public ObservableCollection<DirectoryItem> Children { get; } = new();
        
    }
}
