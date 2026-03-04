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
        private readonly FileManager _fileManager = new();
        private readonly MarkdownService _markdownService = new();
        private readonly VaultService _vaultService = new();

        private string _currentFilePath = string.Empty;
        private string _editorText = string.Empty;
        private string _previewHtml = string.Empty;

        private ObservableCollection<Models.DirectoryItem> VaultItems { get; set; } = new();

        public string PreviewHtml { 
            get => _previewHtml;
            set
            {
                _previewHtml = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel() { 
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile());
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


        private void LoadVault(string path) { 
            VaultItems.Clear();
            var items = _vaultService.LoadVault(path);
            foreach (var item in items) {
                VaultItems.Add(item);
            }
        }
    }
}
