using Marky.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Marky.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        
        private Models.DirectoryItem? _selectedVaultItem;

        private readonly FileManager _fileManager = new();
        private readonly MarkdownService _markdownService = new();
        private readonly VaultService _vaultService = new();

        private string _currentFilePath = string.Empty;
        private string _editorText = string.Empty;
        private string _previewHtml = string.Empty;


        public MainViewModel()
        {
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile());
            OpenVaultCommand = new RelayCommand(_ => OpenVault());
        }


        public Models.DirectoryItem? SelectedItem
        {
            get => _selectedVaultItem;
            set {
                _selectedVaultItem = value;
                OnPropertyChanged();
                OpenSelectedFile();
            }
        }


        public ObservableCollection<Models.DirectoryItem> VaultItems { get; } = new();

        public string PreviewHtml { 
            get => _previewHtml;
            set
            {
                _previewHtml = value;
                OnPropertyChanged();
            }
        }


        public string EditorText { 
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


        private void OpenFile() {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Markdown files (*.md)|*.md";

            if (dialog.ShowDialog() == true) {
                _currentFilePath = dialog.FileName;
                EditorText = _fileManager.OpenFile(_currentFilePath);
            }
        }


        private void SaveFile() {
            if (!string.IsNullOrEmpty(_currentFilePath)) { 
                _fileManager.SaveFile(_currentFilePath, EditorText);
            }
        }


        private void RenderMarkdown() {
            if (EditorText == null) {
                return;
            }

            PreviewHtml = _markdownService.ConvertToHtml(EditorText);
        }


        public void LoadVault(string path) { 
            VaultItems.Clear();
            var items = _vaultService.LoadVault(path);
            foreach (var item in items) {
                VaultItems.Add(item);
            }
        }

        private void OpenVault() {
            var dialog = new Microsoft.Win32.OpenFolderDialog {
                Title = "Select the Vault folder",
                Multiselect = false,
            };

            if (dialog.ShowDialog() == true) {
                LoadVault(dialog.FolderName);
            }
        }

        private void OpenSelectedFile() { 
            if (SelectedItem == null || SelectedItem.IsDirectory) {
                return;
            }

            _currentFilePath = SelectedItem.FullPath;
            EditorText = _fileManager.OpenFile(_currentFilePath);
        }
    }
}
