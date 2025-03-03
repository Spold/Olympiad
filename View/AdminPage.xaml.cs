using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddUserPage());
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog();
            if (dialog.ShowDialog() == true)
            {
                this.NavigationService.Navigate(new EditUserPage(int.Parse(dialog.UserId)));
            }
        }

        private void AddOlimpiada_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddOlimpiadPage());
        }

        private void Protocols_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProtocolsPage());
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ReportPage());
        }
    }
}
