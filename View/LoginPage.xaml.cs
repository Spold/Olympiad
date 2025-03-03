using Olympiad.Controllers;
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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        UserController userController = new UserController(); 
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                bool result = userController.CheckLogin(LoginTextBox.Text, PasswordTextBox.Password);

                if (result)
                {
                    userController.LoginUser(LoginTextBox.Text, PasswordTextBox.Password);
                    this.NavigationService.Navigate(new OlympiadListView());

                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow?.VisabilityItem();

                

                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
