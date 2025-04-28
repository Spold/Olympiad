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
using System.Text.RegularExpressions;

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для OlympiadDetailPage.xaml
    /// </summary>
    public partial class OlympiadDetailPage : Page
    {

        Core db = new Core();
        Olympiads olymp = new Olympiads();
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
            olymp = olympiadsController.LoadOlympiadsAndProtocols(id);

            InitializeComponent();


                olimpId = olymp.OlympiadId;
                OlympiadTextBlock.Text = olymp.Name;
                OlympiadDateTextBlock.Text = olymp.StartDate.ToString("d");
                usersarray = db.context.Users.Where(x => x.UserId == olymp.ResponsibleTeacherUserId).ToList();
                urlPosition = olymp.PositionDocument;
                DocumentsVisible(olymp);
                ActionVisible(olymp.ResponsibleTeacherUserId);
                ButtonState();


            

            foreach (var item in usersarray)
            {
                TeacherTextBlock.Text = item.FIO;
            }
        }

        private void ButtonState()
        {
            try
            {
                var olympiad = db.context.Olympiads.FirstOrDefault(x => x.OlympiadId == olimpId);
                if (olympiad == null) return;

                if (DateTime.Now > olympiad.EndDate)
                {
                    ParticipateButton.Style = (Style)FindResource("DisabledButton");
                    ParticipateButton.Content = "Олимпиада прошла";
                    return;
                }

                if (DateTime.Now >= olympiad.StartDate)
                {
                    ParticipateButton.Style = (Style)FindResource("DisabledButton");
                    ParticipateButton.Content = "Олимпиада уже идет";
                    return;
                }

                if (!registrationsController.CheckUserRegistration(Properties.Settings.Default.UserId, olimpId, false))
                {
                    ParticipateButton.Style = (Style)FindResource("RedButton");
                    ParticipateButton.Content = "Отменить регистрацию";
                    return;
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            
        }
        private void BackArrowlLink(object sender, RequestNavigateEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        public void DocumentsVisible(Olympiads item)
        {

            // Проверка и установка положения
            if (!string.IsNullOrEmpty(item.PositionDocument))
            {
                urlPosition = item.PositionDocument;
            }
            else
            {
                PositionTextBlock.Text = "Положение отсутствует";
            }


            if (!string.IsNullOrEmpty(item.TasksArchive))
            {
                urlArchive = item.TasksArchive;
            }
            else
            {
                ArchiveTextBlock.Text = "Задания отсутствуют";
            }

            var activeProtocol = item.Protocols?
                .FirstOrDefault(p => p.IsPublished && p.Status == "prepared");

            if (activeProtocol != null && !string.IsNullOrEmpty(activeProtocol.FilePath))
            {
                urlProtocol = activeProtocol.FilePath;
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
             
                if (registrationsController.CheckUserRegistration(Properties.Settings.Default.UserId, olimpId, true))
                {
                    MessageBox.Show("Регистрация прошла успешно");
                    registrationsController.RegistrationOnOlimpiad(Properties.Settings.Default.UserId, olimpId);
                    ButtonState();
                }
                else
                {
                    MessageBoxResult rez = MessageBox.Show("Вы уже зарегестрировались в этой олимпиаде! Хотите отменить регистрацию?", "Регистрация", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (rez == MessageBoxResult.Yes)
                    {
                        try
                        {
                            registrationsController.DeleteRegistation(Properties.Settings.Default.UserId, olimpId);
                            MessageBox.Show("Регистрация удалена");
                            ReloadPage();
                            ButtonState();

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Ошибка регистрации.", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

        }

        private void ReloadPage()
        {
            this.NavigationService.Navigate(new OlympiadDetailPage(olimpId));
            if (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void OpenLink(string relativePath)
        {
            try
            {
                string pattern = @"^(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$";

                // Проверка через Regex.IsMatch
                if (Regex.IsMatch(relativePath, pattern, RegexOptions.IgnoreCase))
                {
                    Process.Start(new ProcessStartInfo(relativePath) { UseShellExecute = true });
                }
                else
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

        private void EditOlympiad_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new EditOlympiadPage(id: olimpId));
        }
    }
}
