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
                Debug.WriteLine(EducationLevelComboBox.SelectedItem.ToString());

                user.Login = LoginTextBox.Text;
                    user.PasswordHash = PasswordBox.Password;
                    user.FirstName = FirstNameTextBox.Text;
                    user.LastName = LastNameTextBox.Text;
                    user.Patronymic = MiddleNameTextBox.Text;
                    user.Email = EmailTextBox.Text;
                    user.DateOfBirth = BirthDatePicker.SelectedDate;
                    //user.EducationLevel = (string)EducationLevelComboBox.SelectedItem;
                    user.EducationalInstitution = InstitutionTextBox.Text;
                    user.CourseNumber = CourseComboBox.SelectedIndex;
                    user.Specialty = SpecializationTextBox.Text;
                    db.context.SaveChanges();
                    MessageBox.Show("Данные пользователя успешно обновлены.");
                



                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных пользователя: {ex.Message}");
            }
        }
    }
}
