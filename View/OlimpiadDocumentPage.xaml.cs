using Olympiad.Controllers;
using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using static Olympiad.View.ProtocolEditPage;
using Microsoft.Win32;
using System.Reflection;

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для OlimpiadDocumentPage.xaml
    /// </summary>
    public partial class OlimpiadDocumentPage : Page
    {
        int olimpId;
        Core db = new Core();
        OlympiadsController olympiadsController = new OlympiadsController();


        static string baseDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, @"..\.."));


        string _protocolsPath = System.IO.Path.Combine(projectRoot, "Assets", "Documents", "Protocols");
        string _positionsPath = System.IO.Path.Combine(projectRoot, "Assets", "Documents", "Position");
        string _archivePath = System.IO.Path.Combine(projectRoot, "Assets", "Documents", "Archive");

        public OlimpiadDocumentPage(int olimpiadId)
        {

            InitializeComponent();
            ProtocolStatusBox.ItemsSource = new List<string>
        {
            "draft",
            "prepared"
        };

            olimpId = olimpiadId;

            var olimparr = db.context.Olympiads.Include("Protocols").Where(x => x.OlympiadId == olimpiadId).ToList();

            foreach (var item in olimparr)
            {
                OlimpiadTextBlock.Text = item.Name;
                PositionTextBox.Text = item.PositionDocument;
                TeskArchiveTextBox.Text = item.TasksArchive;
                var protocol = item.Protocols.FirstOrDefault();
                if (protocol != null)
                {
                    ProtocolTextBox.Text = protocol.FilePath;
                    ProtocolStatusBox.SelectedItem = protocol.Status;
                }
                else
                {
                    ProtocolStatusBox.SelectedIndex = 0;
                }

            }

          
        }

        private void HandleFileSelection(string targetFolder, TextBox targetTextBox, string documentType)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string originalFilePath = openFileDialog.FileName;
                    string fileExtension = System.IO.Path.GetExtension(originalFilePath);
                    string fileName = GenerateFileName(documentType, fileExtension);
                    string destinationPath = System.IO.Path.Combine(targetFolder, fileName);

                    File.Copy(originalFilePath, destinationPath, true);

                    string exePath = Assembly.GetExecutingAssembly().Location;
                    string exeDirectory = System.IO.Path.GetDirectoryName(exePath);
                    Uri exeUri = new Uri(exeDirectory + System.IO.Path.DirectorySeparatorChar);
                    Uri destUri = new Uri(destinationPath);
                    Uri relativeUri = exeUri.MakeRelativeUri(destUri);
                    string relativePath = Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', '\\');

                    targetTextBox.Text = relativePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе файла: {ex.Message}");
            }
        }

        private string GenerateFileName(string documentType, string extension)
        {
            switch (documentType)
            {
                case "Position":
                    return $"Положение по {OlimpiadTextBlock.Text}{extension}";
                case "Archive":
                    return $"Архив заданий {OlimpiadTextBlock.Text}{extension}";
                case "Protocol":
                    return $"Протокол {OlimpiadTextBlock.Text}{extension}";
                default:
                    return $"Документ {DateTime.Now:yyyyMMddHHmmss}{extension}";
            }
        }


        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                olympiadsController.UpdateOlympiadData(olimpId, PositionTextBox.Text, TeskArchiveTextBox.Text, ProtocolTextBox.Text, ProtocolStatusBox.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void SelectProtocolBtn_Click(object sender, RoutedEventArgs e)
        {
            HandleFileSelection(_protocolsPath, ProtocolTextBox, "Protocol");
        }

        private void SelectArchiveBtn_Click(object sender, RoutedEventArgs e)
        {
            HandleFileSelection(_archivePath, TeskArchiveTextBox, "Archive");
        }

        private void SelectPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            HandleFileSelection(_positionsPath, PositionTextBox, "Position");
        }
    }
}
