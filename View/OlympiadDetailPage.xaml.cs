using Olympiad.Controllers;
using Olympiad.Model;
using System;
using System.Collections.Generic;
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
using Olympiad.Model.PartialClasses;
using Olympiad.Services;
using System.IO;
using System.Reflection;

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для OlympiadDetailPage.xaml
    /// </summary>
    public partial class OlympiadDetailPage : Page
    {

        Core db = new Core();
        List<Olympiads> olimparr = new List<Olympiads>();
        List<Users> usersarray = new List<Users>();
        RegistrationsController registrationsController = new RegistrationsController();
        ExcelReportService excelReportService = new ExcelReportService();
        OlympiadsController olympiadsController = new OlympiadsController();
        int olimpId;
        string urlProtocol;
        string urlPosition;
        string urlArchive;
        public OlympiadDetailPage(int id)
        {
            olimparr = olympiadsController.LoadOlympiadsAndProtocols(id);

            InitializeComponent();

            foreach (var item in olimparr)
            {
                olimpId = item.OlympiadId;
                OlympiadTextBlock.Text = item.Name;
                OlympiadDateTextBlock.Text = item.StartDate.ToString("d");
                usersarray = db.context.Users.Where(x => x.UserId == item.ResponsibleTeacherUserId).ToList();
                urlPosition = item.PositionDocument;
                DocumentsVisible(item);
                ActionVisible(item.ResponsibleTeacherUserId);
               
            }

            foreach (var item in usersarray)
            {
                TeacherTextBlock.Text = item.FIO;
            }
        }

        private void BackArrowlLink(object sender, RequestNavigateEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        public void DocumentsVisible(Olympiads item)
        {
            if (item.PositionDocument != null)
            {
                urlPosition = item.PositionDocument;
            }
            else
            {
                PositionTextBlock.Text = "Положение отсутсвует";
            }

            if (item.TasksArchive != null)
            {
                urlArchive = item.TasksArchive;
            }
            else
            {
                PositionTextBlock.Text = "Задания отсутсвует";
            }

            var protocol = item.Protocols.Where(x => x.IsPublished == true && x.Status == "prepared").FirstOrDefault();
            if (protocol != null)
            {
                urlProtocol = protocol.FilePath;
            }
            else
            {
                ProtocolTextBlock.Text = "Протокол отсутствует";
            }
        }

        public void ActionVisible(int idTeacher)
        {
            if (Properties.Settings.Default.UserRole == 1)
            {
                ActionStackPanel.Visibility = Visibility.Visible;

            }
            else if (Properties.Settings.Default.UserRole == 2 && Properties.Settings.Default.UserId == idTeacher)
            {
                ActionStackPanel.Visibility = Visibility.Visible;
                TeacherStackPanel.Visibility = Visibility.Visible;
            }
        }


        private void RegistrationOlompiad_CLick(object sender, RoutedEventArgs e)
        {
            try
            {
             
                if (registrationsController.CheckUserRegistration(Properties.Settings.Default.UserId, olimpId))
                {
                    MessageBox.Show("Регистрация прошла успешно");
                    registrationsController.RegistrationOnOlimpiad(Properties.Settings.Default.UserId, olimpId);
                }
                else
                {
                    MessageBox.Show("Вы уже зарегестрировались в этой олимпиаде!");
                }
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

        }

        private void OpenLink(string relativePath)
        {
            try
            {
                // Получение абсолютного пути из относительного
                string exePath = Assembly.GetExecutingAssembly().Location;
                string exeDirectory = System.IO.Path.GetDirectoryName(exePath);
                string absolutePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDirectory, relativePath));

                if (File.Exists(absolutePath))
                {
                    Process.Start(new ProcessStartInfo(absolutePath) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("Файл не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}");
            }
        }

       

        private void PositionLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenLink(urlPosition);
        }

        private void ProtocolLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenLink(urlProtocol);
        }

        private void ArchiveLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenLink(urlArchive);
        }

        private void EditResults_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProtocolEditPage(olimpId));
        }

        private void EditDocument_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new OlimpiadDocumentPage(olimpId));
        }

        private void EditCertificate_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CertificatesPage(olimpId));
        }

        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            excelReportService.ExportOlympiadReport(olimpId);
        }

       
    }
}
