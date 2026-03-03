using System.IO;

namespace Marky.Services
{
    public class FileManager
    {
        // 
        public string OpenFile(string path) {
            return File.ReadAllText(path);
        }

        // 
        public void SaveFile(string path, string content) {
            File.WriteAllText(path, content);
        }

        //
        //public void CloseFile(){}


    }
}
