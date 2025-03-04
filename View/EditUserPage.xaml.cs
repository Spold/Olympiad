using Olympiad.Controllers;
using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для EditUserPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {
        Core db = new Core();
        Users user = new Users();
        UserController userController = new UserController();
        int userId;

        public EditUserPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            List<string> educationList = new List<string>() { "Высшее", "Среднее профессиональное", "Среднее общее"};
            List<int> courseNumberList = new List<int>() {1,2,3,4,5};
            EducationLevelComboBox.ItemsSource = educationList;
            CourseComboBox.ItemsSource = courseNumberList;
            LoadUserData();
        }

        public void LoadUserData()
        {
            try
            {
                user = db.context.Users.Where(x => x.UserId == userId).FirstOrDefault();

                if (user != null)
                {
                    LoginTextBox.Text = user.Login;
                    PasswordBox.Password = user.PasswordHash;
                    FirstNameTextBox.Text = user.FirstName;
                    LastNameTextBox.Text = user.LastName;
                    MiddleNameTextBox.Text = user.Patronymic;
                    EmailTextBox.Text = user.Email;
                    BirthDatePicker.SelectedDate = user.DateOfBirth;
                    EducationLevelComboBox.SelectedItem = user.EducationLevel;
                    CourseComboBox.SelectedItem = user.CourseNumber;
                    InstitutionTextBox.Text = user.EducationalInstitution;
                    SpecializationTextBox.Text = user.Specialty;
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пользователя: {ex.Message}");
            }
        }


        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (user == null)
                {
                    throw new Exception("Пользователь не найден");
                }

                if(userController.CheckNewUser(LoginTextBox.Text, PasswordBox.Password, user.RoleType, FirstNameTextBox.Text, LastNameTextBox.Text, MiddleNameTextBox.Text, EmailTextBox.Text,
                     BirthDatePicker.SelectedDate,  InstitutionTextBox.Text, EducationLevelComboBox.Text,  (int?)CourseComboBox.SelectedItem, SpecializationTextBox.Text, false))
                {
                    userController.UpdateUserData(userId, LoginTextBox.Text, PasswordBox.Password, user.RoleType, FirstNameTextBox.Text, LastNameTextBox.Text, MiddleNameTextBox.Text, EmailTextBox.Text,
                     BirthDatePicker.SelectedDate,  InstitutionTextBox.Text, EducationLevelComboBox.Text, (int?)CourseComboBox.SelectedItem, SpecializationTextBox.Text);
                }
                   



                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных пользователя: {ex.Message}");
            }
        }
    }
}
