using Olympiad.Controllers;
using Olympiad.Model;
using Olympiad.Model.PartialClasses;
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

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для ProtocolsPage.xaml
    /// </summary>
    public partial class ProtocolsPage : Page
    {
        Core db = new Core();
        ProtocolsController protocolsController = new ProtocolsController();
        string url;

        public ProtocolsPage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            var data = protocolsController.LoadPreparedProtocols();

            OlympiadProtocolsList.ItemsSource = data;

            foreach (var o in data)
            {
                url = o.FilePath;
            }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {

            Button button = sender as Button;
            var report = button.DataContext as ProtocolViewModel;

            MessageBoxResult rez = MessageBox.Show("Опубликовать этот протокол?", "Публикация", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (rez == MessageBoxResult.Yes)
            {
                try
                {
                    if (protocolsController.PublishProtocol(report.ProtocolId))
                    {
                        LoadData();
                    }

                    
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка публикации.", "Публикация", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadData();
                }
            }
        }

        private void ReportView_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var report = button.DataContext as Protocols;

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}");
            }

        }

    }
}
