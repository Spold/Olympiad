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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Olympiad.Services;

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для ProtocolEditPage.xaml
    /// </summary>
    public partial class ProtocolEditPage : Page
    {

        Core db = new Core();
        Users users = new Users();
        Olympiads olympiad = new Olympiads();
        RegistrationsController registrationsVM = new RegistrationsController();
        ExcelReportService excelReportService = new ExcelReportService();
        int olympId = 0;

        public ProtocolEditPage(int olympiadId)
        {
            InitializeComponent();
            olympId = olympiadId;
            olympiad = db.context.Olympiads.FirstOrDefault(x => x.OlympiadId == olympiadId);
            var participants = registrationsVM.ProtocolsEdit(olympiadId);
           
            ParticipantsGrid.ItemsSource = participants;
            

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                    var updatedParticipants = ParticipantsGrid.ItemsSource as List<ParticipantInfo>;

                    if (updatedParticipants == null)
                    {
                        MessageBox.Show("Нет данных для сохранения.");
                        return;
                    }

                    foreach (var participant in updatedParticipants)
                    {
                        var registration = db.context.Registrations
                            .Include(r => r.Results)
                            .FirstOrDefault(r => r.RegistrationId == participant.RegistrationId);

                        if (registration == null)
                        {
                            MessageBox.Show($"Регистрация с ID {participant.RegistrationId} не найдена.");
                            continue;
                        }

                        var result = registration.Results.FirstOrDefault();
                        if (result == null)
                        {
                            result = new Results
                            {
                                RegistrationId = participant.RegistrationId,
                                ProtocolId = GetOrCreateProtocolId(db, registration.OlympiadId)
                            };
                            db.context.Results.Add(result);
                        }

                        result.Score = participant.Score ?? 0;
                        result.ResultType = participant.Result;
                }

                    db.context.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private int GetOrCreateProtocolId(Core db, int olympiadId)
        {
            var protocol = db.context.Protocols
                .FirstOrDefault(p => p.OlympiadId == olympiadId);

            if (protocol == null)
            {
                protocol = new Protocols
                {
                    OlympiadId = olympiadId,
                    Status = "draft",
                    FilePath = null,
                    IsPublished = false
                };
                db.context.Protocols.Add(protocol);
                db.context.SaveChanges();
            }

            return protocol.ProtocolId;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton_Click(sender, e);
            excelReportService.ExportOlympiadReport(olympId);


        }

    }
}
