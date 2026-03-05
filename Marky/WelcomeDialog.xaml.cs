using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Marky
{
    public partial class WelcomeDialog : Window
    {
        public string? SelectedVaultPath { get; private set; }
        public bool IsNewVault { get; private set; }

        public WelcomeDialog()
        {
            InitializeComponent();
        }

        private void OpenExistingVault_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Vault Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedVaultPath = dialog.FolderName;
                IsNewVault = false;
                DialogResult = true;
                Close();
            }
        }

        private void CreateNewVault_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
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

                SelectedVaultPath = fullPath;
                IsNewVault = true;
                DialogResult = true;
                Close();
            }
        }
    }
}
