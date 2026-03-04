using System.Collections.Generic;
using System.IO;
using Marky.Models;

namespace Marky.Services
{
    public class VaultService {

        public List<DirectoryItem> LoadVault(string path) {
            var root = new DirectoryItem(path);
            return new List<DirectoryItem>() {
                CreateDirectoryItem(root)
            };
        }

        private DirectoryItem CreateDirectoryItem(DirectoryInfo directory) {
            var item = new DirectoryItem {
                Name = directory.Name,
                FullPath = directory.FullName,
                IsDirectory = true
            };


            foreach (var dir in directory.GetDirectories()) { 
                item.Children.Add(CreateDirectoryItem(dir));
            }

            foreach (var file in directory.GetFiles("*.md")) {
                item.Children.Add(new DirectoryItem { 
                    Name = file.Name,
                    FullPath = file.FullName,
                    IsDirectory = false
                });
            }

            return item;
        }

        





    }
}
