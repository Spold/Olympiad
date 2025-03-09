using Olympiad.Model;
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
    /// Логика взаимодействия для ProfileUserPage.xaml
    /// </summary>
    public partial class ProfileUserPage : Page
    {
        Core db = new Core();
        Users user = new Users();
        public ProfileUserPage()
        {
            InitializeComponent();
            DateTime.TryParse(user.DateOfBirth.ToString(), out DateTime bday);
            user = db.context.Users.Where(x => x.UserId == Properties.Settings.Default.UserId).FirstOrDefault();
            UserNameTextBlock.Text = user.FIO;
            EmailTextBlock.Text = user.Email;
            BirthdayTextBlock.Text = bday.ToString("d");
            SpecializationTextBlock.Text = user.EducationalInstitution;
        }



        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new EditUserPage(Properties.Settings.Default.UserId));
        }

        private void BackArrowlLink(object sender, RequestNavigateEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
