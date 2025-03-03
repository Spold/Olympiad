using Olympiad.View;
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

namespace Olympiad
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new OlympiadListView());
            VisabilityItem(); 

        }

        private void AllOlympiadsLink(object sender, RequestNavigateEventArgs e)
        {
            MainFrame.Navigate(new OlympiadListView());
        }

        private void ProfileLink(object sender, RequestNavigateEventArgs e)
        {
            if (Properties.Settings.Default.UserId == 0)
            {
                MainFrame.Navigate(new LoginPage());
            }
            else
            {
                MainFrame.Navigate(new ProfileUserPage());
            }
        }

        private void ResultsLink(object sender, RequestNavigateEventArgs e)
        {
            MainFrame.Navigate(new ResultUserPage(Properties.Settings.Default.UserId));
        }

        private void AdminPanelLink(object sender, RequestNavigateEventArgs e)
        {
            if (Properties.Settings.Default.UserRole == 1)
            {
                MainFrame.Navigate(new AdminPage());
            }
            else if(Properties.Settings.Default.UserRole == 2){
                MessageBox.Show("Учитель пока не готов");
            }
        }

        private void LogoutLink(object sender, RequestNavigateEventArgs e)
        {
            if (Properties.Settings.Default.UserId == 0)
            {
                MainFrame.Navigate(new LoginPage());
            }
            else
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                VisabilityItem();
                MainFrame.Navigate(new LoginPage());
            }
        }

        public void VisabilityItem()
        {
            AdminPanel.Visibility =
                (Properties.Settings.Default.UserRole == 1 || Properties.Settings.Default.UserRole == 2)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            LogoutText.Text = Properties.Settings.Default.UserId == 0
                ? "Войти"
                : "Выйти";
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }
    }
}
