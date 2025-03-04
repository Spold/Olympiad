using Olympiad.Controllers;
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
using static System.Net.Mime.MediaTypeNames;

namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        UserController userController = new UserController();
        int role = 3;
        public AddUserPage()
        {
            InitializeComponent();
            CourseComboBox.SelectedIndex = 0;
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;
            string firstname = FirstNameTextBox.Text;
            string lastname = LastNameTextBox.Text;
            string patronymic = MiddleNameTextBox.Text;
            string email = EmailTextBox.Text;
            DateTime? dateTime = null;
            if (BirthDatePicker.SelectedDate.HasValue)
            {
                dateTime = BirthDatePicker.SelectedDate.Value;
            }
            string institution = InstitutionTextBox.Text;
            string educationLevel = EducationLevelComboBox.Text;
            int.TryParse(CourseComboBox.Text, out int courseNumber);
            string specialization = SpecializationTextBox.Text;



           try
            {
                if (Properties.Settings.Default.UserId == 1)
                {
                    role = 2;
                }
                bool result = userController.CheckNewUser(login, password, role, firstname, lastname, patronymic, email, dateTime, institution, educationLevel, courseNumber, specialization, true);

                if (result)
                {
                    userController.AddNewUser(login, password, role, firstname, lastname, patronymic, email, dateTime, institution, educationLevel, courseNumber, specialization);
                    this.NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
