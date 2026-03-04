using System.Collections.Generic;
using System.IO;
using Marky.Models;

namespace Marky.Services
{
    public class VaultService {

        public DirectoryItem CreateDirectoryItem(DirectoryInfo dir) {
            var item = new DirectoryItem(dir.Name, dir.FullName, true);

            foreach (var directory in dir.GetDirectories()) { 
                item.Children.Add(CreateDirectoryItem(dir));
            }

            foreach (var file in dir.GetFiles("*.md")) {
                item.Children.Add(new DirectoryItem(
                    file.Name,
                    file.FullName,
                    false
                ));
            }

            return item;
        }


        public List<DirectoryItem> LoadVault(string path) {
            var root = new DirectoryInfo(path);
            return new List<DirectoryItem> {
                CreateDirectoryItem(root)
            };
        }
    }
}
