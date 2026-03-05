using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Windows.Threading;
using Microsoft.Win32;
using Markdig;
using Marky.Services;
using Marky.Models;
using System.Runtime.CompilerServices;


namespace Marky
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileManager _fileManager = new FileManager();
        private string _currentFilePath = string.Empty;
        private DispatcherTimer _autoSaveTimer;
        private MarkdownService _markdownService = new();

        public MainWindow()
        {
            InitializeComponent();
            _autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;
            
            var vm = new ViewModel.MainViewModel();
            vm.PropertyChanged += Vm_PropertyChanged;
            DataContext = vm;
            
            InitializeWebView();
        }
















        // Autosave
        private void AutoSaveTimer_Tick(object? sender, EventArgs e)
        {
            _autoSaveTimer.Stop();

            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                _fileManager.SaveFile(_currentFilePath, TextBox.Text);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                CreateTemponaryFile();
                //_autoSaveTimer.Stop();
                //_autoSaveTimer.Start();
            }

            _autoSaveTimer.Stop();
            _autoSaveTimer.Start();

            Render_Markdown();
        }


        // WebView
        private async void InitializeWebView()
        {

            try
            {
                await PreviewWebView.EnsureCoreWebView2Async(null);
            }
            catch (Exception e)
            {
                // Log            
                MessageBox.Show($"WebView Initialization Error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Fallback to markdown rendering without WebView
                Editor.Width = new GridLength(1, GridUnitType.Star);
                Preview.Width = new GridLength(0);
            }

        }

        // Markdown rendering
        private void Render_Markdown()
        {

            if (PreviewWebView.CoreWebView2 == null)
            {
                return;
            }

            string html_body = _markdownService.ConvertToHtml(TextBox.Text);

            string full_html = $@"    
                <html>
                    <head>
                        <meta charset='UTF-8'>
                        <style>
                            body {{
                                font-family: Segoe UI;
                                padding: 20px;
                                background-color: #1e1e1e;
                                color: white;
                            }}
                            h1, h2, h3 {{
                                color: #4FC3F7;
                            }}
                            code {{
                                background-color: #2d2d2d;
                                padding: 2px 4px;
                                border-radius: 4px;
                            }}
                        </style>
                    </head>
                    <body>
                        {html_body}
                    </body>
                    </html>";

            PreviewWebView.NavigateToString(full_html);
        }


        // File management

        // Open File
        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                _currentFilePath = dialog.FileName;
                TextBox.Text = _fileManager.OpenFile(_currentFilePath);
            }
        }

        // Save File
        private void SaveFile()
        {
            //if (_isTemponaryFile == true)
            //{

            //    SaveFileDialog dialog = new SaveFileDialog();
            //    dialog.Filter = "Markdown files (*.md)|*.md";

            //    if (dialog.ShowDialog() == true)
            //    {
            //        _currentFilePath = dialog.FileName;
            //        _fileManager.SaveFile(_currentFilePath, TextBox.Text);
            //        _isTemponaryFile = false;
            //    }

            //}
            //else if (!string.IsNullOrEmpty(_currentFilePath))
            //{
            //    _fileManager.SaveFile(_currentFilePath, TextBox.Text);
            //}


        }


        // Create temponary file for new documents
        private void CreateTemponaryFile()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = System.IO.Path.Combine(appDataPath, "Marky");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            string tmpFilePath = System.IO.Path.Combine(appFolder, "Untitled.md");

            _currentFilePath = tmpFilePath;

            // If the file doesn't exist, create it
            if (!File.Exists(tmpFilePath))
            {
                File.WriteAllText(tmpFilePath, "Untitled");
            }

        }

        // Menu button inside File
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        // Save button inside File
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Function not added yet.", "", MessageBoxButton.OK);
        }

        // Save As button inside File
        private void Save_As_Click(object sender, RoutedEventArgs e)
        {
            //SaveFile();
            MessageBox.Show("Function not added yet.", "", MessageBoxButton.OK);
        }


        // EDITOR ONLY VIEW
        private void Editor_Only_Click(object sender, RoutedEventArgs e)
        {
            Editor.Width = new GridLength(1, GridUnitType.Star);
            Preview.Width = new GridLength(0);
        }

        // PREVIEW ONLY VIEW
        private void Preview_Only_Click(object sender, RoutedEventArgs e)
        {
            Editor.Width = new GridLength(0);
            Preview.Width = new GridLength(1, GridUnitType.Star);
        }

        // SPLIT VIEW
        private void Split_View_Click(object sender, RoutedEventArgs e)
        {
            Editor.Width = new GridLength(1, GridUnitType.Star);
            Preview.Width = new GridLength(1, GridUnitType.Star);
        }

        private void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "PreviewHtml") {
                if (DataContext is ViewModel.MainViewModel vm) {
                    PreviewWebView.NavigateToString($@"
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                        </head>
                        <body>
                            {vm.PreviewHtml}
                        </body>
                        </html>");
                }
            }
        }
    }
}