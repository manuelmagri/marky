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

using System.Windows.Threading;
using Microsoft.Win32;
using Markdig;
using Marky.Services;


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

        public MainWindow()
        {
            InitializeComponent();

            // File autosave
            _autoSaveTimer = new DispatcherTimer();
            _autoSaveTimer.Interval = TimeSpan.FromMicroseconds(500);
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;

            // Preview Side
            InitializeWebView();
        }

        // Autosave
        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            _autoSaveTimer.Stop();

            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                _fileManager.SaveFile(_currentFilePath, TextBox.Text);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (!string.IsNullOrEmpty(_currentFilePath)) { 
                _autoSaveTimer.Stop();
                _autoSaveTimer.Start();
            }

            Render_Markdown();
        }


        // WebView
        private async void InitializeWebView() {

            try
            {
                await PreviewWebView.EnsureCoreWebView2Async(null);
            }
            catch (Exception e) {
                // Log            
                MessageBox.Show($"WebView Initialization Error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Fallback to markdown rendering without WebView
                Editor.Width = new GridLength(1, GridUnitType.Star);
                Preview.Width = new GridLength(0);
            }
            
        }

        // Markdown
        private void Render_Markdown() {

            if (PreviewWebView.CoreWebView2 == null) {
                return;
            }

            string html_body = Markdown.ToHtml(TextBox.Text);

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

        private void Open_Click(object sender, RoutedEventArgs e) {
            OpenFile();
        }
        
        private void Save_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(_currentFilePath)) {
                _fileManager.SaveFile(_currentFilePath, TextBox.Text);
            }
        }


        // VIEWS
        // EDITOR ONLY VIEW
        private void Editor_Only_Click(object sender, RoutedEventArgs e) {
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
        private void Split_View_Click(object sender, RoutedEventArgs e) {
            Editor.Width = new GridLength(1, GridUnitType.Star);
            Preview.Width = new GridLength(1, GridUnitType.Star);
        }


    }

}