using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

using Marky.Services;

namespace Marky.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        private Models.DirectoryItem? _selectedVaultItem;

        private readonly FileManager _fileManager = new();
        private readonly MarkdownService _markdownService = new();
        private readonly VaultService _vaultService = new();

        private string? _currentFilePath;
        private string? _editorText;
        private string? _previewHtml;
        private string? _lastVaultPath;


        public MainViewModel()
        {
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile());
            OpenVaultCommand = new RelayCommand(_ => OpenVault());
            CreateVaultCommand = new RelayCommand(_ => CreateVault());
            LoadLastVault();
        }


        public Models.DirectoryItem? SelectedItem
        {
            get => _selectedVaultItem;
            set
            {
                _selectedVaultItem = value;
                OnPropertyChanged();
                OpenSelectedFile();
            }
        }


        public ObservableCollection<Models.DirectoryItem> VaultItems { get; } = new();

        public string PreviewHtml
        {
            get => _previewHtml;
            set
            {
                _previewHtml = value;
                OnPropertyChanged();
            }
        }


        public string EditorText
        {
            get => _editorText;
            set
            {
                _editorText = value;
                OnPropertyChanged();
                RenderMarkdown();
            }
        }


        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand OpenVaultCommand { get; }
        public ICommand CreateVaultCommand { get; }


        private void OpenFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Markdown files (*.md)|*.md";

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                EditorText = _fileManager.OpenFile(_currentFilePath);
            }
        }


        private void SaveFile()
        {
            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                _fileManager.SaveFile(_currentFilePath, EditorText);
            }
        }

        public void SaveLastVaultPath(string path)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsFolder = Path.Combine(appDataPath, "Marky");
            string settingsPath = Path.Combine(settingsFolder, "settings.json");

            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
            }

            var settings = new Dictionary<string, string>
            {
                { "lastVaultPath", path }
            };

            var json = JsonSerializer.Serialize(settings);


            File.WriteAllText(settingsPath, json);

            _lastVaultPath = path;
        }


        private void RenderMarkdown()
        {
            if (EditorText == null)
            {
                return;
            }

            PreviewHtml = _markdownService.ConvertToHtml(EditorText);
        }


        public void LoadVault(string path)
        {
            VaultItems.Clear();
            var items = _vaultService.LoadVault(path);
            foreach (var item in items)
            {
                VaultItems.Add(item);
            }
        }

        public void LoadLastVault()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsPath = Path.Combine(appDataPath, "Marky", "settings.json");

            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (settings != null && settings.TryGetValue("lastVaultPath", out var path))
                {
                    _lastVaultPath = path;
                }

            }
        }

        private void OpenVault()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select the Vault folder",
                Multiselect = false,
            };

            if (dialog.ShowDialog() == true)
            {
                LoadVault(dialog.FolderName);
            }
        }

        private void OpenSelectedFile()
        {
            if (SelectedItem == null || SelectedItem.IsDirectory)
            {
                return;
            }

            _currentFilePath = SelectedItem.FullPath;
            EditorText = _fileManager.OpenFile(_currentFilePath);
        }


        public string? GetLastVault() => _lastVaultPath;

        public bool HasLastVault()
        {
            return !string.IsNullOrEmpty(_lastVaultPath) && Directory.Exists(_lastVaultPath);
        }

        private void CreateVault()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Create New Vault",
                Filter = "Folder|*.folder",
                FileName = "Select"
            };

            if (dialog.ShowDialog() == true)
            {
                string folderPath = Path.GetDirectoryName(dialog.FileName);
                string folderName = Path.GetFileNameWithoutExtension(dialog.FileName);
                string fullPath = Path.Combine(folderPath!, folderName);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);

                    string welcomeFilePath = Path.Combine(fullPath, "Welcome.md");
                    string welcomeContent = @"# Welcome to Marky!

                        Start writing your markdown files here.

                        ## Features
                        - Live preview
                        - Vault-based file management
                        - Auto-save
                        ";
                    File.WriteAllText(welcomeFilePath, welcomeContent);
                }

                SaveLastVaultPath(fullPath);
                LoadVault(fullPath);
            }
        }
    }
}
