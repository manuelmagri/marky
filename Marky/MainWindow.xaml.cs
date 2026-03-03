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
        private string _currentFilePath;
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
            await PreviewWebView.EnsureCoreWebView2Async(null);
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

    }

}